using HangmanGame_Cliente.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame_Cliente.Presentation.ViewModels
{
    public class PartidaAhorcadoViewModel : ViewModelBase
    {
        private readonly IHangmanApiClient _apiClient;
        private readonly ISocketClient _socketClient;
        private readonly ISessionService _sessionService;

        public string CodigoPartida { get; }
        private string _pista;
        private string _palabraMostrada;
        private int _intentosFallidos;
        private bool _esAnfitrion;

        public string Pista
        {
            get => _pista;
            private set => SetProperty(ref _pista, value);
        }

        public string PalabraMostrada
        {
            get => _palabraMostrada;
            private set => SetProperty(ref _palabraMostrada, value);
        }

        public int IntentosFallidos
        {
            get => _intentosFallidos;
            private set => SetProperty(ref _intentosFallidos, value);
        }

        public bool EsAnfitrion
        {
            get => _esAnfitrion;
            private set => SetProperty(ref _esAnfitrion, value);
        }

        public PartidaAhorcadoViewModel(
            string codigoPartida,
            IHangmanApiClient apiClient,
            ISocketClient socketClient,
            ISessionService sessionService)
        {
            CodigoPartida = codigoPartida;
            _apiClient = apiClient;
            _socketClient = socketClient;
            _sessionService = sessionService;
        }

        public async Task InicializarAsync()
        {
            var response =
                _apiClient.ObtenerEstadoPartida(
                    CodigoPartida);

            if (response == null ||
                !response.success ||
                response.partida == null)
            {
                throw new InvalidOperationException(
                    response?.message ??
                    "No fue posible obtener la partida.");
            }

            var jugador =
                _sessionService.JugadorActual;

            if (jugador == null)
            {
                throw new InvalidOperationException(
                    "No hay un jugador autenticado en la sesión.");
            }

            EsAnfitrion =
                jugador.IdJugador ==
                response.partida.IdAdivinador;

            Pista =
                IdiomaHelper.IdiomaActual == "es"
                    ? response.partida.Pista
                    : response.partida.PistaEn;

            PalabraMostrada =
                CrearPalabraOculta(
                    response.partida.Palabra);

            await _socketClient.ConectarAsync(
                "127.0.0.1",
                12345);

            await _socketClient.EnviarAsync(
                $"REGISTRO_PARTIDA:{CodigoPartida}");
        }

        private static string CrearPalabraOculta(string palabra)
        {
            if (string.IsNullOrWhiteSpace(palabra))
            {
                return string.Empty;
            }

            return string.Join(
                " ",
                palabra.Select(c =>
                    char.IsWhiteSpace(c)
                        ? " "
                        : "_"));
        }
    }
}
