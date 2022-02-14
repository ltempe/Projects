using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mixmo
{
    /// <summary>
    /// Classe des mots croisés, qui gère la grille d'un joueur, bien que la plupart des fonctions soient dans la classe joueur
    /// </summary>
    public class MotsCroises
    {
        #region Champs
        char[,] grille; //Grille dans laquelle se trouvent les mots
        //J'avais pensé à faire une grille de Lettre plutôt que char pour gagner de la précision sur certains points
        //J'aurais pu ajouter la liste des mots trouvés ici, mais tant pis
        #endregion

        #region Constructeurs
        /// <summary>
        /// Constructeur par taille
        /// </summary>
        /// <param name="taille">Taille de la grille carrée</param>
        public MotsCroises(int taille)
        {
            CreerGrille(taille);
        }

        /// <summary>
        /// Constructeur à partir d'une grille existante
        /// </summary>
        /// <param name="grille"></param>
        public MotsCroises(char[,] grille)
        {
            this.grille = grille;
        }
        #endregion

        #region Attributs
        /// <summary>
        /// Attribut de la grille avec possibilité d'écriture
        /// </summary>
        public char[,] Grille
        {
            get { return this.grille; }
            set { this.grille = value; } //Besoin du set pour gagner du temps dans certains tests unitaires
        }
        #endregion

        #region Methodes
        /// <summary>
        /// Crée la grille carrée du joueur, initialement composée d'espaces vides
        /// </summary>
        /// <param name="tailleGrille">Taille souhaitée de la grille</param>
        public void CreerGrille(int tailleGrille)
        {
            grille = new char[tailleGrille, tailleGrille];
            for (int ligne = 0; ligne < tailleGrille; ligne++)
            {
                for (int colonne = 0; colonne < tailleGrille; colonne++)
                {
                    grille[ligne, colonne] = ' ';
                }
            }
        }


        /// <summary>
        /// Vérifie si une case de la grille est voisine directe avec une case non vide
        /// </summary>
        /// <param name="ligne">ligne de la case</param>
        /// <param name="colonne">colonne de la case</param>
        /// <returns></returns>
        public bool ToucheMots(int ligne, int colonne)
        {
            bool touche = false;
            int etape = 0;
            for (int lignes = ligne-1; lignes <= ligne+1; lignes++)
            {
                for (int colonnes = colonne-1; colonnes <= colonne+1; colonnes++)
                {
                    if (etape % 2 == 1)
                    //On recherche seulement si les 4 cases voisines directes sont vides ou non, pas les 4 autres diagonales
                    //d'où la vérification de la parité des étapes de la boucle
                    {
                        bool vide = true;
                        try
                        //On fait un try dans le cas où la case est en limite du tableau
                        {
                            //La case voisine est vide si elle contient un espace
                            vide = grille[lignes, colonnes] == ' ';
                        }
                        catch
                        //En cas d'erreur on considere que la case voisine est vide
                        {
                            vide = true;
                        }
                        finally
                        {
                            //Si la case n'est pas vide, le booléen touche passe à true
                            if (!vide) touche = true;
                        }
                    }
                    etape++;
                }
            }
            return touche;
        }
        
        /// <summary>
        /// Retourne un affichage clair de la grille sous forme de chaine de caractères
        /// </summary>
        /// <returns>Chaine de caractères en question</returns>
        public string toString()
        {
            string result = "";
            for (int ligne = 0; ligne <= grille.GetLength(0) + 1; ligne++)
            {
                for (int colonne = 0; colonne <= grille.GetLength(1); colonne++)
                {
                    //A partir de là, beaucoup de conditions pour avoir un bel affichage :)
                    if (ligne == 1) result += "---";
                    else
                    {
                        if (ligne == 0 && colonne == 0) result += "   ";
                        else
                        {
                            if (ligne == 0)
                            {
                                if (colonne < 10) result += " ";
                                result += Convert.ToString(colonne) + "|";
                            }
                            else
                            {
                                if (colonne == 0)
                                {
                                    if (ligne - 1 < 10) result += " ";
                                    result += Convert.ToString(ligne - 1) + "| ";
                                }
                                else result += grille[ligne - 2, colonne - 1] + "| ";
                            }
                        }
                    }

                }
                result += "\n";
            }
            return result;
        }
        #endregion
    }
}
