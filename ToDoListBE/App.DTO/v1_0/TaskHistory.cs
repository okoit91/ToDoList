namespace App.DTO.v1_0;

public class TaskHistory
{
    public Guid Id { get; set; }
    
    public Guid TaskId { get; set; }
    
    public Task? Task { get; set; }

    public string CurrentTitle { get; set; } = default!;
    
    public DateTime CompletedAt { get; set; }
    
    public DateTime? RevertedAt { get; set; }
}