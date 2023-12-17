using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherViewer.Models;

namespace WeatherViewer.Data.EntityTypeConfigurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions").HasKey(session => session.SessionId);
        builder.HasIndex(session => session.SessionId).IsUnique();
        
        builder.Property(property => property.SessionId)
            .HasColumnName("session_id");
        
        builder.Property(property => property.UserId)
            .HasColumnName("user_id");
        
        builder.Property(property => property.ExpiresAt)
            .HasColumnName("expires_at");
    }
}