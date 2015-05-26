using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Limpador
{
    public class Limpador
    {
        private string destino = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +  @"\TRALHA";
        private DirectoryInfo di = null;

        public Limpador(string dir) {
            di = new DirectoryInfo(dir);
        }


        public List<FileSystemInfo> GetFileList()
        {
            List<FileSystemInfo> retorno = new List<FileSystemInfo>();

            foreach (FileSystemInfo fi in di.EnumerateFileSystemInfos())
            {
                DateTime antigo = fi.LastAccessTime;

                if (!SanityCheck(fi))
                    continue;

                TimeSpan ts = DateTime.Now - antigo;
                if (ts.TotalHours > 72) {
                    Console.Write("YES: ");
                    Console.WriteLine(fi.Name + " " + antigo + " to " + WeeksInYear(antigo));
                    retorno.Add(fi);
                }
                else
                {
                    Console.Write("NO: ");
                    Console.WriteLine(fi.Name + " " + antigo + " to " + WeeksInYear(antigo));
                }
            }

            return retorno;
        }


        public static int WeeksInYear(DateTime date)
        {
            GregorianCalendar cal = new GregorianCalendar(GregorianCalendarTypes.Localized);
            return cal.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Wednesday);
        }

        public void OrganizeFiles()
        {
            //Console.Out.WriteLine("ok");
            List<FileSystemInfo> ficheiros = GetFileList();

            DirectoryInfo di2 = new DirectoryInfo(destino);

            foreach (FileSystemInfo fi in ficheiros)
            {
                if (!SanityCheck(fi))
                    continue;

                int week = WeeksInYear(fi.LastAccessTime);

                DirectoryInfo destino2 = di2.CreateSubdirectory(DateTime.Now.Year + "-week" + week);
                //Console.Out.WriteLine(fi.Name + "->" + destino2.Name);

                if (fi is FileInfo) {
                    var destinoEmTralha = new FileInfo(destino2.ToString() + Path.DirectorySeparatorChar + fi.Name);
                    if (destinoEmTralha.Exists)
                    {
                        FileInfo Original = (FileInfo)fi;
                        var existente = GetChecksumBuffered(new FileStream(destinoEmTralha.ToString(), FileMode.Open));
                        var candidato = GetChecksumBuffered(new FileStream(destino2.ToString() + Path.DirectorySeparatorChar + Original.ToString(), FileMode.Open));

                        Console.WriteLine("IGNORADO DUP: " + fi.Name);
                    } else {
                        ((FileInfo)fi).MoveTo(destinoEmTralha.ToString());
                    }
                }
                else if (fi is DirectoryInfo)
                {
                    ((DirectoryInfo)fi).MoveTo(destino2.ToString() + Path.DirectorySeparatorChar + fi.Name);
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        private bool SanityCheck(FileSystemInfo fi)
        {
            TimeSpan ts1 = fi.LastWriteTime - fi.LastAccessTime;
            if (Math.Abs(ts1.TotalHours) > 1)
            {
                Console.WriteLine("IMPOSSIBLE: " + fi.Name + " " + fi.LastAccessTime + " VS " + fi.LastWriteTime);
                return false;
            }

            return true;

        }

        private string GetChecksumBuffered(Stream stream)
        {
            using (var bufferedStream = new BufferedStream(stream, 1024 * 32))
            {
                var sha = new SHA256Managed();
                byte[] checksum = sha.ComputeHash(bufferedStream);
                return BitConverter.ToString(checksum).Replace("-", String.Empty);
            }
        }
    }
}
