using RAProject.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
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
            public const string GameBoy = "GameBoy";
            public const string GameBoyAdvance = "GameBoy Advance";
            public const string GameBoyColor = "GameBoy Color";
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
            public const string PokemonMini = "Pokemin Mini";
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
            public const string PhilipsCD_i = "Phillips CD-i";
            public const string _3DOInteractiveMultiplayer = "3DO Interactive Multiplayer";
            public const string ColecoVision = "ColecoVision";
            public const string Intellivision = "IntelliVision";
            public const string Vectrex = "Vectrex";
            public const string PC8000_8800 = "PC8000/8800";
            public const string PC9800 = "PC9800";
            public const string PC_FX = "PC FX";
            public const string Atari5200 = "Atari 5200";
            public const string Atari7800 = "Atari 7800";
            public const string X68K = "X68K";
            public const string WonderSwan = "Wonder Swan";
            public const string CassetteVision = "Cassette Vision";
            public const string SuperCassetteVision = "Super Cassette Vision";
        }

        //public struct ConsoleImages
        //{
        //    public static Image MegaDrive = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\megaDrive.png");
        //    public static Image Nintendo64 = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\n64.png");
        //    public static Image SNES = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\snes.png");
        //    public static Image GameBoy = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\gb.png");
        //    public static Image GameBoyAdvance = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\gba.png");
        //    public static Image GameBoyColor = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\gbc.png");
        //    public static Image NES = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\nes.png");
        //    public static Image PCEngine = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\PC_Engine.png");
        //    public static Image SegaCD = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\sega_cd.png");
        //    public static Image _32X = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\sega_32x.png");
        //    public static Image MasterSystem = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\sms.png");
        //    public static Image PlayStation = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\ps1.png");
        //    public static Image AtariLynx = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\atari_lynx.png");
        //    public static Image NeoGeoPocket = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\neo_geo.png");
        //    public static Image GameGear = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\game_gear.png");
        //    public static Image GameCube = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\gamecube.png");
        //    public static Image AtariJaguar = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\atari_jaguar.png");
        //    public static Image NintendoDS = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\nintendo_ds.png");
        //    public static Image Wii = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\wii.png");
        //    public static Image WiiU = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\wii_u.png");
        //    public static Image PlayStation2 = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\ps2.png");
        //    public static Image Xbox = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\xbox.png");
        //    public static Image PokemonMini = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\pokemon_mini.png");
        //    public static Image Atari2600 = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\atari_2600.png");
        //    public static Image DOS = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\dos.png");
        //    public static Image Arcade = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\arcade.png");
        //    public static Image VirtualBoy = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\cirtual_boy.png");
        //    public static Image MSX = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\msx.png");
        //    public static Image Commodore64 = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\c64.png");
        //    public static Image ZX81 = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\zx81.png");
        //    public static Image Oric = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\oric.png");
        //    public static Image SG_1000 = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\sg_1000.png");
        //    public static Image VIC_20 = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\vic-20.png");
        //    public static Image Amiga = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\amiga.png");
        //    public static Image AtariST = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\atari_st.png");
        //    public static Image AmstradCPC = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\amstrad_cpc.png");
        //    public static Image AppleII = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\apple_ii.png");
        //    public static Image Saturn = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\sega_saturn.png");
        //    public static Image Dreamcast = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\dreamcast.png");
        //    public static Image PlayStationPortable = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\psp.png");
        //    public static Image PhilipsCD_i = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\philips_cd-i.png");
        //    public static Image _3DOInteractiveMultiplayer = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\3do_interactive.png");
        //    public static Image ColecoVision = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\colecovision.png");
        //    public static Image Intellivision = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\intellivision.png");
        //    public static Image Vectrex = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\vectrex.png");
        //    public static Image PC8000_8800 = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\pc-8000.png");
        //    public static Image PC9800 = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\pc-9800.png");
        //    public static Image PC_FX = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\pc-fx.png");
        //    public static Image Atari5200 = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\atari_5200.png");
        //    public static Image Atari7800 = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\atari_7800.png");
        //    public static Image X68K = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\x68k.png");
        //    public static Image WonderSwan = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\wonderswan.png");
        //    public static Image CassetteVision = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\Cassette_Vision.png");
        //    public static Image SuperCassetteVision = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\super_Cassette_Vision.png");
        //}
        public struct ConsoleImages
        {
            public static string MegaDrive = AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\megaDrive.png";
            public static string Nintendo64 =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\n64.png"; 
            public static string SNES =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\snes.png"; 
            public static string GameBoy =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\gb.png"; 
            public static string GameBoyAdvance =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\gba.png"; 
            public static string GameBoyColor =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\gbc.png"; 
            public static string NES =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\nes.png"; 
            public static string PCEngine =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\PC_Engine.png"; 
            public static string SegaCD =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\sega_cd.png"; 
            public static string _32X =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\sega_32x.png"; 
            public static string MasterSystem =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\sms.png"; 
            public static string PlayStation =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\ps1.png"; 
            public static string AtariLynx =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\atari_lynx.png"; 
            public static string NeoGeoPocket =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\neo_geo.png"; 
            public static string GameGear =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\game_gear.png"; 
            public static string GameCube =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\gamecube.png"; 
            public static string AtariJaguar =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\atari_jaguar.png"; 
            public static string NintendoDS =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\nintendo_ds.png"; 
            public static string Wii =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\wii.png"; 
            public static string WiiU =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\wii_u.png"; 
            public static string PlayStation2 =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\ps2.png"; 
            public static string Xbox =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\xbox.png"; 
            public static string PokemonMini =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\pokemon_mini.png"; 
            public static string Atari2600 =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\atari_2600.png"; 
            public static string DOS =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\dos.png"; 
            public static string Arcade =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\arcade.png"; 
            public static string VirtualBoy =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\cirtual_boy.png"; 
            public static string MSX =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\msx.png"; 
            public static string Commodore64 =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\c64.png"; 
            public static string ZX81 =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\zx81.png"; 
            public static string Oric =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\oric.png"; 
            public static string SG_1000 =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\sg_1000.png"; 
            public static string VIC_20 =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\vic-20.png"; 
            public static string Amiga =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\amiga.png"; 
            public static string AtariST =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\atari_st.png"; 
            public static string AmstradCPC =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\amstrad_cpc.png"; 
            public static string AppleII =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\apple_ii.png"; 
            public static string Saturn =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\sega_saturn.png"; 
            public static string Dreamcast =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\dreamcast.png"; 
            public static string PlayStationPortable =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\psp.png"; 
            public static string PhilipsCD_i =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\philips_cd-i.png"; 
            public static string _3DOInteractiveMultiplayer =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\3do_interactive.png"; 
            public static string ColecoVision =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\colecovision.png"; 
            public static string Intellivision =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\intellivision.png"; 
            public static string Vectrex =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\vectrex.png"; 
            public static string PC8000_8800 =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\pc-8000.png"; 
            public static string PC9800 =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\pc-9800.png"; 
            public static string PC_FX =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\pc-fx.png"; 
            public static string Atari5200 =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\atari_5200.png"; 
            public static string Atari7800 =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\atari_7800.png"; 
            public static string X68K =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\x68k.png"; 
            public static string WonderSwan =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\wonderswan.png"; 
            public static string CassetteVision =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\Cassette_Vision.png"; 
            public static string SuperCassetteVision =AppDomain.CurrentDomain.BaseDirectory + "images\\consoles\\super_Cassette_Vision.png"; 
        }

        
        public static Image getConsoleImage(SupportedConsole console)
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
                        return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Image for {0} not found!", console.Name);
                return null;
            }            
        }
    }
}
