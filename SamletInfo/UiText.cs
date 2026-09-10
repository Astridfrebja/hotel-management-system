namespace SamletInfo
{
    public static class UiText
    {
        public static string Status(string? status) => status switch
        {
            "Reserved" => "Reservert",
            "CheckedIn" => "Innsjekket",
            "CheckedOut" => "Utsjekket",
            "Cancelled" => "Kansellert",
            "New" => "Ny",
            "Done" => "Ferdig",
            "Finished" => "Ferdig",
            _ => string.IsNullOrWhiteSpace(status) ? "Reservert" : status
        };

        public static string Role(string? type) => type switch
        {
            "Cleaning" or "Cleaner" => "Rengjøring",
            "Maintenance" => "Vedlikehold",
            "Service" => "Service",
            _ => type ?? ""
        };
    }
}
