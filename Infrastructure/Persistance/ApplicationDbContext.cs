using System.Reflection;
using Credit.Application.Common.Interfaces;
using Credit.Domain.Common;
using Credit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace Credit.Infrastructure.Persistence;



public class UserContext : DbContext
{
    public UserContext (DbContextOptions<UserContext > options)
        : base(options)
    { }

    public DbSet<Supplier> Suppliers{ get; set; }
}


public class ApplicationDbContext : IApplicationDbContext
{
    public DbSet<Supplier> Suppliers => Set<Supplier>();

    private readonly IDomainEventService _domainEventService;


    public ApplicationDbContext(   
        DbContextOptions<ApplicationDbContext> options,
        IDomainEventService domainEventService,
        IDateTime dateTime) : base(options, operationalStoreOptions)
    {


        _domainEventService = domainEventService;
        _dateTime = dateTime;
    }




    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    entry.Entity.Created = _dateTime.Now;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = _currentUserService.UserId;
                    entry.Entity.LastModified = _dateTime.Now;
                    break;
            }
        }

        var events = ChangeTracker.Entries<IHasDomainEvent>()
                .Select(x => x.Entity.DomainEvents)
                .SelectMany(x => x)
                .Where(domainEvent => !domainEvent.IsPublished)
                .ToArray();

        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchEvents(events);

        return result;
    }

    /*
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }
    */
    private async Task DispatchEvents(DomainEvent[] events)
    {
        foreach (var @event in events)
        {
            @event.IsPublished = true;
            await _domainEventService.Publish(@event);
        }
    }
}