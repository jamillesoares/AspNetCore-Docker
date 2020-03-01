using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;

namespace Client.Random
{
    class Program
    {
        private static ILogger logger;
        private static readonly Command[] allCommands;
        private static string id;

        static Program()
        {
            var list = new List<Command>();
            foreach (var c in Enum.GetValues(typeof(Command)))
            {
                list.Add((Command)c);
            };
            //lista dos tipos de métodos http 
            allCommands = list.ToArray();            
        }

        static void Main(string[] args)
        {
            //Carrega configurações do appsettings
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = configurationBuilder.Build();
            // Criando uma função que irá buscar o valor da configuração passada como parâmetro.
            Func<string, string> settingsResolver = (name) => configuration[name];

            // Criando um arquivo de log com os eventos(Comandos) que estão acontecendo no meu projeto.
            var loggingLevelSwitch = new LoggingLevelSwitch();
            Log.Logger = Infrastructure.Logging.ApplicationLogging.CreateLogger(settingsResolver, "docker-dotnetcore-client", loggingLevelSwitch, "./logs");            
            logger = Log.ForContext<Program>();

            var apiUrl = settingsResolver("ApiUrl");
            var maxDelay = TimeSpan.Parse(settingsResolver("MaxDelay"));
            var maxDelayMs = (int)maxDelay.TotalMilliseconds;
            //instanciando a classe de gerador de números aleatórios.
            var rnd = new System.Random();


            logger.Information($"REST API Random Test Client. API Url: {apiUrl}");
            var apiClient = new HttpClient();

            while (true)
            {
                // Chamando o método responsável por retornar o tipo de método http que será executando no momento.
                var c = GetRandomCommand();
                // Escrevendo log de evento Debug
                logger.Debug($"Processing command {c}");
                //Chamando o método responsável para montar a requisição http passando o método e a URL da aplicação, retornando assim o endereço completo da requisição. 
                var request = GetRequest(c, apiUrl);
                try
                {
                    //Registra no log uma informação: o método da requisição que está sendo executado no momento e a URL
                    logger.Information($"{request.Method} {request.RequestUri}");
                    // Está sendo enviado a requisição assincrona e armazenando o resultado na variável
                    var response = apiClient.SendAsync(request).Result;
                    // Registra no log o status da requisição
                    logger.Debug($"{response.StatusCode}");
                    //Suspendendo a thread pelo tempo espeficicado 
                    // o método Next gera um número aleatório até o maxDelayMs.
                    Thread.Sleep(rnd.Next(maxDelayMs));
                }
                catch (Exception ex)
                {
                    //Registra no log o erro ocorrido no momento da execução da requisição
                    logger.Error(ex, $"Failed to process command {c}");
                }
            }
        }

        // Método responsável por instânciar a classe HttpRequestMessage com os parâmetros necessários para montar uma requisão HTTP
        private static HttpRequestMessage GetRequest(Command c, string apiUrl)
        {
            HttpRequestMessage request;
            switch (c)
            {
                case Command.GetAll:
                    request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}/values");
                    break;

                case Command.Add:
                    request = new HttpRequestMessage(HttpMethod.Post, $"{apiUrl}/values") { Content = new StringContent($"{{ \"value\":\"{DateTime.UtcNow}\" }}", Encoding.UTF8, "application/json") };
                    break;

                case Command.GetById:
                    request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}/values/{id}");
                    break;

                case Command.SetById:
                    request = new HttpRequestMessage(HttpMethod.Put, $"{apiUrl}/values/{id}") { Content = new StringContent($"{{ \"value\":\"{DateTime.UtcNow}\" }}", Encoding.UTF8, "application/json") };
                    break;

                case Command.DeleteById:
                    request = new HttpRequestMessage(HttpMethod.Delete, $"{apiUrl}/values/{id}");
                    id = null;
                    break;

                default:
                    Console.WriteLine($"Command {c} not supported");
                    request = new HttpRequestMessage(HttpMethod.Options, $"{apiUrl}");
                    break;
            }

            return request;
        }

        private static void PrintResponse(HttpResponseMessage response, HttpMethod method, Uri requestUri)
        {
            Console.WriteLine("");
            Console.WriteLine($"RESPONSE ({method} {requestUri}):");
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            Console.WriteLine("");
        }

        
        private static Command GetRandomCommand()
        {
            Command? c;

            do
            {
                c = allCommands.TakeRandom();
                switch (c.Value)
                {
                    case Command.GetAll:
                        break;

                    case Command.Add:
                        id = Guid.NewGuid().ToString();
                        break;

                    case Command.DeleteById:
                    case Command.GetById:
                    case Command.SetById:
                        if (string.IsNullOrEmpty(id)) c = null;
                        break;

                    default:
                        break;
                }
            } while (!c.HasValue);

            return c.Value;
        }

        private enum Command
        {
            GetAll,
            Add,
            GetById,
            SetById,
            DeleteById
        }
    }
}

