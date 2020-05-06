using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WaterPoloClock.Models;

namespace WaterPoloClock.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        static Dictionary<string, Game> Games = new Dictionary<string, Game>();

        [HttpGet]
        public Dictionary<string, Game> Get()
        {
            return Games;
        }

        [HttpGet("new")]
        public ActionResult<Game> New([FromQuery]string name, [FromQuery]int secondsInQuarter)
        {
            //do cleanup now to make sure we don't get crazy numbers of games...
            for(var i = Games.Count - 1; i >= 0; i--)
            {
                var gameKey = Games.Keys.ElementAt(i);
                if (Games[gameKey].CreationTime < DateTime.UtcNow.AddHours(-3)) //no game goes longer than 3hrs...
                {
                    Games.Remove(gameKey);
                }
            }

            if (Games.ContainsKey(name))
            {
                return new ConflictResult();
            }
            else
            {
                var game = new Game(name, secondsInQuarter);
                Games.Add(name, game);
                return Ok(game);
            }
        }

        [HttpGet("{name}")]
        public ActionResult<Game.CurrentGameState> GetState(string name)
        {
            if (Games.ContainsKey(name))
            {
                return Games[name].GameState;
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [HttpGet("resume/{name}")]
        public ActionResult<Game.CurrentGameState> Resume(string name)
        {
            if (Games.ContainsKey(name))
            {
                return Games[name].Resume();
            }
            else
            {
                return new NotFoundResult();
            }
        }
    }
}