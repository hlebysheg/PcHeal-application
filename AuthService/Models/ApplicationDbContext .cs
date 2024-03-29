﻿using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace WordBook.Models
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {   // создаем базу данных при первом обращении
        }

        public DbSet<User> Student { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<PcStatistic> PcStatistic { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string adminRoleName = "admin";
            string userRoleName = "user";

            string adminEmail = "adminPdMail@mail.ru";
            string adminPassword = "123456";

			string testUserNamae = "string";
			string testPass = "string";

			// добавляем роли
			Role adminRole = new Role { Id = 1, Name = adminRoleName };
            Role userRole = new Role { Id = 2, Name = userRoleName };
            User adminUser = new User { Id = 1, Email = adminEmail, Password = adminPassword, RoleId = adminRole.Id, Name="Admin" };
			User testUser = new User { Id = 500, Email = testUserNamae, Password = testUserNamae, RoleId = adminRole.Id, Name = "string" };
			modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });
			modelBuilder.Entity<User>().HasData(new User[] { testUser });
			base.OnModelCreating(modelBuilder);
        }
    }
}
