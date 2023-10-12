using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTP_REPACK
{
    public class FinalStruture
    {
        public FinalPoint[] Block1Array;
        public Block2[] Block2Array;

        public void Fill(ref Dictionary<int, Point> PointList, ref Dictionary<ushort, List<Block2>> Block2List) 
        {
            Block1Array = new FinalPoint[PointList.Count];

            List<Block2> Block2Temp = new List<Block2>();

            ushort index = 0;

            for (int i = 0; i < PointList.Count; i++)
            {
                FinalPoint p = new FinalPoint();
                p.ID = (byte)i;
                p.X = PointList[i].X;
                p.Y = PointList[i].Y;
                p.Z = PointList[i].Z;

                p.ConnectionTableIndex = index;

                Block2[] connections = new Block2[0];

                if (Block2List.ContainsKey((ushort)i))
                {
                    connections = Block2List[(ushort)i].ToArray();
                }

                p.ConnectionCount = (ushort)connections.Length;
                index += (ushort)connections.Length;

                Block2Temp.AddRange(connections);

                Block1Array[i] = p;
            }

            Block2Array = Block2Temp.ToArray();
        }



    }


}
