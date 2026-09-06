namespace e_violenciagen.Models;
public abstract class BaseEntity
{
    public Guid Id{get;set;}=Guid.NewGuid();
     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
     public DateTime? UpdatedAd { get; set; }
     public string? CreatedBy {get;set;}
     //public bool IsActive { get; set; } = true;
     public bool Activo { get; set; } = true;

}