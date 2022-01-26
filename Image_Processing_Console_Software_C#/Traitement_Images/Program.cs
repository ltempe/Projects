using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Traitement_Images
{
    public class Program
    {
        static MonImage image;

        /// <summary>
        /// Menu principal du traitement des images
        /// </summary>
        /// <returns>Choix de l'application</returns>
        static int Menu1()
        {
            Console.WriteLine("Vous souhaitez : ");
            Console.WriteLine("1 - Traiter une image");
            Console.WriteLine("2 - Décoder une image");
            Console.WriteLine("3 - Créer des fractales de Mandelbrot ou de Julia");
            Console.WriteLine("4 - Afficher les images que vous avez traité");
            Console.WriteLine("5 - Créer un QR Code");
            Console.WriteLine("6 - Décoder un QR Code");

            int choix = 0;
            while (choix < 1 || choix > 6)
            {
                Console.Write("Veuillez choisir un numéro : ");
                int.TryParse(Console.ReadLine(), out choix);
            }
            Console.Clear();
            return choix;
        }

        /// <summary>
        /// Menu des applications de traitement d'image
        /// </summary>
        /// <returns>Numéro d'application choisie par l'utilisateur</returns>
        static int Menu2()
        {
            Console.WriteLine("Vous souhaitez : ");
            Console.WriteLine("1 - Modifier une image");
            Console.WriteLine("2 - Utiliser des matrices de Convolution");
            Console.WriteLine("3 - Cacher une image dans une autre");
            Console.WriteLine("4 - Réaliser l'histogramme d'une image");

            int choix = 0;
            while (choix < 1 || choix > 4)
            {
                Console.Write("Veuillez choisir un numéro : ");
                int.TryParse(Console.ReadLine(), out choix);
            }
            Console.Clear();
            return choix;
        }

        /// <summary>
        /// Traitement des images
        /// </summary>
        static void TraiterImages()
        {
            if (InitialiserImage("images"))
            {
                bool continuer = true;
                while (continuer)
                {
                    int choix2 = Menu2();
                    switch (choix2)
                    {
                        case 1:
                            ModifierImages();
                            break;

                        case 2:
                            MatricesConvolution();
                            break;

                        case 3:
                            CacherImage();
                            break;

                        case 4:
                            FaireHistogramme();
                            break;
                    }

                    Console.WriteLine("Souhaitez-vous ajouter des modifications à cette image ?");
                    Console.WriteLine("Veuillez répondre par OUI ou par NON. (Appuyer sur ENTREE sera considéré comme NON)");
                    if (Console.ReadLine().ToUpper() != "OUI") continuer = false;
                }
            }
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

        /// <summary>
        /// Affiche l'ensemble des noms des fichiers d'un dossier sur la Console
        /// </summary>
        /// <param name="nomDossier">Nom du dossier, chemin d'accès au dossier</param>
        static void AfficherDossier(string nomDossier)
        {
            Console.WriteLine("Dossier " + nomDossier + " : ");
            string[] images = Directory.GetFiles(nomDossier);
            if (images.Length == 0) Console.WriteLine("(Aucune image pour le moment)");
            for (int i = 0; i < images.Length; i++)
            {
                for (int j = 0; j < 3 && i < images.Length; j++)
                {
                    Console.Write(RecupNom(images[i]));
                    if (j != 2 && i++ != images.Length - 1) Console.Write(" - ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Méthode permettant à l'utilisateur de choisir une image parmi la bibliothèque d'images disponible en entrant le 
        /// nom de l'image, et éventuellement de l'afficher, pour pouvoir la traiter par la suite
        /// </summary>
        /// <returns>Instance de la classe MonImage contenant toutes les informations de l'image choisie</returns>
        static bool InitialiserImage(string nomDossier)
        {
            bool effectue = false;
            bool refaire = true;
            string fichier = "";

            //Console.WriteLine("Images 320x200 : chaton - coco - poisson");
            //Console.WriteLine("Images 512x512 : hibiscus - lena - vangogh");
            //Console.WriteLine("Images 800x600 : automne - cascade - lac");
            //Console.WriteLine("Images 600x800 : joconde - mario - parapluies");
            //Console.WriteLine("Images 1920x1080 : montagne - plage - zelda");

            AfficherDossier(nomDossier);
            Console.Write("Veuillez indiquer le nom de l'image choisie : ");
            string nom = "";
            int essai = 3;
            while (refaire)
            {
                try
                {
                    nom = Console.ReadLine().ToLower();
                    fichier = nomDossier + "\\" + nom + ".bmp";
                    File.OpenRead(fichier); //Teste l'existence du fichier
                    refaire = false;
                }
                catch
                {
                    Console.WriteLine("Le nom que vous avez entré ne correspond pas. Essais restants : " + essai);
                    Console.Write("Veuillez recommencer : ");

                    refaire = essai-- > 0;
                }
            }
            if (essai > 0)
            {
                Console.WriteLine("Pour afficher l'image, appuyez sur ENTREE. Sinon, appuyez sur n'importe quelle touche pour continuer.");
                ConsoleKeyInfo suite = Console.ReadKey();
                if (suite.Key == ConsoleKey.Enter)
                {
                    Process.Start(fichier);
                }
                Console.WriteLine();
                image = new MonImage(fichier);
                image.Nom = nom;
                effectue = true;
            }
            return effectue;
        }

        /// <summary>
        /// Permet à l'utilisateur d'enregistrer son image avec le nom qu'il souhaite, et éventuellement de l'afficher
        /// </summary>
        /// <param name="image">Instance de la classe MonImage à enregistrer et à afficher</param>
        static void SauvegardeImage(string nom, string nomDossier)
        {
            //Console.Write("Veuillez lui donner un nom : ");
            //bool refaire = true;
            //string modif = "";
            //while (refaire)
            //{
            //    try
            //    {
            //        modif = "sortie\\" + image.Nom + "_" + Console.ReadLine().ToLower() + ".bmp";
            //        image.EnregistrerImage(modif);
            //        refaire = false;
            //    }
            //    catch
            //    {
            //        Console.WriteLine("Le nom que vous avez attribué n'est pas conforme ou contient des caractères spéciaux.");
            //        Console.WriteLine("L'image n'a pas pu être enregistrée.");
            //        Console.Write("Veuillez rentrer un nom pour votre image : ");
            //    }
            //}
            image.Nom += "_" + nom;
            string modif = (nomDossier + "\\" + image.Nom + ".bmp");
            image.EnregistrerImage(modif);
            Console.WriteLine(image.ToString() + "\n");
            Console.WriteLine("Image enregistrée dans " + modif);
            Console.WriteLine("Pour afficher l'image, appuyez sur ENTREE. Sinon, appuyez sur n'importe quelle touche pour continuer.");
            ConsoleKeyInfo suite = Console.ReadKey();
            if (suite.Key == ConsoleKey.Enter)
            {
                Process.Start(modif);
            }
        }

        /// <summary>
        /// Méthode proposant plusieurs manipulations basiques à appliquer aux instances de la classe MonImage
        /// </summary>
        static void ModifierImages()
        {
            //MonImage image = InitialiserImage();
            
            Console.Clear();
            Console.WriteLine("\t Modifications de l'image.");
            Console.WriteLine("1 - Rotation de l'image");
            Console.WriteLine("2 - Symétrie selon l'axe vertical");
            Console.WriteLine("3 - Symétrie selon l'axe horizontal");
            Console.WriteLine("4 - Passage en nuances de gris");
            Console.WriteLine("5 - Passage en noir et blanc");
            Console.WriteLine("6 - Inversion des couleurs");
            Console.WriteLine("7 - Changer la taille de l'image");

            int choix = 0;
            while (choix < 1 || choix > 7)
            {
                Console.Write("Veuillez choisir une modification en indiquant un numéro entre 1 et 7 : ");
                int.TryParse(Console.ReadLine(), out choix);
            }
            Console.Clear();
            string nom = "";

            switch (choix)
            {
                case 1:
                    Console.WriteLine("1 - Rotation de l'image");
                    Console.Write("Veuillez entrer un angle de rotation, en degrés : ");
                    int angle;
                    int.TryParse(Console.ReadLine(), out angle);
                    image.Rotation(angle);
                    nom = "rotation_" + angle;
                    break;

                case 2:
                    Console.WriteLine("2 - Symétrie selon l'axe vertical");
                    image.SymetrieVerticale();
                    nom = "symetrie_verticale";
                    break;

                case 3:
                    Console.WriteLine("3 - Symétrie selon l'axe horizontal");
                    image.SymetrieHorizontale();
                    nom = "symetrie_horizontale";
                    break;

                case 4:
                    Console.WriteLine("4 - Passage en nuances de gris");
                    image.NuancesGris();
                    nom = "gris";
                    break;

                case 5:
                    Console.WriteLine("5 - Passage en noir et blanc");
                    image.NoirBlanc();
                    nom = "noir_blanc";
                    break;

                case 6:
                    Console.WriteLine("6 - Inversion des couleurs");
                    image.Negatif();
                    nom = "negatif";
                    break;

                case 7:
                    Console.WriteLine("7 - Changer la taille de l'image");
                    int taille = 0;
                    while (taille < 1)
                    {
                        Console.WriteLine("De combien souhaitez-vous agrandir ou rétrécir la taille de l'image ?");
                        Console.Write("Veuillez entrer un pourcentage (100 = taille normale) : ");
                        int.TryParse(Console.ReadLine(), out taille);
                    }
                    image.ChangerTaille(taille);
                    nom = "taille_" + taille + "%";
                    break;
            }

            Console.WriteLine("Image modifiée.");
            SauvegardeImage(nom, "sortie");

        }

        /// <summary>
        /// Permet de diviser l'ensemble des éléments d'une matrice par un même nombre entré en paramètre, utile pour créer
        /// certains noyaux de convolution
        /// </summary>
        /// <param name="noyau">Matrice dont on souhaite diviser chaque élément</param>
        /// <param name="diviseur">Nombre par lequel on souhaite diviser les éléments</param>
        static void DiviserMatrice(float[,] noyau, int diviseur)
        {
            for (int i = 0; i < noyau.GetLength(0); i++)
            {
                for (int j = 0; j < noyau.GetLength(1); j++)
                {
                    noyau[i, j] /= (float)diviseur;
                }
            }
        }

        /// <summary>
        /// Méthode permettant à l'utilisateur de créer son propre noyau de convolution case par case
        /// </summary>
        /// <returns>Noyau de convolution</returns>
        static float[,] CreerNoyauConvolution()
        {
            int taille = 2;
            while(taille < 3 || taille%2 == 0)
            {
                Console.Write("Veuillez entrer une taille impaire pour votre matrice carrée : ");
                int.TryParse(Console.ReadLine(), out taille);
            }
            float[,] noyau = new float[taille, taille];
            Console.WriteLine("Veuillez pour chaque case un nombre entier pour créer votre noyau de convolution COLONNE PAR COLONNE");
            Console.WriteLine("Toute valeur non conforme (caractère, espace vide, ...) sera remplacée par 0");
            Console.WriteLine("Vous pourrez diviser la matrice par la suite");
            for (int i = 0; i < taille; i++)
            {
                Console.WriteLine("Colonne " + (i + 1));
                for (int j = 0; j < taille; j++)
                {
                    int nombre = 0;
                    int.TryParse(Console.ReadLine(), out nombre);
                    noyau[j, i] = nombre;
                }
                Console.WriteLine();
            }
            return noyau;
        }

        /// <summary>
        /// Permet d'afficher une matrice de réels sur la console avec un affichage simple
        /// </summary>
        /// <param name="matrice"></param>
        public static void AfficherMatrice(float[,] matrice)
        {
            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                Console.Write("|");
                for (int j = 0; j < matrice.GetLength(1); j++)
                {
                    if (matrice[i, j] >= 0)
                    {
                        Console.Write(" ");
                        if (matrice[i, j] < 100) Console.Write(" ");
                        Console.Write(matrice[i, j]);
                        if (matrice[i, j] < 10) Console.Write(" ");
                        Console.Write("|");
                    }
                    else
                    {
                        if (matrice[i, j] > -100) Console.Write(" ");
                        Console.Write(matrice[i, j]);
                        if (matrice[i, j] > -10) Console.Write(" ");
                        Console.Write("|");
                    }
                }
                if (i != matrice.GetLength(0) - 1) Console.WriteLine();
            }
        }

        /// <summary>
        /// Méthode permettant à l'utilisateur d'appliquer des noyaux de convolution sur des instances de la classe MonImage
        /// </summary>
        static void MatricesConvolution()
        {
            Console.WriteLine("\t Utiliser des matrices de convolution.");
            #region noyeaux
            float[,] identite = { { 0, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } };
            float[,] contours1 = { { 1, 0, -1 }, { 0, 0, 0 }, { -1, 0, 1 } };
            float[,] contours2 = { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
            float[,] contours3 = { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };
            float[,] nettete = { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
            float[,] bossage = { { -1, -1, 0 }, { -1, 0, 1 }, { 0, 1, 1 } };
            float[,] repoussage = { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
            float[,] boxBur = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
            float[,] flouGauss3 = { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } };
            float[,] flouGauss5 = { { 1, 4, 6, 4, 1 }, { 4, 16, 24, 16, 4 }, { 6, 24, 36, 24, 6 }, { 4, 16, 24, 16, 4 }, { 1, 4, 6, 4, 1 } };
            float[,] masqueFlou5 = { { 1, 4, 6, 4, 1 }, { 4, 16, 24, 16, 4 }, { 6, 24, -476, 24, 6 }, { 4, 16, 24, 16, 4 }, { 1, 4, 6, 4, 1 } };
            #endregion
            Console.WriteLine("1 - Identité");
            Console.WriteLine("2 - Détection simple des contours");
            Console.WriteLine("3 - Détection moyenne des contours");
            Console.WriteLine("4 - Détection forte des contours");
            Console.WriteLine("5 - Amélioration de la netteté");
            Console.WriteLine("6 - Bossage");
            Console.WriteLine("7 - Repoussage");
            Console.WriteLine("8 - Box Bur");
            Console.WriteLine("9 - Flou de Gauss 3x3");
            Console.WriteLine("10 - Flou de Gauss 5x5");
            Console.WriteLine("11 - Masque flou 5x5");
            Console.WriteLine("12 - Entrez votre propre noyau de convolution");

            int choix = 0;
            while (choix < 1 || choix > 13)
            {
                Console.Write("Veuillez choisir un filtre à appliquer en indiquant un numéro entre 1 et 12 : ");
                int.TryParse(Console.ReadLine(), out choix);
            }
            float[,] noyau = null;
            int diviseur = 0;
            switch (choix)
            {
                case 1:
                    noyau = identite;
                    break;

                case 2:
                    noyau = contours1;
                    break;

                case 3:
                    noyau = contours2;
                    break;

                case 4:
                    noyau = contours3;
                    break;

                case 5:
                    noyau = nettete;
                    break;

                case 6:
                    noyau = bossage;
                    break;

                case 7:
                    noyau = repoussage;
                    break;

                case 8:
                    noyau = boxBur;
                    diviseur = 9;
                    break;

                case 9:
                    diviseur = 16;
                    noyau = flouGauss3;
                    break;

                case 10:
                    diviseur = 256;
                    noyau = flouGauss5;
                    break;

                case 11:
                    diviseur = -256;
                    noyau = masqueFlou5;
                    break;

                case 12:
                    noyau = CreerNoyauConvolution();
                    Console.WriteLine("Par combien souhaitez-vous diviser les éléments de votre matrice ?");
                    Console.Write("Pour ne pas diviser, appuyez sur ENTREE, sinon, entrez un entier : ");
                    int.TryParse(Console.ReadLine(), out diviseur);
                    break;
            }


            Console.WriteLine("Noyau choisi :");
            AfficherMatrice(noyau);
            if (diviseur != 0 && diviseur != 1)
            {
                Console.WriteLine("/" + diviseur);
                DiviserMatrice(noyau, diviseur);
            }
            else Console.WriteLine();

            Console.WriteLine("...Veuillez patienter pendant l'application du filtre...");
            image.Convolution(noyau);
            Console.WriteLine("Le filtre a bien été appliqué.");
            string nom = "convolution";
            SauvegardeImage(nom, "sortie");
        }        

        /// <summary>
        /// Méthode permettant à l'utilisateur de créer des fractales, en choisissant le zoom, le centrage, les couleurs
        /// </summary>
        static void CreationFractales()
        {
            Console.WriteLine("\t Création de fractales de Mandelbrot / Julia.");

            int largeur = 0;
            int hauteur = 0;
            while (largeur < 10)
            {
                Console.Write("Largeur de l'image en pixels (> 10) : ");
                int.TryParse(Console.ReadLine(), out largeur);
            }
            while (hauteur < 10)
            {
                Console.Write("Hauteur de l'image en pixels (> 10) : ");
                int.TryParse(Console.ReadLine(), out hauteur);
            }

            int frac = 0;
            Console.WriteLine("1 - Fractale de Mandelbrot");
            Console.WriteLine("2 - Fractale de Julia");
            while (frac != 1 && frac != 2)
            {
                Console.Write("Veuillez choisir entre 1 et 2 : ");
                int.TryParse(Console.ReadLine(), out frac);
            }
            Console.WriteLine();
            Complexe c = null;
            if (frac == 2)
            {
                Console.WriteLine("Veuillez entrer un nombre imaginaire c pour satisfaire la relation z = z²+c (Ensemble de Julia).");
                Console.WriteLine("Exemple de nombres à entrer pour fractales remarquables :");
                Console.WriteLine("c = -0,75 + 0i");
                Console.WriteLine("c = 0,3 + 0,5i");
                Console.WriteLine("c = 0,285 + 0,01i");
                Console.WriteLine("c = -0,4 + 0,6i");
                Console.WriteLine("c = -0,8 + 0,156i");
                Console.WriteLine("Les nombres non entiers s'écrivent avec une VIRGULE et non un POINT.");

                float re = 0;
                float im = 0;
                string test = "";
                while (test != Convert.ToString(re))
                //Boucle placée pour pas que l'utilisateur s'étonne, s'il a mis un . au lieu d'une , que son
                //ou son imaginaire soit considéré comme nul, l'incitant à faire attention à ce qu'il entre
                {
                    Console.Write("Partie réelle : ");
                    test = Console.ReadLine();
                    float.TryParse(test, out re);
                }
                test = "";
                while (test != Convert.ToString(im))
                {
                    Console.Write("Partie imaginaire : ");
                    test = Console.ReadLine();
                    float.TryParse(test, out im);
                }

                c = new Complexe(re, im);
                Console.WriteLine("z = z² + c, avec c = " + c.ToString() + "\n");
            }

            int central = 0;
            int nuance = 0;
            Console.WriteLine("1 - Noir");
            Console.WriteLine("2 - Rouge");
            Console.WriteLine("3 - Vert");
            Console.WriteLine("4 - Bleu");
            Console.WriteLine("5 - Cyan");
            Console.WriteLine("6 - Magenta");
            Console.WriteLine("7 - Jaune");

            Console.WriteLine("\nVeuillez choisir la couleur des pixels centraux, (pour lesquels la suite complexe z(n+1) = z²(n) + c converge");
            Console.Write("Entrez un chiffre entre 1 et 7 : ");
            int.TryParse(Console.ReadLine(), out central);
            if (central < 1 || central > 7) central = 1;

            Console.WriteLine("\nVeuillez choisir la nuance de couleur autour de la fractale (noir = nuances de gris)");
            Console.Write("Entrez un chiffre entre 0 et 7 (0 = aucune nuance) : ");
            int.TryParse(Console.ReadLine(), out nuance);
            if (nuance < 0 || nuance > 7) nuance = 0;

            //Certaines fractales sont plus belle si l'on augmente le nombre max d'intensité, mais le résultat prends plus de temps
            //à s'afficher. Pas envie de faire trop d'options donc j'enlève celle-ci
            //int max = 50;
            //Console.WriteLine("\nVeuillez choisir le nombre maximal d'itérations pour considérer que la suite donnée par z(n+1) = z²(n) + c converge (minimum 50).");
            //Console.Write("Entrez un nombre ou appuyez sur ENTREE pour une valeur par défaut (50) : ");
            //int.TryParse(Console.ReadLine(), out max);
            //if (max < 50) max = 50;

            bool continuer = true;
            while (continuer)
            //Boucle pour que l'utilisateur puisse continuer à zoomer et décaler l'image créée sans avoir à redéfinir tous les paramètres
            {
                int zoom = 50;
                Console.Write("\nQuel zoom, en pourcentage, voulez-vous effectuer ? (par défaut 50) : ");
                int.TryParse(Console.ReadLine(), out zoom);
                if (zoom < 1) zoom = 50;

                int centreRe = 0;
                int centreIm = 0;
                Console.Write("Veuillez choisir où centrer l'image par rapport à l'axe des réels, en nombre de pixels (par défaut 0) : ");
                int.TryParse(Console.ReadLine(), out centreRe);
                Console.Write("Veuillez choisir où centrer l'image par rapport à l'axe des imaginaires, en nombre de pixels (par défaut 0) : ");
                int.TryParse(Console.ReadLine(), out centreIm);
                Console.WriteLine("...  Veuillez patienter pendant la création de votre image...");

                MonImage fractale = new MonImage(hauteur, largeur);
                fractale.Fractales(c, zoom, centreRe, centreIm, central, nuance);

                fractale.EnregistrerImage("sortie\\fractale.bmp");
                Console.WriteLine("Fractale créée. Appuyez sur n'importe quelle touche pour l'afficher.");
                Console.ReadKey();
                Process.Start("sortie\\fractale.bmp");

                Console.WriteLine("\nVoulez-vous continuer avec cette fractale (modifier zoom et centrage du plan) ?");
                Console.WriteLine("Veuillez répondre par OUI ou par NON. (Appuyer sur ENTREE sera considéré comme NON");
                if (Console.ReadLine().ToUpper() != "OUI") continuer = false;

            }
        }

        /// <summary>
        /// Méthode permettant à l'utilisateur de choisir une image à cacher dans une autre, et de retrouver cette image
        /// </summary>
        static void CacherImage()
        {
            Console.WriteLine("\t Cacher une image dans une autre.");
            //Console.WriteLine("Veuillez choisir une première photo");
            //MonImage image = InitialiserImage();
            MonImage image1 = new MonImage(image);
            Console.WriteLine("Veuillez choisir l'image que vous souhaitez cacher");
            InitialiserImage("images");
            MonImage image2 = new MonImage(image);
            Console.Clear();

            Console.WriteLine("...Veuillez patienter pendant le traitement des images...");
            image1.CacherImage(image2);
            Console.WriteLine("Image cachée.");
            image = image1;
            SauvegardeImage("code", "imagescode");
        }

        /// <summary>
        /// Récupère une image cachée dans une autre
        /// </summary>
        static void RecupImage()
        {
            if (InitialiserImage("imagescode"))
            {
                Console.WriteLine("Appuyez sur n'importe quelle touche pour récupérer l'image cachée");
                Console.ReadKey();
                Console.WriteLine("...Veuillez patienter pendant la récupération de l'image...");

                image.RecuperationImages();
                Console.WriteLine("Image récupérée.");
                SauvegardeImage("decode", "sortie");
            }
        }

        /// <summary>
        /// Applique à une image la méthode de réalisation de son histogramme
        /// </summary>
        static void FaireHistogramme()
        {
            Console.WriteLine("\t Réaliser l'histogramme d'une image");
            //MonImage image = InitialiserImage();
            Console.WriteLine("... Veuillez patienter pendant la création de l'histogramme ...");
            MonImage histogramme = image.Histogramme();
            string sauvegarde = "sortie\\" + image.Nom + "_histogramme.bmp";
            histogramme.EnregistrerImage(sauvegarde);
            Console.WriteLine("Histogramme créé, et enregistré dans " + sauvegarde); 
            Console.WriteLine("Pour l'afficher, appuyez sur ENTREE. Sinon, appuyez sur n'importe quelle touche pour continuer.");
            ConsoleKeyInfo suite = Console.ReadKey();
            if (suite.Key == ConsoleKey.Enter)
            {
                Process.Start(sauvegarde);
            }
        }

        /// <summary>
        /// Crée un code QR
        /// </summary>
        static void CreerQR()
        {
            Console.WriteLine("\tCréation d'un Code QR");
            char niveau = ' ';
            Console.WriteLine("L - Faible niveau de correction (7%)");
            Console.WriteLine("\tCapacité max --> Alphanumérique : 114 caractères ; UTF-8 : 78 caractères\n");
            Console.WriteLine("M - Moyen niveau de correction (15%)");
            Console.WriteLine("\tCapacité max --> Alphanumérique : 90 caractères ; UTF-8 : 62 caractères\n");
            Console.WriteLine("Q - Niveau de correction élevé (25%)");
            Console.WriteLine("\tCapacité max --> Alphanumérique : 67 caractères ; UTF-8 : 46 caractères\n");
            Console.WriteLine("H - Niveau de correction très élevé (30%)");
            Console.WriteLine("\tCapacité max --> Alphanumérique : 50 caractères ; UTF-8 : 34 caractères\n");
            while (niveau != 'L' && niveau != 'M' && niveau != 'Q' && niveau != 'H')
            {
                Console.Write("Veuillez choisir un niveau de correction d'erreur : ");
                char.TryParse(Console.ReadLine().ToUpper(), out niveau);
            }

            Console.WriteLine();
            Console.WriteLine("Veuillez choisir un numéro, entre 0 et 7, de masque à appliquer.");
            Console.Write("Ou bien, appuyez simplement sur ENTREE pour déterminer automatiquement le meilleur masque à appliquer : ");
            string m = Console.ReadLine();
            int masque;
            if (m == "0") masque = 0;
            else
            {
                int.TryParse(m, out masque);
                if (masque < 1 || masque > 7) masque = -1;
            }
            Console.WriteLine();
            Console.Write("Veuillez entrer le mot ou la phrase que vous souhaitez coder : ");
            string phrase = Console.ReadLine();
            QRCode qr = new QRCode(phrase, niveau, masque);
            while (qr.Version == 0)
            {
                Console.WriteLine("\nLa phrase que vous avez entré fait " + phrase.Length + " caractères. Attention à la longueur de la phrase");
                Console.Write("Veuillez entrer le mot ou la phrase que vous souhaitez coder : ");
                phrase = Console.ReadLine();
                qr = new QRCode(phrase, niveau, masque);
            }
            Console.WriteLine("...Veuillez patienter pendant la création du QR Code...");
            qr.ImageQR();

            Console.Clear();
            Console.WriteLine(qr.ToString());
            Console.Write("Veuillez donner un nom à votre QR Code pour l'enregistrer : ");
            bool refaire = true;
            string fichier = "";
            while (refaire)
            {
                try
                {
                    string nom = Console.ReadLine().ToLower();
                    if (nom == "") throw new Exception();
                    fichier = "qrcode\\" + nom + ".bmp";
                    qr.Img.EnregistrerImage(fichier);
                    refaire = false;
                }
                catch
                {
                    Console.WriteLine("Le nom que vous avez attribué n'est pas conforme ou contient des caractères spéciaux.");
                    Console.WriteLine("Le QR Code n'a pas pu être enregistrée.");
                    Console.Write("Veuillez rentrer un nom pour votre QR Code : ");
                }
            }
            
            Console.WriteLine("Image enregistrée dans " + fichier);
            Console.WriteLine("Pour afficher le QR Code, appuyez sur ENTREE. Sinon, appuyez sur n'importe quelle touche pour continuer.");
            ConsoleKeyInfo suite = Console.ReadKey();
            if (suite.Key == ConsoleKey.Enter)
            {
                Process.Start(fichier);
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Récupère un QR Code créé au préalable et affiche toutes les données encodées
        /// </summary>
        static void DecoderQR()
        {
            Console.WriteLine("\tDécoder un QR Code");
            string nomDossier = "qrcode";
            bool refaire = true;
            string fichier = "";

            AfficherDossier(nomDossier);
            Console.Write("\nVeuillez indiquer le nom du QR Code que vous souhaitez déchiffrer : ");
            string nom = "";
            int essai = 3;
            while (refaire)
            {
                try
                {
                    nom = Console.ReadLine().ToLower();
                    fichier = nomDossier + "\\" + nom + ".bmp";
                    File.OpenRead(fichier); //Teste l'existence du fichier
                    refaire = false;
                }
                catch
                {
                    Console.WriteLine("Le nom que vous avez entré ne correspond pas. Essais restants : " + essai);
                    Console.Write("Veuillez recommencer : ");

                    refaire = essai-- > 0;
                }
            }
            if (essai > 0)
            {
                Console.WriteLine("Pour afficher le QR Code, appuyez sur ENTREE. Sinon, appuyez sur n'importe quelle touche pour continuer.");
                ConsoleKeyInfo suite = Console.ReadKey();
                if (suite.Key == ConsoleKey.Enter)
                {
                    Process.Start(fichier);
                }
                Console.WriteLine();
                QRCode qr = new QRCode(fichier);
                Console.WriteLine(qr.Version == 0 ? "Le QR Code n'est pas déchiffrable ... Désolé." : qr.ToString());
                Console.WriteLine("\nAppuyez sur n'importe quelle touche pour continuer");
                Console.ReadKey();
            }
        }

        static void Main(string[] args)
        {
            while (true)
            {
                //Console.Write("Pour quitter, appuyez sur ECHAP. Sinon, appuyez sur n'importe quelle touche pour continuer.");
                //ConsoleKeyInfo fin = Console.ReadKey();
                //if (fin.Key == ConsoleKey.Escape) break;
                //Console.Clear();
                int choix1 = Menu1();
                switch (choix1)
                {
                    case 1:
                        TraiterImages();
                        break;

                    case 2:
                        RecupImage();
                        break;

                    case 3:
                        CreationFractales();
                        break;

                    case 4:
                        InitialiserImage("sortie");
                        break;

                    case 5:
                        CreerQR();
                        break;

                    case 6:
                        DecoderQR();
                        break;
                }
                Console.Clear();
            }
        }
    }
}
