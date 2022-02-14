using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mixmo
{
    /// <summary>
    /// Classe principale du déroulement du jeu avec le main.
    /// Les poids des lettres sont basées sur celles du SCRABBLE, pour que le classement des joueurs varie plus facilement
    /// </summary>
    public class Jeu
    {
        #region Champs
        Dictionnaire dico;
        List<Joueur> joueurs;
        Lettres pioche;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur du jeu
        /// </summary>
        /// <param name="d">Dictionnaire</param>
        /// <param name="j">Liste des joueurs</param>
        /// <param name="p">Lettres de la pioche</param>
        public Jeu(Dictionnaire d, List<Joueur> j, Lettres p)
        {
            this.dico = d;
            this.joueurs = j;
            this.pioche = p;
        }
        #endregion

        #region Méthodes
        /// <summary>
        /// Vérifie si un nom est déjà pris par un autre joueur
        /// </summary>
        /// <param name="nom">Nom à tester</param>
        /// <returns>True si le nom est déjà pris, false sinon</returns>
        public bool NomPris(string nom)
        {
            bool pris = false;
            for (int i = 0; i < joueurs.Count; i++)
            {
                if (joueurs[i].Nom.ToUpper() == nom) pris = true;
            }
            return pris;
        }

        /// <summary>
        /// Trie les positions des joueurs par insertion selon leur score
        /// </summary>
        /// <param name="classementJoueurs">Liste de classement des joueur</param>
        public void TriScore(List<Joueur> classementJoueurs)
        {
            for (int i = 1; i < classementJoueurs.Count; i++)
            {
                //On fait un tri par insertion classique
                Joueur joueur = classementJoueurs[i];
                int j = i;
                while (j > 0 && joueur.Score > classementJoueurs[j - 1].Score)
                {
                    classementJoueurs[j] = classementJoueurs[j - 1];
                    j--;
                }
                classementJoueurs[j] = joueur;
            }
        }

        /// <summary>
        /// Calcule le score final en ajoutant les scores bonus
        /// </summary>
        public void ScoreFinal()
        {
            for (int i = 0; i < joueurs.Count; i++)
            {
                joueurs[i].ScoreBonus();
            }
        }

        /// <summary>
        /// Crée un classement des joueurs selon leurs positions en prenant en compte les égalités
        /// </summary>
        /// <returns>Liste du classement des joueurs</returns>
        public List<Joueur> Classement()
        {
            //On crée une nouvelle liste de joueurs pour ne pas interférer avec la liste des joueurs du jeu
            List<Joueur> classementJoueurs = new List<Joueur>();
            for (int i = 0; i < joueurs.Count; i++)
            {
                Joueur joueur = new Joueur(joueurs[i]);
                classementJoueurs.Add(joueur);
            }
            //On trie les joueurs selon leur position
            TriScore(classementJoueurs);
            classementJoueurs[0].Position = 1;
            //Le premier joueur est en position 1
            for (int i = 1; i < classementJoueurs.Count; i++)
            {
                //Puis pour chaque joueur, si son score est égal au précédent, sa position est la même, sinon il est en position suivante
                if (classementJoueurs[i].Score == classementJoueurs[i - 1].Score)
                {
                    classementJoueurs[i].Position = classementJoueurs[i - 1].Position;
                }
                else
                {
                    classementJoueurs[i].Position = classementJoueurs[i - 1].Position + 1;
                }
            }
            return classementJoueurs;
        }

        /// <summary>
        /// Vérifie si la pioche est vide et qu'un joueur au moins n'a plus de lettres
        /// </summary>
        /// <returns>True si c'est le cas, false sinon</returns>
        public bool PlusDeLettres()
        {
            bool vide = false;
            if (pioche.QuantiteTotale == 0)
            {
                for (int i = 0; i < joueurs.Count; i++)
                {
                    if (joueurs[i].LettresJoueur.QuantiteTotale == 0) vide = true;
                }
            }
            return vide;
        }

        /// <summary>
        /// Décrit le jeu dans une chaîne de caractères
        /// </summary>
        /// <returns>Chaîne de caractère décrivant le jeu</returns>
        public string toString()
        {

            string result = dico.toString();
            result += pioche.toString();
            result += joueurs.Count + " joueurs :\n";
            for (int i = 0; i < joueurs.Count; i++)
            {
                result += "Joueur " + (i + 1) + " : " + joueurs[i].toString() + "\n";
            }
            result += "Classement :\n";
            List<Joueur> classement = Classement();
            for (int i = 0; i < joueurs.Count; i++)
            {
                result += "Position " + classement[i].Position + " : " + classement[i].Nom + ", avec " + classement[i].Score + " points\n";
            }
            return result;
        }
        #endregion

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Pour démarrer une nouvelle partie, appuyez sur n'importe quelle touche. Sinon, appuyez sur ECHAP pour quitter.");
                ConsoleKeyInfo fin = Console.ReadKey();
                if (fin.Key == ConsoleKey.Escape) break;
                Console.Clear();
                Console.WriteLine("\tMIXMO - Nouvelle partie");

                #region Initialisation jeu
                Random r = new Random();
                Dictionnaire dictionnaire = new Dictionnaire("Dictionnaire", "MotsPossibles1.txt");
                List<Joueur> joueurs = new List<Joueur>();
                Lettres pioche = new Lettres("pioche", "Lettre.txt");
                //On crée le jeu à partir de ces objets
                Jeu jeu = new Jeu(dictionnaire, joueurs, pioche);
                #endregion

                #region InitialisationPartie
                int nbJoueurs = 0;
                int taille = 0;
                while (taille < 10 || taille > 100)
                {
                    Console.Write("Quelle taille voulez-vous définir pour chaque grille (carrée) ? (entre 10 et 100) : ");
                    int.TryParse(Console.ReadLine(), out taille);
                }
                while (nbJoueurs < 1 || nbJoueurs > 10)
                {
                    Console.Write("Combien de joueurs êtes-vous ? (de 1 à 10 joueurs) : ");
                    int.TryParse(Console.ReadLine(), out nbJoueurs);
                }

                for (int i = 0; i < nbJoueurs; i++)
                {
                    string nom = "";
                    Console.Write("Nom du joueur " + (i + 1) + " : ");
                    nom = Console.ReadLine();
                    //On fait en sorte que 2 joueurs n'aient pas le même nom
                    while (nom == "" || jeu.NomPris(nom))
                    {
                        Console.WriteLine("Le nom " + nom + " est vide ou déjà utilisé.");
                        Console.Write("Veuillez choisir un nom pour le joueur " + (i + 1) + " : ");
                        nom = Console.ReadLine();
                    }
                    Joueur joueur = new Joueur(nom, taille);
                    joueur.AddLettres(6, pioche, r);
                    joueurs.Add(joueur);
                }
                
                int tempsTour = 0;
                while (tempsTour < 1)
                {
                    Console.Write("Durée d'un tour en minutes : ");
                    int.TryParse(Console.ReadLine(), out tempsTour);
                }

                int tempsPartie = 0;
                while (tempsPartie < tempsTour)
                {
                    Console.Write("Durée de la partie en minutes (minimum " + tempsTour + " minute) : ");
                    int.TryParse(Console.ReadLine(), out tempsPartie);
                }
                #endregion

                Console.WriteLine("Veuillez appuyer sur n'importe quelle touche pour démarrer le jeu : ");
                Console.ReadKey();
                Console.Clear();
                //Le début de la partie est initialisé à partir de ce moment là
                DateTime debut = DateTime.Now;
                bool continuerPartie = true;
                while (continuerPartie)
                {
                    //Le début du tour est initialisé au début de la boucle de la partie
                    DateTime debutTour = DateTime.Now;
                    bool continuerTour = true;
                    while (continuerTour)
                    {
                        Console.WriteLine();
                        Console.WriteLine(jeu.toString());
                        int num = 0;
                        if (nbJoueurs == 1) num = 1;
                        else
                        {
                            Console.WriteLine("Quel joueur souhaite ajouter un mot ?");
                            while (num < 1 || num > joueurs.Count)
                            {
                                Console.Write("Veuillez entrer votre numéro. Joueur : ");
                                int.TryParse(Console.ReadLine(), out num);
                            }
                        }
                        Joueur joueur = joueurs[num - 1];

                        Console.WriteLine();
                        //On propose au joueur de changer la valeur de son joker s'il le souhaite
                        if (joueur.Possede("$"))
                        {
                            char j = '$';
                            Lettre joker = new Lettre(j, 0);
                            int positionJoker = joueur.LettresJoueur.Indice(joker);
                            Console.WriteLine("Indiquez la valeur que vous souhaitez donner à votre Joker. Un seul changement possible.");
                            Console.Write("Pour changer plus tard, entrez \"$\" ou tout autre caractère non compris dans l'alphabet : ");
                            char.TryParse(Console.ReadLine().ToUpper(), out j);
                            joker.Symbole = j;

                            //On a besoin d'une novelle pioche temporaire pour vérifier que la lettre entrée appartienne bien à l'alphabet
                            //et initialiser le poids de la lettre
                            Lettres alphabet = new Lettres("alphabet", "Lettre.txt");
                            int jokerPioche = alphabet.Indice(joker);
                            if (jokerPioche == -1) joueur.LettresJoueur.ListeDeLettres[positionJoker].Symbole = '$';
                            else
                            {
                                //Ensuite on ajoute le symbole et le poids si tout fonctionne
                                joueur.LettresJoueur.ListeDeLettres[positionJoker].Symbole = j;
                                joueur.LettresJoueur.ListeDeLettres[positionJoker].Poids = alphabet.ListeDeLettres[jokerPioche].Poids;
                            }
                            if (joueur.LettresJoueur.ListeDeLettres[positionJoker].Symbole == '$')
                            {
                                Console.WriteLine("Le joker n'a pas été changé.");
                                Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer\n");
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.WriteLine("Vous possédez maintenant la lettre " + j);
                                Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer\n");
                                Console.ReadKey();
                                Console.WriteLine(joueur.toString());
                            }
                        }
                        //On vérifie que la grille du joueur est vide
                        if (joueur.GrilleVide())
                        {
                            Console.WriteLine(joueur.toString());
                            Console.Write("Veuillez entrer un mot : ");
                            string mot = Console.ReadLine().ToUpper();
                            if (!dictionnaire.RechDichoRecursif(mot))
                            {
                                Console.WriteLine("Le mot " + mot + " n'existe pas dans le dictionnaire des mots autorisés");
                                Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer\n");
                                Console.ReadKey();
                            }
                            else
                            {
                                //Que le joueur possède les mots
                                if (!joueur.Possede(mot))
                                {
                                    Console.WriteLine("Vous ne possédez pas les lettres nécessaires pour écrire " + mot);
                                    Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer\n");
                                    Console.ReadKey();
                                }
                                else
                                {
                                    //On lui demande où il souhaite placer ses mots
                                    int colonne = 0;
                                    int ligne = 0;
                                    string pos = "";
                                    Console.WriteLine("Voulez-vous placer votre mot à l'horizontal ou à la verticale ?");
                                    while (pos != "H" && pos != "V")
                                    {
                                        Console.Write("Veuillez répondre par H ou V : ");
                                        pos = Console.ReadLine().ToUpper();
                                    }
                                    bool horizontal = true;
                                    if (pos == "V") horizontal = false;
                                    while (ligne < 1 || ligne > joueur.Grille.GetLength(0))
                                    {
                                        Console.Write("A quelle ligne voulez-vous placer votre mot : ");
                                        int.TryParse(Console.ReadLine(), out ligne);
                                    }
                                    while (colonne < 1 || colonne > joueur.Grille.GetLength(1))
                                    {
                                        Console.Write("A quelle colonne voulez-vous placer votre mot : ");
                                        int.TryParse(Console.ReadLine(), out colonne);
                                    }
                                    //On vérifie que le mot rentre bien dans la grille
                                    if (!joueur.PeutPlacer(mot, ligne, colonne, horizontal))
                                    {
                                        Console.WriteLine(mot + " ne rentre pas dans la grille à position que vous lui avez indiqué.");
                                        Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer\n");
                                        Console.ReadKey();
                                    }
                                    else
                                    {
                                        //Si tout est bon, le joueur place le mot dans la grille
                                        joueur.PlacerMot(mot, ligne, colonne, horizontal);
                                        Console.WriteLine("Le mot " + mot + " a bien été placé");
                                        Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer\n");
                                        Console.ReadKey();
                                        Console.WriteLine(joueur.toString());
                                        Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer\n");
                                        Console.ReadKey();
                                    }
                                }
                            }
                        }
                        else
                        //Si la grille n'est pas vide, on fait quelques conditions supplémentaires
                        {
                            Console.WriteLine(joueur.toString());
                            Console.WriteLine("Voulez-vous récupérer les lettres de la grille dans votre pioche ?");
                            string reponse = "k";
                            //On propose au joueur de récuperer les lettres sur sa grille s'il le souhaite
                            while (reponse != "OUI" && reponse != "NON" && reponse != "")
                            //Pour dire NON le joueur peut aussi ne rien dire pour ne pas perdre trop de temps à chaque fois
                            {
                                Console.Write("Veuillez répondre par OUI ou NON (ou bien, appuyez simplement sur ENTREE pour dire non) : ");
                                reponse = Console.ReadLine().ToUpper();
                            }
                            if (reponse == "OUI")
                            {
                                joueur.RecupererLettres();
                                Console.WriteLine(joueur.toString());
                                Console.WriteLine("Vous avez récupéré toutes vos lettres.");
                                Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer\n");
                                Console.ReadKey();
                            }
                            else
                            //Si la réponse est non
                            {
                                Console.WriteLine(joueur.toString());
                                Console.Write("Veuillez entrer un mot : ");
                                string mot = Console.ReadLine().ToUpper();
                                //On vérifie que le mot est dans le dictionnaire
                                if (!dictionnaire.RechDichoRecursif(mot))
                                {
                                    Console.WriteLine("Le mot " + mot + " n'existe pas dans le dictionnaire des mots autorisés");
                                    Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer\n");
                                    Console.ReadKey();
                                }
                                else
                                {
                                    //On initialise la position du mot
                                    int colonne = 0;
                                    int ligne = 0;
                                    string pos = "";
                                    Console.WriteLine("Voulez-vous placer votre mot à l'horizontal ou à la verticale ?");
                                    while (pos != "H" && pos != "V")
                                    {
                                        Console.Write("Veuillez répondre par H ou V : ");
                                        pos = Console.ReadLine().ToUpper();
                                    }
                                    bool horizontal = true;
                                    if (pos == "V") horizontal = false;
                                    while (ligne < 1 || ligne > joueur.Grille.GetLength(0))
                                    {
                                        Console.Write("A quelle ligne voulez-vous placer votre mot : ");
                                        int.TryParse(Console.ReadLine(), out ligne);
                                    }
                                    while (colonne < 1 || colonne > joueur.Grille.GetLength(1))
                                    {
                                        Console.Write("A quelle colonne voulez-vous placer votre mot : ");
                                        int.TryParse(Console.ReadLine(), out colonne);
                                    }
                                    //Que le mot ne déborde pas dans la grille
                                    if (!joueur.PeutPlacer(mot, ligne, colonne, horizontal))
                                    {
                                        Console.WriteLine(mot + " ne rentre pas dans la grille à position que vous lui avez indiqué.");
                                        Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer\n");
                                        Console.ReadKey();
                                    }
                                    else
                                    {
                                        //On crée une copie du joueur pour ne pas avoir de problème si les prochaines vérifications sont fausses
                                        Joueur joueurTest = new Joueur(joueur);
                                        //On vérifie que le mot peut bien être posé s'il croise des mots
                                        if (!joueurTest.CroiserMot(mot, ligne - 1, colonne - 1, horizontal))
                                        {
                                            Console.WriteLine("Les mots ne se touchent pas ou se croisent mal");
                                            Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer\n");
                                            Console.ReadKey();
                                        }
                                        else
                                        {
                                            //Que le joueur possède les lettres nécessaires pour écrire le mot
                                            if (!joueurTest.Possede(mot))
                                            {
                                                Console.WriteLine("Vous ne possédez pas les lettres nécessaires pour écrire " + mot + " ici");
                                                Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer\n");
                                                Console.ReadKey();
                                            }
                                            else
                                            {
                                                //Que les nouveaux mots créés sont corrects
                                                if (!joueurTest.VerificationMots(dictionnaire, mot, ligne, colonne, horizontal))
                                                {
                                                    Console.WriteLine(joueurTest.toString());
                                                    Console.WriteLine("Le mot ne peut pas être placé car il crée certains mots non autorisés.");
                                                    Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer\n");
                                                    Console.ReadKey();
                                                }
                                                else
                                                {
                                                    //Si tous les tests précédents ont fonctionnés, le joueur redevient la copie du joueur
                                                    joueur = joueurTest;
                                                    joueurs[num - 1] = joueur;
                                                    Console.WriteLine("Le mot " + mot + " a bien été placé");
                                                    Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer\n");
                                                    Console.ReadKey();
                                                    Console.WriteLine(joueur.toString());
                                                    Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer\n");
                                                    Console.ReadKey();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //Si le joueur n'a plus de lettres, le tour finit
                        if (joueur.LettresJoueur.QuantiteTotale == 0)
                        {
                            Console.WriteLine(joueur.Nom + " dit \"MIXMO !\". Tout le monde pioche 2 lettres.");
                            Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer\n");
                            Console.ReadKey();
                            continuerTour = false;
                        }

                        //L'heure actuelle est soustraite à l'heure de début du tour
                        TimeSpan tour = DateTime.Now - debutTour;
                        //Si le temps est supérieur au temps pour un tour, on informe que le temps est écoulé et que tout le monde pioche 2 lettres
                        if (tour.Minutes >= tempsTour)
                        {
                            Console.WriteLine(tempsTour + "mn écoulé. Fin du tour. Tout le monde pioche 2 lettres.");
                            Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer\n");
                            Console.ReadKey();
                            continuerTour = false;
                        }
                    }

                    //L'heure actuelle est soustraite à l'heure de début du tour
                    TimeSpan tempsEcoule = DateTime.Now - debut;
                    //Si le temps est écoulé, on sort de la boucle
                    if (tempsEcoule.Minutes >= tempsPartie)
                    {
                        Console.WriteLine(tempsPartie + " minutes se sont écoulées");
                        continuerPartie = false;
                    }

                    //On vérifie s'il reste des lettres
                    if (jeu.PlusDeLettres())
                    {
                        Console.WriteLine("La poche est vide et un joueur n'a plus de lettres");
                        continuerPartie = false;
                    }

                    //Si la partie continue, on fait piocher 2 lettres à tout le monde
                    if (continuerPartie)
                    {
                        for (int i = 0; i < joueurs.Count; i++)
                        {
                            Console.WriteLine(joueurs[i].Nom + " reçoit 2 lettres");
                            joueurs[i].AddLettres(2, pioche, r);
                        }
                    }
                }
                Console.WriteLine("Partie terminée !");
                Console.WriteLine("Chaque mot de plus de 5 lettres trouvés offrent autant de points bonus que de lettres contenues dans le mot");
                jeu.ScoreFinal();
                Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer et afficher le classement final\n");
                Console.ReadKey();
                Console.WriteLine(jeu.toString());
            }
        }
    }
}
