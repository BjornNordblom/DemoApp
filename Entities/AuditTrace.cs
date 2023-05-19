using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DemoApp.Entities;

public class AuditTrace
{
    public long Id { get; set; }
    public string EntityName { get; set; }
    public string ActionType { get; set; }
    public string Username { get; set; }
    public DateTime TimeStamp { get; set; }
    public string EntityId { get; set; }

    public Dictionary<string, object> Changes { get; set; }

    [NotMapped]
    // TempProperties are used for properties that are only generated on save, e.g. ID's
    public List<PropertyEntry> TempProperties { get; set; }
}
