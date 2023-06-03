namespace Client.News.Models
{
    public class CategoryVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? InsertedAt { get; set; }
        public string AuthorName { get; set; }
        public bool? IsDeleted { get; set; }
        public string? Status { get; set; }
        public string UserId { get; set; }
        public string CategoryId { get; set; }

    }
}
