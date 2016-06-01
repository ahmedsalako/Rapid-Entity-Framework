using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using PersistentManager.Query.Sql;
using PersistentManager.Provider.Functions;

namespace PersistentManager.Provider
{
    internal abstract class DatabaseProvider : IDatabaseProvider
    {
        private string queryLog = string.Empty;
        event RegisterDataReader RegisterReaderEvent;

        public IDbTransaction Transaction { get; set; }
        public IDbConnection Connection { get; set; }

        internal DatabaseProvider( IDbConnection connection , IDbTransaction transaction )
        {
            Connection = connection;
            Transaction = transaction;
        }

        protected virtual IDbCommand CreateCommand( )
        {
            return Connection.CreateCommand( );
        }

        protected virtual IDbCommand CreateCommand( string query , List<DbParameter> parameterCollection )
        {
            IDbCommand command = CreateCommand( );
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            command.Transaction = Transaction;
            command.Connection = Connection;

            AddParameters( command , parameterCollection );

            return command;
        }

        public DbDataReader GetDataReader( string query )
        {
            return GetDataReader( query , new List<DbParameter>( ) );
        }

        public DbDataReader GetDataReader( string query , List<DbParameter> parameterCollection )
        {
            try
            {
                IDataReader dataReader = CreateCommand(query, parameterCollection).ExecuteReader();

                if (SupportsMultipleActiveReader)
                    return RaiseRegisterDataReaderEvent( dataReader );

                return RaiseRegisterDataReaderEvent(new InternalReader(dataReader));
            }
            catch (Exception ex)
            {
                throw new Exception( ex.Message );
            }
            finally
            {
                queryLog = query;
            }
        }

        public int ExecuteUpdate( string query )
        {
            return ExecuteUpdate( query , new List<DbParameter>( ) );
        }

        public int ExecuteUpdate( string query , List<DbParameter> parameterCollection )
        {
            try
            {
                return CreateCommand(query, parameterCollection).ExecuteNonQuery();
            }
            finally
            {
                queryLog = query;
            }
        }

        public object ExecuteScalar( string query )
        {
            try
            {
                return CreateCommand( query , new List<DbParameter>( ) ).ExecuteScalar( );
            }
            catch ( Exception e )
            {
                throw new Exception( e.Message );
            }
            finally
            {
                queryLog = query;
            }
        }

        public object ExecuteScalar( string query , List<DbParameter> parameterCollection )
        {
            try
            {
                return CreateCommand(query, parameterCollection).ExecuteScalar();
            }
            finally
            {
                queryLog = query;
            }
        }

        public virtual int ExecuteUpdateProcedure( string procedureName , List<DbParameter> parameters )
        {
            IDbCommand command = CreateCommand( procedureName , parameters );
            command.CommandType = CommandType.StoredProcedure;

            return command.ExecuteNonQuery( );
        }

        public virtual void AddParameters( IDbCommand command , List<DbParameter> parameters )
        {
            foreach ( DbParameter parameter in parameters )
                command.Parameters.Add( parameter );
        }

        public virtual IDataReader ExecuteStoredProcedure( string procedureName , List<DbParameter> parameters )
        {
            IDbCommand command = CreateCommand( procedureName , parameters );
            command.CommandType = CommandType.StoredProcedure;

            return RaiseRegisterDataReaderEvent( command.ExecuteReader( ) );
        }

        public virtual DbDataReader RaiseRegisterDataReaderEvent( IDataReader dataReader )
        {
            if ( !RegisterReaderEvent.IsNull( ) )
            {
                RegisterReaderEvent( dataReader );
            }

            return dataReader as DbDataReader;
        }

        public virtual event RegisterDataReader RegisterReader
        {
            add
            {
                if ( !RegisterReaderEvent.IsNull( ) )
                {
                    RegisterReaderEvent += value;
                }
                else
                {
                    RegisterReaderEvent = new RegisterDataReader( value );
                }
            }
            remove
            {
                if ( !RegisterReaderEvent.IsNull( ) )
                {
                    RegisterReaderEvent -= value;
                }
            }
        }

        public ProviderFunctions GetProviderFunctions()
        {
            return ProviderFunctions.GetAvailableFunctions( this );
        }

        public abstract DbParameter GetCommandParameter( string name , object value );
        public abstract DbDataReader Range( SQLTokenizer tokens , int StartRange , int Endrange , string query , List<DbParameter> parameters );
        public abstract SQLTokenizer GetFilteredQuerySyntax( SQLTokenizer Tokens , int index );
        public abstract int ExecuteUpdate( string query , ref DbParameter returnedValue , string columnName , List<DbParameter> parameters );
        public abstract string GetNamingStrategyString( );
        public abstract string GetParameterPrefix( );
        public virtual bool SupportsMultipleActiveReader { get { return true; } }

        public IDatabaseProvider Create( )
        {
            return this;
        }


        public string QueryLog
        {
            get
            {
                return queryLog;
            }
            set
            {
                queryLog = value;
            }
        }
    }
}
