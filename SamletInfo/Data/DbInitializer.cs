using SamletInfo.Models;
using SamletInfo.Security;

namespace SamletInfo.Data
{
    public static class DbInitializer
    {
        public static void Seed(HotelContext context)
        {
            if (!context.Rooms.Any())
            {
                context.Rooms.AddRange(
                    new Room { Beds = 1, Quality = "Standard", IsAvailable = true },
                    new Room { Beds = 2, Quality = "Standard", IsAvailable = true },
                    new Room { Beds = 2, Quality = "Deluxe", IsAvailable = true },
                    new Room { Beds = 3, Quality = "Deluxe", IsAvailable = true },
                    new Room { Beds = 2, Quality = "Suite", IsAvailable = true },
                    new Room { Beds = 4, Quality = "Suite", IsAvailable = true }
                );
            }

            if (!context.TaskTemplates.Any())
            {
                context.TaskTemplates.AddRange(
                    new TaskTemplate { Type = "Cleaner", Note = "Full rengjøring etter utsjekk" },
                    new TaskTemplate { Type = "Cleaner", Note = "Bytt håndklær og sengetøy" },
                    new TaskTemplate { Type = "Cleaner", Note = "Fyll opp baderomsartikler" },
                    new TaskTemplate { Type = "Service", Note = "Roomservice / ekstra håndklær" },
                    new TaskTemplate { Type = "Service", Note = "Lever ekstra puter" },
                    new TaskTemplate { Type = "Maintenance", Note = "Sjekk aircondition" },
                    new TaskTemplate { Type = "Maintenance", Note = "Reparer lekk kran" }
                );
            }
            else
            {
                var notes = new Dictionary<string, string>
                {
                    ["Full cleaning after checkout"] = "Full rengjøring etter utsjekk",
                    ["Change towels and linens"] = "Bytt håndklær og sengetøy",
                    ["Restock bathroom supplies"] = "Fyll opp baderomsartikler",
                    ["Room service / extra towels"] = "Roomservice / ekstra håndklær",
                    ["Deliver extra pillows"] = "Lever ekstra puter",
                    ["Check air conditioning"] = "Sjekk aircondition",
                    ["Repair leaking tap"] = "Reparer lekk kran",
                    ["Clean after checkout"] = "Rengjøring etter utsjekk"
                };
                foreach (var template in context.TaskTemplates)
                {
                    if (notes.TryGetValue(template.Note, out var translated))
                    {
                        template.Note = translated;
                    }
                }

                foreach (var task in context.ServiceTasks)
                {
                    if (notes.TryGetValue(task.Note, out var translated))
                    {
                        task.Note = translated;
                    }
                }
            }

            if (!context.Users.Any())
            {
                context.Users.Add(new User
                {
                    Email = "guest@hotel.local",
                    Password = PasswordHasher.Hash("Password123!")
                });
            }

            context.SaveChanges();

            if (!context.ServiceTasks.Any() && context.Rooms.Any())
            {
                var firstRoomId = context.Rooms.OrderBy(r => r.Id).Select(r => r.Id).First();
                context.ServiceTasks.Add(new ServiceTask
                {
                    RoomId = firstRoomId,
                    Type = "Cleaner",
                    Status = "New",
                    Note = "Full rengjøring etter utsjekk"
                });
                context.SaveChanges();
            }
        }
    }
}
