using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MassTransitTest
{
    public class QueryObject
    {
        [JsonProperty("a", Required = Required.Always)]
        public string Plate { get; set; }

        [JsonProperty("b", Required = Required.Always)]
        public int CamId { get; set; }
    }
}
