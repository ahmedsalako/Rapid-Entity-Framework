using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace consist.RapidEntity.CodeGenerator
{
    public class UTF8StringWriter : StringWriter
    {
        Encoding encoding;

        public override Encoding Encoding
        {
            get
            {
                return encoding;
            }
        }

        public UTF8StringWriter( StringBuilder content ) : base(  content )
        {
            encoding = Encoding.UTF8;
        }
    }
}
