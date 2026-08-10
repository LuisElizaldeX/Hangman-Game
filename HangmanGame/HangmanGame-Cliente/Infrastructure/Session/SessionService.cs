using Biblioteca.DTO;
using HangmanGame_Cliente.Application.Interfaces;
using System;

namespace HangmanGame_Cliente.Infrastructure.Session
{
    public class SessionService : ISessionService
    {
        public JugadorDTO JugadorActual { get; private set; }

        public bool EstaAutenticado =>
            JugadorActual != null;

        public void IniciarSesion(JugadorDTO jugador)
        {
            JugadorActual = jugador
                ?? throw new ArgumentNullException(nameof(jugador));
        }

        public void CerrarSesion()
        {
            JugadorActual = null;
        }
    }
}
