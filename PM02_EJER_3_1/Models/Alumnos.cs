using System;
using System.Collections.Generic;
using System.Text;

namespace PM02_EJER_3_1.Models
{
    public class Alumnos
    {
        public int Id { get; set; }

        public string Nombres { get; set; }

        public string Apellidos { get; set; }

        public string Sexo { get; set; }

        public string Direccion { get; set; }

        public byte[] Fotografia { get; set; }

    }
}
