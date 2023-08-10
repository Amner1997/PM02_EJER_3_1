using Plugin.Media;
using Plugin.Media.Abstractions;
using PM02_EJER_3_1.Models;
using PM02_EJER_3_1.Services;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Joins;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PM02_EJER_3_1.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditarAlumxaml : ContentPage
    {
        byte[] Image;
        MediaFile FileFoto = null;
        MediaFile FotoCap = null;
        Alumnos Alum;

        public EditarAlumxaml (Alumnos alum)
		{
			InitializeComponent ();
            this.Alum = alum;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadData();
        }

        void LoadData()
        {
            imgFoto.Source = GetImageResourseFromBytes(Alum.Fotografia);
            txtNombres.Text = Alum.Nombres;
            txtApellidos.Text = Alum.Apellidos;
            txtSexo.Text = Alum.Sexo;
            txtDireccion.Text = Alum.Direccion;
        }

        private ImageSource GetImageResourseFromBytes(byte[] bytes)
        {
            ImageSource retSource = null;

            if (bytes != null)
            {
                byte[] imageAsBytes = (byte[])bytes;
                retSource = ImageSource.FromStream(() => new MemoryStream(imageAsBytes));
            }

            return retSource;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //OnBackButtonPressed();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var EstadoPerm = await Permissions.CheckStatusAsync<Permissions.Photos>();
            if (EstadoPerm == PermissionStatus.Granted)
            {
                FileFoto = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Medium,
                    SaveToAlbum = true
                });

                if (FileFoto == null)
                    return;

                imgFoto.Source = ImageSource.FromStream(() => { return FileFoto.GetStream(); });
                Image = File.ReadAllBytes(FileFoto.Path);
            }
            else
            {
                await Permissions.RequestAsync<Permissions.Camera>();
            }
        }

        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            byte[] audio;

            var ActualConex = Connectivity.NetworkAccess;

            if (ActualConex != NetworkAccess.Internet)
            {
                await DisplayAlert("Aviso", "No hay conexion a internet", "OK");
                return;
            }


              if (Image == null)
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

            if (string.IsNullOrEmpty(txtSexo.Text))
            {
                await DisplayAlert("Error", "Ingrese su sexo", "OK");
                return;
            }

            if (string.IsNullOrEmpty(txtDireccion.Text))
            {
                await DisplayAlert("Error", "Ingrese su direccion", "OK");
                return;
            }


            Alumnos alum = new Alumnos();
            alum.Id = this.Alum.Id;
            alum.Nombres = txtNombres.Text;
            alum.Apellidos = txtApellidos.Text;
            alum.Sexo = txtSexo.Text;
            alum.Direccion = txtDireccion.Text;
            alum.Fotografia = Image;

            FirebaseConnection Crud = new FirebaseConnection();

            var IsSaved = Crud.EditarAlumno(alum);
            await DisplayAlert("Aviso", "Datos del Alumno Actualizados", "OK");
            imgFoto.Source = "Foto.png";
            txtNombres.Text = "";
            txtApellidos.Text = "";
            txtSexo.Text = "";
            txtDireccion.Text = "";
            Image = null;

            await Navigation.PushModalAsync(new ListaAlumnos());
        }

    }
}