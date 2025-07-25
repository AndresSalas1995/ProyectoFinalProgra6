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
    [AutorizarEmpleado]
    public class GestionarCasasController : Controller
    {
        // GET: GestionarCasas
        public ActionResult Index()
        {
            List<SpConsultarCasasResult> lista = new List<SpConsultarCasasResult>();
            {
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    lista = db.SpConsultarCasas().ToList();
                }
            }
            return View(lista);
        }
    }
}