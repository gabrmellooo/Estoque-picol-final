using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Elements.Table;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Estoque_picole
{
    public partial class EstoqueVisualizacao : Page
    {
        public EstoqueVisualizacao()
        {
            InitializeComponent();
            CarregarEstoque();
            CalcularValorTotal();
        }

        private void CarregarEstoque()
        {
            string sql = "SELECT * FROM produtos";
            using (MySqlCommand comando = new MySqlCommand(sql, ConexaoDp.Conexao))
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(comando))
            {
                DataTable tabela = new DataTable();
                adapter.Fill(tabela);
                dgEstoque.ItemsSource = tabela.DefaultView;
            }
        }

        private void CalcularValorTotal()
        {
            if (dgEstoque.ItemsSource == null) return;
            DataView dataView = dgEstoque.ItemsSource as DataView;
            if (dataView == null) return;
            if (!dataView.Table.Columns.Contains("ValorTotal"))
                dataView.Table.Columns.Add("ValorTotal", typeof(decimal));

            foreach (DataRow linha in dataView.Table.Rows)
            {
                if (linha["quantidade"] != DBNull.Value && linha["preco"] != DBNull.Value)
                {
                    int quantidade = Convert.ToInt32(linha["quantidade"]);
                    decimal preco = Convert.ToDecimal(linha["preco"]);
                    linha["ValorTotal"] = quantidade * preco;
                }
                else
                {
                    linha["ValorTotal"] = 0;
                }
            }

            dgEstoque.ItemsSource = dataView;
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Home());
        }

        private void btnsalvar_Click(object sender, RoutedEventArgs e)
        {
            if (dgEstoque.ItemsSource == null)
            {
                MessageBox.Show("Nenhum dado para salvar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string caminho = "C:\\Users\\GABRIELOTAVIOMARTINS\\Documents\\PastaExemplo";

            if (!Directory.Exists(caminho))
                Directory.CreateDirectory(caminho);

            string arquivo = System.IO.Path.Combine(caminho, "Estoque.pdf");

            var linhas = new List<List<string>>();

            var cabecalho = dgEstoque.Columns.Select(c => c.Header.ToString()).ToList();
            linhas.Add(cabecalho);

            foreach (var item in dgEstoque.Items)
            {
                if (item == null) continue;

                var linha = new List<string>();

                foreach (var col in dgEstoque.Columns)
                {
                    var cell = col.GetCellContent(item);
                    string valor = "";

                    if (cell is TextBlock tb)
                        valor = tb.Text;

                    linha.Add(valor);
                }

                linhas.Add(linha);
            }

            Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Margin(20);

                    page.Content().Column(col =>
                    {
                        foreach (var linha in linhas)
                        {
                            col.Item().Text(string.Join(" | ", linha));
                        }
                    });
                });
            })
            .GeneratePdf(arquivo);

            MessageBox.Show("PDF gerado em:\n" + arquivo);
        }
    }
}