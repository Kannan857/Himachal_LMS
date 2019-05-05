using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMSCoreWebAPI.lms
{
    public partial class lmsContext : DbContext
    {
        public lmsContext()
        {
        }

        public lmsContext(DbContextOptions<lmsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Institution> Institution { get; set; }
        public virtual DbSet<Institutiondepartment> Institutiondepartment { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Userdepartment> Userdepartment { get; set; }
        public virtual DbSet<Userinstitution> Userinstitution { get; set; }
        public virtual DbSet<Userrole> Userrole { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL("server=localhost;port=3306;user=root;password=!Password1;database=lms");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("department", "lms");

                entity.HasIndex(e => e.ContactPerson)
                    .HasName("ContactPerson");

                entity.HasIndex(e => e.DepartmentAssistantHead)
                    .HasName("DepartmentAssistantHead");

                entity.HasIndex(e => e.DepartmentHead)
                    .HasName("DepartmentHead");

                entity.Property(e => e.DepartmentId).HasColumnType("int(11)");

                entity.Property(e => e.ContactNumber)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ContactPerson).HasColumnType("int(11)");

                entity.Property(e => e.DepartmentAssistantHead).HasColumnType("int(11)");

                entity.Property(e => e.DepartmentDescription)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.DepartmentHead).HasColumnType("int(11)");

                entity.Property(e => e.DepartmentName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ExtensionNumber)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.HasOne(d => d.ContactPersonNavigation)
                    .WithMany(p => p.DepartmentContactPersonNavigation)
                    .HasForeignKey(d => d.ContactPerson)
                    .HasConstraintName("department_ibfk_3");

                entity.HasOne(d => d.DepartmentAssistantHeadNavigation)
                    .WithMany(p => p.DepartmentDepartmentAssistantHeadNavigation)
                    .HasForeignKey(d => d.DepartmentAssistantHead)
                    .HasConstraintName("department_ibfk_2");

                entity.HasOne(d => d.DepartmentHeadNavigation)
                    .WithMany(p => p.DepartmentDepartmentHeadNavigation)
                    .HasForeignKey(d => d.DepartmentHead)
                    .HasConstraintName("department_ibfk_1");
            });

            modelBuilder.Entity<Institution>(entity =>
            {
                entity.ToTable("institution", "lms");

                entity.Property(e => e.InstitutionId).HasColumnType("int(11)");

                entity.Property(e => e.AddressLine1)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.AddressLine2)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.InstitutionName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.InstitutionUrl)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Website)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Institutiondepartment>(entity =>
            {
                entity.ToTable("institutiondepartment", "lms");

                entity.HasIndex(e => e.DepartmentId)
                    .HasName("DepartmentId");

                entity.HasIndex(e => e.InstitutionId)
                    .HasName("InstitutionId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.DepartmentId).HasColumnType("int(11)");

                entity.Property(e => e.InstitutionId).HasColumnType("int(11)");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Institutiondepartment)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("institutiondepartment_ibfk_2");

                entity.HasOne(d => d.Institution)
                    .WithMany(p => p.Institutiondepartment)
                    .HasForeignKey(d => d.InstitutionId)
                    .HasConstraintName("institutiondepartment_ibfk_1");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role", "lms");

                entity.Property(e => e.RoleId).HasColumnType("int(11)");

                entity.Property(e => e.RoleDescription)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.RoleName)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user", "lms");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.Property(e => e.CreatedBy).HasColumnType("int(11)");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.IsVerified)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.MobileNumber).HasColumnType("int(11)");

                entity.Property(e => e.ModifiedBy).HasColumnType("int(11)");

                entity.Property(e => e.PasswordHash).HasColumnType("blob");

                entity.Property(e => e.PasswordSalt).HasColumnType("blob");

                entity.Property(e => e.ResetPasswordToken).HasColumnType("char(38)");

                entity.Property(e => e.UniqueId)
                    .IsRequired()
                    .HasColumnType("char(38)");

                entity.Property(e => e.UserName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UserStatusId).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Userdepartment>(entity =>
            {
                entity.ToTable("userdepartment", "lms");

                entity.HasIndex(e => e.DepartmentId)
                    .HasName("DepartmentId");

                entity.HasIndex(e => e.UserId)
                    .HasName("UserId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.DepartmentId).HasColumnType("int(11)");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Userdepartment)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("userdepartment_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Userdepartment)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("userdepartment_ibfk_1");
            });

            modelBuilder.Entity<Userinstitution>(entity =>
            {
                entity.ToTable("userinstitution", "lms");

                entity.HasIndex(e => e.InstitutionId)
                    .HasName("InstitutionId");

                entity.HasIndex(e => e.UserId)
                    .HasName("UserId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.InstitutionId).HasColumnType("int(11)");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.Institution)
                    .WithMany(p => p.Userinstitution)
                    .HasForeignKey(d => d.InstitutionId)
                    .HasConstraintName("userinstitution_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Userinstitution)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("userinstitution_ibfk_1");
            });

            modelBuilder.Entity<Userrole>(entity =>
            {
                entity.ToTable("userrole", "lms");

                entity.HasIndex(e => e.RoleId)
                    .HasName("RoleId");

                entity.HasIndex(e => e.UserId)
                    .HasName("UserId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.RoleId).HasColumnType("int(11)");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Userrole)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("userrole_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Userrole)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("userrole_ibfk_1");
            });
        }
    }
}
