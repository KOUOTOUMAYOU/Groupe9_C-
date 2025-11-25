using System;

namespace GestionApp
{
    // Classe de base pour toutes les entités
    public abstract class Entity
    {
        public int Id { get; set; }
    }

    public class Categorie : Entity
    {
        public string Nom { get; set; } = string.Empty;

        public override string ToString() => Nom;
    }

    public class Produit : Entity
    {
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Stock { get; set; }
        public int? CategoryId { get; set; }
        public Categorie? Categorie { get; set; } // Relation d'objet

        public override string ToString() => $"{Name} ({Price:F2} €)";
    }

    public class Client : Entity
    {
        public string Nom { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;

        public override string ToString() => Nom;
    }

    public class Vente : Entity
    {
        public int ProductId { get; set; }
        public int ClientId { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }

        public Produit? Produit { get; set; }
        public Client? Client { get; set; }

        public override string ToString() => $"{Produit?.Name} x {Quantity} ({SaleDate:dd/MM/yyyy})";
    }
}
