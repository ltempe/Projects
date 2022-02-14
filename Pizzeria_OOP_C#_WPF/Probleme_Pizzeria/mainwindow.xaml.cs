using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Probleme_Pizzeria
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// Classe générale de la gestion de la Pizzeria
    /// </summary>
    public partial class Pizzeria : Window
    {
        #region Attributs
        private List<Client> clients;
        private List<Commis> commis;
        private List<Livreur> livreurs;
        private List<Commande> commandes;
        private List<Commande> commandesAffiche; //Liste des commandes à afficher
        private List<Personne> personnesAffiche; //Liste des personnes à afficher
        private static SortedList<string, double[]> pizzas;
        private static SortedList<string, double[]> boissons;
        private double chiffreAffaires;

        //noms des fichiers pour enregistrer chaque données
        private string fClients;
        private string fCommis;
        private string fLivreurs;
        private string fCommandes;
        private string fProduits;
        #endregion

        #region Constructeur
        public Pizzeria()
        {
            InitializeComponent();
            Initialisation();
        }
        #endregion

        #region Proprietes
        /// <summary>
        /// Liste des pizzas disponibles dans la pizzeria
        /// </summary>
        public static SortedList<string, double[]> Pizzas
        { get { return pizzas; } }

        /// <summary>
        /// Liste des boissons disponibles dans la pizzeria
        /// </summary>
        public static SortedList<string, double[]> Boissons
        { get { return boissons; } }
        #endregion

        #region Delegations
        /// <summary>
        /// Delegation utile pour effectuer une action en fonction du type de l'Identifiable
        /// </summary>
        /// <param name="id">Objet de l'interface IIdentifiable</param>
        /// <returns>Booléen</returns>
        private delegate bool TypeId(IIdentifiable id);

        /// <summary>
        /// Delegation utile pour effectuer une action selon les critères de la personne entrée en paramètres
        /// </summary>
        /// <param name="p">Personne</param>
        /// <returns>Booléen</returns>
        private delegate bool Condition(Personne p);

        /// <summary>
        /// Delegation utile pour retourner notamment une propriété de l'Identifiable entré en paramètre
        /// Utilisé pour afficher les statistiques
        /// </summary>
        /// <param name="id">Objet de l'interface IIdentifiable</param>
        /// <returns>Valeur réelle</returns>
        private delegate double Propriete(IIdentifiable id);
        #endregion

        #region Initialisation
        /// <summary>
        /// Retourne une chaine qui représente l'instance de la classe
        /// </summary>
        /// <returns>Chaine qui représente l'instance de la classe</returns>
        public override string ToString()
        {
            string s = "Chiffre d'affaires : " + chiffreAffaires + "euros\n";
            s += "\nClients\n";
            clients.ForEach(c => s += c + "\n");
            s += "\nCommis\n";
            commis.ForEach(c => s += c + "\n");
            s += "\nLivreurs\n";
            livreurs.ForEach(l => s += l + "\n");
            s += "\nCommandes\n";
            commandes.ForEach(c => s += c + "\n");
            s += "\nPizzas\n";
            foreach (KeyValuePair<string, double[]> pizza in pizzas)
            { s += pizza.Key + " : " + " petite->" + pizza.Key[0] + " moyenne->" + pizza.Key[1] + " grande->" + pizza.Key[2] + "\n"; }
            s += "\nBoissons\n";
            foreach (KeyValuePair<string, double[]> boisson in boissons)
            { s += boisson.Key + " : " + " 33cL->" + boisson.Key[0] + " 50cL->" + boisson.Key[1] + " 1L->" + boisson.Key[2] + "\n"; }
            return s;
        }

        /// <summary>
        /// Initialise tous les attributs de la pizzeria, et donne l'affichage par
        /// défaut de la fenêtre WPF
        /// </summary>
        private void Initialisation()
        {
            //Initialisation des listes
            clients = new List<Client>();
            commis = new List<Commis>();
            livreurs = new List<Livreur>();
            commandes = new List<Commande>();
            commandesAffiche = new List<Commande>();
            personnesAffiche = new List<Personne>();
            pizzas = new SortedList<string, double[]>();
            boissons = new SortedList<string, double[]>();

            //Initialisation des noms des fichiers
            fClients = "fichiers/Clients.csv";
            fCommis = "fichiers/Commis.csv";
            fLivreurs = "fichiers/Livreurs.csv";
            fCommandes = "fichiers/Commandes.csv";
            fProduits = "fichiers/Produits.csv";

            //Récupération des données csv
            RecupererDonnees();

            //Evènement 
            rbLivreur.Checked += (s, e) => champTransport.Visibility = Visibility.Visible;
            rbLivreur.Unchecked += (s, e) => champTransport.Visibility = Visibility.Hidden;
            chercher.KeyDown += (s, e) => { if (e.Key == Key.Enter) { Chercher_Click(null, null); } };
            chercherCommande.KeyDown += (s, e) => { if (e.Key == Key.Enter) { ChercherCommande_Click(null, null); } };
            spFiltre.KeyDown += (s, e) => { if (e.Key == Key.Enter) { Filtrer_Click(null, null); } };
            spFiltre2.KeyDown += (s, e) => { if (e.Key == Key.Enter) { FiltrerCommandes_Click(null, null); } };
            btnCmdHonore.Click += (s, e) => { txtHonore.Text = "Commande honorée"; btnCmdHonore.IsEnabled = false; };
            prixMin.Maximum = Commande.PlusChere;
            prixMax.Maximum = Commande.PlusChere;
            prixMin.ValueChanged += (s, r) => prixMin.Value = Convert.ToInt32(prixMin.Value);
            prixMax.ValueChanged += (s, r) => prixMax.Value = Convert.ToInt32(prixMax.Value);

            //Initialisation de certains paramètres par défaut
            CbxProduit(pizza1, true);
            CbxTaille(taille1, true);
            Actualiser();
            CalculerCA();
            btnCmdHonore.IsEnabled = false;
        }

        /// <summary>
        /// Calcule le Chiffre d'Affaires de la pizzeria
        /// </summary>
        private void CalculerCA()
        {
            chiffreAffaires = 0;
            foreach(Client client in clients)
            { chiffreAffaires += client.CumulAchat; }
            txtCA.Text = "C.A : " + chiffreAffaires + "€";
        }

        /// <summary>
        /// Récupère les données de l'ensemble des fichiers csv
        /// </summary>
        private void RecupererDonnees()
        {
            RecupererProduits();
            RecupererClients();
            RecupererCommis();
            RecupererLivreurs();
            RecupererCommandes();
        }

        /// <summary>
        /// Récupère les noms et prix des produits vendus par la pizzeria
        /// </summary>
        private void RecupererProduits()
        {
            string[] lignes = File.ReadAllLines(fProduits);
            int i;
            for (i = 1; lignes[i] != "boissons"; i++)
            {
                //Pizzas
                string[] tabPizzas = lignes[i].Split(';');
                double[] prix = { Convert.ToDouble(tabPizzas[1]), Convert.ToDouble(tabPizzas[2]), Convert.ToDouble(tabPizzas[3]) };
                pizzas.Add(tabPizzas[0], prix);
            }
            for (i++; i < lignes.Length; i++)
            {
                //Boissons
                string[] tabBoissons = lignes[i].Split(';');
                double[] prix = { Convert.ToDouble(tabBoissons[1]), Convert.ToDouble(tabBoissons[2]), Convert.ToDouble(tabBoissons[3]) };
                boissons.Add(tabBoissons[0], prix);
            }
        }

        /// <summary>
        /// Récupère la liste des clients adhérents à la pizzeria
        /// </summary>
        private void RecupererClients()
        {
            string[] lignes = File.ReadAllLines(fClients);
            foreach (string ligne in lignes)
            {
                string[] tabClients = ligne.Split(';');
                clients.Add(new Client(tabClients[1], tabClients[2], tabClients[3], tabClients[4], tabClients[0], Convert.ToInt32(tabClients[5]), Convert.ToDateTime(tabClients[6]), Convert.ToDouble(tabClients[7])));
            }
        }

        /// <summary>
        /// Récupère la liste des commis travaillant à la pizzeria
        /// </summary>
        private void RecupererCommis()
        {
            string[] lignes = File.ReadAllLines(fCommis);
            foreach (string ligne in lignes)
            {
                string[] tabCommis = ligne.Split(';');
                commis.Add(new Commis(tabCommis[1], tabCommis[2], tabCommis[3], tabCommis[4], tabCommis[0], Convert.ToInt32(tabCommis[5]), Convert.ToInt32(tabCommis[6]), Convert.ToDateTime(tabCommis[7])));
            }
        }

        /// <summary>
        /// Récupère la liste des livreurs de la pizzeria
        /// </summary>
        private void RecupererLivreurs()
        {
            string[] lignes = File.ReadAllLines(fLivreurs);
            foreach (string ligne in lignes)
            {
                string[] tabLivreurs = ligne.Split(';');
                livreurs.Add(new Livreur(tabLivreurs[1], tabLivreurs[2], tabLivreurs[3], tabLivreurs[4], tabLivreurs[0], Convert.ToInt32(tabLivreurs[5]), Convert.ToInt32(tabLivreurs[6]), tabLivreurs[7]));
            }
        }

        /// <summary>
        /// Récupère la liste des commandes effectuées dans la pizzeria
        /// </summary>
        private void RecupererCommandes()
        {
            string[] lignes = File.ReadAllLines(fCommandes);
            for (int i = 0; i < lignes.Length; i++)
            {
                //Une commande est définie sur 4 lignes dans le fichier commandes.csv
                string noCommande = lignes[i++]; //1ere ligne : numéro de commande
                string[] tabInfo = lignes[i++].Split(';'); //2e ligne : infos (date, numéro du client, du commis, etc.)
                string[] tabPizzas = lignes[i++].Split(';'); //3e ligne : pizzas et leurs tailles
                string[] tabBoissons = lignes[i].Split(';'); //4e ligne : boissons s'il y en a et leurs tailles
                
                //Récupération de la liste des produits de la commande
                List<Produit> produits = new List<Produit>();
                for (int j = 1; j < tabPizzas.Length; j++)
                { produits.Add(new Pizza(tabPizzas[j++], Convert.ToInt32(tabPizzas[j]))); }
                for (int j = 1; j < tabBoissons.Length; j++)
                { produits.Add(new Boisson(tabBoissons[j++], Convert.ToInt32(tabBoissons[j]))); }
                
                //Récupération du client, du commis et du livreur associé à la commande
                Client client = (Client)RetournerIdentifiable(tabInfo[2]);
                Commis commis = (Commis)RetournerIdentifiable(tabInfo[3]);
                Livreur livreur = (Livreur)RetournerIdentifiable(tabInfo[4]);
                
                //Ajout de la commande
                commandes.Add(new Commande(noCommande, tabInfo[0], tabInfo[1], client, commis, livreur, produits, Convert.ToInt32(tabInfo[5]), Convert.ToBoolean(tabInfo[6])));
            }
        }

        /// <summary>
        /// Retourne un élément identifiable à partir de son numéro s'il existe
        /// </summary>
        /// <param name="numero">Numéro associé à l'identifiable recherché</param>
        /// <returns>Objet identifiable s'il existe, null sinon</returns>
        private IIdentifiable RetournerIdentifiable(string numero)
        {
            IIdentifiable id = null;
            IList[] listes = { commandes, clients, commis, livreurs };
            for(int i = 0; i < 4 && id == null; i++)
            {
                for (int j = 0; j < listes[i].Count && id == null; j++)
                { id = ((IIdentifiable)listes[i][j]).Numero == numero ? ((IIdentifiable)listes[i][j]) : null; }
            }
            return id;
        }

        /// <summary>
        /// Enregistre les données des identifiables dans le fichier csv qui leur est associé
        /// </summary>
        /// <param name="liste">Liste que l'on souhaite enregistrer</param>
        /// <param name="fichier">Nom du fichier dans lequel on souhaite enregistrer les données</param>
        private void EnregistrerIdentifiable(IList liste, string fichier)
        {
            List<string> lignes = new List<string>();
            foreach (IIdentifiable identifiable in liste)
            {  lignes.Add(identifiable.ToFile()); }
            File.WriteAllLines(fichier, lignes);
        }

        /// <summary>
        /// Enregistre toutes les données de chaque liste d'identifiables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            EnregistrerIdentifiable(clients, fClients);
            EnregistrerIdentifiable(commis, fCommis);
            EnregistrerIdentifiable(livreurs, fLivreurs);
            EnregistrerIdentifiable(commandes, fCommandes);
            rectSave.Fill = Brushes.Green; 
        }

        /// <summary>
        /// Actualise l'ensemble des éléments qui ont besoin d'être actualisés lorsque cette méthode
        /// est appelée, principalement pour des raisons d'affichage par défaut
        /// </summary>
        private void Actualiser()
        {
            cbxStatNb.SelectedIndex = 0;
            prixMax.Value = Commande.PlusChere;
            Filtrer_Click(null, null);
            FiltrerCommandes_Click(null, null);
            ChoixPersonnes(choixCommande);
            ChoixPersonnes(choixFiltreCommande);
            Statistiques(clients, nomStatCumul, rectStatCumul, c => ((Client)c).CumulAchat, Client.MaxAchats, Brushes.Cyan, moyCumul);
            Statistiques(commandes, nomStatCmd, rectStatCmd, c => ((Commande)c).CalculerPrix(), Commande.PlusChere, Brushes.Red, moyCmd);
            CbxStatNb_SelectionChanged(null, null);
        }
        #endregion

        #region Module Client Effectif
        #region Affichage Client/Effectif
        /// <summary>
        /// Méthode permettant de défiler plus facilement les scrollviewer avec la molette
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ((ScrollViewer)sender).ScrollToVerticalOffset(((ScrollViewer)sender).VerticalOffset - (e.Delta / 4));
            e.Handled = true;
        }

        /// <summary>
        /// Actualise les listbox d'affichage des personnes et des commandes dans les 2 premiers modules
        /// </summary>
        private void ActualiserListBox()
        {
            //Toujours effacer les items pour ne pas créer de doublons
            listboxEffectifClient.Items.Clear();
            foreach (Personne p in personnesAffiche) //Module effectif/client
            {
                ListBoxItem item = new ListBoxItem() 
                { BorderThickness = new Thickness(4, 2, 4, 2), Content = new TextBlock() { Text = p.ToString() }, Tag = p };
                //On affecte la personne en Tag de l'item, pour pouvoir récupérer les données de la personne plus tard
                
                //switch pour différencier les personnes selon leurs types, avec des couleurs
                switch (p.GetType().Name)
                {
                    case "Client":
                        item.BorderBrush = Brushes.Blue;
                        item.Background = Brushes.SkyBlue;
                        break;

                    case "Commis":
                        item.BorderBrush = Brushes.Purple;
                        item.Background = Brushes.LightPink;
                        break;

                    case "Livreur":
                        item.BorderBrush = Brushes.OrangeRed;
                        item.Background = Brushes.LightSalmon;
                        break;
                }
                item.MouseUp += ModifierPersonne_MouseUp;
                listboxEffectifClient.Items.Add(item);
            }
            listboxCommande.Items.Clear();
            foreach (Commande c in commandesAffiche) //Module commande
            {
                ListBoxItem item = new ListBoxItem()
                {
                    Content = new TextBlock() { Text = c.ToString() },
                    BorderThickness = new Thickness(4, 2, 4, 2),
                    //Coloration des items différentes selon leur état, avec l'utilisation de l'opérateur ? pour ne pas faire trop de lignes
                    BorderBrush = c.Reussie ? Brushes.Green : (c.Etat == 2 ? Brushes.Red : (c.Etat == 0 ? Brushes.Orange : Brushes.CadetBlue)),
                    Background = c.Reussie ? Brushes.LightGreen : (c.Etat == 2 ? Brushes.LightSalmon : (c.Etat == 0 ? Brushes.LightGoldenrodYellow : Brushes.LightPink)),
                    //Affectation de la commande en Tag pour pouvoir revenir dessus plus tard
                    Tag = c
                };
                item.MouseUp += ActualiserCommande_MouseUp;
                listboxCommande.Items.Add(item);
            }
        }

        /// <summary>
        /// Actualise le formulaire d'inscription, en insérant dans les champs toutes les données de la personne sur laquelle
        /// l'utilisateur vient de sélectionner, afin de pouvoir modifier les données, éventuellement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifierPersonne_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //Les données de la personne sont contenues dans la propriété Tag de la ListBoxItem qui lui est associé
            //Le Tag a été défini dans ActualiserListBox()
            //On peut la récupérer si l'objet sender (ListBoxItem) n'est pas null
            Personne p = sender == null ? null : (Personne)(sender as ListBoxItem).Tag;
            if (validerInscription.Visibility == Visibility.Visible || (p != null && p.Numero != tbTelephone.Text))
            {
                txtInfos.Text = "Modifier client / effectif";
                //On ne peut pas modifier le type d'une personne
                rbClient.IsEnabled = false; rbCommis.IsEnabled = false; rbLivreur.IsEnabled = false;

                //On inverse la visibilité des boutons
                validerInscription.Visibility = Visibility.Hidden;
                modifierInscription.Visibility = Visibility.Visible;

                //Cliquer sur la checkbox qui affiche l'état d'un salarié modifie son affichage
                chkEtatSal.Click += (s, r) => chkEtatSal.Content = chkEtatSal.IsChecked == true ? "Sur place" : "En congés";
                chkEtatSal.IsEnabled = true;
                if (p is Client) { rbClient.IsChecked = true; }
                else
                {
                    chkEtatSal.Visibility = Visibility.Visible;
                    chkEtatSal.IsChecked = ((Salarie)p).Etat == 0;
                    chkEtatSal.Content = chkEtatSal.IsChecked == true ? "Sur place" : "En congés";
                    chkEtatSal.IsEnabled = commandes.Find(c => c.Etat != 2 && (c.NumerosAssocies.Split(';')[1] == p.Numero || c.NumerosAssocies.Split(';')[2] == p.Numero)) == null;
                    if (p is Commis)
                    { chkEtatSal.IsThreeState = false; rbCommis.IsChecked = true; }
                    else
                    {
                        rbLivreur.IsChecked = true;
                        tbTransport.Text = ((Livreur)p).Transport;
                        if (((Livreur)p).Etat == 2) //Cas où le livreur est en livraison
                        { chkEtatSal.IsChecked = null; chkEtatSal.Content = "Sur la route"; }
                    }
                }

                //On remplit les champs des textblocks
                tbNom.Text = p.Nom;
                tbPrenom.Text = p.Prenom;
                tbAdresse.Text = p.Adresse;
                tbVille.Text = p.Ville;
                tbTelephone.Text = p.Numero;
                tbTelephone.IsEnabled = false; 
                //Le numéro de téléphone ne peut pas être modifié, il est unique à la personne dès son inscription
            }
            else
            {
                //Si une personne est déjà sélectionnée lors d'un double-clic, on remet le formulaire d'inscription
                //par défaut
                txtInfos.Text = "Ajouter client / effectif";
                validerInscription.Visibility = Visibility.Visible;
                modifierInscription.Visibility = Visibility.Hidden;
                tbNom.Clear(); tbPrenom.Clear(); tbAdresse.Clear(); tbVille.Clear(); tbTelephone.Clear(); tbTransport.Clear();
                tbTelephone.IsEnabled = true;
                rbClient.IsEnabled = true; rbCommis.IsEnabled = true; rbLivreur.IsEnabled = true;
                chkEtatSal.Visibility = Visibility.Hidden;
            }
        }
        #endregion

        #region Inscription Client/Effectif
        /// <summary>
        /// Valide l'inscription si tous les champs sont remplis si l'utilisateur appuie sur le bouton Valider
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValiderInscription_Click(object sender, RoutedEventArgs e)
        {
            //On vérifie que tous les champs sont remplis et ne commencent pas par un espace
            if (tbNom.Text != "" && tbPrenom.Text != "" && tbAdresse.Text != "" && tbVille.Text != ""
                && tbNom.Text[0] != ' ' && tbPrenom.Text[0] != ' ' && tbAdresse.Text[0] != ' ' && tbVille.Text[0] != ' ')
            {
                //Vérification de l'écriture du numéro de téléphone 
                if (tbTelephone.Text.Length == 10 && int.TryParse(tbTelephone.Text, out _))
                {
                    //On vérifie que le numéro de téléphone n'est pas déjà attribué
                    Personne personne = (Personne)RetournerIdentifiable(tbTelephone.Text);
                    if (personne == null)
                    {
                        string[] villeCP = tbVille.Text.Split(' ');
                        if (villeCP.Length > 1 && int.TryParse(villeCP[villeCP.Length - 1], out _))
                        {
                            //Création de la nouvelle personne, en fonction du RadioButton qui a été coché
                            if (rbClient.IsChecked == true)
                            {
                                personne = new Client(tbNom.Text.ToUpper(), tbPrenom.Text.ToLower(), tbAdresse.Text.ToLower(), tbVille.Text.ToUpper(), tbTelephone.Text);
                                clients.Add((Client)personne);
                            }
                            else if (rbCommis.IsChecked == true)
                            {
                                personne = new Commis(tbNom.Text.ToUpper(), tbPrenom.Text.ToLower(), tbAdresse.Text.ToLower(), tbVille.Text.ToUpper(), tbTelephone.Text, 0);
                                commis.Add((Commis)personne);
                            }
                            else
                            {
                                personne = new Livreur(tbNom.Text.ToUpper(), tbPrenom.Text.ToLower(), tbAdresse.Text.ToLower(), tbVille.Text.ToUpper(), tbTelephone.Text, 0, 0, tbTransport.Text.ToLower());
                                livreurs.Add((Livreur)personne);
                            }
                            tbNom.Clear(); tbPrenom.Clear(); tbAdresse.Clear(); tbVille.Clear(); tbTelephone.Clear(); tbTransport.Clear();
                            rectSave.Fill = Brushes.Red;
                            MessageBox.Show("Le " + personne.GetType().Name.ToLower() + " : " + personne.Nom + " " + personne.Prenom + " a bien été ajouté à la liste des " + personne.GetType().Name.ToLower() + (personne is Commis ? "" : "s"));
                        }
                        else MessageBox.Show("Veuillez entrer un nom de ville ET un code postal au champ \"Ville et CP\"");
                    }
                    else MessageBox.Show(tbNom.Text + " " + tbPrenom.Text + " n'a pas pu être ajouté. Le numéro " + tbTelephone.Text + " est déjà attribué au " + personne.GetType().Name.ToLower() + " : " + personne.Nom + " " + personne.Prenom);
                }
                else MessageBox.Show("Le numéro de téléphone n'est pas valide. Il doit faire 10 chiffres et ne doit contenir que des numéros");
            }
            else MessageBox.Show("Un ou plusieurs champs ont mal été renseignés. Un champ ne peut pas commencer par un espace");
            
            //On pense à actualiser l'affichage !
            Actualiser();
        }

        /// <summary>
        /// Modifie les données d'une personne lorsque l'utilisateur clique sur le bouton Modifier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifierInscription_Click(object sender, RoutedEventArgs e)
        {
            //On retrouve la personne grâce à son numéro, et on modifie ses données si l'utilisateur a changé quelque chose
            //en faisant attention aux cas où les champs sont vides
            Personne p = (Personne)RetournerIdentifiable(tbTelephone.Text);
            string[] villeCP = tbVille.Text.Split(' ');
            if (tbNom.Text != "" && tbNom.Text[0] != ' ') { p.Nom = tbNom.Text.ToUpper();}
            if (tbPrenom.Text != "" && tbPrenom.Text[0] != ' ') p.Prenom = tbPrenom.Text.ToLower();
            if (tbAdresse.Text != "" && tbAdresse.Text[0] != ' ') p.Adresse = tbAdresse.Text.ToLower();
            if (tbVille.Text != "" && tbVille.Text[0] != ' ' && villeCP.Length > 1 && int.TryParse(villeCP[villeCP.Length - 1], out _)) p.Ville = tbVille.Text.ToUpper();
            //Etat des livreurs
            if (p is Livreur && tbTransport.Text != "" && tbTransport.Text != " ") ((Livreur)p).Transport = tbTransport.Text.ToLower();
            if (p is Salarie)
            {
                if (chkEtatSal.IsChecked == true) { ((Salarie)p).Etat = 0; }
                else { ((Salarie)p).Etat = 1; }
            }
            Actualiser(); // !

            //Pour remettre les champs par défaut et faire disparaitre le bouton Modifier
            ModifierPersonne_MouseUp(null, null);
            //Le carré devient rouge pour dire à l'utilisateur que les nouvelles données n'ont pas été enregistrées
            rectSave.Fill = Brushes.Red;
        }
        #endregion

        #region Filtre Client/Effectif
        /// <summary>
        /// Affiche l'interface des choix de filtres lorsque l'utilisateur cliques sur le bouton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFiltres_Click(object sender, RoutedEventArgs e)
        {
            //Inversion de la visibilité
            if (spFiltre.Visibility == Visibility.Hidden) spFiltre.Visibility = Visibility.Visible;
            else spFiltre.Visibility = Visibility.Hidden;

            //Mettre les choix par défaut
            cbxVilleFiltre.Items.Clear();
            cbxVilleFiltre.Items.Add(new ComboBoxItem() { Content = "-Ville-", IsSelected = true });
            Personne.Villes.ForEach(ville => cbxVilleFiltre.Items.Add(new ComboBoxItem() { Content = ville }));
            
            //Afficher les filtres particuliers si un seul type de personne est sélectionné
            Filtre_Unchecked(null, null);
        }

        /// <summary>
        /// Remet la comboBox des tris des personnes par défaut
        /// </summary>
        private void ResetCbxTriPersonne()
        {
            //La valeur du 4e item de la combobox est supprimée
            if (cbxTriPersonne.Items.Count > 4)
            {
                if (cbxTriPersonne.SelectedIndex == 4) { cbxTriPersonne.SelectedIndex = 0; }
                cbxTriPersonne.Items.RemoveAt(4);
            }
            //Le stackpanel du filtre créé dynamiquement (propre à chaque type de personne) est vidé
            if (spFiltreDyn != null) { if (spFiltreDyn.Children.Count != 0) spFiltreDyn.Children.Clear();}
        }

        /// <summary>
        /// Change l'affichage de l'interface des filtres et du tri selon les checkbox cochées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_Checked(object sender, RoutedEventArgs e)
        {
            ResetCbxTriPersonne();
            if (spFiltreDyn != null)
            {
                //Filtre visible uniquement si seul la checkbox du client est cochée
                if (chkCommis.IsChecked == false && chkLivreur.IsChecked == false)
                {
                    //Sliders permettant de filtrer les personnes selon la quantité d'achats effectués dans la pizzeria
                    Slider minAchat = new Slider() { Width = 50, Maximum = Client.MaxAchats };
                    Slider maxAchat = new Slider() { Width = 50, Maximum = Client.MaxAchats, Value = Client.MaxAchats };
                    TextBox tb1 = new TextBox() { Text = minAchat.Value.ToString(), Width = 10 * Client.MaxAchats.ToString().Length };
                    TextBox tb2 = new TextBox() { Text = maxAchat.Value.ToString(), Width = 10 * Client.MaxAchats.ToString().Length };
                    //La valeur du slider maxAchat ne doit pas être inférieure à celle de minAchat, le textox qui lui est associé doit être mis à jour automatiquement, comme un DataBinding en mode TwoWay
                    minAchat.ValueChanged += (s, r) => { minAchat.Value = Convert.ToInt32(minAchat.Value); tb1.BorderBrush = Brushes.Transparent; tb1.Text = "" + minAchat.Value; maxAchat.Minimum = minAchat.Value; };
                    maxAchat.ValueChanged += (s, r) => { maxAchat.Value = Convert.ToInt32(maxAchat.Value); tb2.BorderBrush = Brushes.Transparent; tb2.Text = "" + maxAchat.Value; };
                    
                    //Les sliders voient leur valeur mise automatiquement à jour lorsque les textbox associés sont modifiés
                    tb1.KeyUp += (s, r) => 
                    { 
                        if (double.TryParse(tb1.Text, out double val))
                        {
                            //Gérer les cas où la valeur est hors limite
                            if (val >= 0 && val <= minAchat.Maximum) 
                            { minAchat.Value = val; tb1.BorderBrush = Brushes.Transparent; }
                            else { tb1.BorderBrush = Brushes.Red; }
                        }
                        else { tb1.BorderBrush = Brushes.Red; }
                    };
                    tb2.KeyUp += (s, r) =>
                    {
                        if (double.TryParse(tb2.Text, out double val))
                        {
                            if (val >= maxAchat.Minimum && val <= maxAchat.Maximum)
                            { maxAchat.Value = val; tb2.BorderBrush = Brushes.Transparent; }
                            else { tb2.BorderBrush = Brushes.Red; }
                        }
                        else { tb2.BorderBrush = Brushes.Red; }
                    };
                    //On ajoute au stackpanel du filtre dynamique l'affichage du client
                    spFiltreDyn.Children.Add(new TextBlock() { Text = "Achats cumulés entre " });
                    spFiltreDyn.Children.Add(minAchat);
                    spFiltreDyn.Children.Add(tb1);
                    spFiltreDyn.Children.Add(new TextBlock() { Text = " et " });
                    spFiltreDyn.Children.Add(maxAchat);
                    spFiltreDyn.Children.Add(tb2);

                    //Mise à jour de la combobox des tris exclusifs aux clients
                    cbxTriPersonne.Items.Add(new ComboBoxItem() { Content = "Achats cumulés", Tag = "Client" });
                }
            }
        }

        /// <summary>
        /// Change l'affichage de l'interface des filtres et du tri selon les checkbox cochées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Commis_Checked(object sender, RoutedEventArgs e)
        {
            ResetCbxTriPersonne();
            if (chkClient.IsChecked == false && chkLivreur.IsChecked == false)
            {
                //Le filtre exclusif aux commis se fait sur la date d'embauche
                TextBox dateDebut = new TextBox() { Text = "jj/mm/aaaa", Width = 80 };
                TextBox dateFin = new TextBox() { Text = "jj/mm/aaaa", Width = 80 };

                //Double-cliquer ou cliquer-droit permet d'effacer le texte pour gagner du temps
                dateDebut.MouseUp += (s, r) => dateDebut.Clear();
                dateDebut.MouseDoubleClick += (s, r) => dateDebut.Clear();
                dateFin.MouseUp += (s, r) => dateFin.Clear();
                dateFin.MouseDoubleClick += (s, r) => dateFin.Clear();
                spFiltreDyn.Children.Add(new TextBlock() { Text = "Date d'embauche : du " });
                spFiltreDyn.Children.Add(dateDebut);
                spFiltreDyn.Children.Add(new TextBlock() { Text = "au" });
                spFiltreDyn.Children.Add(dateFin);

                //Mise à jour de la combobox des tris exclusifs aux commis
                cbxTriPersonne.Items.Add(new ComboBoxItem() { Content = "Ancienneté", Tag = "Commis" });
            }
        }

        /// <summary>
        /// Change l'affichage de l'interface des filtres et du tri selon les checkbox cochées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Livreur_Checked(object sender, RoutedEventArgs e)
        {
            ResetCbxTriPersonne();
            if (chkClient.IsChecked == false && chkCommis.IsChecked == false)
            {
                //Le filtre exclusif aux livreurs se fait selon son transport
                ComboBox cbx = new ComboBox();
                cbx.Items.Add(new ComboBoxItem() { Content = "-Transport-", IsSelected = true });
                Livreur.Transports.ForEach(transport => cbx.Items.Add(new ComboBoxItem() { Content = transport }));
                spFiltreDyn.Children.Add(new TextBlock() { Text = "Transport : " });
                spFiltreDyn.Children.Add(cbx);
                //Mise à jour de la combobox des tris exclusifs aux livreurs
                cbxTriPersonne.Items.Add(new ComboBoxItem() { Content = "Transport", Tag = "Livreur" });
            }
        }

        #region Ancien code
        //private void Client_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    if (filtreDyn.Children.Count != 0) filtreDyn.Children.Clear();
        //    if (filtreCommis.IsChecked == true) Commis_Checked(null, null);
        //    else if(filtreLivreur.IsChecked == true) Livreur_Checked(null, null);
        //}

        //private void Commis_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    if (filtreDyn.Children.Count != 0) filtreDyn.Children.Clear();
        //    if (filtreClient.IsChecked == true) Client_Checked(null, null); 
        //    else if (filtreLivreur.IsChecked == true) Livreur_Checked(null, null);
        //}
        #endregion

        /// <summary>
        /// Met à jour l'affichage des filtres selon les checkbox cochées, en appelant les méthodes précédentes
        /// pour mettre à jour si, lorsque l'on décoche une des 3 checkbox, seule une autre est cochée
        /// (alors le filtre dynamique exclusif apparaît)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Filtre_Unchecked(object sender, RoutedEventArgs e)
        {
            if(chkClient.IsChecked == true) Client_Checked(null, null);
            if (chkCommis.IsChecked == true) Commis_Checked(null, null);
            if (chkLivreur.IsChecked == true) Livreur_Checked(null, null);
        }

        /// <summary>
        /// Fonction déléguée permettant d'ajouter une personne à une liste générique si une condition est respectée
        /// </summary>
        /// <param name="liste">Liste dans laquelle insérer la personne</param>
        /// <param name="p">Personne que l'on souhaite ajouter à la liste</param>
        /// <param name="cond">Délégation de condition d'ajout</param>
        private void PersonneFiltre(IList liste, Personne p, Condition cond)
        { if(cond(p)) { liste.Add(p); } }

        /// <summary>
        /// Ajoute les personnes d'une liste respectant un ou des critères sélectionnés dans les filtres préalablement,
        /// à la liste des personnes à afficher
        /// </summary>
        /// <param name="liste">Liste des personnes contenant l'ensemble des personnes (clients, commis ou livreurs)</param>
        /// <param name="c">Condition d'ajout</param>
        private void InsererFiltre(IList liste, Condition c)
        {
            if (cbxVilleFiltre.Text != "-Ville-")
            { foreach (Personne personne in liste) { PersonneFiltre(personnesAffiche, personne, p => p.Ville == cbxVilleFiltre.Text && c(p)); } }
            else foreach (Personne personne in liste) { PersonneFiltre(personnesAffiche, personne, p => c(p)); }
        }

        /// <summary>
        /// Filtre l'ensemble des personnes à afficher selon les critères déterminés par l'utilisateur
        /// beaucoup de if/else, mais beaucoup moins que dans mon ancien code !
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Filtrer_Click(object sender, RoutedEventArgs e)
        {
            //On initialise la liste des personnes à afficher
            personnesAffiche = new List<Personne>();
            if (chkClient.IsChecked == true)
            {
                if (spFiltreDyn.Children.Count == 0)
                {
                    //Pas de condition particulère d'affichage des clients dans ce cas-là, on peut mettre n'importe
                    //quelle condition sans conséquence telle que : p => p is Client
                    InsererFiltre(clients, p => p is Client);
                    if (chkCommis.IsChecked == true)
                    {
                        InsererFiltre(commis, p => p is Commis);
                        if (chkLivreur.IsChecked == true)
                        { InsererFiltre(livreurs, p => p is Livreur); }
                    }
                    else if (chkLivreur.IsChecked == true)
                    { InsererFiltre(livreurs, p => p is Livreur); }
                }
                //Si seul le client est coché, seuls les clients dont le cumul des achats est compris entre les valeurs des 2 sliders du filtre exclusif aux
                //clients peuvent être ajoutés à la liste des personnes à afficher
                else { InsererFiltre(clients, p => ((Client)p).CumulAchat >= ((Slider)spFiltreDyn.Children[1]).Value && ((Client)p).CumulAchat <= ((Slider)spFiltreDyn.Children[4]).Value); }
            }
            else if (chkCommis.IsChecked == true)
            {
                if (spFiltreDyn.Children.Count == 0)
                {
                    InsererFiltre(commis, p => p is Commis);
                    if (chkLivreur.IsChecked == true)
                    { InsererFiltre(livreurs, p => p is Livreur); }
                }
                //Si seul le commis est coché, et que les formats de date entrés dans les textbox du filtre exclusif aux commis sont corrects,
                //seuls les commis dont la date d'adhésion est comprise entre les 2 dates peuvent être ajoutés à la liste des personnes à afficher
                else if (DateTime.TryParse(((TextBox)spFiltreDyn.Children[1]).Text, out DateTime debut) && DateTime.TryParse(((TextBox)spFiltreDyn.Children[3]).Text, out DateTime fin))
                { InsererFiltre(commis, p => ((Commis)p).Embauche.CompareTo(debut) > 0 && ((Commis)p).Embauche.CompareTo(fin) < 0); }
                //Sinon, on peut ajouter tous les commis
                else { InsererFiltre(commis, p => p is Commis); }
            }
            else if (chkLivreur.IsChecked == true)
            {
                if (spFiltreDyn.Children.Count == 0)
                { InsererFiltre(livreurs, p => p is Livreur); }
                else
                {
                    //Si seul le livreur est coché, seuls les livreurs correspondant au combobox seront affichés
                    if (cbxVilleFiltre.Text != "-Ville-")
                    { livreurs.ForEach(livreur => PersonneFiltre(personnesAffiche, livreur, p => p.Ville == cbxVilleFiltre.Text && ((Livreur)livreur).Transport == ((ComboBox)spFiltreDyn.Children[1]).Text)); }
                    else if (((ComboBox)spFiltreDyn.Children[1]).Text != "-Transport-") livreurs.ForEach(livreur => PersonneFiltre(personnesAffiche, livreur, p => ((Livreur)livreur).Transport == ((ComboBox)spFiltreDyn.Children[1]).Text));
                    else livreurs.ForEach(livreur => personnesAffiche.Add(livreur));
                }
            }
            //On trie pour actualiser l'affichage, et on cache l'interface de choix des filtres
            Trier_SelectionChanged(null, null);
            spFiltre.Visibility = Visibility.Hidden;
        }

        #region Ancien code moche
        //private void Filtrer_Click(object sender, RoutedEventArgs e)
        //{
        //    personnesAffiche = new List<Personne>();
        //    if (filtreClient.IsChecked == true)
        //    {
        //        if (filtreDyn.Children.Count == 0)
        //        {
        //            if (filtreCommis.IsChecked == true)
        //            {
        //                if (filtreLivreur.IsChecked == true)
        //                {
        //                    if (villeFiltre.Text != "-Ville-")
        //                    { livreurs.ForEach(livreur => PersonneFiltre(personnesAffiche, livreur, p => p.Ville == villeFiltre.Text)); }
        //                    else livreurs.ForEach(livreur => personnesAffiche.Add(livreur));
        //                }
        //                if (villeFiltre.Text != "-Ville-")
        //                { commis.ForEach(com => PersonneFiltre(personnesAffiche, com, p => p.Ville == villeFiltre.Text)); }
        //                else commis.ForEach(com => personnesAffiche.Add(com));
        //            }
        //            else if (filtreLivreur.IsChecked == true)
        //            {
        //                if (villeFiltre.Text != "-Ville-")
        //                { livreurs.ForEach(livreur => PersonneFiltre(personnesAffiche, livreur, p => p.Ville == villeFiltre.Text)); }
        //                else livreurs.ForEach(livreur => personnesAffiche.Add(livreur));
        //            }
        //            if (villeFiltre.Text != "-Ville-")
        //            { clients.ForEach(cli => PersonneFiltre(personnesAffiche, cli, p => p.Ville == villeFiltre.Text)); }
        //            else clients.ForEach(cli => personnesAffiche.Add(cli));
        //        }
        //        else if (villeFiltre.Text != "-Ville-")
        //        { clients.ForEach(cli => PersonneFiltre(personnesAffiche, cli, p => p.Ville == villeFiltre.Text && ((Client)p).CumulAchat >= ((Slider)filtreDyn.Children[1]).Value && ((Client)p).CumulAchat <= ((Slider)filtreDyn.Children[4]).Value)); }
        //        else clients.ForEach(cli => PersonneFiltre(personnesAffiche, cli, p => ((Client)p).CumulAchat >= ((Slider)filtreDyn.Children[1]).Value && ((Client)p).CumulAchat <= ((Slider)filtreDyn.Children[4]).Value));
        //    }
        //    else if (filtreCommis.IsChecked == true)
        //    {
        //        if (filtreDyn.Children.Count == 0)
        //        {
        //            if (filtreLivreur.IsChecked == true)
        //            {
        //                if (villeFiltre.Text != "-Ville-")
        //                { livreurs.ForEach(livreur => PersonneFiltre(personnesAffiche, livreur, p => p.Ville == villeFiltre.Text)); }
        //                else livreurs.ForEach(livreur => personnesAffiche.Add(livreur));
        //            }
        //            if (villeFiltre.Text != "-Ville-")
        //            { commis.ForEach(com => PersonneFiltre(personnesAffiche, com, p => p.Ville == villeFiltre.Text)); }
        //            else commis.ForEach(com => personnesAffiche.Add(com));
        //        }
        //        else if (DateTime.TryParse(((TextBox)filtreDyn.Children[1]).Text, out DateTime debut) && DateTime.TryParse(((TextBox)filtreDyn.Children[3]).Text, out DateTime fin))
        //        {
        //            if (villeFiltre.Text != "-Ville-")
        //            { commis.ForEach(com => PersonneFiltre(personnesAffiche, com, p => p.Ville == villeFiltre.Text && ((Commis)p).Embauche.CompareTo(debut) < 0 && ((Commis)p).Embauche.CompareTo(fin) > 0)); }
        //            else commis.ForEach(com => PersonneFiltre(personnesAffiche, com, p => ((Commis)p).Embauche.CompareTo(debut) > 0 && ((Commis)p).Embauche.CompareTo(fin) < 0));
        //        }
        //        else if (villeFiltre.Text != "-Ville-")
        //        { commis.ForEach(com => PersonneFiltre(personnesAffiche, com, p => p.Ville == villeFiltre.Text)); }
        //        else commis.ForEach(com => personnesAffiche.Add(com));
        //    }
        //    else if (filtreLivreur.IsChecked == true)
        //    {
        //        if (filtreDyn.Children.Count == 0)
        //        {
        //            if (villeFiltre.Text != "-Ville-")
        //            { livreurs.ForEach(livreur => PersonneFiltre(personnesAffiche, livreur, p => p.Ville == villeFiltre.Text)); }
        //            else livreurs.ForEach(livreur => personnesAffiche.Add(livreur));
        //        }
        //        else
        //        {
        //            if (villeFiltre.Text != "-Ville-")
        //            { livreurs.ForEach(livreur => PersonneFiltre(personnesAffiche, livreur, p => p.Ville == villeFiltre.Text && ((Livreur)livreur).Transport == ((ComboBox)filtreDyn.Children[1]).Text)); }
        //            else if (((ComboBox)filtreDyn.Children[1]).Text != "-Transport-") livreurs.ForEach(livreur => PersonneFiltre(personnesAffiche, livreur, p => ((Livreur)livreur).Transport == ((ComboBox)filtreDyn.Children[1]).Text));
        //            else livreurs.ForEach(livreur => personnesAffiche.Add(livreur));
        //        }
        //    }
        //    Client_Unchecked(null, null);
        //    Commis_Unchecked(null, null);
        //    Livreur_Unchecked(null, null);
        //    Trier(null, null);
        //    spFiltre.Visibility = Visibility.Hidden;
        //}
        #endregion
        #endregion

        #region Tri et Recherche Client/Effectif
        /// <summary>
        /// Trie l'ensemble des personnes à afficher selon un critère
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Trier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listboxEffectifClient != null)
            {
                switch (cbxTriPersonne.SelectedIndex)
                {
                    case 1: //Tri par nom (alphabétique)
                        personnesAffiche.Sort((p1, p2) => p1.Nom.CompareTo(p2.Nom));
                        break;

                    case 2: //Tri par ville
                        personnesAffiche.Sort((p1, p2) => p1.Ville.CompareTo(p2.Ville));
                        break;

                    case 3: //Tri par nombre de commandes
                        personnesAffiche.Sort((p1, p2) => p2.NbCommandes.CompareTo(p1.NbCommandes));
                        break;

                    case 4: //Tri relatif à chaque classe, Sort() par défaut, chaque personne étant IComparable
                        personnesAffiche.Sort();
                        break;
                }
                ActualiserListBox(); //Mettre à jour l'affichage
            }
        }

        /// <summary>
        /// Affiche l'ensemble des personnes dont le texte entré dans la barre de recherche est contenu dans
        /// l'affichage de la personne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chercher_Click(object sender, RoutedEventArgs e)
        {
            if (chercher.Text.Length > 0)
            {
                personnesAffiche = new List<Personne>();
                #region Ancien code moche redondant
                //foreach (Client c in clients)
                //{ if (c.ToString().ToLower().Contains(chercher.Text.ToLower())) { personnesAffiche.Add(c); } }
                //foreach (Commis c in commis)
                //{ if (c.ToString().ToLower().Contains(chercher.Text.ToLower())) { personnesAffiche.Add(c); } }
                //foreach (Livreur l in livreurs)
                //{ if (l.ToString().ToLower().Contains(chercher.Text.ToLower())) { personnesAffiche.Add(l); } }
                #endregion

                //Pour chaque liste on vérifie si la recherche est contenue
                IList[] listes = { clients, commis, livreurs };
                foreach (IList personnes in listes)
                {  foreach (Personne p in personnes) { if (p.ToString().ToLower().Contains(chercher.Text.ToLower())) { personnesAffiche.Add(p); } } }
                ActualiserListBox();
            }
            //Si la barre de recherche est vide, on fait un filtre par défaut pour afficher tout le monde
            else { Filtrer_Click(null, null); }
        }
        #endregion
        #endregion

        #region Module Commandes
        #region Affichage Commandes
        /// <summary>
        /// Selectionne le bon Identifiable dans la combobox qui lui est réservée dans la modification d'une commande
        /// </summary>
        /// <param name="liste">liste des clients, des commis ou des livreurs de la pizzeria</param>
        /// <param name="cbx">Combobox du client, du commis ou du livreur associé à la commande</param>
        /// <param name="numero">Numéro du client, du commis ou du livreur associé à la commande</param>
        private void SelectionnerCbx(IList liste, ComboBox cbx, string numero)
        {
            cbx.SelectedIndex = liste.IndexOf(RetournerIdentifiable(numero)) + 1;
            cbx.IsEnabled = false;
            //On désactive la combobox : Elle ne peut pas être modifiée lorsque l'on actualise la commande
        }

        /// <summary>
        /// Passe le formulaire de création de commandes à un formulaire de modification de commandes, lorsque l'utilisateur
        /// sélectionne le ListBoxItem associé à la commande choisie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActualiserCommande_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //La commande est stockée dans le tag du ListBoxItem
            Commande commande = (Commande)((ListBoxItem)sender).Tag;
            ResetFormCommande();
            if (validerCommande.Visibility == Visibility.Visible && (commande != null && commande.Numero != noCommande.Text))
            {
                //On actualise l'interface
                infoCommande.Text = "Actualiser Commande";
                validerCommande.Visibility = Visibility.Hidden;
                actualiserCommande.Visibility = Visibility.Visible;
                ChoixPersonnes(choixCommande);

                noCommande.Text = commande.Numero;
                string[] numeros = commande.NumerosAssocies.Split(';');

                //Pour chaque combobox du client, commis et livreur de la commande, on remplit les champs correctement
                IList[] listes = { clients, commis, livreurs };
                for (int i = 0; i < 3; i++) { SelectionnerCbx(listes[i], (ComboBox)choixCommande.Children[i], numeros[i]); }
                //On raffiche les choix des pizzas pour l'affichage, en les désactivant
                for (int i = 0; i < commande.Pizzas.Count; i++)
                {
                    if (i != 0) { BtnPlusProduits_Click(new Button() { Tag = "p" }, null); }
                    for (int j = 0; j < 2; j++)
                    {
                        ((ComboBox)((StackPanel)spPizzas.Children[i]).Children[j]).SelectedIndex = (j == 0 ? pizzas.IndexOfKey(commande.Pizzas[i].Nom) : commande.Pizzas[i].Taille) + 1;
                        ((ComboBox)((StackPanel)spPizzas.Children[i]).Children[j]).IsEnabled = false;
                    }
                }
                for (int i = 0; i < 2; i++) { ((Button)(boutonsPizza.Children[i])).IsEnabled = false; }
                //Pareil pour les boissons
                for (int i = 0; i < commande.Boissons.Count; i++)
                {
                    BtnPlusProduits_Click(new Button() { Tag = "b" }, null);
                    for (int j = 0; j < 2; j++)
                    {
                        ((ComboBox)((StackPanel)spBoissons.Children[i]).Children[j]).SelectedIndex = (j % 2 == 0 ? boissons.IndexOfKey(commande.Boissons[i].Nom) : commande.Boissons[i].Taille) + 1;
                        ((ComboBox)((StackPanel)spBoissons.Children[i]).Children[j]).IsEnabled = false;
                    }
                }
                for (int i = 0; i < 2; i++) { ((Button)(boutonsBoisson.Children[i])).IsEnabled = false; }
                
                //On associe la commande au bouton btnCmdHonore, pour l'étudier ailleurs
                btnCmdHonore.Tag = commande;
                modification.Visibility = Visibility.Visible;
                List<RadioButton> listeRb = new List<RadioButton>();
                foreach (RadioButton rb in rbEtat.Children) { listeRb.Add(rb); }
                actualiserCommande.IsEnabled = commande.Etat != 2;

                //Affichage de l'état de la commande selon l'état de la commande
                if (commande.Reussie) { listeRb[2].IsChecked = true; txtHonore.Text = "Commande honorée"; btnCmdHonore.IsEnabled = false; }
                else
                {
                    txtHonore.Text = "Commande non honorée";
                    if (commande.Etat == 1) { btnCmdHonore.IsEnabled = true; }
                    listeRb[commande.Etat].IsChecked = true;
                }
                //Une commande fermée ne peut passer ni en livraison, ni en préparation
                //Une commande en livraison ne peut pas passer en préparation
                for (int i = 0; i <= commande.Etat; i++) { listeRb[i].IsEnabled = false; }
            }
            else
            {
                infoCommande.Text = "Ajouter Commande";
                validerCommande.Visibility = Visibility.Visible;
                actualiserCommande.Visibility = Visibility.Hidden;
            }
        }
        #endregion

        #region Nouvelle Commande
        /// <summary>
        /// Met à jour les choix possibles des combobox des clients, commis et livreurs dans la création/modification d'une
        /// commande ou dans l'affichage des filtres
        /// </summary>
        /// <param name="sp">StackPanel dans lequel sont posés les 3 combobox étudiés, en Children</param>
        private void ChoixPersonnes(StackPanel sp)
        {
            IList[] listes = { clients, commis, livreurs }; int i = 0;
            foreach (ComboBox cbx in sp.Children)
            {
                while (cbx.Items.Count != 1) { cbx.Items.RemoveAt(1); }
                foreach (Personne p in listes[i++])
                {
                    //Un commis / livreur ne peut apparaître dans la combobox que s'il est sur place et que l'utilisateur est en train de créer une commande
                    //Dans le cas où l'utilisateur gère les filtres d'affichage ou modifie une commande, on met tous les noms/prénoms dans les combobox
                    if (p is Client || ((Salarie)p).Etat == 0 || sp.Tag.ToString() == "f" || validerCommande.Visibility == Visibility.Hidden)
                    { cbx.Items.Add(new ComboBoxItem() { Content = p.Nom + " " + p.Prenom, Tag = p }); }
                }
            }
            noCommande.Text = (commandes.Count + 1).ToString();

            #region Ancien code moche et redondant
            //ComboBox choixClient = (ComboBox)sp.Children[0];
            //while (choixClient.Items.Count != 1) { choixClient.Items.RemoveAt(1); }
            //foreach (Client client in clients)
            //{
            //    ComboBoxItem item = new ComboBoxItem();
            //    item.Content = client.Nom + " " + client.Prenom;
            //    item.Tag = client;
            //    choixClient.Items.Add(item);
            //}
            //ComboBox choixCommis = (ComboBox)sp.Children[1];
            //while (choixCommis.Items.Count != 1) { choixCommis.Items.RemoveAt(1); }
            //foreach (Commis com in commis)
            //{
            //    if (com.Etat == 0 || validerCommande.Visibility == Visibility.Hidden)
            //    {
            //        ComboBoxItem item = new ComboBoxItem();
            //        item.Content = com.Nom + " " + com.Prenom;
            //        item.Tag = com;
            //        choixCommis.Items.Add(item);
            //    }
            //}
            //ComboBox choixLivreur = (ComboBox)sp.Children[2];
            //while (choixLivreur.Items.Count != 1) { choixLivreur.Items.RemoveAt(1); }
            //foreach (Livreur livreur in livreurs)
            //{
            //    if (livreur.Etat == 0 || validerCommande.Visibility == Visibility.Hidden)
            //    {
            //        ComboBoxItem item = new ComboBoxItem();
            //        item.Content = livreur.Nom + " " + livreur.Prenom;
            //        item.Tag = livreur;
            //        choixLivreur.Items.Add(item);
            //    }
            //}
            #endregion
        }

        /// <summary>
        /// Ajoute tous les produits des listes des produits comme choix possible dans une combobox
        /// </summary>
        /// <param name="cbx">Combobox dans laquelle on souhaite proposer les différents produits</param>
        /// <param name="estPizza">Booléen définissant si la combobox doit contenir les pizzas ou les boissons</param>
        private void CbxProduit(ComboBox cbx, bool estPizza)
        {
            cbx.Items.Add(new ComboBoxItem() { Content = estPizza ? "Pizza n°" + txtQtPizzas.Text : "Boisson n°" + txtQtBoissons.Text, IsSelected = true });
            for (int i = 0; i < (estPizza ? pizzas : boissons).Count; i++)
            { cbx.Items.Add(new ComboBoxItem() { Content = (estPizza ? pizzas : boissons).Keys[i] } ); }
        }

        /// <summary>
        /// Ajoute les tailles disponibles des produits comme choix possible à une combobox
        /// </summary>
        /// <param name="cbx">Combobox dans laquelle on souhaite proposer les différentes tailles</param>
        /// <param name="estPizza">Booléen définissant si la combobox doit contenir les tailles des pizzas ou les volumes des  boissons</param>
        private void CbxTaille(ComboBox cbx, bool estPizza)
        {
            cbx.Items.Add(new ComboBoxItem() { Content = estPizza ? "-Taille-" : "-Volume-", IsSelected = true });
            string[] tailles = { estPizza ? "Petite" : "33cL", estPizza ? "Moyenne" : "50cL", estPizza ? "Grande" : "1L" };
            foreach(string taille in tailles)
            { cbx.Items.Add(new ComboBoxItem() { Content = taille } ); }
        }

        #region Ancien code moche et redondant
        //private void Plus_Pizza(object sender, RoutedEventArgs e)
        //{
        //    StackPanel sp = new StackPanel();
        //    sp.Orientation = Orientation.Horizontal;
        //    qtPizzas.Text = (Convert.ToInt32(qtPizzas.Text) + 1).ToString();
        //    ComboBox cbx = new ComboBox();
        //    cbx.Width = 75;
        //    cbx.Margin = new Thickness(5);
        //    CbxProduit(cbx);
        //    cbx.SelectionChanged += Produit_SelectionChanged;
        //    sp.Children.Add(cbx);
        //    cbx = new ComboBox();
        //    cbx.Width = 75;
        //    cbx.Margin = new Thickness(5);
        //    CbxTaille(cbx);
        //    cbx.SelectionChanged += Produit_SelectionChanged;
        //    sp.Children.Add(cbx);
        //    TextBlock txt = new TextBlock();
        //    txt.VerticalAlignment = VerticalAlignment.Center;
        //    sp.Children.Add(txt);
        //    choixPizzas.Children.Add(sp);
        //}

        //private void Plus_Boisson(object sender, RoutedEventArgs e)
        //{
        //    StackPanel sp = new StackPanel();
        //    sp.Orientation = Orientation.Horizontal;
        //    qtBoissons.Text = (Convert.ToInt32(qtBoissons.Text) + 1).ToString();
        //    ComboBox cbx = new ComboBox();
        //    cbx.Width = 90;
        //    cbx.Margin = new Thickness(5);
        //    CbxProduit(cbx, false);
        //    cbx.SelectionChanged += Produit_SelectionChanged;
        //    sp.Children.Add(cbx);
        //    cbx = new ComboBox();
        //    cbx.Width = 70;
        //    cbx.Margin = new Thickness(5);
        //    CbxTaille(cbx, false);
        //    cbx.SelectionChanged += Produit_SelectionChanged;
        //    sp.Children.Add(cbx);
        //    TextBlock txt = new TextBlock();
        //    txt.VerticalAlignment = VerticalAlignment.Center;
        //    sp.Children.Add(txt);
        //    choixBoissons.Children.Add(sp);
        //}

        //private void Moins_Pizza(object sender, RoutedEventArgs e)
        //{ 
        //    if (qtPizzas.Text != "1") 
        //    { 
        //        qtPizzas.Text = (Convert.ToInt32(qtPizzas.Text) - 1).ToString();
        //        choixPizzas.Children.RemoveAt(choixPizzas.Children.Count - 1);
        //    } 
        //}


        //private void Moins_Boisson(object sender, RoutedEventArgs e)
        //{
        //    if (qtBoissons.Text != "0")
        //    {
        //        qtBoissons.Text = (Convert.ToInt32(qtBoissons.Text) - 1).ToString();
        //        choixBoissons.Children.RemoveAt(choixBoissons.Children.Count - 1);
        //    }
        //}

        //private void Pizza_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    string pizza = ((ComboBoxItem)((ComboBox)((StackPanel)(((ComboBox)sender).Parent)).Children[0]).SelectedItem).Content.ToString();
        //    int taille = ((ComboBox)((StackPanel)(((ComboBox)sender).Parent)).Children[1]).SelectedIndex - 1;
        //    TextBlock txt = ((TextBlock)((StackPanel)(((ComboBox)sender).Parent)).Children[2]);
        //    if (!(pizza.Contains("Pizza") || taille == -1))
        //    { txt.Text = (pizzas[pizza])[taille] + " euros"; }
        //    else { txt.Text = ""; }
        //}

        //private void Boissons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    string boisson = ((ComboBoxItem)((ComboBox)((StackPanel)(((ComboBox)sender).Parent)).Children[0]).SelectedItem).Content.ToString();
        //    int volume = ((ComboBox)((StackPanel)(((ComboBox)sender).Parent)).Children[1]).SelectedIndex - 1;
        //    TextBlock txt = ((TextBlock)((StackPanel)(((ComboBox)sender).Parent)).Children[2]);
        //    if (!(boisson.Contains("Boisson") || volume == -1))
        //    { txt.Text = (boissons[boisson])[volume] + " euros"; }
        //    else { txt.Text = ""; }
        //}
        #endregion

        /// <summary>
        /// Ajoute un choix de pizzas ou de boissons à la commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPlusProduits_Click(object sender, RoutedEventArgs e)
        {
            //Les combobox qui seront créées seront associés aux pizzas si le Tag du bouton vaut "p", aux boissons sinon
            bool estPizza = (sender as Button).Tag.ToString() == "p";
            StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
            TextBlock qt = estPizza ? txtQtPizzas : txtQtBoissons;
            qt.Text = (Convert.ToInt32(qt.Text) + 1).ToString();
            ComboBox cbx = new ComboBox() { Width = 75, Margin = new Thickness(5) };
            cbx.SelectionChanged += Produit_SelectionChanged;
            CbxProduit(cbx, estPizza);
            sp.Children.Add(cbx);
            cbx = new ComboBox() { Width = 75, Margin = new Thickness(5) };
            CbxTaille(cbx, estPizza);
            cbx.SelectionChanged += Produit_SelectionChanged;
            sp.Children.Add(cbx);
            sp.Children.Add(new TextBlock() { VerticalAlignment = VerticalAlignment.Center } );
            (estPizza ? spPizzas : spBoissons).Children.Add(sp);
        }

        /// <summary>
        /// Enlève un choix de produits à la commande si le minimum n'est pas atteint
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMoinsProduits_Click(object sender, RoutedEventArgs e)
        {
            //Booléen qui détermine si le bouton est associé aux pizzas (Tag = "p") ou aux boissons
            bool estPizza = ((Button)sender).Tag.ToString() == "p";
            TextBlock txtQtProduits = estPizza ? txtQtPizzas : txtQtBoissons;
            StackPanel sp = estPizza ? spPizzas : spBoissons;

            //Minimum 1 pizza et 0 boisson par commande
            if (txtQtProduits.Text != (estPizza ? "1" : "0"))
            {
                txtQtProduits.Text = (Convert.ToInt32(txtQtProduits.Text) - 1).ToString();
                sp.Children.RemoveAt(sp.Children.Count - 1);
            }
        }

        /// <summary>
        /// Affiche le prix du produit si celui-ci est sélectionné ainsi que sa taille
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Produit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).Items.Count > 1) //Pour ne pas causer d'erreur au lancement de la fenêtre
            {
                //Le stackpanel est celui dans lequel se trouve la combobox dont la selection vient de changer
                StackPanel sp = ((ComboBox)sender).Parent as StackPanel;

                //Le choix 0 des combobox étant soit "Pizza n°X" soit "Boisson n°X", on peut déterminer le type de produit
                //choisi dans la combobx avec la 1ere lettre du 1er choix, pour changer du Tag
                char type = ((ComboBoxItem)(sp.Children[0] as ComboBox).Items[0]).Content.ToString()[0];

                //Le produit correspond à l'item selectionné du 1er enfant du stackpanel (Combobox)
                string produit = ((ComboBoxItem)(sp.Children[0] as ComboBox).SelectedItem).Content.ToString();

                //La taille correspond à l'indice - 1, de l'item selectionné du 2e enfant du stackpanel (Combobox)
                int taille = (sp.Children[1] as ComboBox).SelectedIndex - 1;
                TextBlock txt = sp.Children[2] as TextBlock;

                //Si les éléments sélectionnés des 2 combobox ne sont pas ceux par défaut, on peut afficher le prix que l'on
                //récupère dans la sortedlist des produits pizzas ou boissons
                if (!(produit.Contains(type == 'P' ? "Pizza" : "Boisson") || taille == -1))
                { txt.Text = ((type == 'P' ? pizzas : boissons)[produit])[taille] + " euros"; }
                else { txt.Text = ""; }
            }
        }

        /// <summary>
        /// Valide la commande et l'ajoute à la liste des commandes si tous les champs ont été remplis correctement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValiderCommande_Click(object sender, RoutedEventArgs e)
        {
            //La commande peut être validée si l'ensemble des combobox d'ajout de commande ont une valeur différente de celles par défaut
            bool valider = true;
            foreach (ComboBox cbx in choixCommande.Children) { valider &= cbx.SelectedIndex != 0; }

            //Si les prix ne sont pas affichés, c'est que certaines combobx ont encore leur valeur par défaut
            foreach (StackPanel sp in spPizzas.Children) { valider &= ((TextBlock)sp.Children[2]).Text != ""; }
            foreach (StackPanel sp in spBoissons.Children) { valider &= ((TextBlock)sp.Children[2]).Text != ""; }
            if (valider)
            {
                //Le client, commis et livreur associé à la commande est stocké dans le Tag de la combobox qui correspond à son type
                List<Personne> personnes = new List<Personne>();
                foreach(ComboBox cbx in choixCommande.Children) { personnes.Add((cbx.SelectedItem as ComboBoxItem).Tag as Personne); }
                
                //Ajout des produits
                List<Produit> produits = new List<Produit>();
                foreach (StackPanel sp in spPizzas.Children)
                {
                    string nom = ((ComboBox)(sp.Children[0])).Text;
                    int taille = ((ComboBox)(sp.Children[1])).SelectedIndex - 1;
                    produits.Add(new Pizza(nom, taille));
                }
                foreach (StackPanel sp in spBoissons.Children)
                {
                    string nom = ((ComboBox)(sp.Children[0])).Text;
                    int volume = ((ComboBox)(sp.Children[1])).SelectedIndex - 1;
                    produits.Add(new Boisson(nom, volume));
                }
                Commande commande = new Commande(noCommande.Text, (Client)personnes[0], (Commis)personnes[1], (Livreur)personnes[2], produits);
                commandes.Add(commande);

                //On actualise la valeur du slider prixMax dans les filtre des commandes
                prixMax.Value = Commande.PlusChere;
                //On informe l'utilisateur que toutes les données n'ont pas été enregistrées (carré rouge), et on actualise
                rectSave.Fill = Brushes.Red;
                Actualiser();
                ResetFormCommande();
                MessageBox.Show("La commande n°" + commandes.Count + " est en préparation. Prix total : " + commande.CalculerPrix() + " euros");
            }
            else { MessageBox.Show("Tous les champs n'ont pas été remplis. La commande n'a pas pu être ajoutée"); }
        }

        /// <summary>
        /// Remet par défaut le formulaire de création des commandes
        /// </summary>
        private void ResetFormCommande()
        {
            //Activer l'édition des contrôles, sélectionner les valeurs par défaut
            foreach(ComboBox cbx in choixCommande.Children) { cbx.SelectedIndex = 0; cbx.IsEnabled = true; }
            for (int i = 0; i < 2; i++)
            {
                ((ComboBox)((StackPanel)(spPizzas.Children[0])).Children[i]).SelectedIndex = 0;
                ((ComboBox)((StackPanel)(spPizzas.Children[0])).Children[i]).IsEnabled = true;
                ((Button)(boutonsPizza.Children[i])).IsEnabled = true;
                ((Button)(boutonsBoisson.Children[i])).IsEnabled = true;
            }
            //Ne laisser qu'un seul choix de pizzas pour la prochaine commande, et 0 de boissons par défaut
            while (spPizzas.Children.Count > 1) { spPizzas.Children.RemoveAt(1); }
            spBoissons.Children.Clear();
            txtQtPizzas.Text = "1"; txtQtBoissons.Text = "0";

            //Le numéro de commande correspond à son indice de création + 1
            noCommande.Text = (commandes.Count + 1).ToString();
            modification.Visibility = Visibility.Hidden;
            btnCmdHonore.IsEnabled = false;
            foreach(RadioButton rb in rbEtat.Children) { rb.IsEnabled = true; }
        }
        
        /// <summary>
        /// Met à jour la commande lorsque l'utilisateur clique sur le bouton Actualiser, quand il modifie une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActualiserCommande_Click(object sender, RoutedEventArgs e)
        {
            //La commande est stockée dans le Tag du bouton indiquant si la commande a été honorée
            Commande commande = (Commande)btnCmdHonore.Tag;
            string[] numeros = commande.NumerosAssocies.Split(';');
            
            //Si la commande a été honorée, la commande est marquée comme réussie et fermée, le chiffre d'affaires de la pizzeria est
            //actualisé, le nombre des commandes effectuées par le client, gérées par le commis et livrées par le livreur est
            //augmenté et le livreur revient sur place
            if (txtHonore.Text == "Commande honorée")
            {
                commande.Reussie = true;
                commande.Etat = 2;
                for (int i = 0; i < 3; i++)
                {
                    Personne p = (Personne)RetournerIdentifiable(numeros[i]); p.NbCommandes++;
                    if (i == 0) { ((Client)p).CumulAchat += commande.CalculerPrix(); }
                    else { ((Salarie)p).Etat = (p is Livreur && commandes.Find(c => c.Etat == 0 && p.Numero == c.NumerosAssocies.Split(';')[2]) != null) ? 2 : 0; }
                    //On ne modifie pas l'état d'un livreur qui est toujours en livraison
                }
                CalculerCA();
                MessageBox.Show("La commande a été honorée et fermée");
            }
            else
            {
                //Dans le cas contraire, l'état de la commande varie en fonction de l'état actualisé
                int i = 0;
                foreach (RadioButton rb in rbEtat.Children)
                { if (rb.IsChecked == true) { commande.Etat = i; } i++; }
                if (commande.Etat == 1) 
                //Si la commande passe à "en livraison", le livreur passe à l'état "Sur la route" (2)
                //Il ne sera plus disponible pour prendre d'autres commandes jusqu'à ce que celle-ci soit fermée
                {
                    ((Livreur)RetournerIdentifiable(numeros[2])).Etat = 2;
                    MessageBox.Show("La commande est en livraison");
                }
                else if (commande.Etat == 2) //Si la commande est fermée sans être honorée, c'est fini, poubelle
                {
                    for (int j = 1; j <= 2; j++) { ((Salarie)RetournerIdentifiable(numeros[j])).Etat = 0; }
                    MessageBox.Show("La commande n'a pas pu être honorée et a été fermée");
                }
            }
            //Actualiser, carré rouge, etc.
            rectSave.Fill = Brushes.Red;
            validerCommande.Visibility = Visibility.Visible;
            actualiserCommande.Visibility = Visibility.Hidden;
            Actualiser();
            ResetFormCommande();
        }
        #endregion

        #region Filtre Commandes
        /// <summary>
        /// Affiche l'interface de choix des filtres des commandes, ou la cache, lorsque l'utilisateur clique sur le bouton des filtres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AffichageFiltresCommandes_Click(object sender, RoutedEventArgs e)
        { 
            spFiltre2.Visibility = spFiltre2.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
            
            //Réglages filtres par défaut
            debutCommande.MouseDoubleClick += (s, r) => debutCommande.Clear();
            finCommande.MouseDoubleClick += (s, r) => finCommande.Clear();
            foreach (ComboBox cbx in choixFiltreCommande.Children) { cbx.SelectedIndex = 0; }
            debutCommande.Text = "jj/mm/aaaa"; finCommande.Text = "jj/mm/aaaa";
            prixMin.Value = 0; prixMax.Value = Commande.PlusChere;
            cbxEtat.SelectedIndex = 0; chkValide.IsChecked = null; ChkValide_Click(null, null);
        }

        /// <summary>
        /// Met à jour le contenu de la checkBox à 3 états dans l'interface des filtres de l'affichage des commandes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkValide_Click(object sender, RoutedEventArgs e)
        {
            switch (chkValide.IsChecked)
            {
                case null:
                    chkValide.Content = "Toutes";
                    break;

                case true:
                    chkValide.Content = "Honorées";
                    break;

                case false:
                    chkValide.Content = "Non honorées";
                    break;
            }
        }

        /// <summary>
        /// Filtre les commandes selon les critères définis préalablement pas l'utilisateur avant qu'il ne clique sur le bouton Filtrer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FiltrerCommandes_Click(object sender, RoutedEventArgs e)
        {
            commandesAffiche.Clear();
            ComboBox cbxClient = (ComboBox)choixFiltreCommande.Children[0];
            ComboBox cbxCommis = (ComboBox)choixFiltreCommande.Children[1];
            ComboBox cbxLivreur = (ComboBox)choixFiltreCommande.Children[2];

            //Pour la liste des commandes, on récupère toutes les commandes dont le client, le commis ou le livreur
            //choisi dans les combobox, correspond
            foreach(Commande commande in commandes)
            {
                string[] numeros = commande.NumerosAssocies.Split(';');
                bool fait = false;
                if (cbxClient.Text != "-Client-")
                {
                    Client client = (Client)RetournerIdentifiable(numeros[0]);
                    if (client == (Client)((ComboBoxItem)cbxClient.SelectedItem).Tag)
                    { commandesAffiche.Add(commande); fait = true; }
                }
                if (cbxCommis.Text != "-Commis-" && !fait)
                {
                    Commis com = (Commis)RetournerIdentifiable(numeros[1]);
                    if (com == (Commis)((ComboBoxItem)cbxCommis.SelectedItem).Tag)
                    { commandesAffiche.Add(commande); fait = true; }
                }
                if (cbxLivreur.Text != "-Livreur-" && !fait)
                {
                    Livreur livreur = (Livreur)RetournerIdentifiable(numeros[2]);
                    if (livreur == (Livreur)((ComboBoxItem)cbxLivreur.SelectedItem).Tag)
                    { commandesAffiche.Add(commande); }
                }
            }
            //Si aucune des combobox n'était sélectionnée, on peut ajouter l'ensemble des commandes à la liste des commandes
            if (commandesAffiche.Count == 0)
            { foreach (Commande commande in commandes) { commandesAffiche.Add(commande); } }

            //Pour chaque commande de la liste des commandes à afficher, si son prix n'est pas dans l'intervalle, on la retire
            foreach (Commande commande in commandes)
            { if (commande.CalculerPrix() < prixMin.Value || commande.CalculerPrix() > prixMax.Value) { commandesAffiche.Remove(commande); } }
            
            //Pareil si la date, entrée correctement, n'est pas comprise dans l'intervalle
            if (DateTime.TryParse(debutCommande.Text, out DateTime debut) && DateTime.TryParse(finCommande.Text, out DateTime fin))
            {
                foreach(Commande commande in commandes)
                { if (commande.Date.CompareTo(debut) < 0 || commande.Date.CompareTo(fin) > 0) { commandesAffiche.Remove(commande); } }
            }

            //Pareil si l'état de la commande est différent de celui choisit
            if (cbxEtat.SelectedIndex != 0)
            {
                foreach (Commande commande in commandes)
                { if (commande.Etat != cbxEtat.SelectedIndex - 1) { commandesAffiche.Remove(commande); } }
            }

            //Et de même si la commande est réussie ou non, selon la valeur de la checkbox à 3 états
            if (chkValide.IsChecked != null)
            {
                foreach (Commande commande in commandes)
                { if (chkValide.IsChecked != commande.Reussie) commandesAffiche.Remove(commande); }
            }
            //On cache ensuite, et on trie pour actualiser
            spFiltre2.Visibility = Visibility.Hidden;
            CbxTriCommande_SelectionChanged(null, null);
        }
        #endregion

        #region Trier et Chercher Commandes
        /// <summary>
        /// Recherche une commande dont le texte cherché est contenu dans l'affichage de la commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChercherCommande_Click(object sender, RoutedEventArgs e)
        {
            commandesAffiche.Clear();
            foreach (Commande commande in commandes)
            { if (commande.ToString().ToLower().Contains(chercherCommande.Text.ToLower())) { commandesAffiche.Add(commande); } }
            ActualiserListBox();
        }

        /// <summary>
        /// Trie les commandes selon le critère défini par l'utilisateur, lorsque la sélection du combobox
        /// associé change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxTriCommande_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listboxCommande != null)
            {
                switch (choixTriCommande.SelectedIndex)
                {
                    case 1: //Tri par numéro
                        commandesAffiche.Sort((c1, c2) => c1.Numero.CompareTo(c2.Numero));
                        break;

                    case 2: //Tri par commande récente
                        commandesAffiche.Sort((c1, c2) => c2.Date.CompareTo(c1.Date));
                        break;

                    case 3: //Tri par prix décroissant
                        commandesAffiche.Sort((c1, c2) => c2.CalculerPrix().CompareTo(c1.CalculerPrix()));
                        break;

                    case 4: //Tri par états des commandes
                        commandesAffiche.Sort();
                        break;
                }
                ActualiserListBox();
            }
        }
        #endregion
        #endregion

        #region Module Statistiques
        /// <summary>
        /// Affiche ou cache le bouton de sauvegarde des données csv selon le tabcontrolitem selectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (spSave != null)
            {
                if (tabControl.SelectedIndex == 0 || tabControl.SelectedIndex == 1) { spSave.Visibility = Visibility.Visible; }
                else { spSave.Visibility = Visibility.Hidden; }
            }
        }

        /// <summary>
        /// Affiche les statistiques du numbre de commandes de la liste des personnes sélectionnées dans la
        /// combobox par l'utilisateur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxStatNb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IList[] listes = { clients, commis, livreurs };
            Statistiques(listes[cbxStatNb.SelectedIndex], nomStatNb, rectStatNb, p => ((Personne)p).NbCommandes, Personne.MaxCommandes, Brushes.Green, moyNb);
        }

        /// <summary>
        /// Effectue les statistiques d'une liste selon la liste, l'emplacement, la propriété à étudier, et d'autres paramètres
        /// </summary>
        /// <param name="liste">Liste dont on souhaite afficher les statistiques</param>
        /// <param name="spNom">stackpanel dans lequel afficher les identités des éléments de la liste</param>
        /// <param name="spRect">stackpanel dans lequel afficher les rectangles pour visualiser les statistiques</param>
        /// <param name="prop">Délégation retournant la propriété souhaitée en paramètre</param>
        /// <param name="max">Element maximum des propriétés possibles, pour avoir un rectange d'une longueur cohérente</param>
        /// <param name="couleur">Couleur que l'on souhaite pour les rectangles de cette statistique</param>
        /// <param name="txtMoy">Textblock affichant la moyenne de cette statistique</param>
        private void Statistiques(IList liste, StackPanel spNom, StackPanel spRect, Propriete prop, double max, SolidColorBrush couleur, TextBlock txtMoy)
        {
            spNom.Children.Clear();
            spRect.Children.Clear();
            double moy = 0;
            foreach (IIdentifiable id in liste)
            {
                moy += prop(id);
                spNom.Children.Add(new TextBlock() { Height = 30, Margin = new Thickness(5), Text = id is Personne ? ((Personne)id).Nom + "\n" + ((Personne)id).Prenom : id.Numero });
                spNom.Children.Add(new Rectangle() { Height = 1, Width = 150, Fill = Brushes.Black });
                Grid grid = new Grid() { HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(5, 10, 0, 10) };
                grid.Children.Add(new Rectangle() { Fill = couleur, Height = 20, Width = (150 * prop(id)) / max }); 
                grid.Children.Add(new TextBlock() { Text = "" + prop(id), HorizontalAlignment = HorizontalAlignment.Center, FontWeight = FontWeight.FromOpenTypeWeight(900) });
                spRect.Children.Add(grid); 
                spRect.Children.Add(new Rectangle() { Height = 1, Width = 350, Fill = Brushes.Black, VerticalAlignment = VerticalAlignment.Bottom });
            }
            moy = Math.Round(moy / liste.Count, 2);
            txtMoy.Text = "" + moy;
        }
        #endregion
    }
}