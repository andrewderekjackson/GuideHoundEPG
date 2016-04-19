using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GuideHoundEPG.Common.Model.MXF {
    
    public abstract class Role {
        public Person Person { get; set; }
        public int Rank { get; set; }
    }
    
    public class ActorRole : Role {
        
    }

    public class DirectorRole : Role {
        
    }

    public class ProducerRole : Role {
       
    }

    public class WriterRole : Role {

    }
}
