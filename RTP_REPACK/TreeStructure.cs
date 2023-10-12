using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTP_REPACK
{
    public class TreeStructure
    {
        Dictionary<int, Point> PointList;
        Dictionary<ushort, List<Block2>> Block2List;

        Dictionary<int, TreeNode> NodeList; //os filhos são a direção para onde ele podem ir.

        public TreeStructure(ref Dictionary<int, Point> PointList, ref Dictionary<ushort, List<Block2>> Block2List)
        {
            this.PointList = PointList;
            this.Block2List = Block2List;
        }

        public void MakeTree()
        {
            NodeList = new Dictionary<int, TreeNode>();
            
            foreach (var item in Block2List)
            {
                int Id = item.Key;

                foreach (var subItem in item.Value)
                {
                    int ConectionId = subItem.ConnectionIndex;

                    TreeNode root = null;

                    if (NodeList.ContainsKey(Id))
                    {
                        root = NodeList[Id];
                    }
                    else
                    {
                        var rootPoint = PointList[Id];
                        root = new TreeNode((ushort)rootPoint.ID);
                        NodeList.Add(Id, root);
                    }

                    TreeNode child = null;

                    if (NodeList.ContainsKey(ConectionId))
                    {
                        child = NodeList[ConectionId];
                    }
                    else
                    {
                        var childPoint = PointList[ConectionId];
                        child = new TreeNode((ushort)childPoint.ID);
                        NodeList.Add(ConectionId, child);
                    }

                    root.Children.Add(child);
                }
            }

            for (ushort i = 0; i < PointList.Count; i++)
            {
                if (!NodeList.ContainsKey(i))
                {
                    NodeList.Add(i, new TreeNode(i));
                }
            }
        }


        public byte[][] MakeMatriz()
        {

            byte[][] block3List = new byte[PointList.Count][];

            // monta a tabela
            for (int i = 0; i < PointList.Count; i++)
            {
                //lista interna
                byte[] list = new byte[PointList.Count];

                // preenche com 0xff
                for (int f = 0; f < PointList.Count; f++)
                {
                    list[f] = 0xFF;
                }

                // ele é ele mesmo
                list[i] = (byte)i;

                block3List[i] = list;
            }

            //lsta com os caminhos
            //sendo (score, caminho) origem, destino  
            KeyValuePair<int, List<int>>[,] pathList = new KeyValuePair<int, List<int>>[PointList.Count, PointList.Count];


            for (int x = 0; x < PointList.Count; x++)
            {
                // lista com os nodes que ja foram vistos.
                HashSet<int> nodesVisited = new HashSet<int>();
                nodesVisited.Add(x);
                TreeNode node = NodeList[x];
                recursive(node, new List<int>() { x }, ref nodesVisited, ref pathList, node.ID, node.ID, 0);

                int nivel = 0;
                recursiveNivel(node, new List<int>() { x }, ref pathList, node.ID, node.ID, 0, ref nivel);

            }

            //column == destino
            //row == origem

            // correto funciona
            for (int column = 0; column < PointList.Count; column++) // destinos
            {
                for (int row = 0; row < PointList.Count; row++) // origem
                {
                    if (pathList[row, column].Value != null)
                    {
                        List<int> path = pathList[row, column].Value;

                        for (int i = 0; i < path.Count - 1; i++)
                        {
                            if (block3List[path[i]][column] == 0xFF)
                            {
                                block3List[path[i]][column] = (byte)path[i + 1];
                            }
                          
                        }
                    }
                }
            }
            

            return block3List;
        }


        // verifica uma rota para cada dois pontos
        private void recursive(TreeNode node, List<int> externalNodes, ref HashSet<int> nodesVisited, ref KeyValuePair<int, List<int>>[,] pathList, int lastId, int origin, int score)
        {
            foreach (var item in node)
            {
                int id = item.ID;

                if (!nodesVisited.Contains(id))
                {
                    List<int> internalNodes = new List<int>();
                    internalNodes.AddRange(externalNodes);

                    nodesVisited.Add(id);
                    internalNodes.Add(id);

                    score += (Block2List[(ushort)lastId].Where(o => o.ConnectionIndex == id).First().Distance);

                    pathList[origin, id] = new KeyValuePair<int, List<int>>(score, internalNodes.ToList());
                  
                    recursive(item, internalNodes, ref nodesVisited, ref pathList, id, origin, score);
                }

            }

        }



        // procura rotas menores, dentro de um limite.
        private void recursiveNivel(TreeNode node, List<int> externalNodes , ref KeyValuePair<int, List<int>>[,] pathList, int lastId, int origin, int score, ref int nivel) 
        {
            
            nivel += 1;

            for (int i = node.Count -1; i >= 0;i--)
            {
                var item = node[i];
       
                int id = item.ID;

                //Console.WriteLine("id: " + id + "  origin: " + origin + "  lastId: " + lastId + " score: " + score + "  nivel: " + nivel);

                if (!externalNodes.Contains(id) && nivel < 10000)
                {
                    List<int> internalNodes = new List<int>();
                    internalNodes.AddRange(externalNodes);

                    internalNodes.Add(id);

                    score += (Block2List[(ushort)lastId].Where(o => o.ConnectionIndex == id).First().Distance);

                    if (score < pathList[origin, id].Key)
                    {
                        pathList[origin, id] = new KeyValuePair<int, List<int>>(score, internalNodes.ToList());
                    }
            
                    recursiveNivel(item, internalNodes, ref pathList, id, origin, score, ref nivel);
                }
                
            }

        }


    }


}
