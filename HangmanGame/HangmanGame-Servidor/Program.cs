using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HangmanGame_Servidor.Modelo;
using Newtonsoft.Json;
using Biblioteca.DTO;
using System.IO;
using System.Data.Entity;

namespace HangmanGame_Servidor
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 8001);
            server.Start();
            Console.WriteLine("Servidor iniciado. Escuchando en el puerto 8001...");

            try
            {
                while (true)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    Console.WriteLine("Cliente conectado desde {0}", client.Client.RemoteEndPoint);
                    _ = Task.Run(() => HandleClient(client)); // Manejar en un hilo separado
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fatal en el servidor: " + ex.Message);
            }
            finally
            {
                server.Stop();
                Console.WriteLine("Servidor detenido.");
            }
        }

        static async Task HandleClient(TcpClient client)
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    Console.WriteLine("Mensaje recibido: " + message);

                    if (string.IsNullOrEmpty(message))
                    {
                        Console.WriteLine("Mensaje vacío recibido. Ignorando.");
                        continue;
                    }

                    // Deserializar el mensaje
                    var jugadorDTO = JsonConvert.DeserializeObject<JugadorDTO>(message);
                    if (jugadorDTO == null)
                    {
                        Console.WriteLine("Error al deserializar el mensaje. Enviando error al cliente.");
                        string errorResponse = JsonConvert.SerializeObject(new { success = false, message = "Error al procesar los datos" });
                        await SendResponse(client, errorResponse);
                        continue;
                    }

                    string correo = jugadorDTO.correo;
                    string contrasena = jugadorDTO.contrasñea;

                    using (var context = new HangmanEntidades())
                    {
                        var jugador = await context.jugador
                            .FirstOrDefaultAsync(j => j.correo == correo && j.contrasena == contrasena);

                        if (jugador != null)
                        {
                            // Construir un nuevo JugadorDTO con los datos recuperados
                            var jugadorResponseDTO = new JugadorDTO
                            {
                                correo = jugador.correo,
                                Usuario = jugador.usuario,
                                nombre = jugador.nombre,
                                fecha_nacimiento = jugador.fecha_nacimiento ?? DateTime.MinValue, // Manejo de null
                                telefono = jugador.telefono ?? 0, // Manejo de null
                                puntuacion = jugador.puntuacion ?? 0 // Manejo de null
                            };

                            // Serializar y enviar respuesta
                            string responseJson = JsonConvert.SerializeObject(new { success = true, data = jugadorResponseDTO });
                            await SendResponse(client, responseJson);
                        }
                        else
                        {
                            string response = JsonConvert.SerializeObject(new { success = false, message = "NO SE ENCONTRO" });
                            await SendResponse(client, response);
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Error de socket: " + ex.Message);
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error de E/S: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inesperado: " + ex.Message);
            }
            finally
            {
                stream?.Dispose();
                client.Close();
                Console.WriteLine("Cliente desconectado.");
            }
        }

        static async Task SendResponse(TcpClient client, string response)
        {
            if (client?.Connected ?? false)
            {
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await client.GetStream().WriteAsync(responseBytes, 0, responseBytes.Length);
            }
        }
    }
}
