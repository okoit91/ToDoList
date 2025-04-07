using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Contracts.Domain;

namespace App.DAL.DTO;

public class ToDoList : IDomainEntityId
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