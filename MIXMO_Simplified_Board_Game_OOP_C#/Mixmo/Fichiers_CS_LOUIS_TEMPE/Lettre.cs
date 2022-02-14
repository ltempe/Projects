using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mixmo
{
    /// <summary>
    /// Classe de lettre, une lettre étant définie par son symbole, sa quantité, et son poids
    /// </summary>
    public class Lettre
    {
        #region Champs
        char symbole; //Symbole de la lettre
        int quantite; //Nombre d'occurrence de la lettre quand elle est placée dans une liste
        int poids; //Nombre de points que donne la liste
        #endregion

        #region Constructeurs
        /// <summary>
        /// Constructeur avec tous les champs
        /// </summary>
        /// <param name="symbole">Symbole de la lettre</param>
        /// <param name="quantite">Nombre d'occurrences de la lettre</param>
        /// <param name="poids">Points qu'offre la lettre</param>
        public Lettre (char symbole, int quantite, int poids)
        {
            this.symbole = symbole;
            this.quantite = quantite;
            this.poids = poids;
        }

        /// <summary>
        /// Constructeur sans la quantité
        /// </summary>
        /// <param name="symbole">Symbole de la lettre</param>
        /// <param name="poids">Points qu'offre la lettre</param>
        public Lettre(char symbole, int poids)
        {
            this.symbole = symbole;
            this.poids = poids;
        }

        /// <summary>
        /// Constructeur de lettre par copie
        /// </summary>
        /// <param name="a">Lettre à copier</param>
        public Lettre(Lettre a)
        {
            this.symbole = a.symbole;
            this.quantite = a.quantite;
            this.poids = a.poids;
        }
        #endregion

        #region Attributs
        /// <summary>
        /// Attribut du symbole avec possibilité d'écriture
        /// </summary>
        public char Symbole
        {
            get { return symbole; }
            set { symbole = value; } //Set utile pour les jockers notamment
        }

        /// <summary>
        /// Attribut de la quantité avec possibilité d'écriture
        /// </summary>
        public int Quantite
        {
            get { return quantite; }
            set { quantite = value; }
        }

        /// <summary>
        /// Attribut du poids avec possibilité d'écriture
        /// </summary>
        public int Poids
        {
            get { return poids; }
            set { poids = value; }
        }
        #endregion

        #region Methode
        /// <summary>
        /// Vérifie si la lettre possède le même symbole qu'une autre
        /// </summary>
        /// <param name="a">Lettre à vérifier</param>
        /// <returns>true si les symboles sont égaux, false sinon</returns>
        public bool EstEgale(Lettre a)
        {
            return this.symbole == a.symbole;
        }

        /// <summary>
        /// Permet de renvoyer les attributs de la lettre sous forme de chaine de caractères
        /// </summary>
        /// <returns>Attributs de la lettre</returns>
        public string toString()
        {
            return symbole + "," + quantite + "," + poids;
        }
        #endregion

    }
}
