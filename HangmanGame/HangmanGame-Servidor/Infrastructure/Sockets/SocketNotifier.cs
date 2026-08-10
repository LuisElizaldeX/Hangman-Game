using HangmanGame_Servidor.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace HangmanGame_Servidor.Infrastructure.Sockets
{
    public class SocketNotifier : ISocketNotifier
    {
        private readonly SocketServidor _socketServidor;

        public SocketNotifier(
            SocketServidor socketServidor)
        {
            _socketServidor = socketServidor;
        }

        public Task NotificarUnionRetadorAsync(
            string codigoPartida,
            string nicknameRetador)
        {
            return _socketServidor.NotificarUnionRetador(
                 codigoPartida,
                 nicknameRetador);
        }
    }
}
