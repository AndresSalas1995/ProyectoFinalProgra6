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

    public class EditarServiciosCobroViewModel
    {
        public int IdCobro { get; set; }

        public string NombreCasa { get; set; }
        public string NombreCliente { get; set; }
        public int Mes { get; set; }
        public int Año { get; set; }
        public decimal Monto { get; set; }

        public List<ServicioEditarViewModel> Servicios { get; set; }
    }

    public class ServicioEditarViewModel
    {
        public int IdServicio { get; set; }
        public string Nombre { get; set; }
        public bool Seleccionado { get; set; }
    }

}