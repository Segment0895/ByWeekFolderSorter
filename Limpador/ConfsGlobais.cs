using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Limpador
{

    public class DadosGlobais
    {
        static public string USERDIR = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public string[] DIRS = { @"C:\pedro\Dropbox\Downloads", @"C:\pedro\Dropbox\Desktop" };
        public string[] IGNORAR1FILES = { @"desktop.ini", "TRALHA"};
        public string[] IGNORAR2REGEX = { @"^([0-9]{4})-week(5[0-3]|[1-4][0-9]|0[1-9])$" };
    }

    public class ConfiguracoesGlobais
    {
        public static DadosGlobais confs = null;

        private ConfiguracoesGlobais()
        {
        }

        public static DadosGlobais GetConfs()
        {
            string CAMINHO = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var path = Path.Combine(Path.GetDirectoryName(CAMINHO), "CONFIGURACOES.txt");

            if (confs == null)
            {
                confs = new DadosGlobais();
                if (!File.Exists(path))
                {
                    var json = new JavaScriptSerializer().Serialize(confs);
                    File.WriteAllText(path, json);
                }
                else
                {
                    var json = File.ReadAllText(path);
                    confs = new JavaScriptSerializer().Deserialize<DadosGlobais>(json);
                }
            }

            return confs;
        }
    }
}
