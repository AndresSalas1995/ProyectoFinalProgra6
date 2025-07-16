using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataModels;
using ProyectoFinalPogragamacionVI.Models;

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
                // 1. Detalle del cobro
                var detalle = db.SpConsultarDetalleCobro(id).FirstOrDefault();
                if (detalle == null)
                {
                    return HttpNotFound();
                }

                // 2. Servicios asociados al cobro
                var serviciosDb = db.SpConsultarServiciosPorCobro(id).ToList();
                var servicios = serviciosDb.Select(s => new ServicioCobroViewModel
                {
                    Nombre = s.Nombre,
                    Incluido = s.Incluido == 1 
                }).ToList();

                // 3. Bitácora del cobro
                var bitacoraDb = db.SpConsultarBitacoraPorCobro(id).ToList();
                var bitacora = bitacoraDb.Select(b => new BitacoraCobroViewModel
                {
                    Fecha = b.Fecha,
                    Accion = b.Accion,
                    Detalle = b.Detalle,
                    RealizadoPor = b.RealizadoPor
                }).ToList();

                // 4. Armar el ViewModel
                var viewModel = new DetalleCobroViewModel
                {
                    Detalle = detalle,
                    Servicios = servicios,
                    Bitacora = bitacora
                };

                return View(viewModel);
            }
        }
    }
}