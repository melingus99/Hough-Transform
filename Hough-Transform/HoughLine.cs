using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Hough_Transform
{
    class HoughLine
    {
        protected double theta;
        protected double r;
        protected int score;
        public int x1;
        public int x2;
        public int y1;
        public int y2;
        /** 
         * Initialises the hough line 
         */
        public HoughLine(double theta, double r, int width, int height, int score)
        {
            this.theta = theta;
            this.r = r;
            this.score = score;

            // During processing h_h is doubled so that -ve r values 
            int houghHeight = (int)(Math.Sqrt(2) * Math.Max(height, width)) / 2;

            // Find edge points and vote in array 
            float centerX = width / 2;
            float centerY = height / 2;

            // Draw edges in output array 
            double tsin = Math.Sin(theta);
            double tcos = Math.Cos(theta);

            int x1, x2, y1, y2;

            if (theta < Math.PI * 0.25 || theta > Math.PI * 0.75)
            {
                x1 = 0;
                y1 = 0;
                x2 = 0;
                y2 = height - 1;

                x1 = (int)((((r - houghHeight) - ((y1 - centerY) * tsin)) / tcos) + centerX);
                x2 = (int)((((r - houghHeight) - ((y2 - centerY) * tsin)) / tcos) + centerX);


            }
            else
            {
                x1 = 0;
                y1 = 0;
                x2 = width - 1;
                y2 = 0;

                y1 = (int)((((r - houghHeight) - ((x1 - centerX) * tcos)) / tsin) + centerY);
                y2 = (int)((((r - houghHeight) - ((x2 - centerX) * tcos)) / tsin) + centerY);

            }
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
        }

        public int compareTo(HoughLine o)
        {
            return (this.score - o.score);
        }
    }
}
