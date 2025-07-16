using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataModels;

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
                var cobro = db.SpConsultarDetalleCobro(id).FirstOrDefault();
                if (cobro == null)
                {
                    return HttpNotFound();
                }
                return View(cobro);

            }
                
        }
    }
}