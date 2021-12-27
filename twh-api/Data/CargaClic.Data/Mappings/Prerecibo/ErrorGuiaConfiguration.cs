using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ErrorGuiaConfiguration : IEntityTypeConfiguration<ErrorGuia>
{
    public void Configure(EntityTypeBuilder<ErrorGuia> builder)
    {
            builder.ToTable("ErrorGuia","Mantenimiento");
            builder.HasKey(x=>x.id);
           
    }
}


