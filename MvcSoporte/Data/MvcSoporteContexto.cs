using Microsoft.EntityFrameworkCore;
using MvcSoporte.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcSoporte.Data
{
    public class MvcSoporteContexto : DbContext
    {
        public MvcSoporteContexto(DbContextOptions<MvcSoporteContexto> options)
        : base(options)
        {
        }
        public DbSet<Aviso> Avisos { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Localizacion> Localizaciones { get; set; }
        public DbSet<TipoAveria> TipoAverias { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Poner el nombre de las tablas en singular
            modelBuilder.Entity<Aviso>().ToTable("Aviso");
            modelBuilder.Entity<Empleado>().ToTable("Empleado");
            modelBuilder.Entity<Equipo>().ToTable("Equipo");
            modelBuilder.Entity<Localizacion>().ToTable("Localizacion");
            modelBuilder.Entity<TipoAveria>().ToTable("TipoAveria");
            // Deshabilitar la eliminación en cascada en todas las relaciones
            base.OnModelCreating(modelBuilder);
            foreach (var relationship in
            modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
