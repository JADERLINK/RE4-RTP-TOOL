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

        Dictionary<int, TreeNode> NodeList; //os filhos são a direção para onde ele podem ir. // os pais são os nodes de onde eles vieram.

        //lista invertida, do destino volta para a origem
        Dictionary<ushort, List<Block2>> Block2InvList;

        public TreeStructure(ref Dictionary<int, Point> PointList, ref Dictionary<ushort, List<Block2>> Block2List)
        {
            this.PointList = PointList;
            this.Block2List = Block2List;

            Block2InvList = new Dictionary<ushort, List<Block2>>();
            foreach (var obj in Block2List)
            {
                foreach (var block2 in obj.Value)
                {
                    Block2 newBlock2 = new Block2();
                    newBlock2.ConnectionIndex = obj.Key;
                    newBlock2.Distance = block2.Distance;

                    if (Block2InvList.ContainsKey(block2.ConnectionIndex))
                    {
                        Block2InvList[block2.ConnectionIndex].Add(newBlock2);
                    }
                    else 
                    {
                        Block2InvList.Add(block2.ConnectionIndex, new List<Block2>() { newBlock2 });
                    }
                }
            }
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

                    // adiciona o seu destino
                    root.Children.Add(child);

                    // adiciona ao destino a origem
                    child.Father.Add(root);
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

            //lista com os caminhos
            //sendo (score, caminho) [origem, destino]  
            KeyValuePair<int, List<int>>[,] pathList = new KeyValuePair<int, List<int>>[PointList.Count, PointList.Count];


            for (int x = 0; x < PointList.Count; x++)
            {
                // lista com os nodes que ja foram vistos.
                HashSet<int> allNodesVisited = new HashSet<int>();
                TreeNode node = NodeList[x];

                List<(TreeNode node, List<int> nodesVisited, int score)> nodeToBeVisited = new List<(TreeNode node, List<int> nodesVisited, int score)>();
                nodeToBeVisited.Add((node, new List<int>(), 0));

                recursiveV2(nodeToBeVisited, ref allNodesVisited, ref pathList, node.ID);

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
                        path.Reverse();

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

        //verifica uma rota para cada dois pontos, usando os pais(Father), versão2
        private void recursiveV2(List<(TreeNode node, List<int> nodesVisited, int score)> nodesToBeVisited, ref HashSet<int> allNodesVisited, ref KeyValuePair<int, List<int>>[,] pathList, int destiny)
        {
            //lista de nodes para serem visitados
            //(node a ser visitado, lista de nodes visitados ate chegar nele, score)
            List<(TreeNode node, List<int> nodesVisited, int score)> internalNodesToBeVisited = new List<(TreeNode node, List<int> nodesVisited, int score)>();

            if (nodesToBeVisited.Count != 0)
            {
                foreach (var item in nodesToBeVisited)
                {
                    int id = item.node.ID;
                    if (!allNodesVisited.Contains(id))
                    {
                        List<int> internalNodes = new List<int>();
                        internalNodes.AddRange(item.nodesVisited);
                        internalNodes.Add(id);

                        int lastId = id;
                        if (item.nodesVisited.Count != 0)
                        {
                            lastId = item.nodesVisited.Last();
                        }

                        int? Distance = null;

                        if (Block2InvList.ContainsKey((ushort)lastId))
                        {
                            Distance = (Block2InvList[(ushort)lastId].Where(o => o.ConnectionIndex == id).FirstOrDefault()?.Distance);
                        }

                        int newscore = item.score + (Distance != null ? (int)Distance : 0);

                        if (pathList[id, destiny].Key == 0 || newscore < pathList[id, destiny].Key)
                        {
                            pathList[id, destiny] = new KeyValuePair<int, List<int>>(newscore, internalNodes.ToList());
                        }

                        foreach (var subItem in item.node.Father)
                        {
                            if (!allNodesVisited.Contains(subItem.ID))
                            {
                                internalNodesToBeVisited.Add((subItem, internalNodes, newscore));
                            }
                        }
                    }
                }

                foreach (var item in nodesToBeVisited)
                {
                    int id = item.node.ID;
                    allNodesVisited.Add(id);
                }

                recursiveV2(internalNodesToBeVisited, ref allNodesVisited, ref pathList, destiny);
            }

        }

     
    }


}
