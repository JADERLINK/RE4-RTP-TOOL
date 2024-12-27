using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SimpleEndianBinaryIO;

namespace RTP_REPACK
{
    public static class FinalFile
    {
        public static void FinalRTP(string pathRTP, ref FinalPoint[] Block1Array, ref Block2[] Block2Array, ref byte[][] block3Array, bool IsPs2, bool isPS4NS, bool isBig) 
        {
            Endianness endianness = Endianness.LittleEndian;
            if (isBig)
            {
                endianness = Endianness.BigEndian;
            }

            EndianBinaryWriter RTP = new EndianBinaryWriter(new FileInfo(pathRTP).Create(), endianness);

            RTP.Write((uint)0x32525450);
            RTP.Write((ushort)0x0000);

            ushort block1Count = (ushort)Block1Array.Length;
            ushort block2Count = (ushort)Block2Array.Length;
            ushort block3Count = (ushort)(Block1Array.Length * Block1Array.Length);

            int block1offset;
            int block2offset;
            int block3offset;

            if (IsPs2)
            {
                block1offset = 0x20;

                block2offset = block1offset + (block1Count * 32);
            }
            else if (isPS4NS)
            {
                block1offset = 0x28;

                block2offset = block1offset + (block1Count * 16);
            }
            else //UHD or big
            {
                block1offset = 0x18;

                block2offset = block1offset + (block1Count * 16);
            }

            block3offset = block2offset + (block2Count * 4);

            //---
            RTP.Write(block1Count);
            RTP.Write(block2Count);
            RTP.Write(block3Count);

            if (isPS4NS)
            {
                RTP.Write((uint)0);

                RTP.Write((long)block1offset);
                RTP.Write((long)block2offset);
                RTP.Write((long)block3offset);
            }
            else //UHD ou PS2 ou Big
            {
                RTP.Write(block1offset);
                RTP.Write(block2offset);
                RTP.Write(block3offset);
            }
       

            RTP.BaseStream.Position = block1offset;

            RTP.Write(MakeBlock1(ref Block1Array, IsPs2, endianness));

            RTP.BaseStream.Position = block2offset;

            RTP.Write(MakeBlock2(ref Block2Array, endianness));

            RTP.BaseStream.Position = block3offset;


            for (int i = 0; i < block3Array.Length; i++)
            {
                RTP.Write(block3Array[i]);
            }

            // end pading
            int allBytes = block3offset + block1Count * block1Count;
            int rest = allBytes % 32;
            int diff = 32 - rest;
            RTP.Write(new byte[diff]);

            RTP.Close();
        }

        private static byte[] MakeBlock1(ref FinalPoint[] Block1Array, bool IsPs2, Endianness endianness) 
        {
            List<byte> b = new List<byte>();

            for (int i = 0; i < Block1Array.Length; i++)
            {
                byte[] x = EndianBitConverter.GetBytes((float)Block1Array[i].X, endianness);
                byte[] y = EndianBitConverter.GetBytes((float)Block1Array[i].Y, endianness);
                byte[] z = EndianBitConverter.GetBytes((float)Block1Array[i].Z, endianness);

                byte[] index = EndianBitConverter.GetBytes((ushort)Block1Array[i].ConnectionTableIndex, endianness);
                byte[] count = EndianBitConverter.GetBytes((ushort)Block1Array[i].ConnectionCount, endianness);

                b.AddRange(x);
                b.AddRange(y);
                b.AddRange(z);

                if (IsPs2)
                {
                    byte[] w = BitConverter.GetBytes((float)1.0f);
                    b.AddRange(w);
                }

                b.AddRange(index);
                b.AddRange(count);

                if (IsPs2)
                {
                    b.AddRange(new byte[12]);
                }
            }

            return b.ToArray();
        }

        private static byte[] MakeBlock2(ref Block2[] Block2Array, Endianness endianness) 
        {
            List<byte> b = new List<byte>();

            for (int i = 0; i < Block2Array.Length; i++)
            {
                b.AddRange(EndianBitConverter.GetBytes((ushort)Block2Array[i].ConnectionIndex, endianness));
                b.AddRange(EndianBitConverter.GetBytes((ushort)Block2Array[i].Distance, endianness));
            }

            return b.ToArray();
        }


