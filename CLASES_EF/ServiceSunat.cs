using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
namespace CPEEnvio
{
    public class ServiceSunat
    {
        public Dictionary<string, string> Envio(string ruc, string usu_sol, string contra_sol, string nombre_archivo, string rutaArchivo, string url, string hash_cpe)
        {
            Dictionary<string, string> dictionary = null;
            try
            {
                XmlDocument doc = new XmlDocument();
                string strCDR = "";
                string strSOAP = "";
                string rutaCompleta = rutaArchivo + nombre_archivo;
                VariablesGlobales.Comprimir(nombre_archivo, rutaArchivo);
                string rutaCdr = rutaArchivo + "R-" + nombre_archivo + ".ZIP";
                string NomFichierZIP = Path.GetFileName(rutaCompleta + ".ZIP");
                byte[] data = File.ReadAllBytes(rutaCompleta + ".ZIP");
                strSOAP = "<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' ";
                strSOAP += "xmlns:ser='http://service.sunat.gob.pe' ";
                strSOAP += "xmlns:wsse='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'> ";
                strSOAP += "<soapenv:Header> ";
                strSOAP += "<wsse:Security> ";
                strSOAP += "<wsse:UsernameToken> ";
                strSOAP += "<wsse:Username>" + ruc + usu_sol + "</wsse:Username> ";
                strSOAP += "<wsse:Password>" + contra_sol + "</wsse:Password> ";
                strSOAP += "</wsse:UsernameToken> ";
                strSOAP += "</wsse:Security> ";
                strSOAP += "</soapenv:Header> ";
                strSOAP += "<soapenv:Body> ";
                strSOAP += "<ser:sendBill> ";
                strSOAP += "<fileName>" + NomFichierZIP + "</fileName> ";
                strSOAP += "<contentFile>" + Convert.ToBase64String(data) + "</contentFile> ";
                strSOAP += "</ser:sendBill> ";
                strSOAP += "</soapenv:Body> ";
                strSOAP += "</soapenv:Envelope>";
                string returned_value = "";
                string strPostData = "";
                strPostData = strSOAP;
                Type tipo = Type.GetTypeFromProgID("MSXML2.ServerXMLHTTP");
                if (tipo != null)
                {
                    dynamic objRequest = Activator.CreateInstance(tipo);
                    if (objRequest != null)
                    {
                        objRequest.Open("POST", url, false);
                        objRequest.setRequestHeader("Content-Type", "application/xml");
                        objRequest.send(strPostData);
                        returned_value = objRequest.responseText;
                    }
                }
                doc.LoadXml(returned_value);
                XmlNodeList Lst = doc.SelectNodes("//faultcode");
                if (Lst != null && Lst.Count > 0)
                {
                    dictionary = new Dictionary<string, string>();
                    dictionary.Add("flg_rta", "0");
                    dictionary.Add("mensaje", "ERROR AL ENVIAR A LA SUNAT");
                    dictionary.Add("cod_sunat", doc.SelectSingleNode("//faultstring").InnerText.Replace("soap-env:Client.", ""));
                    dictionary.Add("msj_sunat", doc.SelectSingleNode("//message").InnerText);
                    dictionary.Add("hash_cdr", "");
                    dictionary.Add("hash_cpe", hash_cpe);
                    dictionary.Add("cdr_base64", "");
                }
                else
                {
                    strCDR = doc.SelectSingleNode("//applicationResponse").InnerText;
                    byte[] byteCDR = Convert.FromBase64String(strCDR);
                    FileStream s = File.Open(rutaCdr, FileMode.Append);
                    s.Write(byteCDR, 0, byteCDR.Length);
                    s.Close();
                    VariablesGlobales.Descomprimir(rutaArchivo, "R-" + nombre_archivo);
                    XmlDocument xmlCDR = new XmlDocument();
                    string rutaxmlCDR = rutaArchivo + "R-" + nombre_archivo + ".XML";
                    xmlCDR.Load(rutaxmlCDR);
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                    nsmgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                    XmlNamespaceManager nsmgrSing = new XmlNamespaceManager(doc.NameTable);
                    nsmgrSing.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
                    dictionary = new Dictionary<string, string>();
                    dictionary.Add("flg_rta", "1");
                    dictionary.Add("mensaje", "COMPROBANTE ENVIADO CORRECTAMENTE");
                    dictionary.Add("cod_sunat", xmlCDR.SelectSingleNode("//cbc:ResponseCode", nsmgr).InnerText);
                    dictionary.Add("msj_sunat", xmlCDR.SelectSingleNode("//cbc:Description", nsmgr).InnerText.ToUpper());
                    dictionary.Add("hash_cdr", xmlCDR.SelectSingleNode("//ds:DigestValue", nsmgrSing).InnerText);
                    dictionary.Add("hash_cpe", hash_cpe);
                    dictionary.Add("cdr_base64", strCDR);
                    File.Delete(rutaCompleta + ".ZIP");
                }
            }
            catch (Exception ex)
            {
                dictionary = new Dictionary<string, string>();
                dictionary.Add("flg_rta", "0");
                dictionary.Add("mensaje", "ERROR AL CONECTARSE A LA SUNAT: " + ex.Message);
                dictionary.Add("cod_sunat", "");
                dictionary.Add("msj_sunat", "");
                dictionary.Add("hash_cdr", "");
                dictionary.Add("hash_cpe", hash_cpe);
                dictionary.Add("cdr_base64", "");
            }
            return dictionary;
        }

