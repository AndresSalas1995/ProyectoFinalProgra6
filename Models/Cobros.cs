using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoFinalPogragamacionVI.Models
{
    public class Cobros
    {
        public int Id_cobro { get; set; }
        public string Nombre_casa { get; set; }
        public string Nombre_cliente { get; set; }

        public int IdCliente { get; set; }
        public int IdCasa { get; set; }
        public int anno { get; set; }
        public int mes { get; set; }

        public string servicio { get; set; } //nombre del servicio
        public List<int> Servicios { get; set; }  //checkbox inputs
    }

}