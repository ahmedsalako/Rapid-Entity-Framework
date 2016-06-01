using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Keywords;
using PersistentManager.Descriptors;
using PersistentManager.Util;
using PersistentManager.Query.Projections;
using PersistentManager.Metadata;

namespace PersistentManager.Query
{
    internal class PathExpressionFactory : This
    {        
        internal int Offset = 0;
        internal int PageSize = 0;

        internal AliasBuilder ALIASBuilder = new AliasBuilder( "t" );
        internal ProjectionBinder ProjectionBinder { get; set; }
        internal IList<string> AllAliases { get; set; }
        internal virtual object QueryableItem { get; set; }
        internal bool IsDistinct { get; set; }
        internal bool IsReversable { get; set; }
      
        internal string ALIAS
        {
            set
            {
                ALIASBuilder.ALIAS = value;
            }
            get
            {
                return ALIASBuilder.ALIAS;
            }
        }

        IDictionary<Guid , PathExpression> queryPaths = new Dictionary<Guid , PathExpression>( );
        IDictionary<string , PathExpression> participants = new Dictionary<string , PathExpression>( );        

        List<NameResolver> groupBys = new List<NameResolver>( );
        List<Criteria> selectArguments = new List<Criteria>( );
        List<Criteria> outerCriterias = new List<Criteria>( );
        
        internal ORDERBY ORDERBY { get; set; }

        internal PathExpressionFactory( )
            : base( )
        {
        }

        internal PathExpressionFactory( Type queryAble ) : base()
        {
            Main = new PathExpression( true , queryAble );
        }

        internal PathExpressionFactory( Type queryAble , Keyword keyword )
            : this( queryAble )
        {
            this.Keyword = keyword;
        }

        internal PathExpressionFactory( Type queryAble , string ALIAS )
            : this( queryAble )
        {
            this.ALIAS = ALIAS;
        }

        internal PathExpressionFactory( PathExpression path , List<Criteria> outerCriterias , string ALIAS )
            : base()
        {
            this.ALIAS = ALIAS;
            this.Main = path;
            this.Main.IsMain = true;
            this.Main.ALIAS = GetNextAlias( );
            this.OuterCriterias = outerCriterias;
        }

        internal PathExpression Main { get; set; }
        internal IDictionary<Guid , PathExpression> QueryPaths
        {
            get { return queryPaths; }
            set { queryPaths = value; }
        }
        internal IDictionary<string , PathExpression> Participants
        {
            get { return participants; }
        }

        internal int ALIASCount { get; set; }
        internal int ProcessCount { get; set; }
        internal ProjectionFunction MainFunctionType { get; set; }
        
        internal List<Criteria> OuterCriterias { get { return outerCriterias; } set { outerCriterias = value; } }

        internal PathExpression Create( string property , Type type , Criteria criteria )
        {
            return new PathExpression( property , type , criteria );
        }

        internal PathExpression Create( string property , Type type )
        {
            return new PathExpression( property , type );
        }

        internal List<Criteria> SelectArguments
        {
            get { return selectArguments;  }
        }

        internal List<NameResolver> GroupBys
        {
            get { return groupBys; }
        }

        internal void AddSelectArgument( Criteria criteria )
        {
            selectArguments.Add( criteria );
        }

        internal void AddGroupBy( string property , Criteria criteria )
        {
            var key = groupBys.FirstOrDefault( c => c.Name == criteria.Name.ToString( ) );

            if ( key.IsNotNull( ) )
            {
                key.Criteria = criteria;
            }
        }

        internal void AddCriteria( SyntaxContainer syntax , PathExpression path , Criteria criteria )
        {
            if ( (!criteria.IsJoined( ) && !criteria.IsGroupingCriteria( )) || criteria.IsJoinAndProjected() ||  criteria.IsJoinedAndSelect() )
            {
                syntax.Add( criteria.QueryPart , criteria );
            }
            else if( criteria.IsJoinedWithAny( path.Criterias ) && !criteria.IsJoinedAndSelect() )
            {
                Criteria join = criteria.GetRightSideJoin( path.Criterias );

                if ( join.IsNull( ) )
                {
                    join = criteria.GetLeftSideJoin( path.Criterias );
                }

                criteria.Value = join.FullyQualifiedName;

                syntax.Add( criteria.QueryPart , criteria );
            }
            else if ( ProcessCount > 0 && syntax.EntityALIAS != Main.ALIAS )
            {
                criteria.QueryPart = QueryPart.SELECT;
                syntax.Add( QueryPart.SELECT , criteria );
            }
        }

