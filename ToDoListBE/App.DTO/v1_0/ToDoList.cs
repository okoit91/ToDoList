using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.DTO.v1_0;

public class ToDoList
{
    public Guid Id { get; set; }
    
    [MaxLength(20)]
    public string Name { get; set; } = default!;
    
    public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
    
    public Guid? ParentListId { get; set; }
    
    [ForeignKey(nameof(ParentListId))]
    public ToDoList? ParentList { get; set; }
    
    public ICollection<ToDoList>? SubLists { get; set; }
}