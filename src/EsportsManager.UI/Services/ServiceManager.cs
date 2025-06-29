using System;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.Controllers.Admin;
using EsportsManager.UI.Controllers.Admin.Handlers;
using EsportsManager.UI.Controllers.Interfaces;
using EsportsManager.UI.Controllers.Player;
using EsportsManager.UI.Controllers.Player.Handlers;
using EsportsManager.UI.Controllers.Shared;
using EsportsManager.UI.Controllers.Shared.Handlers;
using EsportsManager.UI.Controllers.Viewer;
using EsportsManager.UI.Controllers.Viewer.Handlers;

namespace EsportsManager.UI.Services;

/// <summary>
/// ServiceManager - Tích hợp UI và BL layers
/// Đảm bảo UI có thể truy cập vào business logic một cách clean
/// </summary>
public class ServiceManager
{
    private readonly IUserService _userService;
    private readonly ITournamentService _tournamentService;
    private readonly ITeamService _teamService;
    private readonly IWalletService _walletService;
    private readonly IVotingService _votingService;
    private readonly IFeedbackService _feedbackService;
    private readonly IAchievementService _achievementService;

    public ServiceManager(
        IUserService userService,
        ITournamentService tournamentService,
        ITeamService teamService,
        IWalletService walletService,
        IVotingService votingService,
        IFeedbackService feedbackService,
        IAchievementService achievementService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _tournamentService = tournamentService ?? throw new ArgumentNullException(nameof(tournamentService));
        _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
        _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
        _votingService = votingService ?? throw new ArgumentNullException(nameof(votingService));
        _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
        _achievementService = achievementService ?? throw new ArgumentNullException(nameof(achievementService));
    }

    /// <summary>
    /// Tạo AdminUIController với handlers và trả về để gọi ShowAdminMenu() trực tiếp
    /// </summary>
    public AdminUIController CreateAdminController(UserProfileDto adminUser)
    {
        // Create handlers
        var userManagementHandler = new UserManagementHandler(_userService, _achievementService, _tournamentService);
        var tournamentManagementHandler = new EsportsManager.UI.Controllers.Admin.Handlers.TournamentManagementHandler(adminUser, _tournamentService, _teamService);
        var systemStatsHandler = new SystemStatsHandler(_userService, _tournamentService, _teamService);
        var donationReportHandler = new DonationReportHandler(_walletService, _userService);
        var votingResultsHandler = new VotingResultsHandler(_userService, _tournamentService, _votingService);
        var feedbackManagementHandler = new FeedbackManagementHandler(_userService, _tournamentService, _feedbackService);

        return new AdminUIController(
            adminUser,
            userManagementHandler,
            tournamentManagementHandler,
            systemStatsHandler,
            donationReportHandler,
            votingResultsHandler,
            feedbackManagementHandler);
    }

    /// <summary>
    /// Tạo PlayerController với handlers và trả về để gọi ShowPlayerMenu() trực tiếp
    /// </summary>
    public PlayerController CreatePlayerController(UserProfileDto playerUser)
    {
        // Tạo các handler instances với đúng thứ tự tham số
        var tournamentManagementHandler = new EsportsManager.UI.Controllers.Shared.Handlers.TournamentManagementHandler(playerUser, _tournamentService, _teamService);
        var teamManagementHandler = new PlayerTeamManagementHandler(playerUser, _teamService);
        var profileHandler = new PlayerProfileHandler(playerUser, _userService);
        var tournamentViewHandler = new TournamentViewHandler(_tournamentService);
        var feedbackHandler = new PlayerFeedbackHandler(playerUser, _tournamentService);
        var walletHandler = new PlayerWalletHandler(playerUser, _walletService);
        var achievementHandler = new PlayerAchievementHandler(playerUser, _tournamentService, _userService, _achievementService);

        return new PlayerController(playerUser, tournamentManagementHandler, teamManagementHandler, profileHandler, tournamentViewHandler, feedbackHandler, walletHandler, achievementHandler);
    }

    /// <summary>
    /// Tạo ViewerController với handlers và trả về để gọi ShowViewerMenu() trực tiếp
    /// </summary>
    public ViewerController CreateViewerController(UserProfileDto viewerUser)
    {
        // Tạo các handler instances cho Viewer
        var tournamentHandler = new ViewerTournamentHandler(_tournamentService);
        var votingHandler = new ViewerVotingHandler(viewerUser, _tournamentService, _userService, _votingService);
        var donationHandler = new ViewerDonationHandler(viewerUser, _walletService, _userService);
        var profileHandler = new ViewerProfileHandler(viewerUser, _userService);
        var walletHandler = new ViewerWalletHandler(viewerUser, _walletService);

        return new ViewerController(viewerUser, tournamentHandler, votingHandler, donationHandler, profileHandler, walletHandler);
    }
}
