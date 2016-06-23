using UnityEngine;
using System.Collections;

//Handles instantiating blanks - creates blank object for each letter in word.
//Two types of games where we need to create blanks:
// 1. Spelling Game - user drags letters to target rectangle blanks (make user spell the word themselves)
// 2. Sound Game - user drags movable circle blanks (representing phonemes) to letters (make user match the phoneme to the correct letter)
//Currently works for words with 3-5 letters.

namespace WordTree{

	public class BlankCreation : MonoBehaviour {

		static float xScale; // horizontal default scale of blank
		static float yScale; // vertical default scale of blank
		static float blankWidth = 2.5f; //distance between each blank, i.e. how spread out the blanks are
		static int y; // y position of blank
		static int z; // z position of blank

		//Create blanks that are scrambled randomly
		//so we can test the user on matching sound blanks to letters
		public static void CreateScrambledBlanks(string word, string[] sounds, string shape, string tag, string mode)
		{

			Color[] shuffledColors = new Color[word.Length]; //contains possible colors to be assigned to blanks
			Vector3[] posn = new Vector3[word.Length]; //contains the initial position for each blank
			int[,] order = new int[4, word.Length]; //contains pre-specified ways to rearrange the order of blanks

			//Instantiate blanks - normal order
			CreateBlanks (word, sounds, shape, tag, mode);

			// Preset 4 different shuffling templates for each possible word length
			// i.e. different ways to shuffle the letters in the word
			// to make sure that blanks are indeed shuffled well
			if (word.Length == 3){
				order = new int[,] {{2,3,1}, {3,1,2}, {3,2,1}, {2,1,3}};
			}
			if (word.Length == 4){
				order = new int[,] {{2,1,4,3}, {3,4,1,2}, {4,1,3,2}, {1,3,4,2}};
			}
			if (word.Length == 5){
				order = new int[,] {{2,1,5,3,4}, {3,5,2,4,1}, {4,1,2,5,3}, {5,4,1,3,2}};
			}

			//Find all blanks
			GameObject[] blanks = GameObject.FindGameObjectsWithTag(tag);

			//Set possible colors
			Color[] colors = new Color[] {Color.green,Color.yellow,Color.cyan,Color.blue,Color.magenta};

			//Change blanks to random colors
			shuffledColors = ShuffleArrayColor (colors);
			for (int i=0; i<blanks.Length; i++) {
				SpriteRenderer sprite = blanks[i].GetComponent<SpriteRenderer> ();
				sprite.color = shuffledColors[i];
			}

			//Get initial position of each blank
			for (int i=0; i<blanks.Length; i++)
				posn [i] = blanks[i].transform.position;

			//int index: randomly pick a preset shuffling template (4 to choose from currently)
			int index = Random.Range (0,4);
			//Shuffle blanks positions
			//i.e. move blanks to new positions
			for (int i = 0; i<blanks.Length; i++) {
				blanks [i].transform.position = posn [order[index, i]-1];
			}
			
		}

		//For shuffling a Vector3[] array
		static Vector3[] ShuffleArray(Vector3[] array)
		{
			for (int i = array.Length; i > 0; i--)
			{
				int j = Random.Range (0,i);
				Vector3 temp = array[j];
				array[j] = array[i - 1];
				array[i - 1]  = temp;
			}
			return array;
		}

		//For shuffling a Color[] array
		static Color[] ShuffleArrayColor(Color[] array)
		{
			for (int i = array.Length; i > 0; i--)
			{
				int j = Random.Range (0,i);
				Color temp = array[j];
				array[j] = array[i - 1];
				array[i - 1]  = temp;
			}
			return array;
		}

		//Instantiate blanks. Takes in the desired word, phonemes in the word, shape of the blank (Rectangle or Circle),
		// tag of object (MovableBlank or TargetBlank), and type of game (SpellingGame or SoundGame).
		public static void CreateBlanks(string word, string[] sounds, string shape, string tag, string mode)
		{
			//Set default scale, according to blank shape
			if (shape == "Rectangle") {
				xScale = .7f;
				yScale = 1.5f;
			}
			if (shape == "Circle") {
				xScale = .3f;
				yScale = .3f;
			}

			//Set y position, according to game mode
			if (mode == "SpellingGame")
				y = -3;
			if (mode == "SoundGame")
				y = 0;

			//Set z position
			if (tag == Constants.Tags.TAG_MOVABLE_BLANK)
				z = -2; // if draggable, set in front
			if (tag == Constants.Tags.TAG_TARGET_BLANK)
				z = 0;  // if static, set in back


			//Want the word in the format of an array of uppercase letters instead of a string
			string[] letterArray = new string[word.Length];
			for (int i=0; i<word.Length; i++) {
				char letter = System.Char.ToUpper (word [i]);
				letterArray [i] = System.Char.ToString (letter);
			}

			//Set x position of each blank, for each possible word length
			//Word is centered at x = 0, letters are evenly spaced out according to blankWidth
			Vector3[] position = new Vector3[word.Length];
			if (word.Length == 3) {
				position = new Vector3[3]{
					new Vector3 (-1f * blankWidth, y, z),
					new Vector3 (0, y, z),
					new Vector3 (1f * blankWidth, y, z)
				};
			}
			if (word.Length == 4) {
				position = new Vector3[4] {
					new Vector3 (-1.5f * blankWidth, y, z),
					new Vector3 (-.5f * blankWidth, y, z),
					new Vector3 (.5f * blankWidth, y, z),
					new Vector3 (1.5f * blankWidth, y, z),
				};
			}
			if (word.Length == 5) {
				position = new Vector3[5] {
					new Vector3 (-2f * blankWidth, y, z),
					new Vector3 (-1f * blankWidth, y, z),
					new Vector3 (0, y, z),
					new Vector3 (1f * blankWidth, y, z),
					new Vector3 (2f * blankWidth, y, z)
				};
			}

			//Assign properties for the blanks according to input given, then instantiate each blank
			for (int i=0; i<word.Length; i++) {
				ObjectProperties blank = ObjectProperties.CreateInstance (letterArray [i], tag, position [i], new Vector3 (xScale, yScale, 1), shape, "Phonemes/" + sounds [i]);
				ObjectProperties.InstantiateObject (blank);
			}



		}

	}

}
