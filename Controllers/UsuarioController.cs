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
            int idPersona = (int)Session["id_persona"];
            List<SpConsultarCobrosUsuarioResult> lista;

            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                lista = db.SpConsultarCobrosUsuario(idPersona).ToList();
            }

            return View(lista);

        }
    }
}