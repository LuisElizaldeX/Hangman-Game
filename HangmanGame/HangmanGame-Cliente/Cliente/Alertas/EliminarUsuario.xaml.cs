using System.Windows;

namespace HangmanGame_Cliente.Cliente.Alertas
{

    public partial class EliminarUsuario : Window
    {
        public EliminarUsuario()
        {
            InitializeComponent();
        }

        private void btnAceptarEliminarUsuario_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCancelarEliminarUsuario_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
