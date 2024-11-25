using System;
using System.Data;
using System.Data.SqlClient;

namespace PetStore
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=DESKTOP-00L2CO6\\SQLEXPRESS;Database=PetStoreDB;Trusted_Connection=True;";
            SqlConnection conn = new SqlConnection(connectionString);

            while (true)
            {
                Console.WriteLine("Select an option from the list below");
                Console.WriteLine("0 - Get all tables");
                Console.WriteLine("1 - Select Table");
                Console.WriteLine("2 - Insert Pet");
                Console.WriteLine("3 - Get Orders with JOIN");
                Console.WriteLine("4 - Filter Products by Price");
                Console.WriteLine("5 - Get Aggregate Data (Total Stock)");
                Console.WriteLine("6 - Delete Record");
                Console.WriteLine("7 - Cascade Delete Record");
                Console.WriteLine("8 - Update Record");
                Console.WriteLine("9 - Exit");
                Console.Write("Enter your choice: ");

                int swt;
                if (!int.TryParse(Console.ReadLine(), out swt))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (swt)
                {
                    case 0:
                        {
                            Selects sel = new Selects(conn);
                            sel.GetTables();
                            break;
                        }
                    case 1:
                        {
                            Selects sel = new Selects(conn);
                            Console.WriteLine("Enter table name:");
                            string tbln = Console.ReadLine();
                            sel.SelectAllItems(tbln);
                            break;
                        }
                    case 2:
                        {
                            Insert ins = new Insert(conn);
                            Console.WriteLine("Enter Pet Name, Species, Breed, Age (comma-separated):");
                            string petData = Console.ReadLine();
                            ins.InsertPet(petData);
                            break;
                        }
                    case 3:
                        {
                            Selects sel = new Selects(conn);
                            sel.GetOrdersWithJoin();
                            break;
                        }
                    case 4:
                        {
                            Selects sel = new Selects(conn);
                            Console.WriteLine("Enter price to filter:");
                            if (decimal.TryParse(Console.ReadLine(), out decimal price))
                            {
                                sel.FilterProductsByPrice(price);
                            }
                            else
                            {
                                Console.WriteLine("Invalid price input.");
                            }
                            break;
                        }
                    case 5:
                        {
                            Selects sel = new Selects(conn);
                            sel.GetTotalStock();
                            break;
                        }
                    case 6:
                        {
                            Delete del = new Delete(conn);
                            Console.WriteLine("Enter table name:");
                            string tableName = Console.ReadLine();
                            Console.WriteLine("Enter primary key column name:");
                            string columnName = Console.ReadLine();
                            Console.WriteLine("Enter record ID:");
                            int id = Convert.ToInt32(Console.ReadLine());
                            del.DeleteRecord(tableName, columnName, id);
                            break;
                        }
                    case 7:
                        {
                            Delete del = new Delete(conn);
                            Console.WriteLine("Enter table name:");
                            string tableName = Console.ReadLine();
                            Console.WriteLine("Enter primary key column name:");
                            string columnName = Console.ReadLine();
                            Console.WriteLine("Enter record ID:");
                            int id = Convert.ToInt32(Console.ReadLine());
                            del.CascadeDeleteRecord(tableName, columnName, id);
                            break;
                        }
                    case 8:
                        {
                            try
                            {
                                Console.WriteLine("Enter table name:");
                                string tableName = Console.ReadLine();

                                Console.WriteLine("Enter primary key column name:");
                                string primaryKeyColumn = Console.ReadLine();

                                Console.WriteLine("Enter record ID:");
                                if (!int.TryParse(Console.ReadLine(), out int recordID))
                                {
                                    Console.WriteLine("Invalid record ID.");
                                    break;
                                }

                                Console.WriteLine("Enter column name to update:");
                                string columnName = Console.ReadLine();

                                Console.WriteLine("Enter new value:");
                                string newValue = Console.ReadLine();

                                conn.Open();
                                string query = $"UPDATE {tableName} SET {columnName} = @NewValue WHERE {primaryKeyColumn} = @ID";
                                SqlCommand cmd = new SqlCommand(query, conn);
                                cmd.Parameters.AddWithValue("@NewValue", newValue);
                                cmd.Parameters.AddWithValue("@ID", recordID);

                                int rowsAffected = cmd.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    Console.WriteLine("Record updated successfully.");
                                }
                                else
                                {
                                    Console.WriteLine("No records were updated. Check table name, column name, or ID.");
                                }
                            }
                            catch (SqlException ex)
                            {
                                Console.WriteLine($"SQL error: {ex.Message}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            finally
                            {
                                conn.Close();
                            }
                            break;
                        }

                    case 9:
                        {
                            Console.WriteLine("Exiting the program...");
                            return;
                        }
                    default:
                        {
                            Console.WriteLine("Invalid choice. Please select a valid option.");
                            break;
                        }
                }

                Console.WriteLine(); // Вставляємо пустий рядок для відділення запитів
            }
        }
    }

    class Selects
    {
        private SqlConnection conn;
        public Selects(SqlConnection connection)
        {
            conn = connection;
        }

        public void GetTables()
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader[0].ToString());
            }
            conn.Close();
        }

        public void SelectAllItems(string tableName)
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand($"SELECT * FROM {tableName}", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write(reader[i].ToString() + "\t");
                }
                Console.WriteLine();
            }
            conn.Close();
        }

        public void GetOrdersWithJoin()
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand(
                "SELECT o.OrderID, o.OrderDate, p.Name, op.Quantity FROM [Order] o " +
                "JOIN OrderProduct op ON o.OrderID = op.OrderID " +
                "JOIN Product p ON op.ProductID = p.ProductID", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"{reader["OrderID"]}, {reader["OrderDate"]}, {reader["Name"]}, {reader["Quantity"]}");
            }
            conn.Close();
        }

        public void FilterProductsByPrice(decimal price)
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand($"SELECT * FROM Product WHERE Price < @Price", conn);
            cmd.Parameters.AddWithValue("@Price", price);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"{reader["ProductID"]}, {reader["Name"]}, {reader["Price"]}, {reader["Stock"]}");
            }
            conn.Close();
        }

        public void GetTotalStock()
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT SUM(Stock) AS TotalStock FROM Product", conn);
            object result = cmd.ExecuteScalar();
            Console.WriteLine($"Total Stock: {result}");
            conn.Close();
        }
    }

    class Insert
    {
        private SqlConnection conn;
        public Insert(SqlConnection connection)
        {
            conn = connection;
        }

        public void InsertPet(string petData)
        {
            string[] data = petData.Split(',');
            conn.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO Pet (Name, Species, Breed, Age) VALUES (@Name, @Species, @Breed, @Age)", conn);
            cmd.Parameters.AddWithValue("@Name", data[0].Trim());
            cmd.Parameters.AddWithValue("@Species", data[1].Trim());
            cmd.Parameters.AddWithValue("@Breed", data[2].Trim());
            cmd.Parameters.AddWithValue("@Age", Convert.ToInt32(data[3].Trim()));
            cmd.ExecuteNonQuery();
            conn.Close();
            Console.WriteLine("Pet added successfully.");
        }
    }

    class Delete
    {
        private SqlConnection conn;
        public Delete(SqlConnection connection)
        {
            conn = connection;
        }

        public void DeleteRecord(string tableName, string columnName, int id)
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand($"DELETE FROM {tableName} WHERE {columnName} = @ID", conn);
            cmd.Parameters.AddWithValue("@ID", id);
            cmd.ExecuteNonQuery();
            conn.Close();
            Console.WriteLine("Record deleted successfully.");
        }

        public void CascadeDeleteRecord(string tableName, string columnName, int id)
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand($"DELETE FROM {tableName} WHERE {columnName} = @ID", conn);
            cmd.Parameters.AddWithValue("@ID", id);
            cmd.ExecuteNonQuery();
            conn.Close();
            Console.WriteLine("Cascade delete executed successfully.");
        }
    }

    class Update
    {
        private SqlConnection conn;
        public Update(SqlConnection connection)
        {
            conn = connection;
        }

        public void UpdateRecord(string tableName, string columnName, int id, string newValue)
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand($"UPDATE {tableName} SET {columnName} = @Value WHERE Id = @ID", conn);
            cmd.Parameters.AddWithValue("@Value", newValue);
            cmd.Parameters.AddWithValue("@ID", id);
            cmd.ExecuteNonQuery();
            conn.Close();
            Console.WriteLine("Record updated successfully.");
        }
    }
}
