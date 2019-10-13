using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Automatonymous;
using Newtonsoft.Json;

namespace Company.Client.Bets.Services
{
    public class JsonStateSerializer<TStateMachine, TInstance>
        where TStateMachine : StateMachine<TInstance>
        where TInstance : class
    {
        readonly TStateMachine _machine;

        JsonSerializer _deserializer;

        JsonSerializer _serializer;

        public JsonStateSerializer(TStateMachine machine)
        {
            _machine = machine;
        }

        public JsonSerializer Deserializer
        {
            get
            {
                return _deserializer ?? (_deserializer = JsonSerializer.Create(new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Auto,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    TypeNameHandling = TypeNameHandling.Objects,
                    Converters = new List<JsonConverter>(new JsonConverter[]
                    {
                        new StateConverter<TStateMachine>(_machine),
                    })
                }));
            }
        }

        public JsonSerializer Serializer
        {
            get
            {
                return _serializer ?? (_serializer = JsonSerializer.Create(new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Auto,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    TypeNameHandling = TypeNameHandling.Objects,
                    Converters = new List<JsonConverter>(new JsonConverter[]
                    {
                        new StateConverter<TStateMachine>(_machine),
                    }),
                }));
            }
        }

        public string Serialize<T>(T instance) where T : TInstance
        {
            using (var ms = new MemoryStream())
            {
                Serialize(ms, instance);

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        public void Serialize<T>(Stream output, T instance) where T : TInstance
        {
            using (var writer = new StreamWriter(output))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.Formatting = Formatting.Indented;
                Serializer.Serialize(jsonWriter, instance);
                jsonWriter.Flush();
                writer.Flush();
            }
        }

        public T Deserialize<T>(string body) where T : TInstance
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(body)))
            {
                return Deserialize<T>(ms);
            }
        }

        public T Deserialize<T>(Stream input) where T : TInstance
        {
            using (var reader = new StreamReader(input))
            using (var jsonReader = new JsonTextReader(reader))
                return Deserializer.Deserialize<T>(jsonReader);
        }
    }

    public class StateConverter<T> : JsonConverter where T : StateMachine
    {
        readonly T _machine;

        public StateConverter(T machine)
        {
            _machine = machine;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var state = (State)value;
            string text = state.Name;
            if (string.IsNullOrEmpty(text))
                text = "";

            writer.WriteValue(text);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return default(State);

            if (reader.TokenType == JsonToken.String)
            {
                var text = (string)reader.Value;
                if (string.IsNullOrWhiteSpace(text))
                    return default(State);

                return _machine.GetState((string)reader.Value);
            }

            throw new JsonReaderException(string.Format(CultureInfo.InvariantCulture, "Error reading State. Expected a string but got {0}.", new object[] { reader.TokenType }));
        }

        public override bool CanConvert(Type objectType) => typeof(State).IsAssignableFrom(objectType);
    }
}
