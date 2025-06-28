namespace EsportsManager.BL.DTOs
{
    /// <summary>
    /// DTO cho kết quả đăng ký giải đấu
    /// </summary>
    public class TournamentRegistrationResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// Tạo kết quả thành công
        /// </summary>
        public static TournamentRegistrationResultDto CreateSuccess(string message = "Đăng ký giải đấu thành công!")
        {
            return new TournamentRegistrationResultDto
            {
                Success = true,
                Message = message
            };
        }

        /// <summary>
        /// Tạo kết quả thất bại
        /// </summary>
        public static TournamentRegistrationResultDto CreateFailure(string message, string errorCode = "")
        {
            return new TournamentRegistrationResultDto
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };
        }
    }
}
