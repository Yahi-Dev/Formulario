using SCCGasso.Core.Domain.Common;
using SCCGasso.Core.Domain.Enums;

namespace SCCGasso.Core.Domain.Entities
{
    public class Formulario : IAuditableEntity
    {
        public string? NombreComercial { get; set; }
        public string? RazonSocial { get; set; }
        public string? Direccion { get; set; }
        public string? Provincia { get; set; }
        public string? ClaseDeNegocio { get; set; }
        public string? RNC { get; set; }
        public decimal CapitalPagado { get; set; }
        public string? RegistroMercantilNo { get; set; }
        public DateTime? VencimientoRegistro { get; set; }
        public string? Telefono { get; set; }
        public string? Cedula { get; set; }
        public DateTime? Fundada { get; set; }
        public string? Municipio { get; set; }
        public string? Email { get; set; }
        public string? HabilitacionSaludPublica { get; set; }
        public DateTime? VencimientoHabilitacionSaludPublica { get; set; }
        public Motivos Motivo { get; set; }


        public decimal MontoSolicitadoGG { get; set; }
        public decimal MontoSolicitadoIQ { get; set; }
        public string? FirmaDigital { get; set; }



            //Para uso Interno solamente//
        public decimal MontoAprobadoGG { get; set; }//
        public decimal MontoAprobadoIQ { get; set; } //



        public string? CondicionesAprobadasGG { get; set; } //
        public string? CondicionesAprobadasIQ { get; set; } //



        public string? NombreVendedor { get; set; }
        public string? ZonaDeVenta { get; set; }
        public string? CodigoAprobacionNo { get; set; }
        public bool GG { get; set; } //
        public bool IQ { get; set; } //
        public bool DG { get; set; } //
        public string? Observaciones { get; set; } //


        //Navegation Properties
        //public int? IdPersonasAutorizadas { get; set; }
        public ICollection<PersonasAutorizadas>? PersonasAutorizadas { get; set; }

        //public int? IdCuentaBancaria { get; set; }
        public ICollection<CuentasBancarias>? CuentasBancarias { get; set; }

        //public int? IdReferenciasComercilaes { get; set; }
        public ICollection<ReferenciasComerciales>? ReferenciasComercilaes { get; set; }
    }
}
