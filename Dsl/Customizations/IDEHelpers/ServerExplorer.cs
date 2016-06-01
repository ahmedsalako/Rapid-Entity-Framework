using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Data.Interop;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Data;
using EnvDTE80;
using EnvDTE;
using System.Data.Common;
using System.Data.SqlClient;

namespace consist.RapidEntity.Customizations.IDEHelpers
{
    internal class ServerExplorer : IDisposable
    {
        private IDSRefConsumer _consumer;
        private static readonly IntPtr RootNode = IntPtr.Zero;
        public static readonly string DataSourceFormat = "CF_DSREF";
        public static readonly string SelectedNode = "DSREF";
        [DllImport("ole32.dll")]
        private static extern int CreateStreamOnHGlobal(IntPtr ptr, bool delete,
                                                        ref IntPtr stream);

        [DllImport("ole32.dll")]
        private static extern int OleLoadFromStream(IntPtr stream, byte[] iid,
                                                    ref IntPtr obj);

        public ServerExplorer(Stream data)
        {
            _consumer = null;

            IntPtr ptr = IntPtr.Zero;
            IntPtr stream = IntPtr.Zero;
            IntPtr native = IntPtr.Zero;

            try
            {
                byte[] buffer = new byte[data.Length];
                data.Seek(0, SeekOrigin.Begin);
                data.Read(buffer, 0, buffer.Length);

                native = Marshal.AllocHGlobal(buffer.Length);
                Marshal.Copy(buffer, 0, native, buffer.Length);

                int result = CreateStreamOnHGlobal(native, false, ref stream);
                Marshal.ThrowExceptionForHR(result);

                result = OleLoadFromStream(stream,
                                           typeof(IDSRefConsumer).GUID.ToByteArray(), ref ptr);
                Marshal.ThrowExceptionForHR(result);

                _consumer = Marshal.GetObjectForIUnknown(ptr) as IDSRefConsumer;
            }
            finally
            {
                Marshal.Release(ptr);
                Marshal.Release(stream);
                Marshal.Release(native);
            }
        }

        public void Dispose()
        {
            if (_consumer != null)
                Marshal.ReleaseComObject(_consumer);
        }

        public bool ContainsOnlyTables
        {
            get
            {
                if (_consumer != null)
                {
                    try
                    {
                        return (_consumer.GetType(RootNode) &
                                __DSREFTYPE.DSREFTYPE_TABLE) == __DSREFTYPE.DSREFTYPE_TABLE;
                    }
                    catch
                    {
                    }
                }
                return false;
            }
        }

        public IEnumerable<string> Tables
        {
            get
            {
                foreach (ServerExplorerNode child in ChildNodes)
                {
                    if (child.IsTable && child.HasName)
                        yield return child.Name;
                }
            }
        }

        public IDSRefConsumer Consumer
        {
            get { return _consumer; }
            set { _consumer = value; }
        }


        public IEnumerable<ServerExplorerNode> ChildNodes
        {
            get
            {
                Queue<ServerExplorerNode> parents = new Queue<ServerExplorerNode>();
                parents.Enqueue(new ServerExplorerNode(RootNode, this));

                while (parents.Count > 0)
                {
                    ServerExplorerNode parent = parents.Dequeue();
                    ServerExplorerNode child = parent.GetFirstChildNode();

                    while (child != null)
                    {
                        yield return child;
                        parents.Enqueue(child);
                        child = child.GetNextSiblingNode();
                    }
                }
            }
        }

        public IEnumerable<ServerExplorerNode> ChildTableNodes
        {
            get
            {
                Queue<ServerExplorerNode> parents = new Queue<ServerExplorerNode>();
                parents.Enqueue(new ServerExplorerNode(RootNode, this));
                while (parents.Count > 0)
                {
                    ServerExplorerNode parent = parents.Dequeue();
                    ServerExplorerNode child = parent.GetFirstChildNode();

                    while (child != null)
                    {
                        if (child.IsTable && child.HasName)
                            yield return child;

                        parents.Enqueue(child);
                        child = child.GetNextSiblingNode();
                    }
                }
            }
        }


        public string GetConnectionName()
        {
            foreach (ServerExplorerNode child in FirstLevelChildNodes)
            {
                if (child.IsConnection && child.HasName)
                {
                    return child.Name;
                }
            }

            return null;
        }

        public IEnumerable<ServerExplorerNode> FirstLevelChildNodes
        {
            get
            {
                ServerExplorerNode root = new ServerExplorerNode(RootNode, this);
                ServerExplorerNode child = root.GetFirstChildNode();

                while (child != null)
                {
                    yield return child;
                    child = child.GetNextSiblingNode();
                }
            }
        }
    }
}
