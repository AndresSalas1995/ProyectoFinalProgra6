using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoFinalPogragamacionVI.Models
{
    public class SpLoginUsuario_Result
    {
        public int LoginExitoso { get; set; }
        public int? id_persona { get; set; }
        public string nombre_completo { get; set; }
        public int? es_empleado { get; set; }
    }
}