using Microsoft.EntityFrameworkCore;
using SCCGasso.Core.Domain.Entities;
using System.Reflection.Emit;


namespace SCCGasso.Infrastructure.Persistence.Context
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        public DbSet<Formulario> Formularios { get; set; }
        public DbSet<PersonasAutorizadas> PersonasAutorizadas { get; set; }
        public DbSet<ReferenciasComerciales> ReferenciasComercilaes { get; set; }
        public DbSet<Sugerencias> Sugerencias { get; set; }
        public DbSet<CuentasBancarias> CuentasBancarias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Formulario

            modelBuilder.Entity<Formulario>()
                .ToTable("Formularios");

            modelBuilder.Entity<Formulario>()
                .HasKey(f => f.Id);

            //modelBuilder.Entity<Formulario>()
            //    .HasMany(a => a.CuentasBancarias)
            //    .WithOne()
            //    .HasForeignKey(a => a.IdFormulario)
            //    .OnDelete(DeleteBehavior.Cascade)
            //    .HasConstraintName("fk_CuentasBancarias_Formulario");

            //modelBuilder.Entity<Formulario>()
            //    .HasMany(a => a.PersonasAutorizadas)
            //    .WithOne()
            //    .HasForeignKey(a => a.IdFormulario)
            //    .OnDelete(DeleteBehavior.Cascade)
            //    .HasConstraintName("fk_PersonaAutorizadas_Formulario");

            //modelBuilder.Entity<Formulario>()
            //    .HasMany(a => a.ReferenciasComercilaes)
            //    .WithOne()
            //    .HasForeignKey(a => a.IdFormulario)
            //    .OnDelete(DeleteBehavior.Cascade)
            //    .HasConstraintName("fk_ReferenciasComerciales_Formulario");

            modelBuilder.Entity<Formulario>()
                .Property(a => a.CapitalPagado)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Formulario>()
                .Property(a => a.MontoSolicitadoGG)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Formulario>()
                .Property(a => a.MontoSolicitadoIQ)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Formulario>()
                .Property(a => a.MontoAprobadoGG)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Formulario>()
                .Property(a => a.MontoAprobadoIQ)
                .HasColumnType("decimal(18,2)");

            #endregion

            #region Personas Autorizadas

            modelBuilder.Entity<PersonasAutorizadas>()
                .ToTable("PersonasAutorizadas");

            modelBuilder.Entity<PersonasAutorizadas>()
                .HasKey(asg => asg.Id);

            modelBuilder.Entity<PersonasAutorizadas>()
                .HasOne(a => a.Formulario)
                .WithMany()
                .HasForeignKey(a => a.IdFormulario)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_Formulario_PersonasAutorizadas");

            #endregion

            #region Referencias Comerciales

            modelBuilder.Entity<ReferenciasComerciales>()
                .ToTable("ReferenciasComerciales");

            modelBuilder.Entity<ReferenciasComerciales>()
                .HasKey(asg => asg.Id);

            modelBuilder.Entity<ReferenciasComerciales>()
                .HasOne(a => a.Formulario)
                .WithMany()
                .HasForeignKey(a => a.IdFormulario)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_Formulario_ReferenciasComerciales");

            #endregion

            #region Cuentas Bancarias

            modelBuilder.Entity<CuentasBancarias>()
                .ToTable("CuentasBancarias");

            modelBuilder.Entity<CuentasBancarias>()
                .HasKey(asg => asg.Id);

            modelBuilder.Entity<CuentasBancarias>()
                .HasOne(a => a.Formulario)
                .WithMany()
                .HasForeignKey(a => a.IdFormulario)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_Formulario_CuentasBancarias");

            #endregion

            #region Sugerencias 

            modelBuilder.Entity<Sugerencias>()
                .ToTable("Sugerencias");

            modelBuilder.Entity<Sugerencias>()
                .HasKey(asg => asg.Id);

            #endregion
        }
    }
}