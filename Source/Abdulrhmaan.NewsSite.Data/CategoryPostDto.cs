using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abdulrhmaan.NewsSite.Data
{
    public class CategoryPostDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsDeleted { get; set; }
        public string? Status { get; set; }
        public string UserId { get; set; }
        public int CategoryId { get; set; }
    }
}
