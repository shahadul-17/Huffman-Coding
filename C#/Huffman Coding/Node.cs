namespace Huffman_Coding
{
    public class Node
    {
        public int left, right, root;

        public Node leftNode, rightNode, parent;

        public Node(int left, int right)
        {
            this.left = left;
            this.right = right;
            this.root = left + right;
        }

        public override string ToString()
        {
            return "Root = " + root + ", Left = " + left + ", Right = " + right;
        }
    }
}