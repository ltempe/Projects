using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Probleme_Pizzeria
{
    /// <summary>
    /// Classe d'une Pizza, héritant de Produit, caractérisée par un nom et une taille
    /// </summary>
    public class Pizza : Produit
    {
        #region Constructeurs
        /// <summary>
        /// Constructeur d'une instance de la classe Pizza
        /// </summary>
        /// <param name="nom">Nom de la pizza</param>
        /// <param name="taille">0-Petite, 1-Moyenne, 2-Grande</param>
        public Pizza(string nom, int taille) : base (nom, taille) { }
        #endregion

        #region Methodes
        /// <summary>
        /// Détermine le prix de la Pizza à partir de la liste triée de la pizzeria
        /// contenant les prix des pizzas selon leurs tailles
        /// </summary>
        /// <returns>Prix de la pizza</returns>
        public override double DeterminerPrix()
        { return Pizzeria.Pizzas[nom][taille]; }

        /// <summary>
        /// Retourne une chaine qui représente l'instance de la classe
        /// </summary>
        /// <returns>Chaine qui représente l'instance de la classe</returns>
        public override string ToString()
        {
            string s = "Pizza : " + nom + "\tTaille : ";
            switch(taille)
            {
                case 0:
                    s += "petite";
                    break;

                case 1:
                    s += "moyenne";
                    break;

                case 2:
                    s += "grande";
                    break;
            }
            return s + base.ToString();
        }
        #endregion
    }
}
