using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using System.Collections;
using PersistentManager.Ghosting;
using System.Linq.Expressions;
using PersistentManager.Linq;
using PersistentManager.Linq.ExpressionCommands;

namespace PersistentManager.Query.Keywords
{
    public class FunctionalKeywords<TEntity, T> : Keyword where T : Keyword
    {
        internal T Current { get; set; }

        internal FunctionalKeywords() { }

        internal FunctionalKeywords( PathExpressionFactory path )
        {
            Path = path;
        }

        public Constraints<TEntity, T, object> Where(object property)
        {
            return new Constraints<TEntity, T, object>(property, QueryPart.WHERE, Current);
        }

        public StringConstraints<TEntity, T, string> Where(string property)
        {
            return new StringConstraints<TEntity, T, string>(property, QueryPart.WHERE, Current);
        }

        public DateTimeConstraints<TEntity, T, DateTime> Where(DateTime property)
        {
            return new DateTimeConstraints<TEntity, T, DateTime>(property, QueryPart.WHERE, Current);
        }

        public IEnumerableConstraints<TEntity, T, object> Where(IEnumerable property)
        {
            return new IEnumerableConstraints<TEntity, T, object>(property, QueryPart.WHERE, Current);
        }

        public Constraints<TEntity, T, bool> Where(bool property)
        {
            return new Constraints<TEntity, T, bool>(property, QueryPart.WHERE, Current);
        }

        public Constraints<TEntity, T, object> And(object property)
        {
            return new Constraints<TEntity, T, object>(property, QueryPart.AND, Current);
        }

        public StringConstraints<TEntity, T, string> And(string property)
        {
            return new StringConstraints<TEntity, T, string>(property, QueryPart.AND, Current);
        }

        public DateTimeConstraints<TEntity, T, DateTime> And(DateTime property)
        {
            return new DateTimeConstraints<TEntity, T, DateTime>(property, QueryPart.AND, Current);
        }

        public IEnumerableConstraints<TEntity, T, object> And(IEnumerable property)
        {
            return new IEnumerableConstraints<TEntity, T, object>(property, QueryPart.AND, Current);
        }

        public Constraints<TEntity, T, bool> And(bool property)
        {
            return new Constraints<TEntity, T, bool>(property, QueryPart.WHERE, Current);
        }

        public Constraints<TEntity, T, object> Or(object property)
        {
            return new Constraints<TEntity, T, object>(property, QueryPart.OR, Current);
        }

        public Constraints<TEntity, T, bool> Or(bool property)
        {
            return new Constraints<TEntity, T, bool>(property, QueryPart.WHERE, Current);
        }

        public StringConstraints<TEntity, T, string> Or(string property)
        {
            return new StringConstraints<TEntity, T, string>(property, QueryPart.OR, Current);
        }

        public DateTimeConstraints<TEntity, T, DateTime> Or(DateTime property)
        {
            return new DateTimeConstraints<TEntity, T, DateTime>(property, QueryPart.OR, Current);
        }

        public IEnumerableConstraints<TEntity, T, object> Or(IEnumerable property)
        {
            return new IEnumerableConstraints<TEntity, T, object>(property, QueryPart.OR, Current);
        }

        #region Functional Or

        public Constraints<TEntity, Where<TEntity>, object> Or( Func<TEntity, object> property )
        {
            return new Constraints<TEntity, Where<TEntity>, object>(Invoke(property), QueryPart.OR, new Where<TEntity>( Path , Identifier ));
        }

        public StringConstraints<TEntity, Where<TEntity>, string> Or(Func<TEntity, string> property)
        {
            return new StringConstraints<TEntity, Where<TEntity>, string>(Invoke(property), QueryPart.OR, new Where<TEntity>(Path, Identifier) );
        }

        public DateTimeConstraints<TEntity, Where<TEntity>, DateTime> Or(Func<TEntity, DateTime> property)
        {
            return new DateTimeConstraints<TEntity, Where<TEntity>, DateTime>(Invoke(property), QueryPart.OR, new Where<TEntity>(Path, Identifier));
        }

