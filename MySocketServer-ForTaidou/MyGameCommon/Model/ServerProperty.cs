using System;
using System.Collections.Generic;
using System.Text;

namespace MyGameCommon.Model
{
    public class ServerProperty
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string IP { get; set; }
        public virtual int Count { get; set; }

    }
}
