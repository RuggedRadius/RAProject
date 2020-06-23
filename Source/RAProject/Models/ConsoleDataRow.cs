using System;

namespace RAProject.Models
{
    [Serializable]
    public class ConsoleDataRow
    {
        public string ConsoleName { get; set; }
        public string YearReleased { get; set; }
        public int GamesCount { get; set; }

        /// <summary>
        ///  Constructor of custom row object.
        /// </summary>
        /// <param name="_name">Console name</param>
        /// <param name="_year">Year of console release</param>
        /// <param name="_games">Number of games from console</param>
        public ConsoleDataRow (string _name, string _year, int _games)
        {
            ConsoleName = _name;
            YearReleased = _year;
            GamesCount = _games;
        }
    }
}
