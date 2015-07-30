﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WordTree
{
	public class ProgressManager : MonoBehaviour {

		public static string currentWord = "";
		public static string currentLevel = "";
		public static int currentMode = 0;
		
		public static List<string> completedWordsLearn = new List<string>();
		public static List<string> completedWordsSpell = new List<string>();
		public static List<string> completedWordsSound = new List<string>();

		public static List<string> completedLevels = new List<string>();
		public static List<string> unlockedLevels = new List<string> ();

		public static int numLevels = 1;
		public static List<string> levelList = new List<string> ();

		public static string SetLevelOrder(int index)
		{
			if (index == 1)
				return "Fruits";
			else
				return null;
		}

		public static void InitiateLevelList()
		{
			for (int i=1; i<=numLevels; i++) {
				string level = SetLevelOrder (i);
				levelList.Add (level + "1");
				levelList.Add (level + "2");
				levelList.Add (level + "3");
			}
		}

		public static void UnlockNextLevel(string level)
		{
			int index = -1;

			if (ProgressManager.currentMode == 1)
				index = levelList.IndexOf (level + "1");

			if (ProgressManager.currentMode == 2)
				index = levelList.IndexOf (level + "2");

			if (ProgressManager.currentMode == 3)
				index = levelList.IndexOf (level + "3");

			AddUnlockedLevel (levelList[index+1]);

		}



		public static void AddCompletedWord(string word)
		{
			if (Application.loadedLevelName == "4. Learn Spelling")
				completedWordsLearn.Add (word);
			
			if (Application.loadedLevelName == "5. Spelling Game")
				completedWordsSpell.Add (word);
			
			if (Application.loadedLevelName == "6. Sound Game")
				completedWordsSound.Add (word);
		}


		public static void AddCompletedLevel(string level)
		{
			if (ProgressManager.currentMode == 1)
				completedLevels.Add (level + "1");

			if (ProgressManager.currentMode == 2)
				completedLevels.Add (level + "2");

			if (ProgressManager.currentMode == 3)
				completedLevels.Add (level + "3");

		}

		public static void AddUnlockedLevel(string level)
		{
			unlockedLevels.Add (level);
		}


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

		public static bool IsLevelCompleted(string level)
		{
			if (completedLevels.Contains (level))
				return true;
			return false;
			
		}

		public static bool IsLevelUnlocked(string level)
		{
			if (unlockedLevels.Contains (level))
				return true;
			return false;
			
		}


	}
}