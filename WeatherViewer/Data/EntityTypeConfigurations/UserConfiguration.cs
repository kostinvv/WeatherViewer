using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherViewer.Models;

namespace WeatherViewer.Data.EntityTypeConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users").HasKey(user => user.UserId);
        builder.HasIndex(user => user.UserId).IsUnique();
        
        builder.Property(property => property.UserId)
            .HasColumnName("user_id");
        
        builder.Property(property => property.Login)
            .HasColumnName("login");
        
        builder.Property(property => property.Email)
            .HasColumnName("email");
        
        builder.Property(property => property.Password)
            .HasColumnName("password");
    }
}