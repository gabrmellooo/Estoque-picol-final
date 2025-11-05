using System.Text;
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
    public partial class MainWindow : Window
    {


        public MainWindow()
        {

            InitializeComponent();
            ConexaoDp.AbrirConexao("server=sql10.freesqldatabase.com;user=sql10806094;password=knue8iZQp6;database=sql10806094;port=3306");

        }
    }
}