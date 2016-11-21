package huffman.coding;

import java.io.File;
import java.io.FileReader;
import java.io.PrintWriter;
import java.util.Scanner;

public class Main {
	
	private static int size = 26;		// for 26 English letters...
	private static final int[][] relativeFrequencies = new int[2][size];
	
	private static String[] characterCodes = new String[size];
	private static final String[] FILE_NAMES = { "decoded.txt", "encoded.txt" };
	private static Node rootNode;		// this is the root of the huffman tree...
	
	private static Node[] nodes = new Node[size - 1];		// we need two relative-frequencies to create one node...
	
	public static void main(String[] args) {
		Main main = new Main();
		
		try {
			System.out.println("Loading relative frequencies...");
			main.loadRelativeFrequencies();
			System.out.println("Generating huffman tree...\n");
			main.generateHuffmanTree();
			System.out.println("\nGenerating unique codes for characters...\n");
			
			for (int i=0; i<characterCodes.length; i++) {
				characterCodes[i] = main.generateCharacterCode(i);
				
				System.out.println((char) (i + 'A') + " = " + characterCodes[i]);
			}
			
			System.out.print("\nSelect an option -\n\t(1) Encode\n\t(2) Decode\n\nSelection = ");
			
			Scanner scanner = new Scanner(System.in);
			
			int selection = Integer.parseInt(scanner.nextLine());
			
			switch (selection) {
			case 1:
				System.out.println("\nEncoded text = " + main.encode());
				
				break;
			case 2:
				System.out.println("\nDecoded text = " + main.decode());
				
				break;
			default:
				System.out.println("\nInvalid input...");
				
				break;
			}
			
			scanner.close();
		}
		catch (Exception exc) {
			exc.printStackTrace();
		}
	}
	
	private void insertionSort(int[] array, int size) {
		int i, j, key;
		
		for (i = 1; i < size; i++) {
			key = array[i];
			j = i - 1;
			
			while ((j >= 0) && (array[j] > key)) {
				array[j + 1] = array[j];
				j = j - 1;
			}
			
			array[j + 1] = key;
		}
	}
	
	private void loadRelativeFrequencies() throws Exception {
		Scanner scanner = new Scanner(Main.class.getResourceAsStream("/resources/relative_frequencies"));
		
		for (int i=0; scanner.hasNextLine(); i++) {
			relativeFrequencies[0][i] = relativeFrequencies[1][i] = Integer.parseInt(scanner.nextLine());
		}
		
		insertionSort(relativeFrequencies[0], relativeFrequencies[0].length);
		scanner.close();
	}
	
	private String getReversedString(String input) {
		char tempChar;
		char[] charArray = input.toCharArray();
		
		for (int i=0, j; i<charArray.length/2; i++) {
			j = charArray.length - i - 1;
			tempChar = charArray[i];
			charArray[i] = charArray[j];
			charArray[j] = tempChar;
		}
		
		return new String(charArray);
	}
	
	private String generateCharacterCode(int index) {
		int relativeFrequency = relativeFrequencies[1][index];
		
		String charCode = "";
		Node tempNode = null;
		
		for (int i=0; i<nodes.length; i++) {
			tempNode = nodes[i];
			
			if (tempNode.left == relativeFrequency || tempNode.right == relativeFrequency) {
				break;
			}
		}
		
		if (tempNode.left == relativeFrequency) {
			charCode += "0";
		}
		else if (tempNode.right == relativeFrequency) {
			charCode += "1";
		}
		
		while (!tempNode.equals(rootNode)) {
			if (tempNode.root == tempNode.parent.left) {
				charCode += "0";
			}
			else if (tempNode.root == tempNode.parent.right) {
				charCode += "1";
			}
			
			if (tempNode.parent != null) {
				tempNode = tempNode.parent;
			}
		}
		
		return getReversedString(charCode);
	}
	
	private int getMinimumRelativeFrequency() {
		int temp, minimum = relativeFrequencies[0][0];
		
		temp = relativeFrequencies[0][0];
		relativeFrequencies[0][0] = relativeFrequencies[0][size - 1];
		relativeFrequencies[0][size - 1] = temp;
		
		size--;
		
		insertionSort(relativeFrequencies[0], size);
		
		return minimum;
	}
	
	private void generateHuffmanTree() {
		int index = 0;
		
		Node[] nodes = new Node[2];
		
		while (size > 1) {
			nodes[0] = new Node(getMinimumRelativeFrequency(), getMinimumRelativeFrequency());
			
			relativeFrequencies[0][size] = nodes[0].root;
			size++;
			
			insertionSort(relativeFrequencies[0], size);
			
			Main.nodes[index] = nodes[0];
			index++;
		}
		
		Main.rootNode = nodes[0];	// storing the root of the huffman tree to global variable...
		
		for (int i = Main.nodes.length - 1; i >= 0; i--) {		// connecting all the nodes according to their parent-child relationship...
			nodes[0] = Main.nodes[i];
			
			System.out.println(nodes[0]);
			
			for (int j = i - 1; j >= 0; j--) {
				nodes[1] = Main.nodes[j];
				
				if (nodes[0].left == nodes[1].root) {
					nodes[0].leftNode = nodes[1];
					nodes[1].parent = nodes[0];
				}
				else if (nodes[0].right == nodes[1].root) {
					nodes[0].rightNode = nodes[1];
					nodes[1].parent = nodes[0];
				}
				
				if (nodes[0].leftNode != null && nodes[0].rightNode != null) {		// this step reduces wastage iterations...
					break;
				}
			}
		}
	}
	
	private void writeToFile(String fileName, String text) throws Exception {
		PrintWriter printWriter = new PrintWriter(new File(fileName));
		printWriter.print(text);
		printWriter.flush();
		printWriter.close();
	}
	
	private String encode() throws Exception {
		int temp, character;
		
		String text = "";
		FileReader fileReader = new FileReader(new File(FILE_NAMES[0]));
		
		while ((character = fileReader.read()) != -1) {
			temp = Character.toUpperCase((char) character) - 'A';
			
			if (temp >= 0 && temp < 26) {
				text += characterCodes[temp];
			}
			else {
				text += (char) character;
			}
		}
		
		fileReader.close();
		writeToFile(FILE_NAMES[1], text);
		
		return text;
	}
	
	private int indexOf(String characterCode) {
		for (int i = 0; i < characterCodes.length; i++) {
			if (characterCodes[i].equals(characterCode)) {
				return i;
			}
		}
		
		return -1;
	}
	
	private String decode() throws Exception {
		int index, character;
		
		String characterCode = "", text = "";
		FileReader fileReader = new FileReader(new File(FILE_NAMES[1]));
		
		while ((character = fileReader.read()) != -1) {
			if ((char) character == '0' || (char) character == '1') {
				characterCode += (char) character;
				
				if ((index = indexOf(characterCode)) > -1) {
					characterCode = "";
					text += (char) (index + 'A');
				}
			}
			else {
				characterCode = "";
				text += (char) character;
			}
		}
		
		fileReader.close();
		writeToFile(FILE_NAMES[0], text);
		
		return text;
	}
	
}