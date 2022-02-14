using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Probleme_Pizzeria
{
    /// <summary>
    /// Classe de Commande, caractérisée par un numéro, une date, une heure, un Client, un Commis, un Livreur, une liste de produits, un prix, un état
    /// </summary>
    public class Commande : IIdentifiable, IComparable<Commande>
    {
        #region Attributs
        private readonly string noCommande;
        private readonly string heure;
        private readonly string date;
        private readonly Client client;
        private readonly Commis commis;
        private readonly Livreur livreur;
        private readonly List<Produit> produits;
        private readonly double prix;
        private int etat;
        private bool reussie;
        private static double plusChere = 0;
        #endregion

        #region Constructeurs
        /// <summary>
        /// Constructeur d'une instance de la classe commande, en entrant tous ses attributs en paramètres
        /// </summary>
        /// <param name="noCommande">NUméro de commande</param>
        /// <param name="heure">Heure</param>
        /// <param name="date">Date</param>
        /// <param name="client">Client commandant</param>
        /// <param name="commis">Commis gérant la commande</param>
        /// <param name="livreur">Livreur livrant la commande</param>
        /// <param name="produits">Liste des produits contenus dans la commande</param>
        /// <param name="etat">0-En préparation, 1-En livraison, 2-Fermée</param>
        /// <param name="reussie">true si la commande a été honorée, false sinon</param>
        public Commande(string noCommande, string heure, string date, Client client, Commis commis, Livreur livreur, List<Produit> produits, int etat, bool reussie)
        {
            this.noCommande = noCommande;
            this.heure = heure;
            this.date = date;
            this.client = client;
            this.commis = commis;
            this.livreur = livreur;
            this.produits = produits;
            this.etat = etat;
            this.reussie = reussie;
            this.prix = CalculerPrix();
            if (reussie && plusChere < prix) { plusChere = prix; }
        }

        /// <summary>
        /// Constructeur par défaut d'une instance de la classe Commande
        /// </summary>
        /// <param name="noCommande">NUméro de commande</param>
        /// <param name="client">Client commandant</param>
        /// <param name="commis">Commis gérant la commande</param>
        /// <param name="livreur">Livreur livrant la commande</param>
        /// <param name="produits">Liste des produits contenus dans la commande</param>
        public Commande(string noCommande, Client client, Commis commis, Livreur livreur, List<Produit> produits)
        {
            this.noCommande = noCommande;
            this.heure = DateTime.Now.ToShortTimeString();
            this.date = DateTime.Now.ToShortDateString();
            this.client = client;
            this.commis = commis;
            this.livreur = livreur;
            this.produits = produits;
            this.prix = CalculerPrix();
            this.etat = 0;
            this.reussie = false;
            if (plusChere < prix) { plusChere = prix; }
        }
        #endregion

        #region Proprietes
        /// <summary>
        /// Propriété retournant un tableau contenant les numéros du client, du commis et du livreur associé
        /// à la commande, le but étant de ne pas créer de propriétés d'objets ou de collections génériques, 
        /// pour rester sur quelque chose de non modifiable à l'extérieur (principe d'encapsulation, tout ça)
        /// </summary>
        public string NumerosAssocies
        { get { return client.Numero + ";" + commis.Numero + ";" + livreur.Numero; } }

        /// <summary>
        /// Numéro de la commande, en lecture seule. Implémentation de l'interface IIdentifiable
        /// </summary>
        public string Numero
        { get { return this.noCommande; } }

        /// <summary>
        /// Etat de la commande, en lecture et en écriture
        /// </summary>
        public int Etat
        { get { return this.etat; } set { this.etat = value; } }

        /// <summary>
        /// Date et heure de la création de la commande
        /// </summary>
        public DateTime Date
        { get { return Convert.ToDateTime(date + " " + heure); } }

        /// <summary>
        /// Liste des pizzas contenues dans la commande
        /// </summary>
        public List<Pizza> Pizzas
        {
            get
            {
                List<Pizza> pizzas = new List<Pizza>();
                foreach (Produit p in produits)
                { if (p is Pizza) { pizzas.Add((Pizza)p); } }
                return pizzas;
            }
        }

        /// <summary>
        /// Liste des boissons contenues dans la commande
        /// </summary>
        public List<Boisson> Boissons
        {
            get
            {
                List<Boisson> boissons = new List<Boisson>();
                foreach (Produit p in produits)
                { if (p is Boisson) { boissons.Add((Boisson)p); } }
                return boissons;
            }
        }

        /// <summary>
        /// Propriété de l'état réussi ou non de la commande, en lecture et en écriture
        /// </summary>
        public bool Reussie
        { get { return this.reussie; } set { this.reussie = value; plusChere = plusChere < prix ? prix : plusChere; } }

        /// <summary>
        /// Propriété statique de la commande la plus chère parmi toutes les commandes créées
        /// </summary>
        public static double PlusChere
        { get { return plusChere; } }
        #endregion

        #region Methodes
        /// <summary>
        /// Compare la commande étudiée avec une autre commande, sur le critère de l'état de la commande
        /// </summary>
        /// <param name="c">Commande à comparer</param>
        /// <returns>-1, 0 ou 1 selon le résultat de la comparaison</returns>
        public int CompareTo(Commande c)
        {
            int compare = etat.CompareTo(c.etat);
            if (compare == 0) { compare = reussie ? -1 : 1; }
            return compare;
        }

        /// <summary>
        /// Calcule le prix de la commande
        /// </summary>
        /// <returns>Prix de la commande</returns>
        public double CalculerPrix()
        {
            double result = 0;
            foreach(Produit p in produits)
            { result += p.Prix; }
            return result;
        }

        /// <summary>
        /// Retourne une chaine de caractère pouvant être intégrée à une ligne de fichier texte comme un
        /// fichier csv. Implémentation de l'interface IIdentifiable
        /// </summary>
        /// <returns>Chaine de caractère d'attributs séparés par des ';' pouvant être intégrée à une ligne de fichier csv</returns>
        public string ToFile()
        {
            string s = noCommande + "\n";
            s += heure + ";" + date + ";" + client.Numero + ";" + commis.Numero + ";" + livreur.Numero + ";" + etat + ";" + reussie + "\n";
            s += "pizzas;";
            List<Pizza> pizzas = new List<Pizza>();
            foreach(Produit p in produits)
            { if (p is Pizza) { pizzas.Add((Pizza)p); } }
            for (int i = 0; i < pizzas.Count; i++)
            { s += pizzas[i].Nom + ";" + pizzas[i].Taille + (i < pizzas.Count - 1 ? ";" : ""); }
            List<Boisson> boissons = new List<Boisson>();
            foreach (Produit p in produits)
            { if (p is Boisson) { boissons.Add((Boisson)p); } }
            s += "\nboissons" + (boissons.Count == 0 ? "":";");
            for (int i = 0; i < boissons.Count; i++)
            { s += boissons[i].Nom + ";" + boissons[i].Taille + (i < boissons.Count - 1 ? ";" : ""); }
            return s;
        }

        /// <summary>
        /// Retourne une chaine qui représente l'instance de la classe
        /// </summary>
        /// <returns>Chaine qui représente l'instance de la classe</returns>
        public override string ToString()
        {
            string s = "Commande numéro " + noCommande + ", effectuée le " + date + " à " + heure + "\n";
            s += "Client : " + client.Nom + "\t" + client.Prenom + "\t" + client.Numero + "\nCommis : " + commis.Nom + "\t" + commis.Prenom + "\t" + commis.Numero + "\nLivrée par : " + livreur.Nom + "\t" + livreur.Prenom + "\t" + livreur.Numero + "\n";
            foreach (Produit produit in produits) { s += produit + "\n"; }
            s += "Total : " + prix + " euros\t";
            switch (etat)
            {
                case 0:
                    s += "En préparation";
                    break;

                case 1:
                    s += "En livraison";
                    break;

                case 2:
                    s += "Fermée";
                    break;
            }
            s += reussie ? "\tLa commande a été honorée" : (etat == 2 ? "\tLa commande n'a pas pu être honorée" : "\tLa commande n'est pas encore honorée");
            return s;
        }
        #endregion
    }
}
