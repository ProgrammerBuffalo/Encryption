using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SymmetricEncryptClient
{
    [Serializable]
    public class Cypher
    {
        public byte[] Data { get; set; }

        public byte[] IV { get; set; }
    }
}
