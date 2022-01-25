using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mixmo
{
    /// <summary>
    /// Classe d'ensemble de lettres définie par une liste d'objets de la classe Lettre
    /// </summary>
    public class Lettres
    {
        #region Champs
        string nom;
        int quantiteTotale ;
        List<Lettre> lettres = new List<Lettre>();
        #endregion

        #region Constructeurs
        /// <summary>
        /// Constructeur de la pioche à partir d'un fichier texte
        /// </summary>
        /// <param name="nom">Nom de la liste de lettres</param>
        /// <param name="fichier">Fichier pour initialiser la liste des lettres</param>
        public Lettres(string nom, string fichier)
        {
            this.nom = nom;
            CreerPioche(fichier);
        }

        /// <summary>
        /// Constructeur simple
        /// </summary>
        /// <param name="nom">Nom de la liste de lettres</param>
        public Lettres (string nom)
        {
            this.nom = nom;
        }

        /// <summary>
        /// Constructeur par copie
        /// </summary>
        /// <param name="l">Liste de lettres à copier</param>
        public Lettres (Lettres l)
        {
            this.nom = l.nom;
            this.quantiteTotale = l.quantiteTotale;
            for (int i = 0; i < l.lettres.Count; i++)
            //Les lettres de la liste doivent copier les valeurs, et pas se situer à la même adresse
            {
                Lettre a = new Lettre(l.lettres[i]);
                this.lettres.Add(a);
            }
        }
        #endregion

        #region Attributs
        /// <summary>
        /// Attribut du nom de la liste des lettres en lecture seule
        /// </summary>
        public string Nom
        {
            get { return nom; }
        }

        /// <summary>
        /// Attribut de la liste des lettres en lecture seule
        /// </summary>
        public List<Lettre> ListeDeLettres
        {
            get { return this.lettres; }
        }
        #endregion

        #region Methodes
        /// <summary>
        /// Initialise la liste pour la pioche à partir d'un fichier texte
        /// </summary>
        /// <param name="fichier">Fichier texte où se trouvent les lettres et leurs attributs</param>
        public void CreerPioche(string fichier)
        {
            StreamReader sReader = null;
            try
            {
                sReader = new StreamReader(fichier);
                string ligne;
                while ((ligne = sReader.ReadLine()) != null)
                {
                    //On met les valeurs sur chaque ligne comme attribut de chaque lettres
                    string[] alphabet = ligne.Split(',');
                    char symbole = Convert.ToChar(alphabet[0]);
                    int quantite = Convert.ToInt32(alphabet[1]);
                    int poids = Convert.ToInt32(alphabet[2]);
                    if (quantite != 0)
                    {
                        Lettre lettre = new Lettre(symbole, quantite, poids);
                        lettres.Add(lettre);
                    }
                    quantiteTotale += quantite;
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sReader != null) { sReader.Close(); }
            }
        }

        /// <summary>
        /// Attribut de la quantité totale des lettres, avec possibilité d'écriture
        /// </summary>
        public int QuantiteTotale
        {
            get { return quantiteTotale; }
            set { quantiteTotale = value; } //Set utile pour certaines fonctions
        }

        /// <summary>
        /// Détermine l'indice d'une lettre dans la pioche selon la quantité de chaque lettre
        /// </summary>
        /// <param name="nombreLettres">index de la lettre recherchée en prenant en compte la quantité de chaque lettre</param>
        /// <returns>Indice de la lettre piochée sans prendre en compte sa quantité, -1 si hors limites</returns>
        public int IndicePioche(int nombreLettres)
        {
            int indice = -1;
            if (nombreLettres > 0 && nombreLettres <= quantiteTotale)
            {
                for (int indiceLettre = 0; indiceLettre < nombreLettres; indiceLettre++)
                //indiceLettre = indice de la lettre dans la liste de lettres de la pioche
                {
                    indice++;
                    for (int indiceQuantite = 1; indiceQuantite < lettres[indiceLettre].Quantite; indiceQuantite++)
                    //indiceQuantite = indice de la quantité de la lettre en question
                    {
                        nombreLettres--;
                    }
                }
            }
            return indice;
        }

        /// <summary>
        /// Pioche une lettre à un indice donné de la pioche, et retire cette lettre de la pioche en prenant en compte la quantité
        /// </summary>
        /// <param name="indicePioche">Indice de la lettre dans la pioche</param>
        /// <returns>Lettre piochée</returns>
        public Lettre piocherLettre(int indicePioche)
        {
            char symbole = lettres[indicePioche].Symbole;
            int poids = lettres[indicePioche].Poids;
            //On initialise la lettre sans prendre en compte sa quantité pour le moment
            Lettre lettrePioche = new Lettre(symbole, poids);

            //La quantité de la lettre dans le pioche est décrémentée, et si elle atteint 0, la lettre est effacée de la pioche
            lettres[indicePioche].Quantite--;
            quantiteTotale--;
            if (lettres[indicePioche].Quantite == 0) lettres.RemoveAt(indicePioche);
            return lettrePioche;
        }

        /// <summary>
        /// Vérifie si une lettre existe dans une liste de lettres et retourne son indice
        /// </summary>
        /// <param name="a">Lettre dont on souhaite vérifier l'existence</param>
        /// <returns>Indice de la lettre si elle existe dans la liste, ou -1 si elle n'existe pas</returns>
        public int Indice(Lettre a)
        {
            int indice = -1;
            for (int i = 0; i < lettres.Count; i++)
            {
                if (lettres[i].EstEgale(a)) indice = i;
            }
            return indice;
        }

        /// <summary>
        /// Permet d'afficher l'ensemble des lettres de la liste
        /// </summary>
        /// <returns>Chaine de caractère qui décrit la liste de lettres</returns>
        public string toString()
        {
            string result = "Lettres de " + nom + " : \n|Lettres  | ";
            for (int i = 0; i < lettres.Count; i++)
            {
                result += (lettres[i]).Symbole + "| ";
            }
            result += "\n|Quantité |";
            for (int i = 0; i < lettres.Count; i++)
            {
                if (lettres[i].Quantite < 10) result += " ";
                result += lettres[i].Quantite + "|";
            }
            result += "\n|Poids    |";
            for (int i = 0; i < lettres.Count; i++)
            {
                if (lettres[i].Poids < 10) result += " ";
                result += lettres[i].Poids + "|";
            }
            result += "\n";
            return result;
        }
        #endregion
    }
}
