using System.IO;
using System.Linq;
using System;
using System.Text;

namespace Animals
{
    class Animal
    {
        private static void MainAnimal(string[] args)
        {
            StringBuilder logout = new StringBuilder();

            string outdir = string.Empty;

            if (args.Length > 5)
            {
                try
                {
                    int count = Convert.ToInt32(args[0]); // сколько лет
                    int core = (int)DateTime.Now.Ticks / 1000;

                    Area area = new Area(Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), Convert.ToInt32(args[3]), Convert.ToInt32(args[4]), core); // width, high, min, max (0:30,30:60,60:100)
                    area.InitAnimal(Convert.ToInt32(args[5]), 0, Logoutput); //животное, 0, 
                    
                    outdir = string.Join("_", DateTime.Now.TimeOfDay.ToString().Replace(':', '_').Replace('.', '_').Replace(' ', '_'), string.Join("_", args), 0);
                    
                    Print("0 years");
                    area.PrintFullArea(Print);
                    area.PrintFullArea(Logoutput);
                    Logsave(string.Format("Start_info"));
                    for (int i = 0; i < count; i++)
                    {
                        area.NexLive();
                        Console.WriteLine("Year {0}", i);
                        area.PrintArea(Print);
                        Logsave(string.Format("Year_{0}", i));
                        area.PrintFullArea(Logoutput);
                        Logsave(string.Format("Result_Year_{0}", i));
                    }
                    
                    Print("Last year");
                    area.PrintArea(Print);
                    area.PrintFullArea(Logoutput);
                    Logsave(string.Format("Result_Last_year"));

                    area.ClearAnimal();
                    area.InitAnimal(Convert.ToInt32(args[5]), 1, Logoutput);
                    outdir = string.Join("_", DateTime.Now.TimeOfDay.ToString().Replace(':', '_').Replace('.', '_').Replace(' ', '_'), string.Join("_", args), 1);
                    Print("0 years");
                    area.PrintFullArea(Print);
                    area.PrintFullArea(Logoutput);
                    Logsave(string.Format("Start_info"));
                    for (int i = 0; i < count; i++)
                    {
                        area.NexLive();
                        Console.WriteLine("Year {0}", i);
                        area.PrintArea(Print);
                        Logsave(string.Format("Year_{0}", i));
                        area.PrintFullArea(Logoutput);
                        Logsave(string.Format("Result_Year_{0}", i));
                    }
                    Print("Last year");
                    area.PrintArea(Print);
                    area.PrintFullArea(Logoutput);
                    Logsave(string.Format("Result_Last_year"));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(e.Message);
                }
                catch
                {
                    Console.WriteLine("Exeption");
                }
            }


            void Print(string str) //вывод
            {
                Console.WriteLine(str);
            }

            void Logoutput(string str)
            {
                logout.AppendLine(str);
            }


            void Logsave(string str)
            {
                string path = string.Join("_", DateTime.Now.TimeOfDay.ToString().Replace(':', '_').Replace('.', '_').Replace(' ', '_'), str);

                Directory.CreateDirectory(outdir);

                File.WriteAllText(outdir + "\\" + path + ".txt", logout.ToString());
                logout.Clear();

            }
        }
    }
}