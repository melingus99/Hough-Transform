using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Hough_Transform
{
    class Processor
    {
        String imgPath;
        Bitmap img;

        public Processor(String filePath)
        {
            this.imgPath = filePath;
            img = new Bitmap(filePath);
        }

        //makes the image given in the constructor gray.
        public Bitmap rgb2gray()
        {
            Bitmap grayImg = new Bitmap(img.Width, img.Height);

            using (Graphics g = Graphics.FromImage(grayImg))
            {

                ColorMatrix colorMatrix = new ColorMatrix(
                   new float[][]
                   {
             new float[] {.3f, .3f, .3f, 0, 0},
             new float[] {.59f, .59f, .59f, 0, 0},
             new float[] {.11f, .11f, .11f, 0, 0},
             new float[] {0, 0, 0, 1, 0},
             new float[] {0, 0, 0, 0, 1}
                   });

                using (ImageAttributes attributes = new ImageAttributes())
                {

                    attributes.SetColorMatrix(colorMatrix);

                    g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height),
                                0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return grayImg;

        }
    }
}
