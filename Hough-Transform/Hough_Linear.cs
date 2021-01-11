using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;

namespace Hough_Transform
{
    class Hough_Linear
    {
        // The size of the neighbourhood in which to search for other local maxima 
        static int neighbourhoodSize = 4;

        // How many discrete values of theta shall we check? 
        static int maxTheta = 180;

        // Using maxTheta, work out the step 
        double thetaStep = Math.PI / maxTheta;

        // the width and height of the image 
        protected int width, height;

        // the hough array 
        protected int[,] houghArray;

        // the coordinates of the centre of the image 
        protected float centerX, centerY;

        // the height of the hough array 
        protected int houghHeight;

        // double the hough height (allows for negative numbers) 
        protected int doubleHeight;

        // the number of points that have been added 
        protected int numPoints;

        // cache of values of sin and cos for different theta values. Has a significant performance improvement. 
        private double[] sinCache;
        private double[] cosCache;
        Bitmap img;

        public Hough_Linear(Bitmap image)
        {
            img = image;
            initialise(image.Width, image.Height);
            addPoints(image);
        }

        /** 
   * Initialises the hough array. Called by the constructor so you don't need to call it 
   * yourself, however you can use it to reset the transform if you want to plug in another 
   * image (although that image must have the same width and height) 
   */
        public void initialise(int width, int height)
        {
            this.width = width;
            this.height = height;
   
            // Calculate the maximum height the hough array needs to have 
            houghHeight = (int)(Math.Sqrt(2) * Math.Max(height, width)) / 2;

            // Double the height of the hough array to cope with negative r values 
            doubleHeight = 2 * houghHeight;

            // Create the hough array
            houghArray = new int[maxTheta, doubleHeight];

            // Find edge points and vote in array 
            centerX = width / 2;
            centerY = height / 2;

            // Count how many points there are 
            numPoints = 0;

            // cache the values of sin and cos for faster processing 
            sinCache = new double[maxTheta];
            cosCache = new double[maxTheta];
            for (int t = 0; t < maxTheta; t++)
            {
                double realTheta = t * thetaStep;
                sinCache[t] = Math.Sin(realTheta);
                cosCache[t] = Math.Cos(realTheta);
            }
        }

        /** 
   * Adds points from an image. The image is assumed to be greyscale black and white, so all pixels that are 
   * not black are counted as edges. The image should have the same dimensions as the one passed to the constructor. 
   */
        public void addPoints(Bitmap image)
        {

            // Now find edge points and update the hough array 
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    // Find non-black pixels 
                    if (image.GetPixel(x, y).ToArgb() != Color.Black.ToArgb())
                    {
                        addPoint(x, y);
                    }
                }
            }
        }

        /** 
         * Adds a single point to the hough transform. You can use this method directly 
         * if your data isn't represented as a buffered image. 
         */
        public void addPoint(int x, int y)
        {

            // Go through each value of theta 
            for (int t = 0; t < maxTheta; t++)
            {

                //Work out the r values for each theta step 
                int r = (int)(((x - centerX) * cosCache[t]) + ((y - centerY) * sinCache[t]));

                // this copes with negative values of r 
                r += houghHeight;

                if (r < 0 || r >= doubleHeight) continue;

                // Increment the hough array 
                houghArray[t,r]++;
            }

            numPoints++;
        }
        /** 
   * Once points have been added in some way this method extracts the lines and returns them as a Vector 
   * of HoughLine objects, which can be used to draw on the 
   * 
   * @param percentageThreshold The percentage threshold above which lines are determined from the hough array 
   */
        public List<HoughLine> getLines(int n, int threshold)
        {

            // Initialise the vector of lines that we'll return 
            List<HoughLine> lines = new List<HoughLine>(20);

            // Only proceed if the hough array is not empty 
            if (numPoints == 0) return lines;

            // Search for local peaks above threshold to draw 
            for (int t = 0; t < maxTheta; t++)
            {
                for (int r = neighbourhoodSize; r < doubleHeight - neighbourhoodSize; r++)
                {

                    // Only consider points above threshold 
                    if (houghArray[t,r] > threshold)
                    {

                        int peak = houghArray[t,r];

                        // Check that this peak is indeed the local maxima 
                        for (int dx = -neighbourhoodSize; dx <= neighbourhoodSize; dx++)
                        {
                            for (int dy = -neighbourhoodSize; dy <= neighbourhoodSize; dy++)
                            {
                                int dt = t + dx;
                                int dr = r + dy;
                                if (dt < 0) dt = dt + maxTheta;
                                else if (dt >= maxTheta) dt = dt - maxTheta;
                                if (houghArray[dt, dr] > peak)
                                {
                                    // found a bigger point nearby, skip 
                                    goto end_of_loop;
                                }
                            }
                        }
                        // calculate the true value of theta 
                        double theta = t * thetaStep;

                        // add the line to the vector 
                        lines.Add(new HoughLine(theta, r, width, height, houghArray[t,r]));
                    }
                }
                end_of_loop: { }
            }
            lines.Sort(delegate (HoughLine x, HoughLine y) {
                return y.compareTo(x);
            });
            if(lines.Count - n > 0)
                lines.RemoveRange(n, lines.Count - n);
            return lines;
        }

        public void draw()
        {

            List<HoughLine> lines = this.getLines(8, 90);  
            Bitmap bm = new Bitmap(img.Width, img.Height);
            Graphics gr = Graphics.FromImage(bm);
            gr.DrawImage(img, 0, 0);
            if (lines != null)
            {
                using (gr)
                {
                    using (Pen thick_pen = new Pen(Color.FromArgb(100, 255, 0, 0), 1))
                    {
                        for (int j = 0; j < lines.Count; j++)
                        {
                            HoughLine l = lines[j];
                            gr.DrawLine(thick_pen, new Point(l.x1, l.y1), new Point(l.x2, l.y2));
                        }

                    }
                }
            bm.Save("../../../Images/output.png");
            }            
        }
    }
}
