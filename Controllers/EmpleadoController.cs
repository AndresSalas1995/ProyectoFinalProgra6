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
        public ActionResult Index(string nombreCliente, int? mes, int ? anno)
        {
            List<SpConsultarCobroResult> lista = new List<SpConsultarCobroResult>();

            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                if (string.IsNullOrWhiteSpace(nombreCliente))
                {
                    nombreCliente = null;
                }

                //Si no hay filtros uso el sp sin filtros
                if (string.IsNullOrEmpty(nombreCliente) && mes == null && anno == null)
                {
                    lista = db.SpConsultarCobro().ToList();
                }
                //Si hay filtro debo usar el sp con filtros
                else 
                {
                    //Mapeo manual si el SP con filtros devuelve un resultado diferente
                    //Nos aseguramos que se devulven los campos necesarios
                    var resultado = db.SpFiltrarCobrosEmpleado(nombreCliente, mes, anno).ToList();

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
    }
}