        public Dictionary<string, string> EnvioResumen(string ruc, string usu_sol, string contra_sol, string nombre_archivo, string rutaArchivo, string url, string hash_cpe)
        {
            Dictionary<string, string> dictionary = null;
            try
            {
                XmlDocument doc = new XmlDocument();
                string ticket = "";
                string strSOAP = "";
                string rutaCompleta = rutaArchivo + nombre_archivo;
                VariablesGlobales.Comprimir(nombre_archivo, rutaArchivo);
                string rutaCdr = rutaArchivo + "R-" + nombre_archivo + ".ZIP";
                string NomFichierZIP = Path.GetFileName(rutaCompleta + ".ZIP");
                byte[] data = File.ReadAllBytes(rutaCompleta + ".ZIP");
                strSOAP = "<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' ";
                strSOAP += "xmlns:ser='http://service.sunat.gob.pe' ";
                strSOAP += "xmlns:wsse='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'> ";
                strSOAP += "<soapenv:Header> ";
                strSOAP += "<wsse:Security> ";
                strSOAP += "<wsse:UsernameToken> ";
                strSOAP += "<wsse:Username>" + ruc + usu_sol + "</wsse:Username> ";
                strSOAP += "<wsse:Password>" + contra_sol + "</wsse:Password> ";
                strSOAP += "</wsse:UsernameToken> ";
                strSOAP += "</wsse:Security> ";
                strSOAP += "</soapenv:Header> ";
                strSOAP += "<soapenv:Body> ";
                strSOAP += "<ser:sendSummary> ";
                strSOAP += "<fileName>" + NomFichierZIP + "</fileName> ";
                strSOAP += "<contentFile>" + Convert.ToBase64String(data) + "</contentFile> ";
                strSOAP += "</ser:sendSummary> " + "</soapenv:Body> " + "</soapenv:Envelope>";
                string returned_value = "";
                string strPostData = "";
                strPostData = strSOAP;
                Type tipo = Type.GetTypeFromProgID("MSXML2.ServerXMLHTTP");
                if (tipo != null)
                {
                    dynamic objRequest = Activator.CreateInstance(tipo);
                    if (objRequest != null)
                    {
                        objRequest.Open("POST", url, false);
                        objRequest.setRequestHeader("Content-Type", "application/xml");
                        objRequest.send(strPostData);
                        returned_value = objRequest.responseText;
                    }
                }
                if (string.IsNullOrWhiteSpace(returned_value))
                {
                    throw new Exception("Respuesta vacia del servicio SOAP.");
                }
                doc.LoadXml(returned_value);
                XmlNodeList Lst = doc.SelectNodes("//faultcode");
                if (Lst != null && Lst.Count > 0)
                {
                    var fault = LeerNodo(doc, "//faultstring");
                    var detalle = LeerNodo(doc, "//message");
                    if (string.IsNullOrWhiteSpace(detalle))
                    {
                        detalle = fault;
                    }
                    dictionary = new Dictionary<string, string>();
                    dictionary.Add("flg_rta", "0");
                    dictionary.Add("mensaje", "ERROR AL ENVIAR A LA SUNAT");
                    dictionary.Add("cod_sunat", (fault ?? string.Empty).Replace("soap-env:Client.", ""));
                    dictionary.Add("msj_sunat", detalle ?? string.Empty);
                    dictionary.Add("hash_cdr", "");
                    dictionary.Add("hash_cpe", hash_cpe);
                }
                else
                {
                    ticket = LeerNodo(doc, "//ticket");
                    if (string.IsNullOrWhiteSpace(ticket))
                    {
                        var statusCode = LeerNodo(doc, "//statusCode");
                        var statusMessage = LeerNodo(doc, "//statusMessage");
                        var fault = LeerNodo(doc, "//faultstring");

                        dictionary = new Dictionary<string, string>();
                        dictionary.Add("flg_rta", "0");
                        dictionary.Add("mensaje", "RESPUESTA SIN TICKET DEL SERVICIO");
                        dictionary.Add("cod_sunat", statusCode ?? string.Empty);
                        dictionary.Add("msj_sunat",
                            !string.IsNullOrWhiteSpace(statusMessage) ? statusMessage :
                            !string.IsNullOrWhiteSpace(fault) ? fault :
                            "No se obtuvo ticket en la respuesta SOAP.");
                        dictionary.Add("hash_cdr", "");
                        dictionary.Add("hash_cpe", hash_cpe);
                        return dictionary;
                    }
                    dictionary = new Dictionary<string, string>();
                    dictionary.Add("flg_rta", "1");
                    dictionary.Add("mensaje", "COMPROBANTE ENVIADO CORRECTAMENTE");
                    dictionary.Add("cod_sunat", "");
                    dictionary.Add("msj_sunat", ticket);
                    dictionary.Add("hash_cdr", "");
                    dictionary.Add("hash_cpe", hash_cpe);
                    File.Delete(rutaCompleta + ".ZIP");
                }
            }
            catch (Exception ex)
            {
                dictionary = new Dictionary<string, string>();
                dictionary.Add("flg_rta", "0");
                dictionary.Add("mensaje", "ERROR AL CONECTARSE A LA SUNAT: " + ex.Message);
                dictionary.Add("cod_sunat", "");
                dictionary.Add("msj_sunat", "");
                dictionary.Add("hash_cdr", "");
                dictionary.Add("hash_cpe", hash_cpe);
            }
            return dictionary;
        }

