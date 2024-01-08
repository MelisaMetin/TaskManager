using System.ComponentModel.DataAnnotations;
using WebApplication2.Models.Enums;

namespace WebApplication2.Models
{
    public class Task
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; }

        public Status Status { get; set;}

        public Priority Priority { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

    }
}
