using Rooting.Models;

namespace Rooting.WebApi.Controllers
{
    public interface IGameStatistics
    {
        Guid GameId { get; }
        bool GameStarted { get; }
        int Generation { get; set; }
        IEnumerable<Player> Players { get; }
        DateTime TimeStarted { get; }

        PlayingCard[] CurrentInHand(FamilyTypes familyType);

        PlayingCard[] NotPlayedCards(FamilyTypes familyType);

        bool IsPlayerPlaying(FamilyTypes familyType);

        void PlayerIsPlaying(FamilyTypes familyType, bool playingStatus);

        void TakeCardInHand(FamilyTypes familyType, int cardId);
    }
}