using Microsoft.EntityFrameworkCore;
using TodoApp.Business.DTOs;
using TodoApp.Core.Entities;
using TodoApp.Core.Repositories;

namespace TodoApp.Business.Services;

public class TodoService : ITodoService
{
    private readonly IGenericRepository<TodoItem> _todoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TodoService(IGenericRepository<TodoItem> todoRepository, IUnitOfWork unitOfWork)
    {
        _todoRepository = todoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TodoItemDto>> GetAllForUserAsync(int userId)
    {
        var items = await _todoRepository
            .Where(t => t.UserId == userId)
            .ToListAsync();

        return items.Select(t => new TodoItemDto(t.Id, t.Title, t.Description, t.IsCompleted, t.CreatedAt));
    }

    public async Task<TodoItemDto> GetByIdForUserAsync(int id, int userId)
    {
        var item = await _todoRepository.GetByIdAsync(id);
        if (item == null || item.UserId != userId)
            throw new Exception("Item not found or access denied.");

        return new TodoItemDto(item.Id, item.Title, item.Description, item.IsCompleted, item.CreatedAt);
    }

    public async Task<TodoItemDto> CreateAsync(CreateTodoItemDto dto, int userId)
    {
        var item = new TodoItem
        {
            Title = dto.Title,
            Description = dto.Description,
            UserId = userId,
            IsCompleted = false
        };

        await _todoRepository.AddAsync(item);
        await _unitOfWork.CommitAsync();

        return new TodoItemDto(item.Id, item.Title, item.Description, item.IsCompleted, item.CreatedAt);
    }

    public async Task UpdateAsync(int id, UpdateTodoItemDto dto, int userId)
    {
        var item = await _todoRepository.GetByIdAsync(id);
        if (item == null || item.UserId != userId)
            throw new Exception("Item not found or access denied.");

        item.Title = dto.Title;
        item.Description = dto.Description;
        item.IsCompleted = dto.IsCompleted;

        _todoRepository.Update(item);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var item = await _todoRepository.GetByIdAsync(id);
        if (item == null || item.UserId != userId)
            throw new Exception("Item not found or access denied.");

        _todoRepository.Remove(item);
        await _unitOfWork.CommitAsync();
    }
}
