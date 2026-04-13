using System;
using System.Collections.Generic;
using System.IO;
using BE = BusinessEntities;
using EV = CPEEnvio;
using XM = Xml;
using SG = Signature;
namespace MegaRosita.Capa.Aplicacion
{
    public class CPEConfig
    {
        private const string UrlOceDemoDefault = "https://demo-ose.nubefact.com/ol-ti-itcpe/billService?wsdl";
        private const string UrlOceProduccionDefault = "https://ose.nubefact.com/ol-ti-itcpe/billService?wsdl";
        private const string UrlSunatBetaDefault = "https://e-beta.sunat.gob.pe/ol-ti-itcpfegem-beta/billService?wsdl";
        private const string UrlSunatHomologacionDefault = "https://www.sunat.gob.pe/ol-ti-itcpgem-sqa/billService?wsdl";
        private const string UrlSunatProduccionDefault = "https://e-factura.sunat.gob.pe/ol-ti-itcpfegem/billService?wsdl";

        private XM.CrearXML objXML = new XM.CrearXML();
        private SG.FirmadoRequest objPregunta = new SG.FirmadoRequest();
        private SG.FirmadoResponse objRespuesta = new SG.FirmadoResponse();
        private SG.Signature objSignature = new SG.Signature();
        private EV.ServiceSunat objENV = new EV.ServiceSunat();
        Conexion xconexion = new Conexion();
        public Dictionary<string, string> Envio(BE.CPE CPE)
        {
            Dictionary<string, string> dictionary = null;
            string nomARCHIVO = "";
            string ruta = "";
            string rutaFirma = "";
            string url = "";
            CPE.TOTAL_GRAVADAS = (CPE.TOTAL_GRAVADAS != null ? CPE.TOTAL_GRAVADAS : 0);
            CPE.TOTAL_INAFECTA = (CPE.TOTAL_INAFECTA != null ? CPE.TOTAL_INAFECTA : 0);
            CPE.TOTAL_EXONERADAS = (CPE.TOTAL_EXONERADAS != null ? CPE.TOTAL_EXONERADAS : 0);
            CPE.TOTAL_GRATUITAS = (CPE.TOTAL_GRATUITAS != null ? CPE.TOTAL_GRATUITAS : 0);
            CPE.TOTAL_PERCEPCIONES = (CPE.TOTAL_PERCEPCIONES != null ? CPE.TOTAL_PERCEPCIONES : 0);
            CPE.TOTAL_RETENCIONES = (CPE.TOTAL_RETENCIONES != null ? CPE.TOTAL_RETENCIONES : 0);
            CPE.TOTAL_DETRACCIONES = (CPE.TOTAL_DETRACCIONES != null ? CPE.TOTAL_DETRACCIONES : 0);
            CPE.TOTAL_BONIFICACIONES = (CPE.TOTAL_BONIFICACIONES != null ? CPE.TOTAL_BONIFICACIONES : 0);
            CPE.TOTAL_DESCUENTO = (CPE.TOTAL_DESCUENTO != null ? CPE.TOTAL_DESCUENTO : 0);
            CPE.TOTAL_ICBPER = (CPE.TOTAL_ICBPER != null ? CPE.TOTAL_ICBPER : 0);
            CPE.TOTAL_EXPORTACION = (CPE.TOTAL_EXPORTACION != null ? CPE.TOTAL_EXPORTACION : 0);
            nomARCHIVO = CPE.NRO_DOCUMENTO_EMPRESA + "-" + CPE.COD_TIPO_DOCUMENTO + "-" + CPE.NRO_COMPROBANTE;
            url = ResolverUrlOce(CPE.TIPO_PROCESO);
            switch (CPE.TIPO_PROCESO)
            {
                case 3:
                    ruta = ObtenerRutaBeta(xconexion);
                    rutaFirma = ResolverRutaFirma(CPE.RUTA_PFX);
                    if (CPE.COD_TIPO_DOCUMENTO == "01" || CPE.COD_TIPO_DOCUMENTO == "03")
                    {
                        objXML.CPE(CPE, nomARCHIVO, ruta);
                    }
                    else
                    {
                        if (CPE.COD_TIPO_DOCUMENTO == "07")
                        {
                            objXML.CPE_NC(CPE, nomARCHIVO, ruta);
                        }
                        else
                        {
                            if (CPE.COD_TIPO_DOCUMENTO == "08")
                            {
                                objXML.CPE_ND(CPE, nomARCHIVO, ruta);
                            }
                        }
                    }
                    objPregunta.ruta_Firma = rutaFirma;
                    objPregunta.contra_Firma = CPE.CONTRA_FIRMA;
                    objPregunta.ruta_xml = ruta + nomARCHIVO + ".XML";
                    objPregunta.flg_firma = 0;
                    objRespuesta = objSignature.FirmaXMl(objPregunta);
                    dictionary = objENV.Envio(CPE.NRO_DOCUMENTO_EMPRESA, CPE.USUARIO_SOL_EMPRESA, CPE.PASS_SOL_EMPRESA, nomARCHIVO, ruta, url, objRespuesta.DigestValue);
                    break;
                case 2:
                    ruta = ObtenerRutaHomologacion();
                    rutaFirma = ResolverRutaFirma(CPE.RUTA_PFX);
                    if (CPE.COD_TIPO_DOCUMENTO == "01" || CPE.COD_TIPO_DOCUMENTO == "03")
                    {
                        objXML.CPE(CPE, nomARCHIVO, ruta);
                    }
                    else
                    {
                        if (CPE.COD_TIPO_DOCUMENTO == "07")
                        {
                            objXML.CPE_NC(CPE, nomARCHIVO, ruta);
                        }
                        else
                        {
                            if (CPE.COD_TIPO_DOCUMENTO == "08")
                            {
                                objXML.CPE_ND(CPE, nomARCHIVO, ruta);
                            }
                        }
                    }
                    objPregunta.ruta_Firma = rutaFirma;
                    objPregunta.contra_Firma = CPE.CONTRA_FIRMA;
                    objPregunta.ruta_xml = ruta + nomARCHIVO + ".XML";
                    objPregunta.flg_firma = 0;
                    objRespuesta = objSignature.FirmaXMl(objPregunta);
                    dictionary = objENV.Envio(CPE.NRO_DOCUMENTO_EMPRESA, CPE.USUARIO_SOL_EMPRESA, CPE.PASS_SOL_EMPRESA, nomARCHIVO, ruta, url, objRespuesta.DigestValue);
                    break;
                case 1:
                    ruta = ObtenerRutaProduccion(xconexion);
                    rutaFirma = ResolverRutaFirma(CPE.RUTA_PFX);
                    if (CPE.COD_TIPO_DOCUMENTO == "01" || CPE.COD_TIPO_DOCUMENTO == "03")
                    {
                        objXML.CPE(CPE, nomARCHIVO, ruta);
                    }
                    else
                    {
                        if (CPE.COD_TIPO_DOCUMENTO == "07")
                        {
                            objXML.CPE_NC(CPE, nomARCHIVO, ruta);
                        }
                        else
                        {
                            if (CPE.COD_TIPO_DOCUMENTO == "08")
                            {
                                objXML.CPE_ND(CPE, nomARCHIVO, ruta);
                            }
                        }
                    }
                    objPregunta.ruta_Firma = rutaFirma;
                    objPregunta.contra_Firma = CPE.CONTRA_FIRMA;
                    objPregunta.ruta_xml = ruta + nomARCHIVO + ".XML";
                    objPregunta.flg_firma = 0;
                    objRespuesta = objSignature.FirmaXMl(objPregunta);
                    dictionary = objENV.Envio(CPE.NRO_DOCUMENTO_EMPRESA, CPE.USUARIO_SOL_EMPRESA, CPE.PASS_SOL_EMPRESA, nomARCHIVO, ruta, url, objRespuesta.DigestValue);
                    break;
            }
            return dictionary;
        }
        public Dictionary<string, string> EnvioResumen(BE.CPE_RESUMEN_BOLETA CPEResumen)
        {
            Dictionary<string, string> dictionary = null;
            string nomARCHIVO = "";
            string ruta = "";
            string rutaFirma = "";
            string url = "";
            nomARCHIVO = CPEResumen.NRO_DOCUMENTO_EMPRESA + "-" + CPEResumen.CODIGO + "-" + CPEResumen.SERIE + "-" + CPEResumen.SECUENCIA;
            url = ResolverUrlOce(CPEResumen.TIPO_PROCESO);
            switch (CPEResumen.TIPO_PROCESO)
            {
                case 3:
                    ruta = ObtenerRutaBeta(xconexion);
                    rutaFirma = ResolverRutaFirma(CPEResumen.RUTA_PFX);
                    objXML.ResumenBoleta(CPEResumen, nomARCHIVO, ruta);
                    objPregunta.ruta_Firma = rutaFirma;
                    objPregunta.contra_Firma = CPEResumen.CONTRA_FIRMA;
                    objPregunta.ruta_xml = ruta + nomARCHIVO + ".XML";
                    objPregunta.flg_firma = 0;
                    objRespuesta = objSignature.FirmaXMl(objPregunta);
                    dictionary = objENV.EnvioResumen(CPEResumen.NRO_DOCUMENTO_EMPRESA, CPEResumen.USUARIO_SOL_EMPRESA, CPEResumen.PASS_SOL_EMPRESA, nomARCHIVO, ruta, url, objRespuesta.DigestValue);
                    break;
                case 2:
                    ruta = ObtenerRutaHomologacion();
                    rutaFirma = ResolverRutaFirma(CPEResumen.RUTA_PFX);
                    objXML.ResumenBoleta(CPEResumen, nomARCHIVO, ruta);
                    objPregunta.ruta_Firma = rutaFirma;
                    objPregunta.contra_Firma = CPEResumen.CONTRA_FIRMA;
                    objPregunta.ruta_xml = ruta + nomARCHIVO + ".XML";
                    objPregunta.flg_firma = 0;
                    objRespuesta = objSignature.FirmaXMl(objPregunta);
                    dictionary = objENV.EnvioResumen(CPEResumen.NRO_DOCUMENTO_EMPRESA, CPEResumen.USUARIO_SOL_EMPRESA, CPEResumen.PASS_SOL_EMPRESA, nomARCHIVO, ruta, url, objRespuesta.DigestValue);
                    break;
                case 1:
                    ruta = ObtenerRutaProduccion(xconexion);
                    rutaFirma = ResolverRutaFirma(CPEResumen.RUTA_PFX);
                    objXML.ResumenBoleta(CPEResumen, nomARCHIVO, ruta);
                    objPregunta.ruta_Firma = rutaFirma;
                    objPregunta.contra_Firma = CPEResumen.CONTRA_FIRMA;
                    objPregunta.ruta_xml = ruta + nomARCHIVO + ".XML";
                    objPregunta.flg_firma = 0;
                    objRespuesta = objSignature.FirmaXMl(objPregunta);
                    dictionary = objENV.EnvioResumen(CPEResumen.NRO_DOCUMENTO_EMPRESA, CPEResumen.USUARIO_SOL_EMPRESA, CPEResumen.PASS_SOL_EMPRESA, nomARCHIVO, ruta, url, objRespuesta.DigestValue);
                    break;
            }
            return dictionary;
        }

