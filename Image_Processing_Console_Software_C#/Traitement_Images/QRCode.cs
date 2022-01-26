using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traitement_Images
{
    /// <summary>
    /// Classe de création de QR Code et de décodage
    /// </summary>
    public class QRCode
    {
        #region Champs
        int version; //Version du QR Code (maximum 4 pour le moment)
        int donnees; //Nombre d'octets nécessaires pour coder les données de la phrase
        int erreur;  //Nombre d'octets nécessaires pour coder la correction d'erreurs
        char niveauCorrection; //Niveau de correction d'erreurs du QR Code
        string phrase;
        int taille; //Taille de la phrase
        int bloc;   //Nombre de blocs pour coder la correction d'erreurs
        int dimension;  //Nombre de cellules du QR Code
        string code;    //Conversion binaire des données encodées et de la correction d'erreur
        MonImage imageQR; //Instance de la classe MonImage pour retranscrire le QR Code sur une image Bitmap
        int coeffImg;    //Coefficient d'agrandissement de l'image du QR Code par rapport à la matrice de 0 et de 1
        int[,] matrice;   //Matrice composée de 0 et de 1 traduisant le QR Code
        public static SortedList<int, char> alphaNum; //Liste des caractères en code alphanumérique
        bool codeAlpha;   //Vérifie que la phrase est écrite dans un code alphanumérique
        static string[,] masques; //Liste des masques possibles attribués au niveau de correction d'erreurs
        int masqueCorr;
        int masque;       //Numéro du masque appliqué au QR Code


        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur du QR Code
        /// </summary>
        /// <param name="phrase">Phrase à coder</param>
        /// <param name="niveauCorrection">Niveau de correction d'erreur du QR Code</param>
        public QRCode(string phrase, char niveauCorrection, int masque = -1)
        {
            this.phrase = phrase;
            this.niveauCorrection = niveauCorrection;
            version = 0;
            this.code = "";
            this.masque = masque;
            if (phrase != null) taille = phrase.Length;
            if (taille != 0)
            {
                CreerMasques();
                CreerAlphaNumerique();
                codeAlpha = CodeAlphanum(phrase);
                DeterminerVersion();
            }
            if (version != 0)
            {
                PhraseVersBinaire();
                TerminaisonDonnees();
                EncoderErreurCorrection();
                if (bloc > 1) CodeBlocs();
                //Pour les versions 2 à 6, il faut ajouter 7 '0' à la fin du code
                if (version > 1) code += "0000000";
            }
        }

        /// <summary>
        /// Constructeur du QR Code, récupéré à partir d'une image Bitmap, que l'on souhaite décoder
        /// </summary>
        /// <param name="fichier">Nom du fichier Bitmap dans lequel se situe le QR Code</param>
        public QRCode(string fichier)
        {
            this.version = 0;
            this.imageQR = new MonImage(fichier);
            CreerMasques();
            CreerAlphaNumerique();
            ConvertirMatrice();
            RecupererMasque();
            if (masque != -1)
            {
                RecupererInfos();
                EnleverMotifs();
                RecupererCode();
                CodeDansLordre();
                if (RecupererCorrection()) RecupererPhrase();
                else this.version = 0;
            }
        }
        #endregion

        #region Attributs
        /// <summary>
        /// Données encodées en binaire
        /// </summary>
        public string Code
        {
            get { return this.code; }
        }

        /// <summary>
        /// Phrase ou texte encodé
        /// </summary>
        public string Phrase
        {
            get { return this.phrase;}
        }

        /// <summary>
        /// Indicateur du niveau de correction appliqué au QR Code
        /// </summary>
        public char NiveauCorrection
        {
            get { return this.niveauCorrection; }
        }

        /// <summary>
        /// Version du QR Code
        /// </summary>
        public int Version
        {
            get { return this.version; }
        }

        /// <summary>
        /// Instance de la classe MonImage affichant le QR Code
        /// </summary>
        public MonImage Img
        {
            get { return this.imageQR; }
        }
        #endregion

        #region Methodes codage
        /// <summary>
        /// Détermine la version du QR Code en fonction de la taille de la phrase, du niveau de correction, et des caractères de la phrase
        /// (Alphanumérique ou UTF-8), et initialise les champs correspondant à la version.
        /// Méthode longue utilisant beaucoup de conditions, mais simplifiée par rapport à ce que j'avais fait de base.
        /// </summary>
        public void DeterminerVersion()
        {
            bloc = 1;
            switch (niveauCorrection)
            {
                case 'L':
                    if ((codeAlpha && taille <= 25) || taille <= 17)
                    {
                        version = 1;
                        donnees = 19;
                        erreur = 7;
                    }
                    else if ((codeAlpha && taille <= 47) || taille <= 32)
                    {
                        version = 2;
                        donnees = 34;
                        erreur = 10;
                    }
                    else if ((codeAlpha && taille <= 77) || taille <= 53)
                    {
                        version = 3;
                        donnees = 55;
                        erreur = 15;
                    }
                    else if ((codeAlpha && taille <= 114) || taille <= 78)
                    {
                        version = 4;
                        donnees = 80;
                        erreur = 20;
                    }
                    masqueCorr = 0;
                    break;

                case 'M':
                    if ((codeAlpha && taille <= 20) || taille <= 14)
                    {
                        version = 1;
                        donnees = 16;
                        erreur = 10;
                    }
                    else if ((codeAlpha && taille <= 38) || taille <= 26)
                    {
                        version = 2;
                        donnees = 28;
                        erreur = 16;
                    }
                    else if ((codeAlpha && taille <= 61) || taille <= 42)
                    {
                        version = 3;
                        donnees = 44;
                        erreur = 26;
                    }
                    else if ((codeAlpha && taille <= 90) || taille <= 62)
                    {
                        version = 4;
                        donnees = 64;
                        erreur = 18;
                        bloc = 2;
                    }
                    masqueCorr = 1;
                    break;

                case 'Q':
                    if ((codeAlpha && taille <= 16) || taille <= 11)
                    {
                        version = 1;
                        donnees = 13;
                        erreur = 13;
                    }
                    else if ((codeAlpha && taille <= 29) || taille <= 20)
                    {
                        version = 2;
                        donnees = 22;
                        erreur = 22;
                    }
                    else if ((codeAlpha && taille <= 47) || taille <= 32)
                    {
                        version = 3;
                        donnees = 34;
                        erreur = 18;
                        bloc = 2;
                    }
                    else if ((codeAlpha && taille <= 67) || taille <= 46)
                    {
                        version = 4;
                        donnees = 48;
                        erreur = 26;
                        bloc = 2;
                    }
                    masqueCorr = 2;
                    break;

                case 'H':
                    if ((codeAlpha && taille <= 10) || taille <= 7)
                    {
                        version = 1;
                        donnees = 9;
                        erreur = 17;
                    }
                    else if (codeAlpha && (taille <= 20) || taille <= 14)
                    {
                        version = 2;
                        donnees = 16;
                        erreur = 28;
                    }
                    else if ((codeAlpha && taille <= 35) || taille <= 24)
                    {
                        version = 3;
                        donnees = 26;
                        erreur = 22;
                        bloc = 2;
                    }
                    else if ((codeAlpha && taille <= 67) || taille <= 34)
                    {
                        version = 4;
                        donnees = 36;
                        erreur = 16;
                        bloc = 4;
                    }
                    masqueCorr = 3;
                    break;
            }
            dimension = 17 + (4 * version);
            
        }

        /// <summary>
        /// Convertit les données de la phrase dans un langage binaire, selon les caractères utilisés (Alphanumérique ou UTF-8)
        /// </summary>
        public void PhraseVersBinaire()
        {
            if (codeAlpha)
            {
                code += "0010"; //Indicateur du mode alphanumérique
                code += ConvertirVersBinaire(taille, 9);
                for (int i = 0; i < taille; i++)
                {
                    int poids = 0;
                    int puissance = 1;
                    for (int j = i + 1; j >= i && j < taille; j--)
                    //On code les lettres 2 par 2, chaque lettre ayant un poids de 0 à 44 (45 caracteres en alphanumérique)
                    {
                        poids += alphaNum.IndexOfValue(phrase[j]) * puissance;
                        puissance *= alphaNum.Count;
                    }
                    i++;
                    if (i != taille) code += ConvertirVersBinaire(poids, 11);
                    //Les lettres sont codées 2 par 2 sur 11 bits
                    else code += ConvertirVersBinaire(alphaNum.IndexOfValue(phrase[taille - 1]), 6);
                    //Si le nombre de lettres est impair, la dernière lettre est codée sur 6 bits
                }
            }
            else
            {
                code += "0100"; //Indicateur de mode UTF-8
                code += ConvertirVersBinaire(taille, 8);
                //Chaque lettre est codée une par une sur 8 bits, ce qui simplifie la tâche pour un encodage en UTF-8
                for (int i = 0; i < taille; i++) code += ConvertirVersBinaire(phrase[i]);
            }
        }

        /// <summary>
        /// Ajoute les terminaisons nécessaires à la fin du code
        /// </summary>
        public void TerminaisonDonnees()
        {
            int terminaison = (donnees * 8) - code.Length;
            if (terminaison > 4) terminaison = 4; //4 "0" maximum supplémentaires
            for (int i = 0; i < terminaison; i++) code += '0';
            while (code.Length % 8 != 0) code += '0'; //Pour faire un multiple de 8
            int restant = (donnees - (code.Length / 8));
            for (int i = 0; i < restant; i++)
            //Pour les bits restants, on ajoute les octets 236 et 17 à la suite
            {
                code += ConvertirVersBinaire(236);
                if (i++ < restant - 1) code += ConvertirVersBinaire(17);
                //Autant utiliser la méthode de convertion puisqu'elle existe
            }
        }

        /// <summary>
        /// Encode la correction de l'erreur à l'aide du code de Reed Solomon fourni 
        /// J'ai enlevé la ligne de l'erreur associée à la condition ligne 94 du fichier ReedSolomonEncoder.cs qui posait problème lorsque le
        /// nombre de données encodées étaient inférieur au nombre d'octets requis pour la correction d'erreur
        /// </summary>
        public void EncoderErreurCorrection()
        {
            for (int k = 0; k < bloc; k++)
            //Première boucle de paramètre k, pour coder le message sur plusieurs blocs si nécessaires (selon les conventions)
            {
                byte[] message = new byte[donnees / bloc];
                for (int i = 0; i < donnees / bloc; i++)
                {
                    string binaire = "";
                    int separation = (k * 8 * donnees / bloc);
                    //separation correspond à l'indice auquel on s'est arrêtés au bloc précédent
                    while (separation % 8 != 0) separation++;
                    for (int j = 8 * i; j < 8 * (i + 1); j++)
                    {
                        binaire += code[j + separation];
                    }
                    //On convertit le code binaire en une sequence d'octets
                    message[i] = (byte)ConvertirVersEntier(binaire);
                }
                //On code la correction d'erreur à l'aide de l'algorithme de Reed Solomon (3e paramètre : ErrorCorrectionCodeType.QRCode = 0)
                byte[] erreurCode = ReedSolomonAlgorithm.Encode(message, erreur, ErrorCorrectionCodeType.QRCode);
                for (int i = 0; i < erreur; i++) code += ConvertirVersBinaire(erreurCode[i]);
            }
        }

        /// <summary>
        /// Réecriture du code en le séparant en plusieurs blocs pour les versions où c'est nécessaire de le faire
        /// 
        /// Exemple : en supposant que les octets 0123456789ABCDEF doivent être séparés en 2 blocs
        /// avec 0-->9 les données et ABC la 1ere correction d'erreur, DEF la 2e correction d'erreur
        /// 
        /// On veut arriver au résultat suivant :
        /// 0516273849ADBECF
        /// 
        /// Pour une division de 123456789ABCDEF en 3 blocs, on veut arriver à
        /// 147258369ACEBDF
        /// </summary>
        public void CodeBlocs()
        {
            string codeTemporaire = ""; //On va tout mettre dans une variable temporaire
            for (int i = 0; i < donnees / bloc; i++)
            //On sépare dans un premier temps les données par blocs
            {
                for (int k = 0; k < bloc; k++)
                //On sépare les octets en plusieurs blocs
                {
                    for (int j = i * 8; j < 8 * (i + 1); j++)
                    {
                        codeTemporaire += code[j + (k * 8 * donnees / bloc)];
                        //Et on réecrit 8 bits par 8 bits le code dans l'ordre souhaité
                    }
                }
            }
            for (int i = donnees; i < donnees + erreur; i++)
            //On fait de même pour la correction d'erreurs
            {
                for (int k = 0; k < bloc; k++)
                {
                    for (int j = i * 8; j < 8 * (i + 1); j++)
                    {
                        codeTemporaire += code[j + (k * 8 * erreur)];
                    }
                }
            }
            //Ensuite, on replace le code dans sa variable d'instance
            code = codeTemporaire;
        }

        /// <summary>
        /// Retranscrit les données codées sur une matrice, puis sur une instance de la classe MonImage pour l'enregistrer dans une image bitmap
        /// </summary>
        /// <param name="fichier">Nom du fichier dans laquelle on souhaite enregistrer l'image du QR Code</param>
        public void ImageQR()
        {
            coeffImg = 8;
            if (masque == -1) DeterminerMasque();
            InitialiserQR();
            RemplirDonnees();
            ConvertirImage();
        }

        /// <summary>
        /// Place dans la matrice les éléments (motifs, lignes) du QR Code sans prendre en compte les données encodées dans un premier temps
        /// </summary>
        public void InitialiserQR()
        {
            matrice = new int[dimension, dimension];
            for (int i = 0; i < dimension; i++)
                for (int j = 0; j < dimension; j++)
                    matrice[i, j] = 100; //100 pour afficher un pixel gris pour visualiser les problèmes lors de mes tests

            for (int i = 0; i < 5; i++)
            {
                //Positionnement des motifs de synchronisation
                int pos1 = i - 1;
                int pos2 = dimension - 8 + i;
                int taille = 9 - (2 * i);
                int couleur = i % 2;
                if (i == 4)
                {
                    taille = 1;
                    couleur = 1 - couleur;
                }
                //Petite astuce avec les indices et la boucle pour ne pas avoir à appeler 12x la fonction Carre() à l'écrit
                CreerCarre(pos1, pos1, taille, couleur);
                CreerCarre(pos2, pos1, taille, couleur);
                CreerCarre(pos1, pos2, taille, couleur);
                if (version > 1) CreerCarre(pos2 - 1, pos2 - 1, taille - 4, 1 - couleur); 
                //Petit carré en bas à droite pour les versions supérieures à 1
            }

            #region Ancien code
            //CreerCarre(-1, -1, 9, 0);
            //CreerCarre(0, 0, 7, 1);
            //CreerCarre(1, 1, 5, 0);
            //CreerCarre(2, 2, 3, 1);
            //matrice[3, 3] = 1;

            //CreerCarre(dimension - 8, -1, 9, 0);
            //CreerCarre(dimension - 7, 0, 7, 1);
            //CreerCarre(dimension - 6, 1, 5, 0);
            //CreerCarre(dimension - 5, 2, 3, 1);
            //matrice[dimension - 4, 3] = 1;

            //CreerCarre(-1, dimension - 8, 9, 0);
            //CreerCarre(0, dimension - 7, 7, 1);
            //CreerCarre(1, dimension - 6, 5, 0);
            //CreerCarre(2, dimension - 5, 3, 1);
            //matrice[3, dimension - 4] = 1;

            //if (version > 1)
            //{
            //    CreerCarre(dimension - 9, dimension - 9, 5, 1);
            //    CreerCarre(dimension - 8, dimension - 8, 3, 0);
            //    matrice[dimension - 7, dimension - 7] = 1;
            //}
            #endregion

            //Lignes de synchronisation
            for (int i = 8; i < dimension - 8; i ++)
            {
                matrice[6, i] = 1 - i%2;
                matrice[i, 6] = 1 - i%2;
            }

            ///Placement du masque
            for (int i = 0; i <= 7; i++)
            {
                matrice[dimension - i - 1, 8] = masques[masqueCorr, masque][i] - '0';
                matrice[8, dimension - i - 1] = masques[masqueCorr, masque][14 - i] - '0';
                int j = i;
                if (j >= 6) j++;
                matrice[8, j] = masques[masqueCorr, masque][i] - '0';
                matrice[j, 8] = masques[masqueCorr, masque][14 - i] - '0';
            }

            //Placement du module noir
            matrice[4 * version + 9, 8] = 1;
        }

        /// <summary>
        /// Initialise une matrice contenant les 32 masques possibles pour un QR Code (8 pour chaque niveau de correction
        /// </summary>
        public static void CreerMasques()
        {
            masques = new string[4, 8];
            masques[0,0] = "111011111000100"; masques[1, 0] = "101010000010010"; masques[2, 0] = "011010101011111"; masques[3, 0] = "001011010001001";
            masques[0,1] = "111001011110011"; masques[1, 1] = "101000100100101"; masques[2, 1] = "011000001101000"; masques[3, 1] = "001001110111110";
            masques[0,2] = "111110110101010"; masques[1, 2] = "101111001111100"; masques[2, 2] = "011111100110001"; masques[3, 2] = "001110011100111";
            masques[0,3] = "111100010011101"; masques[1, 3] = "101101101001011"; masques[2, 3] = "011101000000110"; masques[3, 3] = "001100111010000";
            masques[0,4] = "110011000101111"; masques[1, 4] = "100010111111001"; masques[2, 4] = "010010010110100"; masques[3, 4] = "000011101100010";
            masques[0,5] = "110001100011000"; masques[1, 5] = "100000011001110"; masques[2, 5] = "010000110000011"; masques[3, 5] = "000001001010101";
            masques[0,6] = "110110001000001"; masques[1, 6] = "100111110010111"; masques[2, 6] = "010111011011010"; masques[3, 6] = "000110100001100";
            masques[0,7] = "110100101110110"; masques[1, 7] = "100101010100000"; masques[2, 7] = "010101111101101"; masques[3, 7] = "000100000111011";
        }

        /// <summary>
        /// Place un 0 ou un 1 à la cellule en [x,y] de la matrice, en fonction du masque à appliquer, et de x et de y
        /// </summary>
        /// <param name="x">Ligne de la matrice</param>
        /// <param name="y">Colonne de la matrice</param>
        /// <param name="pix">Pixel à placer, 0 ou 1</param>
        /// <returns>0 ou 1 selon l'application du masque</returns>
        public int AppliquerMasque(int x, int y, int pix)
        {
            switch (masque)
            {
                case 0:
                    if ((x + y) % 2 == 0) pix = 1 - pix;
                    break;

                case 1:
                    if (x % 2 == 0) pix = 1 - pix;
                    break;

                case 2:
                    if (y % 3 == 0) pix = 1 - pix;
                    break;

                case 3:
                    if ((x + y) % 3 == 0) pix = 1 - pix;
                    break;

                case 4:
                    if (((x / 2) + (y / 3)) % 2 == 0) pix = 1 - pix;
                    break;

                case 5:
                    if (((x * y ) % 2) + ((x * y) % 3) == 0) pix = 1 - pix;
                    break;

                case 6:
                    if ((((x * y) % 3) + (x * y)) % 2 == 0) pix = 1 - pix;
                    break;

                case 7:
                    if ((((x * y) % 3) + (x + y)) % 2 == 0) pix = 1 - pix;
                    break;
            }
            return pix;
        }

        /// <summary>
        /// Détermine le meilleur masque à appliquer, selon certains critères. On compte un certain nombre de "pénalités", et
        /// le masque qui donne le moins de pénalités est le "meilleur" à appliquer
        /// </summary>
        public void DeterminerMasque()
        {
            int[] penalites = new int[8];
            for (int m = 0; m < 8; m++)
            {
                this.masque = m;
                //Pour chaque masque, on crée un QR Code
                InitialiserQR();
                RemplirDonnees();
                //Puis on compte un nombre de "pénalités" qui détermineront lequel des masques est le meilleur
                penalites[m] = CompterPenalite1() + CompterPenalite2() + CompterPenalite3() + CompterPenalite4();
            }
            masque = 0;
            for (int i = 1; i < 8; i++)
            {
                //On prend le masque qui pose le moins de pénalités
                if (penalites[i] < penalites[masque]) masque = i;
            }
        }

        /// <summary>
        /// Premières pénalités à compter
        /// Chaque fois qu'il y a plus de 5 cases pareilles, on compte des pénalités
        /// </summary>
        /// <returns>Compteur de pénalités</returns>
        public int CompterPenalite1()
        {
            int penalite = 0;
            for (int x = 0; x < dimension; x++)
            {
                int compteurH = 1;
                int compteurV = 1;
                for (int y = 1; y < dimension; y++)
                {
                    if (matrice[x, y] != matrice[x, y - 1]) compteurH = 0;
                    if (matrice[y, x] != matrice[y - 1, x]) compteurV = 0;
                    compteurH++;
                    compteurV++;
                    if (compteurH == 5) penalite += 3;
                    if (compteurV == 5) penalite += 3;
                    if (compteurH > 5) penalite++;
                    if (compteurV > 5) penalite++;
                }
            }
            return penalite;
        }

        /// <summary>
        /// Deuxièmes pénalités à compter
        /// Chaque fois que l'on constate un carré de 4 pixels identiques, on ajoute 3 pénalités
        /// </summary>
        /// <returns>Compteur de pénalités</returns>
        public int CompterPenalite2()
        {
            int penalite = 0;
            for (int x = 0; x < dimension - 1; x++)
            {
                for (int y = 0; y < dimension - 1; y++)
                {
                    if (matrice[x, y] == matrice[x, y + 1] && matrice[x, y] == matrice[x + 1, y] && matrice[x, y] == matrice[x + 1, y + 1]) penalite += 3;
                }
            }
            return penalite;
        }

        /// <summary>
        /// Troisièmes pénalités à compter
        /// Chaque fois que l'on observe une suite "10111010000", peu importe le sens et l'orientation,
        /// c'est 40 pénalités que l'on ajoute
        /// </summary>
        /// <returns>Compteur de pénalités</returns>
        public int CompterPenalite3()
        {
            int penalite = 0;
            string test = "10111010000";
            for (int x = 0; x < dimension; x++)
            {
                for (int y = 0; y < dimension - test.Length; y++)
                {
                    string s1 = "";
                    string s2 = "";
                    string s3 = "";
                    string s4 = "";
                    for (int k = y; k < y + test.Length; k++)
                    {
                        s1 += matrice[x, k];
                        s2 += matrice[k, x];
                        s3 += matrice[x, dimension - 1 - k];
                        s4 += matrice[dimension - 1 - k, x];
                    }
                    if (s1 == test) penalite += 40;
                    if (s2 == test) penalite += 40;
                    if (s3 == test) penalite += 40;
                    if (s4 == test) penalite += 40;
                }
            }
            return penalite;
        }

        /// <summary>
        /// Quatrièmes pénalités à appliquer, selon la quantité de pixels blancs par rapport aux noirs
        /// </summary>
        /// <returns>Compteur de pénalités</returns>
        public int CompterPenalite4()
        {
            int penalite = 0;
            int compteur = 0;
            foreach (int element in matrice) compteur += element;

            int pourcentageInf = 100 * compteur / matrice.Length;
            int pourcentageSup = pourcentageInf;
            while (pourcentageInf % 5 != 0) pourcentageInf--;
            while (pourcentageSup % 5 != 0) pourcentageSup++;
            double differenceInf = pourcentageInf - 50;
            if (differenceInf < 0) differenceInf *= -1.0;
            double differenceSup = pourcentageSup - 50;
            if (differenceSup < 0) differenceSup *= -1.0;

            differenceInf /= 5.0;
            differenceSup /= 5.0;

            double min = differenceInf;
            if (differenceInf > differenceSup) min = differenceSup;

            penalite = (int)(min * 10);

            return penalite;
        }

        /// <summary>
        /// Créer un carré de 0 ou de 1 selon la position entrée en paramètre, la taille, et la "couleur", 0 ou 1
        /// </summary>
        /// <param name="posX">Position de la ligne</param>
        /// <param name="posY">Position de la colonne</param>
        /// <param name="tailleCarre">Taille du carré</param>
        /// <param name="couleur">0 ou 1</param>
        public void CreerCarre(int posX, int posY, int tailleCarre, int couleur)
        {
            for (int x = posX; x < dimension; x++)
            {
                for (int y = posY; y < dimension; y++)
                {
                    try //Pour éviter les erreurs des carrés blancs extérieurs
                    {
                        if (x == posX && y < posY + tailleCarre) matrice[x, y] = couleur;
                        if (x < posX + tailleCarre && y == posY) matrice[x, y] = couleur;
                        if (x == posX + tailleCarre - 1 && y < posY + tailleCarre) matrice[x, y] = couleur;
                        if (x < posX + tailleCarre && y == posY + tailleCarre - 1) matrice[x, y] = couleur;
                    }
                    catch (Exception) { }
                }
            }
        }

        /// <summary>
        /// Rempli la matrice des données restantes à mettre dans la matrice du QR Code, comprenant les données codées et le masque
        /// </summary>
        public void RemplirDonnees()
        {
            int i = 0; //Indice pour les caractères du code
            for (int y = dimension - 1; y > 0; y -= 2)
            {
                if (y == 6) y--; //On évite les motifs d'alignement
                for (int x = dimension - 1; x >= 0; x--)
                {
                    //On vérifie que chaque pixel n'est pas utilisé, puis on applique le masque
                    if (matrice[x, y] == 100) matrice[x, y] = AppliquerMasque(x, y, code[i++] - '0');
                    if (matrice[x, y - 1] == 100) matrice[x, y - 1] = AppliquerMasque(x, y - 1, code[i++] - '0');
                }
                y -= 2; //On passe aux deux colonnes précédentes
                if (y == 6) y--; //On évite les motifs d'alignement
                for (int x = 0; x < dimension; x++)
                {
                    if (matrice[x, y] == 100) matrice[x, y] = AppliquerMasque(x, y, code[i++] - '0');
                    if (matrice[x, y - 1] == 100) matrice[x, y - 1] = AppliquerMasque(x, y - 1, code[i++] - '0');
                }
            }
        }

        /// <summary>
        /// Convertit la matrice dans une image, instance de la classe MonImage
        /// </summary>
        public void ConvertirImage()
        {
            //On augmente la taille de l'image pour ne pas avoir une image minuscule
            int dimImg = coeffImg * dimension;
            imageQR = new MonImage(dimImg, dimImg);
            for (int x = 0; x < dimImg; x++)
            {
                for (int y = 0; y < dimImg; y++)
                {
                    //Puis on détermine la couleur en fonction de la valeur de chaque case
                    //J'ai décidé que 0 = blanc et 1 = noir comme dans le sujet, même si j'aurai pu faire l'inverse
                    //Dans ce cas là il faudrait tout changer
                    byte couleur = (byte)(255 - 255 * matrice[x / coeffImg, y / coeffImg]);
                    Pixel pix = new Pixel(couleur, couleur, couleur);
                    imageQR.Image[dimImg - x - 1, y] = pix;
                    //L'image se lit du bas vers le haut contrairement aux matrices
                }
            }
        }
        #endregion

        #region Methodes decodage
        /// <summary>
        /// Convertit l'image noire et blanc en matrice de 0 et de 1
        /// </summary>
        public void ConvertirMatrice()
        {
            //On détermine dans un premier temps le coefficient d'agrandissement de l'image
            int j = 0;
            for (j = 0; imageQR.Image[0, j].Somme() == 0; j++) { }
            coeffImg = j / 7;
            dimension = imageQR.Hauteur / coeffImg;
            //Ensuite, chaque pixel noir vaut 1, les autres valent 0
            matrice = new int[dimension, dimension];
            Pixel noir = new Pixel(0, 0, 0);
            for (int x = 0; x < dimension; x++)
                for (int y = 0; y < dimension; y++)
                    matrice[dimension - 1 - x, y] = (noir.EstEgal(imageQR.Image[x * coeffImg, y * coeffImg]) ? 1 : 0);
        }

        /// <summary>
        /// Récupère la valeur du masque appliqué au QR Code contenant notamment des infos sur le niveau de correction
        /// </summary>
        public void RecupererMasque()
        {
            //On récupère le masque qui détient des informations essentielles
            string m1 = "";
            string m2 = "" + matrice[8, dimension - 8];
            string m3 = "";
            string m4 = "";
            //Le masque est autour des motifs de synchronisation, à certains emplacement
            for (int i = 0; i <= 6; i++)
            {
                m1 += matrice[dimension - i - 1, 8];
                m2 += matrice[8, dimension + i - 7];
                int j = i;
                if (j >= 6) j++;
                m3 += matrice[8, j];
                m4 = matrice[j, 8] + m4;
            }
            m4 = matrice[8, 8] + m4;
            //On vérifie toutes les combinaisons possibles du masque, au cas où certaines cases seraient illisibles
            string[] masqueCode = { m1 + m2, m3 + m4, m1 + m4, m3 + m2 };
            for (int m = 0; m < 4 && masque != -1; m++)
            {
                masqueCorr = 3 - ConvertirVersEntier("" + masqueCode[m][0] + masqueCode[m][1]);
                masque = -1;
                for (int i = 0; i < 8; i++)
                {
                    //On vérifie ensuite de quel masque il s'agit parmi tous les masques
                    if (masqueCode[m] == masques[masqueCorr, i]) masque = i;
                }
            }
        }

        /// <summary>
        /// Récupère les informations utiles au décodage du QR Code, telles que la version, et les données d'encodage
        /// </summary>
        public void RecupererInfos()
        {
            //La version se détermine grâce à la dimension
            version = (dimension - 17) / 4;
            bloc = 1; //Bloc pour les QR Code séparés en plusieurs blocs
            //On détermine ensuite toutes les autres infos selon la version et le niveau de correction
            switch (masqueCorr)
            {
                case 0:
                    niveauCorrection = 'L';
                    switch (version)
                    {
                        case 1:
                            donnees = 19;
                            erreur = 7;
                            break;

                        case 2:
                            donnees = 34;
                            erreur = 10;
                            break;

                        case 3:
                            donnees = 55;
                            erreur = 15;
                            break;

                        case 4:
                            donnees = 80;
                            erreur = 20;
                            break;
                    }
                    break;

                case 1:
                    niveauCorrection = 'M';
                    switch (version)
                    {
                        case 1:
                            donnees = 16;
                            erreur = 10;
                            break;

                        case 2:
                            donnees = 28;
                            erreur = 16;
                            break;

                        case 3:
                            donnees = 44;
                            erreur = 26;
                            break;

                        case 4:
                            donnees = 64;
                            erreur = 18;
                            bloc = 2;
                            break;
                    }
                    break;

                case 2:
                    niveauCorrection = 'Q';
                    switch (version)
                    {
                        case 1:
                            donnees = 13;
                            erreur = 13;
                            break;

                        case 2:
                            donnees = 22;
                            erreur = 22;
                            break;

                        case 3:
                            donnees = 34;
                            erreur = 18;
                            bloc = 2;
                            break;

                        case 4:
                            donnees = 48;
                            erreur = 26;
                            bloc = 2;
                            break;
                    }
                    break;

                case 3:
                    niveauCorrection = 'H';
                    switch (version)
                    {
                        case 1:
                            donnees = 9;
                            erreur = 17;
                            break;

                        case 2:
                            donnees = 16;
                            erreur = 28;
                            break;

                        case 3:
                            donnees = 26;
                            erreur = 22;
                            bloc = 2;
                            break;

                        case 4:
                            donnees = 36;
                            erreur = 16;
                            bloc = 4;
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Remplace les champs dans un rectangle de la matrice de 0 et de 1, par une autre valeur (100)
        /// Permettra par la suite de récupérer uniquement les valeurs concernant les données de la phrase
        /// </summary>
        /// <param name="posX">Ligne du début du rectangle</param>
        /// <param name="posY">Colonne du début du rectangle</param>
        /// <param name="hauteur">Hauteur du rectangle</param>
        /// <param name="largeur">Largeur du rectangle</param>
        public void RemplacerRectangle(int posX, int posY, int hauteur, int largeur)
        {
            for (int x = posX; x < posX + hauteur; x++)
            {
                for (int y = posY; y < posY + largeur; y++)
                {
                    //On donne une valeur autre que 0 ou 1 à toutes les cases d'un rectangle
                    //Choix de 100 pour afficher les pixels plutôt gris que noir ou blanc
                    matrice[x, y] = 100;
                }
            }
        }

        /// <summary>
        /// Enlève tout élément binaire de la matrice ne concernant pas l'encodage des données, à savoir les motifs et 
        /// les éléments décrivant le masque
        /// </summary>
        public void EnleverMotifs()
        {
            //On remplace tout ce qui ne contient pas le message encodé, par 100
            RemplacerRectangle(0, 0, 9, 9);
            RemplacerRectangle(0, dimension - 8, 9, 8);
            RemplacerRectangle(dimension - 8, 0, 8, 9);
            if (version > 1) RemplacerRectangle(dimension - 9, dimension - 9, 5, 5);
            //Et les motifs d'alignement
            for (int i = 9; i < dimension - 8; i++)
            {
                matrice[i, 6] = 100;
                matrice[6, i] = 100;
            }
        }

        /// <summary>
        /// Récupère tout le code placé dans le QR Code
        /// </summary>
        public void RecupererCode()
        {
            code = "";
            for (int y = dimension - 1; y > 0; y -= 2)
            {
                if (y == 6) y--; //On évite les modules d'alignement
                for (int x = dimension - 1; x >= 0; x--)
                {
                    //Et si la case nous intéresse, on fait l'application du masque pour récupérer le code
                    if (matrice[x, y] != 100) code += AppliquerMasque(x, y, matrice[x,y]);
                    if (matrice[x, y - 1] != 100) code += AppliquerMasque(x, y - 1, matrice[x, y - 1]);
                }
                y -= 2; //On passe aux 2 colonnes précedentes entre chaque boucle
                if (y == 6) y--;
                for (int x = 0; x < dimension; x++)
                {
                    if (matrice[x, y] != 100) code += AppliquerMasque(x, y, matrice[x, y]);
                    if (matrice[x, y - 1] != 100) code += AppliquerMasque(x, y - 1, matrice[x, y - 1]);
                }
            }
        }

        /// <summary>
        /// Pour certaines versions du QR Code, selon le niveau de correction, le code peut être divisé en plusieurs blocs.
        /// Cette méthode réécrit le code en plaçant les blocs dans l'ordre
        /// </summary>
        public void CodeDansLordre()
        {
            //On va tout mettre dans une variable temporelle
            string codeTemporaire = "";
            for (int k = 0; k < bloc; k++)
            //On parcourt le code "bloc" par "bloc"
            {
                for (int i = 0; i < donnees; i += bloc)
                {
                    for (int j = (k + i) * 8; j < 8 * (k + i + 1); j++)
                    {
                        codeTemporaire += code[j];
                        //Et on ajoute octet par octet (sur 8 bits)
                    }
                }
            }
            //Tout pareil pour la correction d'erreurs
            for (int k = 0; k < bloc; k++)
            {
                for (int i = donnees; i < donnees + (bloc * erreur); i += bloc)
                {
                    for (int j = (k + i) * 8; j < 8 * (k + i + 1); j++)
                    {
                        codeTemporaire += code[j];
                    }
                }
            }
            code = codeTemporaire;
        }

        /// <summary>
        /// Récupère les données de correction d'erreur du code, et récupère le message complet, à l'aide du décodeur
        /// de l'algorithme de Reed Solomon
        /// </summary>
        /// <returns>Vérifie que tout s'est bien effectué</returns>
        public bool RecupererCorrection()
        {
            bool reussi = false; //Vérifiera que tout a réussi
            byte[] message = null;
            string codeTemporaire = "";
            for (int k = 0; k < bloc; k++)
            //Décodage du message en plusieurs fois selon le nombre de blocs
            { 
                message = new byte[donnees / bloc];
                //Le code est divisé en plusieurs messages selon le nombre de blocs
                for (int i = 0; i < donnees / bloc; i++)
                {
                    //On stocke dans chaque case du tableau de bytes les données octet par octet
                    string mess = "";
                    for (int j = 8 * i; j < 8 * (i + 1); j++)
                    {
                        mess += code[j + (8 * k * donnees / bloc)];
                    }
                    message[i] = (byte)ConvertirVersEntier(mess);
                }

                //On fait les mêmes étapes pour les octets de la correction d'erreurs
                byte[] erreurCode = new byte[erreur];
                for (int i = donnees; i < donnees + erreur; i++)
                {
                    string err = "";
                    for (int j = 8 * i; j < 8 * (i + 1); j++)
                    {
                        err += code[j + (8 * k * erreur)];
                    }
                    erreurCode[i - donnees] = (byte)ConvertirVersEntier(err);
                }

                //On décode ensuite le message avec l'algorithme de Reed Solomon
                message = ReedSolomonAlgorithm.Decode(message, erreurCode, ErrorCorrectionCodeType.QRCode);
                if (message != null && k < bloc - 1)
                    //Si le message n'est pas null, on l'ajoute au code temporaire
                    for (int i = (donnees / bloc) - 1, j = 0; i >= 0; i--) 
                        codeTemporaire += ConvertirVersBinaire(message[j++]);
            }
            if (message != null)
            {
                for (int i = (donnees / bloc) - 1, j = 0; i >= 0; i--)
                {
                    codeTemporaire += ConvertirVersBinaire(message[j++]);
                }
                code = codeTemporaire;
                reussi = true;
            }
            return reussi;
        }

        /// <summary>
        /// Récupère la phrase codée
        /// </summary>
        public void RecupererPhrase()
        {
            phrase = "";
            //Le mode est indiqué sur les 4 premiers bits du code, 0010 pour alphanumérique et 0100 pour
            codeAlpha = "" + code[0] + code[1] + code[2] + code[3] == "0010";

            //La taille est indiquée sur les 8 ou 9 bits suivants selon le mode
            string tailleBinaire = "";
            int debut = 4 + 8 + Convert.ToInt32(codeAlpha);
            for (int i = 4; i < debut; i++) tailleBinaire += code[i];
            taille = ConvertirVersEntier(tailleBinaire);

            if (codeAlpha)
            {
                for (int i = 0; i < (taille % 2) + (taille / 2); i++)
                //En alphanumérique on récupère les lettres 2 par 2 sur 11 bits
                {
                    string caracteres = "";
                    for (int j = 11 * i; j < 11 * (i + 1) && i < taille / 2; j++) caracteres += code[j + debut];
                    if (caracteres != "")
                    {
                        int c = ConvertirVersEntier(caracteres);
                        phrase += alphaNum[c / alphaNum.Count];
                        phrase += alphaNum[c % alphaNum.Count];
                    }
                    else if (taille % 2 == 1)
                    {
                        for (int j = 11 * i; j < (11 * i) + 6; j++) caracteres += code[j + debut];
                        phrase += alphaNum[ConvertirVersEntier(caracteres)];
                    }
                }
            }
            else
            {
                for (int i = 0; i < taille; i++)
                //En UTF-8 on récupère les caractères 8 bits par 8 bits
                {
                    string caractere = "";
                    for (int j = 8 * i; j < 8 * (i + 1); j++) caractere += code[j + debut];
                    phrase += (char)ConvertirVersEntier(caractere);
                }
            }
        }
        #endregion

        #region Autres Methodes
        /// <summary>
        /// Convertit un nombre entier en une chaîne de caractères binaires, de façon récursive
        /// </summary>
        /// <param name="nombre">Nombre à traduire en base 2</param>
        /// <param name="taille">Taille de la chaîne de caractères, par défaut 8</param>
        /// <param name="binaire">Paramètre utile pour rappeler la fonction récursive</param>
        /// <returns>Chaîne de caractère traduisant le nombre dans un langage binaire (base 2)</returns>
        public static string ConvertirVersBinaire(int nombre, int taille = 8, string binaire = "")
        {
            if (nombre > 0)
            {
                binaire = nombre % 2 + binaire;
                return ConvertirVersBinaire(nombre / 2, taille, binaire);
            }
            if (binaire.Length < taille)
            {
                binaire = '0' + binaire;
                return ConvertirVersBinaire(0, taille, binaire);
            }
            return binaire;
        }

        /// <summary>
        /// Convertit une chaîne de caractères binaire en un nombre entier, de façon récursive
        /// </summary>
        /// <param name="binaire">Chaîne de caractère traduisant un nombre dans un langage binaire (base 2)</param>
        /// <param name="entier">Nombre traduit, paramètre utile pour rappeler la fonction récursive</param>
        /// <param name="i">Numéro du caractère étudié sur la chaîne, paramètre utile pour rappeler la fonction récursive</param>
        /// <param name="puissance">Puissance de 2, paramètre utile pour rappeler la fonction récursive</param>
        /// <returns>Entier en base décimale, traduit de la base binaire</returns>
        public static int ConvertirVersEntier(string binaire, int entier = 0, int i = 0, int puissance = 1)
        {
            int taille = binaire.Length - 1;
            if (i <= taille)
            {
                entier += puissance * (binaire[taille - i] - '0');
                return ConvertirVersEntier(binaire, entier, i + 1, 2 * puissance);
            }
            return entier;
        }

        /// <summary>
        /// Initialise les caractères alphanumériques dans une SortedList (Pas trouvé d'autres moyens d'obtenir cette iste)
        /// </summary>
        public static void CreerAlphaNumerique()
        {
            alphaNum = new SortedList<int, char>();
            string alphaN = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:";
            for (int i = 0; i < alphaN.Length; i++) alphaNum.Add(i, alphaN[i]);

            #region Ancien code
            //string speciaux = " $%*+-./:";
            //for (int i = 0; i < 10; i++) alphaNum.Add(i, (char)('0' + i));
            //for (int i = 0; i < 26; i++) alphaNum.Add(i + 10, (char)(65 + i));
            //for (int i = 0; i < speciaux.Length; i++) alphaNum.Add(i + 36, speciaux[i]);
            #endregion
        }

        /// <summary>
        /// Vérifie si la phrase à coder est écrite en caractères alphanumériques
        /// </summary>
        public static bool CodeAlphanum(string phrase)
        {
            bool verif = true;
            for (int i = 0; i < phrase.Length; i++)
            {
                if (alphaNum.IndexOfValue(phrase[i]) < 0) verif = false;
            }
            return verif;
        }

        /// <summary>
        /// Méthode ToString
        /// </summary>
        /// <returns>Infos sur l'instance de la classe</returns>
        public override string ToString()
        {
            string result = "Version de QR Code : " + version + "\n";
            result += "Phrase codée : " + phrase + "\n";
            result += "Caractères : " + (codeAlpha ? "Alphanumérique" : "UTF-8") + "\n";
            result += "Taille : " + dimension + "x" + dimension + "\n";
            result += "Niveau de correction : " + niveauCorrection + "\n";
            result += "Masque appliqué : " + masque + "\n";
            return result;
        }
        #endregion
    }
}