        public IEnumerableConstraints<TEntity, Where<TEntity>, IEnumerable> Or(Func<TEntity, IEnumerable> property)
        {
            return new IEnumerableConstraints<TEntity, Where<TEntity>, IEnumerable>(Invoke(property), QueryPart.OR, new Where<TEntity>(Path, Identifier));
        }

        private object Invoke<T>(Func<TEntity, T> func)
        {
            return func.Invoke((TEntity)Path.QueryableItem);
        }

        # endregion

        #region Functional And

        public Constraints<TEntity, Where<TEntity>, object> And(Func<TEntity, object> property)
        {
            return new Constraints<TEntity, Where<TEntity>, object>(Invoke(property), QueryPart.AND, new Where<TEntity>( Path , Identifier ));
        }

        public StringConstraints<TEntity, Where<TEntity>, string> And(Func<TEntity, string> property)
        {
            return new StringConstraints<TEntity, Where<TEntity>, string>(Invoke(property), QueryPart.AND, new Where<TEntity>( Path , Identifier ));
        }

        public DateTimeConstraints<TEntity, Where<TEntity>, DateTime> And(Func<TEntity, DateTime> property)
        {
            return new DateTimeConstraints<TEntity, Where<TEntity>, DateTime>(Invoke(property), QueryPart.AND, new Where<TEntity>( Path , Identifier ));
        }

        public IEnumerableConstraints<TEntity, Where<TEntity>, IEnumerable> And(Func<TEntity, IEnumerable> property)
        {
            return new IEnumerableConstraints<TEntity, Where<TEntity>, IEnumerable>(Invoke(property), QueryPart.AND, new Where<TEntity>( Path , Identifier ));
        }

        public FunctionalKeywords<TEntity,T> AddBinary(Expression<Func<TEntity, object>> expression)
        {

            BinaryEvaluator evaluator = new BinaryEvaluator();

            expression.Compile().Invoke((TEntity)Path.QueryableItem);

            foreach ( ExpressionAndType current in evaluator.GetBinaryExpressions(((UnaryExpression)expression.Body).Operand as BinaryExpression) )
            {
                ConstantExpression value = ((BinaryExpression)current.Expression).Right as ConstantExpression;
                Condition condition = ExpressionReader.GetCondition( current.Expression.NodeType );

                if ( value.Value == null )
                {
                    condition = current.Expression.NodeType == ExpressionType.NotEqual ? Condition.IsNotNull : Condition.IsNull;
                }                                

                AddConditionExpression( GetParameterName(null) , QueryPart.AND, condition , value.Value );
            }

            return this; // new FunctionalKeywords<TEntity>(Path, Identifier);
        }

        # endregion

        #region Functional Select Region

        public IList<TResult> Select<TResult>( Expression<Func<TEntity, TResult>> selector)
        {
            return new SelectConstraints<TEntity>(Path, Identifier).Select( selector );
        }

        public TResult Average<TResult>( Expression<Func<TEntity, TResult>> selector )
        {
            return new SelectConstraints<TEntity>(Path, Identifier).Average( selector );
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

        #region Functional Order By

        public SelectConstraints<TEntity> OrderByAscending(params Func<TEntity, object>[] properties )
        {
            List<object> orderBys = new List<object>();

            foreach (var property in properties)
            {
                orderBys.Add(Invoke(property));
            }

            Identifier.OrderBy( orderBys.ToArray() );

            return new SelectConstraints<TEntity>( Path , Identifier );
        }

        public SelectConstraints<TEntity> OrderByDescending(params Func<TEntity, object>[] properties)
        {
            List<object> orderBys = new List<object>();

            foreach (var property in properties)
            {
                orderBys.Add(Invoke(property));
            }

            Identifier.OrderByDescending(orderBys.ToArray());

            return new SelectConstraints<TEntity>(Path, Identifier);
        }

        # endregion

    }
}
