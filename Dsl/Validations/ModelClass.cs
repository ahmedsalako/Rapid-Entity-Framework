using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Modeling.Validation;
using consist.RapidEntity.Validations;

namespace consist.RapidEntity
{
    [ValidationState(ValidationState.Enabled)]
    public partial class ModelClass
    {
        [ValidationMethod(ValidationCategories.Menu | ValidationCategories.Load | ValidationCategories.Open | ValidationCategories.Save)]
        public void ValidateClassShape(ValidationContext context)
        {
            ValidationHelper.ValidateClassSpecification(this, context);
            ValidationHelper.ValidatePropertiesSpecification(this, context);
        }
    }
}
