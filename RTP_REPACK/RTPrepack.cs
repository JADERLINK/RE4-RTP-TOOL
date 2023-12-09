using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RTP_REPACK
{
    public static class RTPrepack
    {
        public static void Repack(string rtpPath, string objPath, string txt2Path, bool IsPs2, bool createDebugFile) 
        {
            string patternRTP = "^(_RTP#NODE_)([0]{0,})([0-9]{1,3})(#).*$";
            System.Text.RegularExpressions.Regex regexRTP = new System.Text.RegularExpressions.Regex(patternRTP, System.Text.RegularExpressions.RegexOptions.CultureInvariant);
            string patternCONNECTION = "^(CONNECTION#)(([0]{0,})([0-9]{1,3})(:)(TRUE|FALSE)(#)){2}.*$";
            System.Text.RegularExpressions.Regex regexCONNECTION = new System.Text.RegularExpressions.Regex(patternCONNECTION, System.Text.RegularExpressions.RegexOptions.CultureInvariant);
            //-----

            // load .obj file
            var objLoaderFactory = new ObjLoader.Loader.Loaders.ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create();
            var streamReader = new StreamReader(new FileInfo(objPath).OpenRead(), Encoding.ASCII);
            ObjLoader.Loader.Loaders.LoadResult arqObj = objLoader.Load(streamReader);
            streamReader.Close();

            int lastPoint = 0;

            //to block1
            Dictionary<int, Point> PointList = new Dictionary<int, Point>();

            //to block2 ConnectionList
            Dictionary<ConnectionKey, Conection> ConnectionList = new Dictionary<ConnectionKey, Conection>();

            //converte os dados do .obj para os Dictionary
            for (int i = 0; i < arqObj.Groups.Count; i++)
            {
                string groupName = arqObj.Groups[i].GroupName.ToUpperInvariant();

                if (groupName.StartsWith("_RTP"))
                {
                    //FIX NAME
                    groupName = groupName.Replace("_", "#").Replace("#RTP#NODE#", "_RTP#NODE_");

                    //REGEX
                    if (regexRTP.IsMatch(groupName))
                    {
                        Console.WriteLine("Loading in Obj: " + groupName);
                    }
                    else 
                    {
                        Console.WriteLine("Loading in Obj: " + groupName + "  The group name is wrong;");
                    }

                    int id = -1;

                    var split1 = groupName.Split('#');

                    if (split1.Length >= 2)
                    {
                        var split2 = split1[1].Split('_');

                        if (split2.Length >= 2)
                        {
                            try
                            {
                                id = int.Parse(Utils.ReturnValidDecValue(split2[1]));
                            }
                            catch (Exception)
                            {
                            }
                        }

                    }

                    var xyz = arqObj.Vertices[arqObj.Groups[i].Faces[0][0].VertexIndex - 1];

                    for (int f = 0; f < arqObj.Groups[i].Faces.Count; f++)
                    {
                        for (int v = 0; v < arqObj.Groups[i].Faces[f].Count; v++)
                        {
                            var Nxyz = arqObj.Vertices[arqObj.Groups[i].Faces[f][v].VertexIndex - 1];
                            if (Nxyz.Y < xyz.Y)
                            {
                                xyz = Nxyz;
                            }
                        }

                    }

                    Point point = new Point();
                    point.ID = id;
                    point.X = xyz.X * 100f;
                    point.Y = xyz.Y * 100f;
                    point.Z = xyz.Z * 100f;

                    if (!PointList.ContainsKey(id) && id > -1 && id < 255)
                    {
                        if (lastPoint < id)
                        {
                            lastPoint = id;
                        }
                        PointList.Add(id, point);
                    }
                }
                else if (groupName.StartsWith("CONNECTION"))
                {

                    //FIX NAME
                    groupName = groupName.Replace("_", "#").Replace("#TRUE", ":TRUE").Replace("#FALSE", ":FALSE");

                    //REGEX
                    if (regexCONNECTION.IsMatch(groupName))
                    {
                        Console.WriteLine("Loading in Obj: " + groupName);
                    }
                    else
                    {
                        Console.WriteLine("Loading in Obj: " + groupName + "  The group name is wrong;");
                    }

                    var split1 = groupName.Split('#');

                    if (split1.Length >= 2)
                    {
                        var split2 = split1[1].Split(':');

                        int p1 = -1;
                        int p2 = -1;

                        bool p1_to_p2 = false;
                        bool p2_to_p1 = false;

                        try
                        {
                            p1 = int.Parse(Utils.ReturnValidDecValue(split2[0]), System.Globalization.NumberStyles.Integer);
                        }
                        catch (Exception)
                        {
                        }

                        if (split2.Length >= 2)
                        {
                            try
                            {
                                p1_to_p2 = bool.Parse(split2[1].Trim());
                            }
                            catch (Exception)
                            {
                            }
                        }

                        if (split1.Length >= 3)
                        {
                            var split3 = split1[2].Split(':');

                            try
                            {
                                p2 = int.Parse(Utils.ReturnValidDecValue(split3[0]), System.Globalization.NumberStyles.Integer);
                            }
                            catch (Exception)
                            {
                            }

                            if (split3.Length >= 2)
                            {
                                try
                                {
                                    p2_to_p1 = bool.Parse(split3[1].Trim());
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }

                        if (p1 > -1 && p2 > -1 && p1 < 255 && p2 < 255)
                        {
                            ConnectionKey key = new ConnectionKey((ushort)p1, (ushort)p2);
                            ConnectionKey keyInv = new ConnectionKey((ushort)p2, (ushort)p1);

                            if (!ConnectionList.ContainsKey(key) && !ConnectionList.ContainsKey(keyInv)) // se ja tiver é porque esta duplicado
                            {
                                if (lastPoint < p1)
                                {
                                    lastPoint = p1;
                                }
                                if (lastPoint < p2)
                                {
                                    lastPoint = p2;
                                }

                                Conection conection = new Conection();
                                conection.p1 = (ushort)p1;
                                conection.p2 = (ushort)p2;
                                conection.p1_to_p2 = p1_to_p2;
                                conection.p2_to_p1 = p2_to_p1;
                                ConnectionList.Add(key, conection);
                            }
                        }

                    }

                }
                else 
                {
                    Console.WriteLine("Loading in Obj: " + groupName + "   Warning: Group not used;");
                }
            }

            // adiciona os Points faltantes
            for (int i = 0; i <= lastPoint; i++)
            {
                if (!PointList.ContainsKey(i))
                {
                    Point point = new Point();
                    point.ID = i;
                    PointList.Add(i, point);
                }
            }

            //content to Block2
            Dictionary<ushort, List<Block2>> Block2List = new Dictionary<ushort,List<Block2>>();


            foreach (var item in ConnectionList)
            {
                Point p1 = PointList[item.Key.Key1];
                Point p2 = PointList[item.Key.Key2];

                double x = p2.X - p1.X;
                double y = p2.Y - p1.Y;
                double z = p2.Z - p1.Z;

                double res = Math.Abs(Math.Sqrt(x * x + y * y + z * z)) / 10f;

                ushort distance = Utils.ParseDoubleToUshort(res);

                if (item.Value.p1_to_p2)
                {
                    ushort key = item.Key.Key1;
                    Block2 block2 = new Block2();
                    block2.ConnectionIndex = item.Key.Key2;
                    block2.Distance = distance;

                    if (Block2List.ContainsKey(key))
                    {
                        Block2List[key].Add(block2);
                    }
                    else 
                    {
                        Block2List.Add(key,new List<Block2>() { block2 });
                    }
                   
                }

                if (item.Value.p2_to_p1)
                {
                    ushort key = item.Key.Key2;
                    Block2 block2 = new Block2();
                    block2.ConnectionIndex = item.Key.Key1;
                    block2.Distance = distance;

                    if (Block2List.ContainsKey(key))
                    {
                        Block2List[key].Add(block2);
                    }
                    else
                    {
                        Block2List.Add(key, new List<Block2>() { block2 });
                    }
                }

            }

            // block3
            TreeStructure treeStructure = new TreeStructure(ref PointList, ref Block2List);
            treeStructure.MakeTree();
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            byte[][] block3Array =  treeStructure.MakeMatriz();
            sw.Stop();
            Console.WriteLine("Taken time in Milliseconds: " + sw.ElapsedMilliseconds);

            FinalStruture finalStruture = new FinalStruture();
            finalStruture.Fill(ref PointList, ref Block2List);


            FinalFile.FinalRTP(rtpPath, ref finalStruture.Block1Array, ref finalStruture.Block2Array, ref block3Array, IsPs2);


            if (createDebugFile)
            {
                FinalFile.FinalDebugTxt2(txt2Path, ref finalStruture.Block1Array, ref finalStruture.Block2Array, ref block3Array);
            }

        }

    }




}
