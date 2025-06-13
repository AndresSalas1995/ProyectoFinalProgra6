using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoFinalPogragamacionVI.Permisos
{
    public class ValidarSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Verificamos si la sesión NO contiene el id_persona (usuario no autenticado)
            if (HttpContext.Current.Session["id_persona"] == null)
            {
                // Redireccionar al login
                filterContext.Result = new RedirectResult("~/Autenticacion/Index");
            }

            base.OnActionExecuting(filterContext);
        }   
    }
}