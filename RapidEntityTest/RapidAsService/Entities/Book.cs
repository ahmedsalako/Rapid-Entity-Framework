using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;

namespace Persistent.Entities
{
    [Entity("boeken")]
    public class Book
    {
        [Field("boek_id", AutoKey = true, IsUnique=true)]
        public virtual int BookID { get; set; }
        [Field("datum_eerste_aanschaf", false, true)]
        public virtual DateTime RegisterDate { get; set; }
        [Field("titel", false, true)]
        public virtual string Title { get; set; }
        [Field("subtitel", false, true)]
        public virtual string SubTitle { get; set; }
        [Field("Etalage", false, true)]
        public virtual bool InShowroom { get; set; }
    }
}
