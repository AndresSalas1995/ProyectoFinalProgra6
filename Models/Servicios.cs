using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoFinalPogragamacionVI.Models
{
    public class Servicios
    {
        public int Id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public decimal precio { get; set; }
        public int categoriaId { get; set; }
        public string categoria { get; set; }
    }
}