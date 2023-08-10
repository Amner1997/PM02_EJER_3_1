using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using PM02_EJER_3_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM02_EJER_3_1.Services
{
    public class FirebaseConnection
    {
        public static FirebaseClient conexionFirebase = new FirebaseClient("https://pm2examen2doparcial-default-rtdb.firebaseio.com");

        //Guardar Alumnos
        public async Task<bool> SaveSitios(Alumnos alumnos)
        {
            var data = await conexionFirebase.Child(nameof(Alumnos)).PostAsync(JsonConvert.SerializeObject(alumnos));
            if (!string.IsNullOrEmpty(data.Key))
            {
                return true;
            }
            return false;
        }

        public async Task<string> AgregarAlumnos(Alumnos parametros)
        {
            var data = await conexionFirebase
                  .Child("Alumnos")
                  .PostAsync(new Alumnos()
                  {
                      Id = parametros.Id,
                      Nombres = parametros.Nombres,
                      Apellidos = parametros.Apellidos,
                      Sexo = parametros.Sexo,
                      Direccion = parametros.Direccion,
                      Fotografia = parametros.Fotografia

                  });
            return data.Key;
        }

        //Listar sitios
        public async Task<List<Alumnos>> GetAllAlumnos()
        {
            return (await conexionFirebase.Child(nameof(Alumnos)).OnceAsync<Alumnos>()).Select(item => new Alumnos
            {

                Nombres = item.Object.Nombres,
                Apellidos = item.Object.Apellidos,
                Sexo = item.Object.Sexo,
                Direccion = item.Object.Direccion,
                Fotografia = item.Object.Fotografia,
                Id = item.Object.Id

            }).ToList();
        }

        //Obtener ID
        public async Task<Alumnos> GetById(string id)
        {
            return (await conexionFirebase.Child(nameof(Alumnos) + "/" + id).OnceSingleAsync<Alumnos>());

        }


        public async Task EditarAlumno(Alumnos parametros)
        {
            var data = (await conexionFirebase
                 .Child("Alumnos")
                 .OnceAsync<Alumnos>()).Where(a => a.Object.Id == parametros.Id).FirstOrDefault();

            if (data != null)
            {
                data.Object.Nombres = parametros.Nombres;
                data.Object.Apellidos = parametros.Apellidos;
                data.Object.Sexo = parametros.Sexo;
                data.Object.Direccion = parametros.Direccion;
                data.Object.Fotografia = parametros.Fotografia;

                await conexionFirebase
                    .Child("Alumnos")
                    .Child(data.Key)
                    .PutAsync(data.Object);
            }
        }



        //Eliminar sitio
        public async Task<bool> DeleteSitios(string Id)
        {
            //await conexionFirebase.Child(nameof(Sitios) + "/" + Id).DeleteAsync();
            await conexionFirebase.Child("Alumnos").Child(Id).DeleteAsync();
            return true;
        }

        public async Task deleteAlum(string Id)
        {
            int id = Convert.ToInt32(Id);
            var toDelete = (await conexionFirebase.Child("Alumnos").OnceAsync<Alumnos>()).FirstOrDefault
                (a => a.Object.Id == id);
            await conexionFirebase.Child("Alumnos").Child(toDelete.Key).DeleteAsync();
        }

    }
}
