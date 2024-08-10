using System.ComponentModel.DataAnnotations;

namespace Domain.Common
{
    public class BaseEntity<TId>
    {
        [Key]
        public TId Id { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set;}
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
