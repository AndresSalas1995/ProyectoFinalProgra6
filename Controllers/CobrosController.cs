using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataModels;
using ProyectoFinalPogragamacionVI.Models;
using ProyectoFinalPogragamacionVI.Permisos;

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
                    TempData["Mensaje"] = "Este cobro no puede ser modificado porque ya está pagado o eliminado.";
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


        [HttpPost]
        public ActionResult Pagar(int id)
        {
            try
            {
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    // Validar sesión
                    if (Session["id_persona"] == null)
                    {
                        TempData["Mensaje"] = "La sesión ha expirado. Por favor inicie sesión nuevamente.";
                        return RedirectToAction("Autenticacion", "Index");
                    }

                    int idUsuario = Convert.ToInt32(Session["id_persona"]);
                    bool esEmpleado = Session["es_empleado"] != null && Convert.ToInt32(Session["es_empleado"]) == 1;

                    // Obtener detalle del cobro
                    var detalle = db.SpConsultarDetalleCobro(id).FirstOrDefault();
                    if (detalle == null)
                    {
                        TempData["Mensaje"] = "El cobro no existe.";
                        return RedirectToAction("Index", "Empleado");
                    }

                    // Validar estado del cobro
                    if (detalle.Estado.Equals("Pagado", StringComparison.OrdinalIgnoreCase) ||
                        detalle.Estado.Equals("Eliminado", StringComparison.OrdinalIgnoreCase))
                    {
                        TempData["Mensaje"] = "Este cobro no puede ser pagado porque ya está pagado o eliminado.";
                        return RedirectToAction("Index", "Empleado");
                    }

                    // un empleado no puede pagar cobros que le pertenecen
                    if (esEmpleado && detalle.Id_persona == idUsuario)
                    {
                        TempData["Mensaje"] = "Como empleado, no puede pagar un cobro que le pertenece como cliente.";
                        return RedirectToAction("Index", "Empleado");
                    }

                    // no se puede pagar un cobro de un periodo anterior o igual al actual
                    var fechaActual = DateTime.Now;
                    if (detalle.Año < fechaActual.Year ||
                        (detalle.Año == fechaActual.Year && detalle.Mes <= fechaActual.Month))
                    {
                        TempData["Mensaje"] = "No se puede pagar un cobro de un periodo anterior o actual.";
                        return RedirectToAction("Index", "Empleado");
                    }

                    // Ejecutar procedimiento para marcar el cobro como pagado
                    db.SpPagarCobro(id, idUsuario);

                    TempData["Mensaje"] = "El cobro fue pagado exitosamente.";
                }
                return RedirectToAction("PagoExitoso", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al procesar el pago: " + ex.Message;
                return RedirectToAction("Index", "Empleado");
            }
        }

        //Para crear la vista pago exitoso
        public ActionResult PagoExitoso(int? id)
        {
            ViewBag.Id_Cobro = id;
            return View();
        }

        //Metodo para eliminar un cobro
        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                var detalle = db.SpConsultarDetalleCobro(id).FirstOrDefault();
                if (detalle == null)
                {
                    TempData["Mensaje"] = "Cobro no encontrado.";
                    return RedirectToAction("Index", "Empleado");
                }

                // Validar estado pagado o eliminado
                if (detalle.Estado.Equals("Pagado", StringComparison.OrdinalIgnoreCase) ||
                    detalle.Estado.Equals("Eliminado", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Mensaje"] = "No se puede eliminar un cobro pagado o ya eliminado.";
                    return RedirectToAction("Index", "Empleado");
                }

                // Validar que el cobro NO pertenezca al usuario empleado logeado
                int IdUsuario = Convert.ToInt32(Session["id_persona"]);
                if (detalle.Id_persona == IdUsuario && Convert.ToInt32(Session["es_empleado"]) == 1)
                {
                    TempData["Mensaje"] = "No puede eliminar un cobro que le pertenece.";
                    return RedirectToAction("Index", "Empleado");  
                }

                // Validar que el periodo del cobro sea posterior a la fecha actual
                var fechaActual = DateTime.Now;
                if (detalle.Año < fechaActual.Year ||
                    (detalle.Año == fechaActual.Year && detalle.Mes <= fechaActual.Month))
                {
                    TempData["Mensaje"] = "No se puede eliminar un cobro de un periodo anterior o actual.";
                    return RedirectToAction("Index", "Empleado"); // O a la lista "Consultar Cobros"
                }

                // Si pasa todas las validaciones, eliminar (cambiar estado)
                db.SpEliminarCobro(id, IdUsuario);
            }

            TempData["Mensaje"] = "Cobro eliminado correctamente.";
            return RedirectToAction("PagoEliminadoExitoso", new { id = id });
        }

        //vista para mostrar mensaje de pago eliminado exitoso
        public ActionResult PagoEliminadoExitoso(int? id)
        {
            ViewBag.Id_Cobro = id;
            return View();
        }

    }
}