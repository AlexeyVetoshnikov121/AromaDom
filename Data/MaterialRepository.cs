using ManageMaterials.wpf.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

public class MaterialRepository
{
    private readonly string _connectionString;

    public MaterialRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<Material> GetAllMaterials()
    {
        var list = new List<Material>();

        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            string sql = @"
                SELECT 
                    m.Id,
                    m.MaterialTypeId,
                    mt.Name AS TypeName,
                    m.Name,
                    m.Price,
                    m.Unit,
                    m.QuantityInStock,
                    m.MinQuantity,
                    m.QuantityInPack,
                    ISNULL((
                        SELECT SUM(pm.Quantity) 
                        FROM ProductMaterials pm 
                        WHERE pm.MaterialId = m.Id
                    ), 0) AS RequiredQuantity
                FROM Materials m
                INNER JOIN MaterialTypes mt ON m.MaterialTypeId = mt.Id
                ORDER BY mt.Name, m.Name";

            using (var cmd = new SqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new Material
                    {
                        Id = reader.GetInt32(0),
                        MaterialTypeId = reader.GetInt32(1),
                        TypeName = reader.GetString(2),
                        Name = reader.GetString(3),
                        Price = reader.GetDecimal(4),
                        Unit = reader.GetString(5),
                        QuantityInStock = reader.GetDecimal(6),
                        MinQuantity = reader.GetDecimal(7),
                        QuantityInPack = reader.IsDBNull(8) ? 0 : reader.GetDecimal(8),
                        RequiredQuantity = reader.GetDecimal(9)
                    });
                }
            }
        }
        return list;
    }

    public List<KeyValuePair<int, string>> GetMaterialTypes()
    {
        var list = new List<KeyValuePair<int, string>>();
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new SqlCommand("SELECT Id, Name FROM MaterialTypes ORDER BY Name", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    list.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
            }
        }
        return list;
    }

    public void InsertMaterial(Material material)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            string sql = @"
                INSERT INTO Materials (MaterialTypeId, Name, Price, Unit, QuantityInStock, MinQuantity, QuantityInPack)
                VALUES (@TypeId, @Name, @Price, @Unit, @Stock, @Min, @Pack)";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TypeId", material.MaterialTypeId);
                cmd.Parameters.AddWithValue("@Name", material.Name);
                cmd.Parameters.AddWithValue("@Price", material.Price);
                cmd.Parameters.AddWithValue("@Unit", material.Unit);
                cmd.Parameters.AddWithValue("@Stock", material.QuantityInStock);
                cmd.Parameters.AddWithValue("@Min", material.MinQuantity);
                cmd.Parameters.AddWithValue("@Pack", (object)material.QuantityInPack ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public void UpdateMaterial(Material material)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            string sql = @"
                UPDATE Materials SET
                    MaterialTypeId = @TypeId,
                    Name = @Name,
                    Price = @Price,
                    Unit = @Unit,
                    QuantityInStock = @Stock,
                    MinQuantity = @Min,
                    QuantityInPack = @Pack
                WHERE Id = @Id";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", material.Id);
                cmd.Parameters.AddWithValue("@TypeId", material.MaterialTypeId);
                cmd.Parameters.AddWithValue("@Name", material.Name);
                cmd.Parameters.AddWithValue("@Price", material.Price);
                cmd.Parameters.AddWithValue("@Unit", material.Unit);
                cmd.Parameters.AddWithValue("@Stock", material.QuantityInStock);
                cmd.Parameters.AddWithValue("@Min", material.MinQuantity);
                cmd.Parameters.AddWithValue("@Pack", (object)material.QuantityInPack ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }
    }
}