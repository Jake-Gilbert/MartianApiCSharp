using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MockMartianApi.Models
{
    public class Clearance
    {
        [DataMember]
        public string clearanceLevel;
        [DataMember]
        public int clearanceValue;

        private Clearance(string clearanceLevel, int clearanceValue)
        {
            this.clearanceLevel = clearanceLevel;
            this.clearanceValue = clearanceValue;
        }

        public Clearance()
        {
        }

        public static Clearance ACCESS_RESTRICTED { get { return new Clearance("Access Restricted", 0); } }
        public static Clearance MINIMAL_CLEARANCE { get { return new Clearance("Minimal Clearance", 1); } }
        public static Clearance STANDARD_CLEARANCE { get { return new Clearance("Standard Clearance", 2); } }
        public static Clearance ADVANCED_CLEARANCE { get { return new Clearance("Advanced Clearance", 3); } }
        public static Clearance TOP_LEVEL_CLEARANCE { get { return new Clearance("Top Level Clearance", 4); } }

        public bool AuthorisesClearanceLevel(Clearance clearanceToCheck)
        {
            return clearanceToCheck.clearanceValue >= clearanceValue;
        }
    }
}
