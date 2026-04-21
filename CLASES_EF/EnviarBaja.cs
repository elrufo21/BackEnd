  public int CalcularDias()
        {
            int valor = 0;
            int anoinico = int.Parse(dtimeemison.Value.ToString("yyyy"));
            int mesinicio = int.Parse(dtimeemison.Value.ToString("MM"));
            int diainicio = int.Parse(dtimeemison.Value.ToString("dd"));
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

 private void btnanular_Click(object sender, EventArgs e)
        {
            if (gvdetalle.Rows.Count <= 0 || lblcliente.Text.Length == 0 || lbliddocu.Text.Length == 0)
            {
                men.SeleccioneUnDato();
            }
            else
            {
                if (CalcularDias() >= 2)
                {
                    MessageBox.Show("LA BOLETA NO PUEDE SER DADO DE BAJA POR EXCEDER EL PLAZO DESDE SU FECHA DE EMISIÓN(2 DÍA)", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                        anularDocumento();
                }
            }
        }


      public void anularDocumento()
        {
            if (gvdetalle.Rows.Count <= 0 || lblcliente.Text.Length == 0 || lbliddocu.Text.Length == 0)
                men.SeleccioneUnDato();
            else
            {
                if(lblestadoA.Text.Contains("ANULADO"))
                {
                    MessageBox.Show("El Documento seleccionado ya se encuentra ANULADO","AVISO",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
                else
                {
                    string xdata = string.Empty;
                    int count = gvdetalle.Rows.Count;
                    string xtipoCodigo = string.Empty;
                    if (lbldocumento.Text == "BOLETA") xtipoCodigo = "03";
                    else xtipoCodigo = "00";
                    xdata = lbliddocu.Text + "|" + lblidnota.Text + "|" + xUsuario+"|"+lblconcepto.Text+
                    "|"+txtserie.Text+"-"+txtnumero.Text+"|"+lblcliente.Text+"|"+lblCodigo.Text+"|"+lblTransac.Text+"|"+
                    xtipoCodigo+"[";
                    for (int i = 0; i < count; i++)
                    {
                        xdata += Convert.ToString(gvdetalle.Rows[i].Cells[2].Value);
                        xdata += "|";
                        xdata += Convert.ToDecimal(gvdetalle.Rows[i].Cells[4].Value);
                        xdata += "|";
                        xdata += Convert.ToDecimal(gvdetalle.Rows[i].Cells[7].Value);
                        if (i == count - 1) break;
                        else xdata += ";";
                    }
                    AccesoDatos daSQL = new AccesoDatos("con");
                    string rpt = daSQL.ejecutarComando("anularDocumento", "@ListaOrden", xdata);
                    if (rpt != "")
                    {
                        if (rpt.Contains("PAGO"))
                        {
                            MessageBox.Show("EL DOCUMENTO SELECCIONADO SE ENCUENTRA EN UNA LIQUIDACION DE PAGO/VARIOS...FAVOR DE ELIMINAR PRIMERO EN PAGO VARIOS PARA PODER ANULAR EL DOCUMENTO", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            men.EditoCorrecto();
                            limpiar();
                            listar();
                        }
                    }
                    else
                    {
                        men.EditoError();
                    }
                }
            }
        }




        public void enviarResumenBaja()
        {
            limpiaResSunat();
            calculaFechaReferencia();
            traerFirma();
            Stopwatch oReloj = new Stopwatch();
            oReloj.Start();
            xsecuencia = string.Empty;
            AccesoDatos daSQL = new AccesoDatos("con");
            string rpt = daSQL.ejecutarConsulta("select top 1 convert(varchar,Secuencia+1) " +
                "from ResumenBoletas where CompaniaId ='1' order by Secuencia desc");
            if (!string.IsNullOrEmpty(rpt))
            {
                xsecuencia = rpt;
                objCPE.NRO_DOCUMENTO_EMPRESA = xRuc;
                objCPE.RAZON_SOCIAL = xRazonSocial;
                objCPE.TIPO_DOCUMENTO = "6"; // RUC'
                objCPE.CODIGO = "RC"; // Resumen de Boletas
                objCPE.SERIE = DateTime.Now.ToString("yyyyMMdd"); // fecha sin guines "20161029"
                objCPE.SECUENCIA = xsecuencia;
                objCPE.FECHA_REFERENCIA = xFechaRefencia.ToString();
                objCPE.FECHA_DOCUMENTO = DateTime.Now.ToString("yyyy-MM-dd"); // declarar
                objCPE.TIPO_PROCESO = NROPROCESO;//1=PRODUCCION, 2=HOMOLOGACION, 3=BETA
                objCPE.CONTRA_FIRMA = xPFXClave;
                objCPE.USUARIO_SOL_EMPRESA = xUsuarioSol;
                objCPE.PASS_SOL_EMPRESA = xClaveSol;
                objCPE.RUTA_PFX = xPFX;
                List<BusinessEntities.CPE_RESUMEN_BOLETA_DETALLE> OBJCPE_DETALLE_LIST = new List<BusinessEntities.CPE_RESUMEN_BOLETA_DETALLE>();
                for (int i = 0; i <= gvlista.Rows.Count - 1; i++)
                {
                    objCPE_DETALLE = new BusinessEntities.CPE_RESUMEN_BOLETA_DETALLE();
                    objCPE_DETALLE.ITEM = i + 1;
                    objCPE_DETALLE.TIPO_COMPROBANTE = "03";
                    objCPE_DETALLE.NRO_COMPROBANTE = Convert.ToString(gvlista.Rows[i].Cells[5].Value.ToString()); //"B001-12";
                    objCPE_DETALLE.TIPO_DOCUMENTO = "1"; // dni
                    if (Convert.ToString(gvlista.Rows[i].Cells[7].Value.ToString()) == "" || Convert.ToString(gvlista.Rows[i].Cells[7].Value.ToString()) == null)
                        objCPE_DETALLE.NRO_DOCUMENTO = "00000000";
                    else
                        objCPE_DETALLE.NRO_DOCUMENTO = Convert.ToString(gvlista.Rows[i].Cells[7].Value.ToString());//nro del dni
                    objCPE_DETALLE.TIPO_COMPROBANTE_REF = ""; // nota credito hacer una refeencia 07
                    objCPE_DETALLE.NRO_COMPROBANTE_REF = ""; // nota creditp
                    objCPE_DETALLE.STATU = "3"; // declarar exite '3' anular
                    objCPE_DETALLE.COD_MONEDA = "PEN";
                    objCPE_DETALLE.TOTAL = Convert.ToDecimal(gvlista.Rows[i].Cells[11].Value.ToString());// 1693.39;
                    objCPE_DETALLE.ICBPER = Convert.ToDecimal(gvlista.Rows[i].Cells[10].Value.ToString());
                    objCPE_DETALLE.GRAVADA = Convert.ToDecimal(gvlista.Rows[i].Cells[8].Value.ToString()); //1435.08;
                    objCPE_DETALLE.ISC = 0;
                    objCPE_DETALLE.IGV = Convert.ToDecimal(gvlista.Rows[i].Cells[9].Value.ToString());
                    objCPE_DETALLE.OTROS = 0;
                    objCPE_DETALLE.CARGO_X_ASIGNACION = 1;
                    objCPE_DETALLE.MONTO_CARGO_X_ASIG = 0;
                    objCPE_DETALLE.EXONERADO = 0;
                    objCPE_DETALLE.INAFECTO = 0;
                    objCPE_DETALLE.EXPORTACION = 0;
                    objCPE_DETALLE.GRATUITAS = 0;
                    OBJCPE_DETALLE_LIST.Add(objCPE_DETALLE);
                }
                objCPE.detalle = OBJCPE_DETALLE_LIST;//objCPE.detalle = OBJCPE_DETALLE_LIST;
                // ======================================RESPUESTA====================================
                Dictionary<string, string> dictionaryEnv = new Dictionary<string, string>();
                dictionaryEnv = objF.EnvioResumen(objCPE);
                TXTCOD_SUNAT.Text = dictionaryEnv["cod_sunat"];
                TXT_MSJ_SUNAT.Text = dictionaryEnv["msj_sunat"];
                TXTHASHCPE.Text = dictionaryEnv["hash_cpe"];
                TXTHASHCDR.Text = dictionaryEnv["hash_cdr"];
                // ==============================
                txtticket.Text = dictionaryEnv["msj_sunat"];
                if (!Val.IsMatch(txtticket.Text))
                {
                    MessageBox.Show(TXTCOD_SUNAT.Text + "-" + TXT_MSJ_SUNAT.Text, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    oReloj.Stop();
                    this.Text = ".:.TIEMPO DE RESPUESTA:  " + oReloj.Elapsed.TotalMilliseconds.ToString("N2") + " MS";
                }
                else
                {
                    if (TXTCOD_SUNAT.Text == "0402" || TXTCOD_SUNAT.Text == "0111" || TXTCOD_SUNAT.Text == "2018" || TXTCOD_SUNAT.Text == "100" || TXTCOD_SUNAT.Text == "2223" || TXTCOD_SUNAT.Text == "0135" || TXTCOD_SUNAT.Text == "200" || TXTCOD_SUNAT.Text == "2663" || TXTCOD_SUNAT.Text == "0109" || TXTCOD_SUNAT.Text == "2220")
                    {
                        MessageBox.Show(TXTCOD_SUNAT.Text + "-" + TXT_MSJ_SUNAT.Text, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        oReloj.Stop();
                        this.Text = ".:.TIEMPO DE RESPUESTA:  " + oReloj.Elapsed.TotalMilliseconds.ToString("N2") + " MS";
                    }
                    else
                    {
                        guardarResumen();
                        oReloj.Stop();
                        this.Text = ".:.TIEMPO DE RESPUESTA:  " + oReloj.Elapsed.TotalMilliseconds.ToString("N2") + " MS";
                    }
                }
            }
            else
            {
                oReloj.Stop();
                MessageBox.Show("Error al Traer la secuencia de Resumen de Boletas", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }