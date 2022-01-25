using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EsilvGui;


namespace TD08_09_probleme
{
    class Program
    {
        /// <summary>
        /// Afficher une grille (matrice) de façon esthétique sur la console
        /// </summary>
        /// <param name="grille">Grille à entrer qui sera affichée sur la console</param>
        static void AfficherGrille(int[,] grille)
        {
            if (grille == null)
            {
                Console.WriteLine("La grille est null");
            }
            else
            {
                if (grille.GetLength(0) == 0 || grille.GetLength(1) == 0)
                {
                    Console.WriteLine("La grille est vide");
                }
                else
                {
                    for (int ligne = 0; ligne < grille.GetLength(0); ligne++)
                    {
                        for (int trait = 0; trait < grille.GetLength(1); trait++)
                        //Boucle permettant simplement de séparer chaque ligne d'un trait (esthétique)
                        {
                            Console.Write("----");
                        }
                        Console.WriteLine();

                        Console.Write("| "); //Bâton de la première colonne
                        for (int colonne = 0; colonne < grille.GetLength(1); colonne++)
                        {
                            string cellule = " ";
                            switch (grille[ligne, colonne])
                            {
                                case 0:
                                    cellule = "."; //Cellule morte
                                    break;
                                case 1:
                                    cellule = "#"; //Celulle vivante
                                    break;
                                //Les cas 2 et 3 sont pour la grille définie dans le méthode EvolutionJourSuiant
                                //et permettent de visualiser les états futurs
                                case 2:
                                    cellule = "N"; //Naissance
                                    break;
                                case 3:
                                    cellule = "M"; //Mort
                                    break;

                            }
                            Console.Write(cellule + " | ");
                            //Chaque cellule s'affiche séparée d'un | entre chaque colonne
                        }
                        Console.WriteLine();
                    }
                    for (int trait = 0; trait < grille.GetLength(1); trait++)
                    //Trait de la dernière ligne
                    {
                        Console.Write("----");
                    }
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Affiche de façon esthétique un tableau (idéale pour certains tests)
        /// </summary>
        /// <param name="tableau">Tableau à entrer qui sera affiché sur la console</param>
        static void AfficherTableau(int[] tableau)
        {
            if (tableau == null)
            {
                Console.WriteLine("(tableau null)");
            }
            else if (tableau.Length == 0)
            {
                Console.WriteLine("(tableau vide)");
            }
            else
            {
                for (int compteur = 0; compteur < tableau.Length; compteur++)
                {
                    Console.Write(tableau[compteur]);
                    if (compteur < tableau.Length - 1)
                    {
                        Console.Write(" ; ");
                    }
                    else
                    {
                        Console.WriteLine();
                    }
                }
            }
        }

        /// <summary>
        /// Copie l'intégralité d'une grille (matrice) dans une autre
        /// </summary>
        /// <param name="grille1">Matrice que l'on souhaite copier</param>
        /// <param name="grille2">Matrice dont les éléments seront copiés de grille1</param>
        static void CopierGrille(int[,] grille1, int[,] grille2)
        {
            for (int ligne = 0; ligne < grille1.GetLength(0); ligne++)
            {
                for (int colonne = 0; colonne < grille1.GetLength(1); colonne++)
                {
                    grille2[ligne, colonne] = grille1[ligne, colonne];
                }
            }
        }

        /// <summary>
        /// Crée une grille de taille définie par l'utilisateur avec des cellules aléatoirement réparties
        /// </summary>
        /// <param name="lignes">Nombre de lignes de la grille à créer</param>
        /// <param name="colonnes">Nombre de colonnes de la grille à créer</param>
        /// <param name="tauxVie">Pourcentage correspondant au taux de remplissage de la grille</param>
        /// <param name="version">1 - Grille de 0 et 1 uniquement ; 2 - Grille de 0, 1, ou 4 répartis aléatoirement</param>
        /// <returns>Grille retournée</returns>
        static int[,] GenererGrilleAleatoire(int lignes, int colonnes, int tauxVie, int version)
        {
            int[,] grille = null;
            Random aleatoire = new Random();
            if (lignes > 0 || colonnes > 0) //Bien que par la suite du codage, cette sécurité ne sera plus nécessaire, je préfère la mettre
            {
                grille = new int[lignes, colonnes];
                for (int l = 0; l < lignes; l++)
                {
                    for (int c = 0; c < colonnes; c++)
                    {
                        int cellule = 0;
                        int nbTest = aleatoire.Next(101);
                        //On génère une valeur entre 1 et 100 pour faire un pourcentage de cellules vivantes
                        if (nbTest <= tauxVie)
                        //Si le nombre entré est inférieur ou égal au pourcentage indiqué, la cellule est vivante
                        {
                            cellule = 1;
                        }
                        if (version == 2)
                        //Dans le cas de la variante du jeu
                        {
                            int nombre = aleatoire.Next(2);
                            //On prend un nombre aléatoire entre 0 et 1
                            if (nombre == 1)
                            //Si le nombre vaut 0, alors la cellule vaut 1 (population 1), si le nombre 1, la cellule vaut 4 (population 2)
                            {
                                cellule = 4 * cellule;
                                //Bien sûr, si elle est à 0, elle reste à 0
                            }
                        }
                        grille[l, c] = cellule;
                        //Chaque cellule est soit initialement morte (0), soit vivante (1 ou 4)
                    }
                }
            }
            return grille;
        }

        /// <summary>
        /// (Ancienne version) Compte le nombre de cellules voisines vivantes au rang 1 de la cellule d'une grille donnée
        /// </summary>
        /// <param name="grille">Grille dans laquelle se trouve la cellule</param>
        /// <param name="ligne">Ligne de la cellule</param>
        /// <param name="colonne">Colonne de la cellule</param>
        /// <returns>Tableau de taille 2, [0] = population 1 ; [1] = population 2</returns>
        static int[] CompterNombreVoisinsRang1(int[,] grille, int ligne, int colonne)
        /// (Ancienne) méthode qui permet de compter le nombre de cellules voisines vivantes, parmi les 8 qui l'entourent, d'une cellule donnée
        /// Beaucoup plus longue que la nouvelle créée ensuite. Méthode maintenant inutile, inutilisée
        {
            int[] voisins = new int[2];

            int ligneMax = grille.GetLength(0) - 1;
            int colonneMax = grille.GetLength(1) - 1;

            //On déclare toutes lignes voisines
            int ligneDessus = ligne - 1;
            int ligneDessous = ligne + 1;
            int colonneGauche = colonne - 1;
            int colonneDroite = colonne + 1;

            //On fait attention en cas de dépassement des limites de la grille
            if (ligne == 0) ligneDessus = ligneMax;
            if (ligne == ligneMax) ligneDessous = 0;
            if (colonne == 0) colonneGauche = colonneMax;
            if (colonne == colonneMax) colonneDroite = 0;
            //Pas besoin d'accolades {} pour les conditions précédentes car je ne donne qu'un ordre par condition et pas plus

            /*//Et on peut compter le nombre de cellules voisines vivantes
            voisins += grille[ligneDessus, colonneGauche];
            voisins += grille[ligneDessus, colonne];
            voisins += grille[ligneDessus, colonneDroite];
            voisins += grille[ligne, colonneGauche];
            voisins += grille[ligne, colonneDroite];
            voisins += grille[ligneDessous, colonneGauche];
            voisins += grille[ligneDessous, colonne];
            voisins += grille[ligneDessous, colonneDroite];
            //Comme chaque cellule a comme valeur 0 ou 1, il est facile de compter le nombre de cellules vivantes */

            //Au début, cette méthode ne retournait qu'une population, donc un seul entier et non un tableau, d'où le code commenté

            if (grille[ligneDessus, colonneGauche] == 1) voisins[0]++;
            if (grille[ligneDessus, colonneGauche] == 4) voisins[1]++;
            if (grille[ligneDessus, colonne] == 1) voisins[0]++;
            if (grille[ligneDessus, colonne] == 4) voisins[1]++;
            if (grille[ligneDessus, colonneDroite] == 1) voisins[0]++;
            if (grille[ligneDessus, colonneDroite] == 4) voisins[1]++;
            if (grille[ligne, colonneGauche] == 1) voisins[0]++;
            if (grille[ligne, colonneGauche] == 4) voisins[1]++;
            if (grille[ligne, colonneDroite] == 1) voisins[0]++;
            if (grille[ligne, colonneDroite] == 4) voisins[1]++;
            if (grille[ligneDessous, colonneGauche] == 1) voisins[0]++;
            if (grille[ligneDessous, colonneGauche] == 4) voisins[1]++;
            if (grille[ligneDessous, colonne] == 1) voisins[0]++;
            if (grille[ligneDessous, colonne] == 4) voisins[1]++;
            if (grille[ligneDessous, colonneDroite] == 1) voisins[0]++;
            if (grille[ligneDessous, colonneDroite] == 4) voisins[1]++;

            return voisins;
        } //Méthode inutile à présent

        /// <summary>
        /// (Ancienne version) Compte le nombre de cellules voisines vivantes au rang 2 de la cellule d'une grille donnée
        /// </summary>
        /// <param name="grille">Grille dans laquelle se trouve la cellule</param>
        /// <param name="ligne">Ligne de la cellule</param>
        /// <param name="colonne">Colonne de la cellule</param>
        /// <returns>Tableau de taille 2, [0] = population 1 ; [1] = population 2</returns>
        static int[] CompterNombreVoisinsRang2(int[,] grille, int ligne, int colonne)
        ///Méthode qui renvoie un tableau de deux entiers naturels représentant chacun le nombre d'habitants d'une population
        /// Beaucoup plus longue que la nouvelle créée ensuite. Méthode maintenant inutile, inutilisée
        {
            int[] voisins = new int[2];

            int ligneMax = grille.GetLength(0) - 1;
            int colonneMax = grille.GetLength(1) - 1;

            //On déclare toutes lignes voisines
            int ligneDessus1 = ligne - 1;
            int ligneDessus2 = ligne - 2;
            int ligneDessous1 = ligne + 1;
            int ligneDessous2 = ligne + 2;
            int colonneGauche1 = colonne - 1;
            int colonneGauche2 = colonne - 2;
            int colonneDroite1 = colonne + 1;
            int colonneDroite2 = colonne + 2;

            //On fait attention en cas de dépassement des limites de la grille
            if (ligne == 0)
            {
                ligneDessus1 = ligneMax;
                ligneDessus2 = ligneMax - 1;
            }
            if (ligne == 1)
            {
                ligneDessus2 = ligneMax;
            }
            if (ligne == ligneMax)
            {
                ligneDessous1 = 0;
                ligneDessous2 = 1;
            }
            if (ligne == ligneMax - 1)
            {
                ligneDessous2 = 0;
            }
            if (colonne == 0)
            {
                colonneGauche1 = colonneMax;
                colonneGauche2 = colonneMax - 1;
            }
            if (colonne == 1)
            {
                colonneGauche2 = colonneMax;
            }
            if (colonne == colonneMax)
            {
                colonneDroite1 = 0;
                colonneDroite2 = 1;
            }
            if (colonne == colonneMax - 1)
            {
                colonneDroite2 = 0;
            }

            if (grille[ligneDessus2, colonneGauche2] == 1) voisins[0] += 1;
            if (grille[ligneDessus2, colonneGauche2] == 4) voisins[1] += 1;
            if (grille[ligneDessus2, colonneGauche1] == 1) voisins[0] += 1;
            if (grille[ligneDessus2, colonneGauche1] == 4) voisins[1] += 1;
            if (grille[ligneDessus2, colonne] == 1) voisins[0] += 1;
            if (grille[ligneDessus2, colonne] == 4) voisins[1] += 1;
            if (grille[ligneDessus2, colonneDroite1] == 1) voisins[0] += 1;
            if (grille[ligneDessus2, colonneDroite1] == 4) voisins[1] += 1;
            if (grille[ligneDessus2, colonneDroite2] == 1) voisins[0] += 1;
            if (grille[ligneDessus2, colonneDroite2] == 4) voisins[1] += 1;
            if (grille[ligneDessus1, colonneGauche2] == 1) voisins[0] += 1;
            if (grille[ligneDessus1, colonneGauche2] == 4) voisins[1] += 1;
            if (grille[ligneDessus1, colonneGauche1] == 1) voisins[0] += 1;
            if (grille[ligneDessus1, colonneGauche1] == 4) voisins[1] += 1;
            if (grille[ligneDessus1, colonne] == 1) voisins[0] += 1;
            if (grille[ligneDessus1, colonne] == 4) voisins[1] += 1;
            if (grille[ligneDessus1, colonneDroite1] == 1) voisins[0] += 1;
            if (grille[ligneDessus1, colonneDroite1] == 4) voisins[1] += 1;
            if (grille[ligneDessus1, colonneDroite2] == 1) voisins[0] += 1;
            if (grille[ligneDessus1, colonneDroite2] == 4) voisins[1] += 1;
            if (grille[ligne, colonneGauche2] == 1) voisins[0] += 1;
            if (grille[ligne, colonneGauche2] == 4) voisins[1] += 1;
            if (grille[ligne, colonneGauche1] == 1) voisins[0] += 1;
            if (grille[ligne, colonneGauche1] == 4) voisins[1] += 1;
            if (grille[ligne, colonneDroite1] == 1) voisins[0] += 1;
            if (grille[ligne, colonneDroite1] == 4) voisins[1] += 1;
            if (grille[ligne, colonneDroite2] == 1) voisins[0] += 1;
            if (grille[ligne, colonneDroite2] == 4) voisins[1] += 1;
            if (grille[ligneDessous1, colonneGauche2] == 1) voisins[0] += 1;
            if (grille[ligneDessous1, colonneGauche2] == 4) voisins[1] += 1;
            if (grille[ligneDessous1, colonneGauche1] == 1) voisins[0] += 1;
            if (grille[ligneDessous1, colonneGauche1] == 4) voisins[1] += 1;
            if (grille[ligneDessous1, colonne] == 1) voisins[0] += 1;
            if (grille[ligneDessous1, colonne] == 4) voisins[1] += 1;
            if (grille[ligneDessous1, colonneDroite1] == 1) voisins[0] += 1;
            if (grille[ligneDessous1, colonneDroite1] == 4) voisins[1] += 1;
            if (grille[ligneDessous1, colonneDroite2] == 1) voisins[0] += 1;
            if (grille[ligneDessous1, colonneDroite2] == 4) voisins[1] += 1;
            if (grille[ligneDessous2, colonneGauche2] == 1) voisins[0] += 1;
            if (grille[ligneDessous2, colonneGauche2] == 4) voisins[1] += 1;
            if (grille[ligneDessous2, colonneGauche1] == 1) voisins[0] += 1;
            if (grille[ligneDessous2, colonneGauche1] == 4) voisins[1] += 1;
            if (grille[ligneDessous2, colonne] == 1) voisins[0] += 1;
            if (grille[ligneDessous2, colonne] == 4) voisins[1] += 1;
            if (grille[ligneDessous2, colonneDroite1] == 1) voisins[0] += 1;
            if (grille[ligneDessous2, colonneDroite1] == 4) voisins[1] += 1;
            if (grille[ligneDessous2, colonneDroite2] == 1) voisins[0] += 1;
            if (grille[ligneDessous2, colonneDroite2] == 4) voisins[1] += 1;
            //Avec voisins[0] = population 1 (cellule = 1) et voisins[1] = population 2 (cellule = 4)
            //Beaucoup de conditions mais c'était tout aussi long avec quelque chose comme "switch"

            return voisins;
        } //Méthode inutile à présent

        /// <summary>
        /// Compte le nombre de cellules voisines vivantes de la cellule d'une grille donnée
        /// </summary>
        /// <param name="grille">Grille dans laquelle se trouve la cellule</param>
        /// <param name="ligne">Ligne de la cellule</param>
        /// <param name="colonne">Colonne de la cellule</param>
        /// <param name="rang">Rang auquel on calcule les voisins (rang 1, rang 2)</param>
        /// <returns>Tableau de taille 2, [0] = population 1 ; [1] = population 2</returns>
        static int[] CompterNombreVoisins(int[,] grille, int ligne, int colonne, int rang)
        ///Méthode permettant de compter le nombre de cellules voisines vivantes à un rang donné
        {
            int[] voisins = new int[2];
            for (int l = -rang; l <= rang; l++)
            {
                for (int c = -rang; c <= rang; c++)
                {
                    int ligneVoisin = (ligne + l + grille.GetLength(0)) % grille.GetLength(0);
                    int colonneVoisin = (colonne + c + grille.GetLength(1)) % grille.GetLength(1);
                    //La ligne et la colonne voisine se calculent de façon circulaire afin de prendre compte des limites de la grille

                    if (!(l == 0 && c == 0) && grille[ligneVoisin, colonneVoisin] == 1) voisins[0]++;
                    if (!(l == 0 && c == 0) && grille[ligneVoisin, colonneVoisin] == 4) voisins[1]++;
                    //On fait bien sûr attention à ne pas compter la cellule elle-même comme sa propre voisine
                }
            }
            return voisins;
        }

        /// <summary>
        /// Compte le nombre de cellules vivantes de chaque population sur la grille
        /// </summary>
        /// <param name="grille">Grille du jeu</param>
        /// <returns>Tableau de taille 2, [0] = population 1 ; [1] = population 2</returns>
        static int[] CompterCellulesVivantes(int[,] grille)
        {
            int[] population = new int[2];
            foreach (int element in grille)
            {
                if (element == 1) population[0]++;
                if (element == 4) population[1]++;
                //On incrémente selon la population
            }
            return population;
        }

        /// <summary>
        /// Actualise la grille afin d'afficher les cellules prêtes à naître/mourir
        /// </summary>
        /// <param name="grille">Grille du jeu de la vie</param>
        /// <param name="population">Tableau : [0] = cellules vivantes population 1 ; [1] = cellules vivantes population 2</param>
        static void VisualisationEtatFutur(int[,] grille, int[] population)
        ///Méthode permettant de visualiser le comportement de la génération suivante
        {
            Random aleatoire = new Random();
            int[,] grilleEvolution = new int[grille.GetLength(0), grille.GetLength(1)];
            for (int ligne = 0; ligne < grille.GetLength(0); ligne++)
            {
                for (int colonne = 0; colonne < grille.GetLength(1); colonne++)
                {
                    grilleEvolution[ligne, colonne] = grille[ligne, colonne];
                    //On compte le nombre de cellules voisines vivantes de chaque cellule
                    int[] voisinsRg1 = CompterNombreVoisins(grille, ligne, colonne, 1);
                    int[] voisinsRg2 = CompterNombreVoisins(grille, ligne, colonne, 2);

                    {
                        //int[] voisinsRg1 = CompterNombreVoisinsRang1(grille, ligne, colonne);
                        //int[] voisinsRg2 = CompterNombreVoisinsRang2(grille, ligne, colonne);

                        /* Première version
                        if (grille[ligne, colonne] == 0)
                        {
                            if (voisinsRg1[0] == 3) grilleEvolution[ligne, colonne] = 2;
                            //L'etat 2 veut dire qu'une cellule morte va naître
                        }
                        else
                        {
                            if (voisinsRg1[0] < 2 || voisinsRg1[0] > 3) grilleEvolution[ligne, colonne] = 3;
                            //L'etat 3 veut dire qu'une cellule vivante va mourir
                        }
                        Lignes non nécessaires car dans le cas où il n'y a qu'une population, les lignes suivantes reviennent au même*/
                    } //---//

                    if (grille[ligne, colonne] == 1 && (voisinsRg1[0] < 2 || voisinsRg1[0] > 3)) grilleEvolution[ligne, colonne] = 3;
                    if (grille[ligne, colonne] == 4 && (voisinsRg1[1] < 2 || voisinsRg1[1] > 3)) grilleEvolution[ligne, colonne] = 6;
                    //Règles de naissance au rang 1

                    if (grille[ligne, colonne] == 0 && voisinsRg1[0] == 3 && voisinsRg1[1] == 0) grilleEvolution[ligne, colonne] = 2;
                    if (grille[ligne, colonne] == 0 && voisinsRg1[1] == 3 && voisinsRg1[0] == 0) grilleEvolution[ligne, colonne] = 5;
                    //Règles de naissance au rang 2

                    if (grille[ligne, colonne] == 0 && voisinsRg1[1] == 3 && voisinsRg1[0] == 3)
                    //Règles de naissance en cas d'égalité de voisinage
                    //Code facilement compréhensible en lisant la règle R4b sur l'énoncé
                    {
                        if (voisinsRg2[0] > voisinsRg2[1])
                        {
                            grilleEvolution[ligne, colonne] = 2;
                        }
                        else
                        {
                            if (voisinsRg2[1] > voisinsRg2[0])
                            {
                                grilleEvolution[ligne, colonne] = 5;
                            }
                            else
                            {
                                if (population[0] > population[1])
                                {
                                    grilleEvolution[ligne, colonne] = 2;
                                }
                                else
                                {
                                    if (population[1] > population[0])
                                    {
                                        grilleEvolution[ligne, colonne] = 5;
                                    }
                                    //Variante de la règle R4b
                                    else
                                    {
                                        int cellule = aleatoire.Next(2);
                                        {
                                            if (cellule == 0) grilleEvolution[ligne, colonne] = 2;
                                            if (cellule == 1) grilleEvolution[ligne, colonne] = 5;
                                            Console.WriteLine(cellule);
                                            //Juste pour vérifier que la variante de cette règle est quasiment inutile
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            CopierGrille(grilleEvolution, grille);
        }

        /// <summary>
        /// Actualise la grille d'une génération : les cellules devant naître sont vivantes, celles devant mourir sont mortes
        /// </summary>
        /// <param name="grille">Grille du jeu de la vie, avec la visualisation des etats futurs</param>
        static void EvolutionEtatSuivant(int[,] grille)
        {
            for (int ligne = 0; ligne < grille.GetLength(0); ligne++)
            {
                for (int colonne = 0; colonne < grille.GetLength(1); colonne++)
                {
                    if (grille[ligne, colonne] == 2) grille[ligne, colonne] = 1;
                    if (grille[ligne, colonne] == 5) grille[ligne, colonne] = 4;
                    if (grille[ligne, colonne] == 3 || grille[ligne, colonne] == 6) grille[ligne, colonne] = 0;
                    //Une cellule égale à : 2 devient 1; 5 devient 4; 3 et 6 deviennent 0 tout simplement
                }
            }
        }

        /// <summary>
        /// Tests successifs des fonctions ci-dessus après leurs créations
        /// </summary>
        static void TestGrille()
        ///Void qui a permis de tester chacune des méthodes précédentes et de faire différents tests
        {
            //int[,] grille = GenererGrilleAleatoire(10, 10, 70);
            int[,] grille = { { 0, 4, 1, 0, 0 },
                              { 1, 1, 1, 1, 1 },
                              { 0, 4, 0, 4, 0 },
                              { 0, 1, 4, 1, 0 },
                              { 0, 1, 0, 1, 0 } };
            AfficherGrille(grille);
            AfficherTableau(CompterNombreVoisinsRang2(grille, 1, 1));
            AfficherTableau(CompterNombreVoisinsRang2(grille, 2, 2));
            AfficherTableau(CompterNombreVoisinsRang2(grille, 4, 4));
            AfficherTableau(CompterNombreVoisins(grille, 1, 1, 2));
            AfficherTableau(CompterNombreVoisins(grille, 2, 2, 1));
            AfficherTableau(CompterNombreVoisins(grille, 4, 4, 2));
            //AfficherTableau(CompterNombreVoisinsRang2(grille, 9, 9));

            /*VisualisationEtatFutur(grille);
            AfficherGrille(grille);
            EvolutionEtatSuivant(grille);
            AfficherGrille(grille); */
        }

        /// <summary>
        /// Premier test du jeu de la vie avec la Console, puis avec GUI, première version finale qui a pu aboutir
        /// </summary>
        static void TestJeu1()
        ///Beaucoup de code en commentaire car beaucoup de tests ont été faits (affichage sur console, 
        ///appui sur n'importe quelle touche pour passer à la génération suivante, etc.) 
        ///Cette méthode n'est plus utile maintenant que la vraie version est terminée, mais je la laisse quand même ici
        {
            //On initialise d'abord certaines variables qui seront utilse pour la suite
            int nbLignes = 0;
            int nbColonnes = 0;
            int tauxVie = -1;
            int choix = 0;
            int generation = 0;
            int[] population = new int[2];

            //Dans un premier temps on laisse l'utilisateur mettre ses préférences pour la génération de la grille
            while (nbLignes < 3)
            {
                Console.Write("Veuillez entrer un nombre de lignes au moins supérieur à 3 : ");
                nbLignes = Convert.ToInt32(Console.ReadLine());
            }
            while (nbColonnes < 3)
            {
                Console.Write("Veuillez entrer un nombre de colonnes au moins supérieur à 3 : ");
                nbColonnes = Convert.ToInt32(Console.ReadLine());
            }

            while (tauxVie < 0 || tauxVie > 100)
            {
                Console.Write("Veuillez entrer un pourcentage entre 0 et 100 de cellules vivantes : "); //Marche aussi évidemment avec des valeurs réelles
                tauxVie = Convert.ToInt32(Console.ReadLine());
            }
            tauxVie = 101 - tauxVie;

            Console.WriteLine();
            //On crée ensuite une grille avec les critères entrés par l'utilisateur
            int[,] grille = GenererGrilleAleatoire(nbLignes, nbColonnes, tauxVie, 1);

            //On propose deux choix à l'utilisateur
            Console.WriteLine("1 - Jeu de la vie sans visualisation des étapes futures");
            Console.WriteLine("2 - Jeu de la vie avec visualisation des étapes futures");

            while (choix != 1 && choix != 2)
            {
                Console.Write("Veuillez choisir une des deux propositions : ");
                choix = Convert.ToInt32(Console.ReadLine());
            }

            /*int[,] grille = { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                              { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                              { 0, 0, 0, 1, 0, 1, 0, 0, 0, 0 },
                              { 0, 0, 0, 1, 0, 1, 0, 0, 0, 0 },
                              { 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 }, //grille de test
                              { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                              { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                              { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                              { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                              { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, }; */

            //Génération de l'interface graphique
            //Fenetre gui = new Fenetre(grille, 15, 0, 0, " Jeu de la vie ");


            //Lancement du jeu
            while (true)
            {
                population = CompterCellulesVivantes(grille);

                Console.WriteLine();
                Console.WriteLine("Génération " + generation + " : Taille de la population : " + population[0]);
                AfficherGrille(grille);

                //Actualisation de l'interface graphique
                //gui.RafraichirTout();
                //gui.changerMessage("Génération " + generation + " : Taille de la population : " + population[0]);

                VisualisationEtatFutur(grille, population);
                //On actualise la grille qui vient rendre compte de l'évolution de la prochaine étape
                if (choix == 2)
                {
                    Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer");

                    Console.ReadKey();
                    //System.Threading.Thread.Sleep(500);
                    //gui.RafraichirTout();

                    AfficherGrille(grille);
                }
                EvolutionEtatSuivant(grille);

                Console.WriteLine("Appuyez sur n'importe quelle touche pour passer à la génération suivante");
                //L'utilisateur a juste a appuyer pour n'importe quelle touche pour passer à l'étape suivante
                Console.ReadKey();
                //System.Threading.Thread.Sleep(500);

                generation++;
                //On passe à la génération suivante
            }
        }

        /// <summary>
        /// /Méthode qui renvoie en sortie les règles définies par l'utilisateur, dans un tableau
        /// </summary>
        /// <returns>Tableau dans lequel chaque valeur a une fonction particulière (codant pour oui/non par exemple, ou un nombre...)</returns>
        static int[] Menu()
        {
            int[] menu = new int[7];
            int version = 0;
            int visualisation = 0;
            int vitesse = 0;
            int lignes = 0;
            int colonnes = 0;
            int tauxVie = 0;
            int taille = 0;
            //On initialise les variables

            //A chaque fois, tant que l'utilisateur n'a pas rentré un entier qui convient, il doit réécrire.
            Console.WriteLine("1 - Jeu de la vie : Classique avec 1 population");
            Console.WriteLine("2 - Jeu de la vie : Variante avec 2 populations");
            Console.WriteLine("A quelle version voulez-vous jouer ?");
            while (version != 1 && version != 2)
            {
                Console.Write("Veuillez choisir 1 ou 2 : ");
                int.TryParse(Console.ReadLine(), out version);
            }
            Console.WriteLine();

            Console.WriteLine("1 - Jeu sans visualisation des états futurs");
            Console.WriteLine("2 - Jeu avec visualisation des états futurs");
            Console.WriteLine("Voulez-vous avoir une visualisation des états futurs ?");
            while (visualisation != 1 && visualisation != 2)
            {
                Console.Write("Veuillez choisir entre 1 et 2 : ");
                int.TryParse(Console.ReadLine(), out visualisation);
            }
            Console.WriteLine();

            Console.WriteLine("1 - Appuyer sur n'importe quelle touche DEPUIS la fenêtre de la console");
            Console.WriteLine("2 - Lent");
            Console.WriteLine("3 - Moyen");
            Console.WriteLine("4 - Rapide");
            Console.WriteLine("5 - Très rapide");
            Console.WriteLine("A quelle vitesse souhaitez-vous passer d'une génération à l'autre ?");
            while (vitesse < 1 || vitesse > 5)
            {
                Console.Write("Veuillez choisir la vitesse : ");
                int.TryParse(Console.ReadLine(), out vitesse);
            }
            Console.WriteLine();

            while (lignes < 5)
            {
                Console.Write("Veuillez entrer un nombre de lignes au moins égal à 5 : ");
                int.TryParse(Console.ReadLine(), out lignes);
            }

            while (colonnes < 5)
            {
                Console.Write("Veuillez entrer un nombre de colonnes au moins égal à 5 : ");
                int.TryParse(Console.ReadLine(), out colonnes);
            }
            Console.WriteLine();

            Console.WriteLine("Quel pourcentage de population voulez-vous sur la grille ?");
            while (tauxVie < 1 || tauxVie > 100)
            {
                Console.Write("Veuillez entrer un pourcentage entre 1 et 100 de cellules vivantes : ");
                //Aurait pu marcher aussi évidemment avec des valeurs réelles, avec double, etc..
                int.TryParse(Console.ReadLine(), out tauxVie);
            }
            Console.WriteLine();

            while (taille < 1 || taille > 50)
            {
                Console.Write("Quelle taille (entre 1 et 50 pixels) voulez-vous donner à chaque cellule ? (conseillée 15 pixels) : ");
                taille = Convert.ToInt32(Console.ReadLine());
            }

            Console.WriteLine();

            menu[0] = version;
            menu[1] = visualisation;
            menu[2] = vitesse - 1;
            menu[3] = lignes;
            menu[4] = colonnes;
            menu[5] = tauxVie;
            menu[6] = taille;
            //Chaque élément du tableau a son importance pour l'initialisation de la grille, et les règles d'états suivants

            return menu;
            //On renvoie en sortie le tableau
        }

        /// <summary>
        /// Vérifie si deux matrices sont identiques ou non
        /// </summary>
        /// <param name="grille1">Grille à comparer</param>
        /// <param name="grille2">Grille comparée</param>
        /// <returns>true = grilles identiques ; false = grilles différentes</returns>
        static bool EstIdentique(int[,] grille1, int[,] grille2)
        {
            bool identique = true;
            for (int ligne = 0; ligne < grille1.GetLength(0); ligne++)
            {
                for (int colonne = 0; colonne < grille1.GetLength(1); colonne++)
                {
                    if (grille1[ligne, colonne] != grille2[ligne, colonne])
                    {
                        identique = false;
                    }
                }
            }
            return identique;
        }

        /// <summary>
        /// Version finale du lancement du jeu de la vie dans les 2 versions
        /// </summary>
        static void LancementDuJeu()
        {
            int[] menu = Menu();
            int version = menu[0];
            int visualisation = menu[1];
            int vitesse = menu[2];
            int lignes = menu[3];
            int colonnes = menu[4];
            int tauxVie = menu[5];
            int taille = menu[6];
            //On initialise d'abord la grille

            int[] population = new int[2];
            int generation = -1;         //On initialise certaines variables
            bool stable = false;

            //Puis on génère la grille
            int[,] grille = GenererGrilleAleatoire(lignes, colonnes, tauxVie, version);

            { /*
                int[,] grille = { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                  { 0, 0, 0, 4, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0 },
                                  { 0, 0, 0, 4, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0 },
                                  { 0, 0, 0, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } };
            */

            /*int[,] grille = new int[47, 96];
            grille[23, 47] = 1;
            grille[24, 47] = 1;
            grille[25, 47] = 1;
            grille[25, 46] = 1;
            grille[25, 45] = 1;
            grille[24, 45] = 1;
            grille[23, 45] = 1;*/

            }//grilles de tests


            int[,] grillePrecedente1 = new int[grille.GetLength(0), grille.GetLength(1)];
            int[,] grillePrecedente2 = new int[grille.GetLength(0), grille.GetLength(1)];
            int[,] grillePrecedente3 = new int[grille.GetLength(0), grille.GetLength(1)];
            int[,] grillePrecedente4 = new int[grille.GetLength(0), grille.GetLength(1)];
            int[,] grillePrecedente5 = new int[grille.GetLength(0), grille.GetLength(1)];
            /*Grilles qui permettront de vérifier la pseudo-stabilité de la grille, avec une période maximale de 5
            Si la période est plus grande que 5 (ou inexistante, ce qui semble très peu probable dans une grille 
            circulaire), le jeu ne sera jamais "terminé".
            Seules 2 grilles auraient suffi, étant donné que la génération est aléatoire, il est plutot probable d'avoir 
            des oscillateurs de période 2 maximum.*/

            Fenetre gui = new Fenetre(grille, taille, 0, 0, " Jeu de la vie ");
            while (!stable)
            {
                generation++;
                /*On passe à la génération suivante. A la base, generation valait d'abord 0 et s'actualisait en fin de boucle
                 mais je suis passé au début de la boucle afin que les générations arrêtent de compter lors de la stabilisation 
                 de la grille.*/

                population = CompterCellulesVivantes(grille);
                //On compte le nombre de cellules vivantes
                switch (version)
                {
                    case 1:
                        gui.changerMessage("Génération " + generation + " ; Taille de la population : " + population[0]);
                        break;

                    case 2:
                        gui.changerMessage("Génération " + generation + " ; Population 1 : " + population[0] + " ; Population 2 : " + population[1]);
                        break;
                }
                //Mise à jour de l'interface

                VisualisationEtatFutur(grille, population);
                if (visualisation == 2)
                {
                    switch (vitesse)
                    //En fonction de la vitesse choisie par l'utilisateur, on actualise la fenêtre
                    {
                        case 0:
                            Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer");
                            Console.ReadKey();
                            break;

                        case 1:
                            System.Threading.Thread.Sleep(1000);
                            break;

                        case 2:
                            System.Threading.Thread.Sleep(500);
                            break;

                        case 3:
                            System.Threading.Thread.Sleep(100);
                            break;

                        //Dans le cas où la vitesse vaut "4", il n'y a aucune attente, donc pas besoin de "case 4:".
                    }
                    gui.RafraichirTout();
                }
                EvolutionEtatSuivant(grille);
                //On actualise de nouveau la grille

                switch (vitesse)
                {
                    case 0:
                        Console.WriteLine("Appuyez sur n'importe quelle touche pour passer à la génération suivante");
                        Console.ReadKey();
                        break;

                    case 1:
                        System.Threading.Thread.Sleep(1000);
                        break;

                    case 2:
                        System.Threading.Thread.Sleep(500);
                        break;

                    case 3:
                        System.Threading.Thread.Sleep(100);
                        break;
                }

                stable = EstIdentique(grille, grillePrecedente1);
                if (!stable) stable = EstIdentique(grille, grillePrecedente2);
                if (!stable) stable = EstIdentique(grille, grillePrecedente3);
                if (!stable) stable = EstIdentique(grille, grillePrecedente4);
                if (!stable) stable = EstIdentique(grille, grillePrecedente5);
                //On vérifie que la grille n'est pas "stable"

                CopierGrille(grille, grillePrecedente1);
                if (generation % 2 == 0) CopierGrille(grille, grillePrecedente2);
                if (generation % 3 == 0) CopierGrille(grille, grillePrecedente3);
                if (generation % 4 == 0) CopierGrille(grille, grillePrecedente4);
                if (generation % 5 == 0) CopierGrille(grille, grillePrecedente5);
                //On met à jour les grilles des 5 générations précédentes pour les prochaines vérification de
                // pseudo-stabilisation de la grille.

                gui.RafraichirTout();
                //Puis on actualise la fenêtre
            }
            Console.WriteLine("La grille s'est stabilisée à la génération " + generation + ".");
            switch (version)
            {
                case 1:
                    Console.WriteLine("La taille finale de la population est de " + population[0]);
                    break;
                case 2:
                    Console.WriteLine("La taille finale de la population 1 est de " + population[0] + " et celle de " +
                        "la population 2 est de " + population[1]);
                    break;
            }
            Console.WriteLine("Appuyez sur n'importe quelle touche pour quitter le jeu.");
        }

        [System.STAThreadAttribute()]
        static void Main(string[] args)
        {
            //TestGrille();
            //TestJeu1();

            LancementDuJeu();

            Console.ReadKey();
        }
    }
}
