using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoFinalPogragamacionVI.Models
{
    public class DetalleCobro
    {
            public int IdCobro { get; set; }
            public string NombreCasa { get; set; }
            public string NombreCliente { get; set; }
            public int Mes { get; set; }
            public int Año { get; set; }
            public string Estado { get; set; }
            public DateTime? FechaCancelacion { get; set; }
            public decimal Monto { get; set; }
            public List<string> Servicios { get; set; }

    }
}