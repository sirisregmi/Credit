namespace Credit.Domain.Entities
{

    public class Supplier : AuditableEntity, IHasDomainEvent
    {
        public int Id { get; set; }

        public int ListId { get; set; }

        public string? Title { get; set; }

        public string? Note { get; set; }


        public DateTime? Reminder { get; set; }




        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
