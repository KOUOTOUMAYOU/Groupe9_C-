using System;

namespace GestionApp
{
    // Classe de base abstraite pour tous les menus
    public abstract class MenuBase
    {
        // Méthode abstraite qui doit être implémentée par toutes les classes dérivées (Polymorphisme)
        public abstract void Show();

        // Méthodes utilitaires communes
        protected void ShowTitle(string title)
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

        protected void Pause(string msg = "Appuyez sur une touche pour continuer...")
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n" + msg.PadLeft((Console.WindowWidth + msg.Length) / 2));
            Console.ReadKey(true);
        }
    }
}
