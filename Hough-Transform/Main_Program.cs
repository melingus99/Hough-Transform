using System;
using System.Drawing;
namespace Hough_Transform
{
    class Main_Program
    {
        static void Main(string[] args)
        {
            Bitmap image = new Bitmap("../../../Images/image1.png");
            
            if (args.Length != 0)
            {
                Hough_Process.start(image, args);
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
                        Hough_Linear h = new Hough_Linear(image);
                        h.draw();
                    }
                    else if (option == "2")
                    {
                        Hough_Thread.start();

                    }
                }
            }
            
        }
    }
}
