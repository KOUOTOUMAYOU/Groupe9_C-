using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GestionApp
{
    public class ClientMenu : MenuBase
    {
        private Client? clientActuel;

        public override void Show()
        {
            ShowTitle("CONNEXION CLIENT");
            Console.Write("Votre nom       : ");
            string nom = Console.ReadLine() ?? "";
            Console.Write("Votre téléphone : ");
            string tel = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(tel))
            {
                Console.WriteLine("Informations manquantes !");
                Pause();
                return;
            }

            clientActuel = Database.Clients.GetClientByPhone(tel);

            if (clientActuel == null)
            {
                clientActuel = Database.Clients.AddClient(nom, tel);
                Console.WriteLine("Nouveau client enregistré !");
            }

            Acheter();
        }

        private void Acheter()
        {
            var panier = new List<(Produit Produit, int Qty)>();

            while (true)
            {
                ShowTitle($"ACHATS - {clientActuel!.Nom.ToUpper()}");
                ListerProduitsDisponiblesClient();

                Console.Write("\nID produit (0 = terminer) → ");
                if (!int.TryParse(Console.ReadLine(), out int id) || id == 0) break;

                Console.Write("Quantité → ");
                if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0) continue;

                var produit = Database.Products.GetProductById(id);

                if (produit == null)
                {
                    Console.WriteLine("Produit inconnu !");
                    Pause();
                    continue;
                }

                if (produit.Stock - qty < 5)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ACHAT BLOQUÉ : stock minimum (5 unités) requis !");
                    Console.ResetColor();
                    Pause();
                    continue;
                }

                // Ajout au panier
                panier.Add((produit, qty));

                // Mise à jour du stock
                Database.Products.UpdateStock(produit.Id, -qty);

                // Enregistrement de la vente
                var vente = new Vente
                {
                    ProductId = produit.Id,
                    ClientId = clientActuel.Id,
                    Quantity = qty,
                    SaleDate = DateTime.Now
                };
                Database.Clients.AddSale(vente);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Ajouté : {qty} × {produit.Name}");
                Console.ResetColor();
                Pause();
            }

            if (panier.Count > 0)
            {
                Console.Write("Terminer et générer la facture ? (o/n) → ");
                if (Console.ReadLine()?.Trim().ToLower() == "o")
                    GenererFacture(panier);
            }
            else
            {
                Console.WriteLine("Aucun achat effectué.");
            }

            Pause();
        }

        private void ListerProduitsDisponiblesClient()
        {
            Console.WriteLine("PRODUITS DISPONIBLES (stock > 5)\n");
            Console.WriteLine(" ID │ PRODUIT                 │ PRIX     │ STOCK  │ CATÉGORIE");
            Console.WriteLine(new string('─', 78));

            var produits = Database.Products.GetAvailableProducts();
            foreach (var p in produits)
            {
                Console.WriteLine($" {p.Id,2} │ {p.Name,-23} │ {p.Price,7:F2} € │ {p.Stock,5} │ {p.Categorie?.Nom ?? "Sans catégorie"}");
            }
        }

        private void GenererFacture(List<(Produit Produit, int Qty)> panier)
        {
            string fichier = $"facture_{clientActuel!.Nom}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            using var writer = new StreamWriter(fichier, false, System.Text.Encoding.UTF8);
            writer.WriteLine($"FACTURE - {DateTime.Now:dd/MM/yyyy HH:mm}");
            writer.WriteLine($"Client : {clientActuel.Nom} | Téléphone : {clientActuel.Telephone}");
            writer.WriteLine();
            writer.WriteLine("ID;Produit;Prix unitaire;Quantité;Total ligne");
            double total = 0;
            foreach (var item in panier)
            {
                double ligne = item.Produit.Price * item.Qty;
                total += ligne;
                writer.WriteLine($"{item.Produit.Id};{item.Produit.Name};{item.Produit.Price:F2};{item.Qty};{ligne:F2}");
            }
            writer.WriteLine($";;;TOTAL;{total:F2} €");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nFACTURE GÉNÉRÉE → {fichier}");
            Console.ResetColor();
        }
    }
}
