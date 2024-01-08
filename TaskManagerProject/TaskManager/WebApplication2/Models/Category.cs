namespace WebApplication2.Models
{
    public class Category
    {
        public Category()
        {
            this.Tasks = new List<Task>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public List<Task>? Tasks  { get; set; }
    }
}
