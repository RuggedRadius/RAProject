using RAProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAProject.Utilities
{
    public class Search
    {
        // Binary Search Methods
        private static GameConsole BinarySearch_Consoles(GameConsole[] inputArray, string key, int min, int max)
        {
            if (min > max)
            {
                // Not found
                return null;
            }
            else
            {
                int mid = (min + max) / 2;
                if (key.ToLower() == inputArray[mid].Name.ToLower())
                {
                    // Found
                    return inputArray[mid];
                }
                else if (key.ToLower().CompareTo(inputArray[mid].Name.ToLower()) < 0)
                {
                    return BinarySearch_Consoles(inputArray, key, min, mid - 1);
                }
                else
                {
                    return BinarySearch_Consoles(inputArray, key, mid + 1, max);
                }
            }
        }
        private static Game BinarySearch_Games(Game[] inputArray, string key, int min, int max)
        {
            if (min > max)
            {
                // Not found
                return null;
            }
            else
            {
                int mid = (min + max) / 2;
                if (key.ToLower() == inputArray[mid].Title.ToLower())
                {
                    // Found
                    return inputArray[mid];
                }
                else if (key.ToLower().CompareTo(inputArray[mid].Title.ToLower()) < 0)
                {
                    return BinarySearch_Games(inputArray, key, min, mid - 1);
                }
                else
                {
                    return BinarySearch_Games(inputArray, key, mid + 1, max);
                }
            }
        }
        private static Achievement BinarySearch_Achievements(Achievement[] inputArray, string key, int min, int max)
        {
            Console.WriteLine("Searching...");

            if (min > max)
            {
                // Not found
                return null;
            }
            else
            {
                int mid = (min + max) / 2;
                if (key.ToLower() == inputArray[mid].Title.ToLower())
                {
                    // Found
                    return inputArray[mid];
                }
                else if (key.ToLower().CompareTo(inputArray[mid].Title.ToLower()) < 0)
                {
                    return BinarySearch_Achievements(inputArray, key, min, mid - 1);
                }
                else
                {
                    return BinarySearch_Achievements(inputArray, key, mid + 1, max);
                }
            }
        }

        // Fetch arrays of data
        public static GameConsole[] getAllConsoles()
        {
            // Create list
            var results = new LinkedList<GameConsole>();

            // Populate list
            foreach (GameConsole console in MyData.myData.consoles)
            {
                results.AddLast(console);
            }

            // Return list converted to an array
            return results.ToArray();
        }
        public static Game[] getAllGames()
        {
            // Create list
            LinkedList<Game> results = new LinkedList<Game>();

            // Populate list
            foreach (GameConsole console in MyData.myData.consoles)
            {
                foreach (Game game in console.games)
                {
                    results.AddLast(game);
                }
            }

            // Return list converted to an array
            return results.ToArray();
        }
        public static Achievement[] getAllAchievements()
        {
            // Create list
            LinkedList<Achievement> results = new LinkedList<Achievement>();

            // Populate list
            foreach (GameConsole console in MyData.myData.consoles)
            {
                foreach (Game game in console.games)
                {
                    foreach (Achievement acheivement in game.Achievements)
                    {
                        results.AddLast(acheivement);
                    }
                }
            }

            Console.WriteLine(results.Count + " achievements added to search list.");

            // Return list converted to an array
            return results.ToArray();
        }



        // Public-facing methods
        public static GameConsole SearchConsoles(string query)
        {
            // Get data
            GameConsole[] input = getAllConsoles();

            // Sort data
            MergeSort.Consoles_Rescursive(input, 0, input.Length - 1);

            // Binary search data
            return BinarySearch_Consoles(input, query, 0, input.Length - 1);
        }
        public static Game SearchGames(string query)
        {
            // Get data
            Game[] input = getAllGames();

            // Sort data
            MergeSort.Games_Rescursive(input, 0, input.Length - 1);

            // Binary search data
            return BinarySearch_Games(input, query, 0, input.Length - 1);
        }
        public static Achievement SearchAchievements(string query)
        {
            // Get data
            Achievement[] input = getAllAchievements();

            // Sort data
            MergeSort.Achievements_Rescursive(input, 0, input.Length - 1);

            // Binary search data
            return BinarySearch_Achievements(input, query, 0, input.Length - 1);
        }
    }
}
