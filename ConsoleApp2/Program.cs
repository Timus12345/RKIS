using Npgsql;

public class program
{
    public static class DatabaseService
    {
        private static NpgsqlConnection? _connection;

        private static string GetConnectionString()
        {
            return @"Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=1234";
        }

        public static NpgsqlConnection GetSqlConnection()
        {
            if (_connection is null)
            {
                _connection = new NpgsqlConnection(GetConnectionString());
                _connection.Open();
            }
            return _connection;
        }
    }
    static void runUser()
    {
        int choice;
        
        do
        {
            Console.WriteLine("\n1 - Вход\n2 - Регистрация\n0 - Выход");
            choice = int.Parse(Console.ReadLine());

            if (choice == 1)
            {
                login();
            }
            else if (choice == 2)
            {
                registr();
            }
        } while (choice != 0);
    }
    
    static void login()
    {
        Console.WriteLine("\nВыберите пользователя: ");
        Console.WriteLine("0: Регистрация");
        printPerson();
        int choice = int.Parse(Console.ReadLine());
        
        if (choice > 0)
        {
            personChoice(choice);
        }
        else
        {
            registr();
        }
    }
    
    static void registr()
    {
        Console.Write("\nВведите логин: ");
        string login = Console.ReadLine();
        Console.Write("Введите пароль: ");
        string password = Console.ReadLine();
        
        var querySql = $"INSERT INTO person(login, password) VALUES ('{login}', '{password}')";
        using (var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection()))
        {
            cmd.ExecuteNonQuery();
        }
        Console.WriteLine("\nРегистрация завершена!");
        runUser();
    }
    
    public static void printPerson()
    {
        var querySql = "SELECT * FROM person";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        using var user = cmd.ExecuteReader();

        while (user.Read())
        {
            Console.WriteLine($"{user[0]}: {user[1]}");
        }
    }

    public static void personChoice(int choice)
    {
        var querySql = $"SELECT * FROM person WHERE id = {choice};";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        using var user = cmd.ExecuteReader();

        while (user.Read())
        {
            Console.Write($"\nВведите пароль для {user[1]}: ");
            string password = Console.ReadLine();

            if ((string?)user[2] == password)
            {
                Console.WriteLine($"\nДобро пожаловать {user[1]}!");
                DatabaseService.GetSqlConnection().Close();
                taskSelect();
            }
            else
            {2
                Console.WriteLine("Неверный пароль! Пожалуйста попробуйте ещё раз.");
            }
        }
    }
    static void taskSelect()
    {
        DatabaseService.GetSqlConnection().Close();
        Console.WriteLine("\n1 - Добавить задачу\n2 - Удалить задачу\n3 - Редактировать задачу\n4 - Просмотр задач\n5 - Сменить пользователя");
        int select = int.Parse(Console.ReadLine());

        switch (select)
        {
            case 1:
                DatabaseService.GetSqlConnection().Close();
                addTask();
                break;
            case 2:
                DatabaseService.GetSqlConnection().Close();
                deleteTask();
                break;
            case 3:
                DatabaseService.GetSqlConnection().Close();
                editTask();
                break;
            case 4:
                DatabaseService.GetSqlConnection().Close();
                viewTask();
                break;
            default:
                runUser();
                break;
        }
    }

    static void addTask()
    {
        DatabaseService.GetSqlConnection().Open();
        Console.Write("\nНазвание: ");
        string name = Console.ReadLine();
        Console.Write("Описание: ");
        string description = Console.ReadLine();
        Console.Write("Дата окончания: ");
        string date = Console.ReadLine();
        
        var querySql = $"INSERT INTO task(name, description, date) VALUES ('{name}', '{description}', '{date}')";
        using (var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection()))
        {
            cmd.ExecuteNonQuery();
        }
        
        Console.Write($"\nЗадача \"{name}\" добавлена!\n");
        taskSelect();
    }
    
    static void deleteTask()
    {
        DatabaseService.GetSqlConnection().Open();
        Console.Write("\nНомер задачи: ");
        int choice = int.Parse(Console.ReadLine());
        var querySql = $"DELETE FROM task WHERE id = {choice};";
        using (var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection()))
        {
            cmd.ExecuteNonQuery();
        }
        Console.Write("\nЗадача удалена!\n");
        taskSelect();
    }
    
    static void editTask()
    {
        DatabaseService.GetSqlConnection().Open();
        Console.Write("\nНомер задачи: ");
        int choice = int.Parse(Console.ReadLine());
        Console.Write("Название: ");
        string newName = Console.ReadLine();
        Console.Write("Описание: ");
        string newDescription = Console.ReadLine();
        Console.Write("Дата окончания: ");
        DateOnly newDate = DateOnly.Parse(Console.ReadLine());
        var querySql = $"UPDATE task SET name = '{newName}', description = '{newDescription}', date = '{newDate}' WHERE id = {choice};";
        using (var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection()))
        {
            cmd.ExecuteNonQuery();
        }
        Console.Write($"\nЗадача {newName} обновленна!\n");
        taskSelect();
    }
    
    static void viewTask()
    {
        DatabaseService.GetSqlConnection().Open();
        Console.WriteLine("\n1 - На сегодня\n2 - На завтра\n3 - На неделю\n4 - Выполнено\n5 - Предстоит выполнить\n6 - Все задачи");
        int choice = int.Parse(Console.ReadLine());
        
        switch (choice)
        {
            case 1:
                var today = DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                var querySql = $"SELECT * FROM task WHERE date = '{today}';";
                using (var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection()))
                {
                    using var viewTask = cmd.ExecuteReader();
                    Console.WriteLine();

                    while (viewTask.Read())
                    {
                        Console.WriteLine(
                            $"Id: {viewTask[0]} Название: {viewTask[1]} Описание: {viewTask[2]} Дата окончания: {viewTask[3]}");
                    }
                } 
                taskSelect(); 
                break;
            case 2:
                var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1)).ToString("yyyy-MM-dd");
                querySql = $"SELECT * FROM task WHERE date = '{tomorrow}';";
                using (var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection()))
                {
                    using var viewTask = cmd.ExecuteReader();
                    Console.WriteLine();

                    while (viewTask.Read())
                    {
                        Console.WriteLine(
                            $"Id: {viewTask[0]} Название: {viewTask[1]} Описание: {viewTask[2]} Срок: {viewTask[3]}");
                    }
                } 
                taskSelect();
                break;
            case 3:
                DateOnly now = DateOnly.FromDateTime(DateTime.Today);
                string startDate = now.ToString("yyyy-MM-dd");
                var week = now.AddDays(7 - (int)now.DayOfWeek + 1).ToString("yyyy-MM-dd");
                querySql = $"SELECT * FROM task WHERE date BETWEEN '{startDate}' AND '{week}';";
                using (var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection()))
                {
                    using var viewTask = cmd.ExecuteReader();
                    Console.WriteLine();

                    while (viewTask.Read())
                    {
                        Console.WriteLine(
                            $"Id: {viewTask[0]} Название: {viewTask[1]} Описание: {viewTask[2]} Срок: {viewTask[3]}");
                    }
                } 
                taskSelect();
                break;
            case 4:
                today = DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                querySql = $"SELECT * FROM task WHERE date < '{today}';";
                using (var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection()))
                {
                    using var viewTask = cmd.ExecuteReader();
                    Console.WriteLine();

                    while (viewTask.Read())
                    {
                        Console.WriteLine(
                            $"Id: {viewTask[0]} Название: {viewTask[1]} Описание: {viewTask[2]} Срок: {viewTask[3]}");
                    }
                } 
                taskSelect();
                break;
            case 5:
                today = DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                querySql = $"SELECT * FROM task WHERE date > '{today}';";
                using (var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection()))
                {
                    using var viewTask = cmd.ExecuteReader();
                    Console.WriteLine();

                    while (viewTask.Read())
                    {
                        Console.WriteLine(
                            $"Id: {viewTask[0]} Название: {viewTask[1]} Описание: {viewTask[2]} Срок: {viewTask[3]}");
                    }
                } 
                taskSelect();
                break;
            case 6:
                querySql = "SELECT * FROM task";
                using (var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection()))
                {
                    using var viewTask = cmd.ExecuteReader();
                    Console.WriteLine();

                    while (viewTask.Read())
                    {
                        Console.WriteLine(
                            $"Id: {viewTask[0]} Название: {viewTask[1]} Описание: {viewTask[2]} Срок: {viewTask[3]}");
                    }
                } 
                taskSelect();
                break;
        }
    }

    static void Main(string[] args)
    {
        runUser();
    }
}