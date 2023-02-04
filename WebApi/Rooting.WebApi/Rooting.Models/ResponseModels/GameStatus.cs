namespace Rooting.Models.ResponseModels
{
    public enum GameStatus
    {
        Unknown = 0,

        /// <summary>
        /// Waiting for all player to be claimed
        /// </summary>
        WaitingForPLayers = 1,

        /// <summary>
        /// All players have been claimed, the game can start
        /// </summary>
        GameCanStart = 2,

        /// <summary>
        /// The game engine is between turns and is waiting for the timer tick.
        /// </summary>
        GameWaitingForEndOfTurn = 3,

        /// <summary>
        /// Game is running the calculation for next move
        /// </summary>
        GameCalculation = 4,

        /// <summary>
        /// The game is paused.
        /// </summary>
        GamePaused = 5,

        /// <summary>
        /// The game is stopped before a winner was determined.
        /// </summary>
        GameStopped = 6,

        /// <summary>
        /// No more available moves. the game is finished.
        /// </summary>
        GameFinished = 7,

        /// <summary>
        /// Issues at the server.
        /// </summary>
        ServerTimeout = 99
    }
}