using Biblioteca.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame_Cliente.Application.Interfaces
{
    public interface ISessionService
    {
        JugadorDTO JugadorActual { get; }

        bool EstaAutenticado { get; }

        void IniciarSesion(JugadorDTO jugador);

        void CerrarSesion();
    }
}
