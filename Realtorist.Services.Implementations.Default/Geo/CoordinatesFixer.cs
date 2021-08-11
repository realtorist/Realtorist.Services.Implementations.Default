using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Realtorist.DataAccess.Abstractions;
using Realtorist.GeoCoding.Abstractions;
using Realtorist.Services.Abstractions.Geo;

namespace Realtorist.Services.Implementations.Default.Geo
{
    public class CoordinatesFixer : ICoordinatesFixer
    {
        private readonly IListingsDataAccess _listingsDataAccess;
        private readonly IBatchGeoCoder _batchGeoCoder;

        private readonly ILogger _logger;

        public CoordinatesFixer(IListingsDataAccess listingsDataAccess, IBatchGeoCoder batchGeoCoder, ILogger<CoordinatesFixer> logger)
        {
            _listingsDataAccess = listingsDataAccess ?? throw new ArgumentNullException(nameof(listingsDataAccess));
            _batchGeoCoder = batchGeoCoder ?? throw new ArgumentNullException(nameof(batchGeoCoder));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task FixListingsEmptyCoordinates()
        {
            var listings = await _listingsDataAccess.GetListingsWithEmptyCoordinatesAsync();
            var addresses = listings.ToDictionary(x => x.Id, x => x.Address);

            _logger.LogInformation($"Need to fix coordinates for {addresses.Count} addresses.");

            await _batchGeoCoder.GeoCodeAddressesAsync(addresses, _listingsDataAccess.UpdateListingCoordinatesAsync, (_) => Task.FromResult(0));

            _logger.LogInformation("Done fixing coordinates.");
        }
    }
}