        private static string ResolverRutaFirma(string rutaPfx)
        {
            if (string.IsNullOrWhiteSpace(rutaPfx))
            {
                return "D:\\CPE\\FIRMABETA\\";
            }

            var valor = rutaPfx.Trim();
            if (Path.IsPathRooted(valor))
            {
                return valor;
            }

            return Path.Combine(@"D:\CPE\FIRMABETA", valor);
        }

        private static string ResolverUrlOce(int? tipoProceso)
        {
            var urlGlobal = ObtenerVariable(
                "CPE_WS_URL",
                "CPE_URL_WS",
                "CPE_OCE_URL",
                "CPE_OSE_URL",
                "CPE_URL_OCE",
                "CPE_URL_OSE",
                "CPE_SUNAT_URL",
                "CPE_URL_SUNAT");

            if (!string.IsNullOrWhiteSpace(urlGlobal))
            {
                return urlGlobal;
            }

            var canalEnvio = ObtenerVariable(
                "CPE_CANAL_ENVIO",
                "CPE_CANAL",
                "CPE_PROVIDER",
                "CPE_EMISOR");

            // Compatibilidad: por defecto mantenemos OCE (flujo operativo actual del proyecto).
            // Solo usamos SUNAT directo si el canal se define explícitamente como SUNAT.
            var usarOce = !string.Equals(canalEnvio, "SUNAT", StringComparison.OrdinalIgnoreCase);

            if (usarOce)
            {
                switch (tipoProceso)
                {
                    case 1:
                        return ObtenerVariable(
                                   "CPE_OCE_URL_PRODUCCION",
                                   "CPE_OSE_URL_PRODUCCION",
                                   "CPE_URL_OCE_PRODUCCION",
                                   "CPE_URL_OSE_PRODUCCION")
                               ?? UrlOceProduccionDefault;
                    case 2:
                        return ObtenerVariable(
                                   "CPE_OCE_URL_HOMOLOGACION",
                                   "CPE_OSE_URL_HOMOLOGACION",
                                   "CPE_URL_OCE_HOMOLOGACION",
                                   "CPE_URL_OSE_HOMOLOGACION")
                               ?? UrlOceDemoDefault;
                    case 3:
                    default:
                        return ObtenerVariable(
                                   "CPE_OCE_URL_BETA",
                                   "CPE_OSE_URL_BETA",
                                   "CPE_URL_OCE_BETA",
                                   "CPE_URL_OSE_BETA")
                               ?? UrlOceDemoDefault;
                }
            }

            switch (tipoProceso)
            {
                case 1:
                    return ObtenerVariable(
                               "CPE_SUNAT_URL_PRODUCCION",
                               "CPE_URL_SUNAT_PRODUCCION")
                           ?? UrlSunatProduccionDefault;
                case 2:
                    return ObtenerVariable(
                               "CPE_SUNAT_URL_HOMOLOGACION",
                               "CPE_URL_SUNAT_HOMOLOGACION")
                           ?? UrlSunatHomologacionDefault;
                case 3:
                default:
                    return ObtenerVariable(
                               "CPE_SUNAT_URL_BETA",
                               "CPE_URL_SUNAT_BETA")
                           ?? UrlSunatBetaDefault;
            }
        }

