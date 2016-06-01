using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Query.Processors
{
    internal class SequentialProcessor : PathExpressionProcessor
    {
        public List<string> AllAliases { get; set; }
        EntitySplitMergerProcessor entityMerger;
        AliasApplierProcessor aliasApplier;

        List<PathExpressionProcessor> processors;        

        internal SequentialProcessor( AliasBuilder AliasBuilder ) 
        {
            this.AllAliases = new List<string>();
            this.entityMerger = new EntitySplitMergerProcessor( );
            this.aliasApplier = new AliasApplierProcessor( AliasBuilder );            

            processors = new List<PathExpressionProcessor>( );
            processors.Add( new DiscriminatorProcessor( ) );
            processors.Add( new ExcludeCriteriaProcessor( ) );
            processors.Add( new CompositeKeyProcessor( ) );
            processors.Add( new RelationshipReferenceProcessor( ) );
            processors.Add( new RemoveSelfReferenceProcessor( ) );            
            processors.Add( new CriteriaScalarProcessor( ) );        
        }

        internal override void Process( PathExpression pathExpression )
        {
            new SelectCriteriaInjectorProcessor( ).Process( pathExpression );

            ProcessNextExpression( pathExpression );

            processors.ForEach( p => p.Process( pathExpression ) );
            
        }

        protected void ProcessNextExpression( PathExpression pathExpression )
        {
            entityMerger.Process( pathExpression );
            aliasApplier.Process( pathExpression );

            AllAliases.Add(pathExpression.ALIAS);
            AllAliases.Add(pathExpression.OuterALIAS);
                
            if( pathExpression.HasBase )
            {
                Process( pathExpression.Base );
            }

            foreach ( PathExpression path in pathExpression.References.Values.Where( p => p.UniqueId != pathExpression.UniqueId) ) Process( path );
            foreach ( PathExpression path in pathExpression.Joins.Values ) Process( path );
            foreach ( PathExpression path in pathExpression.Embeddeds.Values ) Process( path ) ;

            if ( pathExpression.Base.IsNotNull( ) )
            {
                foreach ( var reference in pathExpression.Base.References )
                {
                    pathExpression.References.Add( reference );
                }

                foreach ( var embedded in pathExpression.Base.Embeddeds )
                {
                    pathExpression.Embeddeds.Add( embedded );
                }
            }
        }
    }
}
