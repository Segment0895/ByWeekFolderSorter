using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limpador
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new Limpador(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads");
            a.OrganizeFiles();

            var b = new Limpador(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Desktop");
            b.OrganizeFiles();

            Console.WriteLine("Press any key to continue . . . ");
            Console.ReadKey();
        }
    }
}
