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
    /// Tạo giải đấu mới
    /// </summary>
    Task<TournamentDto> CreateTournamentAsync(TournamentCreateDto createDto);

    /// <summary>
    /// Lấy danh sách tất cả giải đấu
    /// </summary>
    Task<List<TournamentDto>> GetAllTournamentsAsync();

    /// <summary>
    /// Lấy giải đấu đang diễn ra
    /// </summary>
    Task<List<TournamentDto>> GetActiveTournamentsAsync();

    /// <summary>
    /// Lấy chi tiết giải đấu theo ID
    /// </summary>
    Task<TournamentDto?> GetTournamentByIdAsync(int tournamentId);

    /// <summary>
    /// Cập nhật thông tin giải đấu
    /// </summary>
    Task<bool> UpdateTournamentAsync(int tournamentId, TournamentUpdateDto updateDto);

    /// <summary>
    /// Xóa giải đấu (chỉ Admin)
    /// </summary>
    Task<bool> DeleteTournamentAsync(int tournamentId);

    /// <summary>
    /// Tìm kiếm giải đấu theo tên
    /// </summary>
    Task<List<TournamentDto>> SearchTournamentsAsync(string searchTerm);

    /// <summary>
    /// Thay đổi trạng thái giải đấu
    /// </summary>
    Task<bool> ToggleTournamentStatusAsync(int tournamentId);
}
