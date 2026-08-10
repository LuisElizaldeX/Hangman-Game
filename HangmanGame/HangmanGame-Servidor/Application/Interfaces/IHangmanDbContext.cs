using HangmanGame_Servidor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HangmanGame_Servidor.Application.Interfaces
{
    public interface IHangmanDbContext : IDisposable
    {
        DbSet<Jugador> Jugadores { get; set; }
        DbSet<Partida> Partidas { get; set; }
        DbSet<Palabra> Palabras { get; set; }
        DbSet<Categoria> Categorias { get; set; }
        DbSet<Resultado> Resultados { get; set; }
        DbSet<EstadoPartida> EstadosPartida { get; set; }
        DbSet<IdiomaPartida> IdiomasPartida { get; set; }

        int SaveChanges();

        IHangmanTransaction BeginTransaction(
           IsolationLevel isolationLevel);

        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken);
    }
}