        internal void AddScaledUpSelect( SyntaxContainer syntax , string ALIAS , List<Criteria> criterias )
        {
            foreach ( var current in criterias.Where( c => c.QueryPart == QueryPart.SELECT ) )
            {
                    Criteria clone = current.Clone( ) as Criteria;
                    clone.Name = current.Name ;
                    clone.Unassigned = true;
                    syntax.Add( QueryPart.SELECT , clone );
            }
        }

        internal void AddScaledUpCriteria( PathExpression path , SyntaxContainer syntax , Criteria criteria , List<Criteria> criterias , bool isScaleUp )
        {
            if ( path.IsMain )
            {
                if ( criteria.IsJoined( ) )
                {
                    foreach ( var current in path.Criterias.Where( cx => cx.IsJoinedWith( criteria ) && !criteria.IsJoinedAndSelect( ) ) )
                    {
                        Criteria clone = ( Criteria )current.Clone( );
                        clone.Name = current.Name;
                        clone.Value = criteria.FullyQualifiedName;
                        clone.QueryPart = current.QueryPart != QueryPart.NONE ? current.QueryPart : QueryPart.AND;
                        clone.Unassigned = false;
                        syntax.Add( clone.QueryPart , clone );
                    }
                }
                else if( !criteria.IsSelectCriteria() && !criteria.IsGroupingCriteria() )
                {
                    syntax.Add( criteria.QueryPart , criteria.Clone( ) as Criteria );
                }
            }
            else if ( path.Parent.IsNotNull( ) )
            {
                foreach ( var current in path.Parent.AllCriterias.Where( cx => cx.IsJoinedWith( criteria ) && !criteria.IsJoinedAndSelect( ) ) )
                {
                    Criteria clone = ( Criteria ) criteria.Clone( );
                    clone.Name = criteria.Name;
                    //clone.Value = criteria.FullyQualifiedName;
                    clone.QueryPart = QueryPart.SELECT;
                    clone.Unassigned = isScaleUp ? true : false;

                    current.Value = criteria.FullyQualifiedName;

                    syntax.Add( clone.QueryPart , clone );
                }
            }

            foreach ( var current in criterias.Where( cx => cx.IsJoinedWith( criteria ) && !criteria.IsJoinedAndSelect( ) ) )
            {
                Criteria clone = ( Criteria )current.Clone( );
                clone.Name = current.Name;
                clone.Value = criteria.FullyQualifiedName;
                clone.QueryPart = QueryPart.AND;
                clone.Unassigned = isScaleUp ? true : false;

                syntax.Add( clone.QueryPart , clone );
            }
        }

        internal void AddSelectArguments( SyntaxContainer syntax , PathExpression Path )
        {
            if ( Path.FunctionType != ProjectionFunction.NOTSET )
            {
                syntax.Add( QueryPart.FUNCTION , Path.FunctionType , 1 );
            }
            //else if( !Path.HasSelectProjections() ) 
            //{
            //    syntax.Add( QueryPart.SELECT );
            //}
        }

        internal void AddGroupingCriteria( SyntaxContainer syntax , PathExpression Path )
        {
            if ( Path.HasGroupingCriterias( ) || Path.HasGroupingScaledCriterias( ) )
            {
                foreach ( Criteria criteria in Path.AllCriterias.Where( c => c.IsGroupingCriteria( ) ) )
                {
                    syntax.Add( criteria.QueryPart , criteria );
                }
            }
        }

        internal string AppendPath( string path , PropertyMetadata property )
        {
            if ( path.IsNullOrEmpty( ) )
                return property.ClassDefinationName;

            return string.Concat( path , "." + property.ClassDefinationName );
        }

        internal void AddOuterCriteria( SyntaxContainer syntax , PathExpression queryPath , string ALIAS )
        {
            if ( ( queryPath.IsMain && OuterCriterias.Count > 0 ) || queryPath.ScaledUpCriterias.Count( c => c.HasCorrelatedPath( ) ) > 0 )
            {
                foreach ( var criteria in queryPath.Criterias )
                {
                    AddScaledUpCriteria( queryPath , syntax , criteria , OuterCriterias , false );
                }

                foreach ( var criteria in queryPath.ScaledUpCriterias )
                {
                    AddScaledUpCriteria( queryPath , syntax , criteria , OuterCriterias , true );
                }

                foreach ( var current in queryPath.ScaledUpCriterias.Where( c => c.HasCorrelatedPath( ) ) )
                {
                    if ( current.HasCorrelatedPath( ) )
                    {
                        current.QueryPart = QueryPart.AND;
                        current.Unassigned = true;

                        syntax.Add( current.QueryPart , current );
                    }
                }
            }
        }

