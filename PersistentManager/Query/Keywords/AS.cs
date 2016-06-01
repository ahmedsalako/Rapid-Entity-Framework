using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using PersistentManager.Ghosting;
using System.Reflection;
using System.Collections;
using PersistentManager.Util;
using PersistentManager.Linq;
using System.Linq.Expressions;

namespace PersistentManager.Query.Keywords
{
    public class AS : Keyword
    {
        internal AS( PathExpressionFactory Path )
        {
            this.Path = Path;
        }

        internal AS( string alias , PathExpressionFactory Path )
        {
            this.Path = Path;
            this.Path.Main.CanonicalAlias = alias;            
        }

        public IN In( string collection )
        {
            return new IN( Path , GetRelation( StringUtil.Split( collection.EraseAlias( ) , "." ) , Path.Main ) );
        }

        public IN In( IEnumerable collection )
        {
            return new IN( Path , GetRelation( StringUtil.Split( GetLastParameter( ).EraseAlias( ) , "." ) , Path.Main ) );
        }

        internal IN In( Type entity , string property )
        {
            return new IN( Path , CreateRelation( entity , property ) );
        }

        public JOIN Join( string relation )
        {
            return new JOIN( Path , GetRelation( StringUtil.Split( relation.EraseAlias( ) , "." ) , Path.Main ) );
        }

        public JOIN Join( object relation )
        {
            string parameter = GetIdentifier( relation );
            return new JOIN( Path , GetRelation( StringUtil.Split( parameter.EraseAlias() , "." ) , Path.Main ) );
        }

        public ON Join( Type type , object alias )
        {
            PathExpression join = CreateJoin( type );
            AddJoin( GetIdentifier( alias ) , join );

            return new ON( Path , join , this );
        }

        public ON Join( Type type , string alias )
        {
            PathExpression join = CreateJoin( type );
            AddJoin( alias , join );

            return new ON( Path , join , this );
        }

        public AS IsOfType( object value , Type type )
        {
            AddConditionExpression( value.IsNotNull() && value.IsString() ? (string)value : GetIdentifier( value ) , QueryPart.WHERE , Condition.Is , type );
            return new AS( Path );
        }

        public Where Where( object name , Condition condition , object value )
        {
            AddConditionExpression( GetParameterName( name ) , QueryPart.WHERE , condition , value );
            return new Where( Path , this );
        }

        public Where Where( object name , Condition condition )
        {
            AddConditionExpression( GetParameterName( name ) , QueryPart.WHERE , condition , string.Empty );
            return new Where( Path , this );
        }

        public Constraints<object,Where, object> Where(object property)
        {
            return new Constraints<object, Where, object>(property, QueryPart.WHERE, new Where(Path, this));
        }

        public Constraints<object,Where, string> Where(Action<string> expression)
        {
            return null;
        }

        public StringConstraints<object, Where, string> Where(string property)
        {
            return new StringConstraints<object, Where, string>(property, QueryPart.WHERE, new Where(Path, this));
        }

        public Constraints<object, Where, bool> Where(bool property)
        {
            return new Constraints<object, Where, bool>(property, QueryPart.WHERE, new Where(Path, this));
        }

        public IEnumerableConstraints<object, Where, object> Where(IEnumerable property)
        {
            return new IEnumerableConstraints<object, Where, object>(property, QueryPart.WHERE, new Where(Path, this));
        }

        public DateTimeConstraints<object, Where, DateTime> Where(DateTime property)
        {
            return new DateTimeConstraints<object, Where, DateTime>(property, QueryPart.WHERE, new Where(Path, this));
        }

        public virtual Where Where( object name , object value )
        {
            return Where( name , Condition.Equals , value );
        }

        internal virtual Where JoinWhere( object name , Criteria join )
        {
            Criteria criteria = AddConditionExpression( GetParameterName( name ) , QueryPart.WHERE , Condition.Equals , null );
            join.JoinCriteria( criteria );
            return new Where( Path , this );
        }

        internal virtual Where JoinWhere( string[] names , Criteria join )
        {
            foreach( string name in names )
            {
                Criteria criteria = AddConditionExpression( name , QueryPart.WHERE , Condition.Equals , null );              
                join.JoinCriteria( criteria );
            }

            return new Where( Path , this );
        }

        public virtual Where Where( object[] names , object[] values )
        {
            if ( values.IsNotNull( ) )
            {
                AddConditionExpression( GetParameterNames( names ) , QueryPart.WHERE , Condition.Equals , values );
            }

            return new Where( Path , this );
        }

