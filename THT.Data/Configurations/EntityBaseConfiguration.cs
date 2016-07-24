using System.Data.Entity.ModelConfiguration;
using THT.Model.Models;

namespace THT.Data.Configurations
{
    public class EntityBaseConfiguration<T> : EntityTypeConfiguration<T> where T : class, IBaseEnity
    {
        public EntityBaseConfiguration()
        {
            HasKey(e => e.ID);
        }
    }
}