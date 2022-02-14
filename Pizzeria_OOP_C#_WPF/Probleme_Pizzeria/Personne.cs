using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Probleme_Pizzeria
{
    /// <summary>
    /// Classe d'une Personne, implémentant l'interface IIdentifiable
    /// </summary>
    public abstract class Personne : IIdentifiable
    {
        #region Attributs
        protected string nom;
        protected string prenom;
        protected string adresse;
        protected string ville;
        protected string telephone;
        protected int nbCommandes;
        private static readonly List<string> villes = new List<string>();
        private static int maxCommandes;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur par défaut d'une instance de la classe Personne
        /// </summary>
        /// <param name="nom">Nom</param>
        /// <param name="prenom">Prénom</param>
        /// <param name="adresse">Adresse (numéro et rue)</param>
        /// <param name="ville">Ville + Code postal</param>
        /// <param name="telephone">Numéro de téléphone</param>
        protected Personne(string nom, string prenom, string adresse, string ville, string telephone)
        {
            this.nom = nom;
            this.prenom = prenom;
            this.adresse = adresse;
            this.ville = ville;
            this.telephone = telephone;
            this.nbCommandes = 0;
            if (villes.IndexOf(ville) == -1) { villes.Add(ville); }
        }

        /// <summary>
        /// Constructeur d'une instance de la classe Personne, en entrant tous ses attributs en paramètres
        /// </summary>
        /// <param name="nom">Nom</param>
        /// <param name="prenom">Prénom</param>
        /// <param name="adresse">Adresse (numéro et rue)</param>
        /// <param name="ville">Ville + Code postal</param>
        /// <param name="telephone">Numéro de téléphone</param>
        /// <param name="nbCommandes">Nombre de commandes effectuées, gérées ou livrées</param>
        protected Personne(string nom, string prenom, string adresse, string ville, string telephone, int nbCommandes)
        {
            this.nom = nom;
            this.prenom = prenom;
            this.adresse = adresse;
            this.ville = ville;
            this.telephone = telephone;
            this.nbCommandes = nbCommandes;
            if (villes.IndexOf(ville) == -1) { villes.Add(ville); }
            if (maxCommandes < nbCommandes) { maxCommandes = nbCommandes; }
        }
        #endregion

        #region Proprietes
        /// <summary>
        /// Propriété statique renvoyant le plus grand nombre de commandes associés à une Personne
        /// parmi toutes les Personnes créées, en lecture seule
        /// </summary>
        public static int MaxCommandes
        { get { return maxCommandes; } }

        /// <summary>
        /// Propriété du nombre de commandes, en lecture et en écriture
        /// </summary>
        public int NbCommandes
        { get { return this.nbCommandes; } set { this.nbCommandes = value; maxCommandes = maxCommandes < value ? value : maxCommandes; } }

        /// <summary>
        /// Numéro de téléphone de la personne en lecture seule, implémentation de l'interface IIdentifiable
        /// </summary>
        public string Numero
        { get { return this.telephone; } }

        /// <summary>
        /// Nom de la personne, en lecture et en écriture
        /// </summary>
        public string Nom
        { get { return this.nom; } set { this.nom = value; } }

        /// <summary>
        /// Prénom de la personne, en lecture et en écriture
        /// </summary>
        public string Prenom
        { get { return this.prenom; } set { this.prenom = value; } }

        /// <summary>
        /// Adresse de la personne, en lecture et en écriture
        /// </summary>
        public string Adresse
        { get { return this.adresse; } set { this.adresse = value; } }

        /// <summary>
        /// Ville et code postal de l'adresse de la personne, en lecture et en écriture
        /// </summary>
        public string Ville
        { get { return this.ville; } set { this.ville = value; } }

        /// <summary>
        /// Propriété statique renvoyant une liste de toutes les villes où habitent les Personnes créées
        /// </summary>
        public static List<string> Villes
        { get { return villes; } }
        #endregion

        #region Methodes
        /// <summary>
        /// Détermine si l'objet spécifié est identique à l'objet actuel.
        /// Ne sert à rien dans ce contexte mais Visual Studio me recommande de l'implémenter dès que je définis
        /// l'opérateur ==
        /// </summary>
        /// <param name="obj">Objet à comparer à l'objet actuel</param>
        /// <returns>true si les 2 objets sont considérés comme égaux, false sinon</returns>
        public override bool Equals(object obj)
        { return base.Equals(obj); }

        /// <summary>
        /// Fait office de fonction de hachage par défaut.
        /// Ne sert à rien dans ce contexte mais Visual Studio me recommande de l'implémenter dès que je définis
        /// l'opérateur ==
        /// </summary>
        /// <returns>Code de hachage pour l'objet actuel.</returns>
        public override int GetHashCode()
        { return base.GetHashCode(); }

        /// <summary>
        /// Détermine si 2 instances de la classe Personne sont considérées comme égales
        /// </summary>
        /// <param name="a">1ere Personne à vérifier</param>
        /// <param name="b">2e Personne à vérifier</param>
        /// <returns>true si le numéro est identique ou si les 2 personnes sont null, false sinon</returns>
        public static bool operator ==(Personne a, Personne b)
        {
            bool result;
            if (a is null && b is null) { result = true; }
            else if (a is null || b is null) { result = false; }
            else { result = a.telephone == b.telephone; }
            return result;
        }

        /// <summary>
        /// Détermine si 2 instances de la classe Personne sont considérées comme différentes
        /// </summary>
        /// <param name="a">1ere Personne à vérifier</param>
        /// <param name="b">2e Personne à vérifier</param>
        /// <returns>false si le numéro est identique ou si les 2 personnes sont null, true sinon</returns>
        public static bool operator !=(Personne a, Personne b)
        { return !(a==b); }

        /// <summary>
        /// Retourne une chaine de caractère pouvant être intégrée à une ligne de fichier texte comme un
        /// fichier csv. Implémentation de l'interface IIdentifiable
        /// </summary>
        /// <returns>Chaine de caractère d'attributs séparés par des ';' pouvant être intégrée à une ligne de fichier csv</returns>
        public virtual string ToFile()
        { return telephone + ";" + nom + ";" + prenom + ";" + adresse + ";" + ville + ";" + nbCommandes + ";"; }

        /// <summary>
        /// Retourne une chaine qui représente l'instance de la classe
        /// </summary>
        /// <returns>Chaine qui représente l'instance de la classe</returns>
        public override string ToString()
        { return "Nom : " + nom + "\tPrénom : " + prenom + "\nAdresse : " + adresse + "\n" + ville + "\tTéléphone : " + telephone + "\t"; }
        #endregion
    }
}
