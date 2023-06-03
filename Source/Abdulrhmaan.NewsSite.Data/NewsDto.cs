namespace Abdulrhmaan.NewsSite.Data
{
    public class NewsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AuthorName { get; set; }
        public string CategoryName { get; set; }
        public DateTime InsertedAt { get; set; }
        public string? Status { get; set; }
        public bool? IsDeleted { get; set; }
        public string UserId { get; set; }
        public int CategoryId { get; set; }

    }
}