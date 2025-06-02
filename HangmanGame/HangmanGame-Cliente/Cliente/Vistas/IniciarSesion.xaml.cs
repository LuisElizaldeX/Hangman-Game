using Biblioteca.DTO;
using HangmanGame_Cliente.Utilidades;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

namespace HangmanGame_Cliente.Cliente.Vistas
{
    /// <summary>
    /// Lógica de interacción para IniciarSesion.xaml
    /// </summary>
    public partial class IniciarSesion : Page
    {
        private SocketCliente socketCliente;
        public IniciarSesion()
        {
            InitializeComponent();
            socketCliente = new SocketCliente();
        }

        private void DarAltaUsuario(object sender, RoutedEventArgs e)
        {
            MainWindow.CambiarPagina(new FormularioUsuario());
        }

        private async void InicioSesion(object sender, RoutedEventArgs e)
        {
            string correo = cuadroTextoCorreo.Text;
            string contrasena = cuadroContrasenaContrasena.Password;

            if (!Seguridad.EsCadenaVacia(correo) &&
                !Seguridad.EsCadenaVacia(contrasena))
            {
                if(!ExistenDatosInvalidos(correo))
                {
                    try
                    {
                        await socketCliente.ConectarAsync("127.0.0.1", 8001);

                        var jugadorDTO = new JugadorDTO
                        {
                            correo = cuadroTextoCorreo.Text,
                            contrasñea = cuadroContrasenaContrasena.Password
                        };
                        string json = JsonConvert.SerializeObject(jugadorDTO);
                        string response = await socketCliente.SendMessageAsync(json);

                        var responseDTO = JsonConvert.DeserializeObject<ResponseDTO>(response);
                        if (responseDTO != null && responseDTO.success && responseDTO.data != null)
                        {
                            Singleton.Instancia.AutenticarJugador(responseDTO.data);
                            var jugadorAutenticado = Singleton.Instancia.JugadorAutenticado;
                            MainWindow.CambiarPagina(new ListaPartidasDisponibles());
                        }
                        else
                        {
                            MessageBox.Show("Credenciales no encontradas");
                        }
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.ConnectionRefused)
                        {
                            MessageBox.Show("Error: El servidor no está disponible. Por favor, verifica que esté en ejecución.");
                        }
                        else
                        {
                            MessageBox.Show($"Error de conexión: {ex.Message}. Intenta de nuevo.");
                        }
                    }
                    catch (IOException ex)
                    {
                        // Manejo para pérdida de conexión durante la comunicación
                        MessageBox.Show($"Error: Se perdió la conexión con el servidor. Detalle: {ex.Message}. Intenta de nuevo.");
                    }
                    catch (JsonException ex)
                    {
                        // Manejo para errores al deserializar la respuesta
                        MessageBox.Show($"Error al procesar la respuesta del servidor: {ex.Message}.");
                    }
                    catch (Exception ex)
                    {
                        // Manejo genérico para otros errores
                        MessageBox.Show($"Error inesperado: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Campos vacíos");
            }
        }

        private bool ExistenDatosInvalidos(string correo)
        {
            bool hayCamposInvalidos = false;

            /*
            if (Seguridad.ExistenCaracteresInvalidosParaContrasena(contrasena))
            {
                MessageBox.Show("Contraseña inválida");
                hayCamposInvalidos = true;
            }*/

            if (!hayCamposInvalidos && Seguridad.
                ExistenCaracteresInvalidosParaCorreo(correo))
            {
                MessageBox.Show("Correo inválido");
                hayCamposInvalidos = true;
            }

            return hayCamposInvalidos;
        }

    }
}
