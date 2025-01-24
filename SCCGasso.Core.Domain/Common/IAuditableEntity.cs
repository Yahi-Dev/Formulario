namespace SCCGasso.Core.Domain.Common
{
    public class IAuditableEntity
    {
        public int? Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedById { get; set; }
    }
}
