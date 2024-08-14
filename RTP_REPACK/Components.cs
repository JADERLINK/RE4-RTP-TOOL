using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTP_REPACK
{
    public class FinalPoint
    {
        public byte ID; // repetindo o id
        public float X;
        public float Y;
        public float Z;

        public ushort ConnectionTableIndex;
        public ushort ConnectionCount;

        public override string ToString()
        {
            return "ID: " + ID + "  X: " + X + "  Y: " + Y + "  Z: " + Z + "  ConnectionTableIndex: " + ConnectionTableIndex + "  ConnectionCount: " + ConnectionCount;
        }
    }


    public class Block2
    {
        public ushort ConnectionIndex;
        public ushort Distance;

        public override string ToString()
        {
            return "ConnectionIndex: " + ConnectionIndex + "  Distance: " + Distance;
        }
    }

    public class Point
    {
        public int ID;
        public float X;
        public float Y;
        public float Z;

        public override string ToString()
        {
            return "ID: " + ID + "  X: " + X + "  Y: " + Y + "  Z: " + Z;
        }
    }

    public class Conection
    {
        public ushort p1 = 0xff;
        public ushort p2 = 0xff;

        public bool p1_to_p2 = false;
        public bool p2_to_p1 = false;

        public override string ToString()
        {
            return "P1: " + p1 + "  p1_to_p2: " + p1_to_p2 + "  P2: " + p2 + "  p2_to_p1: " + p2_to_p1;
        }

    }

    public class ConnectionKey
    {
        private ushort key1;
        private ushort key2;

        public ushort Key1 { get { return key1; } }
        public ushort Key2 { get { return key2; } }

        public ConnectionKey(ushort key1, ushort key2)
        {
            this.key1 = key1;
            this.key2 = key2;
        }

        public override string ToString()
        {
            return key1 + " _ " + key2;
        }

        public override bool Equals(object obj)
        {
            return obj is ConnectionKey k && k.key1 == key1 && k.key2 == key2;
        }

        public override int GetHashCode()
        {
            return (int)((Key1 * 0x10000u) + Key2);
        }
    }

}
