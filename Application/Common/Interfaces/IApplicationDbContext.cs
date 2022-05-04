using Credit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Credit.Application.Common.Interfaces;

public interface IApplicationDbContext:DbContext
{
    DbSet<Supplier> Suppliers { get; }
 
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

     
}