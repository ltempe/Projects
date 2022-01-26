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

namespace Traitement_Images
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// Rien de fonctionnel pour l'instant !!
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void traitement_Click(object sender, RoutedEventArgs e)
        {
            Choisir_Image selection = new Choisir_Image("images");
            this.Visibility = Visibility.Hidden;
            selection.Show();
        }
    }
}
