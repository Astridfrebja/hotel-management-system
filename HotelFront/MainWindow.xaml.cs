using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SamletInfo.Models;

namespace HotelFront
{
    public partial class MainWindow : Window
    {
        private readonly ApiClient _apiClient = new ApiClient();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                await LoadBookings();
                await LoadRooms();
            };
        }

        private async Task LoadBookings()
        {
            BookingGrid.ItemsSource = null;
            var bookings = await _apiClient.GetBookings();
            BookingGrid.ItemsSource = bookings;
        }

        private async Task LoadRooms()
        {
            var rooms = await _apiClient.GetRooms();
            RoomGrid.ItemsSource = rooms;
        }

        private void RoomGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveNewBookingButton.IsEnabled = RoomGrid.SelectedItem != null;
        }

        private async void SaveNewBooking_Click(object sender, RoutedEventArgs e)
        {
            if (RoomGrid.SelectedItem is Room selectedRoom)
            {
                string customerEmail = NewBookingEmailTextBox.Text;
                DateTime? checkIn = NewBookingCheckInDatePicker.SelectedDate;
                DateTime? checkOut = NewBookingCheckOutDatePicker.SelectedDate;

                if (string.IsNullOrWhiteSpace(customerEmail) || checkIn == null || checkOut == null || checkIn >= checkOut)
                {
                    MessageBox.Show("Fyll ut gyldige felter for reservasjon.");
                    return;
                }

                var newBooking = new Booking
                {
                    CustomerEmail = customerEmail,
                    RoomId = selectedRoom.Id,
                    CheckIn = checkIn.Value,
                    CheckOut = checkOut.Value
                };

                if (await _apiClient.AddBooking(newBooking))
                {
                    MessageBox.Show("Reservasjon lagt til.");
                    await LoadBookings();
                    await LoadRooms();
                    NewBookingEmailTextBox.Clear();
                    NewBookingCheckInDatePicker.SelectedDate = null;
                    NewBookingCheckOutDatePicker.SelectedDate = null;
                    SaveNewBookingButton.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("Feil ved lagring av reservasjon.");
                }
            }
        }

        private async void DeleteBooking_Click(object sender, RoutedEventArgs e)
        {
            if (BookingGrid.SelectedItem is Booking selectedBooking)
            {
                if (MessageBox.Show("Slette valgt reservasjon?", "Bekreft", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (await _apiClient.DeleteBooking(selectedBooking.Id))
                    {
                        MessageBox.Show("Reservasjon slettet.");
                        await LoadBookings();
                        await LoadRooms();
                    }
                    else
                    {
                        MessageBox.Show("Feil ved sletting.");
                    }
                }
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadBookings();
            await LoadRooms();
        }

        private void NewBookingEmailTextBox_TextChanged(object sender, TextChangedEventArgs e) { }

        private async void TaskTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TaskTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string roleType = selectedItem.Content?.ToString() == "Cleaning"
                    ? "Cleaner"
                    : selectedItem.Content?.ToString() ?? "";
                await LoadTaskTemplates(roleType);
            }
        }

        private async Task LoadTaskTemplates(string roleType)
        {
            var templates = await _apiClient.GetTaskTemplates();
            var filtered = templates.Where(t => t.Type == roleType).ToList();
            TaskNoteComboBox.ItemsSource = filtered;
            TaskNoteComboBox.DisplayMemberPath = "Note";
        }

        private async void AddServiceTask_Click(object sender, RoutedEventArgs e)
        {
            if (RoomGrid.SelectedItem is Room selectedRoom &&
                TaskTypeComboBox.SelectedItem is ComboBoxItem selectedTypeItem &&
                TaskNoteComboBox.SelectedItem is TaskTemplate selectedTemplate)
            {
                var task = new ServiceTask
                {
                    RoomId = selectedRoom.Id,
                    Type = selectedTypeItem.Content?.ToString() == "Cleaning"
                        ? "Cleaner"
                        : selectedTypeItem.Content?.ToString(),
                    Note = selectedTemplate.Note,
                    Status = "New"
                };

                if (await _apiClient.CreateServiceTask(task))
                {
                    MessageBox.Show("Oppgave lagt til for rom " + selectedRoom.Id);
                }
                else
                {
                    MessageBox.Show("Feil ved oppretting av oppgave.");
                }
            }
            else
            {
                MessageBox.Show("Velg rom, type og oppgave.");
            }
        }

    }
}
