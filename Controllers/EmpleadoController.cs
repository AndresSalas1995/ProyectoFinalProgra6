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
        // GET: Empelado
        public ActionResult Index()
        {
            List<SpConsultarCobroResult> lista;
            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                lista = db.SpConsultarCobro().ToList();
            }
            return View(lista);
        }


    }
}