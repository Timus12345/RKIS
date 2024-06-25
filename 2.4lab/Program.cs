using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace _2._4;
public class Task
{
    public int id { get; set; }
    public int personId { get; set; }
    public string name { get; set; } 
    public string description { get; set; }
    public DateTime date { get; set; }
    public virtual Person idPersonNavigator { get; set; }
}
public class Person
{
    public int id { get; set; }
    public string login { get; set; }
    public string password { get; set; }
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}

public class program
{
    static void Main(string[] args)
    {
        runUser();
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
        using (context db = new context())
        {
            Console.WriteLine("\nВыберите пользователя: ");
            Console.WriteLine("0: Регистрация");

            var persons = db.Persons.ToList();
            for (int i = 0; i < persons.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {persons[i].login}");
            }
            
            int choicePerson = int.Parse(Console.ReadLine());
            
            if (choicePerson >= 1)
            {
                var person = persons[choicePerson - 1];
                Console.WriteLine($"\nВведите пароль для: {person.login}");
                string password = Console.ReadLine();

                if (person.password == password)
                {
                    Console.WriteLine($"\nДобро пожаловать {person.login}!");
                    taskSelect(person.id);
                }
                else
                {
                    Console.WriteLine("Неверный пароль!");
                }
            }
            else
            {
                registr();
            }
        }
    }

    static void registr()
    {
        context db = new context(); 
        var persons = db.Persons.ToList();
        Console.Write("\nВведите логин: ");
        string login = Console.ReadLine();
        Console.Write("Введите пароль: ");
        string password = Console.ReadLine();
        
        Person newPerson = new Person()
        {
            id = persons.Count + 1,
            login = login,
            password = password
        };
        
        db.Persons.Add(newPerson);
        db.SaveChanges();
        Console.WriteLine("\nРегистрация завершена!");
        runUser();
    }

    static void taskSelect(int personId)
    {
        Console.WriteLine("\n1 - Добавить задачу\n2 - Удалить задачу\n3 - Редактировать задачу\n4 - Просмотр задач\n5 - Сменить пользователя");
        int select = int.Parse(Console.ReadLine());
    
        switch (select)
        {
            case 1:
                addTask(personId);
                break;
            case 2:
                deleteTask(personId);
                break;
            case 3:
                editTask(personId);
                break;
            case 4:
                viewTask(personId);
                break;
            default:
                runUser();
                break;
        }
    } 
    
    static void addTask(int personId)
    {
        context db = new context(); 
        Console.Write("\nНазвание: ");
        string name = Console.ReadLine();
        Console.Write("Описание: ");
        string description = Console.ReadLine();
        Console.Write("Дата: ");
        DateTime date = DateTime.Parse(Console.ReadLine());
        
        Task newTask = new Task()
        {
            personId = personId,
            name = name,
            description = description,
            date = date
        };
        
        db.Tasks.Add(newTask);
        db.SaveChanges();
        Console.Write($"\nЗадача \"{name}\" добавлена!\n");
        taskSelect(personId);
    }
    
    static void deleteTask(int personId)
    {
        context db = new context();
        Console.WriteLine();
        var tasks = db.Tasks.Include(r => r.idPersonNavigator).Where(r => r.idPersonNavigator.id == personId).ToList();
        
        foreach (var task in tasks)
        {
            Console.WriteLine($"{task.id}: {task.name}");
        }

        Console.Write("\nId задачи для удаления: ");
        int choice = int.Parse(Console.ReadLine());
        var taskRemove = tasks.FirstOrDefault(t => t.id == choice); 
        Console.Write($"\nЗадача {taskRemove.name} удалена\n"); 
        db.Tasks.Remove(taskRemove); 
        db.SaveChanges();
        taskSelect(personId);
    }
    
    static void editTask(int personId)
    {
        context db = new context();
        Console.WriteLine();
        var tasks = db.Tasks.Include(r => r.idPersonNavigator).Where(r => r.idPersonNavigator.id == personId).ToList();
        
        foreach (var task in tasks)
        {
            Console.WriteLine($"{task.id}: {task.name}");
        }

        Console.Write("\nId задачи для редактирования: ");
        int choice = int.Parse(Console.ReadLine());
        
        Console.Write("\nНазвание: ");
        string name = Console.ReadLine();
        Console.Write("Описание: ");
        string description = Console.ReadLine();
        Console.Write("Дата: ");
        DateTime date = DateTime.Parse(Console.ReadLine());
        var updateTask = tasks.FirstOrDefault(t => t.id == choice);
        updateTask.name = name;
        updateTask.description = description;
        updateTask.date = date;
        db.SaveChanges();
        Console.Write($"\nЗадача {updateTask.name} обновлена!\n");
        taskSelect(personId);
    }

