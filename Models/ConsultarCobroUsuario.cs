using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoFinalPogragamacionVI.Models
{
    public class ConsultarCobroUsuario
    {
        public int id_cobro { get; set; }
        public string nombre_casa { get; set; }
        public int mes { get; set; }
        public int anno { get; set; }
        public string estado { get; set; }
        public decimal monto { get; set; }
        public DateTime? fecha_pagada { get; set; }
    }
}