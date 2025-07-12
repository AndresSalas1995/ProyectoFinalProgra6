using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataModels;
using ProyectoFinalPogragamacionVI.Models;
using ProyectoFinalPogragamacionVI.Permisos;
using static DataModels.PviProyectoFinalDBStoredProcedures;

namespace ProyectoFinalPogragamacionVI.Controllers
{
    [ValidarSession]
    [AutorizarEmpleado]
    public class EmpleadoController : Controller
    {
        // GET: Empleado
        //Carga mi lista en consultar cobro empleado
        public ActionResult Index(int? IdCliente, int? mes, int? anno)
        {
            List<SpConsultarCobroResult> lista = new List<SpConsultarCobroResult>();

            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                if (IdCliente == null && mes == null && anno == null)
                {
                    lista = db.SpConsultarCobro().ToList();
                }
                else
                {
                    var resultado = db.SpFiltrarCobrosEmpleado(IdCliente, mes, anno).ToList();

                    lista = resultado.Select(r => new SpConsultarCobroResult
                    {
                        Id_cobro = r.Id_cobro,
                        Nombre_casa = r.Nombre_casa,
                        Nombre_cliente = r.Nombre_cliente,
                        Periodo = r.Periodo,
                        Estado = r.Estado
                    }).ToList();
                }
            }

            return View(lista);
        }

        //Carga el dropdown de lista de clientes activos
        [HttpGet]
        public JsonResult ObtenerClientesActivos()
        {
            var datos = new List<Dropdown>();
            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                datos = db.SpObtenerClientesActivos()
                    .Select(c => new Dropdown
                    {
                        Id = c.Id_persona,
                        Nombre = c.Nombre_completo
                    }).ToList();
            }
            return Json(datos, JsonRequestBehavior.AllowGet);
        }


    }

}