using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataModels;
using ProyectoFinalPogragamacionVI.Models;

namespace ProyectoFinalPogragamacionVI.Controllers
{
    public class AutenticacionController : Controller
    {
        // GET: Autenticacion
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string Email, string Contrasena)
        {
            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                var resultado = db.SpLoginUsuario(Email, Contrasena).Select(r => new SpLoginUsuario_Result
                {
                    LoginExitoso = r.Login_exitoso,
                    id_persona = r.Id_persona,
                    nombre_completo = r.Nombre_completo,
                    es_empleado = r.Es_empleado
                }).FirstOrDefault();

                if (resultado != null && resultado.LoginExitoso == 1) 
                {
                    Session["id_persona"] = resultado.id_persona;
                    Session["nombre_completo"] = resultado.nombre_completo;
                    Session["es_empleado"] = resultado.es_empleado;

                    if (resultado.es_empleado.HasValue && resultado.es_empleado.Value == 1)
                    {
                        return RedirectToAction("Index", "Empleado");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Usuario");
                    }
                }
                else 
                {
                    ViewBag.Mensaje = "Contraseña o correo incorrecto, intente nuevamente.";
                    return View();
                }
            }
        }
        public ActionResult CerrarSession()
        {
            Session["usuario"] = null;
            Session.Clear();
            return RedirectToAction("Index", "Autenticacion");
        }
    }
}