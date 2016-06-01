using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Modeling;

namespace consist.RapidEntity
{
    public enum Bindings
    {
        Source ,
        Target ,
        Both ,
    }

    public static class CustomExtension
    {
        public static RelationshipTypeEnum Get( this RelationshipTypeEnum value )
        {
            if ( value.IsNull( ) )
                return RelationshipTypeEnum.NOTSET;

            return ( RelationshipTypeEnum ) Enum.Parse( typeof( RelationshipTypeEnum ) , GetName( value ) );
        }

        public static RelationshipTypeEnum GetByName( string name )
        {
            if ( name.IsNullOrEmpty( ) )
                return RelationshipTypeEnum.NOTSET;

            return ( RelationshipTypeEnum ) Enum.Parse( typeof( RelationshipTypeEnum ) , name );
        }

        public static RelationshipTypeEnum GetByName( this RelationshipTypeEnum value , string name )
        {
            if ( name.IsNullOrEmpty( ) )
                return RelationshipTypeEnum.NOTSET;

            return ( RelationshipTypeEnum ) Enum.Parse( typeof( RelationshipTypeEnum ) , name );
        }

        public static string GetName( this RelationshipTypeEnum value )
        {
            return Enum.GetName( typeof( RelationshipTypeEnum ) , value );
        }

        public static void RemoveByModelName( this LinkedElementCollection<ModelClass> collection , string name )
        {
            ModelClass modelClass = collection.Where( m => m.Name == name ).FirstOrDefault( );

            if ( null != modelClass )
            {
                collection.Remove( modelClass );
            }
        }

        public static void RemoveRelationshipBindings( this ModelClass modelClass , string name , BaseRelationship relationship , Bindings bindings )
        {
            RemoveRelationshipBindings( modelClass , name , relationship.GetType( ) , bindings );
        }

        public static void RemoveRelationshipBindings( this ModelClass modelClass , string name , Type type , Bindings bindings )
        {
            if ( type == typeof( ManyToMany ) )
            {
                if ( bindings == Bindings.Source || bindings == Bindings.Both )
                    modelClass.ManySourceModels.RemoveByModelName( name );

                if ( bindings == Bindings.Target || bindings == Bindings.Both )
                    modelClass.ManyTargetModel.RemoveByModelName( name );
            }
            else if ( type == typeof( OneToMany ) )
            {
                if ( bindings == Bindings.Source || bindings == Bindings.Both )
                    modelClass.OneToManySources.RemoveByModelName( name );

                if ( bindings == Bindings.Target || bindings == Bindings.Both )
                    modelClass.OneToManyTargets.RemoveByModelName( name );
            }
            else if ( type == typeof( OneToOne ) )
            {
                if ( bindings == Bindings.Source || bindings == Bindings.Both )
                    modelClass.OneToOneSources.RemoveByModelName( name );

                if ( bindings == Bindings.Target || bindings == Bindings.Both )
                    modelClass.OneToOneTargets.RemoveByModelName( name );
            }
        }

        public static BaseRelationship GetRelationship( this ModelClass target , string name )
        {
            return target.GetRelationships( ).FirstOrDefault( r => r.ReferenceEntity == name || r.OwnerEntity == name  );
        }

        public static BaseRelationship GetRelationshipByTarget( this ModelClass target , string name )
        {
            return target.GetRelationships( ).FirstOrDefault( r => r.ReferenceEntity == name );
        }

        public static BaseRelationship GetRelationshipBySource( this ModelClass source , string name )
        {
            return source.GetRelationships( ).FirstOrDefault( r => r.OwnerEntity == name );
        }

        public static bool IsSource(this BaseRelationship relationship, ModelClass modelClass)
        {
            return (relationship.Source.Id == modelClass.Id) ;                         
        }

        public static bool IsTarget(this BaseRelationship relationship, ModelClass modelClass)
        {
            return !relationship.IsSource(modelClass);
        }

