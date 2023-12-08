using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace RTP_REPACK
{
    public class TreeNode
    {

        public ushort ID { get; private set; }
        // os filhos, são todos os nodes para onde ele pode ir;
        public List<TreeNode> Children { get; set; }
        // os pais, são todos os nodes de onde ele veio;
        public List<TreeNode> Father { get; set; }

        public TreeNode(ushort ID)
        {
            this.ID = ID;
            this.Children = new List<TreeNode>();
            this.Father = new List<TreeNode>();
        }

        public override string ToString()
        {
            return ID.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is TreeNode node && node.ID == ID;
        }

        public override int GetHashCode()
        {
            return (int)ID;
        }

    }

}