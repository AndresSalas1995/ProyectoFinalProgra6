using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataModels;
using ProyectoFinalPogragamacionVI.Controllers;
using ProyectoFinalPogragamacionVI.Models;
using ProyectoFinalPogragamacionVI.Permisos;
using static DataModels.PviProyectoFinalDBStoredProcedures;

namespace ProyectoFinalPogragamacionVI.Controllers
{
    [ValidarSession]
    [AutorizarEmpleado]
    public class GestionarCasasController : Controller
    {
        // GET: GestionarCasas
        //Cargar la vista con la lista de casas
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

        // GET: CrearCasa
        //Cargar la vista para crear una nueva casa
        public ActionResult CrearCasa()
        {
            ViewBag.EsConsulta = false;
            return View(new Casas());
        }

        // POST: CrearCasa
        [HttpPost]
        public ActionResult CrearCasa(Casas casa)
        {
            string mensaje = "";
            try
            {
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    bool estado = true;
                    db.SpAgregarCasa(casa.Nombre, casa.metros, casa.numHabitaciones, casa.numBanos,
                        casa.idCliente, casa.FechaConstruccion, estado);
                    mensaje = "Casa creada correctamente";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al crear la casa: " + ex.Message;
                ModelState.AddModelError("", mensaje);
            }
            ViewBag.mensaje = mensaje;
            ViewBag.EsConsulta = false;
            return View(casa);
        }

        //Carga el dropdown de lista de clientes activos
        [HttpGet]
        public JsonResult ObtenerClientesActivos()
        {
            var datos = new List<Dropdown>();
            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                datos = db.SpObtenerClientesActivos()
                    .Select(c => new Dropdown
                    {
                        Id = c.Id_persona,
                        Nombre = c.Nombre_completo
                    }).ToList();
            }
            return Json(datos, JsonRequestBehavior.AllowGet);
        }

        //Get: EditarCasa
        public ActionResult EditarCasa(int? id)
        {
            Casas casa = new Casas();
            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                var resultado = db.SpObtenerCasaPorId(id).FirstOrDefault();

                if (resultado == null)
                    return HttpNotFound();

                if (resultado.Estado == false)
                    return RedirectToAction("CasaInactivo", new { id });

                casa = new Casas
                {
                    Id = resultado.Id_casa,
                    Nombre = resultado.Nombre_casa,
                    metros = resultado.Metros_cuadrados,
                    numHabitaciones = resultado.Numero_habitaciones,
                    numBanos = resultado.Numero_banos,
                    idCliente = resultado.Id_persona,
                    FechaConstruccion = resultado.Fecha_construccion,
                };
            }
            ViewBag.EsConsulta = false;
            return View("CrearCasa", casa);
        }
        // POST: EditarCasa
        [HttpPost]
        public ActionResult EditarCasa(Casas casa)
        {
            string mensaje = "";
            try
            {
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    db.SpActualizarCasa(casa.Id, casa.Nombre, casa.metros, casa.numHabitaciones, casa.numBanos, casa.FechaConstruccion,
                        casa.idCliente);
                    mensaje = "Casa actualizada correctamente";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al actualizar la casa: " + ex.Message;
                ModelState.AddModelError("", mensaje);
            }
            ViewBag.mensaje = mensaje;
            ViewBag.EsConsulta = true;
            return View("CrearCasa", casa);
        }

        //Para crear la vista de casa inactiva
        public ActionResult CasaInactivo(int? id)
        {
            ViewBag.IdCasa = id;
            return View();
        }

        //Accion para inactivar una casa
        [HttpPost]
        public ActionResult InactivarCasa(int id)
        {
            string mensaje = "";
            try
            {
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    db.SpInactivarCasa(id);
                }
                // Redirijo a la vista de que la casa fue inactivada exitosamente
                return RedirectToAction("CasaInactivoExitosamente", new { id = id });
            }
            catch (Exception ex)
            {
                mensaje = "Ocurrió un error al inactivar la casa: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        //Vista de casa inactiva exitosamente
        public ActionResult CasaInactivoExitosamente(int? id)
        {
            ViewBag.IdCasa = id;
            return View();
        }
    }
}
