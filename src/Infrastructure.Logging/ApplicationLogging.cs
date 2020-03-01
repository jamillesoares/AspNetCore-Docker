using System;
using Serilog;
using Serilog.Core;

namespace Infrastructure.Logging
{
    public static class ApplicationLogging
    {
        //Montando o template do log.
        private const string OutputTemplateFormat = "{{Timestamp:yyyy-MM-dd HH:mm:ss.fff}} [{0,18} ] [{{Level,12}}] [{{SourceContext}}] {{Message}}{{NewLine}}{{Exception}}";

        // Método que irá criar o log da aplicação no meu console.
        public static ILogger CreateLogger(
            Func<string, string> settingsResolver,
            string serviceName,
            LoggingLevelSwitch loggingLevelSwitch,
            string bufferFileLocation,
            params IDestructuringPolicy[] destructuringPolicies)
        {
            if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            if (loggingLevelSwitch == null) throw new ArgumentNullException(nameof(loggingLevelSwitch));

            string outputTemplate = string.Format(OutputTemplateFormat, serviceName);

            
            var loggerConfiguration = ConfigurationFactory.CreateConfiguration(
                settingsResolver: settingsResolver,
                serviceName: serviceName,
                loggingLevelSwitch: loggingLevelSwitch,
                bufferFileFolderLocation: bufferFileLocation);

            if (destructuringPolicies.Length > 0)
            {
                loggerConfiguration = loggerConfiguration.Destructure.With(destructuringPolicies);
            }

            //Criando disparador de log no console usando a configuração especificada
            return loggerConfiguration
                .WriteTo.ColoredConsole(levelSwitch: loggingLevelSwitch, outputTemplate: outputTemplate)
                .CreateLogger();
        }
    }
}
