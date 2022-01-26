using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traitement_Images
{
    /// <summary>
    /// Classe de nombres complexes, utile dans le projet pour créer des fractales, même s'il aurait été possible de s'en
    /// passer en créant simplement 2 suites pour remplacer la partie "réelle" et "imaginaire", la classe m'a semblé intéressante
    /// à creer.
    /// </summary>
    public class Complexe
    {
        #region Champs
        float reel;
        float imaginaire;
        #endregion

        #region Constructeur
        /// <summary>
        /// Crée un nombre complexe à partir de sa partie réelle et de sa partie imaginaire
        /// </summary>
        /// <param name="re">Partie réelle</param>
        /// <param name="im">Partie imaginaire</param>
        public Complexe(float re, float im)
        {
            this.reel = re;
            this.imaginaire = im;
        }
        #endregion

        #region Attributs
        /// <summary>
        /// Partie réelle du complexe créé
        /// </summary>
        public float Re
        {
            get { return this.reel; }
        }

        /// <summary>
        /// Partie imaginaire du complexe créé
        /// </summary>
        public float Im
        {
            get { return this.imaginaire; }
        }

        /// <summary>
        /// Module du complexe
        /// </summary>
        public double Module
        {
            get { return Math.Sqrt(reel * reel + imaginaire * imaginaire); }
        }

        #endregion

        #region Methodes
        /// <summary>
        /// Additionne deux complexes
        /// </summary>
        /// <param name="a">Complexe a</param>
        /// <param name="b">Complexe b</param>
        /// <returns>a + b</returns>
        public static Complexe operator +(Complexe a, Complexe b)
        {
            float re = a.reel + b.reel;
            float im = a.imaginaire + b.imaginaire;
            Complexe resultat = new Complexe(re, im);
            return resultat;
        }

        /// <summary>
        /// Soustrait deux complexes
        /// </summary>
        /// <param name="a">Complexe a</param>
        /// <param name="b">Complexe b</param>
        /// <returns>a - b</returns>
        public static Complexe operator -(Complexe a, Complexe b)
        {
            float re = a.reel - b.reel;
            float im = a.imaginaire - b.imaginaire;
            Complexe resultat = new Complexe(re, im);
            return resultat;
        }

        /// <summary>
        /// Multiplie deux complexes
        /// </summary>
        /// <param name="a">Complexe a</param>
        /// <param name="b">Complexe b</param>
        /// <returns>a * b</returns>
        public static Complexe operator * (Complexe a, Complexe b)
        {
            float re = (a.reel * b.reel) - (a.imaginaire * b.imaginaire);
            float im = (a.reel * b.imaginaire) + (b.reel * a.imaginaire);
            Complexe resultat = new Complexe(re, im);
            return resultat;
        }

        /// <summary>
        /// Vérifie qu'un complexe est égal à un autre
        /// </summary>
        /// <param name="z">Complexe z à vérifier</param>
        /// <returns>true si les complexes sont égaux, false sinon</returns>
        public bool Egal(Complexe z)
        {
            bool egalite = false;
            if (this.reel == z.reel && this.imaginaire == z.imaginaire) egalite = true;
            return egalite;
        }

        /// <summary>
        /// Affiche l'instance de la classe Complexe
        /// </summary>
        /// <returns>x + yi</returns>
        public override string ToString()
        {
            return reel + " + " + imaginaire + "i";
        }
        #endregion
    }
}
