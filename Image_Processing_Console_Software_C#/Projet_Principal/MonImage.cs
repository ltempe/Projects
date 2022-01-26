using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Traitement_Images
{
    /// <summary>
    /// Classe principale, dans laquelle on traite les images
    /// </summary>
    public class MonImage
    {
        #region Champs
        string nom;

        string typeImg;
        int tailleFichier;
        int tailleOffset;
        int largeur;
        int hauteur;
        int nbBits;
        int headerInfo;
        Pixel[,] image;
        byte[] imageByte;
        #endregion

        #region Constructeurs
        /// <summary>
        /// Constructeur d'une instance de la classe MonImage à partir d'un fichier
        /// </summary>
        /// <param name="fichier">Fichier de l'image à instancier</param>
        public MonImage(string fichier)
        {
            //Récupération du fichier
            byte[] monFichier = File.ReadAllBytes(fichier);
            //Récupération des infos en convertissant en caractère ou entier
            typeImg = "" + Convert.ToChar(monFichier[0]) + Convert.ToChar(monFichier[1]);
            tailleFichier = ConvertirEndianEntier(monFichier, 2, 5);
            tailleOffset = ConvertirEndianEntier(monFichier, 10, 13);
            headerInfo = ConvertirEndianEntier(monFichier,14, 17);
            largeur = ConvertirEndianEntier(monFichier, 18, 21);
            hauteur = ConvertirEndianEntier(monFichier, 22, 25);
            nbBits = ConvertirEndianEntier(monFichier, 28, 29);

            int index = tailleOffset;

            //Pour les pixels, création d'instances de la classe Pixel
            image = new Pixel[hauteur, largeur];
            //Le choix de placer l'indice de la hauteur avant celui de la largeur a été fait par habitude et pas analogie 
            //avec les lignes et les colonnes des matrices

            switch (nbBits)
            {
                case 1:
                    //for (int x = 0; x < hauteur; x++)
                    //{
                    //    for (int y = 0; y < largeur; y += 8)
                    //    {
                    //        byte pix = monFichier[index++];
                    //        for (int i = y; i < y + 8 && i < largeur; i++, pix /= 2)
                    //        {
                    //            byte couleur = 0;
                    //            if (pix != 0) couleur = (byte)(255 * (pix % 2));
                    //            image[x, i] = new Pixel(couleur, couleur, couleur);
                    //        }
                    //    }
                    //}
                    break;

                case 24:
                    for (int x = 0; x < hauteur; x++)
                    {
                        for (int y = 0; y < largeur; y++)
                        {
                            //Les pixels sont écrits à l'envers dans le fichier (BVR au lieu de RVB)
                            byte bleu = monFichier[index++];
                            byte vert = monFichier[index++];
                            byte rouge = monFichier[index++];

                            Pixel pix = new Pixel(rouge, vert, bleu);
                            image[x, y] = pix;
                        }
                    }
                    break;
            }
            
        }

        /// <summary>
        /// Constructeur par copie, pour cloner une instance de la classe
        /// </summary>
        /// <param name="img">Image à copier</param>
        public MonImage(MonImage img)
        {
            this.typeImg = img.typeImg;
            this.tailleFichier = img.tailleFichier;
            this.tailleOffset = img.tailleOffset;
            this.largeur = img.largeur;
            this.hauteur = img.hauteur;
            this.nbBits = img.nbBits;
            this.headerInfo = img.headerInfo;
            this.nom = img.nom;
            this.image = new Pixel[hauteur, largeur];
            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    this.image[x, y] = img.image[x, y];
                }
            }
        }

        /// <summary>
        /// Crée une instance de la classe MonImage, au format bitmap, avec que des pixels blancs
        /// </summary>
        /// <param name="h">Hauteur de l'image créée</param>
        /// <param name="l">Largeur de l'image créée</param>
        public MonImage(int h, int l)
        {
            typeImg = "BM";
            while (h % 4 != 0) h++;
            while (l % 4 != 0) l++;
            this.hauteur = h;
            this.largeur = l;
            tailleOffset = 54;
            headerInfo = 40;
            nbBits = 24;
            tailleFichier = tailleOffset + (NbBits * h * l / 8);
            image = new Pixel[h, l];
            for (int x = 0; x < h; x++)
            {
                for (int y = 0; y < l; y++)
                {
                    Pixel pix = new Pixel(255, 255, 255);
                    image[x, y] = pix;
                }
            }
        }

        #endregion

        #region Attributs
        /// <summary>
        /// Attribut du type de l'image
        /// </summary>
        public string TypeImg
        {
            get { return this.typeImg; }
        }

        /// <summary>
        /// Attribut de la taille du fichier
        /// </summary>
        public int TailleFichier
        {
            get { return this.tailleFichier; }
        }

        /// <summary>
        /// Attribut de la hauteur de l'image
        /// </summary>
        public int Largeur
        {
            get { return this.largeur; }
        }

        /// <summary>
        /// Attribut de la largeur de l'image
        /// </summary>
        public int Hauteur
        {
            get { return this.hauteur; }
        }

        /// <summary>
        /// Attribut du nombre de bits par couleur
        /// </summary>
        public int NbBits
        {
            get { return this.nbBits; }
        }

        /// <summary>
        /// Attribut de la matrice de pixels de l'image
        /// </summary>
        public Pixel[,] Image
        {
            get { return this.image; }
        }

        /// <summary>
        /// Nom de l'image, utile uniquement pour enregistrer l'image traitée
        /// </summary>
        public string Nom
        {
            get { return this.nom; }
            set { this.nom = value; }
        }
        #endregion

        #region Methodes
        /// <summary>
        /// Enregistre une instance de la classe dans un fichier image
        /// </summary>
        /// <param name="fichier">Chemin d'accès du fichier à enregistrer</param>
        public void EnregistrerImage(string fichier)
        {
            tailleFichier = tailleOffset + (hauteur * largeur * nbBits / 8);

            imageByte = new byte[tailleFichier];

            //Conversion de tous les attributs de l'instance de la classe en octets
            imageByte[0] = Convert.ToByte(typeImg[0]);
            imageByte[1] = Convert.ToByte(typeImg[1]);

            byte[] tailleFichierEndian = ConvertirEntierEndian(tailleFichier, 4);
            RemplirTableauOctets(tailleFichierEndian, 2);

            byte[] tailleOffsetEndian = ConvertirEntierEndian(tailleOffset, 4);
            RemplirTableauOctets(tailleOffsetEndian, 10);

            byte[] headerInfoEndian = ConvertirEntierEndian(headerInfo, 4);
            RemplirTableauOctets(headerInfoEndian, 14);

            byte[] largeurEndian = ConvertirEntierEndian(largeur, 4);
            RemplirTableauOctets(largeurEndian, 18);

            byte[] hauteurEndian = ConvertirEntierEndian(hauteur, 4);
            RemplirTableauOctets(hauteurEndian, 22);

            byte[] nbBitsEndian = ConvertirEntierEndian(nbBits, 2);
            RemplirTableauOctets(nbBitsEndian, 28);

            int index = tailleOffset;

            switch (nbBits)
            {
                case 1:

                    //imageByte[58] = 255;
                    //imageByte[59] = 255;
                    //imageByte[60] = 255;

                    //for (int x = 0; x < hauteur; x++)
                    //{
                    //    byte pix = 0;
                    //    for (int y = 0; y < largeur; y++)
                    //    {
                    //        for (int j = 1; j < 128; y++, j *= 2)
                    //        {
                    //            pix += (byte)(j * Convert.ToByte(image[x, y].Somme() > 484));
                    //        }
                    //        imageByte[index++] = pix;
                    //    }
                    //}
                    break;

                case 8:
                    //for (int x = 0; x < hauteur; x++)
                    //{
                    //    for (int y = 0; y < largeur; y++)
                    //    {
                    //        byte moyenne = (byte)(image[x, y].Somme() / 3);
                    //        imageByte[index++] = moyenne;
                    //    }
                    //}
                    break;

                case 24:
                    for (int x = 0; x < hauteur; x++)
                    {
                        for (int y = 0; y < largeur; y++)
                        {
                            //Positionnement des pixels à l'envers (BVR au lieu de RVB)
                            imageByte[index++] = image[x, y].B;
                            imageByte[index++] = image[x, y].V;
                            imageByte[index++] = image[x, y].R;
                        }
                    }
                    break;
            }
            
            File.WriteAllBytes(fichier, imageByte);
        }

        /// <summary>
        /// Convertit une séquence d'octets, au format little endian, en entier
        /// </summary>
        /// <param name="endian">Tableau d'octets dont on souhaite convertir une séquence</param>
        /// <param name="debut">Début de la séquence à convertir</param>
        /// <param name="fin">Fin de la séquence à convertir</param>
        /// <returns>Entier correspondant à la séquence d'octets entrée</returns>
        public static int ConvertirEndianEntier(byte[] endian, int debut, int fin)
        {
            int entier = 0;
            for (int i = debut, j = 1; i <= fin; i++, j *= 256)
            {
                entier += j * endian[i];
            }
            return entier;
        }
        
        /// <summary>
        /// Convertit un entier en une séquence d'octets au format little endian
        /// </summary>
        /// <param name="entier">Entier à convertir</param>
        /// <param name="taille">Taille de la séquence d'octets</param>
        /// <returns>Séquence d'octets traduisant l'entier entré</returns>
        public static byte[] ConvertirEntierEndian(int entier, int taille)
        {
            byte[] endian = new byte[taille];
            for (int i = 0; i < taille; i++, entier /= 256)
            {
                endian[i] = (byte)(entier % 256);
            }
            return endian;
        }

        /// <summary>
        /// Code un octet sur 8 bits dans un tableau de booleens
        /// </summary>
        /// <param name="octet">Octet à coder</param>
        /// <returns>Octet codé dans un tableau binaire</returns>
        public static bool[] ConvertirOctetBinaire(byte octet)
        {
            bool[] binaire = new bool[8];
            for (int i = 7; i >= 0; i--, octet /= 2) binaire[i] = (octet % 2 == 1);
            return binaire;
        }

        /// <summary>
        /// Convertit un tableau binaire de 8 bits en octet
        /// </summary>
        /// <param name="binaire">Tableau à convertir</param>
        /// <returns>Octet traduit</returns>
        public static byte ConvertirBinaireOctet(bool[] binaire)
        {
            byte octet = 0;
            int puissance = 1;
            for (int i = 7; i >= 0; i--, puissance *= 2) octet += (byte)(puissance * Convert.ToByte(binaire[i]));
            return octet;
        }

        /// <summary>
        /// Rempit une séquence de cases d'un tableau d'octets par une séquence d'octets
        /// </summary>
        /// <param name="sequence">Séquence d'octets à implémenter</param>
        /// <param name="indice">Position de l'emplacement de début dans le tableau</param>
        void RemplirTableauOctets(byte[] sequence, int indice)
        {
            for (int i = indice; i < indice + sequence.Length; i++)
            {
                imageByte[i] = sequence[i - indice];
            }
        }

        #region anciennesMethodes
        // ----------- 3 méthodes qui n'ont plus d'utilité maintenant ---------------
        /// <summary>
        /// Retourne l'image de 180 degrés
        /// </summary>
        public void Pivoter180()
        {
            Pixel[,] img = new Pixel[hauteur, largeur];
            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    img[x, y] = image[hauteur - x - 1, largeur - y - 1];
                }
            }
            image = img;
        }

        /// <summary>
        /// Retourne l'image de 90 degrés dans le sens direct
        /// </summary>
        public void Pivoter90Direct()
        {
            Pixel[,] img = new Pixel[largeur, hauteur];
            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    img[y, x] = image[hauteur - x - 1, largeur - y - 1];
                }
            }
            int valeur = hauteur;
            hauteur = largeur;
            largeur = valeur;
            image = img;
        }

        /// <summary>
        /// Retourne l'image de 90 degrés dans le sens horaire
        /// </summary>
        public void Pivoter90Horaire()
        {
            Pixel[,] img = new Pixel[largeur, hauteur];
            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    img[y, x] = image[x, y];
                }
            }
            int valeur = hauteur;
            hauteur = largeur;
            largeur = valeur;
            image = img;
        }

        //---------------------------------------------------------------------------
        #endregion

        /// <summary>
        /// Effectue une rotation de l'image selon un angle entré en paramètre, dans le sens direct
        /// </summary>
        /// <param name="angle">Angle de rotation en degrés</param>
        public void Rotation(int angle)
        {
            //On convertit d'abord l'angle en radian pour utiliser la trigonométrie
            double rad = angle * Math.PI / 180.0;
            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);

            //Changement de la taille de l'image selon la nouvelle base formée par l'angle
            int nvHauteur = (int)(Math.Abs(cos) * hauteur + Math.Abs(sin) * largeur);
            int nvLargeur = (int)(Math.Abs(cos) * largeur + Math.Abs(sin) * hauteur); 
            //On prévoit une nouvelle largeur multiple de 4
            while (nvLargeur % 4 != 0) nvLargeur++;
            Pixel[,] img = new Pixel[nvHauteur, nvLargeur];
            for (int x = 0; x < nvHauteur; x++)
            {
                for (int y = 0; y < nvLargeur; y++)
                {
                    //Tous les pixels ne reproduisant pas l'image est initialisé en blanc
                    Pixel pix = new Pixel(255, 255, 255);
                    img[x, y] = pix;

                    //On applique le calcul de matrice de rotation pour chaque pixel, en effectuant la
                    //rotation autour du centre de l'image
                    int i = (int)(cos * (x - nvHauteur/2) - sin * (y - nvLargeur/2) + hauteur/2);
                    int j = (int)(sin * (x - nvHauteur/2) + cos * (y - nvLargeur/2) + largeur/2);

                    //Si i et j sont bien définis, on place le pixel sur la nouvelle matrice de pixels
                    if (!(i < 0 || j < 0 || i >= hauteur || j >= largeur)) img[x, y] = image[i, j];
                }
            }
            //On actualise enfin l'instance de la classe avec ses nouvelles propriétés
            hauteur = nvHauteur;
            largeur = nvLargeur;
            tailleFichier = tailleOffset + (hauteur * largeur * nbBits / 8);
            image = img;
        }

        /// <summary>
        /// Fait une rotation selon l'axe vertical
        /// </summary>
        public void SymetrieVerticale()
        { 
            Pixel[,] img = new Pixel[hauteur, largeur];
            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    //On inverse la position des pixels par rapport à l'axe vertical du milieu
                    img[x, y] = image[x, largeur - y - 1];
                }
            }
            image = img;
        }

        /// <summary>
        /// Fait une rotation selon l'axe horizontal
        /// </summary>
        public void SymetrieHorizontale()
        {
            Pixel[,] img = new Pixel[hauteur, largeur];
            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    //On inverse la position des pixels par rapport à l'axe vertical du milieu
                    img[x, y] = image[hauteur - x - 1, y];
                }
            }
            image = img;
        }

        /// <summary>
        /// Transforme une image en niveaux de gris
        /// </summary>
        public void NuancesGris()
        {
            Pixel[,] img = new Pixel[hauteur, largeur];
            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    //On fait une moyenne des 3 bytes de chaque pixel pour avoir une intensité uniforme sur les bytes
                    int moyenne = image[x, y].Somme() / 3;
                    byte couleur = Convert.ToByte(moyenne);
                    Pixel pix = new Pixel(couleur, couleur, couleur);
                    img[x, y] = pix;
                }
            }
            image = img;
        }

        /// <summary>
        /// Transforme une image couleur en noir et blanc
        /// </summary>
        public void NoirBlanc()
        {
            Pixel[,] img = new Pixel[hauteur, largeur];
            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    Pixel pix = new Pixel(0, 0, 0);
                    //A partir d'un certain seuil, si le pixel est trop clair, il est blanc,
                    //sinon il reste sombre
                    if (image[x, y].Somme() > 384) pix = new Pixel(255, 255, 255);
                    img[x, y] = pix;
                }
            }
            image = img;
        }

        /// <summary>
        /// Inverse les couleurs d'une image
        /// </summary>
        public void Negatif()
        {
            Pixel[,] img = new Pixel[hauteur, largeur];
            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    //On inverse simplement les couleurs en faisant 255 - intensite
                    byte rougeNeg = Convert.ToByte(255 - image[x, y].R);
                    byte vertNeg = Convert.ToByte(255 - image[x, y].V);
                    byte bleuNeg = Convert.ToByte(255 - image[x, y].B);
                    Pixel pix = new Pixel(rougeNeg, vertNeg, bleuNeg);
                    img[x, y] = pix;
                }
            }
            image = img;
        }

        /// <summary>
        /// Change la taille de l'image en fonction du pourcentage entré en paramètre
        /// </summary>
        /// <param name="pourcentage">Inférieur à 100 : rétrécit l'image. Supérieur à 100 : agrandit l'image</param>
        public void ChangerTaille(int pourcentage)
        {
            float coefficient = pourcentage / 100f;
            int nouvelleHauteur = (int)(hauteur * coefficient);
            int nouvelleLargeur = (int)(largeur * coefficient);

            //Gérer les exceptions pour les non-multiples de 4
            nouvelleLargeur -= nouvelleLargeur%4;

            if (nouvelleLargeur == 0)
            //Dans le cas où on s'amuse à vouloir une image de taille 1% par exemple, on évite une erreur
            //en proposant la plus petit taille possible en bitmap, en conservant le ratio hauteur/largeur
            {
                nouvelleLargeur = 4;
                nouvelleHauteur = 4 * hauteur / largeur;
            }
            Pixel[,] img = new Pixel[nouvelleHauteur, nouvelleLargeur];
            if (coefficient < 1)
            //Pour un coefficient < 1, on rétrécit l'image, on parcourt donc l'image de base
            {
                for (int x = 0; x < hauteur; x++)
                {
                    for (int y = 0; y < largeur; y++)
                    {
                        //On multiplie les indices par le coefficient
                        int x2 = (int)(x * coefficient);
                        int y2 = (int)(y * coefficient);
                        //On gère les exceptions dues à l'arrondis lors de la multiplication par le coefficient flottant
                        if (x2 >= nouvelleHauteur) x2 = nouvelleHauteur - 1;
                        if (y2 >= nouvelleLargeur) y2 = nouvelleLargeur - 1;
                        img[x2, y2] = image[x, y];
                    }
                }
            }
            else
            //Pour un coefficient <= 1, on agrandit l'image, on parcourt donc la nouvelle image
            {
                for (int x = 0; x < nouvelleHauteur; x++)
                {
                    for (int y = 0; y < nouvelleLargeur; y++)
                    {
                        //On multiplie les indices par le coefficient
                        int x2 = (int)(x / coefficient);
                        int y2 = (int)(y / coefficient);
                        //On gère les exceptions dues à l'arrondis lors de la multiplication par le coefficient flottant
                        if (x2 >= hauteur) x2 = hauteur - 1;
                        if (y2 >= largeur) y2 = largeur - 1;
                        img[x, y] = image[x2, y2];
                    }
                }
            }
            //Et on n'oublie pas d'actualiser les champs de l'objet
            image = img;
            hauteur = nouvelleHauteur;
            largeur = nouvelleLargeur;
            tailleFichier = tailleOffset + (hauteur * largeur * nbBits / 8);
        }

        /// <summary>
        /// Additionne les produits de tous les pixels voisins aux éléments du noyau entré en paramètre
        /// </summary>
        /// <param name="noyau">Noyeau de convolution</param>
        /// <param name="ligne">Ligne du pixel sur lequel on travaille, dans la matrice de pixels</param>
        /// <param name="colonne">Colonne du pixel sur lequel on travaille, dans la matrice de pixels</param>
        /// <returns></returns>
        Pixel SommePixels(float[,] noyau, int ligne, int colonne)
        {
            //On initialise les sommes à 0
            float sommeR = 0;
            float sommeV = 0;
            float sommeB = 0;

            int ligneNoyau = noyau.GetLength(0);
            int colonneNoyau = noyau.GetLength(1);
            for (int i = 0; i < ligneNoyau; i++)
            {
                for (int j = 0; j < colonneNoyau; j++)
                {
                    //On prend les cases autour du pixel central qui lui sera multiplié par l'élément central du noyau
                    int x = i + (ligne - ligneNoyau / 2);
                    int y = j + (colonne - colonneNoyau / 2);

                    //Pour les bords, on utilise la méthode d'extension :
                    //--> Les pixels en dehors des limites de l'image seront les mêmes que les pixels en bordure, pour faire la somme
                    if (x < 0) x = 0;
                    if (x >= hauteur) x = ligneNoyau - 1;
                    if (y < 0) y = 0;
                    if (y >= largeur) y = colonneNoyau - 1;

                    //On peut ainsi sommer les produits
                    sommeR += noyau[i, j] * image[x,y].R;
                    sommeV += noyau[i, j] * image[x,y].V;
                    sommeB += noyau[i, j] * image[x,y].B;
                }
            }
            //On calcule la somme des élements du noyeau
            int sommeM = 0;
            foreach (int element in noyau) sommeM += element;
            if (sommeM == 0) sommeM = 1;
            sommeR = Math.Abs(sommeR / sommeM);
            sommeV = Math.Abs(sommeV / sommeM);
            sommeB = Math.Abs(sommeB / sommeM);
            Pixel somme = new Pixel((byte)sommeR,(byte)sommeV,(byte)sommeB);
            return somme;
        }

        /// <summary>
        /// Effectue une convolution sur l'image à partir d'un noyau de convolution entré en paramètre
        /// </summary>
        /// <param name="noyau">Noyau de convolution</param>
        public void Convolution(float[,] noyau)
        {
            Pixel[,] convolution = new Pixel[hauteur, largeur];
            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    //Pour chaque pixel, on fait la somme des produits de chaque élément du noyau de convolution
                    //avec les pixels entourant ce pixel, qui lui sera multiplié par l'élement central du noyau
                    Pixel pix = SommePixels(noyau, x, y);
                    convolution[x, y] = pix;
                }
            }
            image = convolution;
        }

        /// <summary>
        /// Insère une fractale de julia dont la suite complexe est z(n+1) = z(n)²+c, avec c entré en paramètre
        /// </summary>
        /// <param name="c">Complexe que l'on additionne au carré de chaque élément du plan complexe pour obtenir la suite complexe</param>
        /// <param name="zoom">Pourcentage de zoom de la fractale</param>
        /// <param name="centreRe">Point central de l'image sur l'axe des réels</param>
        /// <param name="centreIm">Point central de l'image sur l'axe des imaginaires</param>
        /// <param name="central">Numero de la couleur des points centraux, pour lesquels la suite complexe converge</param>
        /// <param name="nuance">Numero de la couleur des points autour, en fonction du temps que met la suite complexe à diverger</param>
        /// <param name="max">Nombre maximal d'itération</param>
        public void Fractales(Complexe c, float zoom, float centreRe, float centreIm, int central, int nuance, int max = 50)
        {
            //Par défaut, l'origine du repère complexe (0,0) est placé au centre de l'image
            centreRe = (largeur / 2f) - (centreRe / 2f);
            centreIm = (hauteur / 2f) - (centreIm / 2f);
            zoom /= 100; //Conversion du zoom en pourcentage
            bool mandelbrot = c == null;
            //Si le complexe c est null, on changera l'algorithme dans la boucle pour créer une fractale de Mandelbrot
            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    //Point réel placé selon le pixel de la colonne de l'image soustraite au réel 0
                    //le tout divisé par le produit de la hauteur totale et du zoom en pourcentage
                    //De même pour le point imaginaire avec la ligne de l'image

                    float re = (y - centreRe) / (zoom * hauteur); //zoom*hauteur pour re et im pour ne pas avoir de déformation
                    float im = (x - centreIm) / (zoom * hauteur); //dans les cas où la largeur et la hauteur sont différents
                    Complexe z = new Complexe(re, im);
                    if (mandelbrot)
                    {
                        c = new Complexe(re, im);
                        z = new Complexe(0, 0);
                    }
                    int n = Converge(z, c, max);

                    //Si n a atteint la valeur "max", cela revient à dire que le module de z est inférieur à 2 et donc qu'il converge.
                    //On colorie donc le pixel en couleur déterminée par l'utilisateau préalablement
                    if (n == max) ColorerFractale(0, x, y, central, n, max);
                    else
                    {
                        byte intensite = (byte)(255 - 255 * n / max);
                        ColorerFractale(intensite, x, y, nuance, n, max);
                    }
                }
            }
        }

        /// <summary>
        /// Vérifie si la suite complexez(n+1) = z(n)² + c converge après un nombre d'itérations
        /// </summary>
        /// <param name="z">Complexe z dans l'équation</param>
        /// <param name="c">Complexe c dans l'équation</param>
        /// <param name="max">Nombre max d'itérations</param>
        /// <returns>Nombre d'itérations de l'équation avant que la suite ne diverge</returns>
        public static int Converge(Complexe z, Complexe c, int max)
        {
            int n = 0;
            while (n < max && z.Module < 2)
            //L'objectif est d'appliquer la suite z(n+1) = z(n)² + c plusieurs fois et voir après un certain nombre d'ittérations
            //si z(n) converge vers l'infini.
            //On considèrera que z converge si son module est inférieur à 2 après "max" ittérations, et qu'il diverge autrement
            {
                z *= z;
                z += c;
                n++;
            }
            return n;
        }

        /// <summary>
        /// Colorie un pixel d'une fractale selon le choix de l'utilisateur, en fonction des paramètres de la fractale et de ce point
        /// Méthode purement esthétique
        /// </summary>
        /// <param name="intensite">Intensité de la couleur</param>
        /// <param name="x">Point de la ligne de la matrice de pixels de l'image</param>
        /// <param name="y">Point de la colonne de la matrice de pixels de l'image</param>
        /// <param name="couleur">Choix de couleur entré par l'utilisateur</param>
        /// <param name="n">Nombre d'itérations avant de sortir de la boucle while</param>
        /// <param name="max">Nombre maximal d'itérations pour sortir de la boucle while</param>
        void ColorerFractale(byte intensite, int x, int y, int couleur, int n, int max)
        {
            Pixel pix = null;
            switch (couleur)
            {
                case 1:
                    pix = new Pixel(intensite, intensite, intensite);
                    break;

                case 2:
                    pix = new Pixel(255, intensite, intensite);
                    break;

                case 3:
                    pix = new Pixel(intensite, 255, intensite);
                    break;

                case 4:
                    pix = new Pixel(intensite, intensite, 255);
                    break;

                case 5:
                    pix = new Pixel(intensite, 255, 255);
                    break;

                case 6:
                    pix = new Pixel(255, intensite, 255);
                    break;

                case 7:
                    pix = new Pixel(255, 255, intensite);
                    break;

                default:
                    pix = new Pixel(255, 255, 255);
                    break;
                }
                image[x, y] = pix;
        }

        /// <summary>
        /// Cache une image dans une autre image
        /// </summary>
        /// <param name="img">Image à cacher</param>
        public void CacherImage(MonImage img)
        {
            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    //Pour chaque pixel on convertit les valeurs en séquence de bits
                    bool[] r = ConvertirOctetBinaire(image[x, y].R);
                    bool[] v = ConvertirOctetBinaire(image[x, y].V);
                    bool[] b = ConvertirOctetBinaire(image[x, y].B);

                    int x1 = x * img.hauteur / hauteur;
                    int y1 = y * img.largeur / largeur;
                    bool[] r1 = ConvertirOctetBinaire(img.image[x1, y1].R);
                    bool[] v1 = ConvertirOctetBinaire(img.image[x1, y1].V);
                    bool[] b1 = ConvertirOctetBinaire(img.image[x1, y1].B);

                    //On prend simplement les 4 bits de poids fort de l'image à cacher que l'on implémente à la place des
                    //4 bits de poids faible de l'image
                    for (int i = 0; i < 4; i++)
                    {
                        r[i + 4] = r1[i];
                        v[i + 4] = v1[i];
                        b[i + 4] = b1[i];
                    }

                    Pixel pix = new Pixel(ConvertirBinaireOctet(r), ConvertirBinaireOctet(v), ConvertirBinaireOctet(b));

                    image[x, y] = pix;
                }
            }
            //On cache la taille de l'image cachée dans les 2 premiers pixels en convertissant hauteur et largeur au format endian
            byte[] hauteurCache = ConvertirEntierEndian(img.hauteur, 3);
            byte[] largeurCache = ConvertirEntierEndian(img.largeur, 3);
            image[0,0] = new Pixel(hauteurCache[0], hauteurCache[1], hauteurCache[2]);
            image[0,1] = new Pixel(largeurCache[0], largeurCache[1], largeurCache[2]);
        }

        /// <summary>
        /// Récupère une image qui est cachée dans une autre
        /// </summary>
        /// <returns>Image récupérée</returns>
        public void RecuperationImage()
        {
            //On récupère la hauteur et la largeur cachée dans les 2 premiers pixels
            Pixel h = image[0,0];
            Pixel l = image[0, 1];
            byte[] hauteurCache = { h.R, h.V, h.B };
            byte[] largeurCache = { l.R, l.V, l.B };
            int hauteurRecup = ConvertirEndianEntier(hauteurCache, 0, 2);
            int largeurRecup = ConvertirEndianEntier(largeurCache, 0, 2);

            Pixel[,] recup = new Pixel[hauteurRecup, largeurRecup];
            for (int x = 0; x < hauteurRecup; x++)
            {
                for (int y = 0; y < largeurRecup; y++)
                {
                    int xr = x * hauteur / hauteurRecup;
                    int yr = y * largeur / largeurRecup;
                    bool[] r = ConvertirOctetBinaire(image[xr, yr].R);
                    bool[] v = ConvertirOctetBinaire(image[xr, yr].V);
                    bool[] b = ConvertirOctetBinaire(image[xr, yr].B);

                    bool[] r1 = new bool[8];
                    bool[] v1 = new bool[8];
                    bool[] b1 = new bool[8];

                    //On récupère les bits de poids faible de l'image "double" et on les implémente comme bits de poids fort
                    //pour l'image que l'on souhaite décoder
                    for (int i = 0; i < 4; i++)
                    {
                        r1[i] = r[i + 4];
                        v1[i] = v[i + 4];
                        b1[i] = b[i + 4];
                    }
                    Pixel pix = new Pixel(ConvertirBinaireOctet(r1), ConvertirBinaireOctet(v1), ConvertirBinaireOctet(b1));
                    recup[x, y] = pix;
                }
            }
            image = recup;
            hauteur = hauteurRecup;
            largeur = largeurRecup;
            tailleFichier = tailleOffset + (hauteur * largeur * nbBits / 8);
        }

        #region Ancien code
        /// <summary>
        /// Compte le nombre d'occurrences d'une couleur de pixel dans une matrice de pixels
        /// Plus utilisée maintenant car trop coûteuse en complexité temporelle, j'ai trouvé une autre solution
        /// </summary>
        /// <param name="intensite">Intensité de la couleur, allant de 0 à 255</param>
        /// <param name="couleur">Couleur en question (R, V, ou B)</param>
        /// <returns>Nombre d'occurrence de l'intensité de la couleur</returns>
        public int CompterOccurrencesPixel(byte intensite, char couleur)
        {
            int occurrence = 0;
            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    switch (couleur)
                    {
                        case 'R':
                            if (image[x, y].R == intensite) occurrence++;
                            break;

                        case 'V':
                            if (image[x, y].V == intensite) occurrence++;
                            break;

                        case 'B':
                            if (image[x, y].B == intensite) occurrence++;
                            break;
                    }
                    
                }
            }
            return occurrence;
        }
        #endregion

        /// <summary>
        /// Calcule la valeur maximale d'un tableau d'entiers
        /// </summary>
        /// <param name="tableau">Tableau d'entiers</param>
        /// <returns>Valeur maximale de ce tableau</returns>
        public static int CalculerMax(int[] tableau)
        {
            int max = 0;
            for (int i = 0; i < tableau.Length; i++)
            {
                if (tableau[i] >= max) max = tableau[i];
            }
            return max;
        }

        /// <summary>
        /// Crée l'histogramme en couleurs superposées de l'instance de la classe MyImage, dans une nouvelle instance de cette classe
        /// </summary>
        /// <returns>Instance de la classe MyImage contenant l'histogramme au format bmp</returns>
        public MonImage Histogramme()
        {
            int[] tableauR = new int[256];
            int[] tableauV = new int[256];
            int[] tableauB = new int[256];

            int largeurHisto = 3 * 256; 
            //On fait une image de largeur un peu plus grande que 256 pixels pour la lisibilité
            int hauteurHisto = 9 * largeurHisto / 16;
            //On équilibre ensuite la largeur de l'histogramme pour avoir une image de 16:9 agréablement 
            //lisible sur la plupart des écrans
            MonImage histogramme = new MonImage(hauteurHisto, largeurHisto);

            #region Ancien code
            //for (int intensite = 0; intensite <= 255; intensite++)
            //{
            //    int occurrenceR = CompterOccurrencesPixel((byte)intensite, 'R');
            //    if (max < occurrenceR) max = occurrenceR;
            //    int occurrenceV = CompterOccurrencesPixel((byte)intensite, 'V');
            //    if (max < occurrenceV) max = occurrenceV;
            //    int occurrenceB = CompterOccurrencesPixel((byte)intensite, 'B');
            //    if (max < occurrenceB) max = occurrenceB;
            //    tableauR[intensite] = occurrenceR;
            //    tableauV[intensite] = occurrenceV;
            //    tableauB[intensite] = occurrenceB;
            //}
            #endregion

            for (int x = 0; x < hauteur; x++)
            {
                for (int y = 0; y < largeur; y++)
                {
                    for (int intensite = 0; intensite < 256; intensite++)
                    {
                        if (image[x, y].R == intensite) tableauR[intensite]++;
                        if (image[x, y].V == intensite) tableauV[intensite]++;
                        if (image[x, y].B == intensite) tableauB[intensite]++;
                    }
                }
            }

            //On calcule le max des 3 tableaux pour définir la hauteur de l'histogramme
            int max = CalculerMax(tableauR);
            int maxV = CalculerMax(tableauV);
            int maxB = CalculerMax(tableauB);
            if (max < maxV) max = maxV;
            if (max < maxB) max = maxB;


            for (int y = 0; y < largeurHisto; y++)
            {
                byte intensite = (byte)(y * 256 / largeurHisto);
                int occurrenceR = tableauR[intensite] * hauteurHisto / max;
                int occurrenceV = tableauV[intensite] * hauteurHisto / max;
                int occurrenceB = tableauB[intensite] * hauteurHisto / max;
                for (int x = 0; x < hauteurHisto; x++)
                {
                    byte resteR = 0;
                    byte resteV = 0;
                    byte resteB = 0;
                    if (occurrenceR > 0)
                    {
                        resteR = 255;
                        occurrenceR--;
                    }
                    if (occurrenceV > 0)
                    {
                        resteV = 255;
                        occurrenceV--;
                    }
                    if (occurrenceB > 0)
                    {
                        resteB = 255;
                        occurrenceB--;
                    }

                    Pixel pix = new Pixel(resteR, resteV, resteB);
                    histogramme.image[x, y] = pix;
                }
            }
            return histogramme;
        }

        /// <summary>
        /// Vérifie qu'une instance de la classe est la même qu'une autre instance de la classe
        /// </summary>
        /// <param name="img">Image comparée</param>
        /// <returns>Vérification : true si les images sont identiques, false sinon</returns>
        public bool EstEgale(MonImage img)
        {
            bool egalite = true;
            if (typeImg != img.typeImg) egalite = false;
            if (tailleFichier != img.tailleFichier) egalite = false;
            if (tailleOffset != img.tailleOffset) egalite = false;
            if (nbBits != img.nbBits) egalite = false;
            if (largeur != img.largeur) egalite = false;
            else
            {
                if (hauteur != img.hauteur) egalite = false;
                else
                {
                    for (int x = 0; x < hauteur; x++)
                    {
                        for (int y = 0; y < largeur; y++)
                        {
                            if (!image[x, y].EstEgal(img.image[x, y])) egalite = false;
                        }
                    }
                }
            }
            return egalite;
        }

        /// <summary>
        /// Méthode ToString
        /// </summary>
        /// <returns>Infos sur l'instance de la classe</returns>
        public override string ToString()
        {
            string result = "Type : " + typeImg + ". \n";
            result += (nom == null) ? "Pas de nom\n" : "Nom : " + nom +"\n";
            result += "Taille : " + tailleFichier + " octets. \n";
            result += "Taille de l'offset : " + tailleOffset + " octets. \n";
            result += "largeur : " + largeur + ", hauteur : " + hauteur + ". \n";
            result += "Image codée sur " + nbBits + " bits. \n";
            return result;
        }
        #endregion
    }
}
