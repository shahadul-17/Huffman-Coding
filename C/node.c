#include "node.h"

static const char *FORMAT = "Root = %d, Left = %d, Right = %d\n";      // a buffer to be returned...

void initializeNode(int left, int right, struct Node *node)
{
    (*node).left = left;
    (*node).right = right;
    (*node).root = left + right;
    (*node).leftNode = (*node).rightNode = (*node).parent = NULL;
}

void printNode(struct Node node)
{
    printf(FORMAT, node.root, node.left, node.right);
}
