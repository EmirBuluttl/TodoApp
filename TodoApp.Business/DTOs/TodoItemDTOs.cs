namespace TodoApp.Business.DTOs;

public record TodoItemDto(int Id, string Title, string? Description, bool IsCompleted, DateTime CreatedAt);
public record CreateTodoItemDto(string Title, string? Description);
public record UpdateTodoItemDto(string Title, string? Description, bool IsCompleted);
