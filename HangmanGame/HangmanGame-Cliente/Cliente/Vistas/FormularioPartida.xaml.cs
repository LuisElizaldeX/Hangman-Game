using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// <summary>
    /// Lógica de interacción para FormularioPartida.xaml
    /// </summary>
    public partial class FormularioPartida : Page
    {
        public FormularioPartida()
        {
            InitializeComponent();
        }

        private void Modificar(object sender, RoutedEventArgs e)
        {
            // Cierra la ventana actual
            Window.GetWindow(this)?.Close();
        }

        private void btnCancelarCreacionPartida_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void btnCategoriaMusica_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCategoriaSeries_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCategoriaPeliculas_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