        private static string LeerNodo(XmlDocument doc, string xpath)
        {
            var node = doc.SelectSingleNode(xpath);
            return node?.InnerText?.Trim() ?? string.Empty;
        }

        public Dictionary<string, string> ConsultaTicket(string ruc, string usu_sol, string contra_sol, string nombre_archivo, string rutaArchivo, string url, string hash_cdr, string ticket)
        {
            Dictionary<string, string> dictionary = null;
            try
            {
                XmlDocument doc = new XmlDocument();
                string strCDR = "";
                string strSOAP = "";
                string rutaCdr = rutaArchivo + "R-" + nombre_archivo + ".ZIP";
                strSOAP = "<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' ";
                strSOAP += "xmlns:ser='http://service.sunat.gob.pe' ";
                strSOAP += "xmlns:wsse='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'> ";
                strSOAP += "<soapenv:Header> ";
                strSOAP += "<wsse:Security> ";
                strSOAP += "<wsse:UsernameToken> ";
                strSOAP += "<wsse:Username>" + ruc + usu_sol + "</wsse:Username> ";
                strSOAP += "<wsse:Password>" + contra_sol + "</wsse:Password> ";
                strSOAP += "</wsse:UsernameToken> ";
                strSOAP += "</wsse:Security> ";
                strSOAP += "</soapenv:Header> ";
                strSOAP += "<soapenv:Body> ";
                strSOAP += "<ser:getStatus> ";
                strSOAP += "<ticket>" + ticket + "</ticket> ";
                strSOAP += "</ser:getStatus>";
                strSOAP += "</soapenv:Body> ";
                strSOAP += "</soapenv:Envelope>";
                string returned_value = "";
                string strPostData = "";
                strPostData = strSOAP;
                Type tipo = Type.GetTypeFromProgID("MSXML2.ServerXMLHTTP");
                if (tipo != null)
                {
                    dynamic objRequest = Activator.CreateInstance(tipo);
                    if (objRequest != null)
                    {
                        objRequest.Open("POST", url, false);
                        objRequest.setRequestHeader("Content-Type", "application/xml");
                        objRequest.send(strPostData);
                        returned_value = objRequest.responseText;
                    }
                }
                doc.LoadXml(returned_value);
                XmlNodeList Lst = doc.SelectNodes("//faultcode");
                if (Lst != null && Lst.Count > 0)
                {
                    dictionary = new Dictionary<string, string>();
                    dictionary.Add("flg_rta", "0");
                    dictionary.Add("mensaje", "ERROR AL ENVIAR A LA SUNAT");
                    dictionary.Add("cod_sunat", doc.SelectSingleNode("//faultstring").InnerText.Replace("soap-env:Client.", ""));
                    dictionary.Add("msj_sunat", doc.SelectSingleNode("//message").InnerText);
                    dictionary.Add("hash_cdr", "");
                    dictionary.Add("hash_cpe", "");
                    dictionary.Add("cdr_base64", "");
                }
                else
                {
                    strCDR = doc.SelectSingleNode("//content").InnerText;
                    byte[] byteCDR = Convert.FromBase64String(strCDR);
                    FileStream s = File.Open(rutaCdr, FileMode.Append);
                    s.Write(byteCDR, 0, byteCDR.Length);
                    s.Position = 0;
                    s.Close();
                    VariablesGlobales.DescomprimirB(rutaArchivo, "R-" + nombre_archivo);
                    XmlDocument xmlCDR = new XmlDocument();
                    string rutaxmlCDR = rutaArchivo + "R-" + nombre_archivo + ".XML";
                    xmlCDR.Load(rutaxmlCDR);
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                    nsmgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                    XmlNamespaceManager nsmgrSing = new XmlNamespaceManager(doc.NameTable);
                    nsmgrSing.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
                    dictionary = new Dictionary<string, string>();
                    dictionary.Add("flg_rta", "1");
                    dictionary.Add("mensaje", "COMPROBANTE ENVIADO CORRECTAMENTE");
                    dictionary.Add("cod_sunat", xmlCDR.SelectSingleNode("//cbc:ResponseCode", nsmgr).InnerText);
                    dictionary.Add("msj_sunat", xmlCDR.SelectSingleNode("//cbc:Description", nsmgr).InnerText.ToUpper());
                    dictionary.Add("hash_cdr", xmlCDR.SelectSingleNode("//ds:DigestValue", nsmgrSing).InnerText);
                    dictionary.Add("hash_cpe", "");
                    dictionary.Add("cdr_base64", strCDR);
                }
            }
            catch (Exception ex)
            {
                dictionary = new Dictionary<string, string>();
                dictionary.Add("flg_rta", "0");
                dictionary.Add("mensaje", "ERROR AL CONECTARSE A LA SUNAT: " + ex.Message);
                dictionary.Add("cod_sunat", "");
                dictionary.Add("msj_sunat", "");
                dictionary.Add("hash_cdr", "");
                dictionary.Add("hash_cpe", "");
                dictionary.Add("cdr_base64", "");
            }
            return dictionary;
        }
        //======================CONSULTA DE FACTURAS ELECTRONICAS================
        public Dictionary<string, string> getStatusFactura(string ruc, string usu_sol, string contra_sol, string url, string ruc_emisor, string tipo_comprobante, string serie, string numero)
        {
            Dictionary<string, string> dictionary;
            try
            {
                XmlDocument doc = new XmlDocument();
                string strSOAP;
                strSOAP = "<soapenv:Envelope "+
                    "xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' " +
                    "xmlns:ser='http://service.sunat.gob.pe' " + 
                    "xmlns:wsse='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'> " +
                    "<soapenv:Header><wsse:Security><wsse:UsernameToken><wsse:Username>" + ruc + usu_sol + 
                    "</wsse:Username><wsse:Password>" + contra_sol + "</wsse:Password>" + 
                    "</wsse:UsernameToken></wsse:Security></soapenv:Header><soapenv:Body> " +
                    "<ser:getStatusCdr><rucComprobante>" + ruc_emisor + "</rucComprobante><tipoComprobante>" +
                    tipo_comprobante + "</tipoComprobante><serieComprobante>" + serie + 
                    "</serieComprobante><numeroComprobante>" + numero +
                    "</numeroComprobante></ser:getStatusCdr></soapenv:Body></soapenv:Envelope>";
                string returned_value = "";
                string strPostData = "";
                strPostData = strSOAP;
                Type tipo = Type.GetTypeFromProgID("MSXML2.ServerXMLHTTP");
                if (tipo != null)
                {
                    dynamic objRequest = Activator.CreateInstance(tipo);
                    if (objRequest != null)
                    {
                        objRequest.Open("POST", url, false);
                        objRequest.setRequestHeader("Content-Type", "text/xml");
                        objRequest.send(strPostData);
                        returned_value = objRequest.responseText;
                    }
                }
                doc.LoadXml(returned_value);
                // =======================validando respuesta========================
                XmlNodeList Lst = doc.SelectNodes("//faultcode");
                if (Lst != null && Lst.Count > 0)
                {
                    dictionary = new Dictionary<string, string>();
                    dictionary.Add("flg_rta", "0");
                    dictionary.Add("mensaje", "ERROR AL CONSULTAR EN LA SUNAT");
                    dictionary.Add("cod_sunat", doc.SelectSingleNode("//faultstring").InnerText.Replace("soap-env:Client.", ""));
                    dictionary.Add("msj_sunat", doc.SelectSingleNode("//message").InnerText);
                    dictionary.Add("hash_cdr", "");
                    dictionary.Add("hash_cpe", "");
                }
                else
                {
                    string statuCode;
                    string statuMensaje;

                    statuCode = doc.SelectSingleNode("//statusCode").InnerText;
                    statuMensaje = doc.SelectSingleNode("//statusMessage").InnerText;

                    // ========================asignamos valores de retorno======================
                    dictionary = new Dictionary<string, string>();
                    dictionary.Add("flg_rta", "1");
                    dictionary.Add("mensaje", "COMPROBANTE CONSULTADO CORRECTAMENTE");
                    dictionary.Add("cod_sunat", statuCode);
                    dictionary.Add("msj_sunat", statuMensaje);
                    dictionary.Add("hash_cdr", "");
                    dictionary.Add("hash_cpe", "");
                }
            }
            catch (Exception ex)
            {
                dictionary = new Dictionary<string, string>();
                dictionary.Add("flg_rta", "0");
                dictionary.Add("mensaje", "ERROR AL CONECTARSE A LA SUNAT: " + ex.Message);
                dictionary.Add("cod_sunat", "");
                dictionary.Add("msj_sunat", "");
                dictionary.Add("hash_cdr", "");
                dictionary.Add("hash_cpe", "");
            }
            return dictionary;
        }
        public Dictionary<string, string> getStatusCDR(string ruc, string usu_sol, string contra_sol, string nombre_archivo, string rutaArchivo, string url, string ruc_emisor, string tipo_comprobante, string serie, string numero)
        {
            Dictionary<string, string> dictionary;
            try
            {
                XmlDocument doc = new XmlDocument();
                string strCDR;
                string strSOAP;
                string rutaCdr = rutaArchivo + "R-" + nombre_archivo + ".ZIP";

                strSOAP = @"<SOAP-ENV:Envelope
                        xmlns:SOAP-ENV='http://schemas.xmlsoap.org/soap/envelope/'
                        xmlns:SOAP-ENC='http://schemas.xmlsoap.org/soap/encoding/'
                        xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'
                        xmlns:xsd='http://www.w3.org/2001/XMLSchema'
                        xmlns:wsse='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'>
                        <SOAP-ENV:Header
                            xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope'>
                            <wsse:Security>
                                <wsse:UsernameToken>
                                    <wsse:Username>" + ruc + usu_sol + @"</wsse:Username>
                                    <wsse:Password>" + contra_sol + @"</wsse:Password>
                                </wsse:UsernameToken>
                            </wsse:Security>
                        </SOAP-ENV:Header>
                        <SOAP-ENV:Body>
                            <m:getStatusCdr
                                xmlns:m='http://service.sunat.gob.pe'>
                                <rucComprobante>" + ruc_emisor + @"</rucComprobante>
                                <tipoComprobante>" + tipo_comprobante + @"</tipoComprobante>
                                <serieComprobante>" + serie + @"</serieComprobante>
                                <numeroComprobante>" + numero + @"</numeroComprobante>
                            </m:getStatusCdr>
                        </SOAP-ENV:Body>
                    </SOAP-ENV:Envelope>";
                string returned_value = "";
                string strPostData = "";

                strPostData = strSOAP; //VariablesGlobales.PrettyXML(strSOAP);
                Type tipo = Type.GetTypeFromProgID("MSXML2.ServerXMLHTTP");
                if (tipo != null)
                {
                    dynamic objRequest = Activator.CreateInstance(tipo);
                    if (objRequest != null)
                    {
                        objRequest.Open("POST", url, false);
                        objRequest.setRequestHeader("Content-Type", "text/xml");
                        objRequest.send(strPostData);
                        returned_value = objRequest.responseText;
                    }
                }
                doc.LoadXml(returned_value);

                // =======================validando respuesta========================
                XmlNodeList Lst = doc.SelectNodes("//faultcode");
                if (Lst != null && Lst.Count > 0)
                {
                    dictionary = new Dictionary<string, string>();
                    dictionary.Add("flg_rta", "0");
                    dictionary.Add("mensaje", "ERROR AL ENVIAR A LA SUNAT");
                    dictionary.Add("cod_sunat", doc.SelectSingleNode("//faultcode").InnerText.Replace("ns0:", ""));
                    dictionary.Add("msj_sunat", doc.SelectSingleNode("//faultstring").InnerText);
                    dictionary.Add("hash_cdr", "");
                    dictionary.Add("hash_cpe", "");
                }
                else
                {
                    string statuCode;
                    statuCode = doc.SelectSingleNode("//statusCode").InnerText;
                    if (statuCode == "0")//statuCode == "0004"
                    {
                        strCDR = doc.SelectSingleNode("//content").InnerText;
                        byte[] byteCDR = Convert.FromBase64String(strCDR);
                        System.IO.FileStream s;
                        s = System.IO.File.Open(rutaCdr, System.IO.FileMode.Append);
                        s.Write(byteCDR, 0, byteCDR.Length);
                        s.Close();

                        // ===============descomprimo el xml=============
                        VariablesGlobales.DescomprimirB(rutaArchivo, "R-" + nombre_archivo);
                        // ================================================================
                        XmlDocument xmlCDR = new XmlDocument();
                        var rutaxmlCDR = rutaArchivo + "R-" + nombre_archivo + ".XML";
                        xmlCDR.Load(rutaxmlCDR);

                        // =======================nombre de espacios para obtener los valores del xml======================
                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                        nsmgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                        XmlNamespaceManager nsmgrSing = new XmlNamespaceManager(doc.NameTable);
                        nsmgrSing.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");

                        // ========================asignamos valores de retorno======================
                        dictionary = new Dictionary<string, string>();
                        dictionary.Add("flg_rta", "1");
                        dictionary.Add("mensaje", "COMPROBANTE ENVIADO CORRECTAMENTE");
                        dictionary.Add("cod_sunat", xmlCDR.SelectSingleNode("//cbc:ResponseCode", nsmgr).InnerText);
                        dictionary.Add("msj_sunat", xmlCDR.SelectSingleNode("//cbc:Description", nsmgr).InnerText.ToUpper());
                        dictionary.Add("hash_cdr", xmlCDR.SelectSingleNode("//ds:DigestValue", nsmgrSing).InnerText);
                        dictionary.Add("hash_cpe", "");
                    }
                    else
                    {
                        dictionary = new Dictionary<string, string>();
                        dictionary.Add("flg_rta", "0");
                        dictionary.Add("mensaje", "ERROR AL CONSULTAR STATUS CDR");
                        dictionary.Add("cod_sunat", doc.SelectSingleNode("//statusCode").InnerText);
                        dictionary.Add("msj_sunat", doc.SelectSingleNode("//statusMessage").InnerText.ToUpper());
                        dictionary.Add("hash_cdr", "");
                        dictionary.Add("hash_cpe", "");
                    }
                }
            }
            catch (Exception ex)
            {
                dictionary = new Dictionary<string, string>();
                dictionary.Add("flg_rta", "0");
                dictionary.Add("mensaje", "ERROR AL CONECTARSE A LA SUNAT: " + ex.Message);
                dictionary.Add("cod_sunat", "");
                dictionary.Add("msj_sunat", "");
                dictionary.Add("hash_cdr", "");
                dictionary.Add("hash_cpe", "");
            }
            return dictionary;
        }
    }
}
