using HangmanGame_Cliente.Cliente.Vistas;
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

namespace HangmanGame_Cliente
{
    public partial class MainWindow : Window
    {
        private static Page PaginaActual { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            PaginaActual = new IniciarSesion();
            MarcoPaginaActual.Navigate(PaginaActual);
        }

        public static void CambiarPagina(Page nuevaPagina)
        {
            MainWindow ventanaPrincipal = ObtenerVentanaActual();
            PaginaActual = nuevaPagina;
            ventanaPrincipal?.MarcoPaginaActual.Navigate(nuevaPagina);
        }

        public static MainWindow ObtenerVentanaActual()
        {
            return (MainWindow)GetWindow(PaginaActual);
        }
    }
}
