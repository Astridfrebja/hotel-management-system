using SharedModels.Models;

namespace ServiceAppMaui;

public partial class MainPage : ContentPage
{
    private readonly ApiClientMaui _api = new ApiClientMaui();
    private List<ServiceTask> _tasks = new();

    public MainPage()
    {
        InitializeComponent();
    }

    private async void RolePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (RolePicker.SelectedItem is not string selected)
        {
            return;
        }

        string actualType = selected switch
        {
            "Cleaning" or "Rengjøring" => "Cleaner",
            "Service" => "Service",
            "Maintenance" or "Vedlikehold" => "Maintenance",
            _ => selected
        };

        try
        {
            _tasks = await _api.GetTasksAsync(actualType);
            TaskList.ItemsSource = _tasks;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Feil", $"Kunne ikke hente oppgaver. Start webappen først.\n{ex.Message}", "OK");
        }
    }

    private async void CompleteTask_Clicked(object sender, EventArgs e)
    {
        if (TaskList.SelectedItem is ServiceTask task)
        {
            task.Status = "Done";
            bool success = await _api.UpdateTaskAsync(task);
            if (success)
                await DisplayAlert("Oppdatert", "Oppgave markert som ferdig", "OK");
        }
    }
}
