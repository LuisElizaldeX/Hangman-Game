using Biblioteca.DTO;

namespace HangmanGame_Cliente.Application.Interfaces
{
    public interface IHangmanApiClient
    {
        ResponsePartidaDTO ObtenerEstadoPartida(
            string codigoPartida);

        ResponseResultadoDTO GuardarResultado(
            int idJugador,
            int idPartida,
            bool gano,
            bool esAbandono);
    }
}
