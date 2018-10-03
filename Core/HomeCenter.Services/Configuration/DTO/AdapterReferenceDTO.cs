﻿using HomeCenter.Model.Core;
using Newtonsoft.Json;
using Proto;
using System.Collections.Generic;

namespace HomeCenter.Services.Configuration.DTO
{
    public class AdapterReferenceDTO
    {
        [JsonProperty("Uid")]
        public string Uid { get; set; }

        [JsonProperty("Properties")]
        [JsonConverter(typeof(PropertyDictionaryConverter))]
        public Dictionary<string, Property> Properties { get; set; }

        [JsonProperty("Tags")]
        public IDictionary<string, string> Tags { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        public PID ID { get; set; }
    }
}