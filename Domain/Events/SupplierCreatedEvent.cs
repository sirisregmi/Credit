namespace Credit.Domain.Events;

public class SupplierCreatedEvent : DomainEvent
{
    public SupplierCreatedEvent(Supplier item)
    {
        Item = item;
    }

    public Supplier Item { get; }
}