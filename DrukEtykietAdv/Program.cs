using System;
using System.Collections.Generic;
using System.IO;

namespace DrukEtykietAdv
{
    class Program
    {
        static void Main(string[] args)
        {
            string configFilePath = "configTEST.json"; // TEST
            // string configFilePath = "config.json"; //         
            // string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"); // 

            Config config = ConfigManager.LoadConfig(configFilePath);

            List<string[]> dostawaList = CsvFileManager.ManageDostawaFile(config.Paths.DostawaCsv);

            DatabaseManager.ManageSQL(config.ConnectionString, dostawaList, config.Paths.TowarEtykietyCsv, config.Paths.TowarBezEtykietyCsv);

            while (true)
            {
                Menu.ShowMenu();
                string choice = Console.ReadLine();
                if (choice != null)
                    Menu.MenuSelect(config, choice);
                else if (choice.Equals(""))
                    Environment.Exit(0);

            }

        }

    }
}