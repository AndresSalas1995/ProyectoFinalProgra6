using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoFinalPogragamacionVI.Models
{
    public class Casas
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int metros { get; set; }
        public int numHabitaciones { get; set; }
        public int numBanos { get; set; }
        public int idCliente { get; set; } // Indica si la casa está activa o no
        public DateTime FechaConstruccion { get; set; } // Fecha de construcción
    }
}