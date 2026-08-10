using Biblioteca.Servicios;
using HangmanGame_Servidor.Application.UseCases.Autenticacion;
using HangmanGame_Servidor.Application.UseCases.Jugadores;
using HangmanGame_Servidor.Application.UseCases.Palabras;
using HangmanGame_Servidor.Application.UseCases.Partidas;
using HangmanGame_Servidor.Application.UseCases.Resultados;
using HangmanGame_Servidor.Infrastructure.Sockets;
using HangmanGame_Servidor.ModeloPostgreSQL;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;

namespace HangmanGame_Servidor
{
    internal class Program
    {
        private static SocketServidor socketServidor;

        private static IConfigurationRoot Configuration;

        static async Task Main(string[] args)
        {
            ServiceHost host = null;
            Task socketTask = null;

            try
            {
                Configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile(
                        "appsettings.json",
                        optional: false,
                        reloadOnChange: false)
                    .Build();

                string connectionString =
                    Configuration.GetConnectionString("HangmanPostgreSQL");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException(
                        "No se encontró la cadena de conexión " +
                        "'HangmanPostgreSQL' en appsettings.json.");
                }

                var contextFactory =
                    new HangmanDbContextFactory(connectionString);

                if (!ProbarConexion(connectionString))
                {
                    Console.WriteLine(
                        "El servidor no se iniciará porque PostgreSQL no está disponible.");

                    return;
                }

                /*
                 * Infraestructura de sockets.
                 * Solo se crea aquí; se inicia después de abrir WCF.
                 */
                socketServidor = new SocketServidor(12345);

                var socketNotifier =
                    new SocketNotifier(socketServidor);

                /*
                 * Casos de uso.
                 */
                var autenticarJugadorService =
                    new AutenticarJugadorService(contextFactory);

                var registrarJugadorService =
                    new RegistrarJugadorService(contextFactory);

                var actualizarJugadorService =
                    new ActualizarJugadorService(contextFactory);

                var crearPartidaService =
                    new CrearPartidaService(contextFactory);

                var unirsePartidaService =
                    new UnirsePartidaService(
                        contextFactory,
                        socketNotifier);

                var cancelarPartidaService =
                    new CancelarPartidaService(contextFactory);

                var obtenerPartidasDisponiblesService =
                    new ObtenerPartidasDisponiblesService(contextFactory);

                var obtenerEstadoPartidaService =
                    new ObtenerEstadoPartidaService(contextFactory);

                var obtenerPalabrasPorCategoriaService =
                    new ObtenerPalabrasPorCategoriaService(contextFactory);

                var guardarResultadosService =
                    new GuardarResultadosService(contextFactory);

                var obtenerEstadisticasJugadorService =
                    new ObtenerEstadisticasJugadorService(contextFactory);

                /*
                 * Adaptador WCF.
                 */
                var servicio = new HangmanServicio(
                    autenticarJugadorService,
                    registrarJugadorService,
                    actualizarJugadorService,
                    crearPartidaService,
                    unirsePartidaService,
                    cancelarPartidaService,
                    obtenerPartidasDisponiblesService,
                    obtenerEstadoPartidaService,
                    obtenerPalabrasPorCategoriaService,
                    guardarResultadosService,
                    obtenerEstadisticasJugadorService);

                host = new ServiceHost(servicio);

                var binding = new NetTcpBinding
                {
                    Security =
            {
                Mode = SecurityMode.None
            }
                };

                host.AddServiceEndpoint(
                    typeof(IHangmanService),
                    binding,
                    "net.tcp://localhost:8001/HangmanService");

                var metadataBehavior =
                    host.Description.Behaviors
                        .Find<ServiceMetadataBehavior>();

                if (metadataBehavior == null)
                {
                    metadataBehavior = new ServiceMetadataBehavior
                    {
                        HttpGetEnabled = false,
                        MetadataExporter =
                {
                    PolicyVersion = PolicyVersion.Policy15
                }
                    };

                    host.Description.Behaviors.Add(metadataBehavior);
                }

                host.AddServiceEndpoint(
                    ServiceMetadataBehavior.MexContractName,
                    MetadataExchangeBindings.CreateMexTcpBinding(),
                    "net.tcp://localhost:8001/HangmanService/mex");

                host.Open();

                Console.WriteLine(
                    "Servicio WCF iniciado en " +
                    "net.tcp://localhost:8001/HangmanService");

                Console.WriteLine(
                    "Metadatos disponibles en " +
                    "net.tcp://localhost:8001/HangmanService/mex");

                /*
                 * Se inicia la misma instancia que utiliza SocketNotifier.
                 */
                socketTask = socketServidor.StartAsync();

                Console.WriteLine(
                    "Presiona cualquier tecla para detener el servidor...");

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al iniciar el servidor:");
                Console.WriteLine(ex);
            }
            finally
            {
                if (socketServidor != null)
                {
                    try
                    {
                        socketServidor.Stop();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"Error al detener el socket: {ex.Message}");
                    }
                }

