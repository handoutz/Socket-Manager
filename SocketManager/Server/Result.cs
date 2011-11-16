using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SocketManager
{
    public class Result
    {
        private byte[] _buff;
        public byte[] RawBuffer { get { return _buff; } }

        private Client cli;
        public Client RemoteClient { get { return cli; } }

        private Encoding Encoder;

        public Result() : this(null, null, Encoding.ASCII) { }
        public Result(byte[] b, Client c, Encoding e)
        {
            _buff = b;
            cli = c;
            Encoder = e;
        }
        public Result(byte[] b, Client c) : this(b, c, Encoding.ASCII) { }
        public string AsString()
        {
            string r = Encoder.GetString(_buff);
            r = Regex.Replace(r, "[^a-zA-Z0-9]", "");
            return r;
        }
        public override string ToString()
        {
            return AsString();
        }
    }
}
