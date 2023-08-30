using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MockMartianApi;
using MockMartianApi.Models;
using Newtonsoft.Json;

namespace MartianAppTests
{
    public class TestMarsApiResource
    {
        private readonly MarsApiResource apiResource = new MarsApiResource();
        private static readonly string MARTIAN_ENTITY = File.ReadAllText(@"resource/martianEntity.json");
        private static readonly string UPDATED_MARTIAN_ENTITY = File.ReadAllText(@"resource/updatedMartianEntity.json");
        private static readonly string NON_EXISTENT_ID = "Non-existent ID";
        private static readonly string UPDATE_404_MESSAGE = "Either you don't have permission or the martian entity you have requested to update does not exist in the database";
        private static readonly string RETRIEVAL_404_MESSAGE = "Either no entity exists with id [{0}] or user lacks the permissions to access the entity";
        private static readonly string DELETION_404_MESSAGE = "The entity you are trying to delete is either not present in the database or you lack permissions to execute the deletion";
        private static string FAILED_DESERIALISE_MESSAGE = "Failed to deserialise entity. Please submit a valid JSON body";

        [Fact]
        public void CallingAddEndpointWhenJsonIsInvalidShouldReturn500Response()
        {
            Assert.Equal(0, apiResource.CountOfEntities());
            var response = apiResource.CreateMartianEntity(null);

            Assert.Equal(0, apiResource.CountOfEntities());
            AssertCorrectResponse(response, "Null payload, please provide a valid MartianEntry in JsonFormat", 500);
        }

        [Fact]
        public void CallingAddEndpointWhenJsonIsValidShouldAddEntityToDbAndReturn200Response()
        {
            Assert.Equal(0, apiResource.CountOfEntities());
            var response = apiResource.CreateMartianEntity(MARTIAN_ENTITY);

            Assert.Equal(1, apiResource.CountOfEntities());
            AssertCorrectResponse(response, String.Format("Successfully created and uploaded entity to DB with id [{0}]", response.id), 200);
        }

        [Fact]
        public void CallingRetrieveEndpointWhenIdNotPresentReturn404Response()
        {
            var response = apiResource.RetrieveMartianEntity(NON_EXISTENT_ID, Clearance.TOP_LEVEL_CLEARANCE);
            AssertCorrectResponse(response, String.Format(RETRIEVAL_404_MESSAGE, NON_EXISTENT_ID), 404);
        }

        [Fact]
        public void CallingRetrieveEndpointWhenUserNotPermittedReturn404Response()
        {
            var responseContainingId = apiResource.CreateMartianEntity(MARTIAN_ENTITY);
            var response = apiResource.RetrieveMartianEntity(responseContainingId.id, Clearance.ACCESS_RESTRICTED);
            AssertCorrectResponse(response, String.Format(RETRIEVAL_404_MESSAGE, responseContainingId.id), 404);
        }

        [Fact]
        public void CallingRetrieveEndpointWhenIdIsPresentShouldRetrieveEntityFromDbAndReturn200Response() 
        {
            var originalEntity = JsonConvert.DeserializeObject<MartianEntity>(MARTIAN_ENTITY);

            var responseContainingId = apiResource.CreateMartianEntity(MARTIAN_ENTITY);
            var response = apiResource.RetrieveMartianEntity(responseContainingId.id, Clearance.TOP_LEVEL_CLEARANCE);
            AssertEntityValuesAreMatching(response, originalEntity);
        }

        [Fact]
        public void CallingUpdateEndpointWhenIdNonExistentShouldReturn404Response()
        {
            var response = apiResource.UpdateMartiaEntity(UPDATED_MARTIAN_ENTITY, NON_EXISTENT_ID, Clearance.TOP_LEVEL_CLEARANCE);
            AssertCorrectResponse(response, UPDATE_404_MESSAGE, 404);
        }

        [Fact]
        public void callingUpdateEndpointWhenUserNotClearedShouldReturn404Response() 
        {
            var responseContainingId = apiResource.CreateMartianEntity(MARTIAN_ENTITY);
            var response = apiResource.UpdateMartiaEntity(UPDATED_MARTIAN_ENTITY, responseContainingId.id, Clearance.ACCESS_RESTRICTED);
            AssertCorrectResponse(response, UPDATE_404_MESSAGE, 404);
        }

