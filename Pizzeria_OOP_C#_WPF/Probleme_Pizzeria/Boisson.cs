using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Probleme_Pizzeria
{
    /// <summary>
    /// Classe de Boisson, héritée de Produit, caractérisée par un nom et une taille
    /// </summary>
    public class Boisson : Produit
    {
        #region Constructeurs
        /// <summary>
        /// Constructeur d'une instance de la classe boisson
        /// </summary>
        /// <param name="nom">Nom de la boisson</param>
        /// <param name="taille">Volume de la boisson</param>
        public Boisson(string nom, int taille) : base (nom, taille)  { }
        #endregion

        #region Methodes
        /// <summary>
        /// Détermine le prix de la Boisson à partir de la liste triée de la pizzeria
        /// contenant les prix des boissons selon leurs tailles
        /// </summary>
        /// <returns>Prix de la boisson</returns>
        public override double DeterminerPrix()
        { return Pizzeria.Boissons[nom][taille]; }

        /// <summary>
        /// Retourne une chaine qui représente l'instance de la classe
        /// </summary>
        /// <returns>Chaine qui représente l'instance de la classe</returns>
        public override string ToString()
        {
            string s = "Boisson : " + nom + "\tVolume : ";
            switch (taille)
            {
                case 0:
                    s += "33cL";
                    break;

                case 1:
                    s += "50cL";
                    break;

                case 2:
                    s += "1L";
                    break;
            }
            return s + base.ToString();
        }
        #endregion
    }
}
