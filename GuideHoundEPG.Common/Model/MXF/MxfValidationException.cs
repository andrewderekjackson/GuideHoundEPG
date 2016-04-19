using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GuideHoundEPG.Common.Model.MXF
{
	public class MxfValidationException : Exception {

        public MxfValidationException(string message) : base(message) {             
        }
	}

}
