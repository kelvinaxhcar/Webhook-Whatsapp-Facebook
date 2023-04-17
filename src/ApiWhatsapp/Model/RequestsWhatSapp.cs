using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ApiWhatsapp.Model
{
    public class RequestsWhatSapp
    {
        private readonly string identificacaoDoNumeroDeTelefone = "113870918259188";
        private readonly string token = "EAAPgyvmKHywBAEA79iB3Xa0vWO02CPZAfvtXJabn5thCJpXKIRKKfA6nZBc3LttQjFRaAahwFAHtznhMF0SBv83TRokBNa7BZB0muubBTlgZCpTttwZBMjMiEPjy0EGZAbb9RztNa3f4rD1uyRzyLX0qD0W369uEslzS2boZCsbZBKzJstcZBbfdk";

        public async Task<string> EnviarRequisicao(string data, string phoneNumberID)
        {
            var url = $"https://graph.facebook.com/v13.0/{phoneNumberID}/messages";

            using var client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var result = await client.PostAsync("", content);
            return await result.Content.ReadAsStringAsync();
        }

        public async Task<string> EnviarMensagem(string mensagem, string numero, string tipo)
        {
            var nu = numero[4..];
            var prefixo = numero[..4];
            var numeroCompleto = $"{prefixo}9{nu}";
            var data = new Dictionary<string, string>()
            {
                {"messaging_product","whatsapp" },
                {"to",numeroCompleto },
                {"type",tipo },
                {"text",  JsonConvert.SerializeObject(new Dictionary<string, string>(){{ "body", mensagem}})},
            };

            return await EnviarRequisicao(JsonConvert.SerializeObject(data), identificacaoDoNumeroDeTelefone);
        }
    }
}
