using System;

namespace Hough_Transform
{
    class Main_Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Pick your poison: \n" +
                "0.exit \n"+
                "1.Linear \n" +
                "2.Threads \n" +
                "3.Processes \n");

                String option = Console.ReadLine();

                if (option == "0")
                {
                    break;
                }

                else if (option == "1")
                {
                    Hough_Linear.start();
                }
                else if (option == "2")
                {
                    Hough_Thread.start();

                }
                else if (option == "3")
                {
                    Hough_Process.start();
                }
            }
        }
    }
}
