using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Estoque_picole
{
    public partial class Estoque : Page
    {
        public Estoque()
        {
            InitializeComponent();
            CarregarEstoque();
        }

        private void CarregarEstoque()
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

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Home());
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
                using (MySqlCommand cmd = new MySqlCommand(query, ConexaoDp.Conexao))
                {
                    cmd.Parameters.AddWithValue("@id", idProduto);
                    cmd.ExecuteNonQuery();
                }

                CarregarEstoque();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir produto: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgEstoque_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit) return;

            var linha = e.Row.Item as DataRowView;

            if (linha == null) return;

            string? coluna = e.Column.Header?.ToString();
            if (string.IsNullOrEmpty(coluna)) return;

            var elemento = e.EditingElement as TextBox;
            if (elemento == null) return;

            string novoValor = elemento.Text;
            int idProduto = Convert.ToInt32(linha["id"]);

            string query = "";

            switch (coluna)
            {
                case "Nome":
                    query = "UPDATE produtos SET nome = @valor WHERE id = @id";
                    break;
                case "Sabor":
                    query = "UPDATE produtos SET sabor = @valor WHERE id = @id";
                    break;
                case "Quantidade":
                    query = "UPDATE produtos SET quantidade = @valor WHERE id = @id";
                    break;
                case "Preço":
                    query = "UPDATE produtos SET preco = @valor WHERE id = @id";
                    break;
                case "Validade":
                    query = "UPDATE produtos SET validade = @valor WHERE id = @id";
                    break;
                default:
                    return;
            }

            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, ConexaoDp.Conexao))
                {
                    cmd.Parameters.AddWithValue("@id", idProduto);

                    // Conversão de tipos
                    if (coluna == "Quantidade")
                        cmd.Parameters.AddWithValue("@valor", Convert.ToInt32(novoValor));
                    else if (coluna == "Preço")
                        cmd.Parameters.AddWithValue("@valor", Convert.ToDecimal(novoValor));
                    else if (coluna == "Validade")
                        cmd.Parameters.AddWithValue("@valor", Convert.ToDateTime(novoValor));
                    else
                        cmd.Parameters.AddWithValue("@valor", novoValor);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar produto: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
