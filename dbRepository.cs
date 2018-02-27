using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using LuisBot.Models;

namespace LuisBot
{
    public static class dbRepository
    {
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
        private static DocumentClient client;


        public static async Task<IList<Receta>> GetRecetasAsync(string collectionId)
        {
            IDocumentQuery<Receta> query = client.CreateDocumentQuery<Receta>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionId),
                new FeedOptions { MaxItemCount = -1 })
                .AsDocumentQuery();

            List<Receta> results = new List<Receta>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<Receta>());
            }

            return results;
        }

        public static void Initialize()
        {
            client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]), ConfigurationManager.AppSettings["authKey"]);
            //CreateDatabaseIfNotExistsAsync().Wait();
            //CreateCollectionIfNotExistsAsync().Wait();
        }
    }
}