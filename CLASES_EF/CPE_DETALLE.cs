using System;
namespace BusinessEntities
{
    //ˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆ
    //Creado por     : Jose Zambrano  (16/10/2017)
    //ˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆ
    // <summary> Entidad = CPE_DETALLE </summary> //
    public class CPE_DETALLE
    {
        public Nullable<int> ID_DETALLE { get; set; }
        public Nullable<int> ID_CABECERA { get; set; }
        public Nullable<int> ITEM { get; set; }
        public string UNIDAD_MEDIDA { get; set; }
        public Nullable<decimal> CANTIDAD { get; set; }
        public Nullable<decimal> PRECIO { get; set; }
        public Nullable<decimal> IMPORTE { get; set; }
        public Nullable<double> IMPUESTO_ICBPER { get; set; }
        public Nullable<int> CANTIDAD_BOLSAS { get; set; }
        public Nullable<double> SUNAT_ICBPER { get; set; }
        public string PRECIO_TIPO_CODIGO { get; set; }
        public Nullable<decimal> IGV { get; set; }
        //===================isc===================
        public Nullable<decimal> BI_ISC { get; set; }//BASE IMPONIBLE ISC UBL 2.1        
        public Nullable<decimal> POR_ISC { get; set; }//UBL 2.1
        public string TIPO_ISC { get; set; }//CATALOGO 8 SUNAT. UBL 2.1
        public Nullable<decimal> ISC { get; set; }
        //=========================================
        public string COD_TIPO_OPERACION { get; set; }//10=gravada, 20=exonerado,30=inafecto,40=exportacion, 31= gratuita QUE SIGNIFICA
        public string CODIGO { get; set; }
        public string CODIGO_SUNAT { get; set; }
        public string DESCRIPCION { get; set; }
        public Nullable<decimal> DESCUENTO { get; set; }
        public Nullable<decimal> SUB_TOTAL { get; set; }
        public Nullable<decimal> PRECIO_SIN_IMPUESTO { get; set; }
        /////////Campos Aumentados////////
        public Nullable<int> TIPO { get; set; }
        public string ID_USUARIO { get; set; }
    }
}