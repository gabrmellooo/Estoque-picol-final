using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
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
    }
}