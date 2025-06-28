using EsportsManager.BL.Interfaces;
using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.MenuHandlers
{
    public class FeedbackManagementHandler
    {
        public FeedbackManagementHandler(IUserService userService, ITournamentService tournamentService, IFeedbackService feedbackService)
        {
            // TODO: implement constructor
        }
        public Task ManageFeedbackAsync() => Task.CompletedTask;
    }
}
