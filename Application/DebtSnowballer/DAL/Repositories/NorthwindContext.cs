using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class NorthwindContext
    {
    internal void Dispose() => throw new NotImplementedException();
    internal int SaveChanges() => throw new NotImplementedException();

    public static implicit operator DbContext(NorthwindContext v) => throw new NotImplementedException();
    }