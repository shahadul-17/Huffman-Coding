#include <stdlib.h>
#include <conio.h>

#include "node.h"       // this also includes the header files stdio.h and string.h...

#define MAXIMUM_RELATIVE_FREQUENCY_LENGTH 10
#define CHARACTER_CODE_LENGTH 11
#define SIZE 26     // for 26 English letters...
#define BUFFER_LENGTH 8192      // this buffer is used to store both encoded and decoded text...

static char buffer[BUFFER_LENGTH], *characterCodes[SIZE];
static const char *FILE_NAMES[] = { "decoded.txt", "encoded.txt" };
static int size = SIZE, relativeFrequencies[2][SIZE];     // for 26 English letters...

static struct Node rootNode, nodes[SIZE - 1];		// we need two relative-frequencies to create one node...

void insertionSort(int array[], int size)
{
    int i, j, key;

    for (i = 1; i < size; i++)
    {
        key = array[i];
        j = i - 1;

        while ((j >= 0) && (array[j] > key))
        {
            array[j + 1] = array[j];
            j = j - 1;
        }

        array[j + 1] = key;
    }
}

void loadRelativeFrequencies()
{
    char relativeFrequencyString[MAXIMUM_RELATIVE_FREQUENCY_LENGTH];
    int i = 0;

    FILE *file = fopen("relative_frequencies", "r");

    while (fscanf(file, "%s", relativeFrequencyString) != EOF)
    {
        relativeFrequencies[0][i] = relativeFrequencies[1][i] = atoi(relativeFrequencyString);

        i++;
    }

    fclose(file);
    insertionSort(relativeFrequencies[0], i);
}

int getMinimumRelativeFrequency()
{
    int temp, minimum = relativeFrequencies[0][0];

    temp = relativeFrequencies[0][0];
    relativeFrequencies[0][0] = relativeFrequencies[0][size - 1];
    relativeFrequencies[0][size - 1] = temp;

    size--;

    insertionSort(relativeFrequencies[0], size);

    return minimum;
}

void generateHuffmanTree()
{
    int i = 0, j = 0, index = 0, left = 0, right = 0;

    struct Node *tempNodes[2];

    while (size > 1)
    {
        left = getMinimumRelativeFrequency();
        right = getMinimumRelativeFrequency();

        initializeNode(left, right, &(nodes[index]));

        relativeFrequencies[0][size] = nodes[index].root;
        size++;

        insertionSort(relativeFrequencies[0], size);

        index++;
    }

    rootNode = nodes[index - 1];	// storing the root of the huffman tree to global variable...

    for (i = SIZE - 2; i >= 0; i--)    // connecting all the nodes according to their parent-child relationship...
    {
        tempNodes[0] = &(nodes[i]);

        printNode(nodes[i]);

        for (j = i - 1; j >= 0; j--)
        {
            tempNodes[1] = &(nodes[j]);

            if ((*tempNodes[0]).left == (*tempNodes[1]).root)
            {
                (*tempNodes[0]).leftNode = tempNodes[1];
                (*tempNodes[1]).parent = tempNodes[0];
            }
            else if ((*tempNodes[0]).right == (*tempNodes[1]).root)
            {
                (*tempNodes[0]).rightNode = tempNodes[1];
                (*tempNodes[1]).parent = tempNodes[0];
            }

            if ((*tempNodes[0]).leftNode != NULL && (*tempNodes[0]).rightNode != NULL)		// this step reduces wastage iterations...
            {
                break;
            }
        }
    }
}

void generateCharacterCode(int index)
{
    int i = 0, relativeFrequency = relativeFrequencies[1][index];

    struct Node *tempNode = NULL;

    characterCodes[index] = malloc(sizeof(char) * CHARACTER_CODE_LENGTH);
    characterCodes[index][0] = '\0';

    for (i = 0; i < SIZE - 1; i++)
    {
        tempNode = &(nodes[i]);

        if ((*tempNode).left == relativeFrequency || (*tempNode).right == relativeFrequency)
        {
            break;
        }
    }

    i = 0;

    if ((*tempNode).left == relativeFrequency)
    {
        characterCodes[index][i] = '0';

        i++;
    }
    else if ((*tempNode).right == relativeFrequency)
    {
        characterCodes[index][i] = '1';

        i++;
    }

    while ((*tempNode).parent != NULL)
    {
        if ((*tempNode).root == (*((*tempNode).parent)).left)
        {
            characterCodes[index][i] = '0';

            i++;
        }
        else if ((*tempNode).root == (*((*tempNode).parent)).right)
        {
            characterCodes[index][i] = '1';

            i++;
        }

        tempNode = (*tempNode).parent;
    }

    characterCodes[index][i] = '\0';

    strrev(characterCodes[index]);
}

char toUpperCase(char character)
{
    if (character >= 'a' && character <= 'z')
    {
        return character - 'a' + 'A';
    }

    return character;
}

char *encode()
{
    char character = '\0';
    int temp = 0;

    FILE *file = fopen(FILE_NAMES[0], "r");

    while (fscanf(file, "%c", &character) != EOF)
    {
        temp = (int)(toUpperCase(character) - 'A');

        if (temp >= 0 && temp < SIZE)
        {
            strcat(buffer, characterCodes[temp]);
        }
        else
        {
            buffer[strlen(buffer)] = character;
        }
    }

    fclose(file);

    return buffer;
}

int indexOf(const char *characterCode)
{
    int i = 0;

    for (i = 0; i < SIZE; i++)
    {
        if (strcmp(characterCodes[i], characterCode) == 0)
        {
            return i;
        }
    }

    return -1;
}

char *decode()
{
    char character, characterCode[CHARACTER_CODE_LENGTH];
    int i = 0, j = 0, index = 0;

    FILE *file = fopen(FILE_NAMES[1], "r");

    while (fscanf(file, "%c", &character) != EOF)
    {
        if (character == '0' || character == '1')
        {
            characterCode[i] = character;
            i++;
            characterCode[i] = '\0';

            if ((index = indexOf(characterCode)) > -1) {
                i = 0;
                buffer[j] = (char)(index + 'A');
                j++;
            }
        }
        else
        {
            i = 0;
            buffer[j] = character;
            j++;
        }
    }

    buffer[j] = '\0';

    fclose(file);

    return buffer;
}

int main()
{
    int i = 0, selection = -1;

    FILE *file = NULL;

    printf("Loading relative frequencies...\n");
    loadRelativeFrequencies();
    printf("Generating huffman tree...\n\n");
    generateHuffmanTree();
    printf("\nGenerating unique codes for characters...\n");

    for (i = 0; i < SIZE; i++)
    {
        generateCharacterCode(i);
        printf("%c = %s\n", (char)(i + 'A'), characterCodes[i]);
    }

    printf("\nSelect an option -\n\t(1) Encode\n\t(2) Decode\n\nSelection = ");
    scanf("%d", &selection);

    switch (selection)
    {
    case 1:
        i = 1;

        printf("\nEncoded text = %s\n", encode());

        break;
    case 2:
        i = 0;

        printf("\nDecoded text = %s\n", decode());

        break;
    default:
        printf("\nInvalid input...\n");

        break;
    }

    file = fopen(FILE_NAMES[i], "w");

    fprintf(file, "%s", buffer);
    fclose(file);
    getch();

    return 0;
}
