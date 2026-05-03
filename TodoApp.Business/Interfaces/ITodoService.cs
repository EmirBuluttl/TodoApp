using TodoApp.Business.DTOs;

namespace TodoApp.Business.Interfaces;

public interface ITodoService
{
    Task<IEnumerable<TodoItemDto>> GetAllForUserAsync(int userId);
    Task<TodoItemDto> GetByIdForUserAsync(int id, int userId);
    Task<TodoItemDto> CreateAsync(CreateTodoItemDto dto, int userId);
    Task UpdateAsync(int id, UpdateTodoItemDto dto, int userId);
    Task DeleteAsync(int id, int userId);
}
