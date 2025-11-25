using System;
using System.Threading;

namespace GestionApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Gestion Stock Pro 2025 - Édition Prestige";
            Console.CursorVisible = false;

            SplashScreen();

            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            try
            {
                // Initialisation de la base de données et des Repositories
                Database.Init();
                
                // Instanciation du menu principal (POO)
                MainMenu mainMenu = new MainMenu();
                mainMenu.Show();
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERREUR CRITIQUE DANS L'APPLICATION");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nDétails de l'erreur :");
                Console.WriteLine($"- {ex.Message}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"- {ex.InnerException.Message}");
                }

                Console.WriteLine("\nVérifiez que :");
                Console.WriteLine("✓ L'application a les permissions d'écriture");
                Console.WriteLine("✓ Le fichier de base de données n'est pas corrompu");
                Console.WriteLine("✓ .NET 8.0 est correctement installé");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nAppuyez sur une touche pour fermer l'application...");
                Console.ReadKey(true);
            }
        }

        static void SplashScreen()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;

            string[] logo = {
                "    ██████╗ ███████╗███████╗████████╗██╗ ██████╗ ███╗   ██╗",
                "   ██╔════╝ ██╔════╝██╔════╝╚══██╔══╝██║██╔═══██╗████╗  ██║",
                "   ██║  ███╗█████╗  ███████╗   ██║   ██║██║   ██║██╔██╗ ██║",
                "   ██║   ██║██╔══╝  ╚════██║   ██║   ██║██║   ██║██║╚██╗██║",
                "   ╚██████╔╝███████╗███████║   ██║   ██║╚██████╔╝██║ ╚████║",
                "    ╚═════╝ ╚══════╝╚══════╝   ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝",
                "",
                "             ███████╗████████╗ ██████╗  ██████╗██╗  ██╗",
                "             ██╔════╝╚══██╔══╝██╔═══██╗██╔════╝██║ ██╔╝",
                "             ███████╗   ██║   ██║   ██║██║     █████╔╝ ",
                "             ╚════██║   ██║   ██║   ██║██║     ██╔═██╗ ",
                "             ███████║   ██║   ╚██████╔╝╚██████╗██║  ██╗",
                "             ╚══════╝   ╚═╝    ╚═════╝  ╚═════╝╚═╝  ╚═╝",
                "",
                "           GESTION DE STOCK PRO 2025 - ÉDITION PRESTIGE"
            };

            int w = Console.WindowWidth;
            int h = Console.WindowHeight;
            int y = Math.Max(3, (h - logo.Length - 8) / 2);

            for (int i = 0; i < logo.Length; i++)
            {
                int x = Math.Max(0, (w - logo[i].Length) / 2);
                Console.SetCursorPosition(x, y + i);
                Console.ForegroundColor = i == logo.Length - 1 ? ConsoleColor.Yellow : ConsoleColor.Cyan;
                Console.WriteLine(logo[i]);
            }

            int barY = y + logo.Length + 3;
            int barX = Math.Max(0, (w - 60) / 2);
            Console.SetCursorPosition(barX, barY);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Chargement du système ");

            for (int i = 0; i <= 100; i += 4)
            {
                int filled = i * 50 / 100;
                string bar = new string('█', filled) + new string('░', 50 - filled);
                Console.SetCursorPosition(barX + 22, barY);
                Console.ForegroundColor = i < 70 ? ConsoleColor.Yellow : ConsoleColor.Green;
                Console.Write($"[{bar}] {i,3}%");
                Thread.Sleep(50);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" OK !");
            Console.Beep(1200, 400);

            Console.SetCursorPosition(Math.Max(0, (w - 50) / 2), barY + 3);
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey(true);
        }
    }
}
