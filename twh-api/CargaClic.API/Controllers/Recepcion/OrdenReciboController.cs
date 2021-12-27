using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using CargaClic.API.Dtos.Recepcion;
using CargaClic.Common;
using CargaClic.Contracts.Parameters.Mantenimiento;
using CargaClic.Contracts.Parameters.Prerecibo;
using CargaClic.Contracts.Results.Mantenimiento;
using CargaClic.Contracts.Results.Prerecibo;
using CargaClic.Data.Interface;
using CargaClic.Domain.Mantenimiento;
using CargaClic.Domain.Prerecibo;
using CargaClic.Repository.Contracts.Seguimiento;
using CargaClic.Repository.Interface;
using Common.QueryHandlers;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CargaClic.API.Controllers.Recepcion
{
    
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenReciboController : ControllerBase
    {
        private readonly IQueryHandler<ListarOrdenReciboParameter> _handler;
        private readonly IRepository<OrdenRecibo> _repository;
         private readonly IConfiguration _config;
        private readonly IRepository<OrdenReciboDetalle> _repositoryDetalle;

        private readonly IRepository<ErrorGuia> _repositoryErrorGuia;
        private readonly IRepository<GuiaCabecera> _repositoryGuiaCabecera;
        private readonly IRepository<GuiaDetalle> _repositoryGuiaDetalle;


        private readonly IQueryHandler<ObtenerOrdenReciboParameter> _handlerCab;
        private readonly IQueryHandler<ObtenerOrdenReciboDetalleParameter> _handlerDetalle;
        private readonly IQueryHandler<ObtenerEquipoTransporteParameter> _handlerEqTransporte;
        private readonly IQueryHandler<ListarEquipoTransporteParameter> _handlerListarEqTransporte;
        private readonly IOrdenReciboRepository _repoOrdenRecibo;
        private readonly IRepository<Vehiculo> _repoVehiculo;
        private readonly IRepository<Chofer> _repoChofer;
        private readonly IRepository<Proveedor> _repoProveedor;
        private readonly IQueryHandler<ListarOrdenReciboByEquipoTransporteParameter> _handlerByEquipoTransporte;
        private readonly IMapper _mapper;

        private readonly IRepository<Producto> _repositoryProducto;

        

        public OrdenReciboController(IQueryHandler<ListarOrdenReciboParameter> handler,
         IRepository<OrdenRecibo> repository,
         IRepository<GuiaCabecera> repositoryGuiaCabecera,
         IRepository<Producto> repositoryProducto,
         IRepository<OrdenReciboDetalle> repositoryDetalle,
         IQueryHandler<ObtenerOrdenReciboParameter> handlerCab,
         IQueryHandler<ObtenerOrdenReciboDetalleParameter> handlerDetalle,
         IQueryHandler<ObtenerEquipoTransporteParameter> handlerEqTransporte,
         IQueryHandler<ListarEquipoTransporteParameter> handlerListarEqTransporte,
         IOrdenReciboRepository repoOrdenRecibo,
         IRepository<Vehiculo> repoVehiculo,
         IRepository<Chofer> repoChofer,
         IRepository<Proveedor> repoProveedor,
         IQueryHandler<ListarOrdenReciboByEquipoTransporteParameter> handlerByEquipoTransporte,

         IMapper mapper
, IRepository<GuiaDetalle> repositoryGuiaDetalle
, IRepository<ErrorGuia> repositoryErrorGuia
, IConfiguration config )
        {
            _handler = handler;
            _repository = repository;
            _repositoryDetalle = repositoryDetalle;
            _handlerCab = handlerCab;
            _handlerDetalle = handlerDetalle;
            _handlerEqTransporte = handlerEqTransporte;
            _handlerListarEqTransporte = handlerListarEqTransporte;
            _repoOrdenRecibo = repoOrdenRecibo;
            _repoVehiculo = repoVehiculo;
            _repositoryProducto =  repositoryProducto;
            _repoChofer = repoChofer;
            _repoProveedor = repoProveedor;
            _handlerByEquipoTransporte = handlerByEquipoTransporte;
            _mapper = mapper;
            _repositoryGuiaCabecera = repositoryGuiaCabecera;
            _repositoryGuiaDetalle = repositoryGuiaDetalle;
            _repositoryErrorGuia = repositoryErrorGuia;
            _config = config;
        }
        //////////////////// Obtener Listado de ordenes /////
        [HttpGet]
      public IActionResult GetOrders(int? PropietarioId, int? EstadoId , int? DaysAgo , string fec_ini , string fec_fin , int? AlmacenId)
      {
          var param = new ListarOrdenReciboParameter
          {   
              PropietarioId = PropietarioId,
              EstadoId = EstadoId,
              DaysAgo = DaysAgo,
              fec_fin = fec_fin,
              fec_ini = fec_ini,
              AlmacenId = AlmacenId
          };
          var resp = (ListarOrdenReciboResult)  _handler.Execute(param);
          return Ok(resp.Hits);
      }
      [HttpDelete("DeleteOrder")]
      public async Task<IActionResult> DeleteOrder(Guid OrdenReciboId)
      {
          var detalles = await _repositoryDetalle.GetAll(x=>x.OrdenReciboId == OrdenReciboId);
           _repositoryDetalle.DeleteAll(detalles);

          var ordenrecibo = await _repository.Get(x=>x.Id == OrdenReciboId);
          

          _repository.Delete(ordenrecibo);
                    
          return Ok(ordenrecibo);
      }
      [HttpDelete("DeleteOrderDetail")]
      public async Task<IActionResult> DeleteOrderDetail(long id)
      {
          var detalle = await _repositoryDetalle.Get(x=>x.Id == id);
          _repositoryDetalle.Delete(detalle);
          return Ok(detalle);
      }
       //////////////////// Obtener Listado de ordenes por EquipoTransporte /////
      [HttpGet("GetOrderbyEquipoTransporte")]
      public IActionResult GetOrderbyEquipoTransporte(long EquipoTransporteId)
      {
          var param = new ListarOrdenReciboByEquipoTransporteParameter
          {   
               EquipoTransporteId  = EquipoTransporteId,
              
          };
          var resp = (ListarOrdenReciboByEquipoTransporteResult)  _handlerByEquipoTransporte.Execute(param);
          return Ok(resp.Hits);
      }
      ///////////////////// Obtener Detalle ///////
      [HttpGet("GetOrderDetail")]
      public IActionResult GetOrderDetail(long Id)
      { 
        var param = new ObtenerOrdenReciboDetalleParameter {
          Id = Id  
        };
        
        var resp = (ObtenerOrdenReciboDetalleResult)_handlerDetalle.Execute(param);
        return Ok(resp);
      }

      ///////////////// Obtener Orden (incluye detalles) /////////
      [HttpGet("GetOrder")]
      public IActionResult GetOrder(Guid Id)
      {
        var param = new ObtenerOrdenReciboParameter {
          Id = Id  
        };
        // var resp =  await  _repository.Get(x=>x.Id == Id);
        // var det =  await _repositoryDetalle.GetAll(x=>x.OrdenReciboId == Id);
        var resp = (ObtenerOrdenReciboResult)_handlerCab.Execute(param);
        return Ok(resp);
      }


#region _RegistroGuias
      [HttpGet("ListarGuiaCabecera")]
      public async Task<IActionResult> ListarGuiaCabecera(int? idcliente, string fecharegistro)
      {
             var resp  = await  _repoOrdenRecibo.ListarGuiaCabecera(idcliente,fecharegistro);
            return Ok (resp);

      }
    [HttpGet("ListarGuiasPendientes")]
      public async Task<IActionResult> ListarGuiasPendientes(int? idcliente, string fecharegistro)
      {
             var resp  = await  _repoOrdenRecibo.ListarGuiaCabecera(idcliente,fecharegistro);
            return Ok (resp);

      }
      [HttpGet("ListarGuiaDetalle")]
      public async Task<IActionResult> ListarGuiaDetalle(long idguia)
      {
             var resp  = await  _repoOrdenRecibo.ListarGuiaDetalle(idguia);
            return Ok (resp);

      }
      [HttpPost("updateGuiaCabecera")]
      public async Task<IActionResult> updateGuiaCabecera(GuiaCabeceraForUpdate guiaCabeceraForRegister)
      {
          var resp  = await _repositoryGuiaCabecera.Get(x=> x.id == guiaCabeceraForRegister.id);
          resp.NroGuia = guiaCabeceraForRegister.NroGuia;
          resp.FechaGuia = guiaCabeceraForRegister.FechaGuia;

          await _repositoryGuiaDetalle.SaveAll();


          return Ok(resp);
      }
      [HttpPost("registerGuiaCabecera")]
      public async Task<IActionResult> RegisterGuiaCabecera(GuiaCabeceraForRegister guiaCabeceraForRegister)
      {
              
              var guiaultima =   await _repositoryGuiaCabecera.GetMaxCarga();
              if(guiaultima == null)
              {
                guiaultima = new GuiaCabecera();
                guiaultima.IdCarga = 1;
              }

              guiaCabeceraForRegister.idcarga = guiaultima.IdCarga;

              GuiaCabecera obj ;
              List<GuiaCabecera> objs = new List<GuiaCabecera>() ;

              for (int i = 0; i < guiaCabeceraForRegister.Cantidad  ; i++)
              {
                  obj = new GuiaCabecera();
                  obj.PropietarioID = guiaCabeceraForRegister.PropietarioID;
                  obj.FechaRegistro = DateTime.Now;
                  obj.TipoMovimientoId = guiaCabeceraForRegister.TipoMovimientoId;
                  obj.IdCarga = guiaultima.IdCarga + 1;
                  obj.IdEstado = 37;
                  objs.Add(obj);
                  await _repositoryGuiaCabecera.AddAsync(obj);
              }
            
               await _repositoryGuiaCabecera.SaveAll();


            return Ok(guiaCabeceraForRegister);
      }

      [HttpPost("registerGuiaDetalle")]
      public async Task<IActionResult> registerGuiaDetalle(GuiaDetalleForRegister guiaDetalleForRegister)
      {
              
              // _repositoryGuiaCabecera.Get(x=> x.NroGuia == guiaCabeceraForRegister. )

            

              GuiaDetalle odetalle = new GuiaDetalle();
              odetalle.Cantidad = guiaDetalleForRegister.cantidad;
              odetalle.EstadoId = guiaDetalleForRegister.estadoid;
              odetalle.FechaRegistro = DateTime.Now;
              odetalle.GuiaId = guiaDetalleForRegister.GuiaId;
              odetalle.Lote = guiaDetalleForRegister.lote;
              odetalle.Referencia = guiaDetalleForRegister.referencia;
              odetalle.ProductoId = guiaDetalleForRegister.productoid;



              await _repositoryGuiaDetalle.AddAsync(odetalle);

            
            
               await _repositoryGuiaCabecera.SaveAll();


              return Ok(odetalle);
      }
  

      


#endregion


#region _Registros
     [HttpPost("adderrorguia")]
      public async Task<IActionResult> adderrorguia(int iddetalle, int iderror)
      {
            ErrorGuia err = new ErrorGuia();
            err.iddetalleguia = iddetalle;
            err.iderror = iderror;

             await  _repositoryErrorGuia.AddAsync(err);
             await _repositoryErrorGuia.SaveAll();

            return Ok(err);
      }
  
      [HttpPost("register")]
      public async Task<IActionResult> Register(OrdenReciboForRegisterDto ordenReciboForRegisterDto)
      {
              var NumOrden =  await   _repository.GetMaxNumOrdenRecibo();

              var existe =  await _repository.Get(x=>x.GuiaRemision == ordenReciboForRegisterDto.GuiaRemision
               && x.PropietarioId == ordenReciboForRegisterDto.PropietarioId);

               if(existe != null)
               {
                   throw new ArgumentException("Ya existe la Guía de Remisión");
               }

            



            var param = new OrdenRecibo {
                Id =  Guid.NewGuid(),
                NumOrden = (Convert.ToInt64(NumOrden.NumOrden) + 1).ToString().PadLeft(7,'0'),
                PropietarioId = ordenReciboForRegisterDto.PropietarioId,
                Propietario = ordenReciboForRegisterDto.Propietario,
                AlmacenId = ordenReciboForRegisterDto.AlmacenId,
                GuiaRemision = ordenReciboForRegisterDto.GuiaRemision,
                FechaEsperada  = Convert.ToDateTime(ordenReciboForRegisterDto.FechaEsperada),
                FechaRegistro = DateTime.Now,
                HoraEsperada = ordenReciboForRegisterDto.HoraEsperada,
                EstadoId = (Int16) Constantes.EstadoOrdenIngreso.Planeado,
                UsuarioRegistro = 1,//ordenReciboForRegisterDto.UsuarioRegistro,
                Activo = true
                
            };
            var createdUser = await _repository.AddAsync(param);
            return Ok(createdUser);
      }
        [HttpPost("UploadFile")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile(int usrid)
        {
          // var seguimiento = new Seguimiento();
            try
            {
                // Grabar Excel en disco
                string fullPath =  SaveFile(0);
                // Leer datos de excel
                var celdas = GetExcel(fullPath);
                // Generar entidades
                var entidades = ObtenerEntidades_CargaMasiva(celdas);
                // Grabar entidades en base de datos
                
               var carga = new CargaMasivaForRegister();
               carga.estado_id = 1;
               carga.fecha_registro = DateTime.Now;
               carga.ordensalidaid = usrid;
            
              var NumOrden =  await   _repository.GetMaxNumOrdenRecibo();

            



            var param = new OrdenRecibo {
                Id =  Guid.NewGuid(),
                NumOrden = (Convert.ToInt64(NumOrden.NumOrden) + 1).ToString().PadLeft(7,'0'),
                PropietarioId = 47,
                Propietario ="EULEN",
                AlmacenId = 8,
                GuiaRemision = "IN EU",
                FechaEsperada  = Convert.ToDateTime("01/12/2021"),
                FechaRegistro = DateTime.Now,
                HoraEsperada = "03:00",
                EstadoId = (Int16) Constantes.EstadoOrdenIngreso.Planeado,
                UsuarioRegistro = 1,//ordenReciboForRegisterDto.UsuarioRegistro,
                Activo = true
                
            };
            var createdUser = await _repository.AddAsync(param);
            await _repository.SaveAll();
            int linea = 1;

          foreach (var item in entidades)
          {

              var idproducto =await  _repositoryProducto.Get( x => x.Codigo == item.codigo );
                   var param2 = new OrdenReciboDetalle {
                OrdenReciboId = createdUser.Id,
                Linea = linea.ToString() ,//ordenReciboDetalleForRegisterDto.Linea,
                ProductoId   =  idproducto.Id,
                Lote = "",
                HuellaId =1 ,
                EstadoID = 1,
                Cantidad = item.stock.Value,
                referencia = "",
                Completo = false
                
            };
            linea = linea + 1;
            var resp = await _repositoryDetalle.AddAsync(param2);
              await _repository.SaveAll();
          }
     



                

            }
            catch (System.Exception ex)
            {
                return Ok(ex.Message);
                throw ex;
              
            }
            return Ok();
         }
         public List<CargaMasivaDetalleForRegister> ObtenerEntidades_CargaMasiva(List<List<String>> data) 
        {
                data = validar_fin(data);
                var totales = new List<CargaMasivaDetalleForRegister>();
                CargaMasivaDetalleForRegister linea ;

                foreach (var item in data.Skip(1))
                {
                    linea =  new CargaMasivaDetalleForRegister();
                    linea.codigo = item[0]  ;
                    linea.stock = Convert.ToInt32(item[4])  ;
                    
   
                
                
                    totales.Add(linea);
                    
                }
                return totales;
        }
           private string ValidarRequerido(string v, string field)
        {
            if(String.IsNullOrEmpty(v))
            {
              throw new ArgumentException( $" {field} no puede estar en blanco .");
            }
            return v;
        }

        private List<List<string>> validar_fin(List<List<string>> data)
        {
            List<List<string>> new_data = new List<List<string>>();
            foreach (var item in data)
            {
                if(item[0] == "" && item[2] == ""){
                    break;
                }
                else 
                new_data.Add(item);
                
            }
            return new_data;
        }

        private  String SaveFile(long usuario_id)
        {
            
            var fullPath = string.Empty;
          
            
            var ruta =  _config.GetSection("AppSettings:Uploads").Value;

            var file = Request.Form.Files[0];
            var idOrden = usuario_id;

            string folderName = idOrden.ToString();
            string webRootPath = ruta ;
            string newPath = Path.Combine(webRootPath, folderName);

            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(Request.Form.Files[0].OpenReadStream()))
            {
                fileData = binaryReader.ReadBytes(Request.Form.Files[0].ContentDisposition.Length);
            }

            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (file.Length > 0)
            {
                string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                fullPath = Path.Combine(newPath, fileName);

                var checkextension = Path.GetExtension(fileName).ToLower();
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {

                    file.CopyTo(stream);
                    
                }

            }
            return fullPath;

        }
          public List<List<string>> GetExcel(string fullPath)
        {     
             List<List<string>> valores = new List<List<string>>();
            try
            {
                
                using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(fullPath, false))
                {
                    WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                    IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                    string relationshipId = sheets.First().Id.Value;
                    WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                    Worksheet workSheet = worksheetPart.Worksheet;
                    SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                    IEnumerable<Row> rows = sheetData.Descendants<Row>();
                    // foreach (Cell cell in rows.ElementAt(0))
                    // {
                    //     dt.Columns.Add(GetCellValue(spreadSheetDocument, cell));
                    // }
                    foreach (Row row in rows) //this will also include your header row...
                    {
                         List<String> linea = new List<string>();
                        int columnIndex = 0;
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            // Gets the column index of the cell with data
                            int cellColumnIndex = (int)GetColumnIndexFromName(GetColumnName(cell.CellReference));
                            cellColumnIndex--; //zero based index
                            if (columnIndex < cellColumnIndex)
                            {
                                do
                                {
                                    linea.Add(""); //Insert blank data here;
                                    columnIndex++;
                                }
                                while (columnIndex < cellColumnIndex);
                            }
                             linea.Add(GetCellValue(spreadSheetDocument, cell));
                          
                            columnIndex++;
                        }
                        valores.Add(linea);
                    }
                }
               // dt.Rows.RemoveAt(0); //...so i'm taking it out here.
                     return valores;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
          public static string GetColumnName(string cellReference)
        {
            // Create a regular expression to match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellReference);
            return match.Value;
        }
      
        public static int? GetColumnIndexFromName(string columnName)
        {
                       
            //return columnIndex;
            string name = columnName;
            int number = 0;
            int pow = 1;
            for (int i = name.Length - 1; i >= 0; i--)
            {
                number += (name[i] - 'A' + 1) * pow;
                pow *= 26;
            }
            return number;
        }
        public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            if (cell.CellValue ==null)
            {
            return "";
            }
            string value = cell.CellValue.InnerXml;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }

      [HttpPost("update")]
      public async Task<IActionResult> Update(OrdenReciboForRegisterDto ordenReciboForRegisterDto)
      {
            var orden = await _repository.Get(x=>x.Id == ordenReciboForRegisterDto.Id);

            orden.PropietarioId = ordenReciboForRegisterDto.PropietarioId;
            orden.Propietario = ordenReciboForRegisterDto.Propietario;
            orden.GuiaRemision = ordenReciboForRegisterDto.GuiaRemision;
            orden.FechaEsperada  = Convert.ToDateTime(ordenReciboForRegisterDto.FechaEsperada);
            orden.HoraEsperada = ordenReciboForRegisterDto.HoraEsperada;
             orden.AlmacenId = ordenReciboForRegisterDto.AlmacenId;

            var createdUser = await _repository.SaveAll();
            return Ok(createdUser);
      }
      [HttpPost("register_detail")]
      public async Task<IActionResult> Register_Detail(OrdenReciboDetalleForRegisterDto ordenReciboDetalleForRegisterDto)
      {
           string linea = "";

           var detalles = await  _repositoryDetalle.GetAll(x=>x.OrdenReciboId == ordenReciboDetalleForRegisterDto.OrdenReciboId);
           if(detalles.Count() == 0)
           {
              linea = "0001";
           }
           else {
            linea = detalles.Max(x=>x.Linea).ToString();
           linea = (Convert.ToInt32(linea) + 1).ToString().PadLeft(4,'0');
           }
           

            var param = new OrdenReciboDetalle {
                OrdenReciboId = ordenReciboDetalleForRegisterDto.OrdenReciboId,
                Linea = linea,//ordenReciboDetalleForRegisterDto.Linea,
                ProductoId   = ordenReciboDetalleForRegisterDto.ProductoId,
                Lote = ordenReciboDetalleForRegisterDto.Lote,
                HuellaId = ordenReciboDetalleForRegisterDto.HuellaId,
                EstadoID = ordenReciboDetalleForRegisterDto.EstadoID,
                Cantidad = ordenReciboDetalleForRegisterDto.cantidad,
                referencia = ordenReciboDetalleForRegisterDto.referencia,
                Completo = false
            };
            var resp = await _repositoryDetalle.AddAsync(param);
            return Ok(resp);
      }
      [HttpPost("identify_detail")]
      public async Task<IActionResult> Identify_detail(OrdenReciboDetalleForIdentifyDto ordenReciboDetalleForIdentifyDto)
      {
                var id = await _repoOrdenRecibo.identifyDetail(ordenReciboDetalleForIdentifyDto);
                return Ok(id);
     
      }
      [HttpPost("identify_detail_mix")]
      public async Task<IActionResult> Identify_detail_mix(IEnumerable<OrdenReciboDetalleForIdentifyDto> ordenReciboDetalleForIdentifyDto)
      {
                var id = await _repoOrdenRecibo.identifyDetailMix(ordenReciboDetalleForIdentifyDto);
                return Ok(id);
     
      }
      [HttpPost("close_details")]
      public async Task<IActionResult> Close_Details(Guid Id)
      {
          //Valida si todos los detalles estan cerrados  
          var detalles = await _repositoryDetalle.GetAll(x=>x.OrdenReciboId == Id);
          var pendientes =  detalles.ToList().Where(x=>x.Completo == false);
          
            if(pendientes.Count() > 0) 
            {
                return NotFound("Hay pendientes de identificación en la OR.");
                // throw new ArgumentException("Hay pendientes de identificación en la OR.");
            }

            await _repoOrdenRecibo.closeDetails(Id);
            //var id = await _repoOrdenRecibo.identifyDetail(ordenReciboDetalleForIdentifyDto);
            return Ok(Id);
     
      }

      
#endregion
#region _repoEquipoTransporte

        [HttpGet("GetEquipoTransporte")]
        public  IActionResult GetEquipoTransporte(string placa)
        {
             var vehiculo =  _repoVehiculo.Get(x=>x.Placa == placa).Result;

             if(vehiculo == null)
                return Ok();
             

            var param = new ObtenerEquipoTransporteParameter
            {
                VehiculoId = vehiculo.Id 
            };
            var result = (ObtenerEquipoTransporteResult)   _handlerEqTransporte.Execute(param);
            return Ok(result);

        }
        [HttpGet("ListEquipoTransporte")]
        public IActionResult ListEquipoTransporte(int? PropietarioId, int EstadoId  
        , string fec_fin, string fec_ini, int? AlmacenId)
        {
            var param = new ListarEquipoTransporteParameter
            {
                EstadoId = EstadoId
                ,PropietarioId = PropietarioId
                ,fec_fin =fec_fin
                ,fec_ini = fec_ini
                ,AlmacenId = AlmacenId
            };
            var result = (ListarEquipoTransporteResult)  _handlerListarEqTransporte.Execute(param);
            return Ok(result.Hits.OrderByDescending(x=>x.EquipoTransporte));
        }

        [HttpPost("RegisterEquipoTransporte")]
        public async Task<IActionResult> RegisterEquipoTransporte(EquipoTransporteForRegisterDto equipotrans)
        {
              
              var param = new EquipoTransporte();
               
              var vehiculo = await _repoVehiculo.Get(x=>x.Placa ==  equipotrans.Placa);
              //Insertar nuevo
               if(vehiculo == null)
               {
                  vehiculo = new Vehiculo();
                  vehiculo.TipoId = equipotrans.tipoVehiculo;
                  vehiculo.MarcaId = equipotrans.marcaVehiculo;
                  vehiculo.Placa = equipotrans.Placa;
                  vehiculo = await _repoVehiculo.AddAsync(vehiculo);
               }
               
              var proveedor = await _repoProveedor.Get(x=>x.Ruc == equipotrans.Ruc);
              if(proveedor == null)
              {
                  proveedor = new Proveedor();
                  proveedor.RazonSocial = equipotrans.RazonSocial;
                  proveedor.Ruc = equipotrans.Ruc;
                  proveedor = await _repoProveedor.AddAsync(proveedor);
              }
               
              
              var chofer = await _repoChofer.Get(x=>x.Dni == equipotrans.Dni);
              if(chofer == null)
              {
                   chofer = new Chofer();
                   chofer.Brevete = equipotrans.Brevete;
                   chofer.Dni = equipotrans.Dni;
                   chofer.NombreCompleto = equipotrans.NombreCompleto;
                   chofer = await _repoChofer.AddAsync(chofer);
              }
              param.ProveedorId = proveedor.Id;
              param.VehiculoId = vehiculo.Id;
              param.ChoferId = chofer.Id;
              param.EstadoId = (int) Constantes.EstadoEquipoTransporte.EnProceso;
              param.PropietarioId = equipotrans.PropietarioId;

             var createdEquipoTransporte = await _repoOrdenRecibo.RegisterEquipoTransporte(param,equipotrans.OrdenReciboId);
             
             return Ok(createdEquipoTransporte);
        }
        [HttpPost("MatchTransporteOrdenIngreso")]
        public async Task<IActionResult> MatchTransporteOrdenIngreso(EquipoTransporteForRegisterDto equipotrans)
        {
             //var param = _mapper.Map<EquipoTransporteForRegisterDto, EquipoTransporte>(equipotrans);
              //Buscar Placa
          var createdEquipoTransporte = await _repoOrdenRecibo.matchTransporteOrdenIngreso(equipotrans);
          return Ok(createdEquipoTransporte);
        }


#endregion
#region _Ubicaciones

        [HttpPost("assignmentOfDoor")]
        public async Task<IActionResult> assignmentOfDoor(LocationsForAssignmentDto locationsForAssignmentDto)
        {
            var result = await _repoOrdenRecibo.assignmentOfDoor(locationsForAssignmentDto.EquipoTransporteId,locationsForAssignmentDto.UbicacionId);
            return Ok(result);
        }



#endregion
       [HttpGet("GetListarCalendario")]
        public async Task<IActionResult> GetListarCalendario()
        { 
            var resp  = await _repoOrdenRecibo.GetListarCalendario();
            return Ok (resp);
        }

    }
}