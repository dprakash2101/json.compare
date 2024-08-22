using json.compare.BusinessLogic.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace json.compare.BusinessLogic.Services
{
    public interface IJsonCompareService
    {
        List<Json_Difference> CompareJson(JsonCompareRequest jsonCompare);
    }

    public class JsonCompareService : IJsonCompareService
    {
        public List<Json_Difference> CompareJson(JsonCompareRequest jsonCompare)
        {
            var differences = new List<Json_Difference>();
            JObject jObj1, jObj2;

            try
            {
                jObj1 = JObject.Parse(jsonCompare.Json1); // Parse JSON strings to JObject
                jObj2 = JObject.Parse(jsonCompare.Json2); // Parse JSON strings to JObject
            }
            catch (JsonException)
            {
                // Handle parsing errors if necessary
                return differences; // Or throw an exception if preferred
            }

            CompareTokens(jObj1, jObj2, differences, "");
            return differences;
        }

        private void CompareTokens(JToken token1, JToken token2, List<Json_Difference> differences, string path)
        {
            if (!JToken.DeepEquals(token1, token2))
            {
                differences.Add(new Json_Difference
                {
                    Path = path,
                    Value1 = token1.ToString(),
                    Value2 = token2.ToString()
                });

                if (token1.Type == JTokenType.Object && token2.Type == JTokenType.Object)
                {
                    var obj1 = (JObject)token1;
                    var obj2 = (JObject)token2;

                    var keys = new HashSet<string>(obj1.Properties().Select(p => p.Name));
                    keys.UnionWith(obj2.Properties().Select(p => p.Name));

                    foreach (var key in keys)
                    {
                        CompareTokens(obj1[key], obj2[key], differences, $"{path}/{key}");
                    }
                }
            }
        }
    }
}
