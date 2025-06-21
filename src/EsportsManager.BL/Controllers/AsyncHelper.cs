using System.Threading.Tasks;

namespace EsportsManager.BL.Controllers;

/// <summary>
/// Helper methods cho Controllers để tránh async warnings
/// </summary>
internal static class AsyncHelper
{
    /// <summary>
    /// Helper method để tránh warning khi mock async methods
    /// </summary>
    public static async Task<T> MockAsync<T>(T result)
    {
        await Task.CompletedTask;
        return result;
    }
    
    /// <summary>
    /// Helper method để tránh warning khi mock async bool methods
    /// </summary>
    public static async Task<bool> MockSuccessAsync()
    {
        await Task.CompletedTask;
        return true;
    }
}
