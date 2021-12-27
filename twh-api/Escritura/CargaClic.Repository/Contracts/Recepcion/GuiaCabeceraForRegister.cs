using System;

namespace CargaClic.API.Dtos.Recepcion
{

    public class GuiaCabeceraForUpdate
    {
        public long id {get;set;}
        public int TipoMovimientoId {get;set;}
        public DateTime FechaGuia {get;set;}
        public string NroGuia {get;set;}
        public int Cantidad  {get;set;}
        public int IdUsuarioRegistro {get;set;}
        public int PropietarioID {get;set;}
    }
    public class GuiaCabeceraForRegister
    {
        
        public int TipoMovimientoId {get;set;}
        public DateTime FechaGuía {get;set;}
        public string NroGuia {get;set;}
        public int Cantidad  {get;set;}
        public int IdUsuarioRegistro {get;set;}
        public int PropietarioID {get;set;}
        public int idcarga {get;set;}
        public int idestado {get;set;}
    }
    public class GuiaDetalleForRegister
    {
        
        
        public Guid productoid {get;set;}
        public int estadoid {get;set;}
        public string referencia {get;set;}
        public string lote {get;set;}
        public DateTime FechaExpire {get;set;}
        public long GuiaId {get;set;}
        public int cantidad {get;set;}


      
    }
    public class GuiaCabeceraDto
    {
        public long id {get;set;}
        public string Cliente {get;set;}
        public DateTime fecharegistro {get;set;}
        public DateTime FechaGuia {get;set;}
        public String NroGuia {get;set;}
        public string TipoMovimiento {get;set;}
        public string estado  {get;set;}

    }

     public class GuiaDetalleDto
    {
        public long id {get;set;}
        public string codigo {get;set;}
        public string producto {get;set;}
        public string lote {get;set;}
        public string referencia {get;set;}
        public int cantidad {get;set;}


        public DateTime fecharegistro {get;set;}
        public DateTime FechaGuia {get;set;}
        public DateTime FechaExpire {get;set;}
        public String NroGuia {get;set;}
        public string TipoMovimiento {get;set;}


    }
}