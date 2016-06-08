using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Keeps track of how far the user has progressed through the word tree
// i.e. what levels and words have been completed

//TODO: auto-save game after each level
 

namespace WordTree
{
	public class ProgressManager : MonoBehaviour {

		public static string chosenKid = ""; // whether girl or boy was picked for kid
		public static string lockStatus = ""; // whether lock is open or closed

		public static string currentWord = ""; // current word
		public static string currentLevel = ""; // current level
		public static int currentMode = 0; // current mode
		
		public static List<string> completedWordsLearn = new List<string>(); // list of words completed for Learn Spelling mode
		public static List<string> completedWordsSpell = new List<string>(); // list of words completed for Spelling Game mode
		public static List<string> completedWordsSound = new List<string>(); // list of words completed for Sound Game mode

		public static List<string> completedLevels = new List<string>(); // list of completed levels
		public static List<string> unlockedLevels = new List<string> (); // list of levels "unlocked" by user

		public static int numLevels = 12; // total number of levels (and each level has three modes - types of games to play for that set of words)
		public static List<string> levelList = new List<string> (); // list of all levels (36 currently, because 3 modes per level/category)

		// set order that levels will be unlocked
		public static string SetLevelOrder(int index)
		{
			if (index == 1)
				return "Animals";
			if (index == 2)
				return "Transportation";
			if (index == 3)
				return "Bathroom";
			if (index == 4)
				return "Kitchen";
			if (index == 5)
				return "Picnic";
			if (index == 6)
				return "Forest";
			if (index == 7)
				return "Bedroom";
			if (index == 8)
				return "School";
			if (index == 9)
				return "Playground";
			if (index == 10)
				return "Clothing";
			if (index == 11)
				return "Garden";
			if (index == 12)
				return "Camping";
			else
				return null;
		}

		// set up the list of levels
		// adds on each level one by one, in the order they should be unlocked
		public static void InitiateLevelList()
		{
			for (int i=1; i<=numLevels; i++) {
				// get a level
				string level = SetLevelOrder (i);

				// add the three game modes for that level to list of levels
				levelList.Add (level + "1");
				levelList.Add (level + "2");
				levelList.Add (level + "3");
			}
		}

		// unlock next level - new level icon appears on word tree
		public static void UnlockNextLevel(string level)
		{
			int index = -1; // index of the level in levelList

			// find level just completed by user in levelList
			// add on the appropriate number (1,2, or 3) to level name to distinguish which mode
			if (ProgressManager.currentMode == 1)
				index = levelList.IndexOf (level + "1");
			if (ProgressManager.currentMode == 2)
				index = levelList.IndexOf (level + "2");
			if (ProgressManager.currentMode == 3)
				index = levelList.IndexOf (level + "3");

			// add the next level to list of unlocked levels
			AddUnlockedLevel (levelList[index+1]);

		}


		// update completed word list
		public static void AddCompletedWord(string word)
		{ 
			// add word to completedWordsLearn list if scene is Learn Spelling
			if (Application.loadedLevelName == "4. Learn Spelling")
				completedWordsLearn.Add (word);

			// add word to completedWordsSpell list if scene is Spelling Game
			if (Application.loadedLevelName == "5. Spelling Game")
				completedWordsSpell.Add (word);

			// add word to completedWordsSound list if scene is Sound Game
			if (Application.loadedLevelName == "6. Sound Game")
				completedWordsSound.Add (word);
		}

		// update completed level list
		public static void AddCompletedLevel(string level)
		{
			if (ProgressManager.currentMode == 1)
				completedLevels.Add (level + "1"); // 1 indicates mode is Learn Spelling

			if (ProgressManager.currentMode == 2)
				completedLevels.Add (level + "2"); // 2 indicates mode is Spelling Game

			if (ProgressManager.currentMode == 3)
				completedLevels.Add (level + "3"); // 3 indicates mode is Sound Game

		}

		// update unlocked level list
		public static void AddUnlockedLevel(string level)
		{
			// check if next level is already unlocked
			// may happen if user chooses to play less advanced level first
			// since multiple levels unlocked at beginning
			if (unlockedLevels.Contains (level)) {
				int index = levelList.IndexOf (level);
				// add the next level not yet unlocked
				unlockedLevels.Add (levelList [index + 3]);
			}

			else 
				unlockedLevels.Add (level);
		}

		// check if word is completed
		public static bool IsWordCompleted(string word)
		{
			if (ProgressManager.currentMode == 1) {
				if (completedWordsLearn.Contains (word))
					return true;
			}
			if (ProgressManager.currentMode == 2) {
				if (completedWordsSpell.Contains (word))
					return true;
			}
			if (ProgressManager.currentMode == 3) {
				if (completedWordsSound.Contains (word))
					return true;
			}
			return false;
			
		}

		// check if level is completed
		public static bool IsLevelCompleted(string level)
		{
			if (completedLevels.Contains (level))
				return true;
			return false;
			
		}

		// check if level is unlocked
		public static bool IsLevelUnlocked(string level)
		{
			if (unlockedLevels.Contains (level))
				return true;
			return false;
			
		}

		// option to show all levels of word tree
		public static void UnlockAllLevels()
		{
			// find level icons
			GameObject[] gos = GameObject.FindGameObjectsWithTag ("LevelIcon");

			foreach (GameObject go in gos) {
				// make level icon appear
				Color color = go.GetComponent<Renderer>().material.color;
				color.a = 1f;
				go.GetComponent<Renderer>().material.color = color;

				// darken level icon - indicates level not yet completed
				go.GetComponent<SpriteRenderer>().color = Color.grey;

				// subscribe level icon to touch gestures
				go.AddComponent<GestureManager>().AddAndSubscribeToGestures(go);

				// start pulsing level icon if not already pulsing
				if (!ProgressManager.IsLevelUnlocked(go.name))
					go.AddComponent<PulseBehavior> ().StartPulsing (go);
			}
		}

		// option to hide levels of word tree user hasn't actually unlocked yet
		public static void RelockLevels()
		{
			// find level icons
			GameObject[] gos = GameObject.FindGameObjectsWithTag ("LevelIcon");

			foreach (GameObject go in gos) {
				// if level hasn't been unlocked yet
				if (!ProgressManager.IsLevelUnlocked (go.name)) {
					// make level icon disappear
					Color color = go.GetComponent<Renderer>().material.color;
					color.a = 0f;
					go.GetComponent<Renderer>().material.color = color;

					// stop pulsing and reset scale for level icon
					go.GetComponent<PulseBehavior>().StopPulsing(go);
					go.transform.localScale = new Vector3(.5f, .5f, 1);

					// disable touch gestures for level icon
					go.GetComponent<GestureManager>().DisableGestures(go);
				}
			}
		}


	}
}
