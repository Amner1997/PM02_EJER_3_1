using PM02_EJER_3_1.Models;
using PM02_EJER_3_1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PM02_EJER_3_1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListaAlumnos : ContentPage
    {
        FirebaseConnection _connection = new FirebaseConnection();
        public Alumnos Alum;
        bool editando;
        public ListaAlumnos()
        {
            InitializeComponent();
            LoadData();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadData();
            editando = false;
            Alum = null;

        }
        private async void LoadData()
        {
            listAlumnos.ItemsSource = await _connection.GetAllAlumnos();


            var current = Connectivity.NetworkAccess;

            if (current != NetworkAccess.Internet)
            {
                await DisplayAlert("Aviso", "No cuenta con acceso a internet", "OK");
                return;
            }
        }
        private void btnRefresh_Clicked(object sender, EventArgs e)
        {
            LoadData();
        }

        private async void SwipeItem_Delete(object sender, EventArgs e)
        {
            var resp = await DisplayAlert("Aviso", "Desea eliminar el campo?", "Si", "No");
            if (resp)
            {
                SwipeItem item = sender as SwipeItem;
                var Id = item.CommandParameter.ToString();
                if (Id != null)
                {
                    await _connection.deleteAlum(Id);
                    LoadData();
                    await DisplayAlert("Alerta", "Alumno eliminado correctamente", "Ok");

                }
                else await DisplayAlert("Error", "error", "ok");
            }
            else
            {
                await DisplayAlert("Error", "Ha ocurrido un error eliminando el Alumno", "Ok");
            }
        }

        private async void SwipeItem_Edit(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new EditarAlumxaml(Alum));
        }

        private void listSites_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Alum = (Alumnos)e.Item;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }
    }
}