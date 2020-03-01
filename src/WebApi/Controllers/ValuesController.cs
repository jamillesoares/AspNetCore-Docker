using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Serilog;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : Controller
    {
        // Está sendo criado um escopo temporário de log com um tipo especifico. Será criado logs da classe ValuesController. 
        private readonly ILogger logger = Log.ForContext<ValuesController>();
        private readonly MongoClient mongoClient;
        private readonly IMongoDatabase db;

        public ValuesController()
        {
            logger.Verbose("ValuesController created");

            // Criando a conexão com o banco mongo
            var mongoClientSettings = new MongoClientSettings();
            mongoClient = new MongoClient(mongoClientSettings);
            db = mongoClient.GetDatabase(MongoDbConfiguration.DatabaseName);

            // Recupera a coleção de valores do banco da tabela values.           
            var c = db.GetCollection<IdValuePairType>("values");
            // Se a coleção estiver vazia ela será criada.
            if (c == null)
            {
                db.CreateCollection("values");
            }
        }

         [HttpGet]
        //Esse método irá retornar uma lista de objetos do tipo IdValuePairType
        public IEnumerable<IdValuePairType> Get()
        {
            // Registrando log.
            logger.Information("GET /api/values");
            var c = db.GetCollection<IdValuePairType>("values");
            return c.Find(Builders<IdValuePairType>.Filter.Empty).ToList();
        }

        [HttpGet("{id}")]
        //Esse método irá retornar um objeto do tipo IdValuePairType de acordo o parâmetro id informado
        public IdValuePairType Get(string id)
        {
            logger.Information($"GET /api/values/{id}");
            var c = db.GetCollection<IdValuePairType>("values");
            return c.Find(Builders<IdValuePairType>.Filter.Eq(x => x.id, id)).FirstOrDefault();
        }

        [HttpPost]
        // Esse método irá gravar o objeto valueEnvelope que foi passado como parâmetro
        public void Post([FromBody]ValueType valueEnvelope)
        {
            var v = valueEnvelope?.value;
            //Esse comando está criando um log de informação
            logger.Information($"POST /api/values {v}");
            var id = Guid.NewGuid().ToString();
            var c = db.GetCollection<IdValuePairType>("values");
            var d = new IdValuePairType(id, v);
            c.InsertOne(d);
        }

         [HttpPut("{id}")]
        // Esse método irá atualizar o registro no banco que tenha o mesmo id que o objeto enviado como parâmetro
        public void Put(string id, [FromBody]ValueType valueEnvelope)
        {
            var v = valueEnvelope?.value;
            logger.Information($"PUT /api/values {v}");
            var c = db.GetCollection<IdValuePairType>("values");
            c.UpdateOne(Builders<IdValuePairType>.Filter.Eq(x => x.id, id), Builders<IdValuePairType>.Update.Set(x => x.value, v));
        }

        [HttpDelete("{id}")]
        // Esse método irá deletar o registro no banco que tenha o id igual ao que foi enviado como parâmetro
        public void Delete(string id)
        {
            logger.Information($"DELETE /api/values/{id}");
            var c = db.GetCollection<IdValuePairType>("values");
            c.DeleteOne(Builders<IdValuePairType>.Filter.Eq(x => x.id, id));
        }

        public class ValueType
        {
            public string value { get; private set;}

            public ValueType(string value)
            {
                this.value = value;
            }
        }

        public class IdValuePairType
        {
            public string id { get; private set;}
            public string value { get; private set;}

            public IdValuePairType(string id, string value)
            {
                this.id = id;
                this.value = value;
            }
        }
        
    }
}
