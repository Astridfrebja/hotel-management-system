using SamletInfo.Models; // Legg til dette øverst i C#-filene dine i Maui

namespace ServiceApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCleanerButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TaskListPage("Renholder")); // Naviger til oppgavelisten med valgt rolle
        }

        private async void OnServiceButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TaskListPage("Serviceperson"));
        }

        private async void OnMaintainerButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TaskListPage("Vedlikeholdsperson"));
        }
    }

}
