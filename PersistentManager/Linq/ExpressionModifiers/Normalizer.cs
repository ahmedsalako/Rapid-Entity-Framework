using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections;

namespace PersistentManager.Linq
{
    internal class Normalizer : ExpressionVisitor
    {
        private readonly static MethodInfo isNotNullMethod = typeof(Normalizer)
                                                                    .GetMethod("IsNotNull",
                                                                               BindingFlags.Static |
                                                                               BindingFlags.Public);

        private readonly static MethodInfo getDateValue = typeof(Normalizer)
                                                                    .GetMethod("GetDateValue",
                                                                               BindingFlags.Static |
                                                                               BindingFlags.Public);

        private readonly static MethodInfo getStringLength = typeof(Normalizer)
                                                                    .GetMethod("GetStringLength",
                                                                               BindingFlags.Static |
                                                                               BindingFlags.Public);

        private readonly static MethodInfo getCountIEnumerable = typeof(Normalizer)
                                                                    .GetMethod("Count",
                                                                               BindingFlags.Static |
                                                                               BindingFlags.Public );

        public Expression Normalize( Expression e )
        {
            return this.Visit(e);
        }

        protected override Expression VisitMemberAccess( MemberExpression method )
        {
            if (method.Member.DeclaringType.Name == "Nullable`1")
            {
                switch (method.Member.Name)
                {
                    case "Value":
                        return method.Expression;
                    case "HasValue":
                        return Expression.Call(isNotNullMethod.MakeGenericMethod(method.Expression.Type),
                                               method.Expression);
                }
            }

            if (method.Expression is MemberExpression)
            {

                var member = (MemberExpression)method.Expression;

                //while (member.Expression as MemberExpression != null)
                //{
                //    member = (MemberExpression)member.Expression;
                //}

                if (method.Member.DeclaringType == typeof(DateTime))
                {
                    return Expression.Call( getDateValue.MakeGenericMethod( method.Type ),
                                           new Expression[]{member, 
                                                                Expression.Constant(method.Member.Name)});
                }

                if ( method.Member.DeclaringType.IsCollection( ) && method.Member.Name == "Count" )
                {
                    //Expression expression = ( ( LambdaExpression) member).Operand;
                    return Expression.Call( getCountIEnumerable.MakeGenericMethod( typeof(int) ) ,
                                           new Expression[] {  member , Expression.Constant( 0 ) } );
                }

                if (method.Member.DeclaringType == typeof(string))
                {
                    return Expression.Call(getStringLength, member);
                }
            }

            return method;
        }

        protected override Expression VisitConditional(ConditionalExpression c)
        {
            if ((bool)(c.Test as ConstantExpression).Value == true)
            {
                return c.IfTrue;
            }

            return c.IfFalse;
        }

        public static bool IsNotNull<T>(T o)
        {
            throw new NotImplementedException();
        }

        public static T Count<T>( Expression expression , int count )
        {
            throw new NotImplementedException( );
        }

        public static T GetDateValue<T>(Expression expression, string datePart)
        {
            throw new NotImplementedException();
        }

        public static int GetStringLength( Expression expression )
        {
            throw new NotImplementedException();
        }
    }
}
