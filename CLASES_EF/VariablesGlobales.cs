using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksum;
using System.IO;
using System.Xml;
using System;
namespace CPEEnvio
{
    public class VariablesGlobales
    {
        public static void Comprimir(string NOM_ARHIVO, string RUTA)
        {
            Crc32 objCrc32 = new Crc32();
            ZipOutputStream strmZipOutputStream;
            strmZipOutputStream = new ZipOutputStream(File.Create(RUTA + NOM_ARHIVO + ".ZIP"));
            strmZipOutputStream.SetLevel(6);
            FileStream strmFile = File.OpenRead(RUTA + NOM_ARHIVO + ".XML");
            byte[] abyBuffer = new byte[(int)strmFile.Length];
            strmFile.Read(abyBuffer, 0, abyBuffer.Length);
            ZipEntry theEntry = new ZipEntry(NOM_ARHIVO + ".XML");
            FileInfo fi = new FileInfo(NOM_ARHIVO + ".XML");
            theEntry.DateTime = fi.LastWriteTime;
            theEntry.Size = strmFile.Length;
            strmFile.Close();
            objCrc32.Reset();
            objCrc32.Update(abyBuffer);
            theEntry.Crc = objCrc32.Value;
            strmZipOutputStream.PutNextEntry(theEntry);
            strmZipOutputStream.Write(abyBuffer, 0, abyBuffer.Length);
            strmZipOutputStream.Finish();
            strmZipOutputStream.Close();
        }
        public static void Descomprimir(string RUTA,string NOM_ARHIVO = "")
        {
            ZipInputStream z = new ZipInputStream(File.OpenRead(RUTA + NOM_ARHIVO + ".ZIP"));
            ZipEntry theEntry = null;
            try
            {
                while (true)
                {
                    theEntry = z.GetNextEntry();
                    if (theEntry != null)
                    {
                        string fileName = theEntry.Name;
                        FileStream streamWriter = null;
                        try
                        {
                            if (fileName != "dummy/")
                            {
                                streamWriter = File.Create(RUTA + fileName);
                                int size;
                                byte[] data = new byte[2049];
                                while (true)
                                {
                                    size = z.Read(data, 0, data.Length);
                                    if (size > 0) streamWriter.Write(data, 0, size);
                                    else break;
                                }
                                streamWriter.Close();
                            }
                        }
                        catch (DirectoryNotFoundException ex)
                        {
                            ex.ToString();
                            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                            streamWriter = File.Create(fileName);
                        }
                    }
                    else break;
                }
            }catch(Exception ex)
            {
                ex.ToString();
            }
            z.Close();
            File.Delete(RUTA + NOM_ARHIVO + ".ZIP");

        }
        public static void DescomprimirB(string RUTA, string NOM_ARHIVO = "")
        {
            ZipInputStream z = new ZipInputStream(File.OpenRead(RUTA + NOM_ARHIVO + ".ZIP"));
            ZipEntry theEntry = null;
            theEntry = z.GetNextEntry();
            if (theEntry != null)
            {
                string fileName = theEntry.Name;
                FileStream streamWriter = null;
                try
                {
                    if (fileName != "dummy/")
                    {
                        streamWriter = File.Create(RUTA + fileName);
                        int size;
                        byte[] data = new byte[2049];
                        while (true)
                        {
                            size = z.Read(data, 0, data.Length);
                            if (size > 0) streamWriter.Write(data, 0, size);
                            else break;
                        }
                        streamWriter.Close();
                    }
                }
                catch (DirectoryNotFoundException ex)
                {
                    ex.ToString();
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                    streamWriter = File.Create(fileName);
                }
            }
            z.Close();
            File.Delete(RUTA + NOM_ARHIVO + ".ZIP");
        }
        public static string PrettyXML(string XMLString)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xw.Formatting = Formatting.Indented;
            xw.Indentation = 4;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XMLString);
            doc.Save(xw);
            return sw.ToString();
        }
    }
}
