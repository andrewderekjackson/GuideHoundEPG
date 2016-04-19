using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GuideHoundEPG.Common.Model.MXF {

    public class ModelValidationError {
        public String Error { get; set; }
    }
    
    interface IModelValidation {
        bool Validate();
        List<ModelValidationError> GetValidationErrors();
    }
}
