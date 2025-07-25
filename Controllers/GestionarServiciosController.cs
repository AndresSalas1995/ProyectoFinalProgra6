using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataModels;
using ProyectoFinalPogragamacionVI.Permisos;
using static DataModels.PviProyectoFinalDBStoredProcedures;

namespace ProyectoFinalPogragamacionVI.Models
{
    [ValidarSession]
    [AutorizarEmpleado]
    public class GestionarServiciosController : Controller
    {
        // GET: GestionarServicios
        public ActionResult Index()
        {
            List<SpConsultarServiciosResult> lista = new List<SpConsultarServiciosResult>();
            {
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    lista = db.SpConsultarServicios().ToList();
                }
            }
            return View(lista);
        }
    }
}