                if (socketTask != null)
                {
                    try
                    {
                        await socketTask;
                    }
                    catch (ObjectDisposedException)
                    {
                        // Cierre normal del listener.
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine(
                            $"El servidor de sockets terminó: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"Error en la tarea del socket: {ex}");
                    }
                }

                CerrarHost(host);

                Console.WriteLine("Servicio WCF detenido.");
                Console.WriteLine("Servidor de sockets detenido.");
            }
        }


        /*Configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: false)
            .Build();

        string connectionString =
            Configuration.GetConnectionString("HangmanPostgreSQL");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "No se encontró la cadena de conexión " +
                "'HangmanPostgreSQL' en appsettings.json.");
        }

        var contextFactory = new HangmanDbContextFactory(connectionString);

        if (!ProbarConexion(connectionString))
        {
            Console.WriteLine(
                "El servidor no se iniciará porque la base de datos no está disponible.");

            return;
        }


        // Iniciar el servidor de sockets en un puerto diferente (12345)
        socketServidor = new SocketServidor(12345);
        Task socketTask = socketServidor.StartAsync();

        // Iniciar el host de WCF
        using (ServiceHost host = new ServiceHost(typeof(HangmanServicio)))
        {
            // Configurar el endpoint principal
            host.AddServiceEndpoint(
                typeof(IHangmanService),
                new NetTcpBinding { Security = { Mode = SecurityMode.None } },
                "net.tcp://localhost:8001/HangmanService");

            // Habilitar metadatos
            ServiceMetadataBehavior metadataBehavior = new ServiceMetadataBehavior();
            metadataBehavior.HttpGetEnabled = false; // No usamos HTTP
            metadataBehavior.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            host.Description.Behaviors.Add(metadataBehavior);

            // Agregar un endpoint para metadatos (usando mex)
            host.AddServiceEndpoint(
                ServiceMetadataBehavior.MexContractName,
                MetadataExchangeBindings.CreateMexTcpBinding(),
                "net.tcp://localhost:8001/HangmanService/mex");

            try
            {
                host.Open();
                Console.WriteLine("Servicio WCF iniciado en net.tcp://localhost:8001/HangmanService");
                Console.WriteLine("Metadatos disponibles en net.tcp://localhost:8001/HangmanService/mex");
                Console.WriteLine("Servidor de sockets iniciado en puerto 12345");
                Console.WriteLine("Presiona cualquier tecla para detener el servidor...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al iniciar el servicio: {ex.Message}");
            }
            finally
            {
                // Detener el servidor de sockets y cerrar el host de WCF
                socketServidor.Stop();
                host.Close();
                Console.WriteLine("Servicio WCF detenido.");
                Console.WriteLine("Servidor de sockets detenido.");
                await socketTask;
            }
        }
    }*/

        // Método estático para que HangmanServicio acceda al servidor de sockets
        public static async Task NotificarUnionRetador(string codigoPartida, string nicknameRetador)
        {
            await socketServidor.NotificarUnionRetador(codigoPartida, nicknameRetador);
        }

        private static bool ProbarConexion(string connectionString)
        {
            try
            {
                using (var context = new HangmanDbContext(connectionString))
                {
                    int cantidadCategorias = context.Categorias.Count();

                    Console.WriteLine("Conexión PostgreSQL exitosa.");
                    Console.WriteLine(
                        $"Categorías registradas: {cantidadCategorias}");

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("No fue posible conectar con PostgreSQL:");
                Console.WriteLine(ex.ToString()); 

                return false;
            }
        }

        private static void CerrarHost(ServiceHost host)
        {
            if (host == null)
            {
                return;
            }

            try
            {
                if (host.State == CommunicationState.Faulted)
                {
                    host.Abort();
                }
                else
                {
                    host.Close();
                }
            }
            catch
            {
                host.Abort();
            }
        }
    }
}