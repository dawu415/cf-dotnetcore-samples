/*
    David Wu
*/
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using CFHelper.Types;

namespace CFHelper
{
    public class CFEnvironmentVariables
    {
        private IConfigurationRoot _configuration { get; set; }
        private IDictionary<string, object> _vcap_services { get; set; }
        public VCAP_APPLICATION vcap_application_data {get; set; }

        public CFEnvironmentVariables(IConfigurationRoot configuration)
        {
            _configuration = configuration;

            var raw_vcap_app = _configuration["VCAP_APPLICATION"];
            var raw_vcap_services = _configuration["VCAP_SERVICES"];

            // If there's a vcap services entry, perform a JSON deserialization.
            if (raw_vcap_services != null)
                _vcap_services = JsonConvert.DeserializeObject<IDictionary<string, object>>(raw_vcap_services, new DictionaryConverter());

            // If there's a vcap application entry, perform a JSON deserialization and attempt to convert to a VCAP_APPLICATION data type
            if (raw_vcap_app != null)
                vcap_application_data = convert_vcap_application_dictionary_to_object(JsonConvert.DeserializeObject<IDictionary<string, object>>(raw_vcap_app, new DictionaryConverter()));
  
        }

        private VCAP_APPLICATION convert_vcap_application_dictionary_to_object(IDictionary<string, object> vcap_app_dictionary)
        {
            VCAP_APPLICATION vcapAppData = new VCAP_APPLICATION();

            // Enumerate through all fields that is available in the dictionary and populate the VCAP_APPLICATION data structure
            foreach(var field in vcap_app_dictionary.Keys)
            {
                var property = vcapAppData.GetType().GetProperty(field.ToString(), BindingFlags.Public | BindingFlags.Instance);
                var fieldValue = vcap_app_dictionary[field.ToString()];
                dynamic output;

                switch (field)
                {
                    // We know that these can be represented as a list of strings
                    case "application_uris":
                    case "uris":
                    {
                        IList<object> objList = fieldValue as IList<object>;
                        output = (IList<string>) (objList.Cast<string>().ToList());
                        break; 
                    }
                    case "limits":
                    {
                        // Convert limits to a dictionary of string keys and int values
                        output = (fieldValue as IDictionary<string, object>).ToDictionary(k => k.Key, k => Convert.ToInt32(k.Value));
                        break;
                    }
                    default:
                    {
                        // Otherwise convert to a string
                        if (fieldValue != null)
                            output = fieldValue as string;
                        else
                            output = "";
        
                        break;
                    }         
                }   

                property.SetValue(vcapAppData, output);   
            }

            return vcapAppData;
        }

    



    }

    // Retrieved DictionaryConverter code from http://stackoverflow.com/questions/11561597/deserialize-json-recursively-to-idictionarystring-object/31250524#31250524
    public class DictionaryConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { this.WriteValue(writer, value); }

        private void WriteValue(JsonWriter writer, object value) {
            var t = JToken.FromObject(value);
            switch (t.Type) {
                case JTokenType.Object:
                    this.WriteObject(writer, value);
                    break;
                case JTokenType.Array:
                    this.WriteArray(writer, value);
                    break;
                default:
                    writer.WriteValue(value);
                    break;
            }
        }

        private void WriteObject(JsonWriter writer, object value) {
            writer.WriteStartObject();
            var obj = value as IDictionary<string, object>;
            foreach (var kvp in obj) {
                writer.WritePropertyName(kvp.Key);
                this.WriteValue(writer, kvp.Value);
            }
            writer.WriteEndObject();
        }

        private void WriteArray(JsonWriter writer, object value) {
            writer.WriteStartArray();
            var array = value as IEnumerable<object>;
            foreach (var o in array) {
                this.WriteValue(writer, o);
            }
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            return ReadValue(reader);
        }

        private object ReadValue(JsonReader reader) {
            while (reader.TokenType == JsonToken.Comment) {
                if (!reader.Read()) throw new JsonSerializationException("Unexpected Token when converting IDictionary<string, object>");
            }

            switch (reader.TokenType) {
                case JsonToken.StartObject:
                    return ReadObject(reader);
                case JsonToken.StartArray:
                    return this.ReadArray(reader);
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Undefined:
                case JsonToken.Null:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    return reader.Value;
                default:
                    throw new JsonSerializationException
                        (string.Format("Unexpected token when converting IDictionary<string, object>: {0}", reader.TokenType));
            }
        }

        private object ReadArray(JsonReader reader) {
            IList<object> list = new List<object>();

            while (reader.Read()) {
                switch (reader.TokenType) {
                    case JsonToken.Comment:
                        break;
                    default:
                        var v = ReadValue(reader);

                        list.Add(v);
                        break;
                    case JsonToken.EndArray:
                        return list;
                }
            }

            throw new JsonSerializationException("Unexpected end when reading IDictionary<string, object>");
        }

        private object ReadObject(JsonReader reader) {
            var obj = new Dictionary<string, object>();

            while (reader.Read()) {
                switch (reader.TokenType) {
                    case JsonToken.PropertyName:
                        var propertyName = reader.Value.ToString();

                        if (!reader.Read()) {
                            throw new JsonSerializationException("Unexpected end when reading IDictionary<string, object>");
                        }

                        var v = ReadValue(reader);

                        obj[propertyName] = v;
                        break;
                    case JsonToken.Comment:
                        break;
                    case JsonToken.EndObject:
                        return obj;
                }
            }

            throw new JsonSerializationException("Unexpected end when reading IDictionary<string, object>");
        }

        public override bool CanConvert(Type objectType) { return typeof(IDictionary<string, object>).GetTypeInfo().IsAssignableFrom(objectType); }
    }

}