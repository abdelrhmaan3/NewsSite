
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abdulrhmaan.NewsSite.Data
{
    public class NewsPostDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Status { get; set; }
        public bool? IsDeleted { get; set; }
        public string UserId { get; set; }
        public int CategoryId { get; set; }
    }
}
