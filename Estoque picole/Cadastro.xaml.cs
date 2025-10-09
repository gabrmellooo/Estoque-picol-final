using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Interação lógica para Cadastro.xam
    /// </summary>
    public partial class Cadastro : Page
    {
        public Cadastro()
        {
            InitializeComponent();
        }

        private string Conexao = "server=localhost;user=root;password=root;database=picole";

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            string nome = txtNome.Text;
            string sabor = txtSabor.Text;
            int quantidade;
            decimal preco;
            DateTime validade;

            // validação simples
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


            // comando SQL
            string query = "INSERT INTO Produtos (Nome, Sabor, Quantidade, Preco, Validade) " +
                           "VALUES (@Nome, @Sabor, @Quantidade, @Preco, @Validade)";

            try
            {
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
        }

        private void LimparCampos()
        {
            txtNome.Clear();
            txtSabor.Clear();
            txtQuantidade.Clear();
            txtPreco.Clear();
            dpValidade.SelectedDate = null;
        }



        private void btnVoltar_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Home());
        }
    }
}
