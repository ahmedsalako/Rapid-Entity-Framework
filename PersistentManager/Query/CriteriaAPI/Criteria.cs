using System;
using System.Collections.Generic;
using System.Text;
using PersistentManager.Query;
using PersistentManager.Query.Keywords;
using PersistentManager.Util;
using System.Linq;

namespace PersistentManager.Descriptors
{
    public class Criteria : ICriteriaCloneable
    {        
        private ProjectionFunction funtionType = ProjectionFunction.NOTSET;        
        private CompositeValue compositeValues;
        private Condition condition;

        internal Criteria( )
        {
            Hash = Guid.NewGuid( );
            Position = Operand.Left;
        }

        internal bool IsDeferedCorrelation { get; set; }

        internal Operand Position { get; set; }

        internal Queue<QueryFunction> Functions { get; set; }

        internal CompositeValue MultiValue
        {
            get { return compositeValues; }
        }

        public Guid Hash { get; set; }

        internal Guid JoinWith { get; set; }

        internal JoinType JoinType { get; set; }

        public string Name { get; set; }

        public object Value { get; set; }

        internal string Alias { get; set; }

        internal string PropertyName { get; set; }

        internal int Ordinal { get; set; }

        internal ProjectionFunction FuntionType
        {
            get { return funtionType; }
            set { funtionType = value; }
        }

        internal Condition Condition
        {
            get { return condition; }
            set { condition = value; }
        }

        internal SyntaxContainer CorrelatedSubQuery { get; set; }

        internal PathExpressionFactory CorrelatedPath { get; set; }

        internal string OwnerAlias { get; set; }

        public bool Unassigned { get; set; }

        public bool ContainedInFunction { get; set; }

        public bool IsProjected { get; set; }

        public Type DeclaringType { get; set; }

        public Type ReflectedType { get; set; }

        public string OwnerPropertyName { get; set; }

        public string Path { get; set; }

        internal QueryPart QueryPart { get; set; }

        internal Guid GroupId { get; set; }

        internal virtual Guid ContainerGuid { get; set; }

        internal string FullyQualifiedName
        {
            get
            {
                return OwnerAlias.IsNullOrEmpty( ) ? Name : OwnerAlias.Dot( Name );
            }
        }       

        internal bool HasFunction( FunctionCall function )
        {
            if ( Functions.IsNotNull( ) )
            {
                return Functions.Count( f => f.Function == function && !f.Function.IsCombining() ) > 0;
            }

            return false;
        }

        internal void RemoveAggregateFunctions( )
        {
            Queue<QueryFunction> newFunctions = new Queue<QueryFunction>( );
            foreach ( var func in Functions )
            {
                if ( !func.Function.IsAggregate() )
                {
                    newFunctions.Enqueue( func );
                }
            }

            Functions = newFunctions;
        }

        internal void AddFunction( QueryFunction function )
        {
            if ( function.IsNull( ) ) return;
            if ( HasFunction( function.Function ) ) return;
            if ( Functions.IsNull() )
            {
                Functions = new Queue<QueryFunction>();
            }

            Functions.Enqueue( function );
        }

        internal void AddFunction( Queue<QueryFunction> functions )
        {
            if ( functions.IsNull( ) ) return;

            IList<int> indices = functions.GetIndices().ToList();
            if ( Functions.IsNull() )
            {
                Functions = new Queue<QueryFunction>();
            }

            foreach ( int index in indices )
            {
                Functions.Enqueue( functions.Dequeue() );
            }
        }

        internal void AddFunction( FunctionCall function , params object[] parameters )
        {
            if (Functions.IsNull())
            {
                Functions = new Queue<QueryFunction>();
            }

            Functions.Enqueue( new QueryFunction( function , parameters ) );
        }        

        internal void AddValue( object value )
        {
            Value = value;
        }

        internal void AddCompositeValue( params object[] values )
        {
            if ( compositeValues.IsNull( ) )
            {
                compositeValues = new CompositeValue( values );
            }
            else
            {
                compositeValues.AddValues( values );
            }
        }

        internal void AddCompositeValue( CompositeValue compositeValues )
        {
            this.compositeValues = compositeValues;
        }

        internal bool IsMultiValue
        {
            get 
            {
                return ( compositeValues.IsNotNull( ) && compositeValues.Count > 0 ) ;
            }
        }

