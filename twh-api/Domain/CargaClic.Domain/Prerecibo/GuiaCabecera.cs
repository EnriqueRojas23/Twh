using System;
using System.ComponentModel.DataAnnotations;
using CargaClic.Common;

public class GuiaCabecera : Entity
{
    [Key]
    public long id {get;set;}
    public int  PropietarioID {get;set;}
    public int TipoMovimientoId {get;set;}
    public DateTime? FechaGuia {get;set;}
    public string NroGuia {get;set;}
    public DateTime FechaRegistro {get;set;}
    public int IdCarga {get;set;}
    public int IdEstado  {get;set;}
}
