using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace WaterPoloClock.Models
{
    public class Game
    {
        #region Team Details

        //default team settings
        public Team Team1 { get; } = new Team() { Name = "Team 1", BackgroundColor = "#1C5D99", ForegroundColor = "#000000" };
        public Team Team2 { get; } = new Team() { Name = "Team 2", BackgroundColor = "#902D41", ForegroundColor = "#000000" };

        public class Team
        {
            public string Name { get; set; }
            public string ForegroundColor { get; set; }
            public string BackgroundColor { get; set; }

        }

        #endregion

        #region Game State
        public CurrentGameState GameState { get; private set; }
        
        public class CurrentGameState //current game state... immutable
        {
            public PausableTime TimeOfNextTurnover { get; } //e.g., figure out how many seconds are left on the clock...
            public PausableTime TimeOfQuarterEnd { get; }

            public TimerState TimerState { get; }

            public Team TeamInPossession { get; }
            private Team OtherTeam { get; }

            public int Team1Score { get; }
            public int Team2Score { get; }

            public int QuarterNumber { get; }

            public CurrentGameState(PausableTime timeOfNextTurnOver, PausableTime timeOfQuarterEnd, TimerState timerState, Team teamInPossession, Team otherTeam, int team1Score, int team2Score, int quarterNumber)
            {
                TimeOfNextTurnover = timeOfNextTurnOver;
                TimeOfQuarterEnd = timeOfQuarterEnd;
                TimerState = timerState;
                TeamInPossession = teamInPossession;
                OtherTeam = otherTeam;
                Team1Score = team1Score;
                Team2Score = team2Score;
                QuarterNumber = quarterNumber;
            }

            //game start
            public CurrentGameState(Team team1, Team team2, int secondsInQuarter) : 
                this(new PausableTime(30), new PausableTime(secondsInQuarter), TimerState.Paused, team1, team2, 0, 0, 1)
            {
            }

            //state change functions
            public CurrentGameState TurnOver(TimerState timerState)
            {
                var secsTillNextTurnOver = Math.Min(TimeOfQuarterEnd.SecondsRemaining, 30);

                if (timerState == TimerState.Paused)
                {
                    return new CurrentGameState(
                        new PausableTime(secsTillNextTurnOver), new PausableTime(TimeOfQuarterEnd.SecondsRemaining), 
                        timerState,
                        teamInPossession: OtherTeam, otherTeam: TeamInPossession,
                        Team1Score, Team2Score, QuarterNumber);
                }
                else
                {
                    return new CurrentGameState(
                        new PausableTime(DateTime.UtcNow.AddSeconds(secsTillNextTurnOver)),
                        new PausableTime(DateTime.UtcNow.AddSeconds(TimeOfQuarterEnd.SecondsRemaining)),
                        timerState, 
                        teamInPossession: OtherTeam, otherTeam: TeamInPossession, 
                        Team1Score, Team2Score, QuarterNumber);
                }
            }

            public CurrentGameState PauseGame()
            {
                return new CurrentGameState(
                    new PausableTime(TimeOfNextTurnover.SecondsRemaining),
                    new PausableTime(TimeOfQuarterEnd.SecondsRemaining),
                    TimerState.Paused,
                    TeamInPossession, OtherTeam, Team1Score, Team2Score, QuarterNumber);
            }

            public CurrentGameState ResumeGame()
            {
                var secsTillNextTurnOver = Math.Min(TimeOfQuarterEnd.SecondsRemaining, 30);

                return new CurrentGameState(
                    new PausableTime(DateTime.UtcNow.AddSeconds(secsTillNextTurnOver)),
                    new PausableTime(DateTime.UtcNow.AddSeconds(TimeOfQuarterEnd.SecondsRemaining)),
                    TimerState.InPlay,
                    TeamInPossession, OtherTeam, Team1Score, Team2Score, QuarterNumber);
            }

            public class PausableTime
            {
                public DateTime? CurrentTargetTime { get; }
                public int? PausedSeconds { get; }

                public int SecondsRemaining => PausedSeconds ?? (int)(CurrentTargetTime - DateTime.UtcNow).Value.TotalSeconds;

                public PausableTime(DateTime targetTime)
                {
                    CurrentTargetTime = targetTime;
                }

                public PausableTime(int pausedSec)
                {
                    PausedSeconds = pausedSec;
                }
            }

        }

        public enum TimerState
        {
            Paused = 0,
            InPlay = 1
        }

        #endregion

        public string Name { get; } 
        public int SecondsInQuarter { get; }
        public DateTime CreationTime { get; } //for cleanup purposes
        private Timer EventTimer { get; set; }

        private void SetAutoTurnoverTimer()
        {
            EventTimer?.Stop(); // just in case
            EventTimer = new Timer(GameState.TimeOfNextTurnover.SecondsRemaining * 1000);
            EventTimer.Elapsed += (o, e) => AutoTurnOver();
            EventTimer.Start();
        }

        private void PauseAutoTurnoverTimer()
        {
            EventTimer?.Stop();
            EventTimer = null;
        }

        public Game(string name, int secondsInQuarter)
        {
            Name = name;
            SecondsInQuarter = secondsInQuarter;
            CreationTime = DateTime.UtcNow;
            GameState = new CurrentGameState(Team1, Team2, SecondsInQuarter);
        }

        public CurrentGameState Resume()
        {
            GameState = GameState.ResumeGame();
            SetAutoTurnoverTimer();
            return GameState;
        }

        public CurrentGameState AutoTurnOver()
        {
            if (GameState.TimeOfQuarterEnd.SecondsRemaining > 0)
            {
                GameState = GameState.TurnOver(TimerState.InPlay);
                SetAutoTurnoverTimer();
            }
            else
            {
                GameState = GameState.PauseGame();
                PauseAutoTurnoverTimer();
            }
            return GameState;
            
        }
    }
}
