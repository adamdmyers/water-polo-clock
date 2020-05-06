import React from 'react';
import { Route, useParams } from 'react-router';
import { CurrentGameState } from '../types/GameState';
import useService from '../services/useService';
import { ServiceStatus } from '../types/Service';
import Box from '@material-ui/core/Box';
import { Typography } from '@material-ui/core';

export default function ShotClock() {

    const { gameName } = useParams();
    const currentGameState = useService<CurrentGameState>('/game/'+gameName);

    let backgroundColor, foregroundColor;
    if (currentGameState.status === ServiceStatus.loaded){
        backgroundColor = currentGameState.payload.teamInPossession.backgroundColor;
        foregroundColor = currentGameState.payload.teamInPossession.foregroundColor;
    }
    else {
        backgroundColor = "red";
        foregroundColor = "black";
    }

        return (
            <Box height='100vh' 
                bgcolor={backgroundColor} 
                color={foregroundColor}
                display='flex'
                justifyContent = 'center'
                alignItems = 'center'
                >
            {currentGameState.status === ServiceStatus.loaded && (
                <Box>
                    <Typography align='center'>{currentGameState.payload.teamInPossession.name}</Typography>
                    <Typography align='center'>{currentGameState.payload.timeOfNextTurnover.secondsRemaining}</Typography>
                </Box>                    
            )}
            {currentGameState.status === ServiceStatus.error && (
                <p>{currentGameState.error.message}</p>
            )}
            </Box>);
}