        internal string GetConditionAsString( )
        {
            switch ( condition )
            {
                case Condition.Equals:
                    return string.Format( " = " );
                case Condition.GreaterThan:
                    return string.Format( " > " );
                case Condition.GreaterThanEqualsTo:
                    return string.Format( " >= " );
                case Condition.NotEquals:
                    return string.Format( " <> " );
                case Condition.LessThan:
                    return string.Format( " < " );
                case Condition.LessThanEqualsTo:
                    return string.Format( " <= " );
                case Condition.IsNull:
                    return string.Format( " IS NULL " );
                case Condition.IsNotNull:
                    return string.Format( " IS NOT NULL " );
                case Condition.StartsWith:
                    return string.Format( " LIKE " );
                case Condition.EndsWith:
                    return string.Format( " LIKE " );
                case Condition.Contains:
                    return string.Format( " LIKE " );
                case Condition.NotContains :
                    return string.Format( " NOT LIKE " );
                case Condition.IN :
                    return " {0} IN({1}) " ;
                case Condition.NOT_IN:
                    return " {0} NOT IN({1}) " ;
                case Condition.Between :
                    return " {0} BETWEEN {1} AND {2} " ;
                default:
                    return string.Format( " = " );
            }
        }

        internal bool RequiresParameter( )
        {
            switch ( condition )
            {
                case Condition.IsNull:
                    return false;
                case Condition.IsNotNull:
                    return false;
                default:
                    return true;
            }
        }

        internal string[] FormatParameterNames( string[] parameters )
        {
            List<string> formats = new List<string>( );

            foreach ( var value in parameters )
            {
                formats.Add( FormatParameterName( value ) );
            }

            return formats.ToArray( );
        }

        internal string FormatParameterName( string parameter )
        {
            if ( condition == Condition.IsNotNull )
                return string.Empty;
            else if ( condition == Condition.IsNull )
                return string.Empty;
            else
                return parameter;
        }

        internal object FormatParameterValue( object value )
        {
            if ( condition == Condition.StartsWith )
                return string.Format( "{0}%" , value );
            else if ( condition == Condition.EndsWith )
                return string.Format( "%{0}" , value );
            else if ( condition == Condition.Contains )
                return string.Format( "%{0}%" , value );
            else if ( condition == Condition.NotContains )
                return string.Format( "%{0}%" , value );
            else if ( condition == Condition.IsNotNull )
                return string.Empty;
            else if ( condition == Condition.IsNull )
                return string.Empty;
            else
                return value;
        }

        internal Criteria MoveAggregates( Criteria criteria )
        {
            if ( Functions.IsNotNull( ) )
            {
                foreach ( QueryFunction function in Functions )
                {
                    if ( function.Function.IsAggregate( ) || function.Function.IsCombining( ) )
                    {
                        criteria.AddFunction( function.CloneSpecial( ) );
                        function.Function = FunctionCall.None;
                    }
                }
            }

            return criteria;
        }

        internal bool IsJoinedAndSelect( )
        {
            return IsJoined( ) && QueryPart == QueryPart.SELECT;
        }

        internal bool IsJoined( )
        {
            return ( JoinWith != Guid.Empty );
        }

        internal bool IsJoinAndProjected( )
        {
            return ( IsJoined( ) && IsProjected );
        }

        internal Criteria GetRightSideJoin( List<Criteria> criterias )
        {
            return criterias.FirstOrDefault( c => c.JoinType == JoinType.RightJoin && c.IsJoinedWith( this ) );
        }

        internal Criteria GetLeftSideJoin( List<Criteria> criterias )
        {
            return criterias.FirstOrDefault( c => c.JoinType == JoinType.LeftJoin && c.IsJoinedWith( this ) );
        }

        internal bool IsJoinedWithAny( List<Criteria> criterias )
        {
            return criterias.Any( c => IsJoinedWith( c ) );
        }

        internal bool IsJoinedWith( Criteria criteria )
        {
            return ( JoinWith == criteria.Hash );
        }

        internal bool IsGroupingCriteria( )
        {
            return ( QueryPart == QueryPart.ORDERBY || QueryPart == QueryPart.GroupBy );
        }

        internal bool IsSelectProjection( )
        {
            return ( QueryPart == QueryPart.SELECT );
        }

        internal bool HasCorrelatedSubQueryValue( )
        {
            return CorrelatedSubQuery.IsNotNull( );
        }

        public bool HasCorrelatedPath( )
        {
            return CorrelatedPath.IsNotNull( );
        }

        internal bool IsNameOverriden( )
        {
            return Unassigned;
        }

