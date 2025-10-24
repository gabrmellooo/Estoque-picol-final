using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Estoque_picole
{
    public partial class Relatorio : Page
    {
        private List<HistoricoAlteracao> historicoAlteracoes = new List<HistoricoAlteracao>();

        public Relatorio()
        {
            InitializeComponent();
            CarregarRelatorio();
            Valortotal();
            CarregarHistorico();
        }

        private void CarregarRelatorio()
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar estoque: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Valortotal()
        {
            try
            {
                if (dgEstoque.ItemsSource == null) return;

                DataView dataView = dgEstoque.ItemsSource as DataView;
                if (dataView == null) return;

                if (!dataView.Table.Columns.Contains("ValorTotal"))
                    dataView.Table.Columns.Add("ValorTotal", typeof(decimal));

                decimal somaTotal = 0;

                foreach (DataRow linha in dataView.Table.Rows)
                {
                    if (linha["quantidade"] != DBNull.Value && linha["preco"] != DBNull.Value)
                    {
                        int quantidade = Convert.ToInt32(linha["quantidade"]);
                        decimal preco = Convert.ToDecimal(linha["preco"]);
                        decimal total = quantidade * preco;

                        linha["ValorTotal"] = total;
                        somaTotal += total;
                    }
                    else
                    {
                        linha["ValorTotal"] = 0;
                    }
                }

                dgEstoque.ItemsSource = dataView;
                txtValorTotal.Text = somaTotal.ToString("C2");
                txtTotalItens.Text = dataView.Table.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao calcular valor total: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Home());
        }

        private void RegistrarAlteracao(string produto, int antiga, int nova)
        {
            try
            {
                string sql = "INSERT INTO historico (Produto, QuantidadeAntiga, QuantidadeNova, DataAlteracao) " +
                             "VALUES (@Produto, @QuantidadeAntiga, @QuantidadeNova, @DataAlteracao)";

                using (MySqlCommand comando = new MySqlCommand(sql, ConexaoDp.Conexao))
                {
                    if (ConexaoDp.Conexao.State != ConnectionState.Open)
                        ConexaoDp.Conexao.Open();

                    comando.Parameters.AddWithValue("@Produto", produto);
                    comando.Parameters.AddWithValue("@QuantidadeAntiga", antiga);
                    comando.Parameters.AddWithValue("@QuantidadeNova", nova);
                    comando.Parameters.AddWithValue("@DataAlteracao", DateTime.Now);

                    comando.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao registrar histórico no banco de dados: {ex.Message}",
                                "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (ConexaoDp.Conexao.State == ConnectionState.Open)
                    ConexaoDp.Conexao.Close();
            }
        }

        private void dgEstoque_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                DataRowView linha = e.Row.Item as DataRowView;
                if (linha == null) return;

                string produto = linha["Nome"].ToString();

                if (e.Column.Header.ToString() == "Quantidade")
                {
                    if (int.TryParse(((TextBox)e.EditingElement).Text, out int novaQuantidade))
                    {
                        int antigaQuantidade = Convert.ToInt32(linha["quantidade"]);

                        RegistrarAlteracao(produto, antigaQuantidade, novaQuantidade);

                        linha["quantidade"] = novaQuantidade;

                        Dispatcher.InvokeAsync(() => Valortotal());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao registrar alteração: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CarregarHistorico()
        {
            try
            {
                string sql = "SELECT * FROM historico ORDER BY DataAlteracao DESC LIMIT 10";
                using (MySqlCommand comando = new MySqlCommand(sql, ConexaoDp.Conexao))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(comando))
                {
                    DataTable tabela = new DataTable();
                    adapter.Fill(tabela);
                    dgHistorico.ItemsSource = tabela.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar histórico: {ex.Message}",
                                "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public class HistoricoAlteracao
        {
            public string Produto { get; set; }
            public int QuantidadeAntiga { get; set; }
            public int QuantidadeNova { get; set; }
            public DateTime DataAlteracao { get; set; }
        }
    }
}
