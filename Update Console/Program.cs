using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Update
{
    class Program
    {
        /// <summary>
        /// Нужно передать путь к архиву, путь к запускаемой программе
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine(args.Count());
            if ( args.Count() >= 2 )
            {
                Console.WriteLine("Параметры для разархивирование программы есть.");
                archive(args[0]);
                Process.Start(string.Join(" ", args.Skip(1)));
            }
            else if ( args.Count() == 0 )
            {
                Console.WriteLine("Нет параметров.");
            }
            //Console.ReadKey();
        }

        /// <summary>
        /// Распаковать архив
        /// </summary>
        /// <param name="pathFile">Путь к архиву</param>
        static void archive(string pathFile)
        {
            using ( var archFile = new ArchiveFile( pathFile) )
            {
                archFile.Extract("");//распаковываем в текущую дерикторию файл
            }
        }
    }
}
