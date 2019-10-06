using Accord.Math;
using Accord.Math.Decompositions;
using Accord.Statistics.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SmartMirrorRpPi
{
    class FaceRecognition
    {

        Double[,] R;
        PrincipalComponentAnalysis pca;

        public void LearnAlgorithm(List<byte[]> imageList)
        {
            byte[] array = new byte[imageList[1].Length];
            byte[] avarageImage = new byte[imageList[1].Length];

            //Calculate avarage face
            for (int i = 0; i < imageList.Count(); i++)
            {
                array = imageList[i];
                for (int j = 0; j < array.Length; j++)
                {
                    avarageImage[j] = (byte)(avarageImage[j] + array[j]);
                }

            }

            for (int j = 0; j < array.Length; j++)
            {
                avarageImage[j] = (byte)(avarageImage[j] / imageList.Count());
            }

            //Make matrix of images

            Double[,] T = new Double[imageList.Count(), avarageImage.Length];

            for (int i = 0; i < imageList.Count(); i++)
            {
                array = imageList[i];
                for (int j = 0; j < avarageImage.Length; j++)
                {

                    T[i, j] = array[j] - avarageImage[j];
                }
            }

            //Making mathematical things

            Double[,] TT = T.Transpose();
            Double[,] C = Accord.Math.Matrix.Multiply(T, TT);
            EigenvalueDecomposition dec2 = new Accord.Math.Decompositions.EigenvalueDecomposition(C);
            Double[,] V = dec2.Eigenvectors;
            Double[,] E = Accord.Math.Matrix.Multiply(TT, V); ; //EE actually is eigenvectors matrix
            E = E.Transpose();
            pca = new PrincipalComponentAnalysis(E);
            pca.Compute();
            R = pca.Transform(T);
        }

        public int RecognizeNewFace(byte[] newPerson)
        {
            Double[] img = new Double[newPerson.Count()];

                for (int i = 0; i < newPerson.Length; i++)
                {
                    img[i] =newPerson[i];
                }
            

            Double[] projection = pca.Transform(img);
            int best = 0;
            double distance = double.MaxValue;
            for (int i = 0; i < R.GetLength(1); i++)
            {
                double sum = 0;

                for (int j = 0; j < R.GetLength(0); j++)
                {
                    sum += Math.Pow(projection[j] - R[i, j], 2);
                }

                if (sum < distance)
                {
                    distance = sum;
                    best = i;
                }
            }

            return best;
        }
    }
}
