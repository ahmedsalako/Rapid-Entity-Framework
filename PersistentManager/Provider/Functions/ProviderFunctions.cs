using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query;

namespace PersistentManager.Provider.Functions
{
    internal abstract class ProviderFunctions
    {
        internal static ProviderFunctions GetAvailableFunctions( DatabaseProvider provider )
        {
            if ( provider is SqlServerProvider )
            {
                return new SqlServerFunctions( provider );
            }
            else if ( provider is OleDbProvider )
            {
                return new MSAccessFunctions( );
            }
            else if ( provider is MySqlProvider )
            {
                return new MySqlFunctions( );
            }
            else if ( provider is OracleClientProvider )
            {
                return new OracleFunctions( );
            }
            else if ( provider is Db2DatabaseProvider )
            {
                return new Db2Functions( );
            }
            else if( provider is SQLLiteDatabaseProvider )
            {
                return new SQLLite3Functions( );
            }

            throw new Exception( " Functions are not supported against this provider " );
        }

        internal string GetFunctionLiteral( object Operand , FunctionCall[] _operators )
        {
            string operand = Operand.ToString( );

            foreach ( FunctionCall _operator in _operators )
            {
                operand += GetFunctionLiteral( operand , _operator );
            }

            return operand;
        }

        internal string GetFunctionLiteral( object Operand , FunctionCall _operator )
        {
            return GetFunctionLiteral( Operand , string.Empty , _operator );
        }

        internal string GetFunctionLiteral( object leftOperand , object rightOperand , FunctionCall _operator )
        {
            switch ( _operator )
            {
                case FunctionCall.Add:
                    return MakeBasicArithmetic( leftOperand , "+" , rightOperand );
                case FunctionCall.Subtract:
                    return MakeBasicArithmetic( leftOperand , "-" , rightOperand );
                case FunctionCall.Multiply:
                    return MakeBasicArithmetic( leftOperand , "*" , rightOperand );
                case FunctionCall.Division:
                    return MakeBasicArithmetic( leftOperand , "/" , rightOperand );
                case FunctionCall.Coalesce:
                    return Coalesce( leftOperand , rightOperand );
                case FunctionCall.Concat:
                    return StringConcatenation( leftOperand , rightOperand );
                case FunctionCall.LowerCase:
                    return LowerCase( leftOperand );
                case FunctionCall.Trim:
                    return Trim( leftOperand );
                case FunctionCall.TrimLeft:
                    return TrimLeft( leftOperand );
                case FunctionCall.TrimRight:
                    return TrimRight( leftOperand );
                case FunctionCall.UpperCase:
                    return UpperCase( leftOperand );
                case FunctionCall.Month:
                    return Month( leftOperand );
                case FunctionCall.Year:
                    return Year( leftOperand );
                case FunctionCall.Day:
                    return Day( leftOperand );
                case FunctionCall.Second:
                    return Seconds( leftOperand );
                case FunctionCall.Hour:
                    return Hour( leftOperand );
                case FunctionCall.Minute:
                    return Minute( leftOperand );
                case FunctionCall.Max:
                    return Max( leftOperand );
                case FunctionCall.Min:
                    return Min( leftOperand );
                case FunctionCall.Sum:
                    return Sum( leftOperand );
                case FunctionCall.Avg:
                    return Average( leftOperand );
                case FunctionCall.Count:
                    return Count( leftOperand );
                case FunctionCall.StringLength:
                    return Length( leftOperand );
            }

            throw new Exception( "Operator not allowed " );
        }

        protected virtual string Length( object leftOperand )
        {
            return string.Format( "LEN({0})" , leftOperand );
        }

        protected virtual string Count( object LeftOperand )
        {
            return string.Format( "COUNT({0})" , LeftOperand );
        }

        protected virtual string Max( object LeftOperand )
        {
            return string.Format( "MAX({0})" , LeftOperand );
        }

        protected virtual string Min( object LeftOperand )
        {
            return string.Format( "MIN({0})" , LeftOperand );
        }

        protected virtual string Sum( object LeftOperand )
        {
            return string.Format( "SUM({0})" , LeftOperand );
        }

        protected virtual string Average( object LeftOperand )
        {
            return string.Format( "AVG({0})" , LeftOperand );
        }

        protected virtual string MakeBasicArithmetic( object leftOperand , string _operator , object rightOperand )
        {
            return string.Format( " ( {0} {1} {2} ) " , leftOperand , _operator , rightOperand );
        }

        protected virtual string StringConcatenation( object leftOperand , object rightOperand )
        {
            return string.Format( "CONCAT({0}, {1})" , leftOperand , rightOperand );
        }

        protected virtual string Coalesce( object leftOperand , object rightOperand )
        {
            return string.Format( "COALESCE({0},{1})" , leftOperand , rightOperand );
        }

        protected virtual string TrimLeft( object leftOperand )
        {
            return string.Format( "LTRIM({0})" , leftOperand );
        }

        protected virtual string TrimRight( object leftOperand )
        {
            return string.Format( "RTRIM({0})" , leftOperand );
        }

        protected virtual string LowerCase( object leftOperand )
        {
            return string.Format( "LOWER({0})" , leftOperand );
        }

        protected virtual string UpperCase( object leftOperand )
        {
            return string.Format( "UPPER({0})" , leftOperand );
        }

        protected virtual string Trim( object leftOperand )
        {
            return string.Format( "TRIM{0})" , leftOperand );
        }

        protected virtual string Month( object leftOperand )
        {
            return string.Empty;
        }

        protected virtual string Year( object leftOperand )
        {
            return string.Empty;
        }

        protected virtual string Day( object leftOperand )
        {
            return string.Empty;
        }

        protected virtual string Hour( object leftOperand )
        {
            return string.Empty;
        }

        protected virtual string Minute( object leftOperand )
        {
            return string.Empty;
        }

        protected virtual string Seconds( object leftOperand )
        {
            return string.Empty;
        }
    }
}
