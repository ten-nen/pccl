using Pccl.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Pccl.Infrastructure.Data.Extensions
{
    internal static class EntityEntryExtensions
    {
        internal static bool ShouldBeAudited(this EntityEntry entry)
        {
            return entry.State != EntityState.Detached && entry.State != EntityState.Unchanged &&
                   (entry.Entity is IAuditedEntity) && entry.IsAuditable();
        }

        internal static bool IsAuditable(this EntityEntry entityEntry)
        {
            var type = entityEntry.Entity.GetType();
            if (type.IsDefined(typeof(AuditableAttribute)))
                return true;
            if (type.IsDefined(typeof(NonAuditableAttribute)))
                return false;
            return true;
        }
        internal static bool IsAuditable(this PropertyEntry propertyEntry)
        {
            Type entityType = propertyEntry.EntityEntry.Entity.GetType();
            PropertyInfo propertyInfo = entityType.GetProperty(propertyEntry.Metadata.Name);
            if (propertyInfo.IsDefined(typeof(AuditableAttribute)))
            {
                return true;
            }
            if (propertyInfo.IsDefined(typeof(NonAuditableAttribute)))
            {
                return false;
            }
            return true;
        }
        public static string ToReadablePrimaryKey(this EntityEntry entry)
        {
            IKey primaryKey = entry.Metadata.FindPrimaryKey();
            if (primaryKey == null)
            {
                return null;
            }
            else
            {
                return string.Join(",", (primaryKey.Properties.ToDictionary(x => x.Name, x => x.PropertyInfo.GetValue(entry.Entity))).Select(x => x.Key + "=" + x.Value));
            }
        }

        public static Guid ToGuidHash(this string readablePrimaryKey)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hashValue = sha512.ComputeHash(Encoding.Default.GetBytes(readablePrimaryKey));
                byte[] reducedHashValue = new byte[16];
                for (int i = 0; i < 16; i++)
                {
                    reducedHashValue[i] = hashValue[i * 4];
                }
                return new Guid(reducedHashValue);
            }
        }
    }
}
