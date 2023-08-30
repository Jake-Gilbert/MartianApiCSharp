using MockMartianApi.Database;
using MockMartianApi.Models;
using Newtonsoft.Json;
using System.Web.Http;

namespace MockMartianApi
{
    public class MarsApiResource
    {
        private DatabaseInstance database = new DatabaseInstance();

        [Route("/retrieve/{id}/{clearance}")]
        public Response RetrieveMartianEntity(string id, Clearance clearance)
        {
            var entity = database.RetrieveMartian(id, clearance);
            if (entity == null)
            {
                //For security reasons only return 404. Returning 403 implies a resource exists.
                return new Response(String.Format("Either no entity exists with id [{0}] or user lacks the permissions to access the entity", id), 404);
            }
            return new Response(JsonConvert.SerializeObject(entity), 200);
        }

        [Route("/uploadEntity")]
        public Response CreateMartianEntity(string? payload)
        {
            if (payload == null)
            {
                return new Response("Null payload, please provide a valid MartianEntry in JsonFormat", 500);
            }
            var deserialisedEntity = JsonConvert.DeserializeObject<MartianEntity>(payload);
            if (deserialisedEntity == null)
            {
                return new Response("Failed to deserialise entity. Please submit a valid JSON body", 500);
            }
            var id = database.AddMartian(deserialisedEntity.species, deserialisedEntity.clearanceRequired);
            return new Response(String.Format("Successfully created and uploaded entity to DB with id [{0}]", id), 200, id);
        }

        [Route("/deleteEntity/{id}/{clearance}")]
        public Response DeleteMartianEntry(string id, Clearance clearance)
        {
            var entityDeleted = database.DeleteMartian(id, clearance);
            if (!entityDeleted)
            {
                return new Response("The entity you are trying to delete is either not present in the database or you lack permissions to execute the deletion", 404);
            }
            return new Response(string.Format("Deleted martian entity from db with id [{0}]", id), 200);
        }

        [Route("/modifyEntity/{id}")]
        public Response UpdateMartiaEntity(string payload, string id, Clearance clearance)
        {
            var deserialisedEntity = TryDeserialisingEntity(payload);
            if (deserialisedEntity == null)
            {
                return new Response("Failed to deserialise entity. Please submit a valid JSON body", 500);
            }

            var retrievedEntity = database.RetrieveMartian(id, clearance);

            if (retrievedEntity == null)
            {
                return new Response("Either you don't have permission or the martian entity you have requested to update does not exist in the database", 404);
            }

            if (!database.UpdateMartian(id, deserialisedEntity))
            {
                return new Response(String.Format("The entity you're trying to update with id [{0}] no longer exists in the database", id), 404);
            }
            return new Response(String.Format("Successfully updated martian entity in DB with id [{0}]", id), 200);
        }

        private MartianEntity? TryDeserialisingEntity(string payload)
        {
            try
            {
                return JsonConvert.DeserializeObject<MartianEntity>(payload);
            }
            catch
            {
                return null;
            }
        }

        [Route("/count")]
        public int CountOfEntities()
        {
            return database.Count();
        }
    }
}
