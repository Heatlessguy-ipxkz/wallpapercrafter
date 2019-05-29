using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPCParseProvider;

namespace Wallpapercrafter
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser ParseUnit = new Parser("hi-tech", "1920x1080");
            ParseUnit.GetWallpapers();

            Console.WriteLine("Press Enter to continue");
            Console.ReadKey();
        }
    }
}
