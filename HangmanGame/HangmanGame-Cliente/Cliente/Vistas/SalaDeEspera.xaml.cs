using Biblioteca.DTO;
using HangmanGame_Cliente.HangmanServicioReferencia;
using HangmanGame_Cliente.Utilidades;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace HangmanGame_Cliente.Cliente.Vistas
{
    /// <summary>
    /// Lógica de interacción para SalaDeEspera.xaml
    /// </summary>
    public partial class SalaDeEspera : Page
    {
        private HangmanServiceClient cliente;
        private SocketCliente socketCliente;
        private string codigoPartida;
        JugadorDTO jugadorDTO;
        private bool isListening;
        private bool isInitialized;
        private int idJugadorActual;

        public SalaDeEspera(string codigoPartida)
        {
            InitializeComponent();
            this.codigoPartida = codigoPartida;
            lbCodigo.Content = codigoPartida;
            cliente = new HangmanServiceClient();
            socketCliente = new SocketCliente();
            isListening = true;
            Loaded += SalaDeEspera_Loaded;
        }

        private void SalaDeEspera_Loaded(object sender, RoutedEventArgs e)
        {
            if (isInitialized) return;
            isInitialized = true;

            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow == null)
            {
                MessageBox.Show("Error: No se encontró MainWindow.");
                return;
            }
            jugadorDTO = mainWindow.GetJugadorAutenticado();
            if (jugadorDTO == null)
            {
                MessageBox.Show("Error: No se encontró el jugador autenticado. MainWindow HashCode: " + mainWindow.GetHashCode());
                mainWindow.CambiarPagina(new IniciarSesion());
                return;
            }
            idJugadorActual = jugadorDTO.id_jugador;

            Task.Run(async () =>
            {
                try
                {
                    await socketCliente.ConectarAsync("127.0.0.1", 12345);
                    await socketCliente.SendMessageAsync($"REGISTRO_PARTIDA:{codigoPartida}");
                    socketCliente.MessageReceived += OnMessageReceived;
                    socketCliente.ConnectionLost += OnConnectionLost;
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Error al conectar con el servidor de sockets: {ex.Message}");
                        var mw = Window.GetWindow(this) as MainWindow;
                        mw?.CambiarPagina(new ListaPartidasDisponibles());
                    });
                }
            });

            VerificarEstadoPartida();
        }

        private void OnMessageReceived(string message)
        {
            Dispatcher.Invoke(() =>
            {
                if (message.StartsWith("UNION_RETADOR:"))
                {
                    string nicknameRetador = message.Substring("UNION_RETADOR:".Length);
                    var estadoPartida = cliente.ObtenerEstadoPartida(codigoPartida);
                    bool esAnfitrion = idJugadorActual == estadoPartida.partida.IdAdivinador;
                    if (esAnfitrion)
                    {
                        tbAvisoPartida.Text = $"Estás por comenzar una partida con \n{nicknameRetador}, ¿estás listo?";
                        lbRetador.Content = $"¡{nicknameRetador} se ha unido a la partida!";
                        btnComenzar.Visibility = Visibility.Visible;
                        lbAvisoPartida.Visibility = Visibility.Visible;
                        lbRetador.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        string nicknameAnfitrion = estadoPartida.partida.Nickname;
                        tbAvisoPartida.Text = $"Estás por comenzar una partida con \n{nicknameAnfitrion}, ¿estás listo?";
                        lbRetador.Content = $"¡Te has unido a la sala, pronto comenzará!";
                        lbRetador.Visibility = Visibility.Visible;
                        lbAvisoPartida.Visibility = Visibility.Visible;
                        btnComenzar.Visibility = Visibility.Hidden;
                    }
                }
                else if (message.StartsWith("INICIAR_PARTIDA:"))
                {
                    var partes = message.Split(':');
                    if (partes.Length >= 2 && partes[1] == codigoPartida)
                    {
                        isListening = false;
                        socketCliente.MessageReceived -= OnMessageReceived;

                        // Cerrar recursos antes de navegar
                        socketCliente.Desconectar();
                        try { cliente.Close(); } catch { cliente.Abort(); }

                        // Intentar obtener la ventana principal
                        var mainWindow = Application.Current.MainWindow as MainWindow;
                        if (mainWindow != null)
                        {
                            mainWindow.CambiarPagina(new PartidaAhorcado(codigoPartida));
                        }
                        else
                        {
                            MessageBox.Show($"Error: MainWindow es null en el retador ID {idJugadorActual}. Application.Current.MainWindow: {Application.Current.MainWindow}");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Código recibido {partes[1]} no coincide con {codigoPartida}");
                    }
                }
                else if (message.StartsWith("PARTIDA_CANCELADA:"))
                {
                    var partes = message.Split(':');
                    if (partes.Length >= 2 && partes[1] == codigoPartida)
                    {
                        MessageBox.Show($"PARTIDA_CANCELADA recibido para ID {idJugadorActual} con código {codigoPartida}");
                        isListening = false;
                        socketCliente.MessageReceived -= OnMessageReceived;

                        socketCliente.Desconectar();
                        try { cliente.Close(); } catch { cliente.Abort(); }

                        // Intentar obtener la ventana principal
                        var mainWindow = Application.Current.MainWindow as MainWindow;
                        if (mainWindow != null)
                        {
                            mainWindow.CambiarPagina(new ListaPartidasDisponibles());
                            // COLOCAR ALERTA DE PARTIDA CANCELADA SI ES QUE LA HAY
                        }
                        else
                        {
                            MessageBox.Show($"Error: MainWindow es null en el retador ID {idJugadorActual}. Application.Current.MainWindow: {Application.Current.MainWindow}");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Código recibido {partes[1]} no coincide con {codigoPartida}");
                    }
                }
                else
                {
                    MessageBox.Show($"Mensaje no reconocido: {message}");
                }
            });
        }

        private void VerificarEstadoPartida()
        {
            try
            {
                ResponsePartidaDTO response = cliente.ObtenerEstadoPartida(codigoPartida);

                if (response != null && response.success && response.partida != null)
                {
                    var partida = response.partida;
                    bool esAnfitrion = idJugadorActual == partida.IdAdivinador;
                    //MessageBox.Show($"Verificación inicial - Es anfitrión: {esAnfitrion}, ID Adivinador: {partida.IdAdivinador}, Tu ID: {jugadorDTO.id_jugador}");
                    if (partida.IdEstadoPartida == 7)
                    {
                        if (esAnfitrion)
                        {
                            tbAvisoPartida.Text = $"Estás por comenzar una partida con \n{partida.NicknameRetador}, ¿estás listo?";
                            lbRetador.Content = $"¡{partida.NicknameRetador} se ha unido a la partida!";
                            
                        }
                        else
                        {
                            tbAvisoPartida.Text = $"Estás por comenzar una partida con \n{partida.Nickname}, ¿estás listo?";
                            lbRetador.Content = $"¡Te has unido a la sala jeje, pronto comenzará!";
                            lbRetador.Visibility = Visibility.Visible;
                            lbAvisoPartida.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        if (esAnfitrion)
                        {
                            tbAvisoPartida.Text = "Esperando a que un retador se una...";
                        }
                        else
                        {
                            tbAvisoPartida.Text = $"Esperando a que {partida.Nickname} comience la partida...";
                        }
                        lbAvisoPartida.Visibility = Visibility.Visible;
                        lbRetador.Visibility = Visibility.Hidden;
                        btnComenzar.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    MessageBox.Show($"Error al verificar el estado: {response?.message ?? "Sin mensaje"}");
                    var mainWindow = Window.GetWindow(this) as MainWindow;
                    mainWindow?.CambiarPagina(new ListaPartidasDisponibles());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al verificar el estado: {ex.Message}");
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.CambiarPagina(new ListaPartidasDisponibles());
            }
        }

        private async void CancelarCreacionPartida(object sender, RoutedEventArgs e)
        {
            try
            {
                string mensaje = $"PARTIDA_CANCELADA:{codigoPartida}";
                //MessageBox.Show("Vamos a enviar mensaje de cancelacion");
                await socketCliente.SendMessageAsync(mensaje);
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cancelar: {ex.Message}");
            }
        }

        private async void IniciarPartida(object sender, RoutedEventArgs e)
        {
            try
            {
                isListening = false; // Desuscribirse de mensajes
                //MessageBox.Show("Me estoy desuscribiendo, soy: " + jugadorDTO.id_jugador);
                socketCliente.MessageReceived -= OnMessageReceived;
                //MessageBox.Show("Ya me desuscribí");
                await socketCliente.SendMessageAsync($"INICIAR_PARTIDA:{codigoPartida}");
                //MessageBox.Show($"Mensaje INICIAR_PARTIDA enviado. Socket conectado: {socketCliente.IsConnected}");

                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.CambiarPagina(new PartidaAhorcado(codigoPartida));
                }
                else
                {
                    MessageBox.Show("Error: MainWindow es null en el anfitrión");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar la partida: {ex.Message}");
            }
        }

        private void OnConnectionLost(string message)
        {
            Dispatcher.Invoke(() =>
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.HandleConnectionLost(message ?? "Se ha perdido la conexión con el servidor.");
            });
        }

    }
}
