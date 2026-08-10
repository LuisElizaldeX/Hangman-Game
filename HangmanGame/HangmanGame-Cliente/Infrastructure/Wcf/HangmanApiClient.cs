using Biblioteca.DTO;
using HangmanGame_Cliente.Application.Interfaces;
using HangmanGame_Cliente.HangmanServicioReferencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame_Cliente.Infrastructure.Wcf
{
    public class HangmanApiClient : IHangmanApiClient, IDisposable
    {
        private readonly HangmanServiceClient _client;

        public HangmanApiClient()
        {
            _client = new HangmanServiceClient();
        }

        public ResponsePartidaDTO ObtenerEstadoPartida(
            string codigoPartida)
        {
            return _client.ObtenerEstadoPartida(
                codigoPartida);
        }

        public ResponseResultadoDTO GuardarResultado(
            int idJugador,
            int idPartida,
            bool gano,
            bool esAbandono)
        {
            return _client.GuardarResultado(
                idJugador,
                idPartida,
                gano,
                esAbandono);
        }

        public void Dispose()
        {
            try
            {
                if (_client.State ==
                    System.ServiceModel.CommunicationState.Faulted)
                {
                    _client.Abort();
                }
                else
                {
                    _client.Close();
                }
            }
            catch
            {
                _client.Abort();
            }
        }
    }
}
