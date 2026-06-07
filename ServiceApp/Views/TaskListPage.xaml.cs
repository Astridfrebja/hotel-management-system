using Microsoft.Maui.Controls;
using SamletInfo.Models; // Legg til dette øverst i C#-filene dine i Maui
namespace ServiceApp.Views;

public partial class TaskListPage : ContentPage
{
    public TaskListPage(string role)
    {
        InitializeComponent();
        // Du kan bruke 'role'-parameteren her for å hente riktige data
        Title = $"Oppgaver for {role}";
    }
}