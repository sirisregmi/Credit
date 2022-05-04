using Credit.Application.Common.Interfaces;
using Credit.Domain.Entities;
using Credit.Domain.Events;
using MediatR;

namespace Credit.Application.Suppliers.Commands.CreateSupplier{

public class CreateSupplierCommand : IRequest<int>
{
    public int ListId { get; set; }

    public string? Title { get; set; }
}

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateSupplierCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var entity = new Supplier

        {
            ListId = request.ListId,
            Title = request.Title,
            
        };

        entity.DomainEvents.Add(new SupplierCreatedEvent(entity));

        _context.Suppliers.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
}