using Microsoft.AspNetCore.Mvc;

namespace Rooting.WebApi.Controllers
{
    public abstract class MarmaladeController : ControllerBase
    {
        protected readonly GameStatistics gameStatistics;

        public MarmaladeController(GameStatistics gameStatistics)
        {
            this.gameStatistics = gameStatistics;
        }
    }
}