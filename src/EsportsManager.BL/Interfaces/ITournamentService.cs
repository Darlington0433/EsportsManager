using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;

namespace EsportsManager.BL.Interfaces
{

    /// <summary>
    /// Tournament Service Interface - Quản lý các thao tác liên quan đến giải đấu
    /// Áp dụng Interface Segregation Principle
    /// </summary>
    public interface ITournamentService
    {
        /// <summary>
        /// Lấy danh sách tất cả giải đấu
        /// </summary>
        Task<List<TournamentInfoDto>> GetAllTournamentsAsync();

        /// <summary>
        /// Lấy danh sách giải đấu có thể đăng ký tham gia
        /// </summary>
        Task<List<TournamentInfoDto>> GetAvailableTournamentsAsync();

        /// <summary>
        /// Lấy thông tin giải đấu theo ID
        /// </summary>
        Task<TournamentInfoDto?> GetTournamentByIdAsync(int tournamentId);

        /// <summary>
        /// Lấy danh sách giải đấu mà team đã đăng ký
        /// </summary>
        Task<List<TournamentInfoDto>> GetTeamTournamentsAsync(int teamId);

        /// <summary>
        /// Đăng ký team tham gia giải đấu
        /// </summary>
        Task<bool> RegisterTeamForTournamentAsync(int tournamentId, int teamId);

        /// <summary>
        /// Hủy đăng ký tham gia giải đấu
        /// </summary>
        Task<bool> UnregisterTeamFromTournamentAsync(int tournamentId, int teamId);

        /// <summary>
        /// Tạo giải đấu mới (Admin function)
        /// </summary>
        Task<TournamentInfoDto> CreateTournamentAsync(TournamentCreateDto tournamentDto);

        /// <summary>
        /// Cập nhật thông tin giải đấu (Admin function)
        /// </summary>
        Task<bool> UpdateTournamentAsync(int tournamentId, TournamentUpdateDto tournamentDto);

        /// <summary>
        /// Xóa giải đấu (Admin function)
        /// </summary>
        Task<bool> DeleteTournamentAsync(int tournamentId);

        /// <summary>
        /// Lấy danh sách team tham gia giải đấu
        /// </summary>
        Task<List<TeamInfoDto>> GetTournamentTeamsAsync(int tournamentId);

        /// <summary>
        /// Gửi feedback về giải đấu
        /// </summary>
        Task<bool> SubmitFeedbackAsync(int userId, FeedbackDto feedbackDto);

        /// <summary>
        /// Lấy danh sách feedback của giải đấu
        /// </summary>
        Task<List<FeedbackDto>> GetTournamentFeedbackAsync(int tournamentId);

        /// <summary>
        /// Lấy thống kê tournament dựa trên game
        /// </summary>
        Task<List<TournamentStatDto>> GetTournamentStatsByGameAsync(int gameId);

        /// <summary>
        /// Lấy kết quả chi tiết của tournament
        /// </summary>
        Task<List<TournamentResultDto>> GetTournamentResultsAsync(int tournamentId);

        /// <summary>
        /// Thêm kết quả mới cho tournament
        /// </summary>
        Task<bool> AddTournamentResultAsync(int tournamentId, int teamId, int position, decimal prizeMoney, string notes);    /// <summary>
                                                                                                                              /// Lấy bảng xếp hạng đầy đủ của tournament
                                                                                                                              /// </summary>
        Task<List<TournamentLeaderboardDto>> GetTournamentLeaderboardAsync(int tournamentId);

        /// <summary>
        /// Lấy danh sách đăng ký tham gia tournament đang chờ duyệt (Admin function)
        /// </summary>
        Task<List<TournamentRegistrationDto>> GetPendingRegistrationsAsync();

        /// <summary>
        /// Lấy danh sách đăng ký tham gia tournament theo trạng thái (Admin function)
        /// </summary>
        Task<List<TournamentRegistrationDto>> GetRegistrationsByStatusAsync(string status);

        /// <summary>
        /// Phê duyệt đăng ký tham gia tournament (Admin function)
        /// </summary>
        Task<bool> ApproveRegistrationAsync(int registrationId);

        /// <summary>
        /// Từ chối đăng ký tham gia tournament (Admin function)
        /// </summary>
        Task<bool> RejectRegistrationAsync(int registrationId);

        /// <summary>
        /// Lấy thông tin chi tiết đăng ký theo ID (Admin function)
        /// </summary>
        Task<TournamentRegistrationDto?> GetRegistrationByIdAsync(int registrationId);
    }
}
