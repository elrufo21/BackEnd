using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessEntities
{
    public class CPE_RESUMEN_BOLETA_DETALLE
    {
        public Nullable<int> ITEM { get; set; }
        public string TIPO_COMPROBANTE { get; set; }
        public string NRO_COMPROBANTE { get; set; }
        public string TIPO_DOCUMENTO { get; set; }
        public string NRO_DOCUMENTO { get; set; }
        public string TIPO_COMPROBANTE_REF { get; set; }
        public string NRO_COMPROBANTE_REF { get; set; }
        public string STATU { get; set; }
        public string COD_MONEDA { get; set; }
        public Nullable<decimal> TOTAL { get; set; }
        public Nullable<decimal> GRAVADA { get; set; }
        public Nullable<decimal> ISC { get; set; }
        public Nullable<decimal> IGV { get; set; }
        public Nullable<decimal> OTROS { get; set; }
        public Nullable<int> CARGO_X_ASIGNACION { get; set; }
        public Nullable<decimal> MONTO_CARGO_X_ASIG { get; set; }
        public string COD_TIPO_IMPORTE1 { get; set; }
        public Nullable<decimal> EXONERADO { get; set; }
        public string COD_TIPO_IMPORTE2 { get; set; }
        public Nullable<decimal> INAFECTO { get; set; }
        public string COD_TIPO_IMPORTE3 { get; set; }
        public Nullable<decimal> EXPORTACION { get; set; }
        public string COD_TIPO_IMPORTE4 { get; set; }
        public Nullable<decimal> GRATUITAS { get; set; }
        public string COD_TIPO_IMPORTE5 { get; set; }
        public string ESTADOS { get; set; }
        public Nullable<int> ID_VENTAS { get; set; }
        public Nullable<int> TIPO { get; set; }
        public Nullable<decimal> ICBPER { get; set; }
    }
}
