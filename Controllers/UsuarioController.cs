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
    public class UsuarioController : Controller
    {
        [ValidarSession]
        // GET: Usuario
        public ActionResult Index()
        {
            int idPersona = (int)Session["id_persona"];
            List<SpConsultarReservacionesUsuarioResult> reservaciones;

            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                reservaciones = db.SpConsultarReservacionesUsuario(idPersona).ToList();
            }

            return View(reservaciones);

        }
    }
}