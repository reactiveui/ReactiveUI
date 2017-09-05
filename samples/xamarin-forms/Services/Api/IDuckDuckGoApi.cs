using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace Services.Api
{
    public interface IDuckDuckGoApi
    {
        [Get("/?q={query}&format=json")]
        IObservable<DuckDuckGoSearchResult> Search(string query);
    }
}
