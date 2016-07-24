using System;

namespace THT.Data.Infrastructure
{
    public interface IDbFactory : IDisposable
    {
        THTDbContext Init();
    }
}