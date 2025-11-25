using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace GestionApp
{
    public static class Database
    {
        private static string GetDatabasePath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, "Gestion Stocks Keyce");
            Directory.CreateDirectory(appFolder);
            return Path.Combine(appFolder, "inventory.db");
        }

        private static string ConnectionString => $"Data Source={GetDatabasePath()}";

        private static SqliteConnection? connection;

        public static SqliteConnection Connection => connection ??= new SqliteConnection(ConnectionString).OpenAndReturn();

        // Repositories
        public static ProductRepository Products { get; private set; } = null!;
        public static ClientRepository Clients { get; private set; } = null!;

        private static SqliteConnection OpenAndReturn(this SqliteConnection conn)
        {
            conn.Open();
            return conn;
        }

        public static void Init()
        {
            // Initialisation de la base de données (schéma)
            string databasePath = GetDatabasePath();
            string appFolder = Path.GetDirectoryName(databasePath)!;
            Directory.CreateDirectory(appFolder);

            string sourceDbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inventory.db");
            if (!File.Exists(databasePath) && File.Exists(sourceDbPath))
            {
                File.Copy(sourceDbPath, databasePath, false);
            }

            using var cmd = Connection.CreateCommand();
            cmd.CommandText = """
                CREATE TABLE IF NOT EXISTS Categories (Id INTEGER PRIMARY KEY AUTOINCREMENT, Nom TEXT NOT NULL UNIQUE);
                CREATE TABLE IF NOT EXISTS Products (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, Price REAL NOT NULL, Stock INTEGER NOT NULL, CategoryId INTEGER);
                CREATE TABLE IF NOT EXISTS Clients (Id INTEGER PRIMARY KEY AUTOINCREMENT, Nom TEXT NOT NULL, Telephone TEXT NOT NULL UNIQUE);
                CREATE TABLE IF NOT EXISTS Sales (Id INTEGER PRIMARY KEY AUTOINCREMENT, ProductId INTEGER, ClientId INTEGER, Quantity INTEGER NOT NULL, SaleDate TEXT NOT NULL);
                """;
            cmd.ExecuteNonQuery();

            // Mise à jour du schéma (si nécessaire)
            void AddColumn(string table, string col, string type)
            {
                cmd.CommandText = $"PRAGMA table_info({table})";
                bool exists = false;
                using var r = cmd.ExecuteReader();
                while (r.Read()) if (r.GetString(1) == col) exists = true;
                if (!exists)
                {
                    cmd.CommandText = $"ALTER TABLE {table} ADD COLUMN {col} {type}";
                    cmd.ExecuteNonQuery();
                }
            }

            AddColumn("Products", "CategoryId", "INTEGER");
            AddColumn("Sales", "ClientId", "INTEGER");

            // Initialisation des données de base
            cmd.CommandText = "INSERT OR IGNORE INTO Clients (Nom, Telephone) VALUES ('Anonyme', '0000000000')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "SELECT Id FROM Clients WHERE Telephone = '0000000000'";
            long anonymeId = (long)cmd.ExecuteScalar()!;

            cmd.CommandText = "UPDATE Sales SET ClientId = @id WHERE ClientId IS NULL";
            cmd.Parameters.AddWithValue("@id", anonymeId);
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT OR IGNORE INTO Categories (Nom) VALUES ('Boissons'), ('Snacks'), ('Hygiène'), ('Divers')";
            cmd.ExecuteNonQuery();

            // Initialisation des Repositories
            Products = new ProductRepository(Connection);
            Clients = new ClientRepository(Connection);
        }
    }
}
