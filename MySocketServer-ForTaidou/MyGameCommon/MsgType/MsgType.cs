using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGameCommon.MsgType
{
    public static class MsgType
    {

    }

    [ProtoContract]
    public class Response
    {
        [ProtoMember(1)]
        public bool IsSuccess
        {
            get;
            set;
        }

        [ProtoMember(2)]
        public string Error
        {
            get;
            set;
        }

        public List<string> data
        {
            get;
            set;
        }


        public override string ToString()
        {
            return "";
        }
    }
}
