using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Ghosting;
using System.Collections;
using PersistentManager.Descriptors;
using PersistentManager.Util;
using System.Runtime.Remoting.Messaging;
using System.Data;
using System.Linq.Expressions;
using PersistentManager.Exceptions;
using PersistentManager.Linq.ExpressionCommands;
using PersistentManager.Metadata;
using System.Reflection;

namespace PersistentManager.Query.Keywords
{
    public class Keyword
    {
        internal virtual Type CompilerGeneratedResultType { get; set; }
        internal virtual bool ResultIsCompilerGenerated { get; set; }        
        internal virtual PathExpressionFactory Path { get; set; }        
        internal virtual Keyword BaseScope { get; set; }
        internal virtual AS Identifier { get; set; }
        private int criteriaOrdinal = 0;

        internal int CriteriaOrdinal
        {
            get 
            { 
                return criteriaOrdinal++; 
            }
        }

        internal List<IDeferedExecution> deferedSelect = new List<IDeferedExecution>( );

        internal List<IDeferedExecution> DeferedSelect
        {
            get { return deferedSelect; }
            set { deferedSelect = value; }
        }

        internal string[] GetParameterNames( object[] args )
        {
            return CallStack.GetAllParameters( args ).ToArray( );
        }

        internal string[] GetParameterNames( object[] args , List<IDeferedExecution> deferedCollection )
        {
            return CallStack.GetParameters( args , deferedCollection ).ToArray( );
        }

        internal string GetParameterName( object parameter )
        {
            return CallStack.GetParameter( parameter );
        }

        internal string ReadLastParameter( object parameter , Guid ScopeId )
        {
            return CallStack.ReadLastParameter( ScopeId );
        }

        internal IEnumerable<Aggregate> GetParameterNames( Aggregate[] aggregates )
        {
            Queue<string> names = new Queue<string>( GetParameterNames( aggregates.Select( a => a.Name ).ToArray( ) ) );
            foreach ( Aggregate aggregate in aggregates )
            {
                aggregate.Name = names.Dequeue( );
                yield return aggregate;
            }
        }

        internal virtual string GetIdentifier( object entity )
        {
            return CallStack.GetIdentifier( entity );
        }

        public virtual SyntaxContainer GetSyntax( )
        {
            return Path.IndexAll( );
        }

        internal virtual PropertyMetadata GetProperty( Type type , string name )
        {
            return EntityMetadata.GetMappingInfo( type ).GetPropertyMappingIncludeBase( name.EraseAlias( ) );
        }

        internal virtual PathExpression CreateJoin( Type type )
        {
            PathExpression expression = Path.Create( type.Name , type );
            Path.Main.AddJoin( expression );

            return expression;
        }

        internal virtual PathExpression GetRelation( string[] expression , PathExpression currentPath )
        {
            if ( expression.Length <= 0 )
            {
                return currentPath;
            }

            PropertyMetadata property = currentPath.GetPropertyMetadata( ArrayUtil.GetFirstElement( expression ) );

            PathExpression relation = Path.Create( property.ClassDefinationName , property.RelationType );

            return GetRelation( ArrayUtil.RemoveNexElement( expression ) , currentPath.AddReferences( relation ) );
        }

        internal virtual PathExpression CreateRelation( string relation )
        {
            return Path.Create( relation.EraseAlias() , GetProperty( Path.Main.Type , relation.EraseAlias() ).RelationType );
        }

        internal virtual PathExpression CreateRelation( Type type , string relation )
        {
            return Path.Create( relation.EraseAlias( ) , type );
        }

        internal virtual string GetLastParameter( )
        {
            return CallStack.GetLastParameter( );
        }

        internal virtual PathExpressionFactory AddJoin( string alias , PathExpression reference )
        {
            reference.CanonicalAlias = alias;
            Path.Main.AddJoin( reference );
            Path.Participants.Add( alias , reference );

            return Path;
        }

        internal virtual PathExpressionFactory AddReference( string alias , PathExpression reference )
        {
            reference.CanonicalAlias = alias;
            Path.Main.References.Add( reference.Property , reference );
            Path.Participants.Add( alias , reference );

            return Path;
        }

        internal virtual void AddProjectionExpression( QueryPart part , FunctionCall function , string name )
        {
            AddExpression( name , Criteria.CreateCriteria( part , function , name ) );
        }

        internal virtual void AddProjectionExpression( string expression , QueryPart part , ProjectionFunction function , object value )
        {
            AddExpression( expression , Criteria.CreateCriteria( part , expression , function , value ) );
        }

