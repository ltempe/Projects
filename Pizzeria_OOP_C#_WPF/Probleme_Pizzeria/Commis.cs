using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Probleme_Pizzeria
{
    /// <summary>
    /// Classe d'un commis, héritant de Salarie, caractérisé par une date d'embauche
    /// </summary>
    public class Commis : Salarie, IComparable<Commis>
    {
        #region Attributs
        private readonly DateTime dateEmbauche;
        #endregion

        #region Constructeurs
        /// <summary>
        /// Constructeur par défaut d'une instance de la classe Commis
        /// </summary>
        /// <param name="nom">Nom</param>
        /// <param name="prenom">Prénom</param>
        /// <param name="adresse">Adresse (numéro et rue)</param>
        /// <param name="ville">Ville + Code postal</param>
        /// <param name="telephone">Numéro de téléphone</param>
        /// <param name="etat">0-Sur place, 1-En congés</param>
        public Commis(string nom, string prenom, string adresse, string ville, string telephone, int etat) 
        : base(nom, prenom, adresse, ville, telephone, etat)
        { this.dateEmbauche = DateTime.Now; this.salaire = CalculerSalaire(); }

        /// <summary>
        /// Constructeur d'un Commis en entrant tous ses attributs en paramètre
        /// </summary>
        /// <param name="nom">Nom</param>
        /// <param name="prenom">Prénom</param>
        /// <param name="adresse">Adresse (numéro et rue)</param>
        /// <param name="ville">Ville + Code postal</param>
        /// <param name="telephone">Numéro de téléphone</param>
        /// <param name="etat">0-Sur place, 1-En congés</param>
        /// <param name="dateEmbauche">Date d'embauche</param>
        public Commis(string nom, string prenom, string adresse, string ville, string telephone, int nbCommandes, int etat, DateTime dateEmbauche) 
        : base(nom, prenom, adresse, ville, telephone, nbCommandes, etat)
        { this.dateEmbauche = dateEmbauche; this.salaire = CalculerSalaire(); }
        #endregion

        #region Proprietes
        /// <summary>
        /// Propriété de la date d'embauche, en lecture seule
        /// </summary>
        public DateTime Embauche
        { get { return this.dateEmbauche; } }
        #endregion

        #region Methodes
        /// <summary>
        /// Compare le commis étudié avec un autre commis, sur le critère d'ancienneté
        /// </summary>
        /// <param name="c">Commis à comparer</param>
        /// <returns>-1, 0 ou 1 selon le résultat de la comparaison</returns>
        public int CompareTo(Commis c)
        { return c.Anciennete().CompareTo(Anciennete()); }

        /// <summary>
        /// Retourne une chaine de caractère pouvant être intégrée à une ligne de fichier texte comme un
        /// fichier csv. Implémentation de l'interface IIdentifiable
        /// </summary>
        /// <returns>Chaine de caractère d'attributs séparés par des ';' pouvant être intégrée à une ligne de fichier csv</returns>
        public override string ToFile()
        { return base.ToFile() + dateEmbauche.ToShortDateString(); }

        /// <summary>
        /// Calcule l'ancienneté en nombre d'années du commis dans la pizzeria
        /// </summary>
        /// <returns>Ancienneté du commis en nombre d'années</returns>
        public int Anciennete()
        { return DateTime.Now.Year - dateEmbauche.Year; }

        /// <summary>
        /// Calcule le salaire du commis, avec un bonus de 100€ par année d'ancienneté
        /// </summary>
        /// <returns>Salaire du commis</returns>
        public override double CalculerSalaire()
        { return 1500 + (100 * Anciennete()); }

        /// <summary>
        /// Retourne une chaine qui représente l'instance de la classe
        /// </summary>
        /// <returns>Chaine qui représente l'instance de la classe</returns>
        public override string ToString()
        {
            string s = base.ToString();
            s += "\tEmbauché le " + dateEmbauche.ToShortDateString() + "\t";
            s += "Ancienneté : " + Anciennete() + " ans\n";
            s += nbCommandes + (nbCommandes < 2 ? " commande gérée" : " commandes gérées");
            return s;
        }
        #endregion
    }
}
