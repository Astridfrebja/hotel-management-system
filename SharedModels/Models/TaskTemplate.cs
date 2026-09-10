namespace SharedModels.Models
{
    public class TaskTemplate
    {
        public int Id { get; set; }
        public string Note { get; set; } = "";
        public string Type { get; set; } = "";

        public override string ToString() => Note;
    }
}
