using MockMartianApi.Database;
using MockMartianApi.Models;
using Newtonsoft.Json;
using System.Web.Http;

namespace MockMartianApi
{

    internal class MarsApiResource
    {
        private DatabaseInstance database = new DatabaseInstance();

        [Route("/retrieve/{id}/{clearance}")]
        public Response RetrieveMartianEntity(string id, Clearance clearance)
        {
            var entity = database.RetrieveMartian(id, clearance);
            return new Response(JsonConvert.SerializeObject(entity), 200);
        }

        [Route("/uploadEntity")]
        public Response CreateMartianEntity(string payload)
        {
            var deserialisedEntity = JsonConvert.DeserializeObject<MartianEntity>(payload);
            if (deserialisedEntity == null)
            {
                return new Response("Failed to deserialise entity", 500);
            }
            var id = database.AddMartian(deserialisedEntity.species, deserialisedEntity.clearanceRequired);
            return new Response(String.Format("Successfully created and uploaded entity to DB with id [%s]", id), 200);
        }

        [Route("api/[controller]")]
        public Response DeleteMartianEntry(string id, Clearance clearance)
        {
            var entityDeleted = database.DeleteMartian(id, clearance);
            if (!entityDeleted)
            {
                return new Response("Martian entity with this id not found in database", 404);
            }
            return new Response(String.Format("Deleted martian entity from db with id [%s]", id), 200);
        }

        [Route("/modifyEntity/{id}")]
        public Response UpdateMartiaEntity(string payload, string id, Clearance clearance)
        {
            var deserialisedEntity = JsonConvert.DeserializeObject<MartianEntity>(payload);
            if (deserialisedEntity == null)
            {
                return new Response("Failed to deserialise martian entity", 500);
            }

            var retrievedEntity = database.RetrieveMartian(id, clearance);

            if (retrievedEntity == null)
            {
                return new Response("You don't have permission or the martian entity you have requested to update does not exist in the database", 404);
            }

            if (!database.UpdateMartian(id, deserialisedEntity))
            {
                return new Response(String.Format("The entity you're trying to update with id [%s] no longer exists in the database", id), 404);
            }
            return new Response(String.Format("Successfully created and uploaded martian entity to DB with id [%s]", id), 200);
        }
    }
}
