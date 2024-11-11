using System;
using System.Collections.Generic;
using System.IO;


namespace DrukEtykietAdv
{
    class CsvFileManager
    {
        public static List<string[]> ManageDostawaFile(string DostawaCsvPath)
        {
            List<string[]> dostawaList = new List<string[]>();
            try
            {
                string[] lines = File.ReadAllLines(DostawaCsvPath);

                foreach (string line in lines)
                {
                    string[] values = line.Split(';');
                    dostawaList.Add(values);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            return dostawaList;

        }

        public static void OutputCsvFiles(string filePath)
        {

            string main_header = "TOWAR ETYKIETY DO WYDRUKU";
            if (filePath.Contains("towar_bez_etykiet"))
            {
                main_header = "TOWAR BEZ ETYKIET";
            }


            try
            {
                // Open the CSV file and read its contents
                using (StreamReader reader = new StreamReader(filePath))
                {
                    int consoleWidth = 62;
                    int padding = (consoleWidth - main_header.Length) / 2;
                    string border = "+" + new string('-', consoleWidth) + "+";
                    string paddedHeader = main_header.PadLeft(main_header.Length + padding).PadRight(consoleWidth);
                    Console.WriteLine(border);
                    Console.WriteLine($"|{paddedHeader}|");
                    Console.WriteLine(border);

                    // Read the first line to get the headers
                    if (!reader.EndOfStream)
                    {
                        string headerLine = reader.ReadLine();
                        string[] headers = headerLine.Split(';');

                        Console.WriteLine($"| {headers[0],-5}| {headers[1],-23}| {headers[2],-20}| {headers[3],-7}|");
                        Console.WriteLine(border);
                    }

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] columns = line.Split(';');
                        Console.WriteLine($"| {columns[0],-4} | {columns[1],-23}| {columns[2],-20}| {columns[3],-7}|");
                    }
                    Console.WriteLine(border);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd przy odczycie pliku: {filePath}");
                Console.WriteLine(ex.Message);
            }
        }
    }


}

