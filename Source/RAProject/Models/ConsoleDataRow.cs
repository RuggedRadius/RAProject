using System;

namespace RAProject.Models
{
    [Serializable]
    public class ConsoleDataRow
    {
        public string ConsoleName { get; set; }
        public string YearReleased { get; set; }
        public int GamesCount { get; set; }

        public ConsoleDataRow()
        {

        }

        public ConsoleDataRow (string _name, string _year, int _games)
        {
            ConsoleName = _name;
            YearReleased = _year;
            GamesCount = _games;
        }
    }
}
