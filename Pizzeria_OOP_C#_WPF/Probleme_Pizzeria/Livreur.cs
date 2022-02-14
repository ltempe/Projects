using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Probleme_Pizzeria
{
    /// <summary>
    /// Classe d'un livreur, héritant de Salarie, caractérisé par un moyen de transport
    /// </summary>
    public class Livreur : Salarie, IComparable<Livreur>
    {
        #region Attributs
        private string transport;
        private readonly static List<string> transports = new List<string>();
        #endregion

        #region Constructeurs
        /// <summary>
        /// Constructeur d'une instance de la classe Livreur
        /// </summary>
        /// <param name="nom">Nom</param>
        /// <param name="prenom">Prénom</param>
        /// <param name="adresse">Adresse (numéro et rue)</param>
        /// <param name="ville">Ville + Code postal</param>
        /// <param name="telephone">Numéro de téléphone</param>
        /// <param name="etat">0-Sur place, 1-En congés, 2-Sur la route</param>
        /// <param name="transport">Moyen de transport</param>
        public Livreur(string nom, string prenom, string adresse, string ville, string telephone, int nbCommandes, int etat, string transport) 
        : base(nom, prenom, adresse, ville, telephone, nbCommandes, etat)
        {
            this.transport = transport;
            if (transports.IndexOf(transport) == -1) { transports.Add(transport); }
            this.salaire = CalculerSalaire();
        }
        #endregion

        #region Proprietes
        /// <summary>
        /// Propriété du moyen de transport, en lecture et en écriture
        /// </summary>
        public string Transport
        { get { return this.transport; } set { this.transport = value; } }

        /// <summary>
        /// Propriété statique renvoyant une liste de tous les transports utilisés par tous les livreurs
        /// en lecture seule
        /// </summary>
        public static List<string> Transports
        { get { return transports; } }
        #endregion

        #region Methodes
        /// <summary>
        /// Compare le livreur étudié avec un autre livreur, sur le critère du moyen de transport
        /// </summary>
        /// <param name="l">Livreur à comparer</param>
        /// <returns>-1, 0 ou 1 selon le résultat de la comparaison</returns>
        public int CompareTo(Livreur l)
        { return transport.CompareTo(l.transport); }

        /// <summary>
        /// Retourne une chaine de caractère pouvant être intégrée à une ligne de fichier texte comme un
        /// fichier csv. Implémentation de l'interface IIdentifiable
        /// </summary>
        /// <returns>Chaine de caractère d'attributs séparés par des ';' pouvant être intégrée à une ligne de fichier csv</returns>
        public override string ToFile()
        { return base.ToFile() + transport; }

        /// <summary>
        /// Calcule le salaire du livreur en fonction du moyen de transport utilisé
        /// (oui je n'ai pas trouvé mieux que la taille du mot pour faire varier le salaire, celui qui utilise
        /// un "cyclomoteur 50 centimètres cubes" a de la chance
        /// </summary>
        /// <returns>Salaire du livreur</returns>
        public override double CalculerSalaire()
        { return 1200 + (nbCommandes * transport.Length); }

        /// <summary>
        /// Retourne une chaine qui représente l'instance de la classe
        /// </summary>
        /// <returns>Chaine qui représente l'instance de la classe</returns>
        public override string ToString()
        {
            string s = base.ToString() + "\t" + nbCommandes + (nbCommandes < 2 ? " livraison effectuée" : " livraisons effectuées");
            s += "\tTransport : " + transport;
            return s;
        }
        #endregion
    }
}
