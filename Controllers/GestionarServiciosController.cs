using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataModels;
using ProyectoFinalPogragamacionVI.Controllers;
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

        // GET: CrearServicio
        public ActionResult CrearServicio()
        {
            ViewBag.EsConsulta = false;
            return View(new Servicios());
        }

        // POST: CrearServicio
        [HttpPost]
        public ActionResult CrearServicio(Servicios servicio)
        {
            string mensaje = "";

            try
            {
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    bool estado = true;
                    db.SpAgregarServicio(servicio.nombre, servicio.descripcion, servicio.precio, servicio.categoriaId, estado);
                    mensaje = "Servicio creado correctamente";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al crear el servicio: " + ex.Message;
                ModelState.AddModelError("", mensaje);
            }

            ViewBag.mensaje = mensaje;
            ViewBag.EsConsulta = false;
            return View(servicio);
        }


        // GET: EditarServicio
        public ActionResult EditarServicio(int? id)
        {
            Servicios servicio = new Servicios();

            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                var resultado = db.SpLeerServicioPorId(id).FirstOrDefault();

                if (resultado == null)
                    return HttpNotFound();

                if (resultado.Estado == false)
                    return RedirectToAction("ServicioInactivo", new { id });

                servicio = new Servicios
                {
                    Id = resultado.Id_servicio,
                    nombre = resultado.Nombre,
                    descripcion = resultado.Descripcion,
                    precio = resultado.Precio,
                    categoria = resultado.Nombre_categoria,
                    categoriaId = resultado.Id_categoria,
                };
            }

            ViewBag.EsConsulta = true;
            return View("CrearServicio", servicio); // reutilizo la misma vista
        }

        // POST: EditarServicio
        [HttpPost]
        public ActionResult EditarServicio(Servicios servicio)
        {
            string mensaje = "";

            try
            {
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    db.SpActualizarServicio(servicio.Id, servicio.nombre, servicio.descripcion, servicio.precio, servicio.categoriaId);
                    mensaje = "Servicio actualizado correctamente";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al actualizar el servicio: " + ex.Message;
                ModelState.AddModelError("", mensaje);
            }

            ViewBag.mensaje = mensaje;
            ViewBag.EsConsulta = true;
            return View("CrearServicio", servicio); // reutilizas la misma vista
        }

        //Para crear la vista de servicio inactivo
        public ActionResult ServicioInactivo(int? id)
        {
            ViewBag.IdServicio = id;
            return View();
        }

        //Accion para inactivar un servicio
        [HttpPost]
        public ActionResult InactivarServicio(int id)
        {
            string mensaje = "";
            try
            {
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    db.SpInactivarServicio(id);
                }
                // Redirijo a la vista de que el serviicio fue inactivado exitosamente
                return RedirectToAction("ServicioInactivoExitosamente", new { id = id });
            }
            catch (Exception ex)
            {
                mensaje = "Ocurrió un error al inactivar el servicio: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        //Vista de servicio inactivo exisotamente
        public ActionResult ServicioInactivoExitosamente(int? id)
        {
            ViewBag.IdServicio = id;
            return View();
        }

        //cargar dropdown de categorias
        public JsonResult CargarCategorias()
        {
            var datos = new List<Dropdown>();
            try
            {
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    datos = db.SpObtenerCategorias().Select(c => new Dropdown
                    {
                        Id = c.Id_categoria,
                        Nombre = c.Nombre,
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                ModelState.AddModelError("", "Ocurrió un error al cargar las categorías: " + ex.Message);
            }
            return Json(datos);
        }
    }
}
