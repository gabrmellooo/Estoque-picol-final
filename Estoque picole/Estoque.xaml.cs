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
    /// Interação lógica para Estoque.xam
    /// </summary>
    public partial class Estoque : Page
    {
        private string Conexao = "server=localhost;user=root;password=root;database=picole";

        public Estoque()
        {
            InitializeComponent();
            string sql = "SELECT * FROM produtos";
            MySqlCommand comando = new MySqlCommand(sql, ConexaoDp.Conexao);

            MySqlDataReader leitor = comando.ExecuteReader();
            dgEstoque.ItemsSource = leitor;
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Home());
        }
    }
}
