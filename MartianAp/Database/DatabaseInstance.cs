using MockMartianApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MockMartianApi.Database
{
    public class DatabaseInstance
    {
        private readonly List<string> presentIds = new List<string>();
        private Dictionary<string, MartianEntity> presentEntities = new Dictionary<string, MartianEntity>();
        private static readonly int ID_LENGTH = 10;
        private static Random random = new Random();

        public string? AddMartian(string? species, Clearance? clearance)
        {
            if (species == null || clearance == null)
            {
                return null;
            }
            var martian = new MartianEntity(species, clearance);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var id =  new string(Enumerable.Repeat(chars, ID_LENGTH).Select(s => s[random.Next(s.Length)]).ToArray());
            presentEntities.Add(id, martian);
            presentIds.Add(id);
            return id;
        }

        public bool DeleteMartian(string id, Clearance clearance)
        {
            return RemoveEntityFromIdAndMartianCollection(id, clearance);
        }

        public MartianEntity? RetrieveMartian(string id, Clearance clearance)
        {
            if (!presentIds.Contains(id))
            {
                return null;
            }
            var entity = presentEntities[id];

            if (!entity.clearanceRequired.AuthorisesClearanceLevel(clearance)) 
            {
                return null;
            }
            return entity;
        }

        public MartianEntity? RetrieveMartianWithoutClearance(string id)
        {
            if (!presentIds.Contains(id))
            {
                return null;
            }
            return presentEntities[id];
        }

        //Permission has already been granted when this method is called
        public bool UpdateMartian(string id, MartianEntity entity)
        {
            if (!presentIds.Contains(id))
            {
                return false;
            }
            presentEntities[id] = entity;
            return true;
        }

        public bool ContainsMartian(string id)
        {
            return presentIds.Contains(id);
        }

        private bool RemoveEntityFromIdAndMartianCollection(string id, Clearance clearance)
        {
            if (presentIds.Contains(id) && presentEntities.ContainsKey(id))
            {
                if (!presentEntities[id].clearanceRequired.AuthorisesClearanceLevel(clearance))
                {
                    return false;
                }
                presentIds.Remove(id);
                presentEntities.Remove(id);
                return true;
            }
            return false;
        }

        public int Count()
        {
            return presentEntities.Count;
        }
    }
}
