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
    public class CobrosController : Controller
    {
        // GET: Cobros
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detalle(int id)
        {
            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                var resultado = db.SpConsultarDetalleCobro(id).ToList();

               
            }
            return View();
        }


    }
}