using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessEntities
{
    public class CPE_RESUMEN_BOLETA
    {
        public string NRO_DOCUMENTO_EMPRESA { get; set; }
        public string RAZON_SOCIAL { get; set; }
        public string TIPO_DOCUMENTO { get; set; }
        public string CODIGO { get; set; }
        public string SERIE { get; set; }
        public string SECUENCIA { get; set; }
        public string FECHA_REFERENCIA { get; set; }
        public string FECHA_DOCUMENTO { get; set; }
        public Nullable<int> TIPO_PROCESO { get; set; }
        public string TICKET { get; set; }
        public string USUARIO_SOL_EMPRESA { get; set; }
        public string PASS_SOL_EMPRESA { get; set; }
        public string CONTRA_FIRMA { get; set; }
        public string COD_RESPUESTA_SUNAT { get; set; }
        public string DESCRIPCION_RESPUESTA { get; set; }
        public string HASH_CPE { get; set; }
        public string HASH_CDR { get; set; }
        public string ESTADO { get; set; }
        public string RUTA_PFX { get; set; }
        /////////detalle////////
        public List<CPE_RESUMEN_BOLETA_DETALLE> detalle = new List<CPE_RESUMEN_BOLETA_DETALLE>();
    }
}
