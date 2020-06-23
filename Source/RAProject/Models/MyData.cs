using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RAProject.Connection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace RAProject.Models
{
    [Serializable]
    public static class MyData
    {
        public static DataFile myData;
        static string dataFileName = "\\myData.dat";

        public struct FileHandling
        {
            /// <summary>
            /// Saves all data to file.
            /// </summary>
            /// <returns>True on completion.</returns>
            public static bool SaveData()
            {
                Console.WriteLine("Saving myData to file...");
                try
                {
                    MyData.myData.username = Properties.Settings.Default.Credential_Username;
                    MyData.myData.apikey = Properties.Settings.Default.Credential_APIKey;

                    string launchDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                    FileInfo dataFile = new FileInfo(launchDirectory + dataFileName);
                    using (FileStream stream = new FileStream(dataFile.FullName, FileMode.Create))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        binaryFormatter.Serialize(stream, myData);
                        Console.WriteLine("File {0} saved successfully.", dataFileName);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: failed to save data to file.");
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
            /// <summary>
            /// Loads all data from file.
            /// </summary>
            /// <returns>True on completion.</returns>
            public static bool LoadData()
            {
                Console.WriteLine("Loading data from file...");
                try
                {
                    string launchDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                    FileInfo dataFile = new FileInfo(launchDirectory + dataFileName);
                    using (Stream stream = File.Open(dataFile.FullName, FileMode.Open))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();

                        // Deserilise stream of binary data into object
                        myData = (DataFile) binaryFormatter.Deserialize(stream);
                    }

                    Properties.Settings.Default.Credential_Username = MyData.myData.username;
                    Properties.Settings.Default.Credential_APIKey = MyData.myData.apikey;

                    Console.WriteLine("Data file {0} loaded.", dataFile.FullName);
                    return true;
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine("No data exists: {0}", ex.Message);
                    myData = new DataFile();
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading: {0}\n{1}", dataFileName, ex.Message);
                    myData = new DataFile();
                    return false;
                }
            }
        }

        /// <summary>
        /// Downloads all console data.
        /// </summary>
        public static void DownloadConsoles()
        {
            Console.WriteLine("Downloading console data...");


            // Fetch console list
            string url = Requests.Consoles.getConsoleIDs();
            string json = Requests.FetchJSON(url);
            dynamic data = JsonConvert.DeserializeObject(json);

            if (data["console"][0] != null)
            {
                if (myData == null)
                {
                    myData = new DataFile();
                }

                // Create list of consoles
                myData.consoles = new List<GameConsole>();

                foreach (JObject j in data["console"][0])
                {
                    // Create console object
                    GameConsole sc = new GameConsole(j);

                    // Add console to list
                    myData.consoles.Add(sc);

                    Console.WriteLine("Added console: " + sc.Name);
                }
            }
            else
            {
                MessageBox.Show(
                    "No data received for consoles, check your credentials in Settings.",
                    "No data received",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
            }
        }
        /// <summary>
        /// Downloads all games for all consoles in MyData.
        /// </summary>
        public static void DownloadAllGames()
        {
            foreach (GameConsole console in MyData.myData.consoles)
            {
                Console.WriteLine("Downloading {0} games...", console.Name);
                console.DownloadConsoleGames();
            }
        }
    }
}
