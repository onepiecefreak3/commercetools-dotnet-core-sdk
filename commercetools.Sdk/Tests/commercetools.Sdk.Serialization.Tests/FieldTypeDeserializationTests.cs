using commercetools.Sdk.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using FluentAssertions.Json;
using Xunit;
using Type = commercetools.Sdk.Domain.Type;

namespace commercetools.Sdk.Serialization.Tests
{
    public class FieldTypeDeserializationTests : IClassFixture<SerializationFixture>
    {
        private readonly SerializationFixture serializationFixture;

        public FieldTypeDeserializationTests(SerializationFixture serializationFixture)
        {
            this.serializationFixture = serializationFixture;
        }

        [Fact]
        public void DeserializeStringFieldType()
        {
            ISerializerService serializerService = this.serializationFixture.SerializerService;
            string serialized = File.ReadAllText("Resources/FieldTypes/String.json");
            Type deserialized = serializerService.Deserialize<Type>(serialized);
            Assert.IsType<StringFieldType>(deserialized.FieldDefinitions[0].Type);
        }

        [Fact]
        public void SerializeStringFieldType()
        {
            ISerializerService serializerService = this.serializationFixture.SerializerService;
            TypeDraft typeDraft = new TypeDraft();
            typeDraft.Key = "new-key";
            typeDraft.ResourceTypeIds = new List<ResourceTypeId>();
            typeDraft.ResourceTypeIds.Add(ResourceTypeId.Category);
            typeDraft.ResourceTypeIds.Add(ResourceTypeId.CustomLineItem);
            FieldDefinition fieldDefinition = new FieldDefinition();
            fieldDefinition.Name = "string-field";
            fieldDefinition.Required = true;
            fieldDefinition.Label = new LocalizedString();
            fieldDefinition.Label.Add("en", "string description");
            fieldDefinition.InputHint = TextInputHint.SingleLine;
            fieldDefinition.Type = new StringFieldType();
            typeDraft.FieldDefinitions = new List<FieldDefinition>();
            typeDraft.FieldDefinitions.Add(fieldDefinition);
            string result = serializerService.Serialize(typeDraft);
            JToken resultFormatted = JValue.Parse(result);
            string serialized = File.ReadAllText("Resources/FieldTypes/Serialized.json");
            JToken serializedFormatted = JValue.Parse(serialized);
            serializedFormatted.Should().BeEquivalentTo(resultFormatted);
        }

        [Fact]
        public void SerializeCustomField()
        {
            ISerializerService serializerService = this.serializationFixture.SerializerService;
            var customFieldsDraft = new CustomFieldsDraft() {
                Fields = new Fields()
                {
                    {"foo", "bar"},
                    {"Fooz", "Bars"},
                    {"Bar", new EnumValue { Key = "BarKey", Label = "BarLabel"} }
                }
            };
            string result1 = serializerService.Serialize(customFieldsDraft);

            JToken resultFormatted = JValue.Parse(result1);
            string serialized = File.ReadAllText("Resources/CustomFields/CustomFieldsDraft.json");
            JToken serializedFormatted = JValue.Parse(serialized);
            serializedFormatted.Should().BeEquivalentTo(resultFormatted);

            var draft = new Fields()
            {
                {"foo", "bar"},
                {"Fooz", "Bars"},
                {"Bar", new EnumValue { Key = "BarKey", Label = "BarLabel"} }
            };

            string result2 = serializerService.Serialize(draft);

            resultFormatted = JValue.Parse(result2);
            serialized = File.ReadAllText("Resources/CustomFields/Fields.json");
            serializedFormatted = JValue.Parse(serialized);
            serializedFormatted.Should().BeEquivalentTo(resultFormatted);
        }

    }
}
