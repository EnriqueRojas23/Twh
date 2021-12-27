using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class GuiaDetalleConfiguration : IEntityTypeConfiguration<GuiaDetalle>
{
    public void Configure(EntityTypeBuilder<GuiaDetalle> builder)
    {
            builder.ToTable("GuiaDetalle","Recepcion");
            builder.HasKey(x=>x.id);
           
    }
}


