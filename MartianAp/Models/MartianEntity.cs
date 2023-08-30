using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MockMartianApi.Models
{
    [DataContract]
    public class MartianEntity
    {
        [DataMember]
        public readonly string species;
        [DataMember]
        public readonly Clearance clearanceRequired;

    public MartianEntity(string species, Clearance clearanceRequired)
        {
            this.species = species;
            this.clearanceRequired = clearanceRequired;
        }
    }
}
