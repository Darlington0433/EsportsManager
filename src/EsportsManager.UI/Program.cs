using System;
using EsportsManager.UI.Legacy;

namespace EsportsManager.UI
{
    /// <summary>
    /// Main program entry point for Legacy UI
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                LegacyUIRunner.RunLegacyUI();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Lỗi: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey();
            }
        }
    }
}
