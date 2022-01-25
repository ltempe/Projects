using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mixmo
{
    //Beaucoup de commentaires pour m'aider à me rappeler pourquoi j'ai fait tels tests, et comment
    [TestClass]
    public class UnitTest1
    {
        Lettres piocheTest = new Lettres("Pioche de A à E", "Lettre_Test_AtoE.txt"); //Pioche avec 3 lettres de chaque, de A à E
        Lettres piochePrincipale = new Lettres("Principale", "Lettre_Principal.txt"); //Pioche principale
        Dictionnaire dictionnaire = new Dictionnaire("Dictionnaire", "MotsPossibles_Test.txt");
        Joueur j = new Joueur("Joueur", 10);
        Random r = new Random();

        [TestMethod]
        public void IndicePiocheTest()
        {
            //Les 5e et 6e lettres de la pioche de test en prenant en compte la quantité sont les mêmes
            //contrairement à la 7e lettre
            int indice1 = piocheTest.IndicePioche(5); //B
            int indice2 = piocheTest.IndicePioche(6); //B
            int indice3 = piocheTest.IndicePioche(7); //C
            Assert.AreEqual(indice1, indice2);
            Assert.AreNotEqual(indice2, indice3);
        }

        [TestMethod]
        public void IndicePiocheIndef()
        {
            int indice1 = piocheTest.IndicePioche(30); //Il n'y a que 15 (5*3) lettres dans la pioche de test
            int indice2 = piocheTest.IndicePioche(-5); //Indice négatif, n'a aucun sens
            int indice3 = piocheTest.IndicePioche(0); //La méthode donne des indices à partir de 1
            Assert.AreEqual(indice1, indice2);
            Assert.AreEqual(indice2, indice3);
            Assert.AreEqual(indice1, -1); //Tous les indices non définis sont égaux à 1
        }

        [TestMethod]
        public void PiocheLettre()
        {
            int indice1 = piocheTest.IndicePioche(5);
            int indice2 = piocheTest.IndicePioche(6);
            int indice3 = piocheTest.IndicePioche(7);
            Lettre B = piocheTest.piocherLettre(indice1);
            Lettre B2 = piocheTest.piocherLettre(indice2);
            Lettre C = piocheTest.piocherLettre(indice3);
            Assert.AreEqual(B.Symbole, 'B');
            Assert.IsTrue(B.EstEgale(B2));
            Assert.IsFalse(C.EstEgale(B));
        }
        
        [TestMethod]
        public void PiocheLettreRetirer()
        {
            int taille1 = piocheTest.QuantiteTotale;
            Assert.AreEqual(piocheTest.ListeDeLettres[2].Quantite, 3); //Il y a 3 fois la lettre à l'indice 2 (soit C) dans la pioche de test
            Lettre C = piocheTest.piocherLettre(2); 
            int taille2 = piocheTest.QuantiteTotale;
            Assert.AreNotEqual(taille1, taille2); //Les tailles doivent être différentes car on retire des lettres de la pioche
            Assert.AreEqual(C.Symbole, 'C'); 
            Assert.AreEqual(piocheTest.ListeDeLettres[2].Quantite, 2); //Un C a été pioché, il n'en reste plus que 2
        }
        
        [TestMethod]
        public void JoueurPioche()
        {
            int taille1 = piocheTest.QuantiteTotale;
            j.AddLettres(6, piocheTest, r);
            int nombreLettres = j.LettresJoueur.QuantiteTotale;
            int taille2 = piocheTest.QuantiteTotale;
            Assert.AreEqual(nombreLettres, 6);
            Assert.AreEqual(taille1 - 6, taille2); //Le joueur a pioché 6 lettres donc la différence de quantité totale doit être de 6
        }

        [TestMethod]
        public void DictionnaireRechercheMot()
        {
            string mot1 = "MERCREDI"; //MERCREDI existe dans le dictionnaire
            string mot2 = "LALALA"; //Mais pas les mots suivants
            string mot3 = "ZZZZ";
            string mot4 = "AAAA";
            string mot5 = "manuellement"; //Le dictionnaire ne possède que des majuscules
            string mot6 = mot5.ToUpper(); //string.ToUpper() convertis en lettres capitales
            Assert.IsTrue(dictionnaire.RechDichoRecursif(mot1));
            Assert.IsFalse(dictionnaire.RechDichoRecursif(mot2));
            Assert.IsFalse(dictionnaire.RechDichoRecursif(mot3));
            Assert.IsFalse(dictionnaire.RechDichoRecursif(mot4));
            Assert.IsFalse(dictionnaire.RechDichoRecursif(mot5));
            Assert.IsTrue(dictionnaire.RechDichoRecursif(mot6));
        }

        [TestMethod]
        public void DictionnaireIndice()
        {
            Assert.AreEqual(dictionnaire.IndiceMotsTaille(1), -1); //Il n'existe pas de mots de 1 lettre dans le dictionnaire
            Assert.AreEqual(dictionnaire.IndiceMotsTaille(2), 0); //Les mots de 2 lettres sont à l'indice 0 dans la sortedList du dictionnaire
            Assert.AreEqual(dictionnaire.IndiceMotsTaille(5), 3); //Ceux de 5 lettres à l'indice 3
            Assert.AreEqual(dictionnaire.IndiceMotsTaille(50), -1); //Il n'existe pas de mots de 50 lettres
        }

        [TestMethod]
        public void IndiceLettreJoueur()
        {
            Lettre b = new Lettre('B', 1, 0);
            Lettre k = new Lettre('K', 1, 0);
            j.AddLettres(15, piocheTest, r); //Le joueur possède 5*3 lettres (de A à E)
            Assert.AreEqual(j.LettresJoueur.Indice(k), -1); //Le joueur ne possède pas la lettre k
            Assert.AreNotEqual(j.LettresJoueur.Indice(b), -1); //Le joueur possède la lettre k
            j.LettresJoueur.ListeDeLettres.Add(k); //On ajoute k aux lettres du joueur
            Assert.AreEqual(j.LettresJoueur.Indice(k), j.LettresJoueur.ListeDeLettres.Count - 1); //k est placé à la dernière position de la liste du joueur
        }

        [TestMethod]
        public void AjoutLettres()
        {
            Joueur j1 = new Joueur("Joueur 1", 10);
            Joueur j2 = new Joueur("Joueur 2", 10);
            bool pioche1 = j1.AddLettres(100, piocheTest, r); //Le joueur 1 pioche plus de lettres qu'il n'en existe dans la pioche
            bool pioche2 = j2.AddLettres(5, piocheTest, r); //Le joueur 2 en pioche 5, mais la pioche est déjà vidée par j1
            Assert.IsTrue(pioche1); //Le joueur 1 a pu piocher des lettres
            Assert.IsFalse(pioche2); //Mais pas le joueur 2 car la pioche est vide
            Assert.AreEqual(j1.LettresJoueur.ListeDeLettres.Count, 5); //Le joueur 1 a 5 lettres (de A à E)
            Assert.AreEqual(j1.LettresJoueur.QuantiteTotale, 15); //Le joueur 2 a 15 lettres au total (5*3)
            Assert.AreEqual(j2.LettresJoueur.ListeDeLettres.Count, 0); //Le jouur 2 n'a aucune lettre

            //Ajout de lettres au joueur 2 (D'une façon lourde, mais efficace..)
            Lettres liste = new Lettres("Test");
            Lettre b = new Lettre('B', 1, 0);
            Lettre e = new Lettre('E', 2, 0);
            Lettre l = new Lettre('L', 2, 0);
            liste.ListeDeLettres.Add(b);
            liste.ListeDeLettres.Add(e);
            liste.ListeDeLettres.Add(l);
            Assert.IsFalse(j2.AddLettres(5, liste, r)); //La quantité totale a besoin d'être prise en compte pour appliquer la méthode
            liste.QuantiteTotale += 5;
            Assert.IsTrue(j2.AddLettres(5, liste, r)); //Une fois la quantité totale de lettres ajustée, la méthode fonctionne
        }

        [TestMethod]
        public void LettresSuffisantes()
        {
            //On ajoute encore des lettres de la même façon, même s'il y avait plus pratique après reflexion ...
            Lettres liste = new Lettres("Test");
            Lettre b = new Lettre('B', 1, 0);
            Lettre e = new Lettre('E', 2, 0);
            Lettre l = new Lettre('L', 2, 0);
            liste.ListeDeLettres.Add(b);
            liste.ListeDeLettres.Add(e);
            liste.ListeDeLettres.Add(l);
            liste.QuantiteTotale += 5;
            j.AddLettres(5, liste, r);
            Assert.IsTrue(j.Possede("BELLE")); //Le joueur possède les lettres contenue dans le mot BELLE
            Assert.IsTrue(j.Possede("BEL")); //De même pour BEL
            Assert.IsTrue(j.Possede("LEBL")); //Et aussi pour tout mélange des 5 lettres, il n'est pas question ici de vérification de leur existance
            Assert.IsFalse(j.Possede("BEBE")); //Le joueur ne possède qu'un seul B
            Assert.IsFalse(j.Possede("BEAU")); //Le joueur ne possède pas les lettres A et U
        }

        [TestMethod]
        public void OterLettresTest()
        {
            j.AddLettres(100, piocheTest, r); //Ajoute le maximum de lettres disponibles dans la pioche de test car il y a moins de 100 lettres
            Assert.IsTrue(j.Possede("EEE")); //Le joueur possède en effet 3 E
            int quantite1 = j.LettresJoueur.QuantiteTotale;
            j.OteLettres("EEE"); //On lui retire les lettres contenues dans la chaîne de caractères "EEE"
            int quantite2 = j.LettresJoueur.QuantiteTotale;
            Assert.IsFalse(j.Possede("EEE")); //Le joueur ne possède donc plus ces lettres
            Assert.AreEqual(quantite1 - quantite2, 3); //Et la quantité de lettres du joueur a bien diminué de 3
        }

        [TestMethod]
        public void GrilleVide()
        {
            Assert.IsTrue(j.GrilleVide()); //La grille du joueur est initialement vide (composée d'espaces, pas vide de taille 0)
            j.AddLettres(100, piocheTest, r); //Le joueur pioche toutes les lettres de A à E
            Assert.IsTrue(j.PeutPlacer("DE", 2, 2, true)); //Le joueur peut placer le mot DE car il possède 3 lettres D et 3 lettres E
            j.PlacerMot("DE", 2, 2, true); //Le joueur place le mot à l'endroit vérifié
            Assert.IsFalse(j.GrilleVide()); //Et la grille n'est plus vide
        }

        [TestMethod]
        public void GrilleNonVidePuisVide()
        {
            j.AddLettres(100, piocheTest, r);
            j.PlacerMot("DE", 2, 2, true);
            Assert.IsFalse(j.GrilleVide()); //La grille n'est pas vide
            int qte1 = j.LettresJoueur.QuantiteTotale;
            j.RecupererLettres(); //Le joueur récupère les lettres de la grille
            int qte2 = j.LettresJoueur.QuantiteTotale;
            Assert.IsTrue(j.GrilleVide()); //Sa grille redevient vide
            Assert.IsTrue(qte2 > qte1); //Son nombre de lettres a augmenté
        }

        [TestMethod]
        public void PlacementPossible()
        {
            //Pas besoin de faire piocher de lettres au joueur pour ce test
            Assert.IsTrue(j.PeutPlacer("ABCDEFG", 1, 1, true)); //Le mot rentre à cette position à l'horizontal
            Assert.IsFalse(j.PeutPlacer("ABCDEFG", 1, 8, true)); //Le mot sort de la grille
            Assert.IsTrue(j.PeutPlacer("ZZZ", 5, 5, true)); //Le mot rentre
            Assert.IsFalse(j.PeutPlacer("AZERTY", 8, 1, false)); //Le mot sort de la grille à la verticale
            Assert.IsTrue(j.PeutPlacer("AZERTY", 3, 4, false)); //Le mot rentre à la verticale
        }

        [TestMethod]
        public void VerificationEmplacement()
        {
                               // 1    2    3    4    5    6
            char[,] grille = { { ' ', ' ', ' ', ' ', ' ', ' '},   //1
                               { 'N', 'O', 'E', 'L', ' ', ' '},   //2
                               { ' ', ' ', ' ', 'A', 'I', 'R'},   //3
                               { ' ', ' ', ' ', 'I', ' ', ' '},   //4
                               { ' ', ' ', 'E', 'T', ' ', ' '},   //5
                               { ' ', ' ', ' ', ' ', ' ', ' '} }; //6
            j.Grille = grille;
            j.AddLettres(200, piochePrincipale, r);
            Assert.IsTrue(j.VerificationMots(dictionnaire, "JE", 1, 3, false)); //JE peut être placé à la verticale à cette position sans créer de faux mots
            Assert.IsFalse(j.VerificationMots(dictionnaire, "JE", 4, 3, false)); //JE (vertical) existe mais pas JI (horizontal)
            Assert.IsFalse(j.MotTrouve("JI")); //Même si JI est placé, il n'existe pas dans le dictionnaire donc n'a pas été ajouté à la liste
            Assert.IsFalse(j.VerificationMots(dictionnaire, "ES", 5, 5, false)); //JI est encore placé sur la grille, donc le méthode renvoie false même si ES existe
                                // 1    2    3    4    5    6
            char[,] grille2 = { { ' ', ' ', ' ', ' ', ' ', ' '},   //1
                                { 'N', 'O', 'E', 'L', ' ', ' '},   //2
                                { ' ', ' ', ' ', 'A', 'I', 'R'},   //3
                                { ' ', ' ', ' ', 'I', ' ', ' '},   //4
                                { ' ', ' ', 'E', 'T', ' ', ' '},   //5
                                { ' ', ' ', ' ', ' ', ' ', ' '} }; //6
            j.Grille = grille2;
            Assert.IsTrue(j.VerificationMots(dictionnaire, "ES", 5, 5, false)); //ES (vertical) existe et ETE (horizontal) aussi
            Assert.IsTrue(j.VerificationMots(dictionnaire, "TU", 6, 2, true)); //TU (horizontal) existe et EU (vertical) aussi
            Assert.IsTrue(j.MotTrouve("NOEL")); //NOEL est déjà placé sur la grille à l'horizontal
            Assert.IsTrue(j.MotTrouve("ETE")); //ETE s'est ajouté lorsque le joueur a ajouté ES à la verticale
            Assert.IsFalse(j.MotTrouve("ET")); //ET n'est plus un mot trouvé car on y a ajouté un E au bout, ce qui a donné ETE
            Assert.IsTrue(j.MotTrouve("LAIT")); //LAIT est déjà placé sur la grille à la verticale
            Assert.IsFalse(j.MotTrouve("NOELS")); //Il n'y a pas de S après NOEL qui ait été ajouté
            Assert.IsFalse(j.MotTrouve("EST")); //Le mot EST n'a été ajouté nulle part sur la grille
        }

        [TestMethod]
        public void CroisementIsolement()
        {
          //La méthode Joueur.CroiserMot a été faite en partant à l'indice 0 de la grille, et non 1, contrairement aux autres méthodes, erreur
          //d'innatention qui ne m'a pas parue gênante au point de devoir changer ça
          //Ici, les mots n'ont pas besoin d'avoir un sens ou d'appartenir au dictionnaire, mais je garde des mots existants pour que ça reste un peu clair
                               // 0    1    2    3    4    5
            char[,] grille = { { ' ', ' ', ' ', ' ', ' ', ' '},   //0
                               { 'N', 'O', 'E', 'L', ' ', ' '},   //1
                               { ' ', ' ', ' ', 'A', 'I', 'R'},   //2
                               { ' ', ' ', ' ', 'I', ' ', ' '},   //3
                               { ' ', ' ', 'E', 'T', ' ', ' '},   //4
                               { ' ', ' ', ' ', ' ', ' ', ' '} }; //5
            j.Grille = grille;
            j.AddLettres(1, piocheTest, r); //On pioche juste une lettre au hasard pour initialiser la liste de lettres
            //que le joueur possède les lettres ou non n'a pas d'importance ici
            Assert.IsTrue(j.CroiserMot("JE", 0, 2, false)); //Il y a bien un E en dessous de l'endroit où l'on souhaite mettre un J pour JE en vertical
            Assert.IsTrue(j.CroiserMot("JETTE", 0, 2, false)); //Le mot croise 2 E sur la grille qui coincident avec le mot
            Assert.IsFalse(j.CroiserMot("JETTA", 0, 2, false)); //Le A de JETTA ne coincide pas avec le E qu'il croise
            Assert.IsFalse(j.CroiserMot("JE", 0, 1, false)); //Le E de JE ne coincide pas avec le O qu'il croise
            Assert.IsTrue(j.CroiserMot("FLAIR", 2, 1, true)); //Toutes les lettres de AIR coincident avec celles de FLAIR
            Assert.IsTrue(j.CroiserMot("FLA", 2, 1, true)); //Même si le mot continue, les premières lettres peuvent être posées car les A coincident
            Assert.IsFalse(j.CroiserMot("LE", 4, 0, false)); //LE ne peut pas être placé ici car il est isolé
            Assert.IsTrue(j.CroiserMot("LE", 4, 0, true)); //Même problème ici, horizontalement comme verticalement
            Assert.IsTrue(j.CroiserMot("E", 4, 4, true)); //E peut être placé ici car il touche un autre mot, pour former ETE
        }

        [TestMethod]
        public void BonusScore()
        {
                               // 1    2    3    4    5    6
            char[,] grille = { { ' ', ' ', ' ', ' ', ' ', ' '},   //1
                               { 'N', 'O', 'E', 'L', 'S', ' '},   //2
                               { ' ', ' ', ' ', 'A', 'I', 'R'},   //3
                               { ' ', ' ', ' ', 'I', ' ', ' '},   //4
                               { 'M', 'E', 'T', 'T', 'R', 'E'},   //5
                               { ' ', ' ', ' ', ' ', ' ', ' '} }; //6
            j.AddLettres(200, piochePrincipale, r); //Le joueur pioche des lettres
            j.Grille = grille;
            Assert.AreEqual(j.Score, 0); //Le score initial du joueur est initialement de 0
            j.ScoreBonus(); //Il n'a trouvé aucun mot donc aucun bonus n'est appliqué
            Assert.AreEqual(j.Score, 0);
            j.VerificationMots(dictionnaire, "ENTAME", 1, 1, false); 
            //Il place entame, et la méthode lui ajoute tous les mots de la grille s'ils existent dans sa liste de mots trouvés
            int score = j.Score; //On calcule le score que lui a donné le placement de ENTAME
            j.ScoreBonus(); //On applique le bonus
            Assert.AreEqual(j.Score, 6 + 6 + score);
            //Le joueur obtient 0 points pour les mots de moins de 5 lettres, 0 points pour NOELS qui n'existe pas dans le dictionnaire,
            //6 points pour ENTAME et 6 points pour METTRE, qui possèdent tous les deux 6 lettres
        }
    }
}
