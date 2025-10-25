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
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void btnCadastro_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Cadastro());
        }

        private void btnSair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnRelatorios_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Relatorio());
        }

        private void btnEstoque_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EstoqueVisualizacao());
        }
    }
}