    static void viewTask(int personId)
    {
        context db = new context();
        Console.WriteLine("\n1 - на сегодня\n2 - на завтра\n3 - на неделю\n4 - выполнено\n5 - предстоящие\n6 - все задачи");
        int choice = int.Parse(Console.ReadLine());
        
        switch (choice)
        {
            case 1: 
                foreach (var task in db.Tasks.Include(r => r.idPersonNavigator).Where(r => r.idPersonNavigator.id == personId && r.date.Date == DateTime.Today))
                {
                    Console.WriteLine($"\nЗадача: {task.name}\nОписание: {task.description}\nДата: {task.date}"); Console.Write("-------------------------"); 
                } 
                Console.WriteLine(); 
                taskSelect(personId); 
                break;
            case 2: 
                foreach (var task in db.Tasks.Include(r => r.idPersonNavigator).Where(r => r.idPersonNavigator.id == personId && r.date.Date == DateTime.Today.AddDays(1)))
                {
                    Console.WriteLine($"\nЗадача: {task.name}\nОписание: {task.description}\nДата: {task.date}"); Console.Write("-------------------------"); 
                } 
                Console.WriteLine(); 
                taskSelect(personId); 
                break;
            case 3:
                foreach (var task in db.Tasks.Include(r => r.idPersonNavigator).Where(r => r.idPersonNavigator.id == personId && r.date.Date >= DateTime.Today && r.date.Date <= DateTime.Today.AddDays(7)))
                {
                    Console.WriteLine($"\nЗадача: {task.name}\nОписание: {task.description}\nДата: {task.date}"); Console.Write("-------------------------"); 
                } 
                Console.WriteLine(); 
                taskSelect(personId); 
                break;
            case 4:
                foreach (var task in db.Tasks.Include(r => r.idPersonNavigator).Where(r => r.idPersonNavigator.id == personId && r.date.Date < DateTime.Today))
                {
                    Console.WriteLine($"\nЗадача: {task.name}\nОписание: {task.description}\nДата: {task.date}"); Console.Write("-------------------------"); 
                } 
                Console.WriteLine(); 
                taskSelect(personId);
                break;
            case 5:
                foreach (var task in db.Tasks.Include(r => r.idPersonNavigator).Where(r => r.idPersonNavigator.id == personId && r.date.Date > DateTime.Today))
                {
                    Console.WriteLine($"\nЗадача: {task.name}\nОписание: {task.description}\nДата: {task.date}"); Console.Write("-------------------------"); 
                } 
                Console.WriteLine(); 
                taskSelect(personId); 
                break;
            case 6: 
                foreach (var task in db.Tasks.Include(r => r.idPersonNavigator).Where(r => r.idPersonNavigator.id == personId))
                {
                    Console.WriteLine($"\nЗадача: {task.name}\nОписание: {task.description}\nДата: {task.date}");
                    Console.Write("-------------------------");
                }
                Console.WriteLine();
                taskSelect(personId);
                break;
        }
    }
}   
public partial class context : DbContext
{
    static context()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
    public context()
    {
    }

    public context(DbContextOptions<context> options) : base(options)
    {
    }

    public virtual DbSet<Person> Persons { get; set; }
    public virtual DbSet<Task> Tasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=1234");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("person");
            entity.Property(e => e.id).HasColumnName("id");
            entity.Property(e => e.login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.password)
                .HasMaxLength(50)
                .HasColumnName("password");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.ToTable("task");
            entity.Property(e => e.id).HasColumnName("id");
            entity.Property(e => e.personId).HasColumnName("personid");
            entity.Property(e => e.name).HasColumnName("name");
            entity.Property(e => e.description).HasColumnName("description");
            entity.Property(e => e.date).HasColumnName("date");
            
            entity.HasOne(t => t.idPersonNavigator).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.personId)
                .HasConstraintName("task_personid_fkey");
        });
        OnModelCreatingPartial(modelBuilder);
    }
    
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}