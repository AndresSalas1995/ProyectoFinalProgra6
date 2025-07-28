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

        [HttpGet]
        public ActionResult Editar(int? id)
        {
            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                var detalle = db.SpConsultarDetalleCobro(id).FirstOrDefault();
                if (detalle == null)
                    return HttpNotFound();

                //Validación Estado pagado o eliminado
                if (detalle.Estado.Equals("Pagado", StringComparison.OrdinalIgnoreCase) ||
                    detalle.Estado.Equals("Eliminado", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Mensaje"] = "Este cobro no puede ser modificado porque está pagado o eliminado.";
                    return RedirectToAction("Index", "Empleado");
                }

                //Periodo anterior o igual al actual
                var fechaActual = DateTime.Now;
                if (detalle.Año < fechaActual.Year ||
                    (detalle.Año == fechaActual.Year && detalle.Mes <= fechaActual.Month))
                {
                    TempData["Mensaje"] = "Este cobro no puede ser editado porque pertenece a un periodo anterior o actual.";
                    return RedirectToAction("Index", "Empleado");
                }

                var serviciosDb = db.SpConsultarServiciosPorCobroId(id).ToList();
                var serviciosActivos = db.Servicios.Where(s => s.Estado == true).ToList();

                var model = new EditarServiciosCobroViewModel
                {
                    IdCobro = detalle.Id_cobro,
                    NombreCasa = detalle.Nombre_casa,
                    NombreCliente = detalle.Nombre_cliente,
                    Mes = detalle.Mes,
                    Año = detalle.Año,
                    Monto = detalle.Monto,
                    Servicios = serviciosActivos.Select(s => new ServicioEditarViewModel
                    {
                        IdServicio = s.IdServicio,
                        Nombre = s.Nombre,
                        Seleccionado = serviciosDb.Any(serv => serv.IdServicio == s.IdServicio && serv.Incluido == 1)
                    }).ToList()
                };

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Editar(EditarServiciosCobroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // En caso de error, recargar servicios activos para volver a mostrar
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    var serviciosActivos = db.Servicios.Where(s => s.Estado == true).ToList();
                    model.Servicios = serviciosActivos.Select(s => new ServicioEditarViewModel
                    {
                        IdServicio = s.IdServicio,
                        Nombre = s.Nombre,
                        Seleccionado = model.Servicios?.FirstOrDefault(x => x.IdServicio == s.IdServicio)?.Seleccionado ?? false
                    }).ToList();
                }

                return View(model);
            }

            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                // Crear DataTable con servicios seleccionados para pasar al procedimiento almacenado
                var serviciosTable = new System.Data.DataTable();
                serviciosTable.Columns.Add("id_servicio", typeof(int));

                foreach (var servicio in model.Servicios.Where(s => s.Seleccionado))
                {
                    serviciosTable.Rows.Add(servicio.IdServicio);
                }

                // Llamar al procedimiento almacenado para actualizar servicios del cobro
                db.SpActualizarCobroCompleto(model.IdCobro, serviciosTable, idUser: Convert.ToInt32(Session["id_persona"]));
            }

            return RedirectToAction("Detalle", new { id = model.IdCobro });
        }


    }
}