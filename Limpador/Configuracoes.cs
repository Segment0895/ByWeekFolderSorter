using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Limpador
{

    public class Dados
    {
        static public string USERDIR = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public string[] DIRS = { @"C:\pedro\Dropbox\Downloads", @"C:\pedro\Dropbox\Desktop" };
        public string[] IGNORAR = { @"desktop.ini", @"TRALHA"};
    }

    public class Configuracoes
    {
        public static Dados confs = null;

        private Configuracoes()
        {
        }

        public static Dados GetConfs()
        {
            string CAMINHO = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var path = Path.Combine(Path.GetDirectoryName(CAMINHO), "CONFIGURACOES.txt");

            if (confs == null)
            {
                confs = new Dados();
                if (!File.Exists(path))
                {
                    var json = new JavaScriptSerializer().Serialize(confs);
                    File.WriteAllText(path, json);
                }
                else
                {
                    var json = File.ReadAllText(path);
                    confs = new JavaScriptSerializer().Deserialize<Dados>(json);
                }
            }

            return confs;
        }
    }
}
