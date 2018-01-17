#ifndef NODE_H
#define NODE_H

#include <stdio.h>
#include <string.h>

struct Node
{
    int left, right, root;

    struct Node *leftNode, *rightNode, *parent;
};

void initializeNode(int, int, struct Node *);
void printNode(struct Node);

#endif // NODE_H
