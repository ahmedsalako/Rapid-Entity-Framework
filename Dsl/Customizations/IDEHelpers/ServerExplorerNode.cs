using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Data.Interop;
using System.Runtime.InteropServices;

namespace consist.RapidEntity.Customizations.IDEHelpers
{
    internal class ServerExplorerNode
    {
        private IntPtr _pointer;
        private ServerExplorer serverExplorer = null;
        private string _name = null;
        private string _owner = null;
        private __DSREFTYPE _type = __DSREFTYPE.DSREFTYPE_NULL;

        public bool HasName
        {
            get { return ((DSRefType & __DSREFTYPE.DSREFTYPE_HASNAME) == __DSREFTYPE.DSREFTYPE_HASNAME); }
        }

        public bool IsTable
        {
            get { return ((DSRefType & __DSREFTYPE.DSREFTYPE_TABLE) == __DSREFTYPE.DSREFTYPE_TABLE); }
        }

      public bool IsConnection {
        get { return ((DSRefType & __DSREFTYPE.DSREFTYPE_DATABASE) == __DSREFTYPE.DSREFTYPE_DATABASE); }
      }

        public __DSREFTYPE DSRefType
        {
            get
            {
                if (_type == __DSREFTYPE.DSREFTYPE_NULL)
                    _type = serverExplorer.Consumer.GetType(_pointer);

                return _type;
            }
        }

        public string Name
        {
            get
            {
                if (_name == null)
                    _name = serverExplorer.Consumer.GetName(_pointer);

                return _name;
            }
        }

        public string Owner
        {
            get
            {
                if (_owner == null)
                    _owner = serverExplorer.Consumer.GetOwner(_pointer);

                return _owner;
            }
        }

        public IntPtr Pointer
        {
            get { return _pointer; }
        }

        public ServerExplorerNode(IntPtr pointer, ServerExplorer navigator)
        {
            _pointer = pointer;
            serverExplorer = navigator;
        }

        public ServerExplorerNode GetFirstChildNode()
        {
            IntPtr child = serverExplorer.Consumer.GetFirstChildNode(_pointer);
            if (child != IntPtr.Zero)
                return new ServerExplorerNode(child, serverExplorer);
            else
                return null;
        }

        public ServerExplorerNode GetNextSiblingNode()
        {
            IntPtr sibling = serverExplorer.Consumer.GetNextSiblingNode(_pointer);

            if (sibling != IntPtr.Zero)
                return new ServerExplorerNode(sibling, serverExplorer);
            else
                return null;
        }
    }
}
