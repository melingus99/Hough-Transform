using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Hough_Transform
{
class Hough_Process
{
        static int neighbourhoodSize = 4;

        public static int maxTheta = 180;

        double thetaStep = Math.PI / maxTheta;

        protected int width, height;

        public int[,] houghArray;

        protected float centerX, centerY;

        protected int houghHeight;

        public int doubleHeight;

        protected int numPoints;

        private double[] sinCache;
        private double[] cosCache;
        Bitmap img;

        public Hough_Process(Bitmap image)
        {
            img = image;
            initialise(image.Width, image.Height);
        }

        public void getSlice(MPI.Communicator comm, out int startX, out int startY, out int len)
        {
            int size = img.Width * img.Height;
            len = size / (comm.Size - 1);

            startX = len * (comm.Rank - 1) / img.Height;
            startY = len * (comm.Rank - 1) % img.Height;
        }

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

   
        public void addPoints(MPI.Communicator comm, int startX, int startY, int len)
        {
            int[,] hough = new int[maxTheta, doubleHeight];
            for (int x = startX; x < img.Width && len != 0; x++)
            {
                for (int y = startY; y < img.Height && len != 0; y++, len--)
                {
                    if (img.GetPixel(x, y).ToArgb() != Color.Black.ToArgb())
                    {
                        addPoint(hough, x, y);
                    }
                }
            }
            comm.Send(hough, 0, 0);
        }

        public void addPoint(int[,] hough, int x, int y)
        {
            for (int t = 0; t < maxTheta; t++)
            {
                int r = (int)(((x - centerX) * cosCache[t]) + ((y - centerY) * sinCache[t]));

                r += houghHeight;

                if (r < 0 || r >= doubleHeight) continue;

                hough[t, r]++;
            }
        }
        public List<HoughLine> getLines(int n, int threshold)
        {
            List<HoughLine> lines = new List<HoughLine>();

            for (int t = 0; t < maxTheta; t++)
            {
                for (int r = neighbourhoodSize; r < doubleHeight - neighbourhoodSize; r++)
                {

                    if (houghArray[t, r] > threshold)
                    {
                        int peak = houghArray[t, r];

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
                                    goto end_of_loop;
                                }
                            }
                        }
                        double theta = t * thetaStep;

                        lines.Add(new HoughLine(theta, r, width, height, houghArray[t, r]));
                    }
                }
            end_of_loop: { }
            }
            lines.Sort(delegate (HoughLine x, HoughLine y) {
                return y.compareTo(x);
            });
            if (lines.Count - n > 0)
                lines.RemoveRange(n, lines.Count - n);
            return lines;
        }

        public void draw()
        {

            List<HoughLine> lines = this.getLines(8, 20);
            Bitmap bm = new Bitmap(img.Width, img.Height);
            Graphics gr = Graphics.FromImage(bm);
            gr.DrawImage(img, 0, 0);
            if (lines != null)
            {
                using (gr)
                {
                    using (Pen thick_pen = new Pen(Color.FromArgb(100, 255, 0, 0), 1))
                    {
                        foreach (var l in lines)
                        {
                            gr.DrawLine(thick_pen, new Point(l.x1, l.y1), new Point(l.x2, l.y2));
                        }

                    }
                }
                bm.Save("../../../Images/output.png");
            }
        }

        public static void start(Bitmap image, string[] args)
        {
            using (new MPI.Environment(ref args))
            {
                var comm = MPI.Communicator.world;

                Hough_Process h = new Hough_Process(image);
                if (comm.Rank == 0)
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    for (int i = 0; i < comm.Size - 1; i++)
                    {
                        comm.Receive(MPI.Communicator.anySource, MPI.Communicator.anyTag, out int[,] hou);
                        for (int j = 0; j < Hough_Process.maxTheta; j++)
                        {
                            for (int k = 0; k < h.doubleHeight; k++)
                            {
                                h.houghArray[j, k] += hou[j, k];
                            }
                        }
                    }
                    h.draw();
                    watch.Stop();
                    Console.WriteLine("Master done");
                    Console.WriteLine($"Done in {watch.ElapsedMilliseconds} ms");
                }
                else
                {
                    h.getSlice(comm, out int startX, out int startY, out int len);
                    h.addPoints(comm, startX, startY, len);
                    Console.WriteLine("Worker done");
                }
            }

        }
    }
}
