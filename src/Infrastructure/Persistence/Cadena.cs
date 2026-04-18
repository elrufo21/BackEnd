using Ecommerce.Domain;

namespace Ecommerce.Infrastructure.Persistence;

public static class Cadena
{
    public static List<EGeneral> AlistaCampos(string data)
    {
        List<EGeneral> lista;
        lista = new List<EGeneral>();
        string[] registros = data.Split('¬');
        int nRegistros = registros.Length;
        string[] campos;
        for (int i = 0; i < nRegistros; i++)
        {
            campos = registros[i].Split('|');
            if (campos[0] == "~") break;
            else lista.Add(new EGeneral { Id = campos[0], Nombre = campos[1] });
        }
        return lista;
    }
    public static List<EListaProducto> AlistaCamposPro(string data)
    {
        List<EListaProducto> lista;
        lista = new List<EListaProducto>();
        string[] registros = data.Split('¬');
        int nRegistros = registros.Length;
        string[] campos;
        for (int i = 0; i < nRegistros; i++)
        {
            campos = registros[i].Split('|');
            if (campos[0] == "~") break;
            else lista.Add(new EListaProducto
            {
                Id = campos[0],
                Descripcion = campos[1],
                Precio = campos[2],
                Stock = campos[3],
                Imagen = campos[4].Contains("ArchivoSistema") ? "" : campos[4],
                Unidad = campos[5].Length > 2 ? campos[5].Substring(0, 3) : campos[5],
                ValorUM = Convert.ToDecimal(campos[6])
            });
        }
        return lista;
    }
    public static List<EListaTemporal> AlistaCamposTem(string data)
    {
        List<EListaTemporal> lista;
        lista = new List<EListaTemporal>();
        string[] registros = data.Split('¬');
        int nRegistros = registros.Length;
        string[] campos;
        for (int i = 0; i < nRegistros; i++)
        {
            campos = registros[i].Split('|');
            if (campos[0] == "~") break;
            else lista.Add(new EListaTemporal
            {
                Id = campos[0],
                productId = campos[1],
                Cantidad = campos[2],
                Unidad = campos[3],
                Producto = campos[4],
                Precio = campos[5],
                Importe = campos[6],
                Imagen = campos[7].Contains("ArchivoSistema") ? "" : campos[7],
                ValorUM = campos[8],
                Costo = campos[9],
                SubTotal = campos[10],
                IGV = campos[11],
                PrecioB = campos[12],
            });
        }
        return lista;
    }

    public static List<EListaNota> AlistaCamposNota(string data)
    {
        List<EListaNota> lista;
        lista = new List<EListaNota>();
        string[] registros = data.Split('¬');
        int nRegistros = registros.Length;
        string[] campos;

        static string GetCampo(string[] values, int index)
            => index >= 0 && index < values.Length ? values[index] : string.Empty;

        for (int i = 0; i < nRegistros; i++)
        {
            campos = registros[i].Split('|');
            if (campos.Length == 0 || campos[0] == "~") break;

            // Formato completo actual de dbo.listaNotaPedido:
            // 0..43 (44 campos)
            if (campos.Length >= 44)
            {
                lista.Add(new EListaNota
                {
                    NotaId = GetCampo(campos, 0),
                    Documento = GetCampo(campos, 1),
                    Fecha = GetCampo(campos, 13),
                    Cliente = GetCampo(campos, 3),
                    FormaPago = GetCampo(campos, 15),
                    Total = GetCampo(campos, 23),
                    Acuenta = GetCampo(campos, 24),
                    Saldo = GetCampo(campos, 25),
                    Usuario = GetCampo(campos, 14),
                    Estado = GetCampo(campos, 29),

                    ClienteId = GetCampo(campos, 2),
                    ClienteRazon = GetCampo(campos, 3),
                    ClienteRuc = GetCampo(campos, 4),
                    ClienteDni = GetCampo(campos, 5),
                    ClienteDireccion = GetCampo(campos, 6),
                    ClienteTelefono = GetCampo(campos, 7),
                    ClienteCorreo = GetCampo(campos, 8),
                    ClienteEstado = GetCampo(campos, 9),
                    ClienteDespacho = GetCampo(campos, 10),
                    ClienteUsuario = GetCampo(campos, 11),
                    ClienteFecha = GetCampo(campos, 12),
                    NotaFecha = GetCampo(campos, 13),
                    NotaUsuario = GetCampo(campos, 14),
                    NotaFormaPago = GetCampo(campos, 15),
                    NotaCondicion = GetCampo(campos, 16),
                    NotaFechaPago = GetCampo(campos, 17),
                    NotaDireccion = GetCampo(campos, 18),
                    NotaTelefono = GetCampo(campos, 19),
                    NotaSubtotal = GetCampo(campos, 20),
                    NotaMovilidad = GetCampo(campos, 21),
                    NotaDescuento = GetCampo(campos, 22),
                    NotaTotal = GetCampo(campos, 23),
                    NotaAcuenta = GetCampo(campos, 24),
                    NotaSaldo = GetCampo(campos, 25),
                    NotaAdicional = GetCampo(campos, 26),
                    NotaTarjeta = GetCampo(campos, 27),
                    NotaPagar = GetCampo(campos, 28),
                    NotaEstado = GetCampo(campos, 29),
                    CompaniaId = GetCampo(campos, 30),
                    NotaEntrega = GetCampo(campos, 31),
                    ModificadoPor = GetCampo(campos, 32),
                    FechaEdita = GetCampo(campos, 33),
                    NotaConcepto = GetCampo(campos, 34),
                    NotaSerie = GetCampo(campos, 35),
                    NotaNumero = GetCampo(campos, 36),
                    NotaGanancia = GetCampo(campos, 37),
                    Icbper = GetCampo(campos, 38),
                    CajaId = GetCampo(campos, 39),
                    EntidadBancaria = GetCampo(campos, 40),
                    NroOperacion = GetCampo(campos, 41),
                    Efectivo = GetCampo(campos, 42),
                    Deposito = GetCampo(campos, 43)
                });
                continue;
            }

            // Formato antiguo:
            // NotaId|Documento|Fecha|Cliente|FormaPago|Total|Usuario|Estado
            if (campos.Length >= 8)
            {
                lista.Add(new EListaNota
                {
                    NotaId = campos[0],
                    Documento = campos[1],
                    Fecha = campos[2],
                    Cliente = campos[3],
                    FormaPago = campos[4],
                    Total = campos[5],
                    Usuario = campos[6],
                    Estado = campos[7]
                });
                continue;
            }

            if (campos.Length >= 7)
            {
                // Formato legado reducido:
                // NotaId|Fecha|Cliente|Total|Acuenta|Saldo|Estado
                lista.Add(new EListaNota
                {
                    NotaId = campos[0],
                    Fecha = campos[1],
                    Cliente = campos[2],
                    Total = campos[3],
                    Acuenta = campos[4],
                    Saldo = campos[5],
                    Estado = campos[6]
                });
            }
        }
        return lista;
    }
}
