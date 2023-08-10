using Plugin.Media;
using Plugin.Media.Abstractions;
using PM02_EJER_3_1.Models;
using PM02_EJER_3_1.Services;
using PM02_EJER_3_1.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Joins;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM02_EJER_3_1
{
    public partial class MainPage : ContentPage
    {
        private int sitioIdCounter = 1; // Variable para el contador de ID
        MediaFile FotoCap = null;
        byte[] Imagen;
        string Sexo;
        public MainPage()
        {
            InitializeComponent();
            rbMascu.CheckedChanged += RadioButton_CheckedChanged;
            rbFemenino.CheckedChanged += RadioButton_CheckedChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            GenerarSiguienteId();

        }

        private int GenerarSiguienteId()
        {

            int id = sitioIdCounter;
            sitioIdCounter++;
            return id;
        }

        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender == rbMascu && e.Value)
            {
                // Opción Masculino seleccionada
                Sexo = "Masculino";
            }
            else if (sender == rbFemenino && e.Value)
            {
                // Opción Femenino seleccionada
                Sexo = "Femenino";
            }
        }

        private Byte[] ConvertirFotoAByteArray()
        {
            if (FotoCap != null)
            {
                using (MemoryStream MemSt = new MemoryStream())
                {
                    Stream stream = FotoCap.GetStream();
                    stream.CopyTo(MemSt);
                    return MemSt.ToArray();
                }
            }
            return null;
        }

        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            var ActualConex = Connectivity.NetworkAccess;

            if (ActualConex != NetworkAccess.Internet)
            {
                await DisplayAlert("Aviso", "No hay conexion a internet", "OK");
                return;
            }


            if (Imagen == null)
            {
                await DisplayAlert("Error", "Todavia no ha tomado foto", "OK");
                return;
            }

            if (string.IsNullOrEmpty(txtNombres.Text))
            {
                await DisplayAlert("Error", "Ingrese su nombre", "OK");
                return;
            }

            if (string.IsNullOrEmpty(txtApellidos.Text))
            {
                await DisplayAlert("Error", "Ingrese su apellido", "OK");
                return;
            }

            if (string.IsNullOrEmpty(Sexo))
            {
                await DisplayAlert("Error", "Ingrese su sexo", "OK");
                return;
            }

            if (string.IsNullOrEmpty(txtDireccion.Text))
            {
                await DisplayAlert("Error", "Ingrese su direccion", "OK");
                return;
            }


            Alumnos sitio = new Alumnos();
            sitio.Id = GenerarSiguienteId();
            sitio.Nombres = txtNombres.Text;
            sitio.Apellidos = txtApellidos.Text;
            sitio.Sexo = Sexo;
            sitio.Direccion = txtDireccion.Text;
            sitio.Fotografia = Imagen;

            FirebaseConnection Crud = new FirebaseConnection();

            var IsSaved = Crud.AgregarAlumnos(sitio);

            await DisplayAlert("Aviso", "Alumno Agregado", "OK");
            imgFoto.Source = "Foto.png";
            txtNombres.Text = "";
            txtApellidos.Text = "";
            //txtSexo.Text = "";
            txtDireccion.Text = "";
            Imagen = null;

        }

        private async void btnList_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ListaAlumnos());
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var EstadoPerm = await Permissions.CheckStatusAsync<Permissions.Photos>();
            if (EstadoPerm == PermissionStatus.Granted)
            {
                FotoCap = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Medium,
                    SaveToAlbum = true
                });

                if (FotoCap == null)
                    return;

                imgFoto.Source = ImageSource.FromStream(() => { return FotoCap.GetStream(); });
                Imagen = File.ReadAllBytes(FotoCap.Path);
            }
            else
            {
                await Permissions.RequestAsync<Permissions.Camera>();
            }
        }
    }
}
