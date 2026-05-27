using Microsoft.EntityFrameworkCore;
using ServiceRequestSystem.Models;

namespace ServiceRequestSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ServiceObject> ServiceObjects { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<RequestStatus> RequestStatuses { get; set; }
        public DbSet<RequestPriority> RequestPriorities { get; set; }
        public DbSet<RequestComment> RequestComments { get; set; }
        public DbSet<RequestHistory> RequestHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Користувач має одну роль, роль може належати багатьом користувачам
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Об'єкт обслуговування має багато обладнання
            modelBuilder.Entity<Equipment>()
                .HasOne(e => e.ServiceObject)
                .WithMany(o => o.Equipment)
                .HasForeignKey(e => e.ServiceObjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Об'єкт обслуговування має багато заявок
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(r => r.ServiceObject)
                .WithMany(o => o.ServiceRequests)
                .HasForeignKey(r => r.ServiceObjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Користувач створює заявку
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(r => r.CreatedByUser)
                .WithMany()
                .HasForeignKey(r => r.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Інженер може бути призначений на заявку
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(r => r.AssignedEngineer)
                .WithMany()
                .HasForeignKey(r => r.AssignedEngineerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Статус заявки
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(r => r.RequestStatus)
                .WithMany(s => s.ServiceRequests)
                .HasForeignKey(r => r.RequestStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Пріоритет заявки
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(r => r.RequestPriority)
                .WithMany(p => p.ServiceRequests)
                .HasForeignKey(r => r.RequestPriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Коментарі до заявки
            modelBuilder.Entity<RequestComment>()
                .HasOne(c => c.ServiceRequest)
                .WithMany(r => r.Comments)
                .HasForeignKey(c => c.ServiceRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RequestComment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Історія змін заявки
            modelBuilder.Entity<RequestHistory>()
                .HasOne(h => h.ServiceRequest)
                .WithMany(r => r.History)
                .HasForeignKey(h => h.ServiceRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RequestHistory>()
                .HasOne(h => h.User)
                .WithMany()
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Початкові ролі користувачів
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Клієнт" },
                new Role { RoleId = 2, RoleName = "Оператор" },
                new Role { RoleId = 3, RoleName = "Інженер" },
                new Role { RoleId = 4, RoleName = "Адміністратор" },
                new Role { RoleId = 5, RoleName = "Керівник" }
            );

            // Початкові статуси заявок
            modelBuilder.Entity<RequestStatus>().HasData(
                new RequestStatus { RequestStatusId = 1, StatusName = "Нова" },
                new RequestStatus { RequestStatusId = 2, StatusName = "Зареєстрована" },
                new RequestStatus { RequestStatusId = 3, StatusName = "Призначена" },
                new RequestStatus { RequestStatusId = 4, StatusName = "У роботі" },
                new RequestStatus { RequestStatusId = 5, StatusName = "Очікує уточнення" },
                new RequestStatus { RequestStatusId = 6, StatusName = "Виконана" },
                new RequestStatus { RequestStatusId = 7, StatusName = "Закрита" },
                new RequestStatus { RequestStatusId = 8, StatusName = "Скасована" }
            );

            // Початкові пріоритети заявок
            modelBuilder.Entity<RequestPriority>().HasData(
                new RequestPriority { RequestPriorityId = 1, PriorityName = "Низький" },
                new RequestPriority { RequestPriorityId = 2, PriorityName = "Середній" },
                new RequestPriority { RequestPriorityId = 3, PriorityName = "Високий" },
                new RequestPriority { RequestPriorityId = 4, PriorityName = "Критичний" }
            );

            // Початкові користувачі системи
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    FullName = "Іваненко Олександр Петрович",
                    Email = "client@example.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEICIAjP9LYBhjQ1X7/U8am9gxBZJwwpMmBYkTgGx/by5uf2rWo2DzWDBiIPFLfjijA==",
                    Phone = "+380501112233",
                    RoleId = 1
                },
                new User
                {
                    UserId = 2,
                    FullName = "Петренко Андрій Сергійович",
                    Email = "operator@example.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEICIAjP9LYBhjQ1X7/U8am9gxBZJwwpMmBYkTgGx/by5uf2rWo2DzWDBiIPFLfjijA==",
                    Phone = "+380502223344",
                    RoleId = 2
                },
                new User
                {
                    UserId = 3,
                    FullName = "Коваленко Дмитро Іванович",
                    Email = "engineer@example.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEICIAjP9LYBhjQ1X7/U8am9gxBZJwwpMmBYkTgGx/by5uf2rWo2DzWDBiIPFLfjijA==",
                    Phone = "+380503334455",
                    RoleId = 3
                },
                new User
                {
                    UserId = 4,
                    FullName = "Мельник Олена Вікторівна",
                    Email = "admin@example.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEICIAjP9LYBhjQ1X7/U8am9gxBZJwwpMmBYkTgGx/by5uf2rWo2DzWDBiIPFLfjijA==",
                    Phone = "+380504445566",
                    RoleId = 4
                },
                new User
                {
                    UserId = 5,
                    FullName = "Савченко Ігор Миколайович",
                    Email = "manager@example.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEICIAjP9LYBhjQ1X7/U8am9gxBZJwwpMmBYkTgGx/by5uf2rWo2DzWDBiIPFLfjijA==",
                    Phone = "+380505556677",
                    RoleId = 5
                },
                new User
                {
                    UserId = 6,
                    FullName = "Сидоренко Марина Олегівна",
                    Email = "client2@example.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEICIAjP9LYBhjQ1X7/U8am9gxBZJwwpMmBYkTgGx/by5uf2rWo2DzWDBiIPFLfjijA==",
                    Phone = "+380506667788",
                    RoleId = 1
                },
                new User
                {
                    UserId = 7,
                    FullName = "Гончаренко Максим Андрійович",
                    Email = "client3@example.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEICIAjP9LYBhjQ1X7/U8am9gxBZJwwpMmBYkTgGx/by5uf2rWo2DzWDBiIPFLfjijA==",
                    Phone = "+380507778899",
                    RoleId = 1
                },
                new User
                {
                    UserId = 8,
                    FullName = "Шевченко Роман Володимирович",
                    Email = "engineer2@example.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEICIAjP9LYBhjQ1X7/U8am9gxBZJwwpMmBYkTgGx/by5uf2rWo2DzWDBiIPFLfjijA==",
                    Phone = "+380508889900",
                    RoleId = 3
                },
                new User
                {
                    UserId = 9,
                    FullName = "Бондар Назар Сергійович",
                    Email = "engineer3@example.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEICIAjP9LYBhjQ1X7/U8am9gxBZJwwpMmBYkTgGx/by5uf2rWo2DzWDBiIPFLfjijA==",
                    Phone = "+380509990011",
                    RoleId = 3
                }
            );

            // Початкові об'єкти обслуговування
            modelBuilder.Entity<ServiceObject>().HasData(
                new ServiceObject
                {
                    ServiceObjectId = 1,
                    ObjectName = "Офіс №1",
                    Address = "м. Київ, вул. Центральна, 10",
                    ContactPerson = "Іваненко Олександр",
                    ContactPhone = "+380501112233",
                    Description = "Офісне приміщення з IP-камерами відеоспостереження"
                },
                new ServiceObject
                {
                    ServiceObjectId = 2,
                    ObjectName = "Офіс №2",
                    Address = "м. Київ, вул. Робоча, 25",
                    ContactPerson = "Сидоренко Марина",
                    ContactPhone = "+380506667788",
                    Description = "Офісне приміщення з відеореєстратором та мережевими камерами"
                }
            );
        }
    }
}
