// UI.cs
using System;

namespace GestionApp
{
    public static class UI
    {
        public static void ShowTitle(string title)
        {
            Console.Clear();
            int w = Console.WindowWidth;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("═".PadRight(w, '═'));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(title.PadLeft((w + title.Length) / 2).PadRight(w));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("═".PadRight(w, '═'));
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Pause(string msg = "Appuyez sur une touche pour continuer...")
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n" + msg.PadLeft((Console.WindowWidth + msg.Length) / 2));
            Console.ReadKey(true);
        }
    }
}