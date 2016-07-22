﻿using UnityEngine;
using System.Collections;

//<summary>
// Main game controller for "Spelling Game" scene
// Creates letters and blanks, creates word object, handles animations
//</summary>
namespace WordTree
{
	public class SpellingGameDirector : MonoBehaviour
	{
		//<summary>
		// called on start, initialize stuff
		//</summary>
		void Start()
		{
			//Scale graphics to screen size
			Utilities.setCameraViewForScreen();
			//create instance of grestureManager
			GestureManager gestureManager = GameObject.FindGameObjectWithTag
				(Constants.Tags.TAG_GESTURE_MANAGER).GetComponent<GestureManager>();
			// create letters, blanks, and word object
			LoadSpellingGameWord(ProgressManager.currentWord);
			// subscribe buttons to gestures
			GameObject[] buttons = GameObject.FindGameObjectsWithTag(Constants.Tags.TAG_BUTTON);
			foreach (GameObject button in buttons) 
			{
				button.AddComponent<GestureManager>().AddAndSubscribeToGestures(button);
			}
			// sound out word
			GameObject[] tar = GameObject.FindGameObjectsWithTag(Constants.Tags.TAG_TARGET_BLANK);
			GameObject audioManager = GameObject.Find("AudioManager");
			audioManager.GetComponent<AudioManager>().SpellOutWord(tar);
			// start pulsing movable letters
			GameObject[] mov = GameObject.FindGameObjectsWithTag(Constants.Tags.TAG_MOVABLE_LETTER);
			foreach (GameObject go in mov) 
			{
				go.GetComponent<PulseBehavior>().StartPulsing(go, (tar.Length + 1) * AudioManager.clipLength);
			}
		}

		//<summary>
		// create all objects necessary for spelling game
		// including letters, blanks, and word object
		//</summary>
		void LoadSpellingGameWord(string word)
		{
			// get properties of current word in game
			WordProperties prop = WordProperties.GetWordProperties(word);
			string[] phonemes = prop.Phonemes(); // phonemes in word
			float objScale = prop.ObjScale(); // scale of object
			// create word with scrambled letters
			WordCreation.CreateScrambledWord(word, phonemes);
			// create blanks
			BlankCreation.CreateBlanks(word, phonemes, "Rectangle", "TargetBlank", "SpellingGame");
			// create word object
			CreateWordImage(word, objScale);
		}

		//<summary>
		// create word object
		//</summary>
		void CreateWordImage(string word, float scale)
		{
			float y = 3; // y-position of object
			// instantiate object
			ObjectProperties Obj = ObjectProperties.CreateInstance(word, "WordObject", 
				new Vector3(0, y, 0), new Vector3(scale, scale, 1), ProgressManager.currentLevel + 
				"/" + word, "Words/" + word);
			ObjectProperties.InstantiateObject(Obj);
		}

		//<summary>
		// sound out word
		//</summary>
		public static void SpellOutWord()
		{
			// find blanks
			GameObject[] tar = GameObject.FindGameObjectsWithTag(Constants.Tags.TAG_TARGET_BLANK);
			// create array to hold letters found
			GameObject[] mov = new GameObject[tar.Length];
			// find letters dragged onto each blank
			// doing it this way so letters can pulse in the order from left to right
			// in case some words have two of the same letters (e.g. "Kiwi")
			for (int i = 0; i < tar.Length; i++) 
			{
				// want position of blank as Vector2
				Vector2 posn = tar[i].transform.position;
				// find letter that on top of blank
				Collider2D[] letters = Physics2D.OverlapCircleAll(posn, 1.0f, 1, -1, -1);
				mov[i] = letters[0].gameObject;
			}

			// sound out word
			GameObject audioManager = GameObject.Find("AudioManager");
			audioManager.GetComponent<AudioManager>().SpellOutWord(mov);
		}

		//<summary>
		// Animation played when user gets word wrong
		// Big red X appears along with a "slap" sound
		//</summary>
		// TODO: make animation more kid-friendly
		public static void TryAgainAnimation()
		{	
			// find red X object
			GameObject tryAgain = GameObject.Find("TryAgain");
			// object appears for a second
			LeanTween.alpha(tryAgain, 1f, .1f);
			LeanTween.alpha(tryAgain, 0f, .1f).setDelay(1f);
			// grow and shrink object once
			LeanTween.scale(tryAgain, new Vector3(2f, 2f, 1), .7f);
			LeanTween.scale(tryAgain, new Vector3(.5f, .5f, 1), .5f).setDelay(.5f);
			// play "slap" sound
			tryAgain.AddComponent<AudioSource>().clip = Resources.Load("Audio/IncorrectSound") as AudioClip;
			if (tryAgain.GetComponent<AudioSource>().clip != null)
			{
				tryAgain.GetComponent<AudioSource>().Play();
			}	
		}

		//<summary>
		// Animation played when user correctly spells word
		// Word object pulses to a cheerful sound
		//</summary>
		public static void CelebratoryAnimation(float delayTime)
		{	
			float time = .3f; // time to complete one pulse
			GameObject go = GameObject.FindGameObjectWithTag(Constants.Tags.TAG_WORD_OBJECT);
			// grow and shirnk object a few times
			Debug.Log("Pulsing " + go.name);
			float objScale = WordProperties.GetWordProperties(go.name).ObjScale();
			LeanTween.scale(go, new Vector3(objScale * 1.5f, objScale * 1.5f, 1f), time).setDelay(delayTime);
			LeanTween.scale(go, new Vector3(objScale * .7f, objScale * .7f, 1), time).setDelay(delayTime + time);
			LeanTween.scale(go, new Vector3(objScale * 1.5f, objScale * 1.5f, 1f), time).setDelay(delayTime + 2 * time);
			LeanTween.scale(go, new Vector3(objScale * .7f, objScale * .7f, 1), time).setDelay(delayTime + 3 * time);
			LeanTween.scale(go, new Vector3(objScale * 1f, objScale * 1f, 1), time).setDelay(delayTime + 4 * time);
			// play cheerful sound
			Debug.Log("Playing clip for congrats");
			AudioSource audio = go.AddComponent<AudioSource>();
			audio.clip = Resources.Load("Audio/CongratsSound") as AudioClip;
			audio.PlayDelayed(delayTime);
		}

		//<summary>
		//called once per frame
		//</summary>
		void Update()
		{
			// if user presses escape or 'back' button on android, exit program
			if (Input.GetKeyDown(KeyCode.Escape))
				Application.Quit();
		}
			
	}
}
