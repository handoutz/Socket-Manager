using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.NextGen.Types
{
    public interface IDataConverter
    {
        byte[] Serialize(object data, Encoding encoder);
    }

    public class StringDataConverter : IDataConverter
    {
        public byte[] Serialize(object data, Encoding encoder)
        {
            return encoder.GetBytes(data.ToString());
        }
    }
}
