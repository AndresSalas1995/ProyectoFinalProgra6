using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using DataModels;
using ProyectoFinalPogragamacionVI.Models;
using ProyectoFinalPogragamacionVI.Permisos;
using static DataModels.PviProyectoFinalDBStoredProcedures;

namespace ProyectoFinalPogragamacionVI.Controllers
{
    [ValidarSession]
    [AutorizarEmpleado]
    public class EmpleadoController : Controller
    {
        // GET: Empleado
        //Carga mi lista en consultar cobro empleado
        public ActionResult Index(int? IdCliente, int? mes, int? anno)
        {
            List<SpConsultarCobroResult> lista = new List<SpConsultarCobroResult>();

            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                if (IdCliente == null && mes == null && anno == null)
                {
                    lista = db.SpConsultarCobro().ToList();
                }
                else
                {
                    var resultado = db.SpFiltrarCobrosEmpleado(IdCliente, mes, anno).ToList();

                    lista = resultado.Select(r => new SpConsultarCobroResult
                    {
                        Id_cobro = r.Id_cobro,
                        Nombre_casa = r.Nombre_casa,
                        Nombre_cliente = r.Nombre_cliente,
                        Periodo = r.Periodo,
                        Estado = r.Estado
                    }).ToList();
                }
            }

            return View(lista);
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

            //Lista para cargar los servicios al chkbox
            List<SelectListItem> servicios;
            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                servicios = db.Servicios
                              .Where(s => s.Estado == true)
                              .Select(s => new SelectListItem
                              {
                                  Value = s.IdServicio.ToString(),
                                  Text = s.Nombre
                              }).ToList();
            }

            ViewBag.Servicios = servicios;

            Cobros cobros = new Cobros();

            try
            {
                //mostramos la vista, se inserta un cobro
                using (var db = new PviProyectoFinalDB("MyDatabase"))
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
            catch (Exception ex)
            {

            }

            return View(cobros);
        }

        [HttpPost] //guarda la informacion
        public ActionResult Crear(Cobros cobros)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    CargarCombos();
                    return View(cobros);
                }
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    //Validacion de si existe un cobro para la casa y mes/año seleccionados
                    var existeCobro = db.SpValidarCobroExistente(cobros.IdCasa, cobros.mes, cobros.anno).FirstOrDefault();
                    if (existeCobro != null && existeCobro.Existe == 1)
                    {
                        ModelState.AddModelError("", "Ya existe un cobro para la casa, mes y año seleccionados.");
                        CargarCombos(); // Cargar los combos para volver a mostrar la vista
                        return View(cobros);
                    }
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
                        idUser: Convert.ToInt32(Session["id_persona"]),
                        serviciosSeleccionados: serviciosTable
                    );
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                CargarCombos();
                return View(cobros);
            }
        }
        private void CargarCombos()
        {
            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                ViewBag.Servicios = db.Servicios
                    .Where(s => s.Estado == true)
                    .Select(s => new SelectListItem
                    {
                        Value = s.IdServicio.ToString(),
                        Text = s.Nombre
                    }).ToList();
            }

            ViewBag.Años = Enumerable.Range(2024, 11)
            .Select(y => new SelectListItem { Value = y.ToString(), Text = y.ToString() })
            .ToList();
            ViewBag.Años.Insert(0, new SelectListItem { Value = "", Text = "Seleccione un año" });

            ViewBag.Meses = new List<SelectListItem>
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
        }
    }
}