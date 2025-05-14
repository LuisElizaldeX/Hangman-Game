using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HangmanGame_Cliente.Cliente.Vistas
{
    public partial class IniciarSesion : Page
    {
        public event EventHandler<string> IdiomaSeleccionado;
        private string idioma;

        public IniciarSesion()
        {
            InitializeComponent();
            EstablecerIdioma("es");
            ActualizarTitulo();
        }

        private void btnCambiarIdioma_Click(object sender, RoutedEventArgs e)
        {
            var currentCulture = Thread.CurrentThread.CurrentUICulture.Name;
            var newCulture = currentCulture == "en" ? "es" : "en";
            EstablecerIdioma(newCulture);
            ActualizarTitulo();
            IdiomaSeleccionado?.Invoke(this, newCulture == "en" ? "Ingles" : "Spanish");
        }

        private void btnEntrar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRegistrarse_Click(object sender, RoutedEventArgs e)
        {
            var formularioUsuarioVista = new FormularioUsuario();
            NavigationService?.Navigate(formularioUsuarioVista);
        }
        private void EstablecerIdioma(string idioma)
        {
            try
            {
                var culture = new CultureInfo(idioma);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                Application.Current.Resources.MergedDictionaries.Clear();
                ResourceDictionary resourceDictionary = new ResourceDictionary
                {
                    Source = new Uri($"/Dictionary-{idioma}.xaml", UriKind.Relative)
                };
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);

                this.idioma = idioma;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cambiar el idioma: {ex.Message}");
            }
        }

        private void ActualizarTitulo()
        {
            bool isEnglish = Thread.CurrentThread.CurrentUICulture.Name == "en";
            imgTituloEN.Visibility = isEnglish ? Visibility.Visible : Visibility.Collapsed;
            imgTituloES.Visibility = isEnglish ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