        internal static Criteria CreateCriteria( QueryPart queryPart , string name , ProjectionFunction function , object value )
        {
            return CreateCriteria( queryPart , name , Condition.NOTSET , value , function , null , FunctionCall.None ); 
        }

        internal static Criteria CreateCriteria( QueryPart queryPart , string name , Type declaringType )
        {
            return CreateCriteria(queryPart, name, Condition.NOTSET, null, ProjectionFunction.NOTSET, declaringType , FunctionCall.None );
        }

        internal static Criteria CreateCriteria( QueryPart queryPart , string name )
        {
            return CreateCriteria( queryPart , name , Condition.NOTSET , null , ProjectionFunction.NOTSET , null , FunctionCall.None );                 
        }

        internal static Criteria CreateCriteria( QueryPart part , FunctionCall function , string name )
        {
            return CreateCriteria( part , name , Condition.NOTSET , null , ProjectionFunction.NOTSET , null , function ); 
        }

        internal static Criteria CreateCriteria( QueryPart queryPart , string name , Condition condition , object value , ProjectionFunction functionType , Type declaringType , FunctionCall function )
        {
            Criteria criteria = CreateCriteria( queryPart , name , condition , value , functionType , declaringType );
            criteria.AddFunction( function.IsNotNull( )  && function != FunctionCall.None ? new QueryFunction( function , null ) : null );
            return criteria;
        }

        internal static Criteria CreateCriteria( QueryPart queryPart , string name , Condition condition , object value , ProjectionFunction functionType , Type declaringType , Queue<QueryFunction> functions )
        {
            Criteria criteria = CreateCriteria( queryPart , name , condition , value , functionType , declaringType , FunctionCall.None );
            criteria.AddFunction( functions );

            return criteria;
        }

        internal static Criteria CreateCriteria( QueryPart queryPart , string name , Condition condition , object value , ProjectionFunction functionType , Type declaringType )
        {
            Criteria criteria = new Criteria( );
            criteria.Name = name;
            criteria.Value = value;
            criteria.Condition = condition;
            criteria.DeclaringType = declaringType;
            criteria.FuntionType = functionType;
            criteria.QueryPart = queryPart;
            return criteria;
        }

        internal static CompositeCriteria CreateCompositeCriteria( QueryPart queryPart , string name , Type declaringType , CompositeType compositeType , int ordinal )
        {
            return new CompositeCriteria
            {
                Name = name ,
                QueryPart = queryPart,
                DeclaringType = declaringType ,
                Ordinal = ordinal ,
                CompositeType = compositeType
            };
        }

        internal static CompositeCriteria CreateCompositeCriteria( Criteria criteria , CompositeType compositeType )
        {
            return CreateCompositeCriteria( criteria.QueryPart , criteria.Name , criteria.DeclaringType , compositeType , criteria.Ordinal );
        }

        internal virtual Criteria ApplyOuterAlias( PathExpression pathExpression )
        {                                
            OwnerAlias = pathExpression.OuterALIAS;

            return this;
        }

        internal virtual Criteria UpdateContainerGuid( Guid oldGuid , Guid newGuid )
        {
            if ( ContainerGuid == oldGuid || ContainerGuid == Guid.Empty )
            {
                ContainerGuid = newGuid;
            }

            return this;
        }

        internal virtual Criteria ApplyAlias( PathExpression pathExpression )
        {
            if ( ContainerGuid == pathExpression.UniqueId ) OwnerAlias = pathExpression.ALIAS;

            return this;
        }

        internal virtual void ScaleCriteria( PathExpression Referenced )
        {
            ApplyOuterAlias( Referenced );
            Unassigned = true;
        }

        internal void JoinCriteria( Criteria criteria )
        {
            JoinWith = criteria.Hash;
            JoinType = JoinType.LeftJoin;

            criteria.JoinWith = Hash;
            criteria.JoinType = JoinType.RightJoin;
        }

        public override string ToString( )
        {
            return Name;
        }

        #region ICloneable Members

        public virtual Criteria CloneScalable( )
        {
            Criteria clone = Clone( ) as Criteria;

            if ( this.CorrelatedPath.IsNotNull( ) )
            {
                this.CorrelatedPath = null;
                this.QueryPart = QueryPart.SELECT; //Hack
            }

            return clone;
        }

        public virtual object Clone( )
        {
            Criteria criteria = (Criteria) this.MemberwiseClone( );
            criteria.Functions = new Queue<QueryFunction>( );
            MoveAggregates( criteria );

            return criteria;
        }

        #endregion
    }
}
