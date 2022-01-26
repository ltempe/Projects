using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traitement_Images
{
    /// <summary>
    /// Classe de pixel, composé de 3 intensités de couleurs allant de 0 à 255, intensités de type byte
    /// </summary>
    public class Pixel
    {
        #region champs
        byte rouge;
        byte vert;
        byte bleu;
        #endregion

        #region constructeurs
        /// <summary>
        /// Constructeur d'un pixel avec les 3 couleurs primaires
        /// </summary>
        /// <param name="r">Quantité de rouge entre 0 et 255</param>
        /// <param name="v">Quatité de vert entre 0 et 255</param>
        /// <param name="b">Quantité de bleu entre 0 et 255</param>
        public Pixel(byte r, byte v, byte b)
        {
            rouge = r;
            vert = v;
            bleu = b;
        }
        #endregion

        #region Attributs
        /// <summary>
        /// Attribut de la quantité de rouge du pixel
        /// </summary>
        public byte R
        {
            get { return this.rouge; }
            set { this.rouge = value; }
        }

        /// <summary>
        /// Attribut de la quantité de vert du pixel
        /// </summary>
        public byte V
        {
            get { return this.vert; }
            set { this.vert = value; }
        }

        /// <summary>
        /// Attribut de la quantité de vert du pixel
        /// </summary>
        public byte B
        {
            get { return this.bleu; }
            set { this.bleu = value; }
        }
        #endregion

        #region Méthodes
        /// <summary>
        /// Calcule la somme des 3 pixels, utile pour certaines méthodes
        /// </summary>
        /// <returns>Somme des 3 pixels</returns>
        public int Somme()
        {
            return rouge + vert + bleu;
        }

        /// <summary>
        /// Vérifie que 2 pixels ont la même valeur
        /// </summary>
        /// <param name="pix">Pixel à vérifier</param>
        /// <returns>Vérification</returns>
        public bool EstEgal(Pixel pix)
        {
            bool egalite = true;
            if (rouge != pix.rouge || vert != pix.vert || bleu != pix.bleu) egalite = false;
            return egalite;
        }

        /// <summary>
        /// Affiche la quantité de chaque couleur du pixel dans une chaîne de caractères
        /// </summary>
        /// <returns>Chaîne retournée</returns>
        public override string ToString()
        {
            return rouge + " " + vert + " " + bleu;
        }
        #endregion
    }
}
