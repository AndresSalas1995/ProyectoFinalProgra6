using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoFinalPogragamacionVI.Models
{
    public class DetalleCobroViewModel
    {
        public DataModels.PviProyectoFinalDBStoredProcedures.SpConsultarDetalleCobroResult Detalle { get; set; }
        public List<ServicioCobroViewModel> Servicios { get; set; }
        public List<BitacoraCobroViewModel> Bitacora { get; set; }
    }

    public class ServicioCobroViewModel
    {
        public string Nombre { get; set; }
        public bool Incluido { get; set; }
    }

    public class BitacoraCobroViewModel
    {
        public DateTime Fecha { get; set; }
        public string Accion { get; set; }
        public string Detalle { get; set; }
        public string RealizadoPor { get; set; }
    }
}