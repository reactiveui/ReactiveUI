using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Api;

namespace Services.Disconnected.Api
{
    public class DuckDuckGoApiServiceDisconnected : IDuckDuckGoApiService
    {
        private readonly IDuckDuckGoApi _duckDuckGoApi;

        public DuckDuckGoApiServiceDisconnected(bool enableRandomDelays, bool enableRandomErrors)
        {
            _duckDuckGoApi = new DuckDuckGoApiDisconnected(enableRandomDelays, enableRandomErrors);
        }

        public IDuckDuckGoApi Background => _duckDuckGoApi;
        public IDuckDuckGoApi Speculative => _duckDuckGoApi;
        public IDuckDuckGoApi UserInitiated => _duckDuckGoApi;
    }
}
