using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataModels;

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
                var resultado = db.SpLoginUsuario(Email, Contrasena).FirstOrDefault();

                if (resultado != null && resultado.Login_exitoso == 1) 
                {
                    Session["id_persona"] = resultado.Id_persona;
                    Session["nombre_completo"] = resultado.Nombre_completo;
                    Session["es_empleado"] = resultado.Es_empleado;

                    return RedirectToAction("Index", "Home");
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