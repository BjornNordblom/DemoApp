using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace DemoApp.Entities;

// https://nwb.one/blog/auditing-dotnet-entity-framework-core
public class AuditTraceTypeConfiguration : IEntityTypeConfiguration<AuditTrace>
{
    public void Configure(EntityTypeBuilder<AuditTrace> builder)
    {
        builder.HasKey(p => p.Id);
        builder
            .Property(at => at.Changes)
            .HasConversion(
                changeObject => JsonConvert.SerializeObject(changeObject),
                changeValue =>
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(changeValue)
            );
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.Property(p => p.EntityName).IsRequired().HasMaxLength(50);
    }
}
