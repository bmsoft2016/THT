namespace THT.Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        private THTDbContext dbContext;

        public THTDbContext Init()
        {
            return dbContext ?? (dbContext = new THTDbContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}