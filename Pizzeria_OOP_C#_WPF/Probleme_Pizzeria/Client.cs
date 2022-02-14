using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Probleme_Pizzeria
{
    /// <summary>
    /// Classe d'un client héritant de la classe Personne, caractérisé par une date d'adhésion et une quantité d'achats cumulés
    /// </summary>
    public class Client : Personne, IComparable<Client>
    {
        #region Attributs
        private readonly DateTime dateAdhesion;
        private double cumulAchats;
        private static double maxAchats;
        #endregion

        #region Constructeurs
        /// <summary>
        /// Constructeur par défaut d'une instance de la classe Client
        /// </summary>
        /// <param name="nom">Nom</param>
        /// <param name="prenom">Prénom</param>
        /// <param name="adresse">Adresse (numéro et rue)</param>
        /// <param name="ville">Ville + Code postal</param>
        /// <param name="telephone">Numéro de téléphone</param>
        public Client(string nom, string prenom, string adresse, string ville, string telephone) 
        : base (nom, prenom, adresse, ville, telephone)
        {
            this.dateAdhesion = DateTime.Now;
            this.cumulAchats = 0;
        }

        /// <summary>
        /// Constructeur d'une instance de la classe Client en entrant tous les paramètres
        /// </summary>
        /// <param name="nom">Nom</param>
        /// <param name="prenom">Prénom</param>
        /// <param name="adresse">Adresse (numéro et rue)</param>
        /// <param name="ville">Ville + Code postal</param>
        /// <param name="telephone">Numéro de téléphone</param>
        /// <param name="nbCommandes">Nombre de commandes effectuées à la pizzeria</param>
        /// <param name="adhesion">Date d'adhésion à la pizzeria</param>
        /// <param name="cumul">Cumul des achats effectués dans la pizzeria</param>
        public Client(string nom, string prenom, string adresse, string ville, string telephone, int nbCommandes, DateTime adhesion, double cumul) 
        : base(nom, prenom, adresse, ville, telephone, nbCommandes)
        {
            this.dateAdhesion = adhesion;
            this.nbCommandes = nbCommandes;
            this.cumulAchats = cumul;
            if (maxAchats < cumulAchats) maxAchats = cumulAchats;
        }
        #endregion

        #region Proprietes
        /// <summary>
        /// Propriété statique de la quantité maximale d'achats cumulés qu'un client a effectué dans la pizzeria parmi
        /// tous les clients créés, en lecture seule
        /// </summary>
        public static double MaxAchats
        { get { return maxAchats; } }

        /// <summary>
        /// Propriété de la quantité d'achats cumulés effectués par le client en lecture et en écriture
        /// </summary>
        public double CumulAchat
        { get { return this.cumulAchats; } set { this.cumulAchats = value; maxAchats = maxAchats < value ?  value : maxAchats; } }
        #endregion

        #region Methodes
        /// <summary>
        /// Compare le client étudié avec un autre client, sur le critère du cumul des achats
        /// </summary>
        /// <param name="c">Client à comparer</param>
        /// <returns>-1 si inférieur, 0 si égal, 1 si supérieur</returns>
        public int CompareTo(Client c)
        { return c.cumulAchats.CompareTo(cumulAchats); }

        /// <summary>
        /// Retourne une chaine de caractère pouvant être intégrée à une ligne de fichier texte comme un
        /// fichier csv. Implémentation de l'interface IIdentifiable
        /// </summary>
        /// <returns>Chaine de caractère d'attributs séparés par des ';' pouvant être intégrée à une ligne de fichier csv</returns>
        public override string ToFile()
        { return base.ToFile() + dateAdhesion.ToShortDateString() + ";" + cumulAchats; }

        /// <summary>
        /// Retourne une chaine qui représente l'instance de la classe
        /// </summary>
        /// <returns>Chaine qui représente l'instance de la classe</returns>
        public override string ToString()
        {
            string s = base.ToString();
            s += "Adhésion le " + dateAdhesion.ToShortDateString() + "\n";
            s += nbCommandes + (nbCommandes < 2 ? " commande effectuée" : " commandes effectuées");
            s += "\tCumul des achats : " + cumulAchats;
            return s;
        }
        #endregion
    }
}
