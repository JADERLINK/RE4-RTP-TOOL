using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RTP_EXTRACT
{
    public class RTPextract
    {

        public static void extract(string FileFullName, bool isPs2, bool isPS4NS, bool createDebugFile)
        {
            var invariant = System.Globalization.CultureInfo.InvariantCulture;

            FileInfo fileInfo = new FileInfo(FileFullName);

            string baseDirectory = Path.GetDirectoryName(fileInfo.FullName);
            string baseFileName = Path.GetFileNameWithoutExtension(fileInfo.FullName);

            string baseFilePath = Path.Combine(baseDirectory, baseFileName);

            string pattern = "^(00)([0-9]{2})$";
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.CultureInvariant);

            if (regex.IsMatch(baseFileName))
            {
                baseFilePath = Path.Combine(baseDirectory, baseFileName + "_RTP");
            }

            //------------------------------------------

            var br = new BinaryReader(new FileStream(FileFullName, FileMode.Open));

            uint RTP_MAGIC = br.ReadUInt32();
            ushort RTP_Unk2 = br.ReadUInt16();

            if (RTP_MAGIC != 0x32525450)
            {
                Console.WriteLine("Invalid RTP file.");
                return;
            }

            var idx = File.CreateText(Path.Combine(baseDirectory, baseFileName + ".idxrtp"));
            idx.WriteLine(Program.headerText());
            idx.WriteLine("# File required for repack;");
            idx.WriteLine("# There is nothing to be edited here;");
            idx.WriteLine("# Edit the .obj file;");
            idx.WriteLine("");
            idx.WriteLine("");
            idx.WriteLine("# JADERLINK: https://residentevilmodding.boards.net/user/10432");
            idx.WriteLine("# GitHub: https://github.com/JADERLINK");
            idx.WriteLine("# Youtube: https://www.youtube.com/@JADERLINK");
            idx.WriteLine("# Site: https://jaderlink.blogspot.com");
            idx.WriteLine("# Email: jaderlinkproject@gmail.com");
            idx.Close();

            var txt = new AltTextWriter(baseFilePath + ".txt2", createDebugFile);
            txt.WriteLine(Program.headerText());
            txt.WriteLine("");
            txt.WriteLine("Informational file only:");
            txt.WriteLine(fileInfo.Name);

            txt.WriteLine("RTP_MAGIC: 0x" + RTP_MAGIC.ToString("X8"));
            txt.WriteLine("RTP_Unk2: 0x" + RTP_Unk2.ToString("X4"));

            //---------------

            ushort ublock1Amount = br.ReadUInt16();
            ushort ublock2Amount = br.ReadUInt16();
            ushort ublock3Amount = br.ReadUInt16();

            txt.WriteLine("block1_Amount:" + ublock1Amount);
            txt.WriteLine("block2_Amount:" + ublock2Amount);
            txt.WriteLine("block3_Amount:" + ublock3Amount);

            //-------------

            long ublock1Offset = 0;
            long ublock2Offset = 0;
            long ublock3Offset = 0;

            if (isPS4NS)
            {
                uint pad = br.ReadUInt32();
                txt.WriteLine("pad:" + pad.ToString("X8"));

                //offset de 64 bits
                ublock1Offset = br.ReadInt64();
                ublock2Offset = br.ReadInt64();
                ublock3Offset = br.ReadInt64();

                txt.WriteLine("block1_Offset:" + ublock1Offset.ToString("X16"));
                txt.WriteLine("block2_Offset:" + ublock2Offset.ToString("X16"));
                txt.WriteLine("block3_Offset:" + ublock3Offset.ToString("X16"));
            }
            else 
            {
                txt.WriteLine("");

                //offset de 32 bytes
                ublock1Offset = br.ReadUInt32();
                ublock2Offset = br.ReadUInt32();
                ublock3Offset = br.ReadUInt32();

                txt.WriteLine("block1_Offset:" + ublock1Offset.ToString("X8"));
                txt.WriteLine("block2_Offset:" + ublock2Offset.ToString("X8"));
                txt.WriteLine("block3_Offset:" + ublock3Offset.ToString("X8"));

            }

            //-----------

            txt.WriteLine("");
            txt.WriteLine("block1: (NodeTable)");

            br.BaseStream.Position = ublock1Offset;

            Block1[] block1List = new Block1[ublock1Amount];

            for (int i = 0; i < ublock1Amount; i++)
            {
                //16
                byte[] line = new byte[16];
                br.BaseStream.Read(line, 0, 16);

                float X = BitConverter.ToSingle(line, 0x0);
                float Y = BitConverter.ToSingle(line, 0x4);
                float Z = BitConverter.ToSingle(line, 0x8);

                ushort connectionTableIndex;
                ushort connectionCount;

                if (isPs2)
                {
                    float W = BitConverter.ToSingle(line, 0xC); // sempre 1.0f

                    byte[] line2 = new byte[16];
                    br.BaseStream.Read(line2, 0, 16);

                    connectionTableIndex = BitConverter.ToUInt16(line2, 0x0);
                    connectionCount = BitConverter.ToUInt16(line2, 0x2);

                    // padding
                }
                else 
                {
                    connectionTableIndex = BitConverter.ToUInt16(line, 0xC);
                    connectionCount = BitConverter.ToUInt16(line, 0xE);
                }

                txt.WriteLine("block1[" + i.ToString("X4") + "];  X:" + X.ToString("f6", invariant).PadLeft(16) + "  Y: " + 
                    Y.ToString("f6", invariant).PadLeft(16) + "  Z: " + Z.ToString("f6", invariant).PadLeft(16) + 
                    "  connectionTableIndex: " + connectionTableIndex.ToString("X4") + "  connectionCount: " + connectionCount.ToString("X4"));

                Block1 block1 = new Block1();
                block1.X = X;
                block1.Y = Y;
                block1.Z = Z;
                block1.connectionTableIndex = connectionTableIndex;
                block1.connectionCount = connectionCount;
                block1List[i] = block1;

            }

            txt.WriteLine("");
            txt.WriteLine("block2: (ConnectionTable)");

            br.BaseStream.Position = ublock2Offset;

            Block2[] block2List = new Block2[ublock2Amount];

            for (int i = 0; i < ublock2Amount; i++)
            {
                // 4
                byte[] line = new byte[4];
                br.BaseStream.Read(line, 0, 4);
                ushort connectionIndex = BitConverter.ToUInt16(line, 0);
                ushort distance = BitConverter.ToUInt16(line, 2);

                txt.WriteLine("block2[" + i.ToString("X4") + "]; connectionIndex: " + connectionIndex.ToString("X4") + "  distance: " + distance.ToString("X4"));

                Block2 block2 = new Block2();
                block2.connectionIndex = connectionIndex;
                block2.distance = distance;
                block2List[i] = block2;
            }

            txt.WriteLine("");
            txt.WriteLine("block3: (Traveling salesman problem table)");

            br.BaseStream.Position = ublock3Offset;

            byte[][] block3 = new byte[ublock1Amount][];

            //ublock3mount
            txt.Write($"Lines|Columns");
            for (int i = 0; i < ublock1Amount; i++)
            {
                txt.Write(" " + i.ToString("X2"));
            }
            txt.WriteLine("");

            for (int i = 0; i < ublock1Amount; i++)
            {
                byte[] block3line = new byte[ublock1Amount];
                br.BaseStream.Read(block3line, 0, ublock1Amount);
                txt.WriteLine($"block3[{i:X4}]: " + BitConverter.ToString(block3line));

                block3[i] = block3line;
            }

            br.Close();

            txt.WriteLine("");
            txt.WriteLine("All possible routes:");
            PrintRoutes(ref block3, ref txt);

            txt.WriteLine("");
            txt.WriteLine("End file");
            txt.Close();

            // obj
            var obj = File.CreateText(baseFilePath + ".obj");
            obj.WriteLine(Program.headerText());
            obj.WriteLine("");
            obj.WriteLine("#vertices:");
            for (int i = 0; i < block1List.Length; i++)
            {
                obj.WriteLine("v " + (block1List[i].X / 100f).ToFloatString()
                   + " " + (block1List[i].Y / 100f).ToFloatString()
                   + " " + (block1List[i].Z / 100f).ToFloatString());
            }

            for (int i = 0; i < block1List.Length; i++)
            {
                obj.WriteLine("v " + ((block1List[i].X - 500) / 100f).ToFloatString()
                   + " " + ((block1List[i].Y + 1000) / 100f).ToFloatString()
                   + " " + (block1List[i].Z / 100f).ToFloatString());
            }


            for (int i = 0; i < block1List.Length; i++)
            {
                obj.WriteLine("v " + ((block1List[i].X + 500) / 100f).ToFloatString()
                   + " " + ((block1List[i].Y + 1000) / 100f).ToFloatString()
                   + " " + (block1List[i].Z / 100f).ToFloatString());
            }

            // points
            obj.WriteLine("#Nodes:");
            for (int i = 0; i < block1List.Length; i++)
            {
                int a = i + 1;
                int b = i + 1 + block1List.Length;
                int c = i + 1 + block1List.Length + block1List.Length;


                obj.WriteLine("g _RTP#Node_" + i.ToString("D3") + "#");
                obj.WriteLine("f " + a + " " + b + " " + c);
            }


            // lines

            Dictionary<ConnectionKey, Connection> connections = new Dictionary<ConnectionKey, Connection>();

            for (ushort i = 0; i < block1List.Length; i++)
            {
                ushort ConnectionTableIndex = block1List[i].connectionTableIndex;
                ushort ConnectionCount = block1List[i].connectionCount;

                for (int j = ConnectionTableIndex; j < ConnectionTableIndex + ConnectionCount; j++)
                {
                    ushort ConnectionIndex = block2List[j].connectionIndex;
                    ushort Distance = block2List[j].distance;

                    ConnectionKey key = new ConnectionKey(i, ConnectionIndex);
                    ConnectionKey keyInv = new ConnectionKey(ConnectionIndex, i);

                    if (connections.ContainsKey(keyInv))
                    {
                        //tem a conexão de volta
                        var c = connections[keyInv];
                        c.p2_to_p1 = true;
                        connections[keyInv] = c;
                    }
                    else if (connections.ContainsKey(key))
                    {
                        // conteudo duplicado
                    }
                    else 
                    {
                        // add um novo

                        Connection c = new Connection();
                        c.p1 = i;
                        c.p2 = ConnectionIndex;
                        c.p1_to_p2 = true;

                        connections.Add(key, c);
                    }

                }

            }


            Connection[] ConnectionList = connections.Values.ToArray();
            
            obj.WriteLine("#Connections:");
            foreach (var c in ConnectionList)
            {
                obj.WriteLine("g Connection#" + c.p1.ToString("D3") + ":" + c.p1_to_p2 + "#"  + c.p2.ToString("D3") + ":"+ c.p2_to_p1 + "#");
                int l1 = c.p1 + 1;
                int l2 = c.p2 + 1;

                obj.WriteLine("l " + l1 + " " + l2);

            }

            obj.Close();
        }

        private static void PrintRoutes(ref byte[][] block3, ref AltTextWriter txt) 
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
                        recursive(ref sb, ref pointsOnThePath, ref block3, row, column, value);
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

        private static void recursive(ref StringBuilder sb, ref HashSet<byte> pointsOnThePath, ref byte[][] block3, int row, int column, byte value) 
        {
            pointsOnThePath.Add(value);
            sb.Append(" " + value.ToString("X2"));
            byte nextPoint = block3[value][column];
            if (value != column && !pointsOnThePath.Contains(nextPoint))
            {
                recursive(ref sb, ref pointsOnThePath, ref block3, row, column, nextPoint);
            }
            else if (value != column && pointsOnThePath.Contains(nextPoint))
            {
                sb.Append("infinite path, crash game!!!");
            }
        }

    }

    public class Block1
    {
        public float X;
        public float Y;
        public float Z;

        public ushort connectionTableIndex; // connection table index
        public ushort connectionCount; // connection count
    }

    public class Block2
    {
        public ushort connectionIndex; //conection index
        public ushort distance; //distance
    }


    public class Connection 
    {
        public ushort p1 = 0xff;
        public ushort p2 = 0xff;

        public bool p1_to_p2 = false;
        public bool p2_to_p1 = false;
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

        public override bool Equals(object obj)
        {
            return obj is ConnectionKey k && k.key1 == key1 && k.key2 == key2;
        }

        public override int GetHashCode()
        {
            return Key1 * 0x100 * Key2;
        }
    }


    //AltTextWriter
    public class AltTextWriter
    {
        private TextWriter text;

        public AltTextWriter(string Filepatch, bool Create)
        {
            if (Create)
            {
                text = new FileInfo(Filepatch).CreateText();
            }

        }

        public void WriteLine(string text)
        {
            if (this.text != null)
            {
                this.text.WriteLine(text);
            }
        }

        public void Write(string text) 
        {
            if (this.text != null)
            {
                this.text.Write(text);
            }
        }

        public void Close()
        {
            if (this.text != null)
            {
                this.text.Close();
            }
        }
    }
}
