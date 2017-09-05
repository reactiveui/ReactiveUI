using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class BlobCacheKeys
    {
        public static string GetKeyForSearch(string query) => string.Format("searchQuery-{0}", query);
    }
}