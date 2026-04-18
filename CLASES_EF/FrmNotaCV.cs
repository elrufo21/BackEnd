using System;
using System.Data;
using System.Windows.Forms;
using MegaRosita.Capa.Comun;
using MegaRosita.Capa.Logica;
using System.Diagnostics;
using BE = BusinessEntities;
using System.Collections.Generic;
using ZXing;
using TEXTO = iTextSharp.text;
using PDFX = iTextSharp.text.pdf;
using System.IO;
using System.Drawing;
namespace MegaRosita.Capa.Aplicacion
{
    public partial class FrmNotaCV : Form
    {
        BE.CPE_DETALLE objCPE_DETALLE = new BE.CPE_DETALLE();
        CPEConfig objF = new CPEConfig();
        BE.CPE objCPE = new BE.CPE();
        LogDocumento objdocu = new LogDocumento();
        Mensajes men = new Mensajes();
        Conexion xconexion = new Conexion();
        private DataTable Tabla;
        private DataTable TablaA;
        private DataView vista;
        private DataView vistaA;
        BindingSource bs;
        BindingSource bsA;
        string[] listas;
        public string xPersonal { get; set; }
        public string xRucPrincipal { get; set; }
        //========FIRMA DE COMPANIA========
        public string xComercial = string.Empty;
        public string xRuc = string.Empty;
        public string xUsuarioSol = string.Empty;
        public string xClaveSol = string.Empty;
        public string xPFX = string.Empty;
        public string xPFXClave = string.Empty;
        public string xEmail = string.Empty;
        public string xDireccion = string.Empty;
        public string xTelefono = string.Empty;
        public string xNombreUBG = string.Empty;
        public string xCodigoUBG = string.Empty;
        public string xDistrito = string.Empty;
        public string xDirecSunat = string.Empty;
        //=====================================
        float tableHeight = 0;
        string xRucCompania = string.Empty;
        int NROPROCESO;
        public double xIMP_BOLSA { get; set; }
        string xConceptoOBS { get; set;}
        string xEntidad { get; set; }
        string xEfectivo { get; set;}
        string xDeposito { get; set;}
        //======DATOS PARA ENVIAR PENDIENTES
        public string pDocuId { get; set;}
        public string pNotaId { get; set; }
        public string pFechaEmision { get; set;}
        public string pNumero { get; set;}
        public string pSerie { get; set; }
        public int xAVISO {get; set;}
        //===================================
        public FrmNotaCV()
        {
            InitializeComponent();
            this.metroTabControl1.SelectedIndex = 0;
            txtserie.Text = "";
            lblasociado.Text = "";
            lblidcliente.Text = "";
            lblidNota.Text = "";
            lblidDocu.Text = "";
            lbltotalT.Text = "0.00";
            cmdConcepto.Text = "ANULACION DE LA OPERACION";
            NROPROCESO = xconexion.TIPO_PROCESO;
        }
        #region
        public void eliminarArchivos()
        {
            string temXML, temRXML, temZIP, temRZIP, temPDF = string.Empty;
            temXML = "\\\\" + xconexion.ServidorIP + "\\ArchivoSistema\\CPE\\PRODUCCION\\" + xRuc + "-07-" + txtserie.Text + "-" + txtnumero.Text + ".XML";
            temRXML = "\\\\" + xconexion.ServidorIP + "\\ArchivoSistema\\CPE\\PRODUCCION\\" + "R-" + xRuc + "-07-" + txtserie.Text + "-" + txtnumero.Text + ".XML";
            temZIP = "\\\\" + xconexion.ServidorIP + "\\ArchivoSistema\\CPE\\PRODUCCION\\" + xRuc + "-07-" + txtserie.Text + "-" + txtnumero.Text + ".ZIP";
            temRZIP = "\\\\" + xconexion.ServidorIP + "\\ArchivoSistema\\CPE\\PRODUCCION\\" + "R-" + xRuc + "-07-" + txtserie.Text + "-" + txtnumero.Text + ".ZIP";
            temPDF = "\\\\" + xconexion.ServidorIP + "\\ArchivoSistema\\CPE\\PRODUCCION\\" + xRuc + "-07-" + txtserie.Text + "-" + txtnumero.Text + ".PDF";
            File.Delete(temXML);
            File.Delete(temRXML);
            File.Delete(temZIP);
            File.Delete(temRZIP);
            File.Delete(temPDF);
        }
        public void enviarSunat()
        {
            eliminarArchivos();
            Stopwatch oReloj = new Stopwatch();
            oReloj.Start();
            try
            {
                if (txtruc.Text.Contains("20522109178")) objCPE.TIPO_OPERACION = "1001";
                else objCPE.TIPO_OPERACION = "0101";
                objCPE.TOTAL_GRAVADAS = Convert.ToDecimal(lbloperacion.Text);//SUB TOTAL SUMATORIA DEL IGV DETALLE
                objCPE.SUB_TOTAL = Convert.ToDecimal(lblsubtotal.Text);//SUB TOTAL - DESCUENTO
                objCPE.POR_IGV = 18; // NUEVO UBL 2.1
                objCPE.TOTAL_IGV = Convert.ToDecimal(lbligv.Text); // TOTAL IGV
                objCPE.TOTAL_ISC = 0;
                objCPE.TOTAL_OTR_IMP = 0;
                objCPE.TOTAL_DESCUENTO = decimal.Parse(lbldescuento.Text);
                objCPE.TOTAL_ICBPER = Convert.ToDecimal(lblICBPER.Text);//TOTAL ICBPER
                objCPE.TOTAL = Convert.ToDecimal(lbltotal.Text);  // TOTAL COMPROBANTE
                objCPE.TOTAL_EXPORTACION = 0; // NUEVO UBL 2.1
                objCPE.TOTAL_LETRAS = txtletras.Text;
                objCPE.NRO_GUIA_REMISION = "";
                objCPE.FECHA_GUIA_REMISION = ""; // NUEVO UBL 2.1
                objCPE.COD_GUIA_REMISION = "";
                objCPE.NRO_OTR_COMPROBANTE = "";
                objCPE.COD_OTR_COMPROBANTE = "";
                objCPE.NRO_COMPROBANTE = txtserie.Text + "-" + txtnumero.Text;
                objCPE.FECHA_DOCUMENTO = dtimeemision.Value.ToString("yyyy-MM-dd"); //DateTime.Now.ToString("yyyy-MM-dd"); // "2018-01-18"
                objCPE.FECHA_VTO = dtimeemision.Value.ToString("yyyy-MM-dd");
                objCPE.HORA_REGISTRO = DateTime.Now.ToString("HH:mm:ss");//Hora Emision
                objCPE.COD_TIPO_DOCUMENTO = "07";  //ARAMIREZ // 01=FACTURA, 03=BOLETA, 07=NOTA CREDITO, 08=NOTA DEBITO
                objCPE.COD_MONEDA = "PEN";
                // ==============PARA PLAA DE VEHICULO=============
                objCPE.PLACA_VEHICULO = "";
                // ========================DATOS NOTA CREDITO/NOTA DEBITO==========================
                if (txtdocumento.Text == "FACTURA") objCPE.TIPO_COMPROBANTE_MODIFICA = "01";
                else objCPE.TIPO_COMPROBANTE_MODIFICA = "03";
                objCPE.NRO_DOCUMENTO_MODIFICA = txtnroDocu.Text;
                objCPE.COD_TIPO_MOTIVO = cmdConcepto.SelectedValue.ToString();
                objCPE.DESCRIPCION_MOTIVO = cmdConcepto.Text;
                // ========================DATOS DEL CIENTE==========================
                if (txtdocumento.Text == "FACTURA")
                {
                    objCPE.NRO_DOCUMENTO_CLIENTE = txtruc.Text;
                    objCPE.RAZON_SOCIAL_CLIENTE = txtcliente.Text;
                    objCPE.TIPO_DOCUMENTO_CLIENTE = "6";   // 1=DNI,6=RUC
                }
                else
                {
                    objCPE.NRO_DOCUMENTO_CLIENTE = txtdni.Text;
                    objCPE.RAZON_SOCIAL_CLIENTE = txtcliente.Text;
                    objCPE.TIPO_DOCUMENTO_CLIENTE = "1";
                }
                objCPE.DIRECCION_CLIENTE = txtfiscal.Text;
                objCPE.CIUDAD_CLIENTE = xconexion.xDepartamento;
                objCPE.COD_PAIS_CLIENTE = "PE";
                objCPE.COD_UBIGEO_CLIENTE = ""; // //NUEVO UBL2.1
                objCPE.DEPARTAMENTO_CLIENTE = ""; // //NUEVO UBL2.1
                objCPE.PROVINCIA_CLIENTE = ""; // //NUEVO UBL2.1
                objCPE.DISTRITO_CLIENTE = ""; // //NUEVO UBL2.1
                // =============================DATOS EMPRESA===========================
                objCPE.NRO_DOCUMENTO_EMPRESA = xRuc;// "10447915125";
                objCPE.TIPO_DOCUMENTO_EMPRESA = "6"; // 1=DNI,6=RUC
                objCPE.NOMBRE_COMERCIAL_EMPRESA = xComercial;
                objCPE.CODIGO_UBIGEO_EMPRESA = xCodigoUBG; //"150106";
                objCPE.CODIGO_ANEXO = xconexion.xANEXO;
                objCPE.DIRECCION_EMPRESA = xDirecSunat;
                objCPE.DEPARTAMENTO_EMPRESA =xconexion.xDepartamento;
                objCPE.PROVINCIA_EMPRESA =xconexion.xProvincia;
                objCPE.DISTRITO_EMPRESA = xDistrito;
                objCPE.CODIGO_PAIS_EMPRESA = "PE";
                objCPE.RAZON_SOCIAL_EMPRESA = cmdcompania.Text;
                objCPE.CONTACTO_EMPRESA = ""; // NUEVO UBL 2.1
                objCPE.USUARIO_SOL_EMPRESA = xUsuarioSol;
                objCPE.PASS_SOL_EMPRESA = xClaveSol;
                objCPE.CONTRA_FIRMA = xPFXClave;
                objCPE.TIPO_PROCESO = NROPROCESO; // 1=PRODUCCION, 2=HOMOLOGACION, 3=BETA
                objCPE.RUTA_PFX = xPFX;
                string vFormaPago = string.Empty;
                if (txtcondicion.Text.Contains("ALCONTADO") || txtcondicion.Text.Contains("PAGO/VARIOS")) vFormaPago = "Contado";
                else vFormaPago = "Credito";
                objCPE.FORMA_PAGO = vFormaPago;
                List<BusinessEntities.CPE_DETALLE> OBJCPE_DETALLE_LIST = new List<BusinessEntities.CPE_DETALLE>();
                for (int i = 0; i <= gvconcepto.Rows.Count - 1; i++)
                {
                    objCPE_DETALLE = new BusinessEntities.CPE_DETALLE();
                    objCPE_DETALLE.ITEM = i + 1;
                    if (txtruc.Text.Contains("20522109178")) objCPE_DETALLE.UNIDAD_MEDIDA = "ZZ";
                    else objCPE_DETALLE.UNIDAD_MEDIDA = "NIU";
                    objCPE_DETALLE.CANTIDAD = Convert.ToDecimal(gvconcepto.Rows[i].Cells[0].Value.ToString());
                    if (Convert.ToString(gvconcepto.Rows[i].Cells[12].Value).Equals("BOLSAS PLASTICAS"))
                    {
                        objCPE_DETALLE.IMPUESTO_ICBPER = Convert.ToDouble(gvconcepto.Rows[i].Cells[0].Value) * xIMP_BOLSA;
                        objCPE_DETALLE.CANTIDAD_BOLSAS = Convert.ToInt32(objCPE_DETALLE.CANTIDAD);
                        objCPE_DETALLE.SUNAT_ICBPER = xIMP_BOLSA;
                    }
                    else
                    {
                        objCPE_DETALLE.IMPUESTO_ICBPER = 0;
                        objCPE_DETALLE.CANTIDAD_BOLSAS = 0;
                        objCPE_DETALLE.SUNAT_ICBPER = xIMP_BOLSA;
                    }
                    objCPE_DETALLE.PRECIO_TIPO_CODIGO = "01";
                    objCPE_DETALLE.PRECIO = Convert.ToDecimal(gvconcepto.Rows[i].Cells[3].Value.ToString());//8
                    objCPE_DETALLE.IMPORTE = Math.Round(Convert.ToDecimal(gvconcepto.Rows[i].Cells[10].Value.ToString()), 2);
                    objCPE_DETALLE.IGV = Math.Round(Convert.ToDecimal(gvconcepto.Rows[i].Cells[9].Value.ToString()), 2);
                    objCPE_DETALLE.ISC = 0;
                    objCPE_DETALLE.COD_TIPO_OPERACION = "10";
                    objCPE_DETALLE.CODIGO = gvconcepto.Rows[i].Cells[11].Value.ToString();
                    objCPE_DETALLE.CODIGO_SUNAT = gvconcepto.Rows[i].Cells[13].Value.ToString();
                    objCPE_DETALLE.DESCRIPCION = gvconcepto.Rows[i].Cells[2].Value.ToString();
                    objCPE_DETALLE.SUB_TOTAL = objCPE_DETALLE.IMPORTE;
                    objCPE_DETALLE.PRECIO_SIN_IMPUESTO = Math.Round(Convert.ToDecimal(gvconcepto.Rows[i].Cells[8].Value.ToString()),6);
                    OBJCPE_DETALLE_LIST.Add(objCPE_DETALLE);
                }
                objCPE.detalle = OBJCPE_DETALLE_LIST;
                Dictionary<string, string> dictionaryEnv = new Dictionary<string, string>();
                dictionaryEnv = objF.Envio(objCPE);
                TXTCOD_SUNAT.Text = dictionaryEnv["cod_sunat"];
                TXT_MSJ_SUNAT.Text = dictionaryEnv["msj_sunat"];
                TXTHASHCPE.Text = dictionaryEnv["hash_cpe"];
                TXTHASHCDR.Text = dictionaryEnv["hash_cdr"];
                if (TXTHASHCPE.Text.Length == 0)
                {
                    MessageBox.Show("Error en el servidor SUNAT...Verificar los datos y volver al intentarlo en unos segundos", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    TXTCOD_SUNAT.Text = "";
                    TXT_MSJ_SUNAT.Text = "";
                    TXTHASHCPE.Text = "";
                    TXTHASHCDR.Text = "";
                }
                else
                {
                    if (TXTCOD_SUNAT.Text.Contains("soap:Server"))
                    {
                        MessageBox.Show(TXTCOD_SUNAT.Text + "- CODIGO DE ERROR NUBEFACT: "+ TXT_MSJ_SUNAT.Text, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (TXTCOD_SUNAT.Text.Length >= 1)
                        {
                            if (TXTCOD_SUNAT.Text == "0")
                            {
                                concatenar("ENVIADO");
                            }
                            else
                            {
                                if (int.Parse(TXTCOD_SUNAT.Text) >= 100 && int.Parse(TXTCOD_SUNAT.Text) <= 500)
                                {
                                    concatenar("PENDIENTE");
                                }
                                else
                                {
                                    if (TXTCOD_SUNAT.Text == "2022")
                                    {
                                        MessageBox.Show(TXTCOD_SUNAT.Text + "-" + TXT_MSJ_SUNAT.Text, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        txtruc.Focus();
                                    }
                                    else if (TXTCOD_SUNAT.Text == "2325")
                                    {
                                        MessageBox.Show(TXTCOD_SUNAT.Text + "-" + TXT_MSJ_SUNAT.Text, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        txtruc.Focus();
                                    }
                                    else if (TXTCOD_SUNAT.Text == "1033")
                                    {
                                        MessageBox.Show(TXTCOD_SUNAT.Text + "-" + TXT_MSJ_SUNAT.Text, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        txtruc.Focus();
                                    }
                                    else
                                    {
                                        if (int.Parse(TXTCOD_SUNAT.Text) >= 2000 && int.Parse(TXTCOD_SUNAT.Text) <= 3999)
                                        {
                                            MessageBox.Show(TXTCOD_SUNAT.Text + "-" + TXT_MSJ_SUNAT.Text, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            txtruc.Focus();
                                        }
                                        else
                                        {
                                            MessageBox.Show(TXTCOD_SUNAT.Text + "-" + TXT_MSJ_SUNAT.Text, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            txtruc.Focus();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            concatenar("PENDIENTE");
                        }
                    }
                }
                oReloj.Stop();
                this.Text = ".:.TIEMPO DE RESPUESTA:  " + oReloj.Elapsed.TotalMilliseconds.ToString("N2") + " MS";
            }
            catch (Exception ex)
            {
                MessageBox.Show(TXTCOD_SUNAT.Text + "-" + TXT_MSJ_SUNAT.Text + "-" + ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                oReloj.Stop();
                this.Text = ".:.TIEMPO DE RESPUESTA:  " + oReloj.Elapsed.TotalMilliseconds.ToString("N2") + " MS";
            }
        }
        //ENVIAR NOTA DE CREDITO SEGUNDA VEZ
        public void enviarSunatB()
        {
            eliminarArchivos();
            Stopwatch oReloj = new Stopwatch();
            oReloj.Start();
            try
            {
                if (txtruc.Text.Contains("20522109178")) objCPE.TIPO_OPERACION = "1001";
                else objCPE.TIPO_OPERACION = "0101";
                objCPE.TOTAL_GRAVADAS = Convert.ToDecimal(lbloperacion.Text);//SUB TOTAL SUMATORIA DEL IGV DETALLE
                objCPE.SUB_TOTAL = Convert.ToDecimal(lblsubtotal.Text);//SUB TOTAL - DESCUENTO
                objCPE.POR_IGV = 18; // NUEVO UBL 2.1
                objCPE.TOTAL_IGV = Convert.ToDecimal(lbligv.Text); // TOTAL IGV
                objCPE.TOTAL_ISC = 0;
                objCPE.TOTAL_OTR_IMP = 0;
                objCPE.TOTAL_DESCUENTO = decimal.Parse(lbldescuento.Text);
                objCPE.TOTAL_ICBPER = Convert.ToDecimal(lblICBPER.Text);//TOTAL ICBPER
                objCPE.TOTAL = Convert.ToDecimal(lbltotal.Text);  // TOTAL COMPROBANTE
                objCPE.TOTAL_EXPORTACION = 0; // NUEVO UBL 2.1
                objCPE.TOTAL_LETRAS = txtletras.Text;
                objCPE.NRO_GUIA_REMISION = "";
                objCPE.FECHA_GUIA_REMISION = ""; // NUEVO UBL 2.1
                objCPE.COD_GUIA_REMISION = "";
                objCPE.NRO_OTR_COMPROBANTE = "";
                objCPE.COD_OTR_COMPROBANTE = "";
                objCPE.NRO_COMPROBANTE = txtserie.Text + "-" + txtnumero.Text;
                objCPE.FECHA_DOCUMENTO = dtimeemision.Value.ToString("yyyy-MM-dd"); //DateTime.Now.ToString("yyyy-MM-dd"); // "2018-01-18"
                objCPE.FECHA_VTO = dtimeemision.Value.ToString("yyyy-MM-dd");//DateTime.Now.ToString("yyyy-MM-dd");// NUEVO UBL 2.1
                objCPE.HORA_REGISTRO = DateTime.Now.ToString("HH:mm:ss");//Hora Emision
                objCPE.COD_TIPO_DOCUMENTO = "07";  // 01=FACTURA, 03=BOLETA, 07=NOTA CREDITO, 08=NOTA DEBITO
                objCPE.COD_MONEDA = "PEN";
                // ==============PARA PLAA DE VEHICULO=============
                objCPE.PLACA_VEHICULO = "";
                // ========================DATOS NOTA CREDITO/NOTA DEBITO==========================
                if (txtdocumento.Text == "FACTURA") objCPE.TIPO_COMPROBANTE_MODIFICA = "01";
                else objCPE.TIPO_COMPROBANTE_MODIFICA = "03";
                objCPE.NRO_DOCUMENTO_MODIFICA = txtnroDocu.Text;
                objCPE.COD_TIPO_MOTIVO = cmdConcepto.SelectedValue.ToString();
                objCPE.DESCRIPCION_MOTIVO = cmdConcepto.Text;
                // ========================DATOS DEL CIENTE==========================
                if (txtdocumento.Text == "FACTURA")
                {
                    objCPE.NRO_DOCUMENTO_CLIENTE = txtruc.Text;
                    objCPE.RAZON_SOCIAL_CLIENTE = txtcliente.Text;
                    objCPE.TIPO_DOCUMENTO_CLIENTE = "6";   // 1=DNI,6=RUC
                }
                else
                {
                    objCPE.NRO_DOCUMENTO_CLIENTE = txtdni.Text;
                    objCPE.RAZON_SOCIAL_CLIENTE = txtcliente.Text;
                    objCPE.TIPO_DOCUMENTO_CLIENTE = "1";
                }
                objCPE.DIRECCION_CLIENTE = txtfiscal.Text;
                objCPE.CIUDAD_CLIENTE = xconexion.xDepartamento;
                objCPE.COD_PAIS_CLIENTE = "PE";
                objCPE.COD_UBIGEO_CLIENTE = ""; // //NUEVO UBL2.1
                objCPE.DEPARTAMENTO_CLIENTE = ""; // //NUEVO UBL2.1
                objCPE.PROVINCIA_CLIENTE = ""; // //NUEVO UBL2.1
                objCPE.DISTRITO_CLIENTE = ""; // //NUEVO UBL2.1
                // =============================DATOS EMPRESA===========================
                objCPE.NRO_DOCUMENTO_EMPRESA = xRuc;// "10447915125";
                objCPE.TIPO_DOCUMENTO_EMPRESA = "6"; // 1=DNI,6=RUC
                objCPE.NOMBRE_COMERCIAL_EMPRESA = xComercial;
                objCPE.CODIGO_UBIGEO_EMPRESA = xCodigoUBG; //"150106";
                objCPE.CODIGO_ANEXO = xconexion.xANEXO;
                objCPE.DIRECCION_EMPRESA = xDirecSunat;
                objCPE.DEPARTAMENTO_EMPRESA = xconexion.xDepartamento;
                objCPE.PROVINCIA_EMPRESA = xconexion.xProvincia;
                objCPE.DISTRITO_EMPRESA = xDistrito;
                objCPE.CODIGO_PAIS_EMPRESA = "PE";
                objCPE.RAZON_SOCIAL_EMPRESA = cmdcompania.Text;
                objCPE.CONTACTO_EMPRESA = ""; // NUEVO UBL 2.1
                objCPE.USUARIO_SOL_EMPRESA = xUsuarioSol;
                objCPE.PASS_SOL_EMPRESA = xClaveSol;
                objCPE.CONTRA_FIRMA = xPFXClave;
                objCPE.TIPO_PROCESO = NROPROCESO; // 1=PRODUCCION, 2=HOMOLOGACION, 3=BETA
                objCPE.RUTA_PFX = xPFX;
                string vFormaPago = string.Empty;
                if (txtcondicion.Text.Contains("ALCONTADO") || txtcondicion.Text.Contains("PAGO/VARIOS")) vFormaPago = "Contado";
                else vFormaPago = "Credito";
                objCPE.FORMA_PAGO = vFormaPago;
                List<BusinessEntities.CPE_DETALLE> OBJCPE_DETALLE_LIST = new List<BusinessEntities.CPE_DETALLE>();
                for (int i = 0; i <= gvconcepto.Rows.Count - 1; i++)
                {
                    objCPE_DETALLE = new BusinessEntities.CPE_DETALLE();
                    objCPE_DETALLE.ITEM = i + 1;
                    if (txtruc.Text.Contains("20522109178")) objCPE_DETALLE.UNIDAD_MEDIDA = "ZZ";
                    else objCPE_DETALLE.UNIDAD_MEDIDA = "NIU";
                    objCPE_DETALLE.CANTIDAD = Convert.ToDecimal(gvconcepto.Rows[i].Cells[0].Value.ToString());
                    if (Convert.ToString(gvconcepto.Rows[i].Cells[12].Value).Equals("BOLSAS PLASTICAS"))
                    {
                        objCPE_DETALLE.IMPUESTO_ICBPER = Convert.ToDouble(gvconcepto.Rows[i].Cells[0].Value) * xIMP_BOLSA;
                        objCPE_DETALLE.CANTIDAD_BOLSAS = Convert.ToInt32(objCPE_DETALLE.CANTIDAD);
                        objCPE_DETALLE.SUNAT_ICBPER = xIMP_BOLSA;
                    }
                    else
                    {
                        objCPE_DETALLE.IMPUESTO_ICBPER = 0;
                        objCPE_DETALLE.CANTIDAD_BOLSAS = 0;
                        objCPE_DETALLE.SUNAT_ICBPER = xIMP_BOLSA;
                    }
                    objCPE_DETALLE.PRECIO_TIPO_CODIGO = "01";
                    objCPE_DETALLE.PRECIO = Convert.ToDecimal(gvconcepto.Rows[i].Cells[3].Value.ToString());//8
                    objCPE_DETALLE.IMPORTE = Math.Round(Convert.ToDecimal(gvconcepto.Rows[i].Cells[10].Value.ToString()), 2);
                    objCPE_DETALLE.IGV = Math.Round(Convert.ToDecimal(gvconcepto.Rows[i].Cells[9].Value.ToString()), 2);
                    objCPE_DETALLE.ISC = 0;
                    objCPE_DETALLE.COD_TIPO_OPERACION = "10";
                    objCPE_DETALLE.CODIGO = gvconcepto.Rows[i].Cells[11].Value.ToString();
                    objCPE_DETALLE.CODIGO_SUNAT = gvconcepto.Rows[i].Cells[13].Value.ToString();
                    objCPE_DETALLE.DESCRIPCION = gvconcepto.Rows[i].Cells[2].Value.ToString();
                    objCPE_DETALLE.SUB_TOTAL = objCPE_DETALLE.IMPORTE;
                    objCPE_DETALLE.PRECIO_SIN_IMPUESTO = Math.Round(Convert.ToDecimal(gvconcepto.Rows[i].Cells[8].Value.ToString()),6);
                    OBJCPE_DETALLE_LIST.Add(objCPE_DETALLE);
                }
                objCPE.detalle = OBJCPE_DETALLE_LIST;
                Dictionary<string, string> dictionaryEnv = new Dictionary<string, string>();
                dictionaryEnv = objF.Envio(objCPE);
                TXTCOD_SUNAT.Text = dictionaryEnv["cod_sunat"];
                TXT_MSJ_SUNAT.Text = dictionaryEnv["msj_sunat"];
                TXTHASHCPE.Text = dictionaryEnv["hash_cpe"];
                TXTHASHCDR.Text = dictionaryEnv["hash_cdr"];
                if (TXTHASHCPE.Text.Length == 0)
                {
                    MessageBox.Show("Error en el servidor SUNAT...Verificar los datos y volver al intentarlo en unos segundos", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    TXTCOD_SUNAT.Text = "";
                    TXT_MSJ_SUNAT.Text = "";
                    TXTHASHCPE.Text = "";
                    TXTHASHCDR.Text = "";
                }
                else
                {
                    if (TXTCOD_SUNAT.Text.Length >= 1)
                    {
                        if (TXTCOD_SUNAT.Text == "0")
                        {
                            reEnviarNota();
                        }
                        else
                        {
                            if (TXTCOD_SUNAT.Text == "1033")
                            {
                                reEnviarNota();
                            }
                            else if (TXTCOD_SUNAT.Text == "2022")
                            {
                                MessageBox.Show(TXTCOD_SUNAT.Text + "-" + TXT_MSJ_SUNAT.Text, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtruc.Focus();
                            }
                            else if (TXTCOD_SUNAT.Text == "2325")
                            {
                                MessageBox.Show(TXTCOD_SUNAT.Text + "-" + TXT_MSJ_SUNAT.Text, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtruc.Focus();
                            }
                            else
                            {
                                if (int.Parse(TXTCOD_SUNAT.Text) >= 2000 && int.Parse(TXTCOD_SUNAT.Text) <= 3999)
                                {
                                    MessageBox.Show(TXTCOD_SUNAT.Text + "-" + TXT_MSJ_SUNAT.Text, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    txtruc.Focus();
                                }
                                else
                                {
                                    MessageBox.Show(TXTCOD_SUNAT.Text + "-" + TXT_MSJ_SUNAT.Text, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    txtruc.Focus();
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("AUN NO SE OBTIENE EL CDR DE SUNAT...FAVOR DE INTENTAR DE NUEVO EN MINUTOS.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                oReloj.Stop();
                this.Text = ".:.TIEMPO DE RESPUESTA:  " + oReloj.Elapsed.TotalMilliseconds.ToString("N2") + " MS";
            }
            catch (Exception ex)
            {
                MessageBox.Show(TXTCOD_SUNAT.Text + "-" + TXT_MSJ_SUNAT.Text + "-" + ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                oReloj.Stop();
                this.Text = ".:.TIEMPO DE RESPUESTA:  " + oReloj.Elapsed.TotalMilliseconds.ToString("N2") + " MS";
            }
        }
        ///
        #endregion
        #region EXPORTARPDF
        private void exportarPDF()
        {
            TEXTO.Document doc = new TEXTO.Document(TEXTO.PageSize.A4, 10, 10, 10, 10);
            string filename, xarchivo = string.Empty;
            xarchivo = String.Format("{0}-{1}-{2}-{3}.PDF",xRucPrincipal ,"07", txtserie.Text, txtnumero.Text);
            filename = "\\\\" + xconexion.ServidorIP + "\\ArchivoSistema\\CPE\\PRODUCCION\\" + xarchivo;
            if (filename.Trim() != "")
            {
                FileStream file = new FileStream(filename,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.ReadWrite);
                PDFX.PdfWriter writer = PDFX.PdfWriter.GetInstance(doc, file);
                doc.Open();
                string xnrodocu, xfecha, xcliente, xfiscal,
                    xdni, xruta, xmoneda = string.Empty;
                xnrodocu = txtserie.Text + "-" + txtnumero.Text;
                xfecha = " Fecha de Emision:           " + dtimeemision.Text;
                xcliente = " Señor(es):                        " + txtcliente.Text;
                xdni = " R.U.C:                              " + txtruc.Text;
                xfiscal = " Direccion:                         " + txtfiscal.Text;
                xmoneda = " Moneda:                           SOLES";
                xruta = @"D:\mp3\NC_HUARAL.png";//ARAMIREZ NC_HUARAL.png
                TEXTO.Image imagen = TEXTO.Image.GetInstance(xruta);
                imagen.BorderWidth = 0;
                imagen.SetAbsolutePosition(415f, 755f);
                imagen.ScalePercent(22);
                doc.Add(imagen);
                TEXTO.Chunk chunk = new TEXTO.Chunk(xconexion.NombreComercialPDF, TEXTO.FontFactory.GetFont("Calibri", 14, TEXTO.Font.BOLD));
                doc.Add(new TEXTO.Paragraph(chunk));
                doc.Add(new TEXTO.Paragraph("  De: " + cmdcompania.Text, TEXTO.FontFactory.GetFont("Calibri", 9)));
                doc.Add(new TEXTO.Paragraph(xconexion.DireccionPDF, TEXTO.FontFactory.GetFont("Calibri", 9)));
                doc.Add(new TEXTO.Paragraph("                                                                                                                                      " + xnrodocu, TEXTO.FontFactory.GetFont("Calibri", 12, TEXTO.Font.BOLD)));
                doc.Add(new TEXTO.Paragraph(xfecha, TEXTO.FontFactory.GetFont("Calibri", 9)));
                doc.Add(new TEXTO.Paragraph(xcliente, TEXTO.FontFactory.GetFont("Calibri", 9)));
                doc.Add(new TEXTO.Paragraph(xdni, TEXTO.FontFactory.GetFont("Calibri", 9)));
                doc.Add(new TEXTO.Paragraph(xfiscal, TEXTO.FontFactory.GetFont("Calibri", 9)));
                doc.Add(new TEXTO.Paragraph(" Doc.Referencia:               " + txtdocumento.Text, TEXTO.FontFactory.GetFont("Calibri", 9)));
                doc.Add(new TEXTO.Paragraph(" N° Doc. Referencia:         " + txtnroDocu.Text, TEXTO.FontFactory.GetFont("Calibri", 9)));
                doc.Add(new TEXTO.Paragraph(" Motivo:                             " + cmdConcepto.Text, TEXTO.FontFactory.GetFont("Calibri", 9)));
                doc.Add(new TEXTO.Paragraph(" Cod.Tipo de Nota Cre.:    " + cmdConcepto.SelectedValue.ToString(), TEXTO.FontFactory.GetFont("Calibri", 9)));
                doc.Add(new TEXTO.Paragraph("                       "));
                GenerarDocumento(doc);
                doc.Add(new TEXTO.Paragraph("                       "));
                TEXTO.Image imgB = TEXTO.Image.GetInstance(codigoQR().ToString());
                imgB.BorderWidth = 0;
                int nropaginas = writer.PageNumber;
                if (tableHeight > 0 && tableHeight <= 476)
                {
                    generaTotalPDF(doc, writer);
                    imgB.SetAbsolutePosition(265, 480 - tableHeight);
                }
                else if (tableHeight >= 476 && tableHeight <= 600)
                {
                    doc.NewPage();
                    generaTotalPDF(doc, writer);
                    imgB.SetAbsolutePosition(265, 690);
                }
                else
                {
                    generaTotalPDF(doc, writer);
                    if (tableHeight == 627) imgB.SetAbsolutePosition(265, 670);
                    else imgB.SetAbsolutePosition(265, 650 - (tableHeight - 627));
                }
                imgB.ScalePercent(40, 40);
                doc.Add(imgB);
                doc.Close();
                if(xAVISO==0)Process.Start(filename);
            }
        }
        public string codigoQR()
        {
            string xvalorBarra, xruta = string.Empty;
            string xarchivo = string.Empty;
            xarchivo = String.Format("{0}-{1}-{2}-{3}.PNG", xRuc, "07", txtserie.Text, txtnumero.Text);
            xvalorBarra = xRuc + "|07|" + txtserie.Text + "|" + txtnumero.Text + "|" + lbligv.Text + "|" + lbltotal.Text + "|" + dtimeemision.Value.ToString("yyyy-MM-dd") + "|06|" + txtruc.Text;
            xruta = "\\\\" + xconexion.ServidorIP + "\\ArchivoSistema\\CPE\\CODIGOBARRA\\" + xarchivo;
            Image pbgenerar = null;
            BarcodeWriter br = new BarcodeWriter();
            br.Format = BarcodeFormat.QR_CODE;
            Bitmap bm = new Bitmap(br.Write(xvalorBarra), 300, 300);
            pbgenerar = bm;
            pbgenerar.Save(xruta);
            return xruta;
        }
        public static float CalculatePdfPTableHeight(PDFX.PdfPTable table)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (TEXTO.Document doc = new TEXTO.Document(TEXTO.PageSize.TABLOID))
                {
                    using (PDFX.PdfWriter w = PDFX.PdfWriter.GetInstance(doc, ms))
                    {
                        doc.Open();
                        table.WriteSelectedRows(0, table.Rows.Count, 0, 0, w.DirectContent);
                        doc.Close();
                        return table.TotalHeight;
                    }
                }
            }
        }
        public void GenerarDocumento(TEXTO.Document document)
        {
            int i;
            PDFX.PdfPTable datatable = new PDFX.PdfPTable(6);
            datatable.DefaultCell.Padding = 3;
            float[] headerwidths = new float[6] { 9, 19, 11, 100, 20, 20 };
            datatable.SetWidths(headerwidths);
            datatable.WidthPercentage = 100;
            datatable.DefaultCell.BorderWidth = 1;
            datatable.DefaultCell.HorizontalAlignment = 1;
            datatable.AddCell(new TEXTO.Phrase("ITEM", TEXTO.FontFactory.GetFont("Calibri", 9)));
            datatable.AddCell(new TEXTO.Phrase("CANTIDAD", TEXTO.FontFactory.GetFont("Calibri", 9)));
            datatable.AddCell(new TEXTO.Phrase("UM", TEXTO.FontFactory.GetFont("Calibri", 9)));
            datatable.AddCell(new TEXTO.Phrase("DESCRIPCION", TEXTO.FontFactory.GetFont("Calibri", 9)));
            datatable.AddCell(new TEXTO.Phrase("P.UNIT.", TEXTO.FontFactory.GetFont("Calibri", 9)));
            datatable.AddCell(new TEXTO.Phrase("IMPORTE", TEXTO.FontFactory.GetFont("Calibri", 9)));
            datatable.HeaderRows = 1;
            PDFX.PdfPCell items = null;
            PDFX.PdfPCell cell = null;
            PDFX.PdfPCell cellPre = null;
            PDFX.PdfPCell cellImp = null;
            PDFX.PdfPCell cellUM = null;
            PDFX.PdfPCell cellDes = null;
            int xcount = gvconcepto.Rows.Count;
            int xitem = 0;
            for (i = 0; i < xcount; i++)
            {
                xitem = i + 1;
                items = new PDFX.PdfPCell(new TEXTO.Phrase(xitem.ToString(), TEXTO.FontFactory.GetFont("Calibri", 8)));
                items.HorizontalAlignment = 2;
                items.UseAscender = false;
                datatable.AddCell(items);
                cell = new PDFX.PdfPCell(new TEXTO.Phrase(gvconcepto[0, i].Value.ToString(), TEXTO.FontFactory.GetFont("Calibri", 8)));
                cell.HorizontalAlignment = 2;
                cell.UseAscender = false;
                datatable.AddCell(cell);
                cellUM = new PDFX.PdfPCell(new TEXTO.Phrase(gvconcepto[1, i].Value.ToString().Substring(0, 3), TEXTO.FontFactory.GetFont("Calibri", 8)));
                cellUM.HorizontalAlignment = 0;
                cellUM.UseAscender = false;
                datatable.AddCell(cellUM);
                cellDes = new PDFX.PdfPCell(new TEXTO.Phrase(gvconcepto[2, i].Value.ToString(), TEXTO.FontFactory.GetFont("Calibri", 8)));
                cellDes.HorizontalAlignment = 0;
                cellDes.UseAscender = false;
                datatable.AddCell(cellDes);
                cellPre = new PDFX.PdfPCell(new TEXTO.Phrase(gvconcepto[3, i].Value.ToString(), TEXTO.FontFactory.GetFont("Calibri", 8)));
                cellPre.HorizontalAlignment = 2;
                cellPre.UseAscender = false;
                datatable.AddCell(cellPre);
                cellImp = new PDFX.PdfPCell(new TEXTO.Phrase(gvconcepto[4, i].Value.ToString(), TEXTO.FontFactory.GetFont("Calibri", 8)));
                cellImp.HorizontalAlignment = 2;
                cellImp.UseAscender = false;
                datatable.AddCell(cellImp);
                datatable.CompleteRow();
            }
            document.Add(datatable);
            tableHeight = CalculatePdfPTableHeight(datatable);
        }
        public void generaTotalPDF(TEXTO.Document document, PDFX.PdfWriter writer)
        {
            PDFX.PdfPTable tableB = new PDFX.PdfPTable(3);
            PDFX.PdfPCell cell = new PDFX.PdfPCell(new TEXTO.Phrase("SON: " + txtletras.Text, TEXTO.FontFactory.GetFont("Calibri", 8)));
            cell.Border = 0;
            cell.Colspan = 3;
            cell.HorizontalAlignment = 0;//0=Left, 1=Centre, 2=Right
            PDFX.PdfPCell cellA = new PDFX.PdfPCell(new TEXTO.Phrase("________________________________________________________________________________________________________________________________°", TEXTO.FontFactory.GetFont("Calibri", 8)));
            cellA.Border = 0;
            cellA.Colspan = 3;
            cellA.HorizontalAlignment = 0;//0=Left, 1=Centre, 2=Right
            float[] headerwidths = new float[3] { 110, 50, 30 };
            tableB.SetWidths(headerwidths);
            tableB.WidthPercentage = 100;
            tableB.DefaultCell.HorizontalAlignment = 2;
            PDFX.PdfPCell cellAuto, cellGrabada, cellsubtotal, repre, cellinafecta, cellIna,
            hash, cellExon, cellExonB, cellDes, cellDescuento, cellDesB, t1, t2, t3, cellIgv, cellIGVB,
            cellisc, celliscB, celltotal, celltotalB = null;
            cellAuto = new PDFX.PdfPCell(new TEXTO.Phrase("Autorizado mediante la resolucion de intendencia", TEXTO.FontFactory.GetFont("Calibri", 8)));
            cellAuto.HorizontalAlignment = 0;
            cellAuto.Border = 0;
            cellGrabada = new PDFX.PdfPCell(new TEXTO.Phrase("OP.GRAVADAS S/", TEXTO.FontFactory.GetFont("Calibri", 10, TEXTO.Font.BOLD)));
            cellGrabada.HorizontalAlignment = 2;
            cellGrabada.Border = 0;
            cellsubtotal = new PDFX.PdfPCell(new TEXTO.Phrase(lbloperacion.Text, TEXTO.FontFactory.GetFont("Calibri", 10, TEXTO.Font.BOLD)));
            cellsubtotal.HorizontalAlignment = 2;
            cellsubtotal.Border = 0;
            repre = new PDFX.PdfPCell(new TEXTO.Phrase("SUNAT/N° 0180050003180", TEXTO.FontFactory.GetFont("Calibri", 8)));
            repre.HorizontalAlignment = 0;
            repre.Border = 0;
            cellinafecta = new PDFX.PdfPCell(new TEXTO.Phrase("DESCUENTOS S/", TEXTO.FontFactory.GetFont("Calibri", 10, TEXTO.Font.BOLD)));
            cellinafecta.HorizontalAlignment = 2;
            cellinafecta.Border = 0;
            cellIna = new PDFX.PdfPCell(new TEXTO.Phrase(lbldescuento.Text, TEXTO.FontFactory.GetFont("Calibri", 10, TEXTO.Font.BOLD)));
            cellIna.HorizontalAlignment = 2;
            cellIna.Border = 0;
            cellIna.UseAscender = false;
            hash = new PDFX.PdfPCell(new TEXTO.Phrase("Representacion impresa de la factura electronica", TEXTO.FontFactory.GetFont("Calibri", 8)));
            hash.HorizontalAlignment = 0;
            hash.Border = 0;
            cellExon = new PDFX.PdfPCell(new TEXTO.Phrase("OP.EXONERADAS S/", TEXTO.FontFactory.GetFont("Calibri", 10, TEXTO.Font.BOLD)));
            cellExon.HorizontalAlignment = 2;
            cellExon.Border = 0;
            cellExonB = new PDFX.PdfPCell(new TEXTO.Phrase("0.00", TEXTO.FontFactory.GetFont("Calibri", 10, TEXTO.Font.BOLD)));
            cellExonB.HorizontalAlignment = 2;
            cellExonB.Border = 0;
            cellDes = new PDFX.PdfPCell(new TEXTO.Phrase("HASH: " + TXTHASHCPE.Text, TEXTO.FontFactory.GetFont("Calibri", 8)));
            cellDes.HorizontalAlignment = 0;
            cellDes.Border = 0;
            cellDescuento = new PDFX.PdfPCell(new TEXTO.Phrase("SUB TOTAL S/", TEXTO.FontFactory.GetFont("Calibri", 10, TEXTO.Font.BOLD)));
            cellDescuento.HorizontalAlignment = 2;
            cellDescuento.Border = 0;
            cellDesB = new PDFX.PdfPCell(new TEXTO.Phrase(lblsubtotal.Text, TEXTO.FontFactory.GetFont("Calibri", 10, TEXTO.Font.BOLD)));
            cellDesB.HorizontalAlignment = 2;
            cellDesB.Border = 0;
            t1 = new PDFX.PdfPCell(new TEXTO.Phrase("Consulta tu Comprobante en: -https://www.nubefact.com/buscar", TEXTO.FontFactory.GetFont("Calibri", 8)));
            t1.HorizontalAlignment = 0;
            t1.Border = 0;
            cellIgv = new PDFX.PdfPCell(new TEXTO.Phrase("I.G.V(18.00)% S/", TEXTO.FontFactory.GetFont("Calibri", 10, TEXTO.Font.BOLD)));
            cellIgv.HorizontalAlignment = 2;
            cellIgv.Border = 0;
            cellIGVB = new PDFX.PdfPCell(new TEXTO.Phrase(lbligv.Text, TEXTO.FontFactory.GetFont("Calibri", 10, TEXTO.Font.BOLD)));
            cellIGVB.HorizontalAlignment = 2;
            cellIGVB.Border = 0;
            t2 = new PDFX.PdfPCell(new TEXTO.Phrase("Email: " + xconexion.CorreoEMP.ToString(), TEXTO.FontFactory.GetFont("Calibri", 8)));
            t2.HorizontalAlignment = 0;
            t2.Border = 0;
            cellisc = new PDFX.PdfPCell(new TEXTO.Phrase("ICBPER S/", TEXTO.FontFactory.GetFont("Calibri", 10, TEXTO.Font.BOLD)));
            cellisc.HorizontalAlignment = 2;
            cellisc.Border = 0;
            celliscB = new PDFX.PdfPCell(new TEXTO.Phrase(lblICBPER.Text, TEXTO.FontFactory.GetFont("Calibri", 10, TEXTO.Font.BOLD)));
            celliscB.HorizontalAlignment = 2;
            celliscB.Border = 0;

            if (btnimprimir.Enabled == true)
            {
                t3 = new PDFX.PdfPCell(new TEXTO.Phrase("Nro Id: " + lblidNota.Text, TEXTO.FontFactory.GetFont("Calibri", 8)));
                t3.HorizontalAlignment = 0;
                t3.Border = 0;
            }
            else
            {
                t3 = new PDFX.PdfPCell(new TEXTO.Phrase("Nro Id: " + txtasociado.Text, TEXTO.FontFactory.GetFont("Calibri", 8)));
                t3.HorizontalAlignment = 0;
                t3.Border = 0;
            }

            celltotal = new PDFX.PdfPCell(new TEXTO.Phrase("TOTAL S/", TEXTO.FontFactory.GetFont("Calibri", 10, TEXTO.Font.BOLD)));
            celltotal.HorizontalAlignment = 2;
            celltotal.Border = 0;
            celltotalB = new PDFX.PdfPCell(new TEXTO.Phrase(lbltotal.Text, TEXTO.FontFactory.GetFont("Calibri", 10, TEXTO.Font.BOLD)));
            celltotalB.HorizontalAlignment = 2;
            celltotalB.Border = 0;
            tableB.AddCell(cell);
            tableB.AddCell(cellA);
            tableB.AddCell(cellAuto);
            tableB.AddCell(cellGrabada);
            tableB.AddCell(cellsubtotal);
            tableB.AddCell(repre);
            tableB.AddCell(cellinafecta);
            tableB.AddCell(cellIna);
            tableB.AddCell(hash);
            tableB.AddCell(cellExon);
            tableB.AddCell(cellExonB);
            tableB.AddCell(cellDes);
            tableB.AddCell(cellDescuento);
            tableB.AddCell(cellDesB);
            tableB.AddCell(t1);
            tableB.AddCell(cellIgv);
            tableB.AddCell(cellIGVB);
            tableB.AddCell(t2);
            tableB.AddCell(cellisc);
            tableB.AddCell(celliscB);
            tableB.AddCell(t3);
            tableB.AddCell(celltotal);
            tableB.AddCell(celltotalB);
            document.Add(tableB);
        }
        #endregion
        public int CalcularDias()
        {
            int valor = 0;
            int anoinico = int.Parse(txtfechaemision.Value.ToString("yyyy"));
            int mesinicio = int.Parse(txtfechaemision.Value.ToString("MM"));
            int diainicio = int.Parse(txtfechaemision.Value.ToString("dd"));
            int anofin = int.Parse(DateTime.Now.ToString("yyyy"));
            int mesfin = int.Parse(DateTime.Now.ToString("MM"));
            int diafin = int.Parse(DateTime.Now.ToString("dd"));
            DateTime fecha1 = new DateTime(anoinico, mesinicio, diainicio);
            DateTime fecha2 = new DateTime(anofin, mesfin, diafin);
            TimeSpan tiempoTranscurrido;
            tiempoTranscurrido = fecha2.Subtract(fecha1);
            valor = Convert.ToInt32(tiempoTranscurrido.Days);
            return valor;
        }
        #region Metodos
        public void botonguardar()
        {
            if (txtasociado.Text.Length == 0 || txtcliente.Text.Length == 0)
            {
                men.SeleccioneDocumento();
            }
            else if (gvconcepto.Rows.Count <= 0)
            {
                men.datosVacios();
            }
            else if (decimal.Parse(lbltotal.Text) <= 0)
            {
                MessageBox.Show("El Total de la Nota de Credito no puede ser menor o igual a Cero", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (decimal.Parse(lbldescuento.Text) > 0)
            {
                MessageBox.Show("SI LA FACTURA TIENE DESCUENTO GLOBAL NO SE PODRA REALIZAR UNA NOTA...SI NO DAR DE BAJA LA FACTURA YA QUE EL XML (SUNAT)" +
                    "DE UNA NOTA DE CREDITO NO TIENE LOS TAG DE CARGOS ADICIONALES NI DESCUENTOS GLOBALES.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (txtfiscal.Text.Length == 0)
                {
                    MessageBox.Show("La Factura no tiene la direccion fiscal...edite en el formulario cliente la direccion del cliente", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtfiscal.Focus();
                }
                else if (txtfiscal.Text.Length > 80)
                {
                    MessageBox.Show("La Factura solo permite 80 caracteres en la Direccion favor de Simplicar la Direccion", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtfiscal.Focus();
                }
                else
                {
                    string xserie, xnro = string.Empty;
                    xserie = txtserie.Text;
                    xnro = txtnumero.Text;
                    if (objdocu.validarNroFactura(xserie, xnro, lblidCompania.Text) && xAVISO == 0)
                    {
                        MessageBox.Show("El Numero de NOTA DE CREDITO ya fue enviada a sunat...favor de revisar en documentos emitidos", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        if (txtestado.Text.Equals("PENDIENTE"))
                        {
                            MessageBox.Show("La Factura y/o Boleta se encuentra en estado Pendiente, por favor de Re-Enviar en Resumenes de Documentos", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            if (xAVISO == 0)
                            {
                                AccesoDatos daSQL = new AccesoDatos("con");
                                string rpt = daSQL.ejecutarComando("uspValidarNotaCre", "@NotaId", lblidNota.Text);
                                if (rpt.Equals("true"))
                                {
                                    if (validaPagoVarios() == 0)
                                    {
                                        if (validarLiquidacion() == 0)
                                        {
                                            if (CalcularDias() >= 7)
                                            {
                                                MessageBox.Show("LA FACTURA NO PUEDE SER DADO DE BAJA POR EXCEDER EL PLAZO DESDE SU FECHA DE EMISIÓN(7 DÍAS)", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            }
                                            else
                                            {
                                                if (xconexion.xCantidadCaja == 1)
                                                {
                                                    enviarSunat();
                                                }
                                                else
                                                {
                                                    concatenar("PENDIENTE");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("El Documento no puede ser ANULADO porque ya tiene una liquidacion de pago", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("EL DOCUMENTO SELECCIONADO SE ENCUENTRA EN UNA LIQUIDACION DE PAGO/VARIOS...FAVOR DE ELIMINAR PRIMERO EN PAGO VARIOS PARA PODER ANULAR EL DOCUMENTO", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("EL DOCUMENTO SELECCIONADO, YA FUE ANULADO ANTERIORMENTE, POR FAVOR REVISAR EN EL LISTADO", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                }
                            }
                            else
                            {
                                consultarNotaCredito();
                            }
                        }
                    }
                }
            }
        }
        public void reEnviarNota()
        {
            string xvalue = string.Empty;
            xvalue =pDocuId + "|" + TXTCOD_SUNAT.Text + "|" + TXT_MSJ_SUNAT.Text;
            AccesoDatos daSQL = new AccesoDatos("con");
            string rpt = daSQL.ejecutarComando("uspReEnviarNotaCredito", "@Data", xvalue);
            if (rpt == "")
            {
                men.ErrorGuardado();
            }
            else
            {
                exportarPDF();
                men.GuardoCorrecto();
                //limpiaTodo();
                this.Close();
            }
        }
        public void consultarNotaCredito()
        {
            CPEEnvio.ServiceSunat objENV = new CPEEnvio.ServiceSunat();
            string NRO_DOCUMENTO_EMPRESA;
            string USUARIO_SOL_EMPRESA;
            string PASS_SOL_EMPRESA;
            string RUC_EMISOR;
            string TIPO_COMPROBANTE;
            string SERIE_COMPROBANTE;
            string NUMERO_COMPROBANTE;

            Dictionary<string, string> dictionary;
            string url;

            NRO_DOCUMENTO_EMPRESA = xRuc;
            USUARIO_SOL_EMPRESA = xUsuarioSol;
            PASS_SOL_EMPRESA = xClaveSol;
            RUC_EMISOR = xRuc;
            TIPO_COMPROBANTE = "07";
            SERIE_COMPROBANTE =txtserie.Text;
            NUMERO_COMPROBANTE =txtnumero.Text;
            if (xconexion.TIPO_PROCESO == 1)
            {
                url = "https://ose.nubefact.com/ol-ti-itcpe/billService?wsdl";
            }
            else
            {
                url = "https://demo-ose.nubefact.com/ol-ti-itcpe/billService?wsdl";
            }
            dictionary = objENV.getStatusFactura(NRO_DOCUMENTO_EMPRESA, USUARIO_SOL_EMPRESA, PASS_SOL_EMPRESA, url, RUC_EMISOR, TIPO_COMPROBANTE, SERIE_COMPROBANTE, NUMERO_COMPROBANTE);
            TXTCOD_SUNAT.Text = dictionary["cod_sunat"];
            TXT_MSJ_SUNAT.Text = dictionary["msj_sunat"];
            if (TXTCOD_SUNAT.Text.Equals("0"))//TXTCOD_SUNAT.Text.Equals("0001")
            {
                reEnviarNota();
                ConsultarCDR();
            }
            else
            {
                enviarSunatB();
            }
        }
        public void ConsultarCDR()
        {
            CPEEnvio.ServiceSunat objENV = new CPEEnvio.ServiceSunat();
            string NRO_DOCUMENTO_EMPRESA;
            string USUARIO_SOL_EMPRESA;
            string PASS_SOL_EMPRESA;
            string RUTA_ARCHIVO;
            string RUC_EMISOR;
            string TIPO_COMPROBANTE;
            string SERIE_COMPROBANTE;
            string NUMERO_COMPROBANTE;
            Dictionary<string, string> dictionary;
            string url;
            string nomARCHIVO;

            NRO_DOCUMENTO_EMPRESA = xRuc;
            USUARIO_SOL_EMPRESA = xUsuarioSol;
            PASS_SOL_EMPRESA = xClaveSol;
            RUC_EMISOR = xRuc;
            TIPO_COMPROBANTE = "07";
            SERIE_COMPROBANTE =txtserie.Text;
            NUMERO_COMPROBANTE =txtnumero.Text;

            nomARCHIVO = NRO_DOCUMENTO_EMPRESA + "-" + TIPO_COMPROBANTE + "-" + SERIE_COMPROBANTE + "-" + NUMERO_COMPROBANTE;
            RUTA_ARCHIVO = "\\\\" + xconexion.ServidorIP + "\\ArchivoSistema\\CPE\\PRODUCCION\\";

            if (xconexion.TIPO_PROCESO == 1)
            {
                url = "https://ose.nubefact.com/ol-ti-itcpe/billService?wsdl";
            }
            else
            {
                url = "https://demo-ose.nubefact.com/ol-ti-itcpe/billService?wsdl";
            }

            dictionary = objENV.getStatusCDR(NRO_DOCUMENTO_EMPRESA, USUARIO_SOL_EMPRESA, PASS_SOL_EMPRESA, nomARCHIVO, RUTA_ARCHIVO, url, RUC_EMISOR, TIPO_COMPROBANTE, SERIE_COMPROBANTE, NUMERO_COMPROBANTE);
            TXTCOD_SUNAT.Text = dictionary["cod_sunat"];
            TXT_MSJ_SUNAT.Text = dictionary["msj_sunat"];
            TXTHASHCDR.Text = dictionary["hash_cdr"];
        }
        public void concatenar(string xEstadoSunat)
        {
            string xdata = string.Empty;
            int count = gvconcepto.Rows.Count;
            xdata = lblidCompania.Text + "|" + lblidNota.Text + "|NOTA DE CREDITO|" +
            txtnumero.Text + "|" + lblidcliente.Text + "|" + dtimeemision.Value.ToString("MM/dd/yyyy") + "|" + double.Parse(lblsubtotal.Text).ToString() + "|" +
            double.Parse(lbligv.Text).ToString() + "|" + double.Parse(lbltotal.Text).ToString() + "|" + xPersonal.ToString() + "|" + txtserie.Text + "|07|" +
            double.Parse(lbldescuento.Text).ToString() + "|" + lblidDocu.Text + "|" + cmdConcepto.Text + "|" + TXTHASHCPE.Text + "|"+xEstadoSunat+"|" + txtletras.Text + "|" +
            txtnroDocu.Text + "|" + lblConcepto.Text + "|" + lblTransac.Text + "|" + txtcliente.Text + "|" + lblCodigo.Text + "|" +
            double.Parse(lblICBPER.Text) + "|" + TXTCOD_SUNAT.Text + "|" + TXT_MSJ_SUNAT.Text + "|" + 
            double.Parse(lbloperacion.Text) +"|"+xConceptoOBS+"|"+txtentrega.Text+
            "|"+txtpago.Text+"|"+xEntidad+"|"+(0-double.Parse(xEfectivo))+"|"+ (0 - double.Parse(xDeposito))+"[";
            for (int i = 0; i < count; i++)
            {
                xdata += Convert.ToDecimal(gvconcepto.Rows[i].Cells[0].Value);
                xdata += "|";
                xdata += Convert.ToString(gvconcepto.Rows[i].Cells[1].Value);
                xdata += "|";
                xdata += Convert.ToDecimal(gvconcepto.Rows[i].Cells[3].Value);
                xdata += "|";
                xdata += Convert.ToDecimal(gvconcepto.Rows[i].Cells[4].Value);
                xdata += "|";
                xdata += Convert.ToString(gvconcepto.Rows[i].Cells[5].Value);
                xdata += "|";
                xdata += Convert.ToString(gvconcepto.Rows[i].Cells[6].Value);
                xdata += "|";
                xdata += Convert.ToDecimal(gvconcepto.Rows[i].Cells[7].Value);
                if (i == count - 1) break;
                else xdata += ";";
            }
            AccesoDatos daSQL = new AccesoDatos("con");
            string rpt = daSQL.ejecutarComando("uspinsertarNC", "@ListaOrden", xdata);
            if (rpt == "true")
            {
                exportarPDF();
                limpiar();
                listar();
            }
            else
            {
                men.ErrorGuardado();
            }
        }
        public void cargarConcepto()
        {
            DataTable dt;
            dt = new DataTable("Tabla");
            dt.Columns.Add("Codigo");
            dt.Columns.Add("Descripcion");
            DataRow dr;
            //dr = dt.NewRow();
            //dr["Codigo"] = "03";
            //dr["Descripcion"] = "PENALIDADES / OTROS CONCEPTOS";//aramirez
            //dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Codigo"] = "01";
            dr["Descripcion"] = "ANULACION DE LA OPERACION";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Codigo"] = "07";
            dr["Descripcion"] = "DEVOLUCION POR ITEM";
            dt.Rows.Add(dr);
            cmdConcepto.DataSource = dt;
            cmdConcepto.ValueMember = "Codigo";
            cmdConcepto.DisplayMember = "Descripcion";
        }
        #endregion
        public void traerDatos()
        {
            string xvalor = string.Empty;
            if(xAVISO==1)
            {
                xvalor =txtasociado.Text+ "|" + pDocuId;
            }
            else
            {
                xvalor = txtasociado.Text + "|0";
            }
            AccesoDatos daSQL = new AccesoDatos("con");
            string rpt = daSQL.ejecutarComando("uspTraerDV", "@Valores", xvalor);
            if (rpt.Length != 0)
            {
                if (rpt == "ANULADO")
                {
                    MessageBox.Show("El id que ingreso es de un (DOCUMENTO ANULADO O DADO DE BAJA)...", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    limpiarCabezera();
                    txtasociado.SelectionStart = txtasociado.Text.Length;
                    txtasociado.Focus();
                }
                else if (rpt == "CANJEADO")
                {
                    MessageBox.Show("El id que ingreso..Ya tiene una Nota de Credito generado", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    limpiarCabezera();
                    txtasociado.SelectionStart = txtasociado.Text.Length;
                    txtasociado.Focus();
                }
                else if (rpt == "NO EXISTE")
                {
                    MessageBox.Show("El id que ingreso no existe...favor de verificar", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    limpiarCabezera();
                    txtasociado.SelectionStart = txtasociado.Text.Length;
                    txtasociado.Focus();
                }
                else
                {
                    listas = rpt.Split('[');
                    traerCabezera();
                    Tabla = Cadena.CrearTabla(listas[1]);
                    vista = Tabla.DefaultView;
                    bs = new BindingSource();
                    bs.DataSource = Tabla;
                    gvconcepto.ReadOnly = true;
                    txtcodigo.Enabled = false;
                    gvconcepto.DataSource = bs;
                    Cadena.ConfigurarGrilla(gvconcepto, Tabla);
                    ocultar();
                    totalConcepto();
                    gvconcepto.Focus();
                }
            }
        }
        public void ocultarB()
        {
            gvconcepto.Columns[5].Visible = false;
            gvconcepto.Columns[6].Visible = false;
            gvconcepto.Columns[7].Visible = false;
            gvconcepto.Columns[8].Visible = false;
            gvconcepto.Columns[9].Visible = false;
            gvconcepto.Columns[10].Visible = false;
            gvconcepto.Columns[11].Visible = false;
            gvconcepto.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gvconcepto.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gvconcepto.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }
        public void ocultar()
        {
            gvconcepto.Columns[5].Visible = false;
            gvconcepto.Columns[6].Visible = false;
            gvconcepto.Columns[7].Visible = false;
            gvconcepto.Columns[8].Visible = false;
            gvconcepto.Columns[9].Visible = false;
            gvconcepto.Columns[10].Visible = false;
            gvconcepto.Columns[11].Visible = false;
            gvconcepto.Columns[12].Visible = false;
            gvconcepto.Columns[13].Visible = false;
            gvconcepto.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gvconcepto.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gvconcepto.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }
        public void canjear()
        {
            if (gvconcepto.Columns.Count > 0) gvconcepto.Columns.Clear();
            gvconcepto.DataSource = null;
            lbloperacion.Text = "0.00";
            lbladicional.Text = "0.00";
            lblsubtotal.Text = "0.00";
            lbligv.Text = "0.00";
            lbltotal.Text = "0.00";
            lblitems.Text = "0";
            if (txtdocumento.Text.Length == 0)
            {
                men.SeleccioneDocumento();
                txtasociado.Focus();
            }
            else
            {
                Tabla = Cadena.CrearTabla(listas[1]);
                vista = Tabla.DefaultView;
                bs = new BindingSource();
                bs.DataSource = Tabla;
                if (cmdConcepto.Text == "ANULACION DE LA OPERACION")
                {
                    gvconcepto.ReadOnly = true;
                    txtcodigo.Enabled = false;
                    gvconcepto.DataSource = bs;
                    Cadena.ConfigurarGrillaB(gvconcepto, Tabla);
                    ocultar();
                    lbltotal.Text = lbltotalT.Text;
                    totalConcepto();
                    gvconcepto.Focus();
                }
            }
        }
        public void traerCabezera()
        {
            TXTCOD_SUNAT.Text = "";
            TXT_MSJ_SUNAT.Text = "";
            TXTHASHCPE.Text = "";
            TXTHASHCDR.Text = "";
            limpiarCabezera();
            string xdata = listas[0].ToString();
            string[] xarreglo = xdata.Split('|');
            txtcondicion.Text = xarreglo[0];
            txtestado.Text = xarreglo[1];
            txtdocumento.Text = xarreglo[2];
            txtnroDocu.Text = xarreglo[3];
            lblidcliente.Text = xarreglo[4];
            txtcliente.Text = xarreglo[5].Replace("&amp;", "&");
            txtruc.Text = xarreglo[6];
            txtdni.Text = xarreglo[7];
            txtfiscal.Text = xarreglo[8];
            txtfechaemision.Text = xarreglo[9];

            txtvendedor.Text = xarreglo[10];

            lbltotalT.Text = xarreglo[11];
            lblidCompania.Text = xarreglo[12];
            lbltotal.Text = lbltotalT.Text;
            if (xAVISO ==0)
            {
                txtnumero.Text =xarreglo[13];// ARAMIREZ "00000001"
                txtserie.Text = xarreglo[19];//"B001"; 
            }
            else
            {
                dtimeemision.Text = pFechaEmision;
                txtserie.Text = pSerie;
                txtnumero.Text = pNumero;
            }
            txtentrega.Text = xarreglo[14];
            txtpago.Text = xarreglo[15];
            txtestadopago.Text = xarreglo[16];
            lblidNota.Text = xarreglo[17];//
            lblidDocu.Text = xarreglo[18];
            cmdcompania.Text = xarreglo[20].ToString();
            xComercial = xarreglo[21].ToString();
            xRuc = xarreglo[22].ToString();
            xUsuarioSol = xarreglo[23].ToString();
            xClaveSol = xarreglo[24].ToString();
            xPFX = xarreglo[25].ToString();
            xPFXClave = xarreglo[26].ToString();
            xEmail = xarreglo[27].ToString();
            xDireccion = xarreglo[28].ToString();
            xTelefono = xarreglo[29].ToString();
            xNombreUBG = xarreglo[30].ToString();
            xCodigoUBG = xarreglo[31].ToString();
            xDistrito = xarreglo[32].ToString();
            xDirecSunat = xarreglo[33].ToString();
            lblConcepto.Text = xarreglo[34].ToString();
            lblCodigo.Text = xarreglo[35].ToString();
            lblTransac.Text = xarreglo[36].ToString();
            lbloperacion.Text= xarreglo[37].ToString();
            lbldescuento.Text= xarreglo[38].ToString();
            lblsubtotal.Text= xarreglo[39].ToString();
            lbligv.Text = xarreglo[40].ToString();
            xConceptoOBS = xarreglo[41].ToString();
            xEntidad= xarreglo[42].ToString();
            xEfectivo= xarreglo[43].ToString();
            xDeposito= xarreglo[44].ToString();
        }
        public void limpiarCabezera()
        {
            dtimeemision.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtletras.Text = "";
            cmdcompania.Text = "";
            lblidCompania.Text = "";
            txtserie.Text = "";
            lblasociado.Text = "";
            lblidNota.Text = "";
            lblidDocu.Text = "";
            txtcondicion.Text = "";
            txtestado.Text = "";
            txtdocumento.Text = "";
            txtnroDocu.Text = "";
            txtnumero.Text = "";
            lblidcliente.Text = "";
            txtcliente.Text = "";
            txtruc.Text = "";
            txtdni.Text = "";
            txtfiscal.Text = "";
            txtfechaemision.Text =DateTime.Now.ToString("dd/MM/yyyy");
            txtvendedor.Text = "";
            txtentrega.Text = "";
            txtpago.Text = "";
            txtestadopago.Text = "";
            lbloperacion.Text = "0.00";
            lbladicional.Text = "0.00";
            lblsubtotal.Text = "0.00";
            lbldescuento.Text = "0.00";
            lbligv.Text = "0.00";
            lbltotal.Text = "0.00";
            lblICBPER.Text = "0.00";
            lbltotalT.Text = "0.00";
            lblitems.Text = "0";
            lblConcepto.Text = "";
            lblTransac.Text = "";
            lblCodigo.Text = "";
            xComercial = string.Empty;
            xRuc = string.Empty;
            xUsuarioSol = string.Empty;
            xClaveSol = string.Empty;
            xPFX = string.Empty;
            xPFXClave = string.Empty;
            xEmail = string.Empty;
            xDireccion = string.Empty;
            xTelefono = string.Empty;
            xRucCompania = string.Empty;
            xNombreUBG = string.Empty;
            xCodigoUBG = string.Empty;
            xDistrito = string.Empty;
            xDirecSunat = string.Empty;
            xConceptoOBS = "";
            xEntidad = "";
            xEfectivo = "";
            xDeposito = "";
            if (gvconcepto.Columns.Count > 0) gvconcepto.Columns.Clear();
            gvconcepto.DataSource = null;
        }
        private void totalConcepto()
        {
            double wtotal = 0;
            double wsubtotal = 0;
            double wigv = 0;
            double wICBPER = 0;
            if (gvconcepto.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in gvconcepto.Rows)
                {
                    wtotal += Convert.ToDouble(row.Cells[4].Value);
                    wigv += Convert.ToDouble(row.Cells[9].Value);
                    wsubtotal += Convert.ToDouble(row.Cells[10].Value);
                    if (Convert.ToString(row.Cells[12].Value).Equals("BOLSAS PLASTICAS"))
                    {
                        wICBPER += Convert.ToDouble(row.Cells[0].Value) * xIMP_BOLSA;
                    }
                }
            }
            lblICBPER.Text = wICBPER.ToString("N2");
            this.lblitems.Text = gvconcepto.Rows.Count.ToString();
            txtletras.Text = Letras.enletras(lbltotal.Text) + "  SOLES";
        }
        public void listar()
        {
            fechaIniFin();
            gvlista.DataSource = null;
            AccesoDatos daSQL = new AccesoDatos("con");
            string rpt = daSQL.ejecutarComando("usplistarNC");
            if (rpt != "")
            {
                TablaA= Cadena.CrearTabla(rpt);
                vistaA = TablaA.DefaultView;
                bsA = new BindingSource();
                bsA.DataSource = TablaA;
                gvlista.DataSource = bsA;
                Cadena.ConfigurarGrilla(gvlista, TablaA);
                ocultarcolumnas();
                total();
            }
        }
        public void listaDetalle()
        {
            string xdetalle = string.Empty;
            AccesoDatos daSQL = new AccesoDatos("con");
            string rpt = daSQL.ejecutarComando("uspDetalleNC", "@DocuId", lblidDocu.Text);
            if (rpt != "")
            {
                xdetalle = rpt;
                Tabla = Cadena.CrearTabla(xdetalle);
                vista = Tabla.DefaultView;
                bs = new BindingSource();
                bs.DataSource = Tabla;
                gvconcepto.DataSource = bs;
                Cadena.ConfigurarGrilla(gvconcepto, Tabla);
                ocultarB();
            }
        }
        public void listarFecha()
        {
            gvlista.DataSource = null;
            AccesoDatos daSQL = new AccesoDatos("con");
            string rpt = daSQL.ejecutaFecha("usplistarNCFecha", dtimeinicio.Value.ToString("MM/dd/yyyy"), dtimefin.Value.ToString("MM/dd/yyyy"));
            if (rpt != "")
            {
                TablaA = Cadena.CrearTabla(rpt);
                vistaA = TablaA.DefaultView;
                bsA = new BindingSource();
                bsA.DataSource = TablaA;
                gvlista.DataSource = bsA;
                Cadena.ConfigurarGrilla(gvlista, TablaA);
                ocultarcolumnas();
                total();
            }
        }
        public void ocultarcolumnas()
        {
            gvlista.Columns[0].Visible = false;
            gvlista.Columns[1].Visible = false;
            gvlista.Columns[4].Visible = false;
            gvlista.Columns[9].Visible = false;
            gvlista.Columns[10].Visible = false;
            gvlista.Columns[16].Visible = false;
            gvlista.Columns[17].Visible = false;
            gvlista.Columns[18].Visible = false;
            gvlista.Columns[19].Visible = false;
            gvlista.Columns[20].Visible = false;
            gvlista.Columns[22].Visible = false;
            gvlista.Columns[23].Visible = false;
            gvlista.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gvlista.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gvlista.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gvlista.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }
        public void total()
        {
            double xsubtotal = 0;
            double xigv = 0;
            double xICBPER = 0;
            double xtotal = 0;
            if (gvlista.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in gvlista.Rows)
                {
                    xtotal += (Convert.ToDouble(row.Cells[11].Value) + Convert.ToDouble(row.Cells[12].Value));
                    xICBPER += Convert.ToDouble(row.Cells[13].Value);
                }
                xsubtotal = (xtotal / 1.18);
                xigv = (xtotal - xsubtotal);
                lblsubtotalC.Text = xsubtotal.ToString("N2");
                lbligvC.Text = xigv.ToString("N2");
                lblICBPER_L.Text = xICBPER.ToString("N2");
                lbltotalC.Text = (xtotal + xICBPER).ToString("N2");
                this.lblcantidad.Text = gvlista.Rows.Count.ToString();
            }
            else
            {
                lblsubtotalC.Text = "0.00";
                lbligvC.Text = "0.00";
                lblICBPER_L.Text = "0.00";
                lbltotalC.Text = "0.00";
                lblcantidad.Text = "0";
            }
        }
        public void fechaIniFin()
        {
            DateTime fechatemp;
            DateTime fechaIni;
            DateTime fechaFin;
            fechatemp = DateTime.Today;
            fechaIni = new DateTime(fechatemp.Year, fechatemp.Month, 1);
            fechaFin = new DateTime(fechatemp.Year, fechatemp.Month, 1).AddMonths(1).AddDays(-1);
            dtimeinicio.Value = fechaIni;
            dtimefin.Value = fechaFin;
        }
        public void cargarload()
        {
            fechaIniFin();
            cargarConcepto();
            men.alternarcolor(gvconcepto);
            if (xAVISO==1)
            {
                this.WindowState=FormWindowState.Normal;
                metroTabControl1.Controls.Remove(metroTabControl1.TabPages[1]);
                txtasociado.Text = pNotaId;
                txtasociado.Enabled = false;
                traerDatos();
            }
            else
            {
                listar();
                cmdfiltrar.Text = "RazonSocial";
            }
        }
        private void FrmNotaCV_Load(object sender, EventArgs e)
        {
            cargarload();
        }
        private void dtimeemision_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
                txtasociado.Focus();
            }
            else
            {
                e.Handled = false;
            }
        }
        private void FrmNotaCV_KeyDown(object sender, KeyEventArgs e)
        {
            if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.P))
            {
                if (btnimprimir.Enabled == true) botonguardar();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.N))
            {
                limpiaTodo();
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.L))
            {
                this.metroTabControl1.SelectedIndex = 1;
                txtbuscar.Focus();
            }
            else if (e.KeyCode == Keys.F5)
            {
                listar();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.metroTabControl1.SelectedIndex = 1;
                gvlista.Focus();
            }
        }
        private void gvconcepto_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in gvconcepto.Rows)
                {
                    row.HeaderCell.Value = (row.Index + 1).ToString();
                }
            }
            catch (Exception ex) { ex.ToString(); }
        }
        private void cmdConcepto_SelectionChangeCommitted(object sender, EventArgs e)
        {
            canjear();
        }
        public void limpiar()
        {
            limpiarCabezera();
            cmdConcepto.Text = "ANULACION DE LA OPERACION";
            txtasociado.Text = "";
            xConceptoOBS = "";
            txtcodigo.Enabled =false;
            txtasociado.Enabled =true;
            btnimprimir.Enabled = true;
            btnexportar.Enabled = false;
            this.metroTabControl1.SelectedIndex =0;
            dtimeemision.Focus();
        }
        public void limpiaTodo()
        {
            limpiar();
            TXTCOD_SUNAT.Text = "";
            TXT_MSJ_SUNAT.Text = "";
            TXTHASHCPE.Text = "";
            TXTHASHCDR.Text = "";
        }
        private void btnuevo_Click(object sender, EventArgs e)
        {
            limpiaTodo();
        }
        public void buscarlista()
        {
            if (txtbuscar.Text != "" && cmdfiltrar.SelectedItem != null)
            {
                string campo = cmdfiltrar.Text;
                string tipo = TablaA.Columns[campo].DataType.ToString();
                if (tipo.Contains("String")) vistaA.RowFilter = "[" + campo + "] Like '%" + txtbuscar.Text + "%'";
                else vistaA.RowFilter = "[" + campo + "] Like '%" + txtbuscar.Text + "%'";
                total();
            }
            else
            {
                vistaA.RowFilter = "";
                total();
            }
        }
        private void txtbuscar_TextChanged(object sender, EventArgs e)
        {
            buscarlista();
        }
        private void linklistar_Click(object sender, EventArgs e)
        {
            listar();
        }
        private void linkbuscar_Click(object sender, EventArgs e)
        {
            listarFecha();
        }
        private void dtimeinicio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
                dtimefin.Focus();
            }
            else
            {
                e.Handled = false;
            }
        }
        private void dtimefin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
                listarFecha();
            }
            else
            {
                e.Handled = false;
            }
        }
        private void dtimefin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)dtimeinicio.Focus();
        }
        private void dtimeinicio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)txtbuscar.Focus();
        }
        private void txtbuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
                gvlista.Focus();
            }
            else
            {
                e.Handled = false;
            }
        }
        public void enviardatos()
        {
            try
            {
                limpiaTodo();
                this.lblidDocu.Text = Convert.ToString(gvlista.CurrentRow.Cells[0].Value);
                this.lblidCompania.Text = Convert.ToString(gvlista.CurrentRow.Cells[1].Value);
                this.txtasociado.Text = Convert.ToString(gvlista.CurrentRow.Cells[2].Value);
                this.dtimeemision.Text = Convert.ToString(gvlista.CurrentRow.Cells[3].Value);
                this.txtdocumento.Text = Convert.ToString(gvlista.CurrentRow.Cells[4].Value);
                this.txtcliente.Text = Convert.ToString(gvlista.CurrentRow.Cells[6].Value);
                this.txtruc.Text = Convert.ToString(gvlista.CurrentRow.Cells[7].Value);
                this.txtnroDocu.Text = Convert.ToString(gvlista.CurrentRow.Cells[8].Value);
                this.txtnumero.Text = Convert.ToString(gvlista.CurrentRow.Cells[9].Value);
                this.txtserie.Text = Convert.ToString(gvlista.CurrentRow.Cells[10].Value);
                this.lblsubtotal.Text = Convert.ToString(gvlista.CurrentRow.Cells[11].Value);
                this.lbligv.Text = Convert.ToString(gvlista.CurrentRow.Cells[12].Value);
                this.lblICBPER.Text = Convert.ToString(gvlista.CurrentRow.Cells[13].Value);
                this.lbltotal.Text = Convert.ToString(gvlista.CurrentRow.Cells[14].Value);
                this.txtfechaemision.Text = dtimeemision.Text;
                this.txtvendedor.Text = Convert.ToString(gvlista.CurrentRow.Cells[15].Value);
                this.txtestadopago.Text = Convert.ToString(gvlista.CurrentRow.Cells[16].Value);
                this.txtfiscal.Text = Convert.ToString(gvlista.CurrentRow.Cells[17].Value);
                this.lblasociado.Text = Convert.ToString(gvlista.CurrentRow.Cells[18].Value);
                this.cmdcompania.Text = Convert.ToString(gvlista.CurrentRow.Cells[19].Value);
                this.xRucCompania = Convert.ToString(gvlista.CurrentRow.Cells[20].Value);
                this.cmdConcepto.Text = Convert.ToString(gvlista.CurrentRow.Cells[21].Value);
                this.lbloperacion.Text = Convert.ToString(gvlista.CurrentRow.Cells[22].Value);
                this.lbldescuento.Text = Convert.ToString(gvlista.CurrentRow.Cells[23].Value);
                listaDetalle();
                txtasociado.Enabled = false;
                btnimprimir.Enabled = false;
                btnexportar.Enabled = true;
                this.metroTabControl1.SelectedIndex = 0;
            }
            catch (Exception ex) { ex.ToString(); }
        }
        private void gvlista_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.enviardatos();
            }
            else if (e.KeyCode == Keys.ShiftKey)
                txtbuscar.Focus();
        }
        public int validarLiquidacion()
        {
            int xcount = 0;
            AccesoDatos daSQL = new AccesoDatos("con");
            string rpt = daSQL.ejecutarComando("uspCantidadLiquidacion", "@NotaId",lblidNota.Text);
            if (string.IsNullOrEmpty(rpt)) xcount = 0;
            else xcount = int.Parse(rpt);
            return xcount;
        }
        public int validaPagoVarios()
        {
            int xcount = 0;
            AccesoDatos daSQL = new AccesoDatos("con");
            string rpt = daSQL.ejecutarConsulta("Declare @Count int set @Count = (select count(d.NotaId) "+
            "from DetallePVarios d where d.NotaId = '"+lblidNota.Text+"') select convert(varchar, @Count)");
            if (string.IsNullOrEmpty(rpt)) xcount = 0;
            else xcount = int.Parse(rpt);
            return xcount;
        }
        private void btnimprimir_Click(object sender, EventArgs e)
        {
            botonguardar();
        }
        private void txtasociado_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)32)
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
                if (txtasociado.Text.Length == 0)
                {
                    men.datosVacios();
                    txtasociado.Focus();
                    limpiarCabezera();
                }
                else
                {
                    traerDatos();
                }
            }
            Validar.SoloNumeros(e);
        }
        public void abrirPDF()
        {
            try
            {
                string xarchivo, xruta= string.Empty;
                xarchivo = String.Format("{0}-{1}-{2}-{3}.PDF", xRucPrincipal,"07", txtserie.Text, txtnumero.Text);
                xruta = "\\\\" + xconexion.ServidorIP + "\\ArchivoSistema\\CPE\\PRODUCCION\\" + xarchivo;
                FileInfo di = new FileInfo(xruta);
                if (di.Exists)
                {
                    Process.Start(xruta);
                }
                else
                {
                    exportarPDF();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnexportar_Click(object sender, EventArgs e)
        {
            abrirPDF();
        }
        private void cmdfiltrar_SelectionChangeCommitted(object sender, EventArgs e)
        {
            txtbuscar.Text = "";
            txtbuscar.Focus();
        }
        private void gvconcepto_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            gvconcepto.Columns[e.Column.Index].SortMode = DataGridViewColumnSortMode.NotSortable;
        }
        private void gvlista_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            gvlista.Columns[e.Column.Index].SortMode = DataGridViewColumnSortMode.NotSortable;
        }
    }
}
