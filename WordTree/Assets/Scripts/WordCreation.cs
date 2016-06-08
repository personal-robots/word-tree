using UnityEngine;
using System.Collections;

// Handles instantiating words - creates letter object for each letter in word.
// Currently works for words with 3-5 letters
// Naming conventions: File name for letter image must be same as letter (e.g. "A")
//					   File name for letter's audio clip must be same as phoneme (e.g. "A-short")
//					   Phonemes of word are specified in WordProperties script.

namespace WordTree
{
	public class WordCreation : MonoBehaviour {

		public static float letterScale = .3f; // default letter scale
		static float letterWidth = 2.4f; // width between letters
		static float y; // y-position of letter
		static int z; // z-position of letter

		// create word with letters scrambled
		// takes in desired word and a list of phonemes in word
		public static void CreateScrambledWord(string word, string[] soundArray)
		{

			Vector3[] posn = new Vector3[word.Length]; // contains original position of each letter
			int[,] order = new int[4, word.Length]; // contains pre-specified ways to shuffle letters

			// create word with movable letters
			CreateWord (word, soundArray, "MovableLetter","SpellingGame");

			// set pre-specified shuffling templates (4 options for each word length)
			if (word.Length == 3){
				order = new int[,] {{2,3,1}, {3,1,2}, {3,2,1}, {2,1,3}};
			}
			if (word.Length == 4){
				order = new int[,] {{2,1,4,3}, {3,4,1,2}, {4,1,3,2}, {1,3,4,2}};
			}
			if (word.Length == 5){
				order = new int[,] {{2,1,5,3,4}, {3,5,2,4,1}, {4,1,2,5,3}, {5,4,1,3,2}};
			}

			// get original position of each letter
			GameObject[] mov = GameObject.FindGameObjectsWithTag ("MovableLetter");
			for (int i=0; i<mov.Length; i++)
				posn [i] = mov[i].transform.position;

			// shuffle letters - move to new position
			int index = Random.Range (0,4);
			for (int i = 0; i<mov.Length; i++) {
				mov [i].transform.position = posn [order[index, i]-1];
			}


		}

		// Shuffle array
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

		// create two sets of words - one with movable letters, one with target letters
		// movable letters on top, user has to move into place onto target letters
		// takes in desired word and list of phonemes in word
		public static void CreateMovableAndTargetWords(string word, string[] soundArray)
		{
			// create movable letters
			CreateWord (word, soundArray, "MovableLetter", "Learn");

			// create target letters
			CreateWord (word, soundArray, "TargetLetter", "Learn");

			// change color of target letters to a faded out gray
			GameObject[] tar = GameObject.FindGameObjectsWithTag ("TargetLetter");
			foreach (GameObject go in tar) {
				SpriteRenderer sprite = go.GetComponent<SpriteRenderer> ();
				sprite.color = Color.black;
				Color color = go.GetComponent<Renderer>().material.color;
				color.a = .5f;
				go.GetComponent<Renderer>().material.color = color;
			}

			GameObject[] mov = GameObject.FindGameObjectsWithTag ("MovableLetter");
			// set possible colors for movable letters
			Color[] colors = new Color[] {Color.cyan,Color.blue,Color.magenta,Color.grey,Color.yellow};
			// change each movable letter to different color
			for (int i=0; i<mov.Length; i++) {
				SpriteRenderer sprite = mov[i].GetComponent<SpriteRenderer> ();
				sprite.color = colors[i];
			}
				

		}

		// Create word with letters. Takes in desired word, list of phonemes in word, tag for each letter (MovableLetter or TargetLetter), 
		// and game mode that word will be created in (Learn, SpellingGame, or SoundGame).
		public static void CreateWord(string word, string[] sounds, string tag, string mode)
		{

			Vector3[] position = new Vector3[word.Length]; // contains desired position of each letter
			string[] letters = new string[word.Length]; // contains letters in word

			// want each letter as an uppercase string
			for (int i=0; i<word.Length; i++) {
				char letter = System.Char.ToUpper (word [i]);
				letters[i] = System.Char.ToString (letter);
			}

			// set y-position for letters
			if (mode == "Learn")
				y = -2;
			if (mode == "SpellingGame")
				y = 0;
			if (mode == "SoundGame")
				y = -3.5f;

			// set z-position for letters
			if (tag == "MovableLetter")
				z = -2;
			if (tag == "TargetLetter")
				z = 0;

			// set x-position for each letter
			// word centered at x = 0, letters evenly spread out according to letterWidth
			if (word.Length == 3) {
				position = new Vector3[3]{
					new Vector3 (-1f*letterWidth, y, z),
					new Vector3 (0, y, z),
					new Vector3 (1f*letterWidth, y, z)
				};
			}
			if (word.Length == 4) {
				position = new Vector3[4] {
					new Vector3 (-1.5f*letterWidth, y, z),
					new Vector3 (-.5f*letterWidth, y, z),
					new Vector3 (.5f*letterWidth, y, z),
					new Vector3 (1.5f*letterWidth, y, z),
				};
			}
			if (word.Length == 5) {
				position = new Vector3[5] {
					new Vector3 (-2f*letterWidth, y, z),
					new Vector3 (-1f*letterWidth, y, z),
					new Vector3 (0, y, z),
					new Vector3 (1f*letterWidth, y, z),
					new Vector3 (2f*letterWidth, y, z)
				};
			}


			for (int i=0; i<word.Length; i++) {
				// create letter with desired properties according to input given
				ObjectProperties letter = ObjectProperties.CreateInstance (letters [i], tag, position [i], new Vector3 (letterScale, letterScale, 1), "Letters/" + letters [i], "Phonemes/" + sounds [i]);
				ObjectProperties.InstantiateObject (letter);
			}


		

		}



	}
}
