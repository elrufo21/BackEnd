using System;
using System.Collections.Generic;

namespace BusinessEntities
{
    //ˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆ
    //Creado por     : Jose Zambrano  (16/10/2017)
    //ˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆˆ
    // <summary> Entidad = CPE </summary> //
    public class CPE
    {
        public Nullable<int> ID { get; set; }
        public string TIPO_OPERACION { get; set; }//==============SEGUN  catalogo51, 
        public string HORA_REGISTRO { get; set; }
        public Nullable<int> ID_EMPRESA { get; set; }
        public Nullable<int> ID_CLIENTE_CPE { get; set; }
        public Nullable<decimal> TOTAL_GRAVADAS { get; set; }
        public Nullable<decimal> TOTAL_INAFECTA { get; set; }
        public Nullable<decimal> TOTAL_EXONERADAS { get; set; }
        public Nullable<decimal> TOTAL_GRATUITAS { get; set; }
        public Nullable<decimal> TOTAL_PERCEPCIONES { get; set; }
        public Nullable<decimal> TOTAL_RETENCIONES { get; set; }
        public Nullable<decimal> TOTAL_DETRACCIONES { get; set; }
        public Nullable<decimal> TOTAL_BONIFICACIONES { get; set; }
        public Nullable<decimal> TOTAL_DESCUENTO { get; set; }
        public Nullable<decimal> SUB_TOTAL { get; set; }
        public Nullable<decimal> POR_IGV { get; set; }//NUEVO UBL2.1
        public Nullable<decimal> TOTAL_IGV { get; set; }
        public Nullable<decimal> TOTAL_ISC { get; set; }
        public Nullable<decimal> TOTAL_EXPORTACION { get; set; }//NUEVO UBL2.1
        public Nullable<decimal> TOTAL_OTR_IMP { get; set; }
        public Nullable<decimal> TOTAL_ICBPER { get; set; }//IMPUESTO ALA BOLSA
        public Nullable<decimal> TOTAL { get; set; }
        public string TOTAL_LETRAS { get; set; }
        public string NRO_GUIA_REMISION { get; set; }
        public string FECHA_GUIA_REMISION { get; set; }//NUEVO UBL2.1
        public string COD_GUIA_REMISION { get; set; }
        public string NRO_OTR_COMPROBANTE { get; set; }
        public string COD_OTR_COMPROBANTE { get; set; }
        public string TIPO_COMPROBANTE_MODIFICA { get; set; }
        public string NRO_DOCUMENTO_MODIFICA { get; set; }
        public string COD_TIPO_MOTIVO { get; set; }
        public string DESCRIPCION_MOTIVO { get; set; }
        public string NRO_COMPROBANTE { get; set; }
        public string FECHA_DOCUMENTO { get; set; }
        public string COD_TIPO_DOCUMENTO { get; set; }
        public string COD_MONEDA { get; set; }
        public string NRO_DOCUMENTO_CLIENTE { get; set; }
        public string RAZON_SOCIAL_CLIENTE { get; set; }
        public string TIPO_DOCUMENTO_CLIENTE { get; set; }
        public string DIRECCION_CLIENTE { get; set; }
        public string CIUDAD_CLIENTE { get; set; }
        public string COD_PAIS_CLIENTE { get; set; }
        public string COD_UBIGEO_CLIENTE { get; set; }//NUEVO UBL2.1
        public string DEPARTAMENTO_CLIENTE { get; set; }//NUEVO UBL2.1
        public string PROVINCIA_CLIENTE { get; set; }//NUEVO UBL2.1
        public string DISTRITO_CLIENTE { get; set; }//NUEVO UBL2.1
        public string NRO_DOCUMENTO_EMPRESA { get; set; }
        public string TIPO_DOCUMENTO_EMPRESA { get; set; }
        public string NOMBRE_COMERCIAL_EMPRESA { get; set; }
        public string CODIGO_UBIGEO_EMPRESA { get; set; }
        public string DIRECCION_EMPRESA { get; set; }
        public string CONTACTO_EMPRESA { get; set; }//NUEVO UBL2.1
        public string DEPARTAMENTO_EMPRESA { get; set; }
        public string PROVINCIA_EMPRESA { get; set; }
        public string DISTRITO_EMPRESA { get; set; }
        public string CODIGO_PAIS_EMPRESA { get; set; }
        public string RAZON_SOCIAL_EMPRESA { get; set; }
        public string USUARIO_SOL_EMPRESA { get; set; }
        public string PASS_SOL_EMPRESA { get; set; }
        public string CONTRA_FIRMA { get; set; }
        public Nullable<int> TIPO_PROCESO { get; set; }
        public string COD_RESPUESTA_SUNAT { get; set; }
        public string DESCRIPCION_RESPUESTA { get; set; }
        public string PLACA_VEHICULO { get; set; }
        public Nullable<int> FLG_ANTICIPO { get; set; }
        public Nullable<int> FLG_REGU_ANTICIPO { get; set; }
        public string NRO_COMPROBANTE_REF_ANT { get; set; }
        public string MONEDA_REGU_ANTICIPO { get; set; }
        public Nullable<decimal> MONTO_REGU_ANTICIPO { get; set; }
        public string TIPO_DOCUMENTO_EMP_REGU_ANT { get; set; }
        public string NRO_DOCUMENTO_EMP_REGU_ANT { get; set; }
        public string ESTADO { get; set; }
        public string HASH_CPE { get; set; }
        public string HASH_CDR { get; set; }
        /////////Campos Aumentados////////
        public Nullable<int> TIPO { get; set; }
        public string ID_USUARIO { get; set; }
        /////////DATA EXTRA////////
        public string FECHA_VTO { get; set; }
        public string TELEFONO_PRINCIPAL { get; set; }
        public string COD_SUCURSAL { get; set; }
        public string DIRECCION_SUCURSAL { get; set; }
        public string TELEFONO_SUCURSAL { get; set; }
        public string FORMA_PAGO { get; set; }
        public Nullable<int> ID_FORMA_PAGO { get; set; }
        public Nullable<int> ID_ALMACEN { get; set; }
        public string GLOSA { get; set; }
        public string SERIE { get; set; }
        public string NUMERO { get; set; }
        public Nullable<int> ID_REFERENCIA { get; set; }
        //=================rutas===================
        public string RUTA_PFX { get; set; }
        public string CODIGO_ANEXO { get; set; }
        public string CUENTA_DETRACCION { get; set; }
        public Nullable<decimal> MONTO_DETRACCION { get; set; }
        public Nullable<decimal> PENDIENTE_PAGO { get; set; }
        public Nullable<decimal> PORCENTAJE_DES { get; set; }
        /////////detalle////////
        public List<CPE_DETALLE> detalle = new List<CPE_DETALLE>();
    }
}