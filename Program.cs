using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ClipboardPartage
{
    // ──────────────────────────────
    // CLASSE PRINCIPALE – ORIENTÉE OBJET
    // ──────────────────────────────
    public class GestionnaireClipboard
    {
        // ██████████████████████████████████████████████████
        // ███ À CHANGER SUR CHAQUE ORDI (1 ligne seulement) ███
        // ██████████████████████████████████████████████████
        private readonly string IP_CIBLE = "192.168.1.42";   // ←←← CHANGE ÇA : IP de l'autre ordi
        // ██████████████████████████████████████████████████

        private const int PORT = 5000;
        private string dernierTexte = "";

        public GestionnaireClipboard()
        {
            Console.Title = "Clipboard partagé – Prêt pour la démo 7h";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════════════╗");
            Console.WriteLine("║       CLIPBOARD PARTAGÉ ENTRE DEUX PC            ║");
            Console.WriteLine("║        Ctrl+C = envoie  │  Ctrl+V = reçoit       ║");
            Console.WriteLine($"║        Envoi vers → {IP_CIBLE}:{PORT}                 ║");
            Console.WriteLine("╚══════════════════════════════════════════════════╝\n");
            Console.ResetColor();

            // Démarre l'écoute en arrière-plan
            new Thread(ServeurEcoute) { IsBackground = true }.Start();

            // Boucle principale
            BouclePrincipale();
        }

        private void BouclePrincipale()
        {
            while (true)
            {
                if (Clipboard.ContainsText())
                {
                    string actuel = Clipboard.GetText();

                    // Détection d'un nouveau Ctrl+C
                    if (!string.IsNullOrWhiteSpace(actuel) && actuel != dernierTexte)
                    {
                        Envoyer(actuel);
                        dernierTexte = actuel;
                    }
                }

                Thread.Sleep(150);
                Application.DoEvents();
            }
        }

        private void Envoyer(string texte)
        {
            try
            {
                using TcpClient client = new TcpClient(IP_CIBLE, PORT);
                byte[] data = Encoding.UTF8.GetBytes(texte);
                client.GetStream().Write(data, 0, data.Length);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"ENVOYÉ → {texte.Substring(0, Math.Min(50, texte.Length))}...");
                Console.ResetColor();
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Impossible d'envoyer (l'autre PC est éteint ?)");
                Console.ResetColor();
            }
        }

        private void ServeurEcoute()
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, PORT);
                listener.Start();

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[1048576];
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    string reçu = Encoding.UTF8.GetString(buffer, 0, bytes);

                    Clipboard.SetText(reçu);
                    SendKeys.SendWait("^v");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"REÇU ET COLLÉ ← {reçu.Substring(0, Math.Min(50, reçu.Length))}...");
                    Console.ResetColor();

                    client.Close();
                }
            }
            catch { }
        }
    }

    // ──────────────────────────────
    // PROGRAMME D'ENTRÉE
    // ──────────────────────────────
    internal class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _ = new GestionnaireClipboard();   // Démarre tout
            Application.Run();                 // Garde l'application vivante
        }
    }
}