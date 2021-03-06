using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using CoolStore.WebUI.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoolStore.WebUI.Host.Controllers
{
    [ApiController]
    [Route("api/inventories")]
    public class InventoryController : ControllerBase
    {
        private readonly IGraphQLClient _client;

        public InventoryController(IGraphQLClient client)
        {
            _client = client;
        }

        [Authorize]
        [HttpGet("stores")]
        public async Task<ImmutableList<KeyValueModel>> GetStores()
        {
            var response = await _client.GetStoresAsync();
            return response.Data.Stores
                .Select(x => new KeyValueModel {Key = x.Id.ToString(), Value = x.Location})
                .ToImmutableList();
        }
    }
}
