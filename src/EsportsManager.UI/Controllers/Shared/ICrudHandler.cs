using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.MenuHandlers.Interfaces.Shared
{
    /// <summary>
    /// Interface for handlers that support CRUD operations
    /// Follows Command Query Responsibility Segregation (CQRS) pattern
    /// </summary>
    public interface ICrudHandler<TEntity, TDto> : IBaseHandler
    {
        Task HandleCreateAsync(TDto dto);
        Task HandleReadAsync(int id);
        Task HandleUpdateAsync(int id, TDto dto);
        Task HandleDeleteAsync(int id);
        Task HandleListAsync();
        Task HandleSearchAsync(string searchTerm);
    }
}
