using Domain.Common;

namespace Domain.Entities;

public class Book : BaseEntity<int>
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
}
