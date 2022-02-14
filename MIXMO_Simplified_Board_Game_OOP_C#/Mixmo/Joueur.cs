using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mixmo
{
    /// <summary>
    /// Classe du joueur
    /// </summary>
    public class Joueur
    {
        #region Champs
        string nom;
        int score = 0;
        int position;
        Lettres lettresJoueur;
        List<string> motsTrouves = new List<string>();
        MotsCroises motsCroises;
        #endregion

        #region Constructeurs
        /// <summary>
        /// Constructeur du joueur
        /// </summary>
        /// <param name="nom">Nom du joueur</param>
        /// <param name="tailleGrille">Taille de la grille du joueur</param>
        public Joueur (string nom, int tailleGrille)
        {
            this.nom = nom;
            motsCroises = new MotsCroises(tailleGrille);
        }

        /// <summary>
        /// Constructeur du joueur par copie
        /// </summary>
        /// <param name="j">Joueur dont on souhaite copier les attributs</param>
        public Joueur (Joueur j)
        {
            nom = j.nom;
            score = j.score;
            Lettres lettres = new Lettres(j.lettresJoueur);
            lettresJoueur = lettres;
            motsTrouves = new List<string>();
            //On fait attention à ne pas copier l'adresse des objets mais bien les variables qu'ils contiennent
            for (int i = 0; i < j.motsTrouves.Count; i++)
            {
                motsTrouves.Add(j.motsTrouves[i]);
            }
            char[,] grille = new char[j.Grille.GetLength(0), j.Grille.GetLength(1)];
            for (int ligne = 0; ligne < grille.GetLength(0); ligne++)
            {
                for (int colonne = 0; colonne < grille.GetLength(1); colonne++)
                {
                    grille[ligne, colonne] = j.Grille[ligne, colonne];
                }
            }
            MotsCroises motsC = new MotsCroises(grille);
            motsCroises = motsC;
        }
        #endregion

        #region Attributs
        /// <summary>
        /// Attribut du nom du joueur
        /// </summary>
        public string Nom
        {
            get { return nom; }
        }

        /// <summary>
        /// Attribut du score du joueur
        /// </summary>
        public int Score
        {
            get { return score; }
        }

        /// <summary>
        /// Attribut de la position du joueur dans le classement avec possibilité d'écriture
        /// </summary>
        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Attribut de la liste des lettres du joueur
        /// </summary>
        public Lettres LettresJoueur
        {
            get { return lettresJoueur; }
        }

        /// <summary>
        /// Attribut de la grille de l'objet motsCroises du joueur, pour éviter de devoir appeler motsCroises chaque fois
        /// </summary>
        public char[,] Grille
        {
            get { return motsCroises.Grille; }
            set { motsCroises.Grille = value; }
        }
        #endregion

        #region Methodes
        /// <summary>
        /// Tire au hasard un nombre de lettres dans la pioche, entrés en paramètres
        /// </summary>
        /// <param name="nb">nombre de lettres à piocher</param>
        /// <param name="pioche">Ensemble des lettres constituant la pioche</param>
        /// <param name="r">Objet permettant de calculer des nombres aléatoires à l'aide de ses méthodes</param>
        /// <returns>True si l'action a pu se produire, false sinon</returns>
        public bool AddLettres(int nb, Lettres pioche, Random r)
        {
            bool valide = false;
            if (lettresJoueur == null) lettresJoueur = new Lettres(nom);
            if (pioche.ListeDeLettres.Count != 0)
            //On vérifie qu'il reste des lettres dans la pioche
            {
                if (pioche.QuantiteTotale < nb) nb = pioche.QuantiteTotale;
                //On vérifie également que le nombre de lettres restantes soit supérieure au nombre de lettres à piocher, 
                //sinon on change le nombre de lettres à piocher
                for (int i = 0; i < nb; i++)
                {
                    //On initialise un nombre aléatoire compris entre 0 et le nombre total de lettres restantes dans la pioche
                    int nombreLettres = r.Next(1, pioche.QuantiteTotale);
                    
                    //On cherche ensuite l'indice de la lettre de la n-ieme quantité totale des lettres de la pioche
                    //Pas besoin de faire de vérification, comme nombreLettres est compris entre 1 et QuantiteTotale,
                    //IndicePioche() ne retournera jamais -1
                    int indicePioche = pioche.IndicePioche(nombreLettres);

                    //On peut alors piocher la lettre
                    Lettre lettrePioche = pioche.piocherLettre(indicePioche);
                    int indiceLettreExistante = lettresJoueur.Indice(lettrePioche);
                    if (indiceLettreExistante == -1)
                    //Si elle n'a pas déjà été piochée par le joueur, on lui donne pour quantité 1 et on ajoute la
                    //lettre à la liste des lettres du joueur
                    {
                        lettrePioche.Quantite = 1;
                        lettresJoueur.ListeDeLettres.Add(lettrePioche);
                        lettresJoueur.QuantiteTotale++;
                    }
                    else
                    //Sinon on incrémente la quantité de la lettre déjà existante de la liste des lettres du joueur
                    {
                        lettresJoueur.ListeDeLettres[indiceLettreExistante].Quantite++;
                        lettresJoueur.QuantiteTotale++;
                    }
                    valide = true;
                }
            }
            return valide;
        }

        /// <summary>
        /// Ajoute un mot dans la liste des mots trouvés par le joueur
        /// </summary>
        /// <param name="mot">Mot trouvé par le joueur</param>
        public void Add(string mot)
        {
            if (!MotTrouve(mot)) motsTrouves.Add(mot);
            //On enlève directement les lettres du mot que possède le joueur
            OteLettres(mot);
        }

        /// <summary>
        /// Vérifie que le joueur possède les lettres nécessaires à l'écriture d'un mot
        /// </summary>
        /// <param name="mot"></param>
        /// <returns></returns>
        public bool Possede(string mot)
        {
            bool possession = true;
            //On crée une nouvelle liste de lettres par copie afin de ne pas changer les valeurs de l'instance des lettres actuelles du joueur
            Lettres liste = new Lettres(lettresJoueur);
            for (int i = 0; i < mot.Length && possession; i++)
            {
                Lettre lettreMot = new Lettre(mot[i], 0);
                int indiceLettre = liste.Indice(lettreMot);
                //On vérifie que le mot peut bien être ajouté
                if (indiceLettre == -1) possession = false;
                else
                {
                    //S'il peut être ajouté on décrémente la quantité de lettres de la liste, puis si elle atteint 0, on supprime la lettre
                    liste.ListeDeLettres[indiceLettre].Quantite--;
                    if (liste.ListeDeLettres[indiceLettre].Quantite == 0) liste.ListeDeLettres.RemoveAt(indiceLettre);
                }
            }
            return possession;
        }

        /// <summary>
        /// Retire les lettres du joueur d'un mot entré
        /// </summary>
        /// <param name="mot">Mot dont on souhaite retirer les lettres</param>
        public void OteLettres(string mot)
        {
            for (int i = 0; i < mot.Length; i++)
            {
                Lettre lettreMot = new Lettre(mot[i], 0);
                int indiceLettre = lettresJoueur.Indice(lettreMot);

                //On augmente le score en fonction du poids de la lettre
                score += lettresJoueur.ListeDeLettres[indiceLettre].Poids;
                //Puis on décrémente la quantité de chaque lettre du mot, en supprimant la lettre si sa quantité atteint 0
                lettresJoueur.ListeDeLettres[indiceLettre].Quantite--;
                lettresJoueur.QuantiteTotale--;
                if (lettresJoueur.ListeDeLettres[indiceLettre].Quantite == 0) lettresJoueur.ListeDeLettres.RemoveAt(indiceLettre);
            }
        }
        
        /// <summary>
        /// Place un mot entré sur la grille du joueur
        /// </summary>
        /// <param name="mot">Mot à placer</param>
        /// <param name="ligne">Ligne de la case de la grille du début du mot</param>
        /// <param name="colonne">Colonne de la case de la grille du début du mot</param>
        /// <param name="horizontal">Position du mot : true = horizontal, false = vertical</param>
        public void PlacerMot(string mot, int ligne, int colonne, bool horizontal)
        {
            if (!horizontal)
            {
                Add(mot);
                //Si le mot est vertical on ajoute les lettres sur la colonne en faisant varier la ligne
                for (int i = ligne; i < ligne + mot.Length; i++)
                {
                    Grille[i - 1, colonne - 1] = mot[i - ligne];
                }
            }
            else
            {
                //Sinon on ajoute les lettres sur la ligne en faisant varier la colonne
                Add(mot);
                for (int i = colonne; i < colonne + mot.Length; i++)
                {
                    Grille[ligne - 1, i - 1] = mot[i - colonne];
                }
            }
        }

        /// <summary>
        /// Vérifie qu'un mot peut être placé sur la grille, sans sortir des limites de la grille
        /// </summary>
        /// <param name="mot">Mot à placer</param>
        /// <param name="ligne">Ligne de la case de la grille du début du mot</param>
        /// <param name="colonne">Colonne de la case de la grille du début du mot</param>
        /// <param name="horizontal">Position du mot : true = horizontal, false = vertical</param>
        /// <returns>True si le mot peut être placé, false si le mot sort de la grille</returns>
        public bool PeutPlacer(string mot, int ligne, int colonne, bool horizontal)
        {
            bool placer = false;
            if (!horizontal)
            {
                if (ligne + mot.Length <= Grille.GetLength(0) + 1) placer = true;
            }
            else
            {
                if (colonne + mot.Length <= Grille.GetLength(1) + 1) placer = true;
            }
            return placer;
        }

        /// <summary>
        /// Vérifie qu'un mot à entrer touche un autre mot, et que deux lettres qui se croisent soient bien les mêmes, dans le cas d'une grille non vide
        /// </summary>
        /// <param name="mot">Mot que l'on souhaite placer</param>
        /// <param name="ligne">Ligne de la case de la grille du début du mot</param>
        /// <param name="colonne">Colonne de la case de la grille du début du mot</param>
        /// <param name="horizontal">Position du mot : true = horizontal, false = vertical</param>
        /// <returns></returns>
        public bool CroiserMot(string mot, int ligne, int colonne, bool horizontal)
        {
            bool croise = false;
            if (!horizontal)
                //Cas où le mot est vertical 
                //(j'ai confondu horizontal et vertical au début de ma programmation, et ait dû changer plein de choses, d'où la notation !horizontal)
            {
                //On vérifie dans un premier temps que le mot est voisin avec au moins un autre mot de la grille
                for (int i = ligne; i < ligne + mot.Length; i++)
                {
                    if (motsCroises.ToucheMots(i, colonne)) croise = true;
                }
                //Puis on vérifie en cas de croisement que les lettres se confondent bien
                for (int i = ligne; i < ligne + mot.Length; i++)
                {
                    if (Grille[i, colonne] != ' ')
                    {
                        if (mot[i - ligne] != Grille[i, colonne]) croise = false;
                        else
                        {
                            //Si les lettres se confondent, on offre une lettre temporaire au joueur, sans valeur pour ne pas avoir de problèmes
                            //lors de la vérification de la possession des lettres
                            Lettre ajout = new Lettre(Grille[i, colonne], 1, 0);
                            lettresJoueur.ListeDeLettres.Add(ajout);
                            lettresJoueur.QuantiteTotale++;
                        }
                    }
                }
            }
            else
            //Cas où le mot est vertical
            {
                //On vérifie dans un premier temps que le mot est voisin avec au moins un autre mot de la grille
                for (int i = colonne; i < colonne + mot.Length; i++)
                {
                    if (motsCroises.ToucheMots(ligne, i)) croise = true;
                }
                //Puis on vérifie en cas de croisement que les lettres se confondent bien
                for (int i = colonne; i < colonne + mot.Length; i++)
                {
                    if (Grille[ligne, i] != ' ')
                    {
                        if (mot[i - colonne] != Grille[ligne, i]) croise = false;
                        else
                        {
                            //Si les lettres se confondent, on offre une lettre temporaire au joueur, sans valeur pour ne pas avoir de problèmes
                            //lors de la vérification de la possession des lettres
                            Lettre ajout = new Lettre(Grille[ligne, i], 1, 0);
                            lettresJoueur.ListeDeLettres.Add(ajout);
                            lettresJoueur.QuantiteTotale++;
                        }
                    }
                }
            }
            return croise;
        }

        /// <summary>
        /// Vérifie si la grille d'un joueur est vide
        /// </summary>
        /// <returns>True si la grille est vide, false sinon</returns>
        public bool GrilleVide()
        {
            bool vide = true;
            for (int ligne = 0; ligne < Grille.GetLength(0); ligne++)
            {
                for (int colonne = 0; colonne < Grille.GetLength(0); colonne++)
                {
                    //La grille si toutes ses cases contiennent un espace
                    if (Grille[ligne, colonne] != ' ') vide = false;
                }
            }
            return vide;
        }

        /// <summary>
        /// Vérifie si un mot a déjà été trouvé par le joueur
        /// </summary>
        /// <param name="mot">Mot à vérifier</param>
        /// <returns></returns>
        public bool MotTrouve(string mot)
        {
            bool trouve = false;
            for (int i = 0; i < motsTrouves.Count; i++)
            {
                if (motsTrouves[i] == mot) trouve = true;
            }
            return trouve;
        }

        /// <summary>
        /// Enlève toutes les lettres de la grille d'un joueur et les remets dans sa liste de lettres
        /// </summary>
        public void RecupererLettres()
        {
            //On réinitialise le score, et la liste des mots trouvés
            score = 0;
            motsTrouves = new List<string>();
            for (int ligne = 0; ligne < Grille.GetLength(0); ligne++)
            {
                for (int colonne = 0; colonne < Grille.GetLength(1); colonne++)
                {
                    if (Grille[ligne, colonne] != ' ')
                    //Si la case n'est pas vide, on crée une nouvelle lettre qui contient son symbole
                    {
                        int poids = 0;
                        char symb = Grille[ligne, colonne];
                        Lettres pioche = new Lettres("Lettres", "..\\..\\..\\Mixmo\\bin\\Debug\\Lettre.txt");
                        //On crée une pioche temporaire juste pour redonner le poids de la lettre au joueur
                        //Long nom d'accès pour faire fonctionner ls tests unitaires
                        Lettre lettre = new Lettre(symb, poids);
                        int indiceLettrePioche = pioche.Indice(lettre);
                        lettre.Poids += pioche.ListeDeLettres[indiceLettrePioche].Poids; //On change le poids de la lettre
                        int indiceLettreJoueur = lettresJoueur.Indice(lettre);
                        if (indiceLettreJoueur == -1)
                        //Si le joueur ne la possède pas déjà, on lui donne pour quantité 1 et on ajoute la
                        //lettre à la liste des lettres du joueur
                        {
                            lettre.Quantite = 1;
                            lettresJoueur.ListeDeLettres.Add(lettre);
                            lettresJoueur.QuantiteTotale++;
                        }
                        else
                        //Sinon on incrémente la quantité de la lettre déjà existante de la liste des lettres du joueur
                        {
                            lettresJoueur.ListeDeLettres[indiceLettreJoueur].Quantite++;
                            lettresJoueur.QuantiteTotale++;
                        }
                        Grille[ligne, colonne] = ' ';
                    }
                }
            }
        }

        /// <summary>
        /// Vérifie que l'ensemble des mots contenus sur la grille horizontaux et verticaux une fois le nouveau mot placé
        /// appartiennent bien au dictionnaire 
        /// </summary>
        /// <param name="dico">Dictionnaire contenant les mots autorisés</param>
        /// <param name="mot">Mot à placer</param>
        /// <param name="ligne">Ligne de la case de la grille du début du mot</param>
        /// <param name="colonne">Colonne de la case de la grille du début du mot</param>
        /// <param name="horizontal">Position du mot : true = horizontal, false = vertical</param>
        /// <returns></returns>
        public bool VerificationMots(Dictionnaire dico, string mot, int ligne, int colonne, bool horizontal)
        {
            bool verif = true;
            //On vérifie que le mot peut être placé sans déborder dans la grille
            if (!PeutPlacer(mot, ligne, colonne, horizontal)) verif = false;
            else
            {
                //On réinitialise la liste des mots trouvés pour ne pas avoir 2 mots 
                //ex : ARBRE + S --> ARBRES
                //             E          E
                //on ne veut pas se retrouver avec ARBRE, ARBRES et SE dans les mots trouvés mais juste ARBRES et SE
                motsTrouves = new List<string>();
                //On place ensuite le mot dans la grille
                PlacerMot(mot, ligne, colonne, horizontal);
                //On parcourt une première fois ligne par ligne
                for (int i = 0; i < Grille.GetLength(0); i++)
                {
                    //Puis on initialise un mot que l'on vérifiera chaque fois qu'il dépasse 2 lettres et que la case suivante est vide
                    string motTest = "";
                    for (int j = 0; j < Grille.GetLength(1); j++)
                    {
                        if (Grille[i, j] != ' ') motTest += Grille[i, j];
                        else
                        {
                            //Si la taille du mot Test est supérieure à 1, on vérifie qu'il existe dans le dictionnaire
                            if (motTest.Length > 1)
                                if (dico.RechDichoRecursif(motTest))
                                {
                                    //Si c'est le cas on l'ajoute dans le dictionnaire
                                    if (!MotTrouve(motTest)) motsTrouves.Add(motTest);
                                }
                                else verif = false;
                            motTest = "";
                        }
                        //On refait à peu près la même chose si on atteint la dernière colonne, dans le cas où des mots sont placés en limite de grille
                        if (j == Grille.GetLength(1) - 1)
                        {
                            if (motTest.Length > 1)
                                if (dico.RechDichoRecursif(motTest))
                                {
                                    if (!MotTrouve(motTest)) motsTrouves.Add(motTest);
                                }
                                else verif = false;
                            motTest = "";
                        }
                    }
                }

                //On refait exactement la même chose en parcourant cette fois colonne par colonne
                for (int i = 0; i < Grille.GetLength(1); i++)
                {
                    string motTest = "";
                    for (int j = 0; j < Grille.GetLength(0); j++)
                    {
                        if (Grille[j, i] != ' ') motTest += Grille[j, i];
                        else
                        {
                            if (motTest.Length > 1)
                                if (dico.RechDichoRecursif(motTest))
                                {
                                    if (!MotTrouve(motTest)) motsTrouves.Add(motTest);
                                }
                                else verif = false;
                            motTest = "";
                        }
                        if (j == Grille.GetLength(1) - 1)
                        {
                            if (motTest.Length > 1)
                                if (dico.RechDichoRecursif(motTest))
                                {
                                    if (!MotTrouve(motTest)) motsTrouves.Add(motTest);
                                }
                                else verif = false;
                            motTest = "";
                        }
                    }
                }
            }
            return verif;
        }

        /// <summary>
        /// Ajoute un score bonus si les mots trouvés ont une taille supérieure à 5
        /// </summary>
        public void ScoreBonus()
        {
            for (int i = 0; i < motsTrouves.Count; i++)
            {
                if (motsTrouves[i].Length > 5) score += motsTrouves[i].Length;
            }
        }

        /// <summary>
        /// Met les attributs du joueur sous forme de chaine de caractères
        /// </summary>
        /// <returns>Chaine de caractère avec les attributs du joueur</returns>
        public string toString()
        {
            string result = nom + "\n";
            result += motsCroises.toString();
            result += lettresJoueur.toString();
            result += "Score : " + this.score + "\n";
            if (motsTrouves.Count == 0)
            {
                result += "Aucun mot trouvé\n";
            }
            else
            {
                result += "\nMots trouvés : ";
                for (int i = 0; i < motsTrouves.Count; i++)
                {
                    result += motsTrouves[i] + " ";
                }
                result += "\n";
            }
            return result;
        }
        #endregion
    }
}
