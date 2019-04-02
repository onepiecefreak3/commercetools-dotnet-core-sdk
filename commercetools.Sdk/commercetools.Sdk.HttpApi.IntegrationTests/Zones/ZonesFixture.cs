using System;
using System.Collections.Generic;
using commercetools.Sdk.Client;
using commercetools.Sdk.Domain;
using commercetools.Sdk.Domain.Categories;
using commercetools.Sdk.Domain.Messages;
using commercetools.Sdk.Domain.ShippingMethods;
using commercetools.Sdk.Domain.Zones;

namespace commercetools.Sdk.HttpApi.IntegrationTests.Zones
{
    public class ZonesFixture : ClientFixture, IDisposable
    {

        public List<Zone> ZonesToDelete { get; private set; }

        public ZonesFixture() : base()
        {
            this.ZonesToDelete = new List<Zone>();
        }

        public void Dispose()
        {
            IClient commerceToolsClient = this.GetService<IClient>();
            this.ZonesToDelete.Reverse();
            foreach (Zone zone in this.ZonesToDelete)
            {
                Zone deletedZone = commerceToolsClient.ExecuteAsync(new DeleteByIdCommand<Zone>(new Guid(zone.Id), zone.Version)).Result;
            }
        }

        /// <summary>
        /// Create Zone Draft
        /// </summary>
        /// <returns></returns>

        public ZoneDraft GetZoneDraft(string country = null)
        {
            int ran = this.RandomInt();
            string zoneCountry = country ?? this.GetRandomEuropeCountry();
            var locations = new List<Location>() { new Location() { Country =  zoneCountry}};
            ZoneDraft zoneDraft = new ZoneDraft()
            {
                Name = $"EuropeZone_{ran}",
                Key = $"Zone_Key_{ran}",
                Locations = locations
            };

            return zoneDraft;
        }

        public Zone CreateZone(string country = null)
        {
            return this.CreateZone(this.GetZoneDraft(country));
        }

        public Zone CreateZone(ZoneDraft zoneDraft)
        {
            IClient commerceToolsClient = this.GetService<IClient>();
            Zone zone = commerceToolsClient.ExecuteAsync(new CreateCommand<Zone>(zoneDraft)).Result;
            return zone;
        }

    }
}
