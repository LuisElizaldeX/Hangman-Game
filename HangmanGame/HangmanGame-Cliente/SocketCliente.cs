using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame_Cliente
{
    public class SocketCliente
    {
        private TcpClient client;
        private NetworkStream stream;

        public async Task ConectarAsync(string ip, int port)
        {
            client = new TcpClient();
            await client.ConnectAsync(ip, port);
            stream = client.GetStream();
        }

        public async Task<string> SendMessageAsync(string message)
        {
            if (client == null || !client.Connected)
                throw new InvalidOperationException("No conectado al servidor.");

            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);

            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        public void Desconectar()
        {
            stream?.Close();
            client?.Close();
        }
    }
}
