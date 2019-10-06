using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace SmartMirrorRpPi
{
     public class Faces
    {

        public List<byte[]> imageList = new List<byte[]>();
        public List<string> Names = new List<string>();
        public byte[] newFace;
        public Faces()
        {
           // DownloadFaces();
        }
         public async Task DownloadFaces()
        {
            var InstallationFolder = KnownFolders.SavedPictures;

            IReadOnlyList<StorageFile> files;
            files = await InstallationFolder.GetFilesAsync();

            byte[] array;

            for (int i = 0; i <= files.Count - 1; i++)
            {
                var file2 = await InstallationFolder.GetFileAsync(files[i].Name);
                var elo = files[i].Name;
                using (Stream imagestream = await file2.OpenStreamForReadAsync())
                {
                    Names.Add(files[i].Name);
                    //Change png to pgm
                    BitmapDecoder dec = await BitmapDecoder.CreateAsync(imagestream.AsRandomAccessStream());
                    var data = await dec.GetPixelDataAsync();
                    var pixels = data.DetachPixelData();
                    int l = pixels.Length;
                    array = new byte[l / 4];
                    byte r, g, b;
                    int n = 0;

                    for (int j = 0; j < l; j++)
                    {
                        j++;
                        r = pixels[j];
                        j++;
                        g = pixels[j];
                        j++;
                        b = pixels[j];

                        array[n] = (byte)((r + g + b) / 3);
                        n++;
                    }
                    n = 0;

                    //Histogram equalization
                    byte Lmin = array[0], Lmax = array[0];
                    for (int j = 1; j < l / 4; j++)
                    {
                        if (array[j] > Lmax)
                        {
                            Lmax = array[j];
                        }
                        if (array[j] < Lmin)
                        {
                            Lmin = array[j];
                        }
                    }

                    for (int j = 0; j < l / 4; j++)
                    {
                        array[j] = (byte)((array[j] - Lmin) * (255) / (Lmax - Lmin));
                    }
                };
                imageList.Add(array);

            }
        }

        public async Task addNewFace()
        {
            var InstallationFolder = KnownFolders.SavedPictures;
            IReadOnlyList<StorageFile> files = await InstallationFolder.GetFilesAsync();

            var file = await InstallationFolder.GetFileAsync(files[files.Count -1].Name);
            using (Stream imagestream = await file.OpenStreamForReadAsync())
            {
                //Change png to pgm
                BitmapDecoder dec = await BitmapDecoder.CreateAsync(imagestream.AsRandomAccessStream());
                var data = await dec.GetPixelDataAsync();
                var pixels = data.DetachPixelData();
                int l = pixels.Length;
                newFace = new byte[l / 4];
                byte r, g, b;
                int n = 0;

                for (int j = 0; j < l; j++)
                {
                    j++;
                    r = pixels[j];
                    j++;
                    g = pixels[j];
                    j++;
                    b = pixels[j];

                    newFace[n] = (byte)((r + g + b) / 3);
                    n++;
                }
                n = 0;

                //Histogram equalization
                byte Lmin = newFace[0], Lmax = newFace[0];
                for (int j = 1; j < l / 4; j++)
                {
                    if (newFace[j] > Lmax)
                    {
                        Lmax = newFace[j];
                    }
                    if (newFace[j] < Lmin)
                    {
                        Lmin = newFace[j];
                    }
                }

                for (int j = 0; j < l / 4; j++)
                {
                    newFace[j] = (byte)((newFace[j] - Lmin) * (255) / (Lmax - Lmin));
                }
            };
        }

        public string getName(int number)
        {

            return Names[number];
        }


    }
}
