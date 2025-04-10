using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Base.Contracts.Domain;

namespace App.DAL.DTO;

public class Task : IDomainEntityId
{
    public Guid Id { get; set; }
    
    public Guid ToDoListId { get; set; }
    
    [JsonIgnore]
    public ToDoList? ToDoList { get; set; }

    public string Title { get; set; } = default!;
    
    public string? Description { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public bool IsCompleted { get; set; }
    
    public bool IsArchived { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
}
