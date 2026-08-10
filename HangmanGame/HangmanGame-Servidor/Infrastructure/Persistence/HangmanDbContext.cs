using HangmanGame_Servidor.Application.Interfaces;
using HangmanGame_Servidor.Domain.Entities;
using HangmanGame_Servidor.Infrastructure.Persistence;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Data.Entity;

namespace HangmanGame_Servidor.ModeloPostgreSQL
{
    [DbConfigurationType(typeof(NpgsqlEfConfiguration))]
    public class HangmanDbContext : DbContext, IHangmanDbContext
    {
        public HangmanDbContext(DbConnection connection)
           : base(connection, contextOwnsConnection: true)
        {
            Database.SetInitializer<HangmanDbContext>(null);
        }

        public IHangmanTransaction BeginTransaction(
            IsolationLevel isolationLevel)
        {
            return new HangmanTransaction(
                Database.BeginTransaction(isolationLevel));
        }

        public HangmanDbContext(string connectionString)
            : this(new NpgsqlConnection(connectionString))
        {
        }

        public virtual DbSet<Categoria> Categorias { get; set; }
        public virtual DbSet<EstadoPartida> EstadosPartida { get; set; }
        public virtual DbSet<IdiomaPartida> IdiomasPartida { get; set; }
        public virtual DbSet<Jugador> Jugadores { get; set; }
        public virtual DbSet<Palabra> Palabras { get; set; }
        public virtual DbSet<Partida> Partidas { get; set; }
        public virtual DbSet<Resultado> Resultados { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            ConfigurarCategoria(modelBuilder);
            ConfigurarPalabra(modelBuilder);
            ConfigurarJugador(modelBuilder);
            ConfigurarEstadoPartida(modelBuilder);
            ConfigurarIdiomaPartida(modelBuilder);
            ConfigurarPartida(modelBuilder);
            ConfigurarResultado(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void ConfigurarCategoria(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categoria>()
                .ToTable("category");

            modelBuilder.Entity<Categoria>()
                .HasKey(x => x.IdCategoria);

            modelBuilder.Entity<Categoria>()
                .Property(x => x.IdCategoria)
                .HasColumnName("id_category");

            modelBuilder.Entity<Categoria>()
                .Property(x => x.Nombre)
                .HasColumnName("name")
                .HasMaxLength(45)
                .IsRequired();

            modelBuilder.Entity<Categoria>()
                .Property(x => x.NombreEn)
                .HasColumnName("name_en")
                .HasMaxLength(45)
                .IsRequired();
        }

        private static void ConfigurarPalabra(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Palabra>()
                .ToTable("word");

            modelBuilder.Entity<Palabra>()
                .HasKey(x => x.IdPalabra);

            modelBuilder.Entity<Palabra>()
                .Property(x => x.IdPalabra)
                .HasColumnName("id_word");

            modelBuilder.Entity<Palabra>()
                .Property(x => x.Nombre)
                .HasColumnName("name");

            modelBuilder.Entity<Palabra>()
                .Property(x => x.NombreEn)
                .HasColumnName("name_en");

            modelBuilder.Entity<Palabra>()
                .Property(x => x.Pista)
                .HasColumnName("hint");

            modelBuilder.Entity<Palabra>()
                .Property(x => x.Descripcion)
                .HasColumnName("description");

            modelBuilder.Entity<Palabra>()
                .Property(x => x.DescripcionEn)
                .HasColumnName("description_en");

            modelBuilder.Entity<Palabra>()
                .Property(x => x.IdCategoria)
                .HasColumnName("id_category");

            modelBuilder.Entity<Palabra>()
                .HasRequired(x => x.Categoria)
                .WithMany(x => x.Palabras)
                .HasForeignKey(x => x.IdCategoria)
                .WillCascadeOnDelete(false);
        }

        private static void ConfigurarJugador(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Jugador>()
                .ToTable("player");

            modelBuilder.Entity<Jugador>()
                .HasKey(x => x.IdJugador);

            modelBuilder.Entity<Jugador>()
                .Property(x => x.IdJugador)
                .HasColumnName("id_player");

            modelBuilder.Entity<Jugador>()
                .Property(x => x.Usuario)
                .HasColumnName("username");

            modelBuilder.Entity<Jugador>()
                .Property(x => x.Nombre)
                .HasColumnName("name");

            modelBuilder.Entity<Jugador>()
                .Property(x => x.FechaNacimiento)
                .HasColumnName("birthdate");

            modelBuilder.Entity<Jugador>()
                .Property(x => x.Telefono)
                .HasColumnName("phonenumber");

            modelBuilder.Entity<Jugador>()
                .Property(x => x.Correo)
                .HasColumnName("email");

            modelBuilder.Entity<Jugador>()
                .Property(x => x.Contrasena)
                .HasColumnName("password");

            modelBuilder.Entity<Jugador>()
                .Property(x => x.Puntuacion)
                .HasColumnName("score");
        }

        private static void ConfigurarEstadoPartida(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EstadoPartida>()
                .ToTable("match_status");

            modelBuilder.Entity<EstadoPartida>()
                .HasKey(x => x.IdEstadoPartida);

            modelBuilder.Entity<EstadoPartida>()
                .Property(x => x.IdEstadoPartida)
                .HasColumnName("id_match_status");

            modelBuilder.Entity<EstadoPartida>()
                .Property(x => x.Nombre)
                .HasColumnName("name");
        }

        private static void ConfigurarIdiomaPartida(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdiomaPartida>()
                .ToTable("match_language");

            modelBuilder.Entity<IdiomaPartida>()
                .HasKey(x => x.IdIdiomaPartida);

            modelBuilder.Entity<IdiomaPartida>()
                .Property(x => x.IdIdiomaPartida)
                .HasColumnName("id_match_language");

            modelBuilder.Entity<IdiomaPartida>()
                .Property(x => x.Nombre)
                .HasColumnName("name")
                .HasMaxLength(10)
                .IsRequired();
        }

        private static void ConfigurarPartida(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Partida>()
                .ToTable("gamematch");

            modelBuilder.Entity<Partida>()
                .HasKey(x => x.IdPartida);

            modelBuilder.Entity<Partida>()
                .Property(x => x.IdPartida)
                .HasColumnName("id_gamematch");

            modelBuilder.Entity<Partida>()
                .Property(x => x.Codigo)
                .HasColumnName("code");

            modelBuilder.Entity<Partida>()
                .Property(x => x.IdPalabra)
                .HasColumnName("id_word");

            modelBuilder.Entity<Partida>()
                .Property(x => x.IdRetador)
                .HasColumnName("id_challenger");

            modelBuilder.Entity<Partida>()
                .Property(x => x.IdAdivinador)
                .HasColumnName("id_guesser");

            modelBuilder.Entity<Partida>()
                .Property(x => x.IdEstadoPartida)
                .HasColumnName("id_match_status");

            modelBuilder.Entity<Partida>()
                .Property(x => x.IdIdiomaPartida)
                .HasColumnName("id_match_language");

            modelBuilder.Entity<Partida>()
                .Property(x => x.FechaCreacion)
                .HasColumnName("fecha_creacion");

            modelBuilder.Entity<Partida>()
                .HasRequired(x => x.Palabra)
                .WithMany(x => x.Partidas)
                .HasForeignKey(x => x.IdPalabra)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Partida>()
                .HasOptional(x => x.Retador)
                .WithMany(x => x.PartidasComoRetador)
                .HasForeignKey(x => x.IdRetador)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Partida>()
                .HasOptional(x => x.Adivinador)
                .WithMany(x => x.PartidasComoAdivinador)
                .HasForeignKey(x => x.IdAdivinador)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Partida>()
                .HasRequired(x => x.EstadoPartida)
                .WithMany(x => x.Partidas)
                .HasForeignKey(x => x.IdEstadoPartida)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Partida>()
                .HasRequired(x => x.IdiomaPartida)
                .WithMany(x => x.Partidas)
                .HasForeignKey(x => x.IdIdiomaPartida)
                .WillCascadeOnDelete(false);
        }

        private static void ConfigurarResultado(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resultado>()
                .ToTable("match_result");

            modelBuilder.Entity<Resultado>()
                .HasKey(x => x.IdResultado);

            modelBuilder.Entity<Resultado>()
                .Property(x => x.IdResultado)
                .HasColumnName("id_match_result");

            modelBuilder.Entity<Resultado>()
                .Property(x => x.IdJugador)
                .HasColumnName("id_player");

            modelBuilder.Entity<Resultado>()
                .Property(x => x.IdPartida)
                .HasColumnName("id_gamematch");

            modelBuilder.Entity<Resultado>()
                .Property(x => x.Gano)
                .HasColumnName("won");

            modelBuilder.Entity<Resultado>()
                .Property(x => x.Puntos)
                .HasColumnName("points");

            modelBuilder.Entity<Resultado>()
                .HasRequired(x => x.Jugador)
                .WithMany(x => x.Resultados)
                .HasForeignKey(x => x.IdJugador)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Resultado>()
                .HasRequired(x => x.Partida)
                .WithMany(x => x.Resultados)
                .HasForeignKey(x => x.IdPartida)
                .WillCascadeOnDelete(false);
        }
    }
}
