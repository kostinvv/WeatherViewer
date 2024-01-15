using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherViewer.Models;

namespace WeatherViewer.Data.EntityTypeConfigurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations").HasKey(location => location.LocationId);
        builder.HasIndex(location => location.LocationId).IsUnique();
        
        builder.HasIndex(location => new { location.UserId, location.LocationId });
        
        builder.Property(property => property.LocationId)
            .HasColumnName("location_id");
        
        builder.Property(property => property.Name)
            .HasColumnName("location_name");
        
        builder.Property(property => property.UserId)
            .HasColumnName("user_id");
        
        builder.Property(property => property.Longitude)
            .HasColumnName("lon");
        
        builder.Property(property => property.Latitude)
            .HasColumnName("lat");
    }
}