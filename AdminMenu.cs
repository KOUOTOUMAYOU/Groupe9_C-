using System;

namespace GestionApp
{
    public class AdminMenu : MenuBase
    {
        public override void Show()
        {
            while (true)
            {
                ShowTitle("=== PANNEAU ADMINISTRATION ===");
                Console.WriteLine("1. Gérer catégories");
                Console.WriteLine("2. Ajouter produit");
                Console.WriteLine("3. Modifier produit");
                Console.WriteLine("4. Supprimer produit");
                Console.WriteLine("5. Liste produits");
                Console.WriteLine("6. Statistiques ventes");
                Console.WriteLine("7. Retour");
                Console.Write("\n→ ");
                switch (Console.ReadLine())
                {
                    case "1": GererCategories(); break;
                    case "2": AjouterProduit(); break;
                    case "3": ModifierProduit(); break;
                    case "4": SupprimerProduit(); break;
                    case "5": ListerTousLesProduits(); break;
                    case "6": StatistiquesVentes(); break;
                    case "7": return;
                }
            }
        }

        private void GererCategories()
        {
            while (true)
            {
                ShowTitle("GESTION DES CATÉGORIES");
                ListerCategories();
                Console.WriteLine("\n1. Ajouter catégorie");
                Console.WriteLine("2. Supprimer catégorie");
                Console.WriteLine("3. Retour");
                Console.Write("→ ");
                string? c = Console.ReadLine();

                if (c == "1")
                {
                    Console.Write("Nom : ");
                    string? nom = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(nom))
                    {
                        if (Database.Products.AddCategorie(nom))
                            Console.WriteLine("Catégorie ajoutée !");
                        else
                            Console.WriteLine("Cette catégorie existe déjà.");
                        Pause();
                    }
                }
                else if (c == "2")
                {
                    Console.Write("ID à supprimer : ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    {
                        Console.WriteLine(Database.Products.DeleteCategorie(id) ? "Catégorie supprimée !" : "ID introuvable.");
                        Pause();
                    }
                }
                else if (c == "3") return;
            }
        }

        private void ListerCategories()
        {
            var categories = Database.Products.GetAllCategories();
            Console.WriteLine(" ID │ CATÉGORIE");
            Console.WriteLine("────┼────────────────────────────────");
            foreach (var cat in categories)
                Console.WriteLine($" {cat.Id,2} │ {cat.Nom}");
        }

        private void AjouterProduit()
        {
            ShowTitle("AJOUTER UN PRODUIT");
            ListerCategories();
            Console.Write("\nID catégorie (0 = aucune) : ");
            int.TryParse(Console.ReadLine(), out int catId);
            Console.Write("Nom du produit : "); string nom = Console.ReadLine() ?? "";
            Console.Write("Prix (€) : "); double.TryParse(Console.ReadLine(), out double prix);
            Console.Write("Stock initial : "); int.TryParse(Console.ReadLine(), out int stock);

            if (string.IsNullOrWhiteSpace(nom) || prix <= 0 || stock < 0)
            { Console.WriteLine("Données invalides !"); Pause(); return; }

            var produit = new Produit
            {
                Name = nom,
                Price = prix,
                Stock = stock,
                CategoryId = catId == 0 ? null : catId
            };

            if (Database.Products.AddProduct(produit))
                Console.WriteLine("Produit ajouté avec succès !");
            else
                Console.WriteLine("Erreur lors de l'ajout du produit.");
            Pause();
        }

        private void ModifierProduit()
        {
            ListerTousLesProduits(false);
            Console.Write("\nID à modifier : ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            Console.Write("Nouveau nom (vide = garder) : "); string? nom = Console.ReadLine();
            Console.Write("Nouveau prix (vide = garder) : "); string? prixStr = Console.ReadLine();
            Console.Write("Nouveau stock (vide = garder) : "); string? stockStr = Console.ReadLine();
            Console.Write("Nouvelle catégorie ID (vide = garder) : "); string? catStr = Console.ReadLine();

            double? prix = !string.IsNullOrWhiteSpace(prixStr) ? double.Parse(prixStr) : null;
            int? stock = !string.IsNullOrWhiteSpace(stockStr) ? int.Parse(stockStr) : null;
            int? catId = !string.IsNullOrWhiteSpace(catStr) ? int.Parse(catStr) : null;

            Console.WriteLine(Database.Products.UpdateProduct(id, nom, prix, stock, catId) ? "Produit modifié !" : "Produit introuvable.");
            Pause();
        }

        private void SupprimerProduit()
        {
            ListerTousLesProduits(false);
            Console.Write("\nID à supprimer : ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine(Database.Products.DeleteProduct(id) ? "Produit supprimé !" : "ID introuvable.");
            }
            Pause();
        }

        private void ListerTousLesProduits(bool pause = true)
        {
            ShowTitle("LISTE COMPLÈTE DES PRODUITS");
            Console.WriteLine(" ID │ PRODUIT                 │ PRIX     │ STOCK  │ CATÉGORIE");
            Console.WriteLine(new string('─', 78));

            var produits = Database.Products.GetAllProducts(true);
            foreach (var p in produits)
            {
                Console.ForegroundColor = p.Stock <= 5 ? ConsoleColor.Red : ConsoleColor.White;
                Console.WriteLine($" {p.Id,2} │ {p.Name,-23} │ {p.Price,7:F2} € │ {p.Stock,5} │ {p.Categorie?.Nom ?? "Sans catégorie"}");
                Console.ResetColor();
            }
            if(pause) Pause();
        }

        private void StatistiquesVentes()
        {
            ShowTitle("STATISTIQUES DES VENTES");
            var ventes = Database.Clients.GetSalesStats();
            if (ventes.Count == 0)
                Console.WriteLine("Aucune vente enregistrée.");
            else
            {
                foreach (var v in ventes)
                {
                    Console.WriteLine($"{v.Produit?.Name} × {v.Quantity} | {v.SaleDate:dd/MM/yyyy HH:mm} | Client: {v.Client?.Nom}");
                }
            }
            Pause();
        }
    }
}
