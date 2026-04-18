using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MegaRosita.Capa.Comun;
using ClosedXML.Excel;
using System.Diagnostics;
namespace MegaRosita.Capa.Aplicacion
{
    public partial class LDdocumento : Form
    {
        private string[] lista;
        private List<string> filtro;
        private string cabeceraTexto;
        DataTable tabla;
        private string[] listaB;
        private List<string> filtroB;
        private string cabeceraTextoB;
        DataTable tablaB;
        string xrut = string.Empty;
        Conexion xconexion = new Conexion();
        public LDdocumento()
        {
            InitializeComponent();
        }
        #region Ventas
        private void filtrar(object sender, EventArgs e)
        {
            try
            {
                borrarLisView();
                filtro.Clear();
                tabla.Clear();
                DataRow drw = null;
                Control.ControlCollection textos = lvwproducto.Controls;
                int ntexto = textos.Count;
                List<string> valores = new List<string>();
                for (int j = 0; j < ntexto; j++)
                {
                    valores.Add(((System.Windows.Forms.TextBox)lvwproducto.Controls[j]).Text.ToLower());
                }
                int nRegistros = lista.Length;
                string[] campos;
                int nCampos;
                bool exito;
                ListViewItem fila = null;
                lvwproducto.BeginUpdate();
                for (var i = 2; i < nRegistros; i++)
                {
                    campos = lista[i].Split('|');
                    nCampos = campos.Length;
                    exito = true;
                    for (int j = 0; j < nCampos; j++)
                    {
                        exito = (valores[j] == "" || campos[j].ToLower().Contains(valores[j]));
                        if (!exito) break;
                    }
                    if (exito)
                    {
                        drw = tabla.NewRow();
                        filtro.Add(lista[i]);
                        for (int j = 0; j < nCampos; j++)
                        {
                            if (j == 0) fila = lvwproducto.Items.Add(campos[j]);
                            else fila.SubItems.Add(campos[j].Replace("&amp;", "&"));
                            drw[j] = campos[j];
                        }
                        tabla.Rows.Add(drw);
                    }
                }
                lvwproducto.EndUpdate();
                this.lblcantidad.Text = String.Format("Items:  {0}", lvwproducto.Items.Count - 1);
                total();
            }
            catch (Exception ex) { ex.ToString(); }
        }
        private void borrarLisView()
        {
            lvwproducto.BeginUpdate();
            for (int i = lvwproducto.Items.Count - 1; i > 0; i--)
            {
                lvwproducto.Items.RemoveAt(i);
            }
            lvwproducto.EndUpdate();
        }
        private void borrarTextos()
        {
            Control.ControlCollection textos = lvwproducto.Controls;
            int ntexto = textos.Count;
            List<string> valores = new List<string>();
            for (int j = 0; j < ntexto; j++)
            {
                if (lvwproducto.Controls[j] is System.Windows.Forms.TextBox) ((System.Windows.Forms.TextBox)lvwproducto.Controls[j]).Clear();
            }
        }
        public void total()
        {
            double xsubtotal = 0;
            double xigv = 0;
            double xtotal = 0;
            double xsunat = 0;
            double xigvCom = 0;
            double xrenta = 0;
            double xIMPB = 0;
            double xtotalFinal = 0;
            xigvCom = double.Parse(lbligvC.Text);
            foreach (ListViewItem item in lvwproducto.Items)
            {
                if (item.SubItems[7].Text != "")
                {
                    xtotal += Convert.ToDouble(item.SubItems[6].Text) + Convert.ToDouble(item.SubItems[7].Text);
                    xIMPB += Convert.ToDouble(item.SubItems[8].Text);
                }
            }
            xsubtotal = (xtotal / 1.18);
            xigv = ((xtotal / 1.18) * 0.18);
            xsunat = (xigv - xigvCom);
            xrenta = ((xsubtotal) / 100);
            xtotalFinal = xtotal + xIMPB;
            lblsubtotal.Text = xsubtotal.ToString("N2");
            lbligv.Text = xigv.ToString("N2");
            lblICBPER.Text = xIMPB.ToString("N2");
            lbltotal.Text = xtotalFinal.ToString("N2");
            double a = xsunat;
            double b = (a % 1);
            if (b < 0.45) lblsunat.Text = Math.Round(a).ToString("#,#");
            else if (b >= 0.45) lblsunat.Text = Math.Ceiling(a).ToString("#,#");
            double c = xrenta;
            double d = (c % 1);
            if (d < 0.45) lblrenta.Text = Math.Round(c).ToString("#,#");
            else if (d >= 0.45) lblrenta.Text = Math.Ceiling(c).ToString("#,#");
        }
        public void listaVentas()
        {
            string xvalue = string.Empty;
            xvalue = dtimeinicio.Value.ToString("MM/dd/yyyy") + "|" + dtimefin.Value.ToString("MM/dd/yyyy");
            AccesoDatos daSQL = new AccesoDatos("con");
            string rpt = daSQL.ejecutarComando("LDdocumentos","@Data",xvalue);
            if(rpt.Length==0)
            {
                //
            }
            else
            {
                lista = rpt.Split('¬');
                filtro = rpt.Split('¬').ToList();
                string[] cabezeras = lista[0].Split('|');
                cabeceraTexto = String.Join(",", cabezeras);
                string[] anchos = lista[1].Split('|');
                int ancho;
                tabla = new System.Data.DataTable();
                for (int j = 0; j < cabezeras.Length; j++)
                {
                    ancho = int.Parse(anchos[j]);
                    lvwproducto.Columns.Add(cabezeras[j], cabezeras[j], ancho);
                    tabla.Columns.Add(cabezeras[j], Type.GetType("System.String"));
                    tabla.Columns[j].Caption = ancho.ToString();
                }
                lvwproducto.View = View.Details;
                lvwproducto.FullRowSelect = true;
                lvwproducto.GridLines = true;
                ListViewItem fila = null;
                System.Windows.Forms.TextBox txt;
                int total = 0;
                int altofila;
                for (int j = 0; j < cabezeras.Length; j++)
                {
                    if (j == 0) fila = lvwproducto.Items.Add("");
                    else fila.SubItems.Add("");
                    altofila = lvwproducto.Items[0].Bounds.Height;
                    ancho = int.Parse(anchos[j]);
                    txt = new System.Windows.Forms.TextBox();
                    txt.Width = ancho - 5;
                    txt.Height = 10;
                    txt.Font = new System.Drawing.Font("Arial", 9);
                    txt.SetBounds(total, altofila + 7, ancho, txt.Height);
                    txt.TextChanged += new EventHandler(filtrar);
                    lvwproducto.Controls.Add(txt);
                    total += ancho;
                }
                lvwproducto.Columns[6].TextAlign = HorizontalAlignment.Right;
                lvwproducto.Columns[7].TextAlign = HorizontalAlignment.Right;
                lvwproducto.Columns[8].TextAlign = HorizontalAlignment.Right;
                lvwproducto.Columns[9].TextAlign = HorizontalAlignment.Right;
                filtrar(null, null);
            }
        }
        #endregion
        #region compras
        private void borrarLisViewB()
        {
            lvwCompra.BeginUpdate();
            for (int i = lvwCompra.Items.Count - 1; i > 0; i--)
            {
                lvwCompra.Items.RemoveAt(i);
            }
            lvwCompra.EndUpdate();
        }
        private void borrarTextosB()
        {
            Control.ControlCollection textos = lvwCompra.Controls;
            int ntexto = textos.Count;
            List<string> valores = new List<string>();
            for (int j = 0; j < ntexto; j++)
            {
                if (lvwCompra.Controls[j] is System.Windows.Forms.TextBox) ((System.Windows.Forms.TextBox)lvwCompra.Controls[j]).Clear();
            }
        }
        public void totalB()
        {
            double xsubtotal = 0;
            double xigv = 0;
            double xtotal = 0;
            double xsunat = 0;
            double xigvVen = 0;
            xigvVen = double.Parse(lbligv.Text);
            foreach (ListViewItem item in lvwCompra.Items)
            {
                if (item.SubItems[5].Text != "")
                {
                    xtotal += Convert.ToDouble(item.SubItems[8].Text);
                }
            }
            xsubtotal = (xtotal / 1.18);
            xigv = ((xtotal / 1.18) * 0.18);
            xsunat = xigvVen - xigv;
            lblSubC.Text =xsubtotal.ToString("N2");
            lbligvC.Text = (xigv).ToString("N2");
            lblTotalC.Text = (xtotal).ToString("N2");
            double a = xsunat;
            double b = (a % 1);
            if (b < 0.45) lblsunat.Text = Math.Round(a).ToString("#,#");
            else if (b >= 0.45) lblsunat.Text = Math.Ceiling(a).ToString("#,#");
        }
        public void listaCompra()
        {
            string xvalue = string.Empty;
            xvalue = dtimeinicio.Value.ToString("MM/dd/yyyy") + "|" + dtimefin.Value.ToString("MM/dd/yyyy");
            AccesoDatos daSQL = new AccesoDatos("con");
            string rpt = daSQL.ejecutarComando("LDrptCompra","@Data",xvalue);
            if (rpt.Length == 0)
            {
                //
            }
            else
            {
                listaB = rpt.Split('¬');
                filtroB = rpt.Split('¬').ToList();
                string[] cabezerasB = listaB[0].Split('|');
                cabeceraTextoB = String.Join(",", cabezerasB);
                string[] anchos = listaB[1].Split('|');
                int ancho;
                tablaB = new System.Data.DataTable();
                for (int j = 0; j < cabezerasB.Length; j++)
                {
                    ancho = int.Parse(anchos[j]);
                    lvwCompra.Columns.Add(cabezerasB[j], cabezerasB[j], ancho);
                    tablaB.Columns.Add(cabezerasB[j], Type.GetType("System.String"));
                    tablaB.Columns[j].Caption = ancho.ToString();
                }
                lvwCompra.View = View.Details;
                lvwCompra.FullRowSelect = true;
                lvwCompra.GridLines = true;
                ListViewItem fila = null;
                System.Windows.Forms.TextBox txt;
                int total = 0;
                int altofila;
                for (int j = 0; j < cabezerasB.Length; j++)
                {
                    if (j == 0) fila = lvwCompra.Items.Add("");
                    else fila.SubItems.Add("");
                    altofila = lvwCompra.Items[0].Bounds.Height;
                    ancho = int.Parse(anchos[j]);
                    txt = new System.Windows.Forms.TextBox();
                    txt.Width = ancho - 5;
                    txt.Height = 10;
                    txt.Font = new System.Drawing.Font("Arial", 9);
                    txt.SetBounds(total, altofila + 7, ancho, txt.Height);
                    txt.TextChanged += new EventHandler(filtrarB);
                    lvwCompra.Controls.Add(txt);
                    total += ancho;
                }
                lvwCompra.Columns[6].TextAlign = HorizontalAlignment.Right;
                lvwCompra.Columns[7].TextAlign = HorizontalAlignment.Right;
                lvwCompra.Columns[8].TextAlign = HorizontalAlignment.Right;
                lvwCompra.Columns[10].TextAlign = HorizontalAlignment.Right;
                lvwCompra.Columns[11].TextAlign = HorizontalAlignment.Right;
                filtrarB(null, null);
            }
        }
        #endregion
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
        private void LDdocumentocs_Load(object sender, EventArgs e)
        {
            fechaIniFin();
            listaVentas();
            listaCompra();
        }
        public void listar()
        {
            lvwCompra.Items.Clear();
            lvwCompra.Columns.Clear();
            lvwproducto.Items.Clear();
            lvwproducto.Columns.Clear();
            lblcantidad.Text = "0";
            lblitemC.Text = "0";
            lblsubtotal.Text = "0.00";
            lbligv.Text = "0.00";
            lbltotal.Text = "0.00";
            lblSubC.Text = "0.00";
            lbligvC.Text = "0.00";
            lblTotalC.Text = "0.00";
            lblsunat.Text = "0.00";
            lblrenta.Text = "0";
            listaVentas();
            listaCompra();
        }
        private void LDdocumento_KeyDown(object sender, KeyEventArgs e)
        {
            if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.N))
            {
                if (this.tabControl1.SelectedIndex == 0)
                {
                    borrarTextos();
                    filtrar(null, null);
                }
                else if (this.tabControl1.SelectedIndex == 1)
                {
                    borrarTextosB();
                    filtrarB(null, null);
                }
            }
            else if (Convert.ToInt32(e.KeyData) == Convert.ToInt32(Keys.Control) + Convert.ToInt32(Keys.P))
            {
                exportarExcel();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                if (this.tabControl1.SelectedIndex == 0)
                    this.tabControl1.SelectedIndex = 1;
                else if (this.tabControl1.SelectedIndex == 1)
                    this.tabControl1.SelectedIndex = 0;
            }
            else if (e.KeyCode == Keys.F5)
            {
                fechaIniFin();
                listar();
            }
        }
        private void filtrarB(object sender, EventArgs e)
        {
            try
            {
                borrarLisViewB();
                filtroB.Clear();
                tablaB.Clear();
                DataRow drw = null;
                Control.ControlCollection textos = lvwCompra.Controls;
                int ntexto = textos.Count;
                List<string> valores = new List<string>();
                for (int j = 0; j < ntexto; j++)
                {
                    valores.Add(((System.Windows.Forms.TextBox)lvwCompra.Controls[j]).Text.ToLower());
                }
                int nRegistros = listaB.Length;
                string[] campos;
                int nCampos;
                bool exito;
                ListViewItem fila = null;
                lvwCompra.BeginUpdate();
                for (var i = 2; i < nRegistros; i++)
                {
                    campos = listaB[i].Split('|');
                    nCampos = campos.Length;
                    exito = true;
                    for (int j = 0; j < nCampos; j++)
                    {
                        exito = (valores[j] == "" || campos[j].ToLower().Contains(valores[j]));
                        if (!exito) break;
                    }
                    if (exito)
                    {
                        drw = tablaB.NewRow();
                        filtroB.Add(listaB[i]);
                        for (int j = 0; j < nCampos; j++)
                        {
                            if (j == 0) fila = lvwCompra.Items.Add(campos[j]);
                            else fila.SubItems.Add(campos[j].Replace("&amp;", "&"));
                            drw[j] = campos[j];
                        }
                        tablaB.Rows.Add(drw);
                    }
                }
                lvwCompra.EndUpdate();
                this.lblitemC.Text = String.Format("Items:  {0}",lvwCompra.Items.Count - 1);
                totalB();
            }
            catch (Exception ex) { ex.ToString(); }
        }
        public void exportarAsyn(string xfile)
        {
            int xcantidad = this.lvwCompra.Items.Count;
            Stopwatch oReloj = new Stopwatch();
            oReloj.Start();
            DataTable dt = new DataTable();
            dt.Columns.Add("Emision", typeof(String));
            dt.Columns.Add("Documento", typeof(String));
            dt.Columns.Add("RUC", typeof(Decimal));
            dt.Columns.Add("RazonSocial", typeof(String));
            dt.Columns.Add("Tipo", typeof(String));
            dt.Columns.Add("BaseImp", typeof(Decimal));
            dt.Columns.Add("IGV", typeof(Decimal));
            dt.Columns.Add("Total", typeof(Decimal));
            dt.Columns.Add("Moneda", typeof(String));
            dt.Columns.Add("TipoSunat", typeof(Decimal));
            dt.Columns.Add("Monto", typeof(Decimal));
            dt.Columns.Add("Referenia", typeof(String));
            foreach (ListViewItem item in lvwCompra.Items)
            {
                if (item.SubItems[7].Text == "")
                {
                    //
                }
                else
                {
                    dt.Rows.Add();
                    dt.Rows[dt.Rows.Count - 1][0] = item.SubItems[1].Text;
                    dt.Rows[dt.Rows.Count - 1][1] = item.SubItems[2].Text;
                    dt.Rows[dt.Rows.Count - 1][2] = item.SubItems[3].Text;
                    dt.Rows[dt.Rows.Count - 1][3] = item.SubItems[4].Text;
                    dt.Rows[dt.Rows.Count - 1][4] = item.SubItems[5].Text;
                    dt.Rows[dt.Rows.Count - 1][5] = item.SubItems[6].Text;
                    dt.Rows[dt.Rows.Count - 1][6] = item.SubItems[7].Text;
                    dt.Rows[dt.Rows.Count - 1][7] = item.SubItems[8].Text;
                    dt.Rows[dt.Rows.Count - 1][8] = item.SubItems[9].Text;
                    dt.Rows[dt.Rows.Count - 1][9] = item.SubItems[10].Text;
                    dt.Rows[dt.Rows.Count - 1][10] = item.SubItems[11].Text;
                    dt.Rows[dt.Rows.Count - 1][11] = item.SubItems[12].Text;
                }
            }
            string folderPath = xfile;
            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(dt, "Compras");
                worksheet.Range("A1:L1").Style
                     .Font.SetFontSize(13)
                     .Font.SetBold(true)
                     .Font.SetFontColor(XLColor.White)
                     .Fill.SetBackgroundColor(XLColor.Orange);
                worksheet.Cell(xcantidad + 2, 1).Value = lblitemC.Text;
                worksheet.Cell(xcantidad + 2, 6).Value = lblSubC.Text;
                worksheet.Cell(xcantidad + 2, 7).Value = lbligvC.Text;
                worksheet.Cell(xcantidad + 2, 8).Value = lblTotalC.Text;
                worksheet.Cell(xcantidad + 2, 6).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(xcantidad + 2, 7).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(xcantidad + 2, 8).Style.NumberFormat.Format = "#,##0.00";
                var cA = worksheet.Column(1);
                cA.Width = 12;
                var cB = worksheet.Column(2);
                cB.Width = 15;
                var col = worksheet.Column(4);
                col.Width = 56;
                var c5 = worksheet.Column(5);
                c5.Width = 8;
                var col1 = worksheet.Column(6);
                col1.Width = 14.3;
                var col2 = worksheet.Column(7);
                col2.Width = 14.3;
                var col3 = worksheet.Column(8);
                col3.Width = 14.3;
                var c9 = worksheet.Column(9);
                c9.Width = 12;
                var c10 = worksheet.Column(10);
                c10.Width = 13;
                var col4 = worksheet.Column(11);
                col4.Width = 14.3;
                var col5 = worksheet.Column(12);
                col5.Width = 14.3;
                for (int i = 2; i <= xcantidad; i++)
                {
                    worksheet.Cell(i, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(i, 5).Style.NumberFormat.Format = "@";
                    worksheet.Cell(i, 6).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(i, 7).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(i, 8).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(i, 11).Style.NumberFormat.Format = "#,##0.00";
                    if (Convert.ToDecimal(worksheet.Cell(i, 6).Value) <= 0)
                        worksheet.Cell(i, 6).Style.Font.SetFontColor(XLColor.Red);
                    if (Convert.ToDecimal(worksheet.Cell(i, 7).Value) <= 0)
                        worksheet.Cell(i, 7).Style.Font.SetFontColor(XLColor.Red);
                    if (Convert.ToDecimal(worksheet.Cell(i, 8).Value) <= 0)
                        worksheet.Cell(i, 8).Style.Font.SetFontColor(XLColor.Red);
                    if (Convert.ToDecimal(worksheet.Cell(i, 11).Value) <= 0)
                        worksheet.Cell(i, 11).Style.Font.SetFontColor(XLColor.Red);
                }
                wb.SaveAs(folderPath);
            }
            MessageBox.Show("Se Exporto Correctamente en:  " + oReloj.Elapsed.TotalMilliseconds.ToString("N2") + " MS", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            xrut = string.Empty;
        }
        public void exportarVentas(string xfile)
        {
            int xcantidad = this.lvwproducto.Items.Count;
            Stopwatch oReloj = new Stopwatch();
            oReloj.Start();
            DataTable dt = new DataTable();
            dt.Columns.Add("Emision", typeof(String));
            dt.Columns.Add("Documento", typeof(String));
            dt.Columns.Add("NroDoc", typeof(String));
            dt.Columns.Add("RazonSocial", typeof(String));
            dt.Columns.Add("RUC", typeof(String));
            dt.Columns.Add("DNI", typeof(String));
            dt.Columns.Add("BaseImp", typeof(Decimal));
            dt.Columns.Add("IGV", typeof(Decimal));
            dt.Columns.Add("ICBPER", typeof(Decimal));
            dt.Columns.Add("Total", typeof(Decimal));
            dt.Columns.Add("Referencia", typeof(String));
            dt.Columns.Add("Estado", typeof(String));
            dt.Columns.Add("CDSunat", typeof(String));
            dt.Columns.Add("Mensaje", typeof(String));

            dt.Columns.Add("Condicion", typeof(String));
            dt.Columns.Add("FormaPago", typeof(String));
            dt.Columns.Add("EntidadBancaria", typeof(String));
            dt.Columns.Add("NroOperacion", typeof(String));
            dt.Columns.Add("Efectivo", typeof(Decimal));
            dt.Columns.Add("Deposito", typeof(Decimal));


            foreach (ListViewItem item in lvwproducto.Items)
            {
                if (item.SubItems[1].Text == "")
                {
                    //
                }
                else
                {
                    dt.Rows.Add();
                    dt.Rows[dt.Rows.Count - 1][0] = item.SubItems[0].Text;
                    dt.Rows[dt.Rows.Count - 1][1] = item.SubItems[1].Text;
                    dt.Rows[dt.Rows.Count - 1][2] = item.SubItems[2].Text;
                    dt.Rows[dt.Rows.Count - 1][3] = item.SubItems[3].Text;
                    dt.Rows[dt.Rows.Count - 1][4] = item.SubItems[4].Text;
                    dt.Rows[dt.Rows.Count - 1][5] = item.SubItems[5].Text;
                    dt.Rows[dt.Rows.Count - 1][6] = item.SubItems[6].Text;
                    dt.Rows[dt.Rows.Count - 1][7] = item.SubItems[7].Text;
                    dt.Rows[dt.Rows.Count - 1][8] = item.SubItems[8].Text;
                    dt.Rows[dt.Rows.Count - 1][9] = item.SubItems[9].Text;
                    dt.Rows[dt.Rows.Count - 1][10] = item.SubItems[12].Text;
                    dt.Rows[dt.Rows.Count - 1][11] = item.SubItems[11].Text;
                    dt.Rows[dt.Rows.Count - 1][12] = item.SubItems[13].Text;
                    dt.Rows[dt.Rows.Count - 1][13] = item.SubItems[14].Text;

                    dt.Rows[dt.Rows.Count - 1][14] = item.SubItems[15].Text;
                    dt.Rows[dt.Rows.Count - 1][15] = item.SubItems[16].Text;
                    dt.Rows[dt.Rows.Count - 1][16] = item.SubItems[17].Text;
                    dt.Rows[dt.Rows.Count - 1][17] = item.SubItems[18].Text;
                    dt.Rows[dt.Rows.Count - 1][18] = item.SubItems[19].Text;
                    dt.Rows[dt.Rows.Count - 1][19] = item.SubItems[20].Text;
                }
            }
            string folderPath = xfile;
            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(dt, "Ventas");
                worksheet.Range("A1:T1").Style
                     .Font.SetFontSize(13)
                     .Font.SetBold(true)
                     .Font.SetFontColor(XLColor.White)
                     .Fill.SetBackgroundColor(XLColor.FromHtml("#3377FF"));
                worksheet.Cell(xcantidad + 2, 1).Value = lblcantidad.Text;
                worksheet.Cell(xcantidad + 2, 7).Value = lblsubtotal.Text;
                worksheet.Cell(xcantidad + 2, 8).Value = lbligv.Text;
                worksheet.Cell(xcantidad + 2, 9).Value = lblICBPER.Text;
                worksheet.Cell(xcantidad + 2, 10).Value = lbltotal.Text;
                worksheet.Cell(xcantidad + 2, 7).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(xcantidad + 2, 8).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(xcantidad + 2, 9).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(xcantidad + 2, 10).Style.NumberFormat.Format = "#,##0.00";
                var c2 = worksheet.Column(2);
                c2.Width = 16;
                var col = worksheet.Column(4);
                col.Width = 56;
                var col5 = worksheet.Column(5);
                col5.Width = 13;
                var col6 = worksheet.Column(6);
                col6.Width = 11.6;
                var col1 = worksheet.Column(7);
                col1.Width = 14.3;
                var col2 = worksheet.Column(8);
                col2.Width = 14.3;
                var col3 = worksheet.Column(9);
                col3.Width = 13.5;
                var col4 = worksheet.Column(10);
                col4.Width = 14.3;
                var col11 = worksheet.Column(11);
                col11.Width = 14.3;
                var col13 = worksheet.Column(13);
                col13.Width = 12.5;
                var col14 = worksheet.Column(14);
                col14.Width =62;

                var col15 = worksheet.Column(15);
                col15.Width = 14;

                var col16= worksheet.Column(16);
                col16.Width = 18;
                var col17 = worksheet.Column(17);
                col17.Width =22.5;
                var col18 = worksheet.Column(18);
                col18.Width =23;
                var col19 = worksheet.Column(19);
                col19.Width =14;
                var col20 = worksheet.Column(20);
                col20.Width =14;

                for (int i = 2; i <= xcantidad; i++)
                {
                    worksheet.Cell(i,5).Style.NumberFormat.Format = "@";
                    worksheet.Cell(i,5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(i,7).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(i,8).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(i,9).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(i,10).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(i,19).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(i,20).Style.NumberFormat.Format = "#,##0.00";
                    if (Convert.ToDecimal(worksheet.Cell(i, 7).Value) <= 0)
                    {
                        worksheet.Cell(i, 7).Style.Font.SetFontColor(XLColor.Red);
                        worksheet.Cell(i, 8).Style.Font.SetFontColor(XLColor.Red);
                        worksheet.Cell(i, 9).Style.Font.SetFontColor(XLColor.Red);
                        worksheet.Cell(i, 10).Style.Font.SetFontColor(XLColor.Red);
                        worksheet.Cell(i, 12).Style.Font.SetFontColor(XLColor.Red);
                        worksheet.Cell(i, 19).Style.Font.SetFontColor(XLColor.Red);
                        worksheet.Cell(i, 20).Style.Font.SetFontColor(XLColor.Red);
                    }
                    worksheet.Cell(i, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                }
                wb.SaveAs(folderPath);
            }
            MessageBox.Show("Se Exporto Correctamente en:  " + oReloj.Elapsed.TotalMilliseconds.ToString("N2") + " MS", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            xrut = string.Empty;
        }
        public void exportarExcel()
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx", ValidateNames = true })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    xrut = sfd.FileName.ToString();
                    if (this.tabControl1.SelectedIndex == 1)exportarAsyn(xrut);
                    else exportarVentas(xrut);
                }
            }
        }
        private void btnexportar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            exportarExcel();
        }
        private void btnbuscar_Click(object sender, EventArgs e)
        {
            listar();
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
                listar();
            }
            else
            {
                e.Handled = false;
            }
        }
        private void dtimefin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey) dtimeinicio.Focus();
        }
    }
}
