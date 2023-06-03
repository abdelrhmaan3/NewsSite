using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abdulrhmaan.News.SQlServer;

    public class Category
    {
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? UserId { get; set; }
    public  User? User { get; set; }
    public DateTime InsertedAt { get; set; }
    public string? Status { get; set; }
    public bool? IsDeleted { get; set; }
    public ICollection<News> News { get; set; }


}
