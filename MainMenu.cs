using System;

namespace GestionApp
{
    public class MainMenu : MenuBase
    {
        private const string ADMIN_PASSWORD = "1234";

        public override void Show()
        {
            while (true)
            {
                ShowTitle("GESTION DE STOCK KEYCE PRO 2025");
                Console.WriteLine("1. Admin (mot de passe requis)");
                Console.WriteLine("2. Client");
                Console.WriteLine("3. Quitter");
                Console.Write("→ ");
                string? choix = Console.ReadLine();

                if (choix == "1")
                {
                    Console.Write("Mot de passe admin : ");
                    if (Console.ReadLine() == ADMIN_PASSWORD)
                    {
                        // Polymorphisme : On appelle Show() sur l'instance de AdminMenu
                        MenuBase adminMenu = new AdminMenu();
                        adminMenu.Show();
                    }
                    else { Console.WriteLine("Accès refusé !"); Pause(); }
                }
                else if (choix == "2")
                {
                    // Polymorphisme : On appelle Show() sur l'instance de ClientMenu
                    MenuBase clientMenu = new ClientMenu();
                    clientMenu.Show();
                }
                else if (choix == "3") break;
            }

            Console.Clear();
            Console.WriteLine("Merci et à bientôt !".PadLeft(50));
            System.Threading.Thread.Sleep(2000);
        }
    }
}
