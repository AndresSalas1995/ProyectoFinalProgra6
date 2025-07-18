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
    [ValidarSession]
    public class UsuarioController : Controller
    {
        // GET: Usuario
        public ActionResult Index()
        {
            List<SpConsultarCobrosUsuarioResult> lista;
            int idPersona = (int)Session["id_persona"];
            string nombreUsuario = "";

            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                lista = db.SpConsultarCobrosUsuario(idPersona).ToList();
                // Obtener el nombre del usuario autenticado
                var persona = db.Personas.FirstOrDefault(p => p.IdPersona == idPersona);
                if (persona != null)
                {
                    nombreUsuario = persona.Nombre + " " + persona.Apellido;
                }
            }

            ViewBag.NombreUsuario = nombreUsuario;
            ViewBag.IdUsuario = idPersona;

            return View(lista);

        }

        //Dropdown para cargar casas por cliente
        [HttpGet]
        public JsonResult ObtenerCasasPorCliente(int idCliente)
        {
            var casas = new List<Dropdown>();

            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                casas = db.SpObtenerCasasPorCliente(idCliente)
                    .Select(c => new Dropdown
                    {
                        Id = c.Id_casa,
                        Nombre = c.Nombre_casa
                    }).ToList();
            }

            return Json(casas, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Crear(int? id)
        {
            int idPersona = (int)Session["id_persona"];
            string nombreUsuario = "";
            List<SelectListItem> servicios;
            Cobros cobros = new Cobros();

            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                // Obtener nombre usuario
                var persona = db.Personas.FirstOrDefault(p => p.IdPersona == idPersona);
                if (persona != null)
                {
                    nombreUsuario = persona.Nombre + " " + persona.Apellido;
                }

                // Servicios activos
                servicios = db.Servicios
                              .Where(s => s.Estado == true)
                              .Select(s => new SelectListItem
                              {
                                  Value = s.IdServicio.ToString(),
                                  Text = s.Nombre
                              }).ToList();

                // Si hay id, obtener cobro
                if (id.HasValue)
                {
                    cobros = db.SpObtenerCobroPorId(id).Select(c => new Cobros
                    {
                        Id_cobro = c.Id_cobro,
                        Nombre_casa = c.Nombre_casa,
                        Nombre_cliente = c.Propietario,
                        anno = c.Anno,
                        mes = c.Mes,
                        servicio = c.Nombre_servicio,
                    }).FirstOrDefault();
                }
            }

            ViewBag.IdUsuario = idPersona;
            ViewBag.NombreUsuario = nombreUsuario;
            ViewBag.Servicios = servicios;

            // Listas internas año y mes
            var años = Enumerable.Range(2024, 11)  // 2024 a 2034
                .Select(y => new SelectListItem { Value = y.ToString(), Text = y.ToString() })
                .ToList();

            años.Insert(0, new SelectListItem { Value = "", Text = "Seleccione un año" });

            var meses = new List<SelectListItem>
            {
                 new SelectListItem { Value = "", Text = "Seleccione un mes" },
                 new SelectListItem { Value = "1", Text = "Enero" },
                 new SelectListItem { Value = "2", Text = "Febrero" },
                 new SelectListItem { Value = "3", Text = "Marzo" },
                 new SelectListItem { Value = "4", Text = "Abril" },
                 new SelectListItem { Value = "5", Text = "Mayo" },
                 new SelectListItem { Value = "6", Text = "Junio" },
                 new SelectListItem { Value = "7", Text = "Julio" },
                 new SelectListItem { Value = "8", Text = "Agosto" },
                 new SelectListItem { Value = "9", Text = "Septiembre" },
                 new SelectListItem { Value = "10", Text = "Octubre" },
                 new SelectListItem { Value = "11", Text = "Noviembre" },
                 new SelectListItem { Value = "12", Text = "Diciembre" }
             };

            ViewBag.Años = años;
            ViewBag.Meses = meses;


            return View(cobros);
        }

        [HttpPost] //guarda la informacion
        public ActionResult Crear(Cobros cobros)
        {
            try
            {
                // Validamos que este seleccionado un cliente
                if (cobros.IdCliente == 0)
                {
                    int idCliente;
                    if (int.TryParse(Request["idCliente"], out idCliente))
                    {
                        cobros.IdCliente = idCliente;
                    }
                    else
                    {
                        cobros.IdCliente = (int)Session["id_persona"];
                    }
                }

                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    // Crear DataTable para los servicios seleccionados
                    var serviciosTable = new System.Data.DataTable();
                    serviciosTable.Columns.Add("id_servicio", typeof(int));

                    foreach (var idServicio in cobros.Servicios)
                    {
                        serviciosTable.Rows.Add(idServicio);
                    }

                    // Ejecutar el procedimiento almacenado
                    db.SpInsertarCobroCompleto(
                        idCasa: cobros.IdCasa,
                        mes: cobros.mes,
                        anno: cobros.anno,
                        idUser: cobros.IdCliente, // aca usamos el cliente seleccionado
                        serviciosSeleccionados: serviciosTable
                    );
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(cobros);
            }
        }
    }
}