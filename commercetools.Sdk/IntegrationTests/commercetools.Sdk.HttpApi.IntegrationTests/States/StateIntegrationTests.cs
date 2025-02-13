using System;
using commercetools.Sdk.Client;
using commercetools.Sdk.Domain.States;
using commercetools.Sdk.Domain.Zones;
using commercetools.Sdk.HttpApi.IntegrationTests.States;
using Xunit;

namespace commercetools.Sdk.HttpApi.IntegrationTests.States
{
    [Collection("Integration Tests")]
    public class StatesIntegrationTests : IDisposable
    {
        private readonly StatesFixture statesFixture;

        public StatesIntegrationTests(ServiceProviderFixture serviceProviderFixture)
        {
            this.statesFixture = new StatesFixture(serviceProviderFixture);
        }

        public void Dispose()
        {
            this.statesFixture.Dispose();
        }

        [Fact]
        public void CreateState()
        {
            IClient commerceToolsClient = this.statesFixture.GetService<IClient>();
            string stateKey = $"Key-{TestingUtility.RandomInt()}";
            StateDraft stateDraft = this.statesFixture.GetStateDraft(stateKey);
            State state = commerceToolsClient
                .ExecuteAsync(new CreateCommand<State>(stateDraft)).Result;
            this.statesFixture.StatesToDelete.Add(state);
            Assert.Equal(stateDraft.Key, state.Key);
        }
    }
}
