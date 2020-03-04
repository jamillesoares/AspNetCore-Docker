namespace WebApi.Models
{
    //A BookstoreDatabaseSettingsclasse anterior é usada para armazenar os valores de propriedade do arquivo appsettings.jsonBookstoreDatabaseSettings . Os nomes de propriedade JSON e C # são nomeados de forma idêntica para facilitar o processo de mapeamento.
    public class BookstoreDatabaseSettings : IBookstoreDatabaseSettings
    {
        public string BooksCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IBookstoreDatabaseSettings
    {
        string BooksCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}