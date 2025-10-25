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
                int totalItens = 0;

                foreach (DataRow linha in dataView.Table.Rows)
                {
                    int quantidade = 0;
                    decimal preco = 0;

                    if (linha["Quantidade"] != DBNull.Value)
                        quantidade = Convert.ToInt32(linha["Quantidade"]);

                    if (linha["Preco"] != DBNull.Value)
                        preco = Convert.ToDecimal(linha["Preco"]);

                    linha["ValorTotal"] = quantidade * preco;

                    somaTotal += quantidade * preco;
                    totalItens += quantidade;
                }

                dgEstoque.ItemsSource = dataView;
                txtValorTotal.Text = somaTotal.ToString("C2");
                txtTotalItens.Text = totalItens.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao calcular valor total: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Erro ao carregar histórico: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Home());
        }

        private void RegistrarAlteracao(string produto, int antiga, int nova, int idProduto)
        {
            try
            {
                string sqlUpdate = "UPDATE produtos SET quantidade = @Quantidade WHERE id = @Id";
                using (MySqlCommand cmdUpdate = new MySqlCommand(sqlUpdate, ConexaoDp.Conexao))
                {
                    if (ConexaoDp.Conexao.State != ConnectionState.Open)
                        ConexaoDp.Conexao.Open();

                    cmdUpdate.Parameters.AddWithValue("@Quantidade", nova);
                    cmdUpdate.Parameters.AddWithValue("@Id", idProduto);

                    cmdUpdate.ExecuteNonQuery();
                }

                string sqlInsert = "INSERT INTO historico (Produto, QuantidadeAntiga, QuantidadeNova, DataAlteracao) " +
                                   "VALUES (@Produto, @QuantidadeAntiga, @QuantidadeNova, @DataAlteracao)";
                using (MySqlCommand cmdInsert = new MySqlCommand(sqlInsert, ConexaoDp.Conexao))
                {
                    cmdInsert.Parameters.AddWithValue("@Produto", produto);
                    cmdInsert.Parameters.AddWithValue("@QuantidadeAntiga", antiga);
                    cmdInsert.Parameters.AddWithValue("@QuantidadeNova", nova);
                    cmdInsert.Parameters.AddWithValue("@DataAlteracao", DateTime.Now);

                    cmdInsert.ExecuteNonQuery();
                }

                CarregarHistorico();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar produto: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                int idProduto = Convert.ToInt32(linha["id"]); // assume que existe coluna 'id'

                if (e.Column.Header.ToString() == "Quantidade")
                {
                    if (int.TryParse(((TextBox)e.EditingElement).Text, out int novaQuantidade))
                    {
                        int antigaQuantidade = Convert.ToInt32(linha["quantidade"]);

                        RegistrarAlteracao(produto, antigaQuantidade, novaQuantidade, idProduto);

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

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (dgEstoque.SelectedItem == null)
            {
                MessageBox.Show("Selecione um item para excluir.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var linha = dgEstoque.SelectedItem as DataRowView;
            if (linha == null || linha["id"] == DBNull.Value)
            {
                MessageBox.Show("Não foi possível obter o ID do produto selecionado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int idProduto = Convert.ToInt32(linha["id"]);

            var confirm = MessageBox.Show(
                $"Tem certeza que deseja excluir o produto com ID {idProduto}?",
                "Confirmar exclusão",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                string query = "DELETE FROM produtos WHERE id = @id";
                if (ConexaoDp.Conexao.State != ConnectionState.Open)
                    ConexaoDp.Conexao.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, ConexaoDp.Conexao))
                {
                    cmd.Parameters.AddWithValue("@id", idProduto);
                    cmd.ExecuteNonQuery();
                }

                CarregarRelatorio();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir produto: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (ConexaoDp.Conexao.State == ConnectionState.Open)
                    ConexaoDp.Conexao.Close();
            }
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
