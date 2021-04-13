using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlogApp
{
    class Program
    {
        private static string _connectionString = @"Server=NOTEBOOK\SQLEXPRESS;Database=CustomerOrderSystem;Trusted_Connection=True;";

        static void Main(string[] args)
        {
            string command = "get-info";

            if (command == "get-info")
            {
                List<Customer> customerInfos = GetCustomerInfos();
                foreach (Customer customer in customerInfos)
                {
                    Console.WriteLine(customer.Name + " " + customer.OrderNumber + " " + customer.Price);
                }
            }
        }

        private static List<Customer> GetCustomerInfos()
        {
            List<Customer> customerInfos = new List<Customer>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText =
                        @"SELECT [Customer].[CustomerId], 
		                         [Customer].[Name] as [CustomerName],
		                         COUNT([Order].[OrderId]) as [OrdersCount], 
		                         SUM([Order].[Price]) as [TotalPrice] 
		                  FROM [Customer] 
                          LEFT JOIN [Order] 
		                    ON ([Customer].[CustomerId] = [Order].[CustomerId]) 
                            GROUP BY [Customer].[CustomerId], [Customer].[Name]";
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var customer = new Customer
                            {
                                Name = Convert.ToString(reader["CustomerName"]),
                                OrderNumber = Convert.ToInt32(reader["OrdersNumber"]),
                                Price = Convert.ToInt32(reader["Price"])
                            };
                            customerInfos.Add(customer);
                        }
                    }
                }
            }
            return customerInfos;
        }
    }
}

