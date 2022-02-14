using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Probleme_Pizzeria
{
    /// <summary>
    /// Interface d'objets identifiables dont les données peuvent être enregistrées dans des fichiers csv
    /// </summary>
    public interface IIdentifiable
    {
        /// <summary>
        /// Donne le numéro d'identification de l'identifiable en lecture seule
        /// </summary>
        string Numero { get; }

        /// <summary>
        /// Retourne une chaine de caractère pouvant être intégrée à une ligne de fichier texte comme un
        /// fichier csv
        /// </summary>
        /// <returns>Chaine de caractère d'attributs séparés par des ';' pouvant être intégrée à une ligne de fichier csv</returns>
        string ToFile();
    }
}
