namespace TodoApp.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    // Navigation property
    public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
}
