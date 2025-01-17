﻿using RAProject.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace RAProject.Modules
{
    public static class ConsoleInformation
    {
        public struct ConsoleNames
        {
            public const string MegaDrive = "Mega Drive";
            public const string Nintendo64 = "Nintendo 64";
            public const string SNES = "SNES";
            public const string GameBoy = "Game Boy";
            public const string GameBoyAdvance = "Game Boy Advance";
            public const string GameBoyColor = "Game Boy Color";
            public const string NES = "NES";
            public const string PCEngine = "PC Engine";
            public const string SegaCD = "Sega CD";
            public const string _32X = "32X";
            public const string MasterSystem = "Master System";
            public const string PlayStation = "PlayStation";
            public const string AtariLynx = "Atari Lynx";
            public const string NeoGeoPocket = "Neo Geo Pocket";
            public const string GameGear = "Game Gear";
            public const string GameCube = "GameCube";
            public const string AtariJaguar = "Atari Jaguar";
            public const string NintendoDS = "Nintendo DS";
            public const string Wii = "Wii";
            public const string WiiU = "Wii U";
            public const string PlayStation2 = "PlayStation 2";
            public const string Xbox = "Xbox";
            public const string PokemonMini = "Pokemon Mini";
            public const string Atari2600 = "Atari 2600";
            public const string DOS = "DOS";
            public const string Arcade = "Arcade";
            public const string VirtualBoy = "Virtual Boy";
            public const string MSX = "MSX";
            public const string Commodore64 = "Commodore 64";
            public const string ZX81 = "ZX81";
            public const string Oric = "Oric";
            public const string SG_1000 = "SG-1000";
            public const string VIC_20 = "VIC-20";
            public const string Amiga = "Amiga";
            public const string AtariST = "Atari ST";
            public const string AmstradCPC = "Amstrad CPC";
            public const string AppleII = "Apple II";
            public const string Saturn = "Saturn";
            public const string Dreamcast = "Dreamcast";
            public const string PlayStationPortable = "PlayStation Portable";
            public const string PhilipsCD_i = "Philips CD-i";
            public const string _3DOInteractiveMultiplayer = "3DO Interactive Multiplayer";
            public const string ColecoVision = "ColecoVision";
            public const string Intellivision = "Intellivision";
            public const string Vectrex = "Vectrex";
            public const string PC8000_8800 = "PC-8000/8800";
            public const string PC9800 = "PC-9800";
            public const string PC_FX = "PC-FX";
            public const string Atari5200 = "Atari 5200";
            public const string Atari7800 = "Atari 7800";
            public const string X68K = "X68K";
            public const string WonderSwan = "WonderSwan";
            public const string CassetteVision = "Cassette Vision";
            public const string SuperCassetteVision = "Super Cassette Vision";
        }

        public struct ConsoleImages
        {
            public static string MegaDrive = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\megaDrive.png"; // AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\megaDrive.png";
            public static string Nintendo64 = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\n64.png"; 
            public static string SNES = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\snes.png"; 
            public static string GameBoy = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\gb.png"; 
            public static string GameBoyAdvance = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\gba.png"; 
            public static string GameBoyColor = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\gbc.png"; 
            public static string NES = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\nes.png"; 
            public static string PCEngine = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\PC_Engine.png"; 
            public static string SegaCD = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\sega_cd.png"; 
            public static string _32X = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\sega_32x.png"; 
            public static string MasterSystem = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\sms.png"; 
            public static string PlayStation = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\ps1.png"; 
            public static string AtariLynx = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\atari_lynx.png"; 
            public static string NeoGeoPocket = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\neo_geo.png"; 
            public static string GameGear = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\game_gear.png"; 
            public static string GameCube = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\gamecube.png"; 
            public static string AtariJaguar = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\atari_jaguar.png"; 
            public static string NintendoDS = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\nintendo_ds.png"; 
            public static string Wii = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\wii.png"; 
            public static string WiiU = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\wii_u.png"; 
            public static string PlayStation2 = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\ps2.png"; 
            public static string Xbox = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\xbox.png"; 
            public static string PokemonMini = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\pokemon_mini.png"; 
            public static string Atari2600 = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\atari_2600.png"; 
            public static string DOS = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\dos.png"; 
            public static string Arcade = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\arcade.png"; 
            public static string VirtualBoy = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\cirtual_boy.png"; 
            public static string MSX = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\msx.png"; 
            public static string Commodore64 = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\c64.png"; 
            public static string ZX81 = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\zx81.png"; 
            public static string Oric = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\oric.png"; 
            public static string SG_1000 = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\sg_1000.png"; 
            public static string VIC_20 = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\vic-20.png"; 
            public static string Amiga = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\amiga.png"; 
            public static string AtariST = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\atari_st.png"; 
            public static string AmstradCPC = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\amstrad_cpc.png"; 
            public static string AppleII = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\apple_ii.png"; 
            public static string Saturn = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\sega_saturn.png"; 
            public static string Dreamcast = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\dreamcast.png"; 
            public static string PlayStationPortable = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\psp.png"; 
            public static string PhilipsCD_i = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\philips_cd-i.png"; 
            public static string _3DOInteractiveMultiplayer = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\3do_interactive.png"; 
            public static string ColecoVision = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\colecovision.png"; 
            public static string Intellivision = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\intellivision.png"; 
            public static string Vectrex = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\vectrex.png"; 
            public static string PC8000_8800 = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\pc-8000.png"; 
            public static string PC9800 = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\pc-9800.png"; 
            public static string PC_FX = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\pc-fx.png"; 
            public static string Atari5200 = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\atari_5200.png"; 
            public static string Atari7800 = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\atari_7800.png"; 
            public static string X68K = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\x68k.png"; 
            public static string WonderSwan = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\wonderswan.png"; 
            public static string CassetteVision = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\Cassette Vision.png"; 
            public static string SuperCassetteVision = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "") + @"\Resources\Consoles\super Cassette Vision.png"; 
        }
        
        /// <summary>
        /// Get the images of the given console.
        /// </summary>
        /// <param name="console">Console of which to return an image of</param>
        /// <returns>An image of the given console</returns>
        public static Image getConsoleImage(GameConsole console)
        {
            try
            {
                switch (console.Name)
                {
                    case ConsoleNames.MegaDrive:
                        return Image.FromFile(ConsoleImages.MegaDrive);

                    case ConsoleNames.Nintendo64:
                        return Image.FromFile(ConsoleImages.Nintendo64);

                    case ConsoleNames.SNES:
                        return Image.FromFile(ConsoleImages.SNES);

                    case ConsoleNames.GameBoy:
                        return Image.FromFile(ConsoleImages.GameBoy);

                    case ConsoleNames.GameBoyAdvance:
                        return Image.FromFile(ConsoleImages.GameBoyAdvance);

                    case ConsoleNames.GameBoyColor:
                        return Image.FromFile(ConsoleImages.GameBoyColor);

                    case ConsoleNames.NES:
                        return Image.FromFile(ConsoleImages.NES);

                    case ConsoleNames.PCEngine:
                        return Image.FromFile(ConsoleImages.PCEngine);

                    case ConsoleNames.SegaCD:
                        return Image.FromFile(ConsoleImages.SegaCD);

                    case ConsoleNames._32X:
                        return Image.FromFile(ConsoleImages._32X);

                    case ConsoleNames.MasterSystem:
                        return Image.FromFile(ConsoleImages.MasterSystem);

                    case ConsoleNames.PlayStation:
                        return Image.FromFile(ConsoleImages.PlayStation);

                    case ConsoleNames.AtariLynx:
                        return Image.FromFile(ConsoleImages.AtariLynx);

                    case ConsoleNames.NeoGeoPocket:
                        return Image.FromFile(ConsoleImages.NeoGeoPocket);

                    case ConsoleNames.GameGear:
                        return Image.FromFile(ConsoleImages.GameGear);

                    case ConsoleNames.GameCube:
                        return Image.FromFile(ConsoleImages.GameCube);

                    case ConsoleNames.AtariJaguar:
                        return Image.FromFile(ConsoleImages.AtariJaguar);

                    case ConsoleNames.NintendoDS:
                        return Image.FromFile(ConsoleImages.NintendoDS);

                    case ConsoleNames.Wii:
                        return Image.FromFile(ConsoleImages.Wii);

                    case ConsoleNames.WiiU:
                        return Image.FromFile(ConsoleImages.WiiU);

                    case ConsoleNames.PlayStation2:
                        return Image.FromFile(ConsoleImages.PlayStation2);

                    case ConsoleNames.Xbox:
                        return Image.FromFile(ConsoleImages.Xbox);

                    case ConsoleNames.PokemonMini:
                        return Image.FromFile(ConsoleImages.PokemonMini);

                    case ConsoleNames.Atari2600:
                        return Image.FromFile(ConsoleImages.Atari2600);

                    case ConsoleNames.DOS:
                        return Image.FromFile(ConsoleImages.DOS);

                    case ConsoleNames.Arcade:
                        return Image.FromFile(ConsoleImages.Arcade);

                    case ConsoleNames.VirtualBoy:
                        return Image.FromFile(ConsoleImages.VirtualBoy);

                    case ConsoleNames.MSX:
                        return Image.FromFile(ConsoleImages.MSX);

                    case ConsoleNames.Commodore64:
                        return Image.FromFile(ConsoleImages.Commodore64);

                    case ConsoleNames.ZX81:
                        return Image.FromFile(ConsoleImages.ZX81);

                    case ConsoleNames.Oric:
                        return Image.FromFile(ConsoleImages.Oric);

                    case ConsoleNames.SG_1000:
                        return Image.FromFile(ConsoleImages.SG_1000);

                    case ConsoleNames.VIC_20:
                        return Image.FromFile(ConsoleImages.VIC_20);

                    case ConsoleNames.Amiga:
                        return Image.FromFile(ConsoleImages.Amiga);

                    case ConsoleNames.AtariST:
                        return Image.FromFile(ConsoleImages.AtariST);

                    case ConsoleNames.AmstradCPC:
                        return Image.FromFile(ConsoleImages.AmstradCPC);

                    case ConsoleNames.AppleII:
                        return Image.FromFile(ConsoleImages.AppleII);

                    case ConsoleNames.Saturn:
                        return Image.FromFile(ConsoleImages.Saturn);

                    case ConsoleNames.Dreamcast:
                        return Image.FromFile(ConsoleImages.Dreamcast);

                    case ConsoleNames.PlayStationPortable:
                        return Image.FromFile(ConsoleImages.PlayStationPortable);

                    case ConsoleNames.PhilipsCD_i:
                        return Image.FromFile(ConsoleImages.PhilipsCD_i);

                    case ConsoleNames._3DOInteractiveMultiplayer:
                        return Image.FromFile(ConsoleImages._3DOInteractiveMultiplayer);

                    case ConsoleNames.ColecoVision:
                        return Image.FromFile(ConsoleImages.ColecoVision);

                    case ConsoleNames.Intellivision:
                        return Image.FromFile(ConsoleImages.Intellivision);

                    case ConsoleNames.Vectrex:
                        return Image.FromFile(ConsoleImages.Vectrex);

                    case ConsoleNames.PC8000_8800:
                        return Image.FromFile(ConsoleImages.PC8000_8800);

                    case ConsoleNames.PC9800:
                        return Image.FromFile(ConsoleImages.PC9800);

                    case ConsoleNames.PC_FX:
                        return Image.FromFile(ConsoleImages.PC_FX);

                    case ConsoleNames.Atari5200:
                        return Image.FromFile(ConsoleImages.Atari5200);

                    case ConsoleNames.Atari7800:
                        return Image.FromFile(ConsoleImages.Atari7800);

                    case ConsoleNames.X68K:
                        return Image.FromFile(ConsoleImages.X68K);

                    case ConsoleNames.WonderSwan:
                        return Image.FromFile(ConsoleImages.WonderSwan);

                    case ConsoleNames.CassetteVision:
                        return Image.FromFile(ConsoleImages.CassetteVision);

                    case ConsoleNames.SuperCassetteVision:
                        return Image.FromFile(ConsoleImages.SuperCassetteVision);

                    default:
                        Console.WriteLine("ERROR: TYPO SOMEHWERE!!!");
                        return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Image for {0} not found!\n{1}", console.Name, ex.Message);
                return null;
            }            
        }
    }
}
