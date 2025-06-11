using Biblioteca.DTO;
using HangmanGame_Cliente.Utilidades;
using HangmanGame_Cliente.HangmanServicioReferencia;
using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;

namespace HangmanGame_Cliente.Cliente.Vistas
{
    /// <summary>
    /// Lógica de interacción para IniciarSesion.xaml
    /// </summary>
    public partial class IniciarSesion : Page
    {

        private HangmanServiceClient cliente;

        public IniciarSesion()
        {
            InitializeComponent();
            cliente = new HangmanServiceClient(); 
        }

        private void DarAltaUsuario(object sender, RoutedEventArgs e)
        {
            
        }

        private void InicioSesion(object sender, RoutedEventArgs e)
        {
            string correo = cuadroTextoCorreo.Text;
            string contrasena = cuadroContrasenaContrasena.Password;

            if (!Seguridad.EsCadenaVacia(correo) && !Seguridad.EsCadenaVacia(contrasena))
            {
                if (!ExistenDatosInvalidos(correo, contrasena))
                {
                    try
                    {
                        ResponseDTO response = cliente.Autenticacion(correo, contrasena);

                        if (response != null && response.success && response.data != null)
                        {
                            var jugadorDTO = response.data as JugadorDTO;
                            if (jugadorDTO != null)
                            {
                                var mainWindow = Window.GetWindow(this) as MainWindow;
                                if (mainWindow != null)
                                {
                                    mainWindow.SetJugadorAutenticado(jugadorDTO);
                                    var jugadorAutenticado = mainWindow.GetJugadorAutenticado();
                                    if (jugadorAutenticado != null)
                                    {

                                    }
                                    else
                                    {
                                        MessageBox.Show("Error: JugadorAutenticado es null después de setearlo.");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Error: No se encontró MainWindow.");
                                }
                                string mensaje = "Datos válidos. Jugador autenticado:\n" +
                                              $"Correo: {jugadorDTO.correo ?? "No disponible"}\n" +
                                              $"Usuario: {jugadorDTO.usuario ?? "No disponible"}\n" +
                                              $"Nombre: {jugadorDTO.nombre ?? "No disponible"}\n" +
                                              $"Fecha de Nacimiento: {(jugadorDTO.fecha_nacimiento != default(DateTime) ? jugadorDTO.fecha_nacimiento.ToString("yyyy-MM-dd") : "No disponible")}\n" +
                                              $"Teléfono: {jugadorDTO.telefono}\n" +
                                              $"Puntuación: {jugadorDTO.puntuacion}";
                                MessageBox.Show(mensaje);
                                if (mainWindow != null)
                                {
                                    mainWindow.CambiarPagina(new ListaPartidasDisponibles());
                                }
                            }
                            else
                            {
                                MessageBox.Show("Error: No se pudo convertir los datos del jugador.");
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Credenciales no encontradas o error: {response?.message ?? "Sin mensaje"}");
                        }
                    }
                    catch (CommunicationException ex)
                    {
                        MessageBox.Show($"Error de comunicación con el servidor: {ex.Message}. Verifica que el servidor esté en ejecución.");
                    }
                    catch (TimeoutException ex)
                    {
                        MessageBox.Show($"Tiempo de espera agotado: {ex.Message}. Verifica la conexión.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error inesperado: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Campos vacíos");
            }
        }

        private bool ExistenDatosInvalidos(string correo, string contrasena)
        {
            bool hayCamposInvalidos = false;

            if (Seguridad.ExistenCaracteresInvalidosParaContrasena(contrasena))
            {
                MessageBox.Show("Contraseña inválida"); // COLOCAR ALERTA
                hayCamposInvalidos = true;
            }

            if (!hayCamposInvalidos && Seguridad.
                ExistenCaracteresInvalidosParaCorreo(correo))
            {
                MessageBox.Show("Correo inválido"); // COLOCAR ALERTA
                hayCamposInvalidos = true;
            }

            return hayCamposInvalidos;
        }

    }
}
