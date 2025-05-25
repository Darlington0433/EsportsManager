using System;
using System.Collections.Generic;

namespace EsportManager.Utils
{
    public static class UIHelper
    {
        public static void SafeClear()
        {
            try
            {
                Console.Clear();
            }
            catch (Exception)
            {
                try
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Console.WriteLine();
                    }
                    Console.SetCursorPosition(0, 0);
                }
                catch
                {
                }
            }
        }

        public static void SafeSetCursorPosition(int left, int top)
        {
            try
            {
                int windowWidth = SafeGetWindowWidth();
                int windowHeight = SafeGetWindowHeight();

                int safeLeft = Math.Max(0, Math.Min(left, windowWidth - 1));
                int safeTop = Math.Max(0, Math.Min(top, windowHeight - 1));

                Console.SetCursorPosition(safeLeft, safeTop);
            }
            catch
            {
            }
        }

        public static string[] CreatePixelFont(string text)
        {
            Dictionary<char, string[]> pixelLetters = new Dictionary<char, string[]>
            {
                {'E', new string[] {"█████", "█    ", "████ ", "█    ", "█████"}},
                {'S', new string[] {"█████", "█    ", "█████", "    █", "█████"}},
                {'P', new string[] {"█████", "█   █", "█████", "█    ", "█    "}},
                {'O', new string[] {"█████", "█   █", "█   █", "█   █", "█████"}},
                {'R', new string[] {"█████", "█   █", "████ ", "█  █ ", "█   █"}},
                {'T', new string[] {"█████", "  █  ", "  █  ", "  █  ", "  █  "}},
                {'M', new string[] {"█   █", "██ ██", "█ █ █", "█   █", "█   █"}},
                {'A', new string[] {"█████", "█   █", "█████", "█   █", "█   █"}},
                {'N', new string[] {"█   █", "██  █", "█ █ █", "█  ██", "█   █"}},
                {'G', new string[] {"█████", "█    ", "█  ██", "█   █", "█████"}},
                {' ', new string[] {"     ", "     ", "     ", "     ", "     "}}
            };

            string[] result = new string[5];
            for (int i = 0; i < 5; i++)
            {
                result[i] = "";
            }

            foreach (char c in text.ToUpper())
            {
                if (pixelLetters.ContainsKey(c))
                {
                    string[] letterPattern = pixelLetters[c];
                    for (int i = 0; i < 5; i++)
                    {
                        result[i] += letterPattern[i] + " ";
                    }
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        result[i] += "      ";
                    }
                }
            }

            return result;
        }

        public static int SafeGetWindowWidth(int defaultWidth = 80)
        {
            try
            {
                return Console.WindowWidth;
            }
            catch
            {
                return defaultWidth;
            }
        }

        public static int SafeGetWindowHeight(int defaultHeight = 25)
        {
            try
            {
                return Console.WindowHeight;
            }
            catch
            {
                return defaultHeight;
            }
        }
    }
}