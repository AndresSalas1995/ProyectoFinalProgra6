using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoFinalPogragamacionVI.Permisos
{
    public class AutorizarEmpleadoAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Si no está logueado, redirigir al login
            if (HttpContext.Current.Session["id_persona"] == null)
            {
                filterContext.Result = new RedirectResult("~/Autenticacion/Index");
                return;
            }

            // Si está logueado pero NO es empleado, redirigir a su área
            if (HttpContext.Current.Session["es_empleado"] == null || (int)HttpContext.Current.Session["es_empleado"] != 1)
            {
                // Redirigir a la vista de usuario
                filterContext.Result = new RedirectResult("~/Usuario/Index");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}