        private static string? ObtenerVariable(params string[] nombres)
        {
            foreach (var nombre in nombres)
            {
                var valor = Environment.GetEnvironmentVariable(nombre);
                if (!string.IsNullOrWhiteSpace(valor))
                {
                    return valor.Trim();
                }
            }

            return null;
        }

        private static string ObtenerRutaBeta(Conexion conexion)
        {
            return ResolverRutaTrabajo(
                Environment.GetEnvironmentVariable("CPE_RUTA_BETA"),
                "\\\\" + conexion.ServidorIP + "\\ArchivoSistema\\CPE\\BETA\\",
                "BETA");
        }

        private static string ObtenerRutaProduccion(Conexion conexion)
        {
            return ResolverRutaTrabajo(
                Environment.GetEnvironmentVariable("CPE_RUTA_PRODUCCION"),
                "\\\\" + conexion.ServidorIP + "\\ArchivoSistema\\CPE\\PRODUCCION\\",
                "PRODUCCION");
        }

        private static string ObtenerRutaHomologacion()
        {
            return ResolverRutaTrabajo(
                Environment.GetEnvironmentVariable("CPE_RUTA_HOMOLOGACION"),
                "D:\\CPE\\HOMOLOGACION\\",
                "HOMOLOGACION");
        }

        private static string ResolverRutaTrabajo(string rutaOverride, string rutaPorDefecto, string carpetaFallback)
        {
            var rutaObjetivo = string.IsNullOrWhiteSpace(rutaOverride) ? rutaPorDefecto : rutaOverride.Trim();
            rutaObjetivo = AsegurarSlashFinal(rutaObjetivo);

            try
            {
                Directory.CreateDirectory(rutaObjetivo);
                return rutaObjetivo;
            }
            catch
            {
                var fallback = Path.Combine(AppContext.BaseDirectory, "legacy-cpe", carpetaFallback);
                Directory.CreateDirectory(fallback);
                return AsegurarSlashFinal(fallback);
            }
        }

