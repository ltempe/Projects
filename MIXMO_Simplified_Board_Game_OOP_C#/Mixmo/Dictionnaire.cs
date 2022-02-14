using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mixmo
{
    /// <summary>
    /// Classe du dictionnaire des mots possibles
    /// </summary>
    public class Dictionnaire
    {
        #region Champs
        string nom; //Nom du dictionnaire, peu utile
        int nombreMots; //Compte le nombre de mots pour chaque collection, peu utile aussi
        SortedList<int, string[]> mots = new SortedList<int, string[]>();
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur du dictionnaire
        /// </summary>
        /// <param name="nom">Nom du dictionnaire</param>
        /// <param name="fichier">Fichier dans lequel se situent les mots acceptés</param>
        public Dictionnaire(string nom, string fichier)
        {
            this.nom = nom;
            CreerDictionnaire(fichier);
        }
        #endregion

        #region Methodes
        /// <summary>
        /// Crée un dictionnaire à partir d'un fichier contenant les mots autorisés
        /// </summary>
        /// <param name="fichier">Fichier dans lequel se situent les mots acceptés</param>
        public void CreerDictionnaire(string fichier)
        {
            StreamReader sReader = null;
            try
            {
                sReader = new StreamReader(fichier);
                string ligne;
                while ((ligne = sReader.ReadLine()) != null)
                {
                    int nombre = Convert.ToInt32(ligne);
                    ligne = sReader.ReadLine();
                    string[] ensembleMots = ligne.Split(' ');
                    this.nombreMots += ensembleMots.Length;
                    mots.Add(nombre, ensembleMots);
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
        /// Donne l'indice dans le dictionnaire de la catégorie correspondant à la taille d'un mot, bien que totalement inutile au
        /// final car j'ai découvert mots.IndexOfKey(int key)
        /// </summary>
        /// <param name="taille">Taille dont on recherche l'indice</param>
        /// <returns>Indice recherché</returns>
        public int IndiceMotsTaille(int taille)
        {
            //Exactement le même principe que mots.IndexOfkey(int key), que j'ai découvert après, bref ...
            int indice = -1;
            for (int i = 0; i < mots.Keys.Count; i++)
            {
                if (mots.Keys[i] == taille) indice = i;
            }
            return indice;
        }

        /// <summary>
        /// Vérifie de façon dichotomique et récursive qu'un mot existe dans le dictionnaire
        /// </summary>
        /// <param name="mot">Mot à rechercher</param>
        /// <param name="debut">Indice de début de recherche, nul par défaut</param>
        /// <param name="fin">Indice de fin de recherche, -2 par défaut pour être initialisé directement</param>
        public bool RechDichoRecursif(string mot, int debut = 0, int fin = -2)
        {
            //On vérifie qu'il existe dans le dictionnaire des mots de la taille du mot recherché
            //int indiceTaille = IndiceMotsTaille(mot.Length);
            int indiceTaille = mots.IndexOfKey(mot.Length);
            if (indiceTaille == -1) return false;
            //On initialise les paramètres dès le début, pour ne pas avoir à les entrer en dehors de la méthode
            if (fin == -2) fin = mots.Values[indiceTaille].Length - 1;
            int milieu = (debut + fin) / 2;
            if (debut > fin) return false;
            else
                if (mot == mots.Values[indiceTaille][milieu])
                return true;
            else
                if (String.Compare(mot, mots.Values[indiceTaille][milieu]) < 0)
                return RechDichoRecursif(mot, debut, milieu - 1);
            else
                return RechDichoRecursif(mot, milieu + 1, fin);
        }
        
        /// <summary>
        /// Met le nombre de mots possibles, en fonction du nombre de lettres, du dictionnaire sous forme de chaine de caractères
        /// </summary>
        /// <returns>Chaine de caractères en question</returns>
        public string toString()
        {
            string result = nom + " possède " + nombreMots + " mots possibles";
            for (int i = 0; i < mots.Keys.Count; i++)
            {
                result += "\n" + mots.Keys[i] + " lettres : " + mots.Values[i].Length + " mots possibles";
            }
            result += "\n";
            return result;
        }
        #endregion
    }
}
