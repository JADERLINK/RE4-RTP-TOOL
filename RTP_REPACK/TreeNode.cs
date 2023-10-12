using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace RTP_REPACK
{
    public class TreeNode : IEnumerable<TreeNode>
    {

        public ushort ID { get; private set; }
        public List<TreeNode> Children { get; set; }

        public int Count { get { return Children.Count; } }

        public TreeNode this[int i]
        {
            get { return Children[i]; }
        }

        public TreeNode(ushort ID)
        {
            this.ID = ID;
            this.Children = new List<TreeNode>();
        }

        public override string ToString()
        {
            return ID.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TreeNode> GetEnumerator()
        {
            foreach (var Child in Children)
            {
                yield return Child;
            }
        }

    }

}