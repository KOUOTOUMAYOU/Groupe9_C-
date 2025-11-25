using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace GestionApp
{
    public class ClientRepository
    {
        private readonly SqliteConnection _connection;

        public ClientRepository(SqliteConnection connection)
        {
            _connection = connection;
        }

        public Client? GetClientByPhone(string telephone)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Nom, Telephone FROM Clients WHERE Telephone = @tel";
            cmd.Parameters.AddWithValue("@tel", telephone);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                return new Client
                {
                    Id = r.GetInt32(0),
                    Nom = r.GetString(1),
                    Telephone = r.GetString(2)
                };
            }
            return null;
        }

        public Client AddClient(string nom, string telephone)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Clients (Nom, Telephone) VALUES (@n, @t); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@n", nom);
            cmd.Parameters.AddWithValue("@t", telephone);
            int id = Convert.ToInt32(cmd.ExecuteScalar()!);
            return new Client { Id = id, Nom = nom, Telephone = telephone };
        }

        public void AddSale(Vente vente)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Sales (ProductId, ClientId, Quantity, SaleDate) VALUES (@p, @c, @q, @d)";
            cmd.Parameters.AddWithValue("@p", vente.ProductId);
            cmd.Parameters.AddWithValue("@c", vente.ClientId);
            cmd.Parameters.AddWithValue("@q", vente.Quantity);
            cmd.Parameters.AddWithValue("@d", vente.SaleDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
        }

        public List<Vente> GetSalesStats()
        {
            var ventes = new List<Vente>();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = """
                SELECT s.Quantity, s.SaleDate, p.Name, COALESCE(cl.Nom, 'Anonyme')
                FROM Sales s
                JOIN Products p ON s.ProductId = p.Id
                LEFT JOIN Clients cl ON s.ClientId = cl.Id
                ORDER BY s.SaleDate DESC
                """;
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                ventes.Add(new Vente
                {
                    Quantity = r.GetInt32(0),
                    SaleDate = DateTime.Parse(r.GetString(1)),
                    Produit = new Produit { Name = r.GetString(2) },
                    Client = new Client { Nom = r.GetString(3) }
                });
            }
            return ventes;
        }
    }
}
