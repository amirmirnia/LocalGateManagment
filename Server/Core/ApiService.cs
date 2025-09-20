using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;


namespace ServicesGateManagment.Server.Handlers
{


    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // متد GET که پاسخ خام (JSON) برمی‌گرداند
        public async Task<string> GetRawAsync(string endpoint)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        // متد GET که مستقیماً به نوع T تبدیل می‌کند
        public async Task<T> GetAsync<T>(string endpoint)
        {
            var jsonContent = await GetRawAsync(endpoint);
            return Deserialize<T>(jsonContent);
        }

        // متد GET که HttpResponseMessage برمی‌گرداند (برای انعطاف‌پذیری بیشتر)
        public async Task<HttpResponseMessage> GetRawResponseAsync(string endpoint)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return response;
        }

        // متد POST که در پاسخ به سؤال قبلی اضافه شده
        public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string endpoint, T data)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync(endpoint, data);
            return response;
        }

        public async Task<string> PostRawAsync(string endpoint, object data)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            var jsonContent = await PostRawAsync(endpoint, data);
            return Deserialize<T>(jsonContent);
        }

        // متد کمکی برای تبدیل JSON به نوع دلخواه
        public static T Deserialize<T>(string jsonContent)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<T>(jsonContent, options);
        }
    }
}
