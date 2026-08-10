using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame_Servidor.Application.Interfaces
{
    public interface ISocketNotifier
    {
        Task NotificarUnionRetadorAsync(
           string codigoPartida,
           string nicknameRetador);
    }
}
