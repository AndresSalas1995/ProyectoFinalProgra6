using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataModels;
using ProyectoFinalPogragamacionVI.Permisos;
using static DataModels.PviProyectoFinalDBStoredProcedures;

namespace ProyectoFinalPogragamacionVI.Controllers
{
    [ValidarSession]
    public class EmpleadoController : Controller
    {
        // GET: Empleado
        //Carga mi lista en consultar cobro empleado
        public ActionResult Index(string nombreCliente, int? mes, int ? anno)
        {
            List<SpConsultarCobroResult> lista;

            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                //Si no hay filtros uso el sp sin filtros
                if (string.IsNullOrEmpty(nombreCliente) && mes == null && anno == null)
                {
                    lista = db.SpConsultarCobro().ToList();
                }
                //Si hay filtro debo usar el sp con filtros
                else
                {
                    lista = db.SpFiltrarCobrosEmpleado(nombreCliente, mes, anno
                    ).ToList();
                }
            }
            return View(lista);
        }

    }
}