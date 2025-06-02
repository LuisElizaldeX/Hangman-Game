using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
{
    public class ResponseDTO
    {
        public bool success { get; set; }
        public string message { get; set; }
        public JugadorDTO data { get; set; }
    }
}