        public static void FinalDebugTxt2(string pathTxt2, ref FinalPoint[] Block1Array, ref Block2[] Block2Array, ref byte[][] block3Array) 
        {
            var invariant = System.Globalization.CultureInfo.InvariantCulture;

            var txt = new FileInfo(pathTxt2).CreateText();
            txt.WriteLine(Program.headerText());
            txt.WriteLine();
            txt.WriteLine("Informational file only:");
            txt.WriteLine();
            txt.WriteLine();
            txt.WriteLine();

            ushort block1Count = (ushort)Block1Array.Length;
            ushort block2Count = (ushort)Block2Array.Length;
            ushort block3Count = (ushort)(Block1Array.Length * Block1Array.Length);

            txt.WriteLine("block1_Amount:" + block1Count);
            txt.WriteLine("block2_Amount:" + block2Count);
            txt.WriteLine("block3_Amount:" + block3Count);

            txt.WriteLine();
            txt.WriteLine();
            txt.WriteLine();
            txt.WriteLine();
            txt.WriteLine();

            txt.WriteLine("block1: (NodeTable)");

            for (int i = 0; i < Block1Array.Length; i++)
            {
                txt.WriteLine("block1[" + i.ToString("X4") + "];  X:" + Block1Array[i].X.ToString("f6", invariant).PadLeft(16) + "  Y: " +
                   Block1Array[i].Y.ToString("f6", invariant).PadLeft(16) + "  Z: " + Block1Array[i].Z.ToString("f6", invariant).PadLeft(16) +
                   "  connectionTableIndex: " + Block1Array[i].ConnectionTableIndex.ToString("X4") + "  connectionCount: " + Block1Array[i].ConnectionCount.ToString("X4"));
            }


            txt.WriteLine();
            txt.WriteLine("block2: (ConnectionTable)");

            for (int i = 0; i < Block2Array.Length; i++)
            {
                txt.WriteLine("block2[" + i.ToString("X4") + "]; connectionIndex: " + Block2Array[i].ConnectionIndex.ToString("X4") + "  distance: " + Block2Array[i].Distance.ToString("X4"));
            }

            txt.WriteLine();
            txt.WriteLine("block3: (Traveling salesman problem table)");

            txt.Write($"Lines|Columns");
            for (int i = 0; i < Block1Array.Length; i++)
            {
                txt.Write(" " + i.ToString("X2"));
            }
            txt.WriteLine("");

            for (int i = 0; i < block3Array.Length; i++)
            {
                txt.WriteLine($"block3[{i.ToString("X4")}]: " + BitConverter.ToString(block3Array[i]));
            }

            txt.WriteLine("");
            txt.WriteLine("All possible routes:");
            PrintRoutes(ref block3Array, ref txt);

            txt.WriteLine();
            txt.WriteLine("End file");

            txt.Close();
        }


        private static void PrintRoutes(ref byte[][] block3, ref StreamWriter txt)
        {
            for (int row = 0; row < block3.Length; row++) // ponto de partida
            {
                for (int column = 0; column < block3.Length; column++) // ponto de chegada
                {

                    if (block3[row][column] != 0xFF && block3[row][column] != row)
                    {
                        HashSet<byte> pointsOnThePath = new HashSet<byte>();

                        txt.Write($".[{row.ToString("X2")}][{column.ToString("X2")}]: " + row.ToString("X2"));

                        byte value = block3[row][column];

                        StringBuilder sb = new StringBuilder(30);
                        Recursive(ref sb, ref pointsOnThePath, ref block3, row, column, value);
                        txt.Write(sb.ToString());
                    }
                    else if (block3[row][column] == row)
                    {
                        txt.Write($".[{row.ToString("X2")}][{column.ToString("X2")}]: You are already at destination");
                    }
                    else
                    {
                        txt.Write($".[{row.ToString("X2")}][{column.ToString("X2")}]: unreachable destination");
                    }
                    txt.WriteLine("");
                }

            }

        }

        private static void Recursive(ref StringBuilder sb, ref HashSet<byte> pointsOnThePath, ref byte[][] block3, int row, int column, byte value)
        {
            pointsOnThePath.Add(value);
            sb.Append(" " + value.ToString("X2"));
            byte nextPoint = block3[value][column];
            if (value != column && !pointsOnThePath.Contains(nextPoint))
            {
                Recursive(ref sb, ref pointsOnThePath, ref block3, row, column, nextPoint);
            }
            else if (value != column && pointsOnThePath.Contains(nextPoint))
            {
                sb.Append("infinite path, crash game!!!");
            }
        }

    }

}

