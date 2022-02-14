using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Probleme_Pizzeria
{
    /// <summary>
    /// Classe abstraite d'un Salarie, héritant de Personne
    /// </summary>
    public abstract class Salarie : Personne
    {
        #region Attributs
        protected int etat;
        protected double salaire;
        #endregion

        #region Constructeurs
        /// <summary>
        /// Constructeur d'un salarié par défaut
        /// </summary>
        /// <param name="nom">Nom</param>
        /// <param name="prenom">Prénom</param>
        /// <param name="adresse">Adresse (numéro et rue)</param>
        /// <param name="ville">Ville + code postal</param>
        /// <param name="telephone">Numéro de téléphone</param>
        /// <param name="etat">0-Sur place, 1-En congés, 2-En livraison</param>
        protected Salarie(string nom, string prenom, string adresse, string ville, string telephone, int etat) 
        : base (nom, prenom, adresse, ville, telephone)
        { this.etat = etat; }

        /// <summary>
        /// Constructeur d'un salarié, en entrant tous ses paramètres
        /// </summary>
        /// <param name="nom">Nom</param>
        /// <param name="prenom">Prénom</param>
        /// <param name="adresse">Adresse (numéro et rue)</param>
        /// <param name="ville">Ville + code postal</param>
        /// <param name="telephone">Numéro de téléphone</param>
        /// <param name="nbCommandes">Nombre de commandes gérées ou livrées</param>
        /// <param name="etat">0-Sur place, 1-En congés, 2-En livraison</param>
        protected Salarie(string nom, string prenom, string adresse, string ville, string telephone, int nbCommandes, int etat) 
        : base(nom, prenom, adresse, ville, telephone, nbCommandes)
        { this.etat = etat; }
        #endregion

        #region Proprietes
        /// <summary>
        /// Propriété de l'état en lecture et en écriture
        /// </summary>
        public int Etat
        { get { return this.etat; } set { this.etat = value; } }
        #endregion

        #region Methodes
        /// <summary>
        /// Retourne une chaine de caractère pouvant être intégrée à une ligne de fichier texte comme un
        /// fichier csv. Implémentation de l'interface IIdentifiable
        /// </summary>
        /// <returns>Chaine de caractère d'attributs séparés par des ';' pouvant être intégrée à une ligne de fichier csv</returns>
        public override string ToFile()
        { return base.ToFile() + etat + ";"; }

        /// <summary>
        /// Méthode abstraite qui calcule le salaire du Salarié selon des critères
        /// </summary>
        /// <returns>Salaire</returns>
        public abstract double CalculerSalaire();

        /// <summary>
        /// Retourne une chaine qui représente l'instance de la classe
        /// </summary>
        /// <returns>Chaine qui représente l'instance de la classe</returns>
        public override string ToString()
        { 
            string s = base.ToString() + "Salaire : " + salaire + " euros\n";
            switch (etat)
            {
                case 0:
                    s += "Sur place";
                    break;

                case 1:
                    s += "En congé";
                    break;

                case 2:
                    s += "Sur la route";
                    break;
            }
            return s;
        }
        #endregion
    }
}