        internal virtual void AddProjectionExpression( string[] expressions , QueryPart part )
        {
            foreach ( string expression in expressions )
            {
                AddExpression( expression , Criteria.CreateCriteria( part , expression ) );
            }
        }

        internal virtual Criteria AddProjectionExpression( string expression , QueryPart part )
        {
            return AddExpression( expression , Criteria.CreateCriteria( part , expression ) );
        }

        internal virtual void AddConditionExpression( IEnumerable<string> expressions , QueryPart part , Condition condition , IEnumerable<object> values )
        {
            AddConditionExpression( expressions.ToArray( ) , part , condition , values.ToArray( ) );
        }

        internal virtual void AddConditionExpression( string[] expressions , QueryPart part , Condition condition , object[] values )
        {
            if ( expressions.IsNotNull() )
            {
                foreach (int index in expressions.GetIndices())
                {
                    object value = values.IsNull() ? null : values.Length == 1 ? values[0] : values[index];
                    AddConditionExpression( expressions[index] , part , condition , values.IsNotNull( ) ? value : null );
                }
            }
        }

        internal virtual Criteria AddConditionExpression( string expression , QueryPart part , Condition condition , object value )
        {
            return AddConditionExpression( expression , part , condition , value , null );
        }

        internal virtual Criteria AddConditionExpression( string expression , QueryPart part , Condition condition , object value , Queue<QueryFunction> functions )
        {
            Criteria leftCriteria = Criteria.CreateCriteria( part , null , condition , value , ProjectionFunction.NOTSET , null , functions );

            return AddConditionExpression( expression , part , leftCriteria , value );
        }

        internal virtual Criteria AddConditionExpression( string expression , QueryPart part , Condition condition , CompositeValue value , Queue<QueryFunction> functions )
        {
            Criteria leftCriteria = Criteria.CreateCriteria( part , null , condition , value , ProjectionFunction.NOTSET , null , functions );
            leftCriteria.AddCompositeValue( value );

            return AddConditionExpression( expression , part , leftCriteria , null );
        }

        internal virtual Criteria AddConditionExpression( string expression , QueryPart part , Condition condition , CompositeValue value )
        {
            return AddConditionExpression( expression , part , condition , value , null );
        }

        internal virtual Criteria AddConditionExpression( string expression , QueryPart part , Criteria leftCriteria , object value )
        {
            leftCriteria = AddExpression( expression , leftCriteria );

            if ( !value.IsNullOrEmpty( ) )
            {
                string[] right = StringUtil.Split( value.ToString( ) , "." );

                if ( Path.Participants.ContainsKey( right[0] ) || Path.Main.CanonicalAlias == right[0] )
                {
                    Criteria rightCriteria = Criteria.CreateCriteria( part , null , leftCriteria.Condition , null , ProjectionFunction.NOTSET , null , FunctionCall.None );
                    rightCriteria = AddExpression( value.ToString( ) , rightCriteria );
                    leftCriteria.JoinType = JoinType.LeftJoin;
                    leftCriteria.JoinWith = rightCriteria.Hash;

                    rightCriteria.JoinType = JoinType.RightJoin;
                    rightCriteria.JoinWith = leftCriteria.Hash;

                    return leftCriteria;
                }
                else if ( leftCriteria.Condition == Condition.Is || value != leftCriteria.Value )
                {
                    return leftCriteria;
                }
            }

            leftCriteria.Value = value;

            return leftCriteria;
        }

        internal virtual Criteria AddExpression( string expression , Criteria criteria )
        {
            if ( expression.IsNull( ) )
                return criteria;

            string[] array = StringUtil.Split( expression , "." );
            PathExpression currentPath = PathExpression.Empty;
            PropertyMetadata property = null;
            string propertyName = string.Empty;
            string parameter = string.Empty;

            if ( Path.Participants.ContainsKey( array[0] ) )
            {
                parameter = array[0];
                currentPath = Path.Participants[array[0]];

                array = ArrayUtil.RemoveNexElement( array ); // strip alias

                propertyName = ArrayUtil.GetFirstElement( array );

                property = currentPath.GetPropertyMetadata( propertyName );

                array = ArrayUtil.RemoveNexElement( array );    //Strip current property           
            }
            else if ( Path.Main.CanonicalAlias == array[0] )
            {
                parameter = Path.Main.CanonicalAlias;
                currentPath = Path.Main;

                array = ArrayUtil.RemoveNexElement( array ); // strip alias

                propertyName = ArrayUtil.GetFirstElement( array );

                property = currentPath.GetPropertyMetadata( propertyName );

                array = ArrayUtil.RemoveNexElement( array );    //Strip current property              
            }
            else
            {
                parameter = Path.Main.CanonicalAlias;
                currentPath = Path.Main;

                array = array.Length >= 2 ? ArrayUtil.RemoveNexElement( array ) : array;

                propertyName = ArrayUtil.GetFirstElement( array );
                property = currentPath.GetPropertyMetadata( propertyName );
                array = ArrayUtil.RemoveNexElement( array );    //Strip current property 
            }
            
            return GetExpression( property , array , currentPath , criteria , propertyName );
        }

