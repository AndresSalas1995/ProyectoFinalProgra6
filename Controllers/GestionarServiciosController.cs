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

        //Crear Servicio
        public ActionResult CrearServicio(int? id)
        {
            Servicios servicio = new Servicios();

            try
            {
                if (id.HasValue)
                {
                    using (var db = new PviProyectoFinalDB("MyDatabase"))
                    {
                        servicio = db.SpLeerServicioPorId(id).Select(s => new Servicios
                        {
                            Id = s.Id_servicio,
                            nombre = s.Nombre,
                            descripcion = s.Descripcion,
                            precio = s.Precio,
                            categoria = s.Nombre_categoria,
                            categoriaId = s.Id_categoria,
                        }).FirstOrDefault();

                    }
                }
            }
                
            catch (Exception ex)
            {
                // Manejo de excepciones
                ModelState.AddModelError("", "Ocurrió un error al crear el servicio: " + ex.Message);
            }

            return View(servicio);
        }

        [HttpPost]
        public ActionResult CrearServicio(Servicios servicio)
        {
            string mensaje = "";
            bool estado = true;
                try
                {
                    using (var db = new PviProyectoFinalDB("MyDatabase"))
                    {
                        db.SpAgregarServicio(servicio.nombre, servicio.descripcion, servicio.precio, servicio.categoriaId, estado);
                        mensaje = "Se ha insertado correctamente el jugador";
                    }
                }
                catch (Exception ex)
                {
                    mensaje = "No se ha insertado correctamente el jugador";
                }
            ViewBag.mensaje = mensaje;
            return View(servicio);

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
