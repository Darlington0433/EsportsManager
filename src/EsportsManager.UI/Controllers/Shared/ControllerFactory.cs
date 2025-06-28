using System;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.Controllers.Admin;
using EsportsManager.UI.Controllers.Admin.Handlers;
using EsportsManager.UI.Controllers.Base;
using EsportsManager.UI.Controllers.Interfaces;
using EsportsManager.UI.Controllers.Player;
using EsportsManager.UI.Controllers.Player.Handlers;
using EsportsManager.UI.Controllers.Shared.Handlers;
using EsportsManager.UI.Controllers.Viewer;
using EsportsManager.UI.Controllers.Viewer.Handlers;

namespace EsportsManager.UI.Controllers.Shared
{
    /// <summary>
    /// Controller Factory - Enterprise pattern for controller creation
    /// Áp dụng Factory Pattern, Dependency Injection và Single Responsibility
    /// </summary>
    public class ControllerFactory : IControllerFactory
    {
        private readonly IUserService _userService;
        private readonly ITournamentService _tournamentService;
        private readonly ITeamService _teamService;
        private readonly IWalletService _walletService;
        private readonly IVotingService _votingService;
        private readonly IFeedbackService _feedbackService;
        private readonly ISystemSettingsService _systemSettingsService;
        private readonly IAchievementService _achievementService;
        private readonly Services.SystemIntegrityService _systemIntegrityService;

        public ControllerFactory(
            IUserService userService,
            ITournamentService tournamentService,
            ITeamService teamService,
            IVotingService votingService,
            IWalletService walletService,
            IFeedbackService feedbackService,
            ISystemSettingsService systemSettingsService,
            IAchievementService achievementService,
            Services.SystemIntegrityService systemIntegrityService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tournamentService = tournamentService ?? throw new ArgumentNullException(nameof(tournamentService));
            _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
            _votingService = votingService ?? throw new ArgumentNullException(nameof(votingService));
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
            _systemSettingsService = systemSettingsService ?? throw new ArgumentNullException(nameof(systemSettingsService));
            _achievementService = achievementService ?? throw new ArgumentNullException(nameof(achievementService));
            _systemIntegrityService = systemIntegrityService ?? throw new ArgumentNullException(nameof(systemIntegrityService));
        }

        /// <summary>
        /// Factory method - creates appropriate controller based on user role
        /// </summary>
        public IController CreateController(UserProfileDto user)
        {
            return user.Role.ToLower() switch
            {
                "admin" => CreateAdminController(user),
                "player" => CreatePlayerController(user),
                "viewer" => CreateViewerController(user),
                _ => throw new ArgumentException($"Unsupported user role: {user.Role}")
            };
        }

        /// <summary>
        /// Creates PlayerController with all dependencies
        /// </summary>
        public PlayerController CreatePlayerController(UserProfileDto user)
        {
            // Create handlers with dependency injection
            var tournamentManagementHandler = new TournamentManagementHandler(user, _tournamentService, _teamService);
            var teamManagementHandler = new PlayerTeamManagementHandler(user, _teamService);
            var profileHandler = new PlayerProfileHandler(user, _userService);
            var tournamentViewHandler = new TournamentViewHandler(_tournamentService);
            var feedbackHandler = new PlayerFeedbackHandler(user, _tournamentService);
            var walletHandler = new PlayerWalletHandler(user, _walletService);
            var achievementHandler = new PlayerAchievementHandler(user, _tournamentService, _userService, _achievementService);

            return new PlayerController(
                user,
                tournamentManagementHandler,
                teamManagementHandler,
                profileHandler,
                tournamentViewHandler,
                feedbackHandler,
                walletHandler,
                achievementHandler);
        }

        /// <summary>
        /// Creates ViewerController with all dependencies
        /// </summary>
        public ViewerController CreateViewerController(UserProfileDto user)
        {
            var tournamentHandler = new ViewerTournamentHandler(_tournamentService);
            var votingHandler = new ViewerVotingHandler(user, _tournamentService, _userService, _votingService);
            var donationHandler = new ViewerDonationHandler(user, _walletService, _userService);
            var profileHandler = new ViewerProfileHandler(user, _userService);
            var walletHandler = new ViewerWalletHandler(user, _walletService);

            return new ViewerController(
                user,
                tournamentHandler,
                votingHandler,
                donationHandler,
                profileHandler,
                walletHandler);
        }

        /// <summary>
        /// Creates AdminController with all dependencies
        /// </summary>
        public AdminUIController CreateAdminController(UserProfileDto user)
        {
            var userManagementHandler = new UserManagementHandler(_userService, _achievementService, _tournamentService);
            var tournamentManagementHandler = new AdminTournamentManagementHandler(_tournamentService);
            var systemStatsHandler = new SystemStatsHandler(_userService, _tournamentService, _teamService);
            var donationReportHandler = new DonationReportHandler(_walletService, _userService);
            var votingResultsHandler = new VotingResultsHandler(_userService, _tournamentService, _votingService);
            var feedbackManagementHandler = new FeedbackManagementHandler(_userService, _tournamentService, _feedbackService);

            return new AdminUIController(
                user,
                userManagementHandler,
                tournamentManagementHandler,
                systemStatsHandler,
                donationReportHandler,
                votingResultsHandler,
                feedbackManagementHandler);
        }
    }

    /// <summary>
    /// Interface for Controller Factory
    /// </summary>
    public interface IControllerFactory
    {
        IController CreateController(UserProfileDto user);
        PlayerController CreatePlayerController(UserProfileDto user);
        ViewerController CreateViewerController(UserProfileDto user);
        AdminUIController CreateAdminController(UserProfileDto user);
    }
}
