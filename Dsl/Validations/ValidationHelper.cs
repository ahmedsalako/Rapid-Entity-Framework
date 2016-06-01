using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Modeling.Validation;

namespace consist.RapidEntity.Validations
{
    public static class ValidationHelper
    {
        public const string NO_NAME = "NAME_001";
        public const string NO_TABLE_ERROR_CODE = "NOTABLENAME_001";
        public const string NO_PRIMARYKEY = "NO_PRIMARYKEY_001";

        public static void ValidateClassSpecification(this ModelClass current, ValidationContext context)
        {
            if (current.HasNoName())
            {
                context.LogError("The selected class must have a valid Name!", NO_NAME, current);
            }
            else if (current.HasNoTableName())
            {
                context.LogError(string.Format("The {0} class must have a valid Table Name!", current.Name), NO_TABLE_ERROR_CODE, current);
            }
            else if (current.DoesntHaveAtleastOneKey() && !current.ParentHasKey())
            {
                context.LogError(string.Format("The {0} class must have at least one Persistent Key!", current.Name), NO_PRIMARYKEY, current);
            }
            else if (!current.DoesntHaveAtleastOneKey() && current.ParentHasKey())
            {
                context.LogError(string.Format("The {0} class parent already has a key!", current.Name), NO_PRIMARYKEY, current);
            }
        }

        public static void ValidatePropertiesSpecification(this ModelClass current, ValidationContext context)
        {
            foreach (ModelAttribute property in current.GetAllProperties())
            {
                if (property.HasNoName())
                {
                    context.LogError(string.Format("A property in {0} class must have a valid Name!", current.Name), NO_NAME, property);
                }
                
                if (property.HasNoColumnName())
                {
                    context.LogError(string.Format("The property {0} of class {1}, does not have a Column Name!", property.ColumnName, current.Name), NO_NAME, property);
                }
                
                if (property.HasNoType())
                {
                    //if (property is Relationship)
                    //{
                        //context.LogError(string.Format("The {0} Relationship of class {1}, does not have a Type!", property.Name, current.Name), NO_NAME, property);
                    //}
                    //if
                    //{
                        context.LogError(string.Format("The property {0} of class {1}, does not have a Type!", property.Name, current.Name), NO_NAME, property);
                    //}
                }
            }
        }

        public static bool HasNoName(this ModelClass current)
        {
            return current.Name.IsNullOrEmpty();
        }

        public static bool HasNoTableName(this ModelClass current)
        {
            return current.TableName.IsNullOrEmpty();
        }

        public static bool DoesntHaveAtleastOneKey(this ModelClass current)
        {
            return (current.PersistentKeys.Count <= 0);
        }

        public static bool ParentHasKey(this ModelClass current)
        {
            if (current.Superclass.IsNull())
                return false;

            return !current.Superclass.DoesntHaveAtleastOneKey();
        }

        public static bool HasNoName(this ModelAttribute current)
        {
            return current.Name.IsNullOrEmpty();
        }

        public static bool HasNoColumnName(this ModelAttribute current)
        {
            return current.ColumnName.IsNullOrEmpty();
        }

        public static bool HasNoType(this ModelAttribute current)
        {
            return current.Type.IsNullOrEmpty();
        }

        public static bool FieldsMustHaveType(this ModelAttribute field)
        {
            return (!field.Type.IsNullOrEmpty());
        }
    }
}
