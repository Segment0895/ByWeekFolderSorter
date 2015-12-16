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
            //new Testes();
            //return;

            if (args.Count() == 0) {
                var CONFS = Configuracoes.GetConfs().DIRS;
                foreach (string str in CONFS)
                {
                    var obj = new Limpador(str);
                    obj.OrganizeFiles();
                }
            }

            Console.WriteLine("Press any key to continue . . . ");
            Console.ReadKey();
        }
    }
}
