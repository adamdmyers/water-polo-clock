interface Team {
    name: string,
    foregroundColor: string,
    backgroundColor: string
}

export enum TimerState{
    paused = 0,
    inPlay = 1
}

interface PausableTime{
    currentTargetTime?: Date,
    pausedSeconds?: number
    secondsRemaining: number
}

export interface CurrentGameState{
    timeOfNextTurnover: PausableTime,
    timeOfQuarterEnd: PausableTime,
    timerState: TimerState,
    teamInPossession: Team,
    team1Score: number,
    team2Score: number,
    quarterNumber: number
}