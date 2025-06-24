using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;

namespace EsportsManager.BL.Interfaces;

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
}
