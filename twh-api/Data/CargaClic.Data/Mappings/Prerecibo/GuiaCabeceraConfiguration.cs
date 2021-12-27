using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class GuiaCabeceraConfiguration : IEntityTypeConfiguration<GuiaCabecera>
{
    public void Configure(EntityTypeBuilder<GuiaCabecera> builder)
    {
            builder.ToTable("GuiaCabecera","Recepcion");
            builder.HasKey(x=>x.id);
           
    }
}


