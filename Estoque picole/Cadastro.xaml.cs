using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Estoque_picole
{
    public partial class Cadastro : Page
    {
        public Cadastro()
        {
            InitializeComponent();
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            string nome = txtNome.Text;
            string sabor = txtSabor.Text;
            int quantidade;
            decimal preco;
            DateTime validade;

            if (!int.TryParse(txtQuantidade.Text, out quantidade))
            {
                MessageBox.Show("Quantidade inválida!");
                return;
            }

            if (!decimal.TryParse(txtPreco.Text, out preco))
            {
                MessageBox.Show("Preço inválido!");
                return;
            }

            if (!dpValidade.SelectedDate.HasValue)
            {
                MessageBox.Show("Selecione uma data de validade!");
                return;
            }

            validade = dpValidade.SelectedDate.Value;

            string query = "INSERT INTO Produtos (Nome, Sabor, Quantidade, Preco, Validade) " +
                           "VALUES (@Nome, @Sabor, @Quantidade, @Preco, @Validade)";

            try
            {
                if (ConexaoDp.Conexao.State != ConnectionState.Open)
                    ConexaoDp.Conexao.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, ConexaoDp.Conexao))
                {
                    cmd.Parameters.AddWithValue("@Nome", nome);
                    cmd.Parameters.AddWithValue("@Sabor", sabor);
                    cmd.Parameters.AddWithValue("@Quantidade", quantidade);
                    cmd.Parameters.AddWithValue("@Preco", preco);
                    cmd.Parameters.AddWithValue("@Validade", validade);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Cadastro salvo com sucesso!");
                LimparCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar: " + ex.Message);
            }
            finally
            {
                if (ConexaoDp.Conexao.State == ConnectionState.Open)
                    ConexaoDp.Conexao.Close();
            }
        }

        private void LimparCampos()
        {
            txtNome.Clear();
            txtSabor.Clear();
            txtQuantidade.Clear();
            txtPreco.Clear();
            dpValidade.SelectedDate = null;
        }

        private void dgEstoque_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit) return;

            var linha = e.Row.Item as DataRowView;
            if (linha == null) return;

            string coluna = e.Column.Header?.ToString();
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
                if (ConexaoDp.Conexao.State != ConnectionState.Open)
                    ConexaoDp.Conexao.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, ConexaoDp.Conexao))
                {
                    cmd.Parameters.AddWithValue("@id", idProduto);

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
            finally
            {
                if (ConexaoDp.Conexao.State == ConnectionState.Open)
                    ConexaoDp.Conexao.Close();
            }
        }

        private void btnVoltar_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Home());
        }
    }
}
