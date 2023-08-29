using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockMartianApi.Models
{
    public class MartianEntity
    {
        public readonly string species;
        public readonly Clearance clearanceRequired;

    public MartianEntity(string species, Clearance clearanceRequired)
        {
            this.species = species;
            this.clearanceRequired = clearanceRequired;
        }
    }
}
