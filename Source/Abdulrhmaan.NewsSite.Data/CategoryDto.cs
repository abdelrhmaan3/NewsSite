using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abdulrhmaan.NewsSite.Data
{
    public class CategoryDto
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
