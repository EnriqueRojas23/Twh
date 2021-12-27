using System;

namespace CargaClic.Repository.Contracts.Seguimiento
{
    public class CargaMasivaDetalleForRegister
    {
            public long id { get; set; }
            public string codigo { get; set; }
            public string pedido { get; set; }
            public string referencia { get; set; }
            public string razonsocial { get; set; }

            public string ruc {get;set;}
            public string direccion  {get;set;}
            public int? stock { get; set; }  
            public string codigodespacho {get;set;}
            public string distrito {get;set;}

            public string departamento {get;set;}

            public string contacto {get;set;}
            public string telefono {get;set;}

    }

    
}