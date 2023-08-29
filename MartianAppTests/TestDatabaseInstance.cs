using FluentAssertions;
using MockMartianApi.Database;
using MockMartianApi.Models;

namespace MartianAppTests
{
    public class TestDatabaseInstance
    {
        private readonly DatabaseInstance database = new DatabaseInstance();
        private readonly Dictionary<string, string> entityAndIdMap = new Dictionary<string, string>();
        private readonly MartianEntity xenomorph = new MartianEntity(XENOMORPH, Clearance.TOP_LEVEL_CLEARANCE);
        private readonly MartianEntity updatedXenomorph = new MartianEntity(String.Format("New and improved %s", XENOMORPH), Clearance.ADVANCED_CLEARANCE);
        private readonly MartianEntity fredTheMartian = new MartianEntity("Fred the Martian", Clearance.MINIMAL_CLEARANCE);
        private readonly MartianEntity theArbiter = new MartianEntity("The Arbiter", Clearance.ADVANCED_CLEARANCE);
        private static readonly string XENOMORPH = "Xenomorph";
        private static readonly string NON_EXISTENT_ID = "Non-existent ID";

        public TestDatabaseInstance()
        {
            entityAndIdMap.Add(XENOMORPH, database.AddMartian(XENOMORPH, xenomorph.clearanceRequired));
            entityAndIdMap.Add(fredTheMartian.species, database.AddMartian(fredTheMartian.species, fredTheMartian.clearanceRequired));
            entityAndIdMap.Add(theArbiter.species, database.AddMartian(theArbiter.species, theArbiter.clearanceRequired));
        }

        [Fact]
        public void AttemptingToAddEntityToDbWhenSpeciesIsNullShouldReturnNull()
        {
            var generatedId = database.AddMartian(null, Clearance.ACCESS_RESTRICTED);
            Assert.True(generatedId == null);
        }

        [Fact]
        public void AttemptingToAddEntityToDbWhenClearanceIsNullShouldReturnNull()
        {
            var generatedId = database.AddMartian("Test", null);
            Assert.True(generatedId == null);
        }

        [Fact]
        public void AttemptingToAddEntityToDbWhenUserAuthorisedShouldResultInSuccess()
        {
            Assert.Equal(3, database.Count());
            var generatedId = database.AddMartian("Test", Clearance.MINIMAL_CLEARANCE);
            Assert.NotNull(generatedId);
            Assert.Equal(4, database.Count());
        }

        [Fact]
        public void AttemptingToRetrieveEntityWhichExistsIenDbAndUserAuthoriseToAccessShouldReturnEntity()
        {
            var retrievedEntity = database.RetrieveMartian(entityAndIdMap[XENOMORPH], Clearance.TOP_LEVEL_CLEARANCE);
            retrievedEntity.Should()
                .BeEquivalentTo
                (new
                {
                    species = XENOMORPH,
                    clearanceRequired = Clearance.TOP_LEVEL_CLEARANCE
                });
        }

        [Fact]
        public void AttemptingToRetrieveEntityWhichDoesNotExistInDbShouldReturnNull()
        {
            var retrievedEntity = database.RetrieveMartian(NON_EXISTENT_ID, Clearance.ACCESS_RESTRICTED);
            Assert.Null(retrievedEntity);
        }

        [Fact]
        public void AttemptingToRetrieveEntityWhichExistsInDbButUserNotAuthorisedShouldReturnNull()
        {
            var retrievedEntity = database.RetrieveMartian(entityAndIdMap["Xenomorph"], Clearance.ACCESS_RESTRICTED);
            Assert.Null(retrievedEntity);
        }

        [Fact]
        public void AttemptingToUpdateEntityInDbWhenIdNotPresentInDbShouldReturnFalse()
        {
            var hasBeenUpdated = database.UpdateMartian(NON_EXISTENT_ID, updatedXenomorph);
            Assert.False(hasBeenUpdated);
        }

        [Fact]
        public void AttemptingToUpdateEntityInDbWhenIdPresentInDbShouldReturnTrue()
        {
            var hasBeenUpdated = database.UpdateMartian(entityAndIdMap[XENOMORPH], updatedXenomorph);
            Assert.True(hasBeenUpdated);
            Assert.True(database.RetrieveMartianWithoutClearance(entityAndIdMap[XENOMORPH]) == updatedXenomorph);
        }

        [Fact]
        public void AttemptingToDeleteEntityInDbWhenIdNotPresentInDbShouldReturnFalse()
        {
            var hasBeenDeleted = database.DeleteMartian(NON_EXISTENT_ID, Clearance.TOP_LEVEL_CLEARANCE);
            Assert.False(hasBeenDeleted);
        }

        [Fact]
        public void AttemptingToDeleteEntityInDbWhenClearanceNotSufficientShouldReturnFalse()
        {
            var hasBeenDeleted = database.DeleteMartian(entityAndIdMap[XENOMORPH], Clearance.ACCESS_RESTRICTED);
            Assert.False(hasBeenDeleted);
        }

        [Fact]
        public void AttemptingToDeleteEntityInDbWhenIdPresentInDbAndClearanceSufficientShouldReturnTrue()
        {
            var hasBeenDeleted = database.DeleteMartian(entityAndIdMap[XENOMORPH], Clearance.TOP_LEVEL_CLEARANCE);
            Assert.True(hasBeenDeleted);
            Assert.Null(database.RetrieveMartianWithoutClearance(entityAndIdMap[XENOMORPH]));
            Assert.Equal(2, database.Count());
        }
    }
}