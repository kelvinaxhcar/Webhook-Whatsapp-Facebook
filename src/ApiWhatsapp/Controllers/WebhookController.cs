using ApiWhatsapp.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;

namespace ApiWhatsapp.Controllers
{
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {

        [HttpGet]
        [Route("webhooks")]
        public string WebhookAutenticacao()
        {
            var challenge = Request.Query["hub.challenge"];
            return challenge;
        }

        [HttpPost]
        [Route("webhooks")]
        public async Task<OkResult> WebhookRecepcao()
        {
            Request.EnableBuffering();
            Request.Body.Position = 0;
            var rawRequestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            var res = JsonConvert.DeserializeObject<Root>(rawRequestBody);

            var message = await ObterMensagem(res);
            if (message != null)
            {
                var thread = new Thread(() => Executar(res));
                thread.Start();
            }
            
            return Ok();
        }

        private async void Executar(Root res)
        {
            var message = await ObterMensagem(res);
            Console.WriteLine(message);
            var numero = await ObterNumero(res);
            var RequestsWhatSapp = new RequestsWhatSapp();
            if (message != null)
            {
                await RequestsWhatSapp.EnviarMensagem("teste", numero, "TEXT");
            }
        }

        private async Task<string> ObterMensagem(Root res)
        {
            if (res.entry.First().changes.First().value.messages != null)
            {
                if (res.entry.First().changes.First().value.messages.First().type == "text")
                {
                    return res.entry.First().changes.First().value.messages.First().text.body;
                }
            }

            return null;
        }

        private async Task<string> ObterNumero(Root res)
        {
            if (res.entry.First().changes.First().value.messages != null)
            {
                if (res.entry.First().changes.First().value.messages.First().type == "text")
                {
                    return res.entry.First().changes.First().value.contacts.First().wa_id.ToString();
                }
            }

            return null;
        }

        //private async Task<string> ExecutarOpenIA(string mensagem)
        //{
        //    var openAiService = new OpenAIService(new OpenAiOptions()
        //    {
        //        ApiKey = "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",
                
        //    });

        //    var completionResult = await openAiService.Completions.CreateCompletion(new CompletionCreateRequest()
        //    {
        //        Prompt = mensagem,
        //        Model = Models.TextDavinciV3,
        //        MaxTokens= 100,
        //    });

        //    var mensagemDeRetorno = string.Empty;

        //    if (completionResult.Successful)
        //    {
        //        mensagemDeRetorno = completionResult.Choices.FirstOrDefault().Text;
        //    }
        //    else
        //    {

        //        mensagemDeRetorno = $"Erro {completionResult.Error.Code}: {completionResult.Error.Message}";
        //    }

        //    return mensagemDeRetorno;
        //}
    }
}
