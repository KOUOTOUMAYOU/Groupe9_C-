using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;

namespace GestionApp
{
    // Implémentation du pattern Repository pour les entités Produit et Catégorie
    public class ProductRepository
    {
        private readonly SqliteConnection _connection;

        public ProductRepository(SqliteConnection connection)
        {
            _connection = connection;
        }

        // --- Catégories ---

        public List<Categorie> GetAllCategories()
        {
            var categories = new List<Categorie>();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Nom FROM Categories ORDER BY Nom";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                categories.Add(new Categorie { Id = r.GetInt32(0), Nom = r.GetString(1) });
            }
            return categories;
        }

        public bool AddCategorie(string nom)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Categories (Nom) VALUES (@n)";
            cmd.Parameters.AddWithValue("@n", nom);
            try { return cmd.ExecuteNonQuery() > 0; }
            catch { return false; } // Catégorie existe déjà
        }

        public bool DeleteCategorie(int id)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Categories WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        // --- Produits ---

        public List<Produit> GetAllProducts(bool includeCategories = false)
        {
            var produits = new List<Produit>();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = """
                SELECT p.Id, p.Name, p.Price, p.Stock, p.CategoryId, c.Nom
                FROM Products p LEFT JOIN Categories c ON p.CategoryId = c.Id
                ORDER BY c.Nom, p.Name
                """;
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var produit = new Produit
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    Price = r.GetDouble(2),
                    Stock = r.GetInt32(3),
                    CategoryId = r.IsDBNull(4) ? null : r.GetInt32(4),
                };

                if (includeCategories && !r.IsDBNull(5))
                {
                    produit.Categorie = new Categorie { Id = produit.CategoryId.Value, Nom = r.GetString(5) };
                }
                produits.Add(produit);
            }
            return produits;
        }

        public List<Produit> GetAvailableProducts()
        {
            return GetAllProducts(true).Where(p => p.Stock > 5).ToList();
        }

        public Produit? GetProductById(int id)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Price, Stock, CategoryId FROM Products WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                return new Produit
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    Price = r.GetDouble(2),
                    Stock = r.GetInt32(3),
                    CategoryId = r.IsDBNull(4) ? null : r.GetInt32(4),
                };
            }
            return null;
        }

        public bool AddProduct(Produit produit)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Products (Name, Price, Stock, CategoryId) VALUES (@n, @p, @s, @c)";
            cmd.Parameters.AddWithValue("@n", produit.Name);
            cmd.Parameters.AddWithValue("@p", produit.Price);
            cmd.Parameters.AddWithValue("@s", produit.Stock);
            cmd.Parameters.AddWithValue("@c", produit.CategoryId.HasValue ? produit.CategoryId.Value : DBNull.Value);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool UpdateProduct(int id, string? name, double? price, int? stock, int? categoryId)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = """
                UPDATE Products SET
                    Name = COALESCE(@n, Name),
                    Price = COALESCE(@p, Price),
                    Stock = COALESCE(@s, Stock),
                    CategoryId = COALESCE(@c, CategoryId)
                WHERE Id = @id
                """;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@n", name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@p", price ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@s", stock ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@c", categoryId ?? DBNull.Value);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteProduct(int id)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Products WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool UpdateStock(int productId, int quantityChange)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "UPDATE Products SET Stock = Stock + @q WHERE Id = @id";
            cmd.Parameters.AddWithValue("@q", quantityChange);
            cmd.Parameters.AddWithValue("@id", productId);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