        internal virtual Where WhereEntity( object entity )
        {
            EntityMetadata metadata = MetaDataManager.PrepareMetadata( entity.GetType( ) );

            AddConditionExpression( metadata.Properties( ) , QueryPart.WHERE , Condition.Equals , metadata.Values( ) );

            return new Where( Path , this );
        }

        internal virtual AND And( object[] names , Condition condition , object[] values )
        {
            AddConditionExpression( GetParameterNames( names ) , QueryPart.AND , condition , values );
            return new AND( Path , this );
        }

        public AND And( object name , Condition condition )
        {
            return And( name , condition , string.Empty );
        }

        public AND And( object name , Condition condition , object value )
        {
            AddConditionExpression( GetParameterName( name ) , QueryPart.AND, condition , value );
            return new AND( Path , this );
        }

        public OR Or( object name , Condition condition )
        {
            AddConditionExpression( GetParameterName( name ) , QueryPart.OR , condition , string.Empty );
            return new OR( Path , this );
        }

        internal virtual OR Or( object[] names , Condition condition , object[] values )
        {
            AddConditionExpression( GetParameterNames( names ) , QueryPart.OR , condition , values );
            return new OR( Path , this );
        }

        public GroupBy GroupBy( params object[] properties )
        {
            AddProjectionExpression( GetParameterNames( properties ) , QueryPart.GroupBy );
            return new GroupBy( Path , this );
        }

        public OrderBy OrderByDescending( params object[] parameters )
        {
            AddProjectionExpression( GetParameterNames( parameters ) , QueryPart.ORDERBY );
            return new OrderBy( Path , this , ORDERBY.DESC );
        }

        public OrderBy OrderBy( params object[] parameters )
        {
            AddProjectionExpression( GetParameterNames( parameters ) , QueryPart.ORDERBY );
            return new OrderBy( Path , this , ORDERBY.ASC );
        }
    }

    public class AS<TEntity> : AS
    {
        internal AS(PathExpressionFactory path) : base(path) { }

        public Constraints<TEntity, Where<TEntity>, object> Where(Func<TEntity, object> property)
        {
            return new Constraints<TEntity, Where<TEntity>, object>(Invoke(property), QueryPart.WHERE, new Where<TEntity>( Path , (AS)this ) );
        }

        public StringConstraints<TEntity, Where<TEntity>, string> Where(Func<TEntity, string> property)
        {
            return new StringConstraints<TEntity, Where<TEntity>, string>(Invoke(property), QueryPart.WHERE, new Where<TEntity>( Path , (AS) this ));
        }

        public Constraints<TEntity, Where<TEntity>, bool> Where(Func<TEntity, bool> func)
        {
            return new Constraints<TEntity, Where<TEntity>, bool>(Invoke(func), QueryPart.WHERE, new Where<TEntity>( Path , (AS)this));
        }

        public IEnumerableConstraints<TEntity, Where<TEntity>, object> Where(Func<TEntity, IEnumerable> property)
        {
            return new IEnumerableConstraints<TEntity, Where<TEntity>, object>(Invoke(property), QueryPart.WHERE, new Where<TEntity>( Path , (AS)this ));
        }

        public DateTimeConstraints<TEntity, Where<TEntity>, DateTime> Where(Func<TEntity, DateTime> property)
        {
            return new DateTimeConstraints<TEntity, Where<TEntity>, DateTime>(Invoke(property), QueryPart.WHERE, new Where<TEntity>( Path , (AS)this ));
        }

        public FunctionalKeywords<TEntity, Where<TEntity>> WhereBinary(Expression<Func<TEntity, object>> expression)
        {
            Where<TEntity> functionalKeywords = new Where<TEntity>( Path , this );

            return (FunctionalKeywords<TEntity,Where<TEntity>>) functionalKeywords.AddBinary( expression );
        }

        #region Functional Select Region

        public IList<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return new SelectConstraints<TEntity>(Path, Identifier).Select(selector);
        }

        public TResult Average<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return new SelectConstraints<TEntity>(Path, Identifier).Average(selector);
        }

        public TResult Min<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return new SelectConstraints<TEntity>(Path, Identifier).Min(selector);
        }

        public TResult Max<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return new SelectConstraints<TEntity>(Path, Identifier).Max(selector);
        }

        public int Count()
        {
            return new SelectConstraints<TEntity>(Path, Identifier).Count();
        }        

        #endregion

        private object Invoke<T>( Func<TEntity,T> func )
        {
            return func.Invoke((TEntity) Path.QueryableItem );
        }
    }
}