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
using System.Windows.Shapes;
using System.IO;

namespace Traitement_Images
{
    /// <summary>
    /// Logique d'interaction pour Choisir_Image.xaml
    /// </summary>
    public partial class Choisir_Image : Window
    {
        string nomDossier;
        MonImage image;

        public Choisir_Image(string nomDossier)
        {
            this.nomDossier = nomDossier;
            InitializeComponent();
            txt.Text += nomDossier;
            WrapPanel wp = new WrapPanel();
            ScrollViewer scroll = new ScrollViewer();
            Thickness margin = new Thickness();
            margin.Top = 40;
            margin.Left = 40;
            scroll.Margin = margin;

            string[] images = Directory.GetFiles(nomDossier);

            for (int i = 0; i < images.Length; i++)
            {
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Vertical;
                Button b = new Button();
                b.Click += B_Click;
                b.Content = RecupNom(images[i]);
                Uri uri = new Uri(images[i], UriKind.RelativeOrAbsolute);
                BmpBitmapDecoder decoder = new BmpBitmapDecoder(uri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bmpSource = decoder.Frames[0];
                Image img = new Image();
                img.Source = bmpSource;
                img.Height = 100;
                sp.Children.Add(img);
                sp.Children.Add(b);
                sp.Margin = new Thickness(20);
                wp.Children.Add(sp);
            }
            scroll.Content = wp;
            grille.Children.Add(scroll);
        }

        /// <summary>
        /// Récupère le nom d'un fichier, en enlevant le nom du dossier qui le contient ainsi que son format, pour l'afficher
        /// en beau sur la console par la suite
        /// </summary>
        /// <param name="fichier">Chemin d'accès du fichier</param>
        /// <returns>Nom du fichier</returns>
        static string RecupNom(string fichier)
        {
            int i = 0;
            string nom = "";
            while (fichier[i] != '\\') i++;
            i++;
            while (fichier[i] != '.') nom += fichier[i++];
            return nom;
        }

        private void B_Click(object sender, RoutedEventArgs e)
        {
            string nom = "";
            for (int i = sender.ToString().Length - 1; sender.ToString()[i] != ' '; i--)
            {
                nom = sender.ToString()[i] + nom;
            }
            nom = nomDossier + "/" + nom + ".bmp";
            image = new MonImage(nom);
            Traiter traiter = new Traiter(image);
            this.Visibility = Visibility.Hidden;
            traiter.Show();
        }
    }
}
