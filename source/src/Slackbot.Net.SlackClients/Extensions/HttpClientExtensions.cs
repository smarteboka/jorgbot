using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Slackbot.Net.SlackClients.Exceptions;
using Slackbot.Net.SlackClients.Models.Responses;

namespace Slackbot.Net.SlackClients.Extensions
{
    internal static class HttpClientExtensions
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver()};

        public static async Task<T> PostJson<T>(this HttpClient httpClient, object payload, string api, Action<string> logger = null) where T:Response
        {
            var serializedObject = JsonConvert.SerializeObject(payload, JsonSerializerSettings);
            var httpContent = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, api)
            {
                Content = httpContent
            };

            var response =  await httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            logger?.Invoke($"{response.StatusCode} - ${responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                throw new SlackApiException($"Status code {response.StatusCode} \n {responseContent}");
            }
            
            var resObj = JsonConvert.DeserializeObject<T>(responseContent, JsonSerializerSettings);
            
            if(!resObj.ok)
                throw new SlackApiException($"{resObj.error}");
            
            return resObj;
        }
        
        public static async Task<T> PostParametersAsForm<T>(this HttpClient httpClient, IEnumerable<KeyValuePair<string, string>> parameters, string api, Action<string> logger = null) where T: Response
        {
            var formUrlEncodedContent = new FormUrlEncodedContent(parameters);

            var requestContent = await formUrlEncodedContent.ReadAsStringAsync();
            var httpContent = new StringContent(requestContent, Encoding.UTF8, "application/x-www-form-urlencoded");
            httpContent.Headers.ContentType.CharSet = string.Empty;

            var request = new HttpRequestMessage(HttpMethod.Post, api)
            {
                Content = httpContent
            };

            var response =  await httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            logger?.Invoke($"{response.StatusCode} \n {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                throw new SlackApiException($"Status code {response.StatusCode} \n {responseContent}");
            }
            
            var resObj = JsonConvert.DeserializeObject<T>(responseContent, JsonSerializerSettings);
            
            if(!resObj.ok)
                throw new SlackApiException($"{resObj.error}");
            
            return resObj;        
        }
    }
}