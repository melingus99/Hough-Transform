using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Hough_Transform
{
    class Hough_Linear
    {

        public static void start()
        {
            Processor processor = new Processor("C:\\Users\\Bubu\\source\\repos\\Hough-Transform\\Hough-Transform\\Images\\Boogie.jpg");
            Bitmap image = processor.rgb2gray();



        }
    }
}
