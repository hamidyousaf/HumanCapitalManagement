using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            #region Entity configuration
            builder
                .HasKey(x => x.Id);
            builder
                .Property(x => x.Title)
                .HasMaxLength(1024)
                .IsRequired();
            builder
                .Property(x => x.Author)
                .HasMaxLength(512)
                .IsRequired();
            builder
                .Property(x => x.Description)
                .IsRequired(false);
            builder
                .Property(x => x.IsActive)
                .HasDefaultValue(true)
                .IsRequired();
            builder
                .Property(x => x.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();
            builder
             .Property(b => b.CreatedDate)
             .HasDefaultValueSql("GETUTCDATE()")
             .IsRequired();
            builder
                .Property(x => x.UpdatedDate)
                .IsRequired(false);
            #endregion
            #region Global query filter
            builder.HasQueryFilter(x => !x.IsDeleted && x.IsActive);
            #endregion
        }
    }
}
