namespace TodoApp.Entities;

public class TodoItem : BaseEntity
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    
    // Foreign Key
    public int UserId { get; set; }
    
    // Navigation property
    public User User { get; set; } = null!;
}
