using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace WebApi
{
    public class Startup
    {
        // :Interface essencial para configurarmos nossos serviços por tipo de ambiente
        public Startup(IHostingEnvironment env)
        {
            // Está sendo criado uma nova instância de ConfigurationBuilder, informando quais providers será usado como fonte de configuração.
            // SetBasePath: está sendo informado o caminho do diretório onde os arquivos de providers estão.
            // AddJsonFile(): está sendo adicionado o arquivo provider de configuração. E quando o arquivo for atualizado deve ser recarregado.
            // AddEnvironmentVariables(): está sendo adicionado as variáveis de ambiente prefixadas.
            // Build()
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            // Criando uma função que irá buscar o valor da configuração passada como parâmetro.
            Func<string, string> settingsResolver = (name) => Configuration[name];

            // Instanciando a classe de log
            var loggingLevelSwitch = new LoggingLevelSwitch();
            //Configurando a API de log na aplicação
            Log.Logger = Infrastructure.Logging.ApplicationLogging.CreateLogger(settingsResolver, "docker-dotnetcore-webapi", loggingLevelSwitch, "./logs");

            // Configurando o MongoDb na aplicação
            MongoDbConfiguration.ServerAddress = settingsResolver("MongoDb.ServerAddress");
            MongoDbConfiguration.ServerPort = int.Parse(settingsResolver("MongoDb.ServerPort"));
            MongoDbConfiguration.DatabaseName = settingsResolver("MongoDb.DatabaseName");
            MongoDbConfiguration.UserName = settingsResolver("MongoDb.UserName");
            MongoDbConfiguration.UserPassword = settingsResolver("MongoDb.UserPassword");

            // Comando para escrever no arquivo de log as configurações do MongoDb.
            Log.Information($"WebAPI MongoDb: Server {MongoDbConfiguration.ServerAddress}:{MongoDbConfiguration.ServerPort}/{MongoDbConfiguration.DatabaseName}");
        }

        public IConfigurationRoot Configuration { get; }
        
        //IServiceCollection : servirá para injetarmos nossos próprios services ou qualquer “coisa” que usaremos no longo de nossa aplicação.
        public void ConfigureServices(IServiceCollection services)
        {
            //SetCompatibilityVersion – Necessário para manter a compatibilidade entre versões de framework.
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        
        // Este método será usado para configurar o pipeline de solicitação HTTP.        
        // IHostingEnvironment: Interface essencial para configurarmos nossos serviços por tipo de ambiente
        // IApplicationBuilder: é quem encapsula todos os tratamentos do Pipeline.
        // ILoggerFactory: Interface responsável em criar logs na aplicação (funciona em conjunto com com os providers)
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                //UseDeveloperExceptionPage: Ele captura qualquer exception e formata em um html antes de responder ao cliente. 
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // UseHsts: Envia cabeçalho HTTP Strict Transport Security Protocol (HSTS) aos clientes.
                app.UseHsts();
            }

            /// UseHttpsRedirection: redireciona solicitações HTTP para HTTPS.
            //app.UseHttpsRedirection();
            // UseMvc: Adiciona MVC ao pipeline de execução da aplicação.
            app.UseMvc();
        }
    }
}
