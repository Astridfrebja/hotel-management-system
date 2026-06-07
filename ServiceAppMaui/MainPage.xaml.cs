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
        string selected = RolePicker.SelectedItem.ToString();

        string actualType = selected switch
        {
            "Cleaning" => "Cleaner",
            "Service" => "Service",
            "Maintenance" => "Maintenance",
            _ => selected
        };

        _tasks = await _api.GetTasksAsync(actualType);
        TaskList.ItemsSource = _tasks;
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
