using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading;

namespace DrukEtykietAdv
{
    class PrinterService
    {
        public static void PrintSelectedLabel(Config config, string towarEtykietyCsv)
        {
            int labelLP = 0;
            string labelName = "";
            string itemCode = "";
            // number of rows count
            var allLines = File.ReadAllLines(towarEtykietyCsv);
            int rowCount = allLines.Length - 1;
            Console.WriteLine($"Total number of rows: {rowCount}");

            while (true)
            {
                CsvFileManager.OutputCsvFiles(towarEtykietyCsv);
                Console.WriteLine($"Wybierz LP produktu (1 - {rowCount}) lub \"ENTER\" aby wyjść do menu");
                string selection = Console.ReadLine();
                if ((selection != null) && (selection.Equals("")))
                {
                    break;
                }
                if ((selection != null) && (int.TryParse(selection, out labelLP)) && (labelLP > 0) && (labelLP <= rowCount))
                {
                    break;
                }
                else
                    Console.Clear();
            }

            if (labelLP > 0)
            {
                // get label name 
                foreach (var line in File.ReadLines(towarEtykietyCsv))
                {
                    var columns = line.Split(';');
                    if (columns[0] == labelLP.ToString())
                    {
                        itemCode = columns[1].Trim();
                        labelName = columns[2].Trim();
                    }
                }
                Console.WriteLine($"Drukuję 1 etykietę {labelName} dla {itemCode}");

                if (!ChangeDefaultPrinter(config.Printers.LabelPrinter))
                    return;

                Thread.Sleep(500);

                string pdfFilePath = Path.Combine(config.Paths.LabelPdf, labelName);

                ExecutePrinting(pdfFilePath); //<------------ execute printing
                Thread.Sleep(500);
                // po wydrukowaniu zmiana drukarki domyślnej
                if (!ChangeDefaultPrinter(config.Printers.DefaultPrinter))
                    if (!ChangeDefaultPrinter(config.Printers.DefaultPrinter2))
                        return;

                while (true)
                {
                    if (Menu.YesOrNo("Czy chesz wydrukować kolejną etykietę? \nWybierz t/n i potwiedź Enter?"))
                        Menu.MenuSelect(config, "4");
                    break;
                }
            }
        }



        public static void PrintLabels(Config config, string towarEtykietyCsv, string zapisWydrukuTxt)
        {
            Console.WriteLine("Drukuję etyiety...");

            // zmiana drukarki domyślnej
            if (!ChangeDefaultPrinter(config.Printers.LabelPrinter))
                return;

            // drukowanie
            try
            {
                using (StreamWriter logFileWriter = new StreamWriter(zapisWydrukuTxt))
                {
                    using (StreamReader reader = new StreamReader(towarEtykietyCsv))
                    {
                        reader.ReadLine();
                        int counter = 1;

                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            string[] columns = line.Split(';');
                            string itemCode = columns[1].Trim();
                            string labelName = columns[2].Trim();
                            int labelQty = Convert.ToInt32(columns[3].Trim());

                            string pdfFilePath = Path.Combine(config.Paths.LabelPdf, labelName);

                            for (int i = 1; i <= labelQty; i++)
                            {
                                Thread.Sleep(2000);
                                ExecutePrinting(pdfFilePath);   /// <------------------- execute printing
                                string logFileOutput = $"{counter}. {itemCode} -- etykieta {labelName} -- numer {i}";
                                Console.WriteLine(logFileOutput);
                                logFileWriter.WriteLine(logFileOutput);
                                counter++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd podczas drukowania:");
                Console.WriteLine(ex.Message);
            }

            Thread.Sleep(2000);
            // po wydrukowaniu zmiana drukarki domyślnej
            if (!ChangeDefaultPrinter(config.Printers.DefaultPrinter))
                if (!ChangeDefaultPrinter(config.Printers.DefaultPrinter2))
                    return;

            Menu.PressEnter();
        }


        static bool ChangeDefaultPrinter(string printerName)
        {
            try
            {
                using (var printerSearcher = new ManagementObjectSearcher($"SELECT * FROM Win32_Printer WHERE Name = '{printerName}'"))
                {
                    foreach (ManagementObject printer in printerSearcher.Get())
                    {
                        printer.InvokeMethod("SetDefaultPrinter", null, null);
                        // Console.WriteLine($"Drukarka domyślna: {printerName}.");
                        return true;
                    }
                    Console.WriteLine("Drukarka nie znaleziona.");
                    Console.WriteLine($"Sprawdź podłączenie drukarki {printerName}.");
                    Menu.PressEnter();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd zmiany drukarki domyślej {printerName}: {ex.Message}");
                return false;
            }
        }

        public static void ExecutePrinting(string filePath)
        {
            try
            {
                ProcessStartInfo printInfo = new ProcessStartInfo
                {
                    Verb = "print",
                    FileName = filePath,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                using (Process printProcess = new Process { StartInfo = printInfo })
                {
                    printProcess.Start();
                }
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Console.WriteLine($"Błąd systemowy podczas uruchamiania procesu dukowania: {ex.Message}.");
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Console.WriteLine($"Plik nie znostał znaleziony: {ex.Message}. Sprawdź popranośc ścieżki: {filePath}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"ruchomiwnie procesu drukowania nie powiodło się: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Nieoczekiwany błąd: {ex.Message}");
            }
        }
    }
}
