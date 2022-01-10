using RAProject.Models;
using RAProject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAProject.Utilities.Images
{
    class DetermineBoxArtSizeByConsole
    {
        public static System.Windows.Controls.Image s_SetBoxArtImageSize(System.Windows.Controls.Image img, GameConsole console)
        {
            switch (console.Name)
            {
                case ConsoleInformation.ConsoleNames.GameBoy:
                    img.Width = 160;
                    img.Height = 160;
                    break;

                case ConsoleInformation.ConsoleNames.GameBoyAdvance:
                    img.Width = 160;
                    img.Height = 160;
                    break;

                case ConsoleInformation.ConsoleNames.GameBoyColor:
                    img.Width = 160;
                    img.Height = 160;
                    break;

                case ConsoleInformation.ConsoleNames.SNES:
                    img.Width = 240;
                    img.Height = 160;
                    break;

                case ConsoleInformation.ConsoleNames.PlayStation:
                    img.Width = 160;
                    img.Height = 160;
                    break;

                default:
                    img.Width = 160;
                    img.Height = 240;
                    break;
            }

            return img;
        }
    }
}
