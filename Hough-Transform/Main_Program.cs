using System;
using System.Drawing;
namespace Hough_Transform
{
    class Main_Program
    {
        static void Main(string[] args)
        {
            Bitmap image = new Bitmap("../../../Images/image4.png");

            if (args.Length != 0)
            {
                //Hough_Process.start(image, args);
            }
            else
            {
                while (true) {
                    Console.WriteLine("Pick your poison: \n" +
                    "0.exit \n" +
                    "1.Linear \n" +
                    "2.Threads \n");

                    String option = Console.ReadLine();

                    if (option == "0")
                    {
                        break;
                    }

                    else if (option == "1")
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        Hough_Linear h = new Hough_Linear(image);
                        h.draw();
                        watch.Stop();
                        Console.WriteLine(watch.ElapsedMilliseconds.ToString() + " ms");
                    }
                    else if (option == "2")
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        Hough_Thread h = new Hough_Thread(image);
                        h.draw();
                        watch.Stop();
                        Console.WriteLine(watch.ElapsedMilliseconds.ToString()+" ms");

                    }
                }
            }
            
        }
    }
}
