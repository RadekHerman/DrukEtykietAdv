using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;


namespace DrukEtykietAdv
{
    class DatabaseManager
    {

        public static void ManageSQL(string connectionString, List<string[]> dostawaList, string towarEtykietyCsv, string towarBezEtykietyCsv)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();
                    Console.WriteLine("Nastąpiło połącznie z bazą SQLite.");

                    // clear delivery table
                    string sql = "DELETE FROM delivery";
                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    command.ExecuteNonQuery();
                    Console.WriteLine("Baza przygotowana na nowe dane.");

                    // upload data from dostawa.csv to delivery table of database

                    UploadDostawaToDatabase(dostawaList, connection, command);
                    // create csv files
                    CreateCsvFiles(connection, command, towarEtykietyCsv, towarBezEtykietyCsv);
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd połącznia z bazą danych: {ex.Message}");
                }
            }
        }


        private static void UploadDostawaToDatabase(List<string[]> dostawaList, SQLiteConnection connection, SQLiteCommand command)
        {
            {
                foreach (string[] row in dostawaList)
                {
                    try
                    {
                        string itemCode = row[0]?.Trim();
                        if (string.IsNullOrEmpty(itemCode) || !int.TryParse(row[1]?.Trim(), out int itemQuantity))
                        {
                            Console.WriteLine("Złe dane, wiersz pominięty.");
                            continue;
                        }
                        string sql = "INSERT INTO delivery (kod_towaru, sztuk) VALUES (@itemCode, @itemQuantity)";
                        command = new SQLiteCommand(sql, connection);
                        command.Parameters.AddWithValue("@itemCode", itemCode);
                        command.Parameters.AddWithValue("@itemQuantity", itemQuantity);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd przy wprowadzaniu danych: {ex.Message}");
                    }
                }
                Console.WriteLine("Dane z pliku dostawa.csv zostały przetworzone.");
            }
        }


        private static void CreateCsvFiles(SQLiteConnection connection, SQLiteCommand command, string towarEtykietyCsv, string towarBezEtykietyCsv)
        {
            try
            {
                string[,] sqlQueries = new string[,]
                {
                        {towarEtykietyCsv, @"SELECT main.kod_towaru, etykieta_pdf, SUM(sztuk) 
                                                          FROM main 
                                                          INNER JOIN delivery on delivery.kod_towaru = main.kod_towaru 
                                                          GROUP BY delivery.kod_towaru"},
                        {towarBezEtykietyCsv, @"SELECT delivery.kod_towaru, main.etykieta_pdf, sum(sztuk)
                                                    FROM delivery 
                                                    LEFT JOIN main on delivery.kod_towaru = main.kod_towaru 
                                                    WHERE main.kod_towaru IS NULL 
                                                    GROUP BY delivery.kod_towaru"}
                };


                for (int i = 0; i < 2; i++)
                {
                    string sql = @sqlQueries[i, 1];
                    command = new SQLiteCommand(sql, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    StreamWriter writer = new StreamWriter(sqlQueries[i, 0]);
                    writer.WriteLine("LP;SYMBOL;NAZWA ETYKIETY;SZTUK");

                    int counter = 1;
                    while (reader.Read())
                    {
                        string symbol = reader.GetString(0);
                        string etykieta;
                        if (reader.IsDBNull(1))
                            etykieta = "brak etykiety";
                        else
                            etykieta = reader.GetString(1);

                        int sztukToPrint = reader.GetInt32(2);
                        // print to the csv file 
                        writer.WriteLine($"{counter};{symbol};{etykieta};{sztukToPrint}");
                        counter++;
                    }
                    writer.Close();
                    reader.Close();

                    Console.WriteLine($"Utworzono plik {sqlQueries[i, 0]}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd przy tworzeniu plków CSV {ex.Message}");
            }

        }
    }


}

