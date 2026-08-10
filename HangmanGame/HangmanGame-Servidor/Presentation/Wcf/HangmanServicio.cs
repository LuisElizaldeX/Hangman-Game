using Biblioteca.DTO;
using Biblioteca.Servicios;
using HangmanGame_Servidor.Application.UseCases.Autenticacion;
using HangmanGame_Servidor.Application.UseCases.Jugadores;
using HangmanGame_Servidor.Application.UseCases.Palabras;
using HangmanGame_Servidor.Application.UseCases.Partidas;
using HangmanGame_Servidor.Application.UseCases.Resultados;
using System.ServiceModel;

namespace HangmanGame_Servidor
{
    [ServiceBehavior(
    InstanceContextMode = InstanceContextMode.Single,
    ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class HangmanServicio : IHangmanService
    {
        private readonly AutenticarJugadorService _autenticarJugadorService;
        private readonly RegistrarJugadorService _registrarJugador;
        private readonly ActualizarJugadorService _actualizarJugador;
        private readonly CrearPartidaService _crearPartida;
        private readonly UnirsePartidaService _unirsePartida;
        private readonly CancelarPartidaService _cancelarPartida;
        private readonly ObtenerPartidasDisponiblesService _obtenerPartidas;
        private readonly ObtenerEstadoPartidaService _obtenerEstado;
        private readonly ObtenerPalabrasPorCategoriaService _obtenerPalabras;
        private readonly GuardarResultadosService _guardarResultado;
        private readonly ObtenerEstadisticasJugadorService _obtenerEstadisticas;

        public HangmanServicio( 
            AutenticarJugadorService autenticarJugadorService,
            RegistrarJugadorService registrarJugador,
            ActualizarJugadorService actualizarJugador,
            CrearPartidaService crearPartida,
            UnirsePartidaService unirsePartida,
            CancelarPartidaService cancelarPartida,
            ObtenerPartidasDisponiblesService obtenerPartidas,
            ObtenerEstadoPartidaService obtenerEstado,
            ObtenerPalabrasPorCategoriaService obtenerPalabras,
            GuardarResultadosService guardarResultado,
            ObtenerEstadisticasJugadorService obtenerEstadisticas)
        {
            _autenticarJugadorService = autenticarJugadorService;
            _registrarJugador = registrarJugador;
            _actualizarJugador = actualizarJugador;
            _crearPartida = crearPartida;
            _unirsePartida = unirsePartida;
            _cancelarPartida = cancelarPartida;
            _obtenerPartidas = obtenerPartidas;
            _obtenerEstado = obtenerEstado;
            _obtenerPalabras = obtenerPalabras;
            _guardarResultado = guardarResultado;
            _obtenerEstadisticas = obtenerEstadisticas;
        }

        public ResponseDTO Autenticacion(string correo, string contrasena)
        {
            return _autenticarJugadorService.Ejecutar(correo, contrasena);
        }

        public ResponsePalabrasConIdDTO ObtenerPalabrasPorCategoria(string categoria)
        {
            return _obtenerPalabras.Ejecutar(categoria);
        }

        public ResponsePartidaDTO CrearPartida(int idAdivinador, int idPalabra, string codigo, int idIdiomaPartida)
        {
            return _crearPartida
                .Ejecutar(idAdivinador, idPalabra, codigo, idIdiomaPartida);
        }

        public ResponsePartidaDTO ObtenerPartidasDisponibles()
        {
            return _obtenerPartidas.Ejecutar();
        }

        public ResponsePartidaDTO UnirsePartida(string codigo, int idJugador)
        {
            return _unirsePartida.Ejecutar(codigo, idJugador);
        }

        public ResponsePartidaDTO ObtenerEstadoPartida(string codigoPartida)
        {
            return _obtenerEstado.Ejecutar(codigoPartida);
        }

        public ResponseResultadoDTO GuardarResultado(int idJugador, int idPartida, bool gano, bool esAbandono = false)
        {
            return _guardarResultado.Ejecutar(idJugador, idPartida, gano, esAbandono);
        }

        public ResponseEstadisticaDTO ObtenerEstadisticasPorJugador(int idJugador)
        { 
            return _obtenerEstadisticas.Ejecutar(idJugador);
        }

        public ResponseDTO RegistrarJugador(JugadorDTO nuevoJugador)
        {
            return _registrarJugador.Ejecutar(nuevoJugador);
        }

        public ResponseDTO ActualizarJugador(JugadorDTO jugadorActualizado)
        {
            return _actualizarJugador.Ejecutar(jugadorActualizado);
        }

        public ResponsePartidaDTO CancelarPartida(string codigoPartida)
        {
            return _cancelarPartida.Ejecutar(codigoPartida);
        }
    }
}