        internal virtual Criteria AddJoin( PathExpression ownerPath , Type joinType , Criteria criteria )
        {
            Criteria joinedCriteria = Criteria.CreateCriteria( QueryPart.WHERE , criteria.Name );
            joinedCriteria.DeclaringType = joinType;
            joinedCriteria.ReflectedType = joinType;
            joinedCriteria.Condition = Condition.Equals;
            joinedCriteria.JoinWith = criteria.Hash;
            joinedCriteria.JoinType = JoinType.RightJoin;

            criteria.JoinWith = joinedCriteria.Hash;
            criteria.JoinType = JoinType.LeftJoin;
            criteria.Condition = Condition.Equals;

            ownerPath.AddToJoin( joinedCriteria );

            return criteria;
        }

        internal virtual Criteria GetOfTypeExpression( PathExpression currentPath , Criteria criteria , PropertyMetadata property )
        {
            Type subclass = ( Type ) criteria.Value;
            EntityMetadata classMetadata = EntityMetadata.GetMappingInfo( subclass );
            PropertyMetadata propertyMetadata;

            if ( classMetadata.HasDiscriminator )
            {
                propertyMetadata = classMetadata.GetDiscriminatorProperty( );
                criteria.Name = propertyMetadata.MappingName;
                criteria.Condition = Condition.Equals;
                criteria.DeclaringType = propertyMetadata.DeclaringType;
                criteria.Value = propertyMetadata.FieldValue;

                if ( property.IsNotNull( ) )
                {
                    PathExpression relation = Path.Create( property.ClassDefinationName , property.RelationType );
                    relation.AddCriteria( criteria );
                    relation = currentPath.AddReferences( relation );
                }
                else
                {
                    currentPath.AddCriteria( criteria );
                }
            }
            else if ( classMetadata.HasBaseEntity )
            {
                propertyMetadata = classMetadata.Keys.FirstOrDefault( k => k.IsInheritance );
                criteria.Name = propertyMetadata.ClassDefinationName;
                criteria.DeclaringType = propertyMetadata.DeclaringType;

                if ( property.IsNotNull( ) )
                {
                    PathExpression relation = Path.Create( property.ClassDefinationName , property.RelationType );
                    relation.AddCriteria( criteria );
                    relation = currentPath.AddReferences( relation );
                    AddJoin( relation , subclass , criteria );
                }
                else
                {
                    currentPath.AddCriteria( criteria );
                    AddJoin( currentPath , subclass , criteria );
                }
            }
            else
            {
                Throwable.ThrowException( string.Format( "There is no 'is a' relationship defined between {0} and {1} entity. " , property.IsNotNull() ? property.PropertyType.Name : currentPath.Type.Name , subclass.Name ) );
            }

            return criteria;
        }

        internal virtual Criteria GetExpression( PropertyMetadata property , string[] expression , PathExpression currentPath , Criteria criteria , string propertyName )
        {
            if ( expression.Length <= 0 )
            {
                if ( criteria.Condition == Condition.Is ) return GetOfTypeExpression( currentPath , criteria , property );

                if ( criteria.QueryPart == QueryPart.SELECT )
                {
                    criteria.Ordinal = CriteriaOrdinal;
                    Path.SelectArguments.Add( criteria );
                }

                if ( criteria.DeclaringType.IsNull( ) )
                {
                    criteria.DeclaringType = currentPath.Type;
                }

                criteria.Name = criteria.PropertyName = property.IsNotNull( ) ? property.ClassDefinationName : propertyName;             
                currentPath.AddCriteria( criteria );

                return criteria;
            }

            criteria.DeclaringType = property.PropertyType;
            return GetExpression( property.PropertyType.GetMetataData( ).PropertyMapping( expression[0] ) , ArrayUtil.RemoveNexElement( expression ) , AddRelatedPath( property , currentPath ) , criteria , property.ClassDefinationName );
        }

