using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace WebApi
{
    //Essa classe é responsável pela inicialização da aplicação através da criação/compilação do host
    public class Program
    {
         //O método Main recebe uma lista de argumentos. 
        public static void Main(string[] args)
        {
            ///.UseContentRoot(Directory.GetCurrentDirectory()): é um método que especifica o diretório atual como um diretório raiz.
           ///.Build(): método que retorna uma instância do IWebHost usando a configuração especificada acima.
            /// Run(): método inicia o aplicativo Web e bloqueia o encadeamento de chamada até que o host seja encerrado.
            CreateWebHostBuilder(args)
            //.UseUrls("http://*:5000")
            //.UseContentRoot(Directory.GetCurrentDirectory())
            .Build()
            .Run();
        }

        //Método responsável em criar nosso host
        //A classe WebHost usar o Kestrel como servidor web, habilitar integração no IIS, carregar variáveis de ambiente, configurações do appsettings.json e saída de logs para alguns providers.
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