        [Fact]
        public void callingUpdateEndpointWhenJsonIsInvalidShouldReturn500Response()
        {
            var responseContainingId = apiResource.CreateMartianEntity(MARTIAN_ENTITY);
            var response = apiResource.UpdateMartiaEntity("Fake Json", responseContainingId.id, Clearance.TOP_LEVEL_CLEARANCE);
            AssertCorrectResponse(response, FAILED_DESERIALISE_MESSAGE, 500);
        }

        [Fact]
        public void callingUpdateEndpointWhenJsonIsValidShouldAddEntityToDbAndReturn200Response() 
        {
            var responseContainingId = apiResource.CreateMartianEntity(MARTIAN_ENTITY);
            var originalRetrievedEntity = JsonConvert.DeserializeObject<MartianEntity>(apiResource.RetrieveMartianEntity(responseContainingId.id, Clearance.TOP_LEVEL_CLEARANCE).message);

            var response = apiResource.UpdateMartiaEntity(UPDATED_MARTIAN_ENTITY, responseContainingId.id, Clearance.TOP_LEVEL_CLEARANCE);
            var updatedRetrievedEntry = JsonConvert.DeserializeObject<MartianEntity>(UPDATED_MARTIAN_ENTITY);

            AssertCorrectResponse(response, string.Format("Successfully updated martian entity in DB with id [{0}]", responseContainingId.id), 200);
            AssertEntityValuesAreNotMatching(originalRetrievedEntity, updatedRetrievedEntry);
        }

        [Fact]
        public void callingDeleteEndpointWhenIdIsInvalidShouldReturn404Response() 
        {
            var responseContainingId = apiResource.CreateMartianEntity(MARTIAN_ENTITY);
            var response = apiResource.DeleteMartianEntry(NON_EXISTENT_ID, Clearance.TOP_LEVEL_CLEARANCE);
            AssertCorrectResponse(response, DELETION_404_MESSAGE, 404);
        }

        [Fact]
        public void callingDeleteEndpointWhenClearanceIsInsufficientShouldReturn404Response() 
        {
            var responseContainingId = apiResource.CreateMartianEntity(MARTIAN_ENTITY);
            var response = apiResource.DeleteMartianEntry(responseContainingId.id, Clearance.ACCESS_RESTRICTED);
            AssertCorrectResponse(response, DELETION_404_MESSAGE, 404);
        }

        [Fact]
        public void callingDeleteEndpointWhenJsonIsValidShouldAddEntityToDbAndReturn200Response()
        {
            var responseContainingId = apiResource.CreateMartianEntity(MARTIAN_ENTITY);
            var response = apiResource.DeleteMartianEntry(responseContainingId.id, Clearance.TOP_LEVEL_CLEARANCE);
            AssertCorrectResponse(response, string.Format("Deleted martian entity from db with id [{0}]", responseContainingId.id), 200);
            var retrieveDeletedEntryResponse = apiResource.RetrieveMartianEntity(responseContainingId.id, Clearance.TOP_LEVEL_CLEARANCE);
            AssertCorrectResponse(retrieveDeletedEntryResponse, String.Format(RETRIEVAL_404_MESSAGE, responseContainingId.id), 404);
        }

        private void AssertCorrectResponse(Response response, string message, int statusCode)
        {
            response
                .Should()
                .BeEquivalentTo
                (new
                {
                     statusCode,
                    message
                });

        }

        private void AssertEntityValuesAreMatching(Response response, MartianEntity entityToCompareTo)
        {
            JsonConvert.DeserializeObject<MartianEntity>(response.message)
                .Should()
                .BeEquivalentTo
                (new
                {
                    entityToCompareTo.species,
                    entityToCompareTo.clearanceRequired
                });
        }

        private void AssertEntityValuesAreNotMatching(MartianEntity entity, MartianEntity entityToCompareTo)
        {
            entity
                .Should()
                .NotBeEquivalentTo
                (new
                {
                    entityToCompareTo.species,
                    entityToCompareTo.clearanceRequired
                });
        }
    }
}
