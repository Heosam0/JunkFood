using Dapper;
using JunkFood.AppData;
using Npgsql;
using Npgsql.PostgresTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JunkFood.AppData
{
    public class db
    {

        public NpgsqlConnection connection;
        private string login;

        public string Initialize(string login, string password)
        {
            this.login = login;
            try
            {
                string connectionString = $"Host=localhost;Port=5432;Username={login};Password={password};Database=jf";
                connection = new NpgsqlConnection(connectionString);
                connection.Open();
                return "Successful";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void AddOrderToDatabase(Order order)
        {
            try
            {
                

                    string query = "INSERT INTO orders (howcall, datetime, fordelivery, status, sum, tablenumber) " +
                                   $"VALUES (@HowCall, @DateTime, @ForDelivery, '{order.Status}'::Status, {order.Sum}::money, @TableNumber::integer) RETURNING id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@HowCall", order.HowCall);
                        command.Parameters.AddWithValue("@DateTime", order.DateTime);
                        command.Parameters.AddWithValue("@ForDelivery", order.ForDelivery);
                        command.Parameters.AddWithValue("@TableNumber", order.TableNumber);

                        // Получение ID новой записи
                        order.Id = (int)command.ExecuteScalar();
                    }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddEmployeeToDatabase(Employee employee)
        {
            try
            {

                    string query = "INSERT INTO employees (first_name, last_name, patronymic, phone, birthday, start_date, post_id) " +
                                   "VALUES (@FirstName, @LastName, @Patronymic, @Phone, @Birthday, @StartDate, @PostId) RETURNING id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", employee.first_name);
                        command.Parameters.AddWithValue("@LastName", employee.last_name);
                        command.Parameters.AddWithValue("@Patronymic", employee.Patronymic);
                        command.Parameters.AddWithValue("@Phone", employee.Phone);
                        command.Parameters.AddWithValue("@Birthday", employee.Birthday);
                        command.Parameters.AddWithValue("@StartDate", employee.Start_date);
                        command.Parameters.AddWithValue("@PostId", employee.Post_id);

                        employee.Id = (int)command.ExecuteScalar();
                    }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении сотрудника: {ex.Message}");
            }
        }

        public void AddMenuItemToDatabase(MenuItem menuItem)
        {
            try
            {

                    string query = "INSERT INTO menu (name, description, price) VALUES (@Name, @Description, @Price) RETURNING id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", menuItem.Name);
                        command.Parameters.AddWithValue("@Description", menuItem.Description);
                        command.Parameters.AddWithValue("@Price", menuItem.Price);

                        menuItem.Id = (int)command.ExecuteScalar();
                    }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении пункта меню: {ex.Message}");
            }
        }
        public void AddIngredientToDatabase(MenuIngredient ingredient)
        {
            try
            {

                    string query = "INSERT INTO menu_ingredients (name, count) VALUES (@Name, @Count) RETURNING id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", ingredient.Name);
                        command.Parameters.AddWithValue("@Count", ingredient.Unit);

                        ingredient.Id = (int)command.ExecuteScalar();
                    }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении ингредиента: {ex.Message}");
            }
        }



        public List<Employee> GetEmployees()
        {
            return connection.Query<Employee>("SELECT * FROM employees order by id").ToList();
        }

        public List<Order> GetOrders()
        {
            return connection.Query<Order>("SELECT * FROM orders order by id").ToList();
        }

        public List<MenuItem> GetMenuItems()
        {
            return connection.Query<MenuItem>("SELECT * FROM menu order by id").ToList();
        }
        public List<MenuIngredient> GetMenuIngreds()
        {
            return connection.Query<MenuIngredient>("SELECT * FROM show_ingredients order by id").ToList();
        }

        public void UpdateEmployee(Employee employee)
        {
            var sql = "UPDATE employees SET first_name = @FirstName, last_name = @LastName, patronymic = @Patronymic, phone = @Phone, birthday = @Birthday, start_date = @StartDate, post_id = @PostId WHERE id = @Id";
            connection.Execute(sql, employee);
        }

        public void AddEmployee(Employee employee)
        {
            var sql = "INSERT INTO employees (first_name, last_name, patronymic, phone, birthday, start_date, post_id) VALUES (@FirstName, @LastName, @Patronymic, @Phone, @Birthday, @StartDate, @PostId)";
            connection.Execute(sql, employee);
        }

        public void DeleteEmployee(int id)
        {
            var sql = "DELETE FROM employees WHERE id = @Id";
            connection.Execute(sql, new { Id = id });
        }






        public void Code(string sql)
        {
            try
            {
                connection.Execute(sql);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

            }

        }
        public bool UpdateRecord(string tableName, int id, Dictionary<string, object> updatedValues)
        {
            // Проверка роли текущего пользователя
            if (HasUpdateAccess(tableName))
            {
                MessageBox.Show("У вас нет прав для редактирования этой записи.");
                return false;
            }

            // Формируем SQL-запрос для обновления записи
            var sql = new StringBuilder($"UPDATE {tableName} SET ");
            var parameters = new List<NpgsqlParameter>();
            int paramIndex = 0;

            foreach (var item in updatedValues)
            {
                sql.Append($"{item.Key} = @param{paramIndex}, ");
                parameters.Add(new NpgsqlParameter($"@param{paramIndex}", item.Value));
                paramIndex++;
            }

            // Убираем лишнюю запятую и пробел в конце строки
            sql.Length -= 2;
            sql.Append(" WHERE id = @id");

            parameters.Add(new NpgsqlParameter("@id", id));

            try
            {
                using (var command = new NpgsqlCommand(sql.ToString(), connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());
                    command.ExecuteNonQuery();
                    MessageBox.Show("Запись успешно обновлена.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении записи: {ex.Message}");
                return false;
            }
        }

        // Метод для проверки прав на редактирование в зависимости от роли
        private bool HasUpdateAccess(string tableName)
        {
            switch (login)
            {
                case "postgre":
                    return true; // доступ ко всем таблицам
                case "manager":
                    return tableName == "employees" || tableName == "orders" || tableName == "menu";
                case "cashier":
                    return tableName == "orders";
                default:
                    return false;
            }
        }




    }
}