        internal SyntaxContainer CreateQuery( SyntaxContainer syntax , PathExpression queryPath , string ALIAS )
        {
            foreach ( var criteria in queryPath.Criterias  )
            {
                AddCriteria( syntax , queryPath , criteria );
            }

            foreach ( var criteria in queryPath.ScaledUpCriterias )
            {
                AddScaledUpCriteria( queryPath , syntax , criteria , queryPath.Criterias , true );
            }

            AddOuterCriteria( syntax , queryPath , ALIAS );
            AddScaledUpSelect( syntax , syntax.EntityALIAS , queryPath.ScaledUpCriterias );

            ProcessCount++;

            foreach ( var reference in queryPath.References )
            {
                string currentProperty = reference.Value.Property; 
                PathExpression currentPath = reference.Value;

                EntityMetadata metadata = EntityMetadata.GetMappingInfo( queryPath.Type );
                PropertyMetadata property = metadata.GetPropertyMappingIncludeBase( currentProperty );

                AddRelatedQuery( currentPath , syntax );
            }

            foreach ( var join in queryPath.Joins )
            {
                Type currentType = join.Key; PathExpression currentPath = join.Value;

                AddJoinQuery( currentPath , syntax );
            }

            foreach ( var embedded in queryPath.Embeddeds )
            {
                syntax.JoinEmbeddedReference.Add( embedded.Value.ALIAS , new KeyValuePair<string, Type>( embedded.Value.ALIAS , embedded.Key ) );
                syntax.JoinEmbeddedRelation.Add( embedded.Value.ALIAS , new KeyValuePair<string, string>( embedded.Value.Parent.ALIAS , embedded.Value.Parent.MetaData.SchemaName ));
            }

            if ( queryPath.HasBase )
            {
                PathExpression baseExpression = queryPath.Base;

                do
                {
                    syntax.InheritanceReference.Add( baseExpression.ALIAS , baseExpression.MetaData.SchemaName );
                    baseExpression = baseExpression.Base;

                } 
                while ( baseExpression.IsNotNull( ) );
            }

            return syntax;
        }

        private void AddRelatedQuery( PathExpression currentPath , SyntaxContainer syntax )
        {
            syntax.FromClauseSubQueries.Add( currentPath.OuterALIAS , CreateQuery( new SyntaxContainer( currentPath.Type , QueryType.Select , currentPath.ALIAS ) , currentPath , currentPath.OuterALIAS ) );
        }

        private void AddJoinQuery( PathExpression currentPath , SyntaxContainer syntax )
        {
            syntax.FromClauseSubQueries.Add( currentPath.OuterALIAS , CreateQuery( new SyntaxContainer( currentPath.Type , QueryType.Select , currentPath.ALIAS ) , currentPath , currentPath.OuterALIAS ) );
        }

        internal SyntaxContainer IndexAll( )
        {
            AllAliases = Main.IndexAll( ALIASBuilder );

            SyntaxContainer syntax = new SyntaxContainer
            ( 
                Main.Type , 
                QueryType.NOTSET , 
                Main.ALIAS 
            );

            syntax.AllAliases = AllAliases;

            return GetSyntaxContainer( syntax , Main );
        }

        private SyntaxContainer GetSyntaxContainer( SyntaxContainer syntax , PathExpression queryPath )
        {
            syntax = CreateQuery( syntax , queryPath , queryPath.ALIAS );

            syntax.Range( Offset , PageSize );
            syntax.FunctionType = MainFunctionType;

            AddGroupingCriteria( syntax , queryPath ); //Do Grouping or Sorting
            AddSelectArguments( syntax , queryPath );  //select All if not exist

            syntax.HasProjectionBinding = ProjectionBinder.IsNotNull( );

            syntax.OrderBy = ORDERBY;
            syntax.IsDistinct = IsDistinct;

            List<Criteria> outerCriterias = new List<Criteria>( Main.AllCriterias );

            EnsureCorrelatedPaths( outerCriterias );

            foreach ( var outer in outerCriterias )
            {
                if ( outer.IsProjected )
                {
                    if ( outer.QueryPart != QueryPart.SELECT )
                    {
                        syntax.Add( QueryPart.SELECT , outer );
                    }
                }
            }

            syntax.Factory = this;

            RemoveCurrentScope( );

            return syntax;
        }

        internal string GetNextAlias( )
        {
            return ALIASBuilder.GetNextAlias( );
        }

        private void EnsureCorrelatedPaths( List<Criteria> criterias )
        {
            foreach ( Criteria criteria in criterias.Where( c => c.HasCorrelatedPath( ) ) )
            {
                PathExpressionFactory correlated = criteria.CorrelatedPath;
                correlated.OuterCriterias = criterias;
                correlated.ALIAS = "sub";
                correlated.QueryPaths.Clear( );

                criteria.CorrelatedPath.Main = correlated.Main;
                criteria.CorrelatedSubQuery = correlated.IndexAll( );

                //After Indexing a correlated then get the main to join the outermost criterias
            }
        }
    }
}