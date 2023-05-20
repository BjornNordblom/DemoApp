using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DemoApp.Entities;

public class AuditTrace
{
    public long Id { get; set; }
    public string EntityName { get; set; } = null!;
    public string ActionType { get; set; } = null!;
    public string Username { get; set; } = null!;
    public DateTime TimeStamp { get; set; }
    public string EntityId { get; set; } = null!;

    public AuditTrace()
    {
        Changes = new Dictionary<string, object?>();
        TempProperties = new List<PropertyEntry>();
    }

    public static AuditTrace Create(
        string actionType,
        string entityId,
        string entityName,
        string username,
        DateTime timeStamp,
        Dictionary<string, object?> changes,
        List<PropertyEntry> tempProperties
    )
    {
        return new()
        {
            ActionType = actionType,
            EntityId = entityId,
            EntityName = entityName,
            Username = username,
            TimeStamp = timeStamp,
            Changes = changes,
            TempProperties = tempProperties
        };
    }

    public Dictionary<string, object?> Changes { get; set; }

    [NotMapped]
    // TempProperties are used for properties that are only generated on save, e.g. ID's
    public List<PropertyEntry> TempProperties { get; set; }
}
