using System;

namespace Rooting.Models.ResponseModels
{
    public enum GamePlayState
    {
        Unknown = 0,

        /// <summary>
        /// Waiting for all player to be claimed
        /// </summary>
        WaitingForPlayers,

        /// <summary>
        /// All players have been claimed, the game can start
        /// </summary>
        AllFamiliesRegistered,

        /// <summary>
        /// Game is waiting for first cycle.
        /// </summary>
        Starting,

        /// <summary>
        /// The game engine is between turns and is waiting for the timer tick.
        /// </summary>
        GameWaitingForEndOfTurn,

        /// <summary>
        /// Game is running the calculation for next move
        /// </summary>
        GameCalculation,

        /// <summary>
        /// The game is paused.
        /// </summary>
        GamePaused,

        /// <summary>
        /// The game is stopped before a winner was determined.
        /// </summary>
        GameStopped,

        /// <summary>
        /// No more available moves. the game is finished.
        /// </summary>
        GameFinished,

        /// <summary>
        /// Issues at the server.
        /// </summary>
        ServerTimeou
    }
}