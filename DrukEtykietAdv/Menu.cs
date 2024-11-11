using System;

namespace DrukEtykietAdv
{
    public class Menu
    {

        public static void MenuSelect(Config config, string choice)
        {
            switch (choice)
            {
                case "1":
                    CsvFileManager.OutputCsvFiles(config.Paths.TowarEtykietyCsv);
                    if (YesOrNo("Czy wydrukować etykiety (t/n)?"))
                        PrinterService.PrintLabels(config, config.Paths.TowarEtykietyCsv, config.Paths.ZapisWydrukuTxt);
                    Console.Clear();
                    break;

                case "2":
                    CsvFileManager.OutputCsvFiles(config.Paths.TowarEtykietyCsv);
                    PressEnter();
                    Console.Clear();
                    break;

                case "3":
                    CsvFileManager.OutputCsvFiles(config.Paths.TowarBezEtykietyCsv);
                    PressEnter();
                    Console.Clear();
                    break;
                case "4":
                    PrinterService.PrintSelectedLabel(config, config.Paths.TowarEtykietyCsv);

                    Console.Clear();
                    break;
                case "5":
                    PrinterService.ExecutePrinting(config.Paths.ZapisWydrukuTxt);
                    PressEnter();
                    Console.Clear();
                    break;
                default:
                    if (choice.Equals(""))
                        Environment.Exit(0);
                    Console.Clear();
                    Console.WriteLine("**********************************************************");
                    Console.WriteLine("----------------------------------------------------------");
                    Console.WriteLine("Proszę wybrać pomiędzy 1-5 lub Enter aby wyjśc z programu.");
                    Console.WriteLine("----------------------------------------------------------");
                    break;
            }

        }


        public static void ShowMenu()
        {
            Console.WriteLine("**********************************************************");
            Console.WriteLine("* (1) - drukowanie etykiet                               *");
            Console.WriteLine("* (2) - lista towarów z etykietami                       *");
            Console.WriteLine("* (3) - lista towarów bez etykiet                        *");
            Console.WriteLine("* (4) - drukuj wybraną etykietę                          *");
            Console.WriteLine("* (5) - drukuj zapis wydruku                             *");
            Console.WriteLine("* Enter aby wyjśc z programu.                            *");
            Console.WriteLine("**********************************************************");

        }

        public static void PressEnter()
        {
            Console.WriteLine("Naciśnij Enter aby kontynuować");
            Console.ReadLine();
        }

        public static bool YesOrNo(string message)
        {
            while (true)
            {
                Console.WriteLine(message);
                string result = Console.ReadLine();
                if (result != null && result.ToLower() == "n")
                    return false;
                if (result != null && result.ToLower() == "t")
                    return true;
            }
        }

    }
}
