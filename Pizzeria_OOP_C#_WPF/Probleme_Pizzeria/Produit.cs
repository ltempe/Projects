using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Probleme_Pizzeria
{
    /// <summary>
    /// Classe abstraite d'un Produit, caractérisé par un nom, une taille et un prix
    /// </summary>
    public abstract class Produit
    {
        #region Attributs
        protected double prix;
        protected string nom;
        protected int taille;
        #endregion

        #region Constructeurs
        /// <summary>
        /// Constructeur d'un produit
        /// </summary>
        /// <param name="nom">Nom du produit</param>
        /// <param name="taille">Taille du produit</param>
        protected Produit(string nom, int taille) 
        { this.nom = nom; this.taille = taille; this.prix = DeterminerPrix(); }
        #endregion

        #region Proprietes
        /// <summary>
        /// Propriété du nom, en lecture seule
        /// </summary>
        public string Nom
        { get { return this.nom; } }

        /// <summary>
        /// Propriété de la taille, en lecture seule
        /// </summary>
        public int Taille
        { get { return this.taille; } }

        /// <summary>
        /// Propriété du prix du produit
        /// </summary>
        public double Prix
        { get { return this.prix; } }
        #endregion

        #region Methodes
        /// <summary>
        /// Détermine le prix du produit
        /// </summary>
        /// <returns>Prix du produit en euros</returns>
        public abstract double DeterminerPrix();

        /// <summary>
        /// Retourne une chaine qui représente l'instance de la classe
        /// </summary>
        /// <returns>Chaine qui représente l'instance de la classe</returns>
        public override string ToString()
        { return "\tPrix : " + prix + " euros"; }
        #endregion
    }
}
