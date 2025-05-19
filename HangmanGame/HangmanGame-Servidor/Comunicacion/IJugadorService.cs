using HangmanGame_Servidor.Dominio;
using System.ServiceModel;
using System.Threading.Tasks;

namespace HangmanGame_Servidor.Comunicacion
{
    [ServiceContract]
    public interface IJugadorService
    {
        [OperationContract]
        Task<bool> RegistrarJugador(JugadorDTO nuevoJugador);
    }
}
