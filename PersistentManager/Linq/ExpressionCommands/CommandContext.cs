using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using PersistentManager.Query;

namespace PersistentManager.Linq.ExpressionCommands
{
    internal class CommandContext
    {
        internal MethodCallExpression CurrentCall { get; set; }
        internal Expression Expression { get; set; }
        internal PathExpressionFactory Factory { get; set; }
    }
}
