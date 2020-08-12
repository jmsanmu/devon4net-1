﻿using System.Text.Json.Serialization;

namespace Devon4Net.Infrastructure.SmaxHcm.Dto.Designer
{
    public class GetDesignTagsResponseDto
    {
        [JsonPropertyName("@type")]
        public string type { get; set; }

        [JsonPropertyName("@self")]
        public string self { get; set; }

        [JsonPropertyName("@total_results")]
        public int total_results { get; set; }

        [JsonPropertyName("@start_index")]
        public int start_index { get; set; }

        [JsonPropertyName("@items_per_page")]
        public int items_per_page { get; set; }

        public GetDesignTagsResponseDto_Member[] members { get; set; }
    }

    public class GetDesignTagsResponseDto_Member
    {
        [JsonPropertyName("@self")]
        public string self { get; set; }

        [JsonPropertyName("@type")]
        public string type { get; set; }

        [JsonPropertyName("@content_version")]
        public int content_version { get; set; }

        public string name { get; set; }
        public GetDesignTagsResponseDto_Ext ext { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public string color { get; set; }
        public string[] scopes { get; set; }
    }

    public class GetDesignTagsResponseDto_Ext
    {
        public string csa_name_key { get; set; }
    }
}
