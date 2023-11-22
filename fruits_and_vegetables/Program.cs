using Microsoft.Data.Sqlite;
using System.Data;
using System.Drawing;
using System.Text;

internal class Program
{
    private static string ConnectionString = "Data Source=fruits_and_vegetables.sqlite;";

    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        using var connection = new SqliteConnection(ConnectionString);

        connection.Open();

        CreateTable(connection);

        CreateItem(connection);
        ReadAndDisplayAll(connection, "SELECT * FROM fruits_and_vegetables", "Усі дані про фрукти та овочі");
        ReadAndDisplayAll(connection, "SELECT name FROM fruits_and_vegetables", "Всі назви овочів і фруктів");
        ReadAndDisplayAll(connection, "SELECT color FROM fruits_and_vegetables", "Всі назви кольорів");
        ReadAndDisplayAll(connection, "SELECT MIN(calorie) FROM fruits_and_vegetables", "Мінімальна калорійність");
        ReadAndDisplayAll(connection, "SELECT MAX(calorie) FROM fruits_and_vegetables", "Максимальна калорійність");
        ReadAndDisplayAll(connection, "SELECT AVG(calorie) FROM fruits_and_vegetables", "Середня калорійність");
        ReadAndDisplayAll(connection, "SELECT COUNT(*) FROM fruits_and_vegetables WHERE type =\"Овоч\" ", "Кількість овочів");
        ReadAndDisplayAll(connection, "SELECT COUNT(*) FROM fruits_and_vegetables WHERE type =\"Фрукт\" ", "Кількість фруктів");
        ReadAndDisplayAll(connection, "SELECT COUNT(*) FROM fruits_and_vegetables WHERE color =\"Білий\" ", "Кількість овочів і фруктів білого кольору");
        ReadAndDisplayAll(connection, "SELECT color,  COUNT(*) AS count FROM fruits_and_vegetables GROUP BY color", "Кількість овочів і фруктів кожного кольору");
        ReadAndDisplayAll(connection, "SELECT name, calorie FROM fruits_and_vegetables WHERE calorie < 50", "Овочі і фрукти з калорійністю нижче 50");
        ReadAndDisplayAll(connection, "SELECT name, calorie FROM fruits_and_vegetables WHERE calorie > 50", "Овочі і фрукти з калорійністю вище 50");
        ReadAndDisplayAll(connection, "SELECT name, calorie FROM fruits_and_vegetables WHERE calorie > 30 AND calorie < 70", "Овочі і фрукти з калорійністю в діапазоні від 30 до 70");
        ReadAndDisplayAll(connection, "SELECT name FROM fruits_and_vegetables WHERE color =\"Червоний\" ", "Овочі і фрукти червоного кольору");
        ReadAndDisplayAll(connection, "SELECT name FROM fruits_and_vegetables WHERE color =\"Жовтий\" ", "Овочі і фрукти жовтого кольору");
    }

    private static void CreateTable(SqliteConnection connection)
    {
        string sql = "create table fruits_and_vegetables (name varchar(255), type varchar(255), color varchar(255), calorie int)";
        var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static void CreateItem(SqliteConnection connection)
    {
        using var transaction = connection.BeginTransaction();

        string[,] items = {
    {"Яблуко", "Фрукт", "Червоний", "52"},
    {"Апельсин", "Фрукт", "Помаранчевий", "47"},
    {"Банан", "Фрукт", "Жовтий", "89"},
    {"Авокадо", "Фрукт", "Зелений", "160"},
    {"Груша", "Фрукт", "Жовто-зелений", "57"},
    {"Картопля", "Овоч", "Коричневий", "77"},
    {"Томат", "Овоч", "Червоний", "18"},
    {"Буряк", "Овоч", "Фіолетовий", "45"},
    {"Морква", "Овоч", "Помаранчевий", "41"},
    {"Цибуля", "Овоч", "Білий", "40"},
    {"Грейпфрут", "Фрукт", "Рожевий", "42"},
    {"Мандарин", "Фрукт", "Оранжевий", "53"},
    {"Помідор", "Овоч", "Червоний", "27"},
    {"Кавун", "Фрукт", "Зелений", "30"},
    {"Патисон", "Овоч", "Білий", "18"},
    {"Брокколі", "Овоч", "Зелений", "34"},
    {"Айва", "Фрукт", "Жовто-зелений", "43"}
};

        for (int i = 0; i < items.GetLength(0); i++)
        {
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO fruits_and_vegetables (name, type, color, calorie) VALUES ($name, $type, $color, $calorie)";

            string name = items[i, 0];
            string type = items[i, 1];
            string color = items[i, 2];
            int calorie = int.Parse(items[i, 3]);

            insertCommand.Parameters.AddWithValue("$name", name);
            insertCommand.Parameters.AddWithValue("$type", type);
            insertCommand.Parameters.AddWithValue("$color", color);
            insertCommand.Parameters.AddWithValue("$calorie", calorie);

            insertCommand.ExecuteNonQuery();
        }

        transaction.Commit();

    }

    private static void ReadAndDisplayAll(SqliteConnection connection, string comm, string displayMessage)
    {
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = comm;

        using var reader = command.ExecuteReader();
        Console.WriteLine();
        Console.WriteLine(displayMessage);
        Console.WriteLine();

        while (reader.Read())
        {
            string output = "";
            for (int i = 0; i < reader.FieldCount; i++)
            {
                output += $"{reader.GetString(i),-15}\t";
            }
            Console.WriteLine(output);
        }
    }
}