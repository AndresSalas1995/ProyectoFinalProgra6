using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoFinalPogragamacionVI.Models
{
    public class ConsultarCobro
    {
        public int id_cobro { get; set; }
        public string nombre_casa { get; set; }
        public string nombre_cliente { get; set; }
        public string periodo { get; set; }
        public string estado { get; set; }
    }
}