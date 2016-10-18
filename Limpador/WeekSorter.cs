using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Limpador
{
    public class WeekSorter
    {

        private string alvo;
        private DirectoryInfo di = null;

        public WeekSorter(string dir)
        {
            try
            {
                di = new DirectoryInfo(dir);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(dir.ToString() + " não foi encontrada.");
                throw ex;
            }
            alvo = di.ToString() + @""; // antigamente era em TRALHA, agora já não para me obrigar a arrumar
        }


        public List<FileSystemInfo> GetFileList()
        {
            List<FileSystemInfo> retorno = new List<FileSystemInfo>();

            foreach (FileSystemInfo fi in di.EnumerateFileSystemInfos())
            {
                DateTime antigo;
                ObterUltimoAcessoOuMod(fi, out antigo);

                TimeSpan ts = DateTime.Now - antigo;
                if (ts.TotalHours > 72)
                {
                    retorno.Add(fi);
                }
                else
                {
                    Console.Write("IGNORED (LRU): ");
                    Console.WriteLine(fi.Name + " " + antigo + " to " + WeeksInYear(antigo));
                }
            }

            return retorno;
        }


        private static Calendar cal = CultureInfo.InvariantCulture.Calendar;
        public static int WeeksInYear(DateTime time)
        {
            // This presumes that weeks start with Monday.
            // Week 1 is the 1st week of the year with a Thursday in it.

            // Seriously cheat.  If it's Monday, Tuesday or Wednesday, then it'll
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = cal.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return cal.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public void OrganizeFiles()
        {
            //Console.Out.WriteLine("ok");
            List<FileSystemInfo> ficheiros = GetFileList();
            

            DirectoryInfo di2 = new DirectoryInfo(alvo);

            foreach (FileSystemInfo fi in ficheiros)
            {
                bool aIgnorar = false;

                var CONFS1 = ConfiguracoesGlobais.GetConfs().IGNORAR1FILES;
                foreach (string str in CONFS1)
                {
                    if (String.Compare(fi.Name, str, true) == 0)
                    {
                        aIgnorar = true;
                        goto end;
                    }
                }


                var CONFS2 = ConfiguracoesGlobais.GetConfs().IGNORAR2REGEX;
                foreach (string str in CONFS2)
                {
                    if (Regex.Match(fi.Name, str).Success == true)
                    {
                        aIgnorar = true;
                        goto end;
                    }
                }

                if (String.Compare(fi.Name, "desktop.ini", true) == 0)
                {
                    aIgnorar = true;
                    goto end;
                }

                if (fi.Name.StartsWith(".") == true)
                {
                    aIgnorar = true;
                    goto end;
                }
            end:
                
                if (aIgnorar) {
                    Console.WriteLine("IGNORADO (rule matched): " + fi.Name);
                }
                else
                {

                    DateTime lastTouchDate;
                    ObterUltimoAcessoOuMod(fi, out lastTouchDate);
                    var weeksInYear = WeeksInYear(lastTouchDate);
                    DirectoryInfo destino2 = di2.CreateSubdirectory(lastTouchDate.Year + "-week" + weeksInYear);
                    //Console.Out.WriteLine(fi.Name + "->" + destino2.Name);


                    if (fi is FileInfo)
                    {
                        var destinoEmTralha = new FileInfo(destino2.ToString() + Path.DirectorySeparatorChar + fi.Name);
                        if (destinoEmTralha.Exists)
                        {
                            FileInfo Original = (FileInfo)fi;
                            var existente = GetChecksumBuffered(new FileStream(destinoEmTralha.ToString(), FileMode.Open));
                            var candidato = GetChecksumBuffered(new FileStream(destino2.ToString() + Path.DirectorySeparatorChar + Original.ToString(), FileMode.Open));

                            Console.WriteLine("IGNORADO DUP: " + fi.Name);
                        }
                        else
                        {
                            try
                            {
                                ((FileInfo)fi).MoveTo(destinoEmTralha.ToString());
                                Console.Write("MOVIDO: ");
                                Console.WriteLine(fi.Name);
                            }
                            catch (Exception ex)
                            {
                                Console.Write("FALHOU A MOVER (excepção): ");
                                Console.WriteLine(fi.Name);
                            }
                        }
                    }
                    else if (fi is DirectoryInfo)
                    {
                        try
                        {
                            Console.Write("MOVIDO: ");
                            Console.WriteLine(fi.Name);

                            ((DirectoryInfo)fi).MoveTo(destino2.ToString() + Path.DirectorySeparatorChar + fi.Name);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("IGNORADO EXCEPCAO: " + fi.Name);
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
        }

        private void ObterUltimoAcessoOuMod(FileSystemInfo fi, out DateTime Datae)
        {
            TimeSpan ts1 = fi.LastWriteTime - fi.LastAccessTime; // > 0 => LastWriteTime > LastAccessTime
            Datae = DateTime.Now;

            if (Math.Abs(ts1.TotalHours) > 1)
            {
                Datae = fi.LastWriteTime;
            }
            else
            {
                Datae = fi.LastAccessTime;
            }

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
