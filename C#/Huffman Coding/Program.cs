using System;
using System.IO;
using System.Reflection;

namespace Huffman_Coding
{
    public class Program
    {
        private static int size = 26;		// for 26 English letters...
        private static readonly int[][] relativeFrequencies = new int[2][];
        private static string[] characterCodes = new string[size];
        private static readonly string[] FILE_NAMES = { "decoded.txt", "encoded.txt" };

        private static Node rootNode;		// this is the root of the huffman tree...
        private static Node[] nodes = new Node[size - 1];

        public static void Main(string[] args)
        {
            Program program = new Program();

            try
            {
                Console.WriteLine("Loading relative frequencies...");
                program.LoadRelativeFrequencies();
                Console.WriteLine("Generating huffman tree...\n");
                program.GenerateHuffmanTree();
                Console.WriteLine("\nGenerating unique codes for characters...\n");

                for (int i = 0; i < characterCodes.Length; i++)
                {
                    characterCodes[i] = program.GenerateCharacterCode(i);

                    Console.WriteLine((char)(i + 'A') + " = " + characterCodes[i]);
                }

                Console.Write("\nSelect an option -\n\t(1) Encode\n\t(2) Decode\n\nSelection = ");

                int selection = int.Parse(Console.ReadLine());

                switch (selection)
                {
                    case 1:
                        Console.WriteLine("\nEncoded text = " + program.Encode());

                        break;
                    case 2:
                        Console.WriteLine("\nDecoded text = " + program.Decode());

                        break;
                    default:
                        Console.WriteLine("\nInvalid input...");

                        break;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.StackTrace);
            }

            Console.ReadKey();
        }

        private void InsertionSort(int[] array, int size)
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

        private void LoadRelativeFrequencies()
        {
            for (int i = 0; i < relativeFrequencies.Length; i++)
            {
                if (relativeFrequencies[i] == null)
                {
                    relativeFrequencies[i] = new int[size];
                }
            }

            try
            {
                using (StreamReader streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Huffman_Coding.Resources.relative_frequencies")))
                {
                    int i = 0;
                    string line;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        relativeFrequencies[0][i] = relativeFrequencies[1][i] = int.Parse(line);
                        i++;
                    }
                }

                InsertionSort(relativeFrequencies[0], relativeFrequencies[0].Length);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private string Reverse(string input)
        {
            char tempChar;
            char[] charArray = input.ToCharArray();

            for (int i = 0, j; i < charArray.Length / 2; i++)
            {
                j = charArray.Length - i - 1;
                tempChar = charArray[i];
                charArray[i] = charArray[j];
                charArray[j] = tempChar;
            }

            return new string(charArray);
        }

        private string GenerateCharacterCode(int index)
        {
            int relativeFrequency = relativeFrequencies[1][index];
            string charCode = "";

            Node tempNode = null;

            for (int i = 0; i < nodes.Length; i++)
            {
                tempNode = nodes[i];

                if (tempNode.left == relativeFrequency || tempNode.right == relativeFrequency)
                {
                    break;
                }
            }

            if (tempNode.left == relativeFrequency)
            {
                charCode += "0";
            }
            else if (tempNode.right == relativeFrequency)
            {
                charCode += "1";
            }

            while (!tempNode.Equals(rootNode))
            {
                if (tempNode.root == tempNode.parent.left)
                {
                    charCode += "0";
                }
                else if (tempNode.root == tempNode.parent.right)
                {
                    charCode += "1";
                }

                if (tempNode.parent != null)
                {
                    tempNode = tempNode.parent;
                }
            }

            return Reverse(charCode);
        }

        private int GetMinimumRelativeFrequency()
        {
            int temp, minimum = relativeFrequencies[0][0];

            temp = relativeFrequencies[0][0];
            relativeFrequencies[0][0] = relativeFrequencies[0][size - 1];
            relativeFrequencies[0][size - 1] = temp;

            size--;

            InsertionSort(relativeFrequencies[0], size);

            return minimum;
        }

        private void GenerateHuffmanTree()
        {
            int index = 0;

            Node[] nodes = new Node[2];

            while (size > 1)
            {
                nodes[0] = new Node(GetMinimumRelativeFrequency(), GetMinimumRelativeFrequency());

                relativeFrequencies[0][size] = nodes[0].root;
                size++;

                InsertionSort(relativeFrequencies[0], size);

                Program.nodes[index] = nodes[0];
                index++;
            }

            rootNode = nodes[0];    // storing the root of the huffman tree to global variable...

            for (int i = Program.nodes.Length - 1; i >= 0; i--)    // connecting all the nodes according to their parent-child relationship...
            {
                nodes[0] = Program.nodes[i];

                Console.WriteLine(nodes[0]);

                for (int j = i - 1; j >= 0; j--)
                {
                    nodes[1] = Program.nodes[j];

                    if (nodes[0].left == nodes[1].root)
                    {
                        nodes[0].leftNode = nodes[1];
                        nodes[1].parent = nodes[0];
                    }
                    else if (nodes[0].right == nodes[1].root)
                    {
                        nodes[0].rightNode = nodes[1];
                        nodes[1].parent = nodes[0];
                    }

                    if (nodes[0].leftNode != null && nodes[0].rightNode != null)		// this step reduces wastage iterations...
                    {
                        break;
                    }
                }
            }
        }

        private void WriteToFile(string fileName, string text)
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(fileName))
                {
                    streamWriter.Write(text);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private string Encode()
        {
            int temp, character;
            string text = "";

            try
            {
                using (StreamReader streamReader = new StreamReader(FILE_NAMES[0]))
                {
                    while ((character = streamReader.Read()) != -1)
                    {
                        temp = char.ToUpper((char)character) - 'A';

                        if (temp >= 0 && temp < relativeFrequencies[1].Length)
                        {
                            text += characterCodes[temp];
                        }
                        else
                        {
                            text += (char)character;
                        }
                    }
                }

                WriteToFile(FILE_NAMES[1], text);
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return text;
        }

        private int IndexOf(string characterCode)
        {
            for (int i = 0; i < characterCodes.Length; i++)
            {
                if (characterCodes[i] == characterCode)
                {
                    return i;
                }
            }

            return -1;
        }

        private string Decode()
        {
            int index, character;
            string characterCode = "", text = "";

            try
            {
                using (StreamReader streamReader = new StreamReader(FILE_NAMES[1]))
                {
                    while ((character = streamReader.Read()) != -1)
                    {
                        if ((char)character == '0' || (char)character == '1')
                        {
                            characterCode += (char)character;

                            if ((index = IndexOf(characterCode)) > -1)
                            {
                                characterCode = "";
                                text += (char)(index + 'A');
                            }
                        }
                        else
                        {
                            characterCode = "";
                            text += (char)character;
                        }
                    }
                }

                WriteToFile(FILE_NAMES[0], text);
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return text;
        }
    }
}