        public static IEnumerable<BaseRelationship> GetRelationships( this ModelClass model )
        {
           foreach( BaseRelationship relationship in model.Store.GetAllConnectors() )
           {
               if ( relationship.Source.Id == model.Id || relationship.Target.Id == model.Id )
               {
                   yield return relationship;
               }
           }
        }

        public static bool RelationshipAlreadyExistIn( this BaseRelationship relationship , ModelClass owner , ModelClass target )
        {
            return owner.GetRelationship( target.Name ).IsNotNull( );
        }

        public static string GetRelationshipTypeAsString( this BaseRelationship relationship )
        {
            if ( relationship is OneToMany )
            {
                return RelationshipTypeEnum.OneToMany.GetName( );
            }
            else if ( relationship is OneToOne )
            {
                return RelationshipTypeEnum.OneToOne.GetName( );
            }
            else if ( relationship is ManyToMany )
            {
                return RelationshipTypeEnum.ManyToMany.GetName( );
            }

            return string.Empty;
        }

        public static RelationshipTypeEnum GetRelationshipType( this BaseRelationship relationship )
        {
            if ( relationship is OneToMany )
            {
                return RelationshipTypeEnum.ManyToOne;
            }
            else if ( relationship is OneToOne )
            {
                return RelationshipTypeEnum.OneToOne;
            }
            else if ( relationship is ManyToMany )
            {
                return RelationshipTypeEnum.ManyToMany;
            }

            return RelationshipTypeEnum.NOTSET;
        }

        public static string GetRelationshipInverseAsString( this BaseRelationship relationship )
        {
            if ( relationship is OneToMany )
            {
                return RelationshipTypeEnum.ManyToOne.GetName( );
            }
            else if ( relationship is OneToOne )
            {
                return RelationshipTypeEnum.OneToOne.GetName( );
            }
            else if ( relationship is ManyToMany )
            {
                return RelationshipTypeEnum.ManyToMany.GetName( );
            }

            return string.Empty;
        }

        public static ModelClass GetModelClassByName( this Store store , string name )
        {
            return store.GetAllModelClass( ).Where( m => m.Name == name ).FirstOrDefault( );
        }

        public static IEnumerable<ModelClass> GetAllModelClass( this Store store )
        {
            foreach ( ModelElement element in store.ElementDirectory.AllElements )
                if ( element is ModelClass )
                    yield return ( ModelClass ) element;
        }

        public static IEnumerable<ModelClass> GetAllModelClass( this ModelClass modelClass )
        {
            return modelClass.Store.GetAllModelClass( );
        }

        public static IEnumerable<BaseRelationship> GetAllConnectors( this Store store )
        {
            foreach ( ModelElement element in store.ElementDirectory.AllElements )
                if ( element is BaseRelationship )
                    yield return ( BaseRelationship ) element;
        }

        public static bool StoreTransactionIsSerializing( this ModelElement modelElement )
        {
            var transaction = modelElement.Store.TransactionManager.CurrentTransaction;
            return ( transaction != null && transaction.IsSerializing );
        }

        public static bool IsNull( this object value )
        {
            return ( null == value );
        }

        public static bool NotEquals( this object value , string content )
        {
            return ( value.ToString( ) != content );
        }

        public static bool IsNotNull( this object value )
        {
            return !value.IsNull( );
        }

        public static int ParseSpecial( this object value )
        {
            if ( value.IsNullOrEmpty( ) )
            {
                return 0;
            }

            return int.Parse( value.ToString( ) );
        }

        public static string ToStringSpecial( this object value )
        {
            if ( value is DBNull || value.IsNull() )
                return string.Empty;

            return value.ToString( );
        }

        public static bool ContainsAny( this string value , params string[] values )
        {
            foreach ( var text in values )
            {
                if ( value.Contains( text ) )
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsNullOrEmpty( this object value )
        {
            if ( null == value )
                return true;

            return string.IsNullOrEmpty( value.ToString( ) );
        }

        public static T OverrideNullOrEmpty<T>( this T value1 , T value2 )
        {
            if ( value1.IsNullOrEmpty( ) )
                return value2;

            return value1;
        }

        public static string SetNewColumnName( this ModelClass source , string name )
        {
            int increment = 1;
            foreach ( var property in source.GetAllProperties( ) )
            {
                if ( property.ColumnName.ContainsAny( name ) )
                {
                    increment = increment + 1;                    
                }
            }

            return string.Concat( name , increment );
        }

        public static IEnumerable<ModelAttribute> GetProperties( this ModelClass model )
        {
            foreach ( Field field in model.Fields )
                yield return field;

            foreach ( ModelAttribute key in model.PersistentKeys )
                yield return key;
        }

        public static bool ParentIsAbstractOrNoParent( this ModelClass model )
        {
            if ( model.IsAbstract == InheritanceModifier.Abstract )
                return false;

            if ( model.Superclass.IsNull( ) )
                return true;

            return ( model.Superclass.IsAbstract == InheritanceModifier.Abstract );
        }

        public static IEnumerable<ModelClass> GetSubclassesOrSelf( this ModelClass model )
        {
            if ( model.Subclasses.Count <= 0 || model.IsAbstract != InheritanceModifier.Abstract )
            {
                yield return model;
            }
            else
            {
                foreach ( ModelClass subClass in model.Subclasses )
                {
                    yield return subClass;
                }
            }
        }

        public static bool HasSiblings( this ModelClass model )
        {
            return ( model.Subclasses.Count > 0 );
        }

        public static PersistentKey GetPersistentKeyIncludeInheritanceByColumnName( this ModelClass model , string columnName )
        {
            return ( PersistentKey ) model.GetPropertiesIncludeInheritance( )
                .Where( p => p is PersistentKey && p.ColumnName.Trim().ToLower() == columnName.Trim().ToLower() ).FirstOrDefault( );
        }

        public static PersistentKey GetPersistentKeyIncludeInheritance( this ModelClass model )
        {
            return ( PersistentKey ) model.GetPropertiesIncludeInheritance( ).Where( p => p is PersistentKey ).FirstOrDefault( );
        }

        public static IEnumerable<BaseRelationship> GetRelationtionshipByTarget( this ModelClass target )
        {
            foreach ( var relationship in target.GetRelationships( ) )
            {
                if ( relationship.Target.Id == target.Id )
                {
                    yield return relationship;
                }
            }
        }

        public static bool ReferenceColumnIsFieldOrKey( this ModelClass source , BaseRelationship relationship )
        {
            foreach ( var property in source.GetAllProperties() )
            {
                if ( property.ColumnName.ToLower( ).Trim( ) == relationship.ReferenceColumn.ToLower( ).Trim( ) )
                {
                    return true;
                }
            }

            return false;
        }

        public static BaseRelationship GetRelationship( this ModelAttribute attribute )
        {
            foreach ( BaseRelationship relationship in attribute.GetModelClass( ).GetRelationshipsByTargetIncludeInheritance( ) )
            {
                if ( attribute.ColumnName.ToLower( ).Trim( ) == relationship.ReferenceColumn.ToLower( ).Trim( ) )
                {
                    return relationship;
                }
            }

            return null;
        }

        public static bool IsAlsoReferenced( this ModelAttribute attribute )
        {
            foreach ( BaseRelationship relationship in attribute.GetModelClass( ).GetRelationshipsByTargetIncludeInheritance( ) )
            {
                if ( attribute.ColumnName.ToLower( ).Trim( ) == relationship.ReferenceColumn.ToLower( ).Trim( ) )
                {
                    return true;
                }
            }

            return false;
        }

        public static IEnumerable<BaseRelationship> GetRelationshipsByTargetIncludeInheritance( this ModelClass source )
        {
            foreach ( var relationship in source.GetRelationtionshipByTarget( ) )
            {
                yield return relationship;
            }

            if ( source.Superclass.IsNotNull( ) )
            {
                //In case of table per class hierarchy
                if ( source.Superclass.IsAbstract == InheritanceModifier.Abstract )
                {
                    foreach ( var relationship in source.Superclass.GetRelationtionshipByTarget() )
                        yield return relationship;
                }
            }

            if ( source.Subclasses.Count > 0 && source.IsAbstract != InheritanceModifier.Abstract )
            {
                foreach ( ModelClass subclass in source.Subclasses )
                {
                    foreach ( var relationship in subclass.GetRelationtionshipByTarget() )
                    {
                        yield return relationship;
                    }
                }
            }
        }

        public static IEnumerable<BaseRelationship> GetRelationshipsIncludeInheritance( this ModelClass model )
        {
            foreach ( var relationship in model.GetRelationships( ) )
                yield return relationship;

            if ( model.Superclass.IsNotNull( ) )
            {
                //In case of table per class hierarchy
                if ( model.Superclass.IsAbstract == InheritanceModifier.Abstract )
                {
                    foreach ( var relationship in model.Superclass.GetRelationships( ) )
                        yield return relationship;
                }
            }

            if ( model.Subclasses.Count > 0 && model.IsAbstract != InheritanceModifier.Abstract )
            {
                foreach ( ModelClass subclass in model.Subclasses )
                {
                    foreach ( var relationship in subclass.GetRelationships( ) )
                    {
                        yield return relationship;
                    }
                }
            }
        }

        public static IEnumerable<ModelAttribute> GetPropertiesIncludeInheritance( this ModelClass model )
        {
            foreach ( Field field in model.Fields )
                yield return field;

            foreach ( ModelAttribute key in model.PersistentKeys )
                yield return key;

            if ( model.Superclass.IsNotNull( ) )
            {
                //In case of table per class hierarchy
                if ( model.Superclass.IsAbstract == InheritanceModifier.Abstract )
                {
                    foreach ( ModelAttribute field in model.Superclass.GetPropertiesIncludeInheritance( ) )
                        yield return field;
                }
            }

            if ( model.Subclasses.Count > 0 && model.IsAbstract != InheritanceModifier.Abstract )
            {
                foreach ( ModelClass subclass in model.Subclasses )
                {
                    foreach ( ModelAttribute field in subclass.GetPropertiesIncludeInheritance( ) )
                    {
                        yield return field;
                    }
                }
            }
        }

        public static IEnumerable<ModelAttribute> GetAllProperties( this ModelClass model )
        {
            foreach ( Field field in model.Fields )
                yield return field;

            foreach ( ModelAttribute key in model.PersistentKeys )
                yield return key;
        }

        public static ModelAttribute GetByPropertyName( this ModelClass model , string name )
        {
            foreach ( Field field in model.Fields )
            {
                if ( field.Name == name )
                {
                    return field;
                }
            }

            foreach ( ModelAttribute key in model.PersistentKeys )
            {
                if ( key.Name == name )
                {
                    return key;
                }
            }

            return null;
        }

        public static bool HasPersistentKey( this ModelClass model )
        {
            return ( model.PersistentKeys.Count > 0 );
        }

        public static bool UpdatedPropertyExist( this ModelClass model , ModelAttribute attribute )
        {
            int count = model.GetProperties( ).Count( p => p.Name == attribute.Name );
            if ( count > 1 )
                return true;

            return false;
        }

        public static bool PropertyExist( this ModelClass model , string propertyName )
        {
            int count = model.GetProperties( ).Count( p => p.Name == propertyName );
            if ( count > 0 )
                return true;

            return false;
        }

        public static bool AddedPropertyExist( this ModelClass model , ModelAttribute attribute )
        {
            int count = model.GetProperties( ).Count( p => p.Name == attribute.Name );
            if ( count > 1 )
                return true;

            return false;
        }

        public static void AddName( this ModelClass modelClass , string name , int count )
        {
            if ( modelClass.PropertyExist( name ) )
            {
                modelClass.AddName( name + count , ++count );

                return;
            }

            modelClass.Name = name;
        }

        public static ModelClass GetModelClass( this ModelAttribute modelAttribute )
        {
            if ( modelAttribute is PersistentKey )
                return ( ( PersistentKey ) modelAttribute ).ModelClass;

            else if ( modelAttribute is Field )
                return ( ( Field ) modelAttribute ).ModelClass;

            return null;
        }
    }
}
