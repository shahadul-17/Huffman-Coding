package huffman.coding;

public class Node {
	
	public int left, right, root;
	
	public Node leftNode, rightNode, parent;
	
	public Node(int left, int right) {
		this.left = left;
		this.right = right;
		this.root = left + right;
	}
	
	@Override
	public String toString() {
		return "Root = " + root + ", Left = " + left + ", Right = " + right;
	}
	
}