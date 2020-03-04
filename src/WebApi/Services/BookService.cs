using WebApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Services
{
    public class BookService
    {
        private readonly IMongoCollection<Book> _books;

        // uma IBookstoreDatabaseSettings instância é recuperada do DI via injeção de construtor. Essa técnica fornece acesso aos valores de configuração appsettings.json que foram adicionados na seção.
        public BookService(IBookstoreDatabaseSettings settings)
        {
            //MongoClient - Lê a instância do servidor para executar operações do banco de dados. O construtor desta classe é fornecido com a cadeia de conexão MongoDB.
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _books = database.GetCollection<Book>(settings.BooksCollectionName);
        }

        //Find <TDocument> - Retorna todos os documentos da coleção que correspondem aos critérios de pesquisa fornecidos.
        public List<Book> Get() =>
            _books.Find(book => true).ToList();

        
        public Book Get(string id) =>
            _books.Find<Book>(book => book._Id == id).FirstOrDefault();

        public Book Create(Book book)
        {
            //InsertOne - insere o objeto fornecido como um novo documento na coleção.
            _books.InsertOne(book);
            return book;
        }

        //ReplaceOne - Substitui o documento único que corresponde aos critérios de pesquisa fornecidos pelo objeto fornecido.
        public void Update(string id, Book bookIn) =>
            _books.ReplaceOne(book => book._Id == id, bookIn);

        public void Remove(Book bookIn) =>
            _books.DeleteOne(book => book._Id == bookIn._Id);

        //DeleteOne - Exclui um único documento que corresponde aos critérios de pesquisa fornecidos.
        public void Remove(string id) => 
            _books.DeleteOne(book => book._Id == id);
    }
}