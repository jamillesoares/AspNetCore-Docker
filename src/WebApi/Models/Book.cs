using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace WebApi.Models
{
    public class Book
    {
        //para designar essa propriedade como chave primária do documento.
        [BsonId]
        //para permitir a passagem do parâmetro como tipo em string vez de uma estrutura ObjectId . O Mongo lida com a conversão de string para ObjectId.
        [BsonRepresentation(BsonType.ObjectId)]
        public string _Id { get; set; }
        //atributo O valor do atributo de Name representa o nome da propriedade na coleção MongoDB.
        [BsonElement("Name")]
        [JsonProperty("Name")]
        public string BookName { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        public string Author { get; set; }

        public Book(string id, string bookName, decimal price, string category, string author){
            this._Id = id;
            this.BookName = bookName;
            this.Price = price;
            this.Category = category;
            this.Author = author;
        }
    }
}