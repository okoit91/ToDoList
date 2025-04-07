namespace App.DTO.v1_0;

public class Task
{
    public Guid Id { get; set; }
    
    public Guid ToDoListId { get; set; }
    
    public ToDoList? ToDoList { get; set; }

    public string Title { get; set; } = default!;
    
    public string? Description { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public bool IsCompleted { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
}