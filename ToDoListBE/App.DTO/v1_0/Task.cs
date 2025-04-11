using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace App.DTO.v1_0;

public class Task
{
    public Guid Id { get; set; }
    
    public Guid ToDoListId { get; set; }
    
    [JsonIgnore]
    public ToDoList? ToDoList { get; set; }

    [MaxLength(20)]
    public string Title { get; set; } = default!;
    
    [MaxLength(100)]
    public string? Description { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public bool IsCompleted { get; set; }
    
    public bool IsArchived { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
}