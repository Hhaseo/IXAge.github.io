
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IXAge_IHM.Shared.Parsing;

namespace Legendary_ConvertPDF
{


    class Program
    {
        static void Main(string[] args)
        {
            //pg.Load(@"C:/Users/flori/source/repos/IXAge/Files/T9A-FB_2ed_VC_2022_EN.pdf");
            //DirectoryInfo diTop = new DirectoryInfo(@"../../../../Files/");
            DirectoryInfo diTop = new DirectoryInfo(@"../../../../Files/Source/");
            //List<string> dirs = new List<string>(Directory.EnumerateDirectories(docPath));

            foreach (var dir in diTop.EnumerateFiles())
            {
                Console.WriteLine("Load File : " + dir.Name);
                var pg = new Parsing();
                pg.Load(dir.FullName);
                Console.WriteLine(pg.ToString());
                Console.ReadLine();
            }
        }
    }
}
