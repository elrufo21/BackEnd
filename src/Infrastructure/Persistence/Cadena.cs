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
        for (int i = 0; i < nRegistros; i++)
        {
            campos = registros[i].Split('|');
            if (campos[0] == "~") break;
            else lista.Add(new EListaNota
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
        }
        return lista;
    }
}