        private PathExpression AddRelatedPath( PropertyMetadata property , PathExpression currentPath )
        {
            PathExpression relation = Path.Create( property.ClassDefinationName , property.RelationType );
            return currentPath.AddReferences( relation );
        }

        public DeferedExecution<T> SelectNew<T>( Expression<Func<T>> selector )
        {
            object[] projections = null;
            string name = string.Empty;

            if ( selector.Body is MemberInitExpression && !SyntaxContainer.IsCompilerGeneratedType( typeof( T ) ) )
            {

                Path.ProjectionBinder = SelectCommand.VisitProjectionBinder( Path ,
                                        ( ( MemberInitExpression )selector.Body ).Bindings );

                Path.ProjectionBinder.LeftAssignment = typeof( T );

                projections = Path.ProjectionBinder.GetProperties();
            }
            else if ( selector.Body is MemberExpression )
            {
                MemberExpression member = ( selector.Body as MemberExpression );
                name = GetParameterName( member.Member.Name );
                

                if ( (selector.Body.Type.IsClassOrInterface( )) && (member.Expression is ConstantExpression ))
                {
                    ConstantExpression expression = ( ConstantExpression ) member.Expression;       
                    name = GetIdentifier( MetaDataManager.GetFieldValue( name ,  expression.Value ) );
                }
            }

            return SelectNew( selector.Compile( ).Invoke( ) , projections , name );
        }

        internal DeferedExecution<TResult> SelectNew<T , TResult>( Expression<Func<T , TResult>> selector , T argument )
        {
            object[] projections = null;
            string name = string.Empty;

            if ( selector.Body is MemberInitExpression && !SyntaxContainer.IsCompilerGeneratedType( typeof( T ) ) )
            {

                Path.ProjectionBinder = SelectCommand.VisitProjectionBinder( Path ,
                                        ( ( MemberInitExpression ) selector.Body ).Bindings );

                Path.ProjectionBinder.LeftAssignment = typeof( TResult );

                projections = Path.ProjectionBinder.GetProperties( );
            }
            else if( selector.Body is MemberExpression )
            {
                name = ( selector.Body as MemberExpression ).Member.Name;
            }
            else if ( selector.Body is ParameterExpression )
            {
                name = GetIdentifier( Path.QueryableItem );
            }

            return SelectNew( selector.Compile( ).Invoke( argument ) , projections , name );
        }

        internal DeferedExecution<T> SelectNew<T>( T selector , object[] projections , string name )
        {
            SyntaxContainer syntax = null;
            if ( SyntaxContainer.IsCompilerGeneratedType( typeof( T ) ) )
            {
                Select select = new Select( selector , projections );
                select.ExecuteDefered( Path );

                syntax = GetSyntax( );
                syntax.CompilerGeneratedResultType = select.CompilerGeneratedResultType;
                syntax.ResultIsCompilerGenerated = select.ResultIsCompilerGenerated;
                syntax.DeferedSelect = select.DeferedSelect;
            }
            else if ( typeof( T ) == Path.Main.Type && Path.Main.CanonicalAlias == name )
            {
                Select select = new Select( );
                select.ExecuteDefered( Path );
                syntax = GetSyntax( );
            }
            else if ( typeof( T ).IsCollection( ) || typeof( T ).IsClassOrInterface( ) )
            {
                Select select = new Select( selector , projections );
                select.ExecuteDefered( Path );
                syntax = GetSyntax( );
                syntax.ResultIsCollection = typeof( T ).IsCollection( );
                syntax.ResultIsEntityClass = typeof( T ).IsClassOrInterface( ) && !typeof( T ).IsCollection( );
            }
            else
            {
                Select select = new Select( selector , projections );
                select.ExecuteDefered( Path );               
                syntax = GetSyntax( );
                syntax.ReturnType = typeof( T );
            }

            return new DeferedExecution<T>( selector , syntax );
        }

        public DeferedExecution<IQueryResult> Select( )
        {
            new Select( ).ExecuteDefered( Path );

            return new DeferedExecution<IQueryResult>( GetSyntax( ) );
        }

        public DeferedExecution<IQueryResult> Select( string[] parameters )
        {
            Select select = new Select( parameters );
            select.ExecuteDefered( Path );

            return new DeferedExecution<IQueryResult>( GetSyntax( ) );
        }

        public DeferedExecution<IQueryResult> Select( params Aggregate[] aggregates )
        {
            Select select = new Select( aggregates );
            select.ExecuteDefered( Path );

            return new DeferedExecution<IQueryResult>( GetSyntax( ) );
        }

    }
}
