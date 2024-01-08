using WebApplication2.Models.Enums;

namespace WebApplication2.Models.ViewModels
{
    public class TaskFilterViewModel
    {
        public Status? SelectedStatus { get; set; }
        public string TitleSearch { get; set; }

        public Priority? PriorityFilter { get; set; }
        public List<Task> Tasks { get; set; }
    }


}
