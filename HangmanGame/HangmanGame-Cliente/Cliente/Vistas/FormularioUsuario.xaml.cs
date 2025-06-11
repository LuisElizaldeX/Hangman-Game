using HangmanGame_Cliente.Cliente.Alertas;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace HangmanGame_Cliente.Cliente.Vistas
{
    public partial class FormularioUsuario : Page
    {
        SolidColorBrush rojo = new SolidColorBrush(Colors.Red);
        SolidColorBrush transparente = new SolidColorBrush(Colors.Transparent);

        public FormularioUsuario()
        {
            InitializeComponent();
            cargarInformacion();
        }

        private void btnGuardarUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarCampos())
            {
                try
                {/*
                    var nuevoJugador = new JugadorDTO()
                    {
                        Usuario = txtUsuario.Text,
                        Nombre = txtNombreCompleto.Text,
                        FechaNacimiento = DateTime.ParseExact(txtFechaNacimiento.Text, "dd-MM-yyyy", null),
                        Telefono = int.Parse(txtTelefono.Text,),
                        Correo = txtCorreo.Text,
                        Contrasena = psBContrasenia.Password
                    };
                    
                    MostrarAlertaBloqueante(new CreacionUsuarioCorrecto());
                    NavigationService?.GoBack();*/
                }
                catch (FormatException ex)
                {
                    MostrarAlertaBloqueante(new SinConexionBaseDatos());
                    Console.WriteLine("Error: " + ex.Message);
                }
                catch (Exception ex)
                {
                    MostrarAlertaBloqueante(new SinConexionServidor());
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    NavigationService?.GoBack();
                }
            }
        }

        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void limpiarBordes()
        {
            txtUsuario.BorderBrush = transparente;
            txtNombreCompleto.BorderBrush = transparente;
            txtFechaNacimiento.BorderBrush = transparente;
            txtTelefono.BorderBrush = transparente;
            txtCorreo.BorderBrush = transparente;
            psBContrasenia.BorderBrush = transparente;
        }

        private bool ValidarCampos()
        {
            limpiarBordes();

            bool usuarioVacio = string.IsNullOrWhiteSpace(txtUsuario.Text);
            bool nombreVacio = string.IsNullOrWhiteSpace(txtNombreCompleto.Text);
            bool fechaVacia = string.IsNullOrWhiteSpace(txtFechaNacimiento.Text);
            bool telefonoVacio = string.IsNullOrWhiteSpace(txtTelefono.Text);
            bool correoVacio = string.IsNullOrWhiteSpace(txtCorreo.Text);
            bool contraseniaVacia = string.IsNullOrWhiteSpace(psBContrasenia.Password);

            List<bool> camposVacios = new List<bool>
            {
                usuarioVacio, nombreVacio, fechaVacia,
                telefonoVacio, correoVacio, contraseniaVacia
            };

            if (camposVacios.Contains(true))
            {
                if (usuarioVacio) txtUsuario.BorderBrush = rojo;
                if (nombreVacio) txtNombreCompleto.BorderBrush = rojo;
                if (fechaVacia) txtFechaNacimiento.BorderBrush = rojo;
                if (telefonoVacio) txtTelefono.BorderBrush = rojo;
                if (correoVacio) txtCorreo.BorderBrush = rojo;
                if (contraseniaVacia) psBContrasenia.BorderBrush = rojo;

                MostrarAlertaBloqueante(new CamposVacios());
                return false;
            }

            if (!Regex.IsMatch(txtUsuario.Text, @"^[a-zA-Z0-9]+$")) // Validar que no contenga caracteres especiales ni espacios
            {
                txtUsuario.BorderBrush = rojo;
                MostrarAlertaBloqueante(new CamposErroneos());
                return false;
            }

            if (!Regex.IsMatch(txtNombreCompleto.Text, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                txtNombreCompleto.BorderBrush = rojo;
                MostrarAlertaBloqueante(new CamposErroneos());
                return false;
            }

            if (!DateTime.TryParseExact(txtFechaNacimiento.Text, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out _)) // Validar que la fecha tenga el formato dd-MM-yyyy
            {
                txtFechaNacimiento.BorderBrush = rojo;
                MostrarAlertaBloqueante(new FechaNacimientoErroneo());
                return false;
            }

            if (!Regex.IsMatch(txtTelefono.Text, @"^\d{10}$")) // Validar que el teléfono tenga 10 dígitos
            {
                txtTelefono.BorderBrush = rojo;
                MostrarAlertaBloqueante(new CamposErroneos());
                return false;
            }

            if (!Regex.IsMatch(txtCorreo.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) //Validar que tengan la estructura sea algo@algo.algo, sin permitir espacios ni múltiples símbolos @.
            {
                txtCorreo.BorderBrush = rojo;
                MostrarAlertaBloqueante(new CorreoElectronicoErroneo());
                return false;
            }

            if (psBContrasenia.Password.Length < 8)
            {
                psBContrasenia.BorderBrush = rojo;
                MostrarAlertaBloqueante(new ContraseniaErronea());
                return false;
            }
            return true;
        }

        private void txtUsuarioChanged(object sender, TextChangedEventArgs e) //Valida que el nombre de usuario no exceda los 20 caracteres
        {
            if (txtUsuario.Text.Length > 20)
            {
                txtUsuario.Text = txtUsuario.Text.Substring(0, 20);
                txtUsuario.CaretIndex = txtUsuario.Text.Length;
            }
        }

        private void txtNombreCompletoChanged(object sender, TextChangedEventArgs e) {
            if (txtNombreCompleto.Text.Length > 28)
            {
                txtNombreCompleto.Text = txtNombreCompleto.Text.Substring(0, 28);
                txtNombreCompleto.CaretIndex = txtNombreCompleto.Text.Length;
            }
        }

        private void txtFechaNacimientoChanged(object sender, TextChangedEventArgs e) 
        {
            if (txtFechaNacimiento.Text.Length > 10)
            {
                txtFechaNacimiento.Text = txtFechaNacimiento.Text.Substring(0, 10);
                txtFechaNacimiento.CaretIndex = txtFechaNacimiento.Text.Length;
            }
        }

        private void txtTelefonoChanged(object sender, TextChangedEventArgs e) 
        {
            if (txtTelefono.Text.Length > 10)
            {
                txtTelefono.Text = txtTelefono.Text.Substring(0, 10);
                txtTelefono.CaretIndex = txtTelefono.Text.Length;
            }
        }

        private void txtCorreoChanged(object sender, TextChangedEventArgs e) 
        {
            if (txtCorreo.Text.Length > 28)
            {
                txtCorreo.Text = txtCorreo.Text.Substring(0, 28);
                txtCorreo.CaretIndex = txtCorreo.Text.Length;
            }
        }

        private void psBContraseniaChanged(object sender, RoutedEventArgs e)
        {
            if (psBContrasenia.Password.Length > 8)
            {
                psBContrasenia.Password = psBContrasenia.Password.Substring(0, 8);
            }
        }

        public void cargarInformacion()
        {/*
            txtUsuario.Text = activePlayer.Username;
            txtNombreCompleto.Text = activePlayer.Name;
            txtFechaNacimiento.Text = activePlayer.Birthday.ToString("dd-MM-yyyy");
            txtTelefono.Text = activePlayer.Phonenumber;
            txtCorreo.Text = activePlayer.Email;
            bloquearCORREO
            psBContrasenia.Password = activePlayer.Password;*/
        }

        private void MostrarAlertaBloqueante(Window alerta)
        {
            alerta.Owner = Window.GetWindow(this);
            alerta.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            alerta.ShowDialog();
        }
    }
}
