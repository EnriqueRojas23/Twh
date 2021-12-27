using System;
using System.ComponentModel.DataAnnotations;
using CargaClic.Common;

public class GuiaDetalle : Entity
{
    [Key]
    public long id {get;set;}
    public int  Cantidad {get;set;}
    public string Lote {get;set;}
    public int EstadoId {get;set;}
    public string Referencia {get;set;}

    public int? ErrorGuiaId {get;set;}
    public long GuiaId {get;set;}

    public DateTime FechaRegistro {get;set;}
    public DateTime FechaExpire {get;set;}
    public Guid ProductoId{get;set;}
}
