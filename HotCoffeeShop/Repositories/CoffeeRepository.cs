using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using HotCoffeeShop.Models;
using Microsoft.Data.SqlClient;

namespace HotCoffeeShop.Repositories
{
				public class CoffeeRepository
				{
								private readonly string _connectionString;
								public CoffeeRepository(IConfiguration configuration)
								{
												_connectionString = configuration.GetConnectionString("Default Connection");
								}

								private SqlConnection Connection
								{
												get { return new SqlConnection(_connectionString); }
								}

								public List<Coffee> GetAll()
								{
												using (SqlConnection conn = Connection)
												{
																conn.Open();
																using (SqlCommand cmd = conn.CreateCommand())
																{
																				cmd.CommandText = @"
																								SELECT 
																												cf.Id,
																												cf.Title,
																												bv.Name AS BeanVarietyName
																								FROM Coffee cf
																								LEFT JOIN BeanVariety bv ON cf.BeanVarietyId = bv.Id
																				";
																				SqlDataReader reader = cmd.ExecuteReader();
																				List<Coffee> coffees = new List<Coffee>();
																				while (reader.Read())
																				{
																								Coffee coffee = new Coffee()
																								{
																												Id = reader.GetInt32(reader.GetOrdinal("Id")),
																												Name = reader.GetString(reader.GetOrdinal("Title")),
																								};
																								if (!reader.IsDBNull(reader.GetOrdinal("BeanVarietyName")))
																								{
																												coffee.BeanVariety = reader.GetString(reader.GetOrdinal("BeanVarietyName"))
																								}

																								coffees.Add(coffee);
																				}

																				reader.Close();
																				return coffees;

																}
												}
								}

								public Coffee Get(int id)
								{
												using (SqlConnection conn = Connection)
												{
																conn.Open();
																using (var cmd = conn.CreateCommand())
																{
																				cmd.CommandText = @"
																								SELECT 
																												cf.Id,
																												cf.Title,
																												bv.Name AS BeanVarietyName
																								FROM Coffee cf
																								LEFT JOIN BeanVariety bv ON cf.BeanVarietyId = bv.Id
																								WHERE Id = @id
																				";

																				cmd.Parameters.AddWithValue("@id", id);

																				SqlDataReader reader = cmd.ExecuteReader();

																				Coffee coffee = null;
																				if (reader.Read())
																				{
																								coffee = new Coffee()
																								{
																												Id = reader.GetInt32(reader.GetOrdinal("Id")),
																												Name = reader.GetString(reader.GetOrdinal("Title")),
																								};
																								if (!reader.IsDBNull(reader.GetOrdinal("BeanVarietyName")))
																								{
																												coffee.BeanVariety = reader.GetString(reader.GetOrdinal("BeanVarietyName"))
																								}
																				}

																				reader.Close();

																				return coffee;
																}
												}
								}

								public void Add(Coffee coffee)
								{
												using (SqlConnection conn = Connection)
												{
																conn.Open();
																using (SqlCommand cmd = conn.CreateCommand())
																{
																				cmd.CommandText = @"
																								INSERT INTO Coffee (Title, BeanVarietyId)
																								OUTPUT INSERTED.ID
																								VALUES (@title, @beanVarietyId)
																				";

																				cmd.Parameters.AddWithValue("@title", coffee.Name);
																				cmd.Parameters.AddWithValue("@beanVarietyId", coffee.BeanVarietyId);

																				coffee.Id = (int)cmd.ExecuteScalar();
																}

												}
								}

								public void Update(Coffee coffee)
								{
												using (SqlConnection conn = Connection)
												{
																conn.Open();
																using (SqlCommand cmd = conn.CreateCommand())
																{
																				cmd.CommandText = @"
																								UPDATE  coffee
																												SET Title = @name,
																																BeanVarietyId = @beanVarietyId
																								WHERE Id = @id
																				";
																				cmd.Parameters.AddWithValue("@id", coffee.Id);
																				cmd.Parameters.AddWithValue("@name", coffee.Name);
																				cmd.Parameters.AddWithValue("@beanVarietyId", DBNull.Value);

																				cmd.ExecuteNonQuery();
																}

												}
								}

								public void Delete(int id)
								{
												using (SqlConnection conn = Connection)
												{
																conn.Open();
																using (SqlCommand cmd = conn.CreateCommand())
																{
																				cmd.CommandText = "DELETED FROM Coffee WHERE Id = @id";
																				cmd.Parameters.AddWithValue("@id", id);

																				cmd.ExecuteNonQuery();
																}
												}
								}
				}
}