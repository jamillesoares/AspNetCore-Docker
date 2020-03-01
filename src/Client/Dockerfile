
# define a imagem base
FROM microsoft/dotnet:2.2-aspnetcore-runtime
# define nosso diretório de trabalho na pasta do aplicativo, dentro do container que estamos construindo.
WORKDIR /app
# expõe a porta informada do container
EXPOSE 5000
# Copia arquivos do ambiente para o container
COPY _publish .
COPY appsettings.json .
# executa os comandos dentro do diretório logs
RUN mkdir ./logs
# Define o aplicativo padrão usado toda vez que um contêiner é criado a partir da imagem
ENTRYPOINT ["dotnet", "Client.dll"]
