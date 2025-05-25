using System;
using System.Collections.Generic;

namespace EsportManager.Utils
{
    public static class AsciiArt
    {
        public static string[] EsportLogo = new string[]
        {
            @"   ______ _____ ____   ____  ____ ______  ",
            @"  / ____// ___// __ \ / __ \/ __ \/_  __/ ",
            @" / __/   \__ \/ /_/ // / / / /_/ / / /    ",
            @"/ /___  ___/ / ____// /_/ / _, _/ / /     ",
            @"\____/ /____/_/     \____/_/ |_| /_/      ",
            @"    __  ___  ___    _   __  ___    ______  ______ ____  ",
            @"   /  |/  / /   |  / | / / /   |  / ____/ / ____// __ \ ",
            @"  / /|_/ / / /| | /  |/ / / /| | / / __  / __/  / /_/ / ",
            @" / /  / / / ___ |/ /|  / / ___ |/ /_/ / / /___ / _, _/  ",
            @"/_/  /_/ /_/  |_/_/ |_/ /_/  |_|\____/ /_____//_/ |_|   "
        };

        public static string[] Trophy = new string[]
        {
            @"     ___________     ",
            @"    '._==_==_=_.'    ",
            @"    .-\:      /-.    ",
            @"   | (|:.     |) |   ",
            @"    '-|:.     |-'    ",
            @"      \::.    /      ",
            @"       '::. .'       ",
            @"         ) (         ",
            @"       _.' '._       ",
            @"      '-------'      "
        };

        public static string[] Gamepad = new string[]
        {
            @"    ____________________________    ",
            @"   /                            \   ",
            @"  |    _________________________|_  ",
            @"  |   /                            \ ",
            @"  |  /                              \ ",
            @"  | |   _                      _    | ",
            @"  | |  (O)                    (O)   | ",
            @"  | |                               | ",
            @"  | |       /\              []      | ",
            @"  | |      /  \             []      | ",
            @"  | |      \  /                     | ",
            @"  | |       \/                      | ",
            @"  | |                               | ",
            @"  |  \_                           _/ ",
            @"  |    \_                       _/   ",
            @"  |      |_____________________|     ",
            @"   \____/                      \____/ "
        };

        public static string[] Keyboard = new string[]
        {
            @"  _________________________________________________  ",
            @" |  _____________________________________________  | ",
            @" | |                                             | | ",
            @" | |                                             | | ",
            @" | |_____________________________________________| | ",
            @" |     _    _    _    _    _    _    _    _    _   | ",
            @" |    / \  / \  / \  / \  / \  / \  / \  / \  / \  | ",
            @" |   | Q || W || E || R || T || Y || U || I || O | | ",
            @" |    \_/  \_/  \_/  \_/  \_/  \_/  \_/  \_/  \_/  | ",
            @" |     _    _    _    _    _    _    _    _    _   | ",
            @" |    / \  / \  / \  / \  / \  / \  / \  / \  / \  | ",
            @" |   | A || S || D || F || G || H || J || K || L | | ",
            @" |    \_/  \_/  \_/  \_/  \_/  \_/  \_/  \_/  \_/  | ",
            @" |_________________________________________________| "
        };

        public static string[] Computer = new string[]
        {
            @"      _____                    ",
            @"     /     \                   ",
            @"    /       \                  ",
            @"   /         \                 ",
            @"  /           \                ",
            @" /_____________\               ",
            @" |  _________  |               ",
            @" | |         | |               ",
            @" | |  ESPORT | |               ",
            @" | |  GAMING | |               ",
            @" | |_________| |               ",
            @" |   _______   |               ",
            @" |  |   _   |  |               ",
            @" |__|       |__|               ",
            @"    |_______|                  "
        };

        public static string[] Fire = new string[]
        {
            @"       (                    )",
            @"      (  )               (  )",
            @"       )(                 )( ",
            @"      (  )               (  )",
            @"       )(                 )( ",
            @"     /|  |\             /|  |\",
            @"    / |  | \           / |  | \",
            @"   /__|__|__\         /__|__|__\",
            @"      |  |               |  |",
            @"      |  |               |  |"
        };

        public static string[] Star = new string[]
        {
            @"         .__.",
            @"       .      .",
            @"     .          .",
            @"   .              .",
            @" .____________________.",
            @"           ||",
            @"           ||",
            @"           ||",
            @"           ||",
            @"           ||",
            @"           ||"
        };

        public static void DrawColorfulArt(string[] art, int x, int y, bool randomColors = true)
        {
            Random random = new Random();
            ConsoleColor[] colors = (ConsoleColor[])Enum.GetValues(typeof(ConsoleColor));

            List<ConsoleColor> brightColors = new List<ConsoleColor>();
            foreach (var color in colors)
            {
                if (color != ConsoleColor.Black && color != ConsoleColor.DarkGray
                    && color != ConsoleColor.Gray && color != ConsoleColor.White)
                {
                    brightColors.Add(color);
                }
            }

            for (int i = 0; i < art.Length; i++)
            {
                Console.SetCursorPosition(x, y + i);

                if (randomColors)
                {
                    Console.ForegroundColor = brightColors[random.Next(brightColors.Count)];
                    Console.Write(art[i]);
                }
                else
                {
                    foreach (char c in art[i])
                    {
                        if (c != ' ')
                        {
                            Console.ForegroundColor = brightColors[random.Next(brightColors.Count)];
                        }
                        Console.Write(c);
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void DrawBlinkingArt(string[] art, int x, int y, int times = 3, int delay = 200)
        {
            for (int t = 0; t < times; t++)
            {
                DrawColorfulArt(art, x, y, true);
                System.Threading.Thread.Sleep(delay);

                for (int i = 0; i < art.Length; i++)
                {
                    Console.SetCursorPosition(x, y + i);
                    Console.Write(new string(' ', art[i].Length));
                }

                System.Threading.Thread.Sleep(delay / 2);
            }

            DrawColorfulArt(art, x, y, true);
        }

        public static void SlideInFromLeft(string[] art, int finalX, int y, int speed = 50)
        {
            int maxLength = 0;
            foreach (string line in art)
            {
                maxLength = Math.Max(maxLength, line.Length);
            }

            for (int currentX = -maxLength; currentX <= finalX; currentX += 2)
            {
                for (int i = 0; i < art.Length; i++)
                {
                    Console.SetCursorPosition(Math.Max(0, currentX - 2), y + i);
                    Console.Write(new string(' ', maxLength + 2));
                }

                for (int i = 0; i < art.Length; i++)
                {
                    if (currentX < 0)
                    {
                        int visibleStart = Math.Abs(currentX);
                        if (visibleStart < art[i].Length)
                        {
                            Console.SetCursorPosition(0, y + i);
                            Console.Write(art[i].Substring(visibleStart));
                        }
                    }
                    else
                    {
                        Console.SetCursorPosition(currentX, y + i);
                        Console.Write(art[i]);
                    }
                }

                System.Threading.Thread.Sleep(speed);
            }
        }
    }
}