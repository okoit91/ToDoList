using Base.Contracts.Domain;

namespace App.DAL.DTO;

public class TaskHistory : IDomainEntityId
{
    public Guid Id { get; set; }
    
    public Guid TaskId { get; set; }
    
    public Task? Task { get; set; }

    public string CurrentTitle { get; set; } = default!;
    
    public DateTime CompletedAt { get; set; }
    
    public DateTime? RevertedAt { get; set; }
}