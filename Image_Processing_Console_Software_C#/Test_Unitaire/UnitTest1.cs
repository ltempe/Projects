using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Traitement_Images
{
    [TestClass]
    public class UnitTest1
    {
        MonImage img = new MonImage("images\\testrvb.bmp");
        MonImage coco = new MonImage("images\\coco.bmp");

        [TestMethod]
        public void Conversion_EndianEntier()
        {
            byte[] endian = { 56, 3, 4, 0, 154, 4, 0, 0, 1 };
            Assert.AreEqual(MonImage.ConvertirEndianEntier(endian, 0, 0), 56);
            Assert.AreEqual(MonImage.ConvertirEndianEntier(endian, 1, 2), 3 + 4 * 256);
            Assert.AreEqual(MonImage.ConvertirEndianEntier(endian, 2, 4), 4 + 154 * 256 * 256);
            Assert.AreEqual(MonImage.ConvertirEndianEntier(endian, 5, 8), 4 + 256 * 256 * 256);
        }

        [TestMethod]
        public void Conversion_EntierEndian()
        {
            Assert.AreEqual(MonImage.ConvertirEntierEndian(257, 2)[0], 1);
            Assert.AreEqual(MonImage.ConvertirEntierEndian(257, 2)[1], 1);
            Assert.AreEqual(MonImage.ConvertirEntierEndian(257, 5)[3], 0);
            Assert.AreEqual(MonImage.ConvertirEntierEndian(8*256*256*256, 4)[3], 8);
        }

        [TestMethod]
        public void CreationImage()
        {
            Assert.AreEqual(img.Hauteur, 4);
            Assert.AreEqual(img.Largeur, 8);
            Assert.AreEqual(img.NbBits, 24);
            img.EnregistrerImage("sortie\\rvb.bmp");

            MonImage img2 = new MonImage("sortie\\rvb.bmp");
            Assert.IsTrue(img.EstEgale(img2));

            MonImage img3 = new MonImage(img);
            Assert.IsTrue(img.EstEgale(img3));


        }

        [TestMethod]
        public void Pivot180Deg()
        {
            MonImage cocoPivot = new MonImage(coco);
            cocoPivot.Pivoter180();
            cocoPivot.EnregistrerImage("sortie\\pivot180.bmp");
            MonImage test = new MonImage("images\\cocoPivot180.bmp"); //image retournée avec Paint
            Assert.IsTrue(cocoPivot.EstEgale(test));
        }

        [TestMethod]
        public void RotationY()
        {
            MonImage cocoRY = new MonImage(coco);
            cocoRY.SymetrieVerticale();
            cocoRY.EnregistrerImage("sortie\\RotY.bmp");
            MonImage test = new MonImage("images\\cocoRotY.bmp"); //image retournée avec Paint
            Assert.IsTrue(cocoRY.EstEgale(test));
        }

        [TestMethod]
        public void RotationX()
        {
            MonImage cocoRX = new MonImage(coco);
            cocoRX.SymetrieHorizontale();
            cocoRX.EnregistrerImage("sortie\\RotX.bmp");
            MonImage test = new MonImage("images\\cocoRotX.bmp"); //image retournée avec Paint
            Assert.IsTrue(cocoRX.EstEgale(test));
        }

        [TestMethod]
        public void CouleursNegatif()
        {
            MonImage cocoNeg = new MonImage(coco);
            cocoNeg.Negatif();
            cocoNeg.EnregistrerImage("sortie\\negatif.bmp");
            MonImage test = new MonImage("images\\cocoNegatif.bmp"); //image retournée avec Paint
            Assert.IsTrue(cocoNeg.EstEgale(test));
        }

        [TestMethod]
        public void GrandirRetrecir()
        {
            MonImage cocoGrand = new MonImage(coco);
            cocoGrand.ChangerTaille(200);
            Assert.AreEqual(cocoGrand.Hauteur, 2 * coco.Hauteur);
            Assert.AreEqual(cocoGrand.Largeur, 2 * coco.Largeur);
            Assert.IsFalse(coco.EstEgale(cocoGrand));
            cocoGrand.EnregistrerImage("sortie\\agrandir.bmp");
            cocoGrand.ChangerTaille(50);
            Assert.IsTrue(coco.EstEgale(cocoGrand));
        }

        Complexe i = new Complexe(0, 1);
        Complexe un = new Complexe(1, 0);
        Complexe z = new Complexe(1, 1);
        Complexe a = new Complexe(2, 5);
        Complexe b = new Complexe(4, 3);
        Complexe c = new Complexe(6, 8);

        [TestMethod]
        public void AdditionsComplexes()
        {
            Assert.IsTrue((a + b).Egal(c));
            Assert.IsTrue((c - a).Egal(b));
            Assert.IsTrue((un + i).Egal(z));
        }

        [TestMethod]
        public void MultiplicationComplexes()
        {
            Assert.AreEqual(i.Re, 0);
            Assert.AreEqual(i.Im, 1);
            Complexe resultat = i * i;
            Assert.AreEqual(resultat.Re, -1);
            Assert.AreEqual(resultat.Im, 0);
        }

        [TestMethod]
        public void Convergence()
        {
            z = new Complexe(0.1f, 0.1f);
            c = new Complexe(0.1f, 0.1f);
            Assert.AreEqual(MonImage.Converge(z, c, 50), 50);

            z = new Complexe(2, 0);
            c = new Complexe(0, 0);
            Assert.AreEqual(MonImage.Converge(z, c, 50), 0);

            z = new Complexe(0, 0);
            c = new Complexe(0, 2);
            Assert.AreEqual(MonImage.Converge(z, c, 50), 1);

            z = new Complexe(0.1f, 0.2f);
            c = new Complexe(0.5f, 0.2f);
            Assert.AreEqual(MonImage.Converge(z, c, 50), 6);

            z = new Complexe(0.2f, 0.15f);
            c = new Complexe(0.285f, 0.01f);
            Assert.AreEqual(MonImage.Converge(z, c, 50), 32);
        }

        [TestMethod]
        public void Stenographie()
        {
            MonImage img1 = new MonImage("images\\coco.bmp");
            MonImage img2 = new MonImage("images\\lena.bmp");

            int h = img1.Hauteur;
            int l = img1.Largeur;

            img1.CacherImage(img2);
            Assert.AreEqual(img1.Hauteur, h);
            Assert.AreEqual(img1.Largeur, l);

            Assert.AreNotEqual(img1.Hauteur, img2.Hauteur);
            Assert.AreNotEqual(img1.Largeur, img2.Largeur);

            img1.RecuperationImage();
            Assert.AreEqual(img1.Hauteur, img2.Hauteur);
            Assert.AreEqual(img1.Largeur, img2.Largeur);
        }

        [TestMethod]
        public void CalculMax()
        {
            int[] tab1 = { 1, 2, 3, 4, 5, 6 };
            int[] tab2 = { -15, 10, 8, -2, 7 };
            int[] tab3 = { 15, 28, 3, 72, 4, 72, 41 };
            Assert.AreEqual(MonImage.CalculerMax(tab1), 6);
            Assert.AreEqual(MonImage.CalculerMax(tab2), 10);
            Assert.AreEqual(MonImage.CalculerMax(tab3), 72);
        }

        [TestMethod]
        public void Conversion_OctetBinaireBool()
        {
            bool[] b255 = { true, true, true, true, true, true, true, true };
            bool[] b1 = { false, false, false, false, false, false, false, true };
            bool[] b45 = { false, false, true, false, true, true, false, true };
            bool[] b66 = { false, true, false, false, false, false, true, false };
            CollectionAssert.AreEqual(b255, MonImage.ConvertirOctetBinaire(255));
            CollectionAssert.AreEqual(b1, MonImage.ConvertirOctetBinaire(1));
            CollectionAssert.AreEqual(b45, MonImage.ConvertirOctetBinaire(45));
            CollectionAssert.AreEqual(b66, MonImage.ConvertirOctetBinaire(66));
            CollectionAssert.AreNotEqual(b66, MonImage.ConvertirOctetBinaire(45));
            CollectionAssert.AreNotEqual(b1, MonImage.ConvertirOctetBinaire(255));
        }

        [TestMethod]
        public void Conversion_BinaireOctetBool()
        {
            bool[] b255 = { true, true, true, true, true, true, true, true };
            bool[] b1 = { false, false, false, false, false, false, false, true };
            bool[] b45 = { false, false, true, false, true, true, false, true };
            bool[] b66 = { false, true, false, false, false, false, true, false };
            Assert.AreEqual(255, MonImage.ConvertirBinaireOctet(b255));
            Assert.AreEqual(1, MonImage.ConvertirBinaireOctet(b1));
            Assert.AreEqual(45, MonImage.ConvertirBinaireOctet(b45));
            Assert.AreEqual(66, MonImage.ConvertirBinaireOctet(b66));
            Assert.AreNotEqual(66, MonImage.ConvertirBinaireOctet(b45));
            Assert.AreNotEqual(1, MonImage.ConvertirBinaireOctet(b255));
        }

        [TestMethod]
        public void ConvRecursif_EntierBinaireString()
        {
            string b255 = "11111111";
            string b1 = "00001";
            string b45 = "101101";
            string b66 = "1000010";
            Assert.AreEqual(b255, QRCode.ConvertirVersBinaire(255));
            Assert.AreEqual(b255, QRCode.ConvertirVersBinaire(255, 8));
            Assert.AreNotEqual(b255, QRCode.ConvertirVersBinaire(255, 10));
            Assert.AreNotEqual(b255, QRCode.ConvertirVersBinaire(250));
            Assert.AreEqual(b1, QRCode.ConvertirVersBinaire(1, 5));
            Assert.AreNotEqual(b1, QRCode.ConvertirVersBinaire(1));
            Assert.AreEqual(b45, QRCode.ConvertirVersBinaire(45, 6));
            Assert.AreNotEqual(b45, QRCode.ConvertirVersBinaire(45));
            Assert.AreEqual(b66, QRCode.ConvertirVersBinaire(66, 7));
        }

        [TestMethod]
        public void ConvRecursif_BinaireEntierString()
        {
            string b255 = "11111111";
            string b1 = "00001";
            string b45 = "101101";
            string b66 = "1000010";
            Assert.AreEqual(255, QRCode.ConvertirVersEntier(b255));
            Assert.AreEqual(255, QRCode.ConvertirVersEntier("0000" + b255));
            Assert.AreEqual(1, QRCode.ConvertirVersEntier(b1));
            Assert.AreEqual(1, QRCode.ConvertirVersEntier("1"));
            Assert.AreEqual(45, QRCode.ConvertirVersEntier(b45));
            Assert.AreEqual(66, QRCode.ConvertirVersEntier(b66));
            Assert.AreNotEqual(45, QRCode.ConvertirVersEntier(b66));
        }

        [TestMethod]
        public void EstAlphaNumerique()
        {
            QRCode.CreerAlphaNumerique();
            string a = "BONJOUR TOUT LE MONDE";
            string b = "BONJOUR, TOUT LE MONDE";
            string c = "Bonjour tout le monde";
            string d = "BONJOUR!";
            string e = "E";
            string f = "f";
            Assert.IsTrue(QRCode.CodeAlphanum(a));
            Assert.IsFalse(QRCode.CodeAlphanum(b));
            Assert.IsFalse(QRCode.CodeAlphanum(c));
            Assert.IsTrue(QRCode.CodeAlphanum(c.ToUpper()));
            Assert.IsFalse(QRCode.CodeAlphanum(d));
            Assert.IsTrue(QRCode.CodeAlphanum(e));
            Assert.IsFalse(QRCode.CodeAlphanum(f));
        }

        [TestMethod]
        public void QRTest()
        {
            string test1 = "HELLO WORLD";
            string test2 = "Bonjour, tout le monde !";
            QRCode a = new QRCode(test1, 'L');
            Assert.AreEqual(a.Code, "001000000101101100001011011110001101000101110010110111000100" +
                "11010100001101000000111011000001000111101100000100011110110000010001111011000001" +
                "00011110110011010001111011111100010011001111010011101100001101101101");
            Assert.AreEqual(a.Version, 1);
            a.ImageQR();
            a.Img.EnregistrerImage("qrcodes\\test1.bmp");

            QRCode b = new QRCode("qrcodes\\test1.bmp");
            Assert.AreEqual(b.Version, 1);
            Assert.AreEqual(b.NiveauCorrection, 'L');
            Assert.AreEqual(b.Phrase, test1);

            a = new QRCode(test2, 'H');
            Assert.AreEqual(a.Code, "010000010101011110000100010000100010011000000110111101101100" +
                "01101110011001010010101001100000011011110111110101100101011111110110001000101110" +
                "01101100001001000110000001110101001001000110000000101111011100010000000110111100" +
                "01110000101100110111111110100001111100010001100111001110000001100001100110100011" +
                "10001111010010001000010011010111001010111110000010010000001010001000001111100000" +
                "01100000010011010000010000110111010000010111110111010100011010011000100000001010" +
                "10101011001100100111111100011111011110100001001000011010000111111100001111010100" +
                "100011110000000110000000000");
            Assert.AreEqual(a.Version, 3);
            a.ImageQR();
            a.Img.EnregistrerImage("qrcodes\\test2.bmp");

            b = new QRCode("qrcodes\\test2.bmp");
            Assert.AreEqual(b.Version, 3);
            Assert.AreEqual(b.NiveauCorrection, 'H');
            Assert.AreEqual(b.Phrase, test2);

        }
    }
}