        private static string AsegurarSlashFinal(string ruta)
        {
            if (string.IsNullOrWhiteSpace(ruta))
            {
                return ruta;
            }

            return ruta.EndsWith("\\", StringComparison.Ordinal) || ruta.EndsWith("/", StringComparison.Ordinal)
                ? ruta
                : ruta + "\\";
        }

        public Dictionary<string, string> EnvioBaja(BE.CPE_BAJA CPEBaja)
        {
            Dictionary<string, string> dictionary = null;
            string nomARCHIVO = "";
            string ruta = "";
            string rutaFirma = "";
            string url = "";
            nomARCHIVO = CPEBaja.NRO_DOCUMENTO_EMPRESA + "-" + CPEBaja.CODIGO + "-" + CPEBaja.SERIE + "-" + CPEBaja.SECUENCIA;
            url = ResolverUrlOce(CPEBaja.TIPO_PROCESO);
            switch (CPEBaja.TIPO_PROCESO)
            {
                case 3:
                    ruta = ObtenerRutaBeta(xconexion);
                    rutaFirma = ResolverRutaFirma(CPEBaja.RUTA_PFX);
                    objXML.ResumenBaja(CPEBaja, nomARCHIVO, ruta);
                    objPregunta.ruta_Firma = rutaFirma;
                    objPregunta.contra_Firma = CPEBaja.CONTRA_FIRMA;
                    objPregunta.ruta_xml = ruta + nomARCHIVO + ".XML";
                    objPregunta.flg_firma = 0;
                    objRespuesta = objSignature.FirmaXMl(objPregunta);
                    dictionary = objENV.EnvioResumen(CPEBaja.NRO_DOCUMENTO_EMPRESA, CPEBaja.USUARIO_SOL_EMPRESA, CPEBaja.PASS_SOL_EMPRESA, nomARCHIVO, ruta, url, objRespuesta.DigestValue);
                    break;
                case 2:
                    ruta = ObtenerRutaHomologacion();
                    rutaFirma = ResolverRutaFirma(CPEBaja.RUTA_PFX);
                    objXML.ResumenBaja(CPEBaja, nomARCHIVO, ruta);
                    objPregunta.ruta_Firma = rutaFirma;
                    objPregunta.contra_Firma = CPEBaja.CONTRA_FIRMA;
                    objPregunta.ruta_xml = ruta + nomARCHIVO + ".XML";
                    objPregunta.flg_firma = 0;
                    objRespuesta = objSignature.FirmaXMl(objPregunta);
                    dictionary = objENV.EnvioResumen(CPEBaja.NRO_DOCUMENTO_EMPRESA, CPEBaja.USUARIO_SOL_EMPRESA, CPEBaja.PASS_SOL_EMPRESA, nomARCHIVO, ruta, url, objRespuesta.DigestValue);
                    break;
                case 1:
                    ruta = ObtenerRutaProduccion(xconexion);
                    rutaFirma = ResolverRutaFirma(CPEBaja.RUTA_PFX);
                    objXML.ResumenBaja(CPEBaja, nomARCHIVO, ruta);
                    objPregunta.ruta_Firma = rutaFirma;
                    objPregunta.contra_Firma = CPEBaja.CONTRA_FIRMA;
                    objPregunta.ruta_xml = ruta + nomARCHIVO + ".XML";
                    objPregunta.flg_firma = 0;
                    objRespuesta = objSignature.FirmaXMl(objPregunta);
                    dictionary = objENV.EnvioResumen(CPEBaja.NRO_DOCUMENTO_EMPRESA, CPEBaja.USUARIO_SOL_EMPRESA, CPEBaja.PASS_SOL_EMPRESA, nomARCHIVO, ruta, url, objRespuesta.DigestValue);
                    break;
            }
            return dictionary;
        }
        public Dictionary<string, string> ConsultaTicket(BE.CONSULTA_TICKET CPETicket)
        {
            Dictionary<string, string> dictionary = null;
            string nomARCHIVO = "";
            string ruta = "";
            string url = "";
            nomARCHIVO = CPETicket.NRO_DOCUMENTO_EMPRESA + "-" + CPETicket.TIPO_DOCUMENTO + "-" + CPETicket.NRO_DOCUMENTO;
            url = ResolverUrlOce(CPETicket.TIPO_PROCESO);
            switch (CPETicket.TIPO_PROCESO)
            {
                case 3:
                    ruta = ObtenerRutaBeta(xconexion);
                    dictionary = objENV.ConsultaTicket(CPETicket.NRO_DOCUMENTO_EMPRESA, CPETicket.USUARIO_SOL_EMPRESA, CPETicket.PASS_SOL_EMPRESA, nomARCHIVO, ruta, url, objRespuesta.DigestValue, CPETicket.TICKET);
                    break;
                case 2:
                    ruta = ObtenerRutaHomologacion();
                    dictionary = objENV.ConsultaTicket(CPETicket.NRO_DOCUMENTO_EMPRESA, CPETicket.USUARIO_SOL_EMPRESA, CPETicket.PASS_SOL_EMPRESA, nomARCHIVO, ruta, url, objRespuesta.DigestValue, CPETicket.TICKET);
                    break;
                case 1:
                    ruta = ObtenerRutaProduccion(xconexion);
                    dictionary = objENV.ConsultaTicket(CPETicket.NRO_DOCUMENTO_EMPRESA, CPETicket.USUARIO_SOL_EMPRESA, CPETicket.PASS_SOL_EMPRESA, nomARCHIVO, ruta, url, objRespuesta.DigestValue, CPETicket.TICKET);
                    break;
            }
            return dictionary;
        }
    }
}






