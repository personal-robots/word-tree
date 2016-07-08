using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Main game controller for "Learn Spelling" scene.
// Creates two sets of words, handles word explosion and animations

namespace WordTree
{
	public class LearnSpellingDirector : MonoBehaviour
	{
		// Initalize list of possible locations for letters to go
		List<Vector3> points = new List<Vector3>();

	
		// called on start, initialize stuff
		void Start ()
		{
			//create instance of grestureManager
			GestureManager gestureManager = GameObject.FindGameObjectWithTag
				(Constants.Tags.TAG_GESTURE_MANAGER).GetComponent<GestureManager>();
			// create two sets of words - movable and target
			LoadSpellingLesson(ProgressManager.currentWord);
			// subscribe buttons to gestures
			GameObject[] buttons = GameObject.FindGameObjectsWithTag(Constants.Tags.TAG_BUTTON);
			foreach (GameObject button in buttons)
				gestureManager.AddAndSubscribeToGestures(button);
			// play word's sound
			GameObject word = GameObject.FindGameObjectWithTag(Constants.Tags.TAG_WORD_OBJECT);
			word.GetComponent<AudioSource>().Play();
			// start pulsing movable letters
			StartCoroutine(StartPulsing(.5f));
			// then explode the letters
			StartCoroutine(ExplodeWord(1));
			// then enable collisions to occur
			StartCoroutine(EnableCollisions(2));

			//Possible regions where letters can move
			//TODO make sure these locations are on the screen
			points.Add(new Vector3(-5.5f, 3.5f, 0f)); 
			points.Add(new Vector3(-6f, -3.9f, 0f));
			points.Add(new Vector3(-6f, 0f, 0f));
			points.Add(new Vector3(4f, 3f, 0f));
			points.Add(new Vector3(6f, 0f, 0f));

		}

		// create all letters and word object
		void LoadSpellingLesson (string word)
		{
			// get properties of current word being learned
			WordProperties prop = WordProperties.GetWordProperties(word);
			string[] phonemes = prop.Phonemes(); // phonemes in word
			float objScale = prop.ObjScale(); // scale of object
			// create movable and target letters
			WordCreation.CreateMovableAndTargetWords(word, phonemes);
			// create word object
			CreateWordImage(word, objScale);
		}
		// create word object
		void CreateWordImage(string word, float scale)
		{
			float y = 2; // y-position of object
			// instantiate word object from properties given
			ObjectProperties Obj = ObjectProperties.CreateInstance(word, "WordObject", new Vector3 (0, y, 0), new Vector3 (scale, scale, 1), ProgressManager.currentLevel + "/" + word, "Words/" + word);
			ObjectProperties.InstantiateObject(Obj);
		}
		// explode letters of word
		// currently handles words with 3-5 letters
		IEnumerator ExplodeWord(float delayTime)
		{
			//wait for scene to load before exploding
			yield return new WaitForSeconds(delayTime);

			// find movable letters
			GameObject[] gos = GameObject.FindGameObjectsWithTag(Constants.Tags.TAG_MOVABLE_LETTER);
			System.Random rnd = new System.Random();

			//randomize the positions that letters can explode too
			 this.points= this.points.OrderBy(x => rnd.Next ()).ToList();
			//move each letter to a random position
			for (int i=0; i<gos.Length; i++)
			{
				//if we have more letters than points to move them to, then we should not 
				//try to move the letters; otherwise, app will throw error and crash
				if (i >= points.Count) {
					Debug.LogError("We have more letters (" + gos.Length + ") than positions to "
						+ "explode them to (" + points.Count + ")! We moved all we can.");
					break;
				}
					//move letter to new position and spin letter
				    LeanTween.move(gos[i], points[i], 1.0f);
					LeanTween.rotateAround(gos[i], Vector3.forward, 360f, 1.0f);
				} 
			}

		// enable collisions between target and movable letters
		IEnumerator EnableCollisions(float delayTime)
		{
			// wait for letters to explode before enabling collisions
			// so letters don't collide prematurely and stick together
			yield return new WaitForSeconds(delayTime);
			// find target letters
			GameObject[] gos = GameObject.FindGameObjectsWithTag(Constants.Tags.TAG_TARGET_LETTER); 
			foreach (GameObject go in gos)
				// add collision manager so we can get trigger enter events
				go.AddComponent<CollisionManager>();
			Debug.Log("Enabled Collisions");
		}
		// start pulsing draggable letters
		IEnumerator StartPulsing(float delayTime)
		{
			// wait for scene to load before pulsing
			yield return new WaitForSeconds(delayTime);
			// start pulsing letters
			GameObject[] gos = GameObject.FindGameObjectsWithTag(Constants.Tags.TAG_MOVABLE_LETTER);
			foreach (GameObject go in gos)
			{
				go.GetComponent<PulseBehavior>().StartPulsing(go);
			}
		}
		// play celebratory animation when word is completed
		// word object spins around to a cheerful sound
		public static void CelebratoryAnimation(float delayTime)
		{
			
			float time = 1f; // time to complete animation
			GameObject go = GameObject.FindGameObjectWithTag(Constants.Tags.TAG_WORD_OBJECT);
			Debug.Log("Spinning " + go.name);
			// spin object around once
			LeanTween.rotateAround(go, Vector3.forward, 360f, time).setDelay(delayTime);
			// scale object up
			LeanTween.scale(go, new Vector3 (go.transform.localScale.x * 1.3f, go.transform.localScale.y * 1.3f, 1), time).setDelay(delayTime);
			// move object down
			LeanTween.moveY(go, 1.5f, time).setDelay(delayTime);
			// move target letters down
			GameObject[] tar = GameObject.FindGameObjectsWithTag(Constants.Tags.TAG_TARGET_LETTER);
			foreach (GameObject letter in tar)
				LeanTween.moveY(letter, -3f, time).setDelay(delayTime);
			// play sound 
			Debug.Log("Playing clip for congrats");
			AudioSource audio = go.AddComponent<AudioSource>();
			audio.clip = Resources.Load ("Audio/CongratsSound") as AudioClip;
			audio.PlayDelayed(delayTime);
		}

		void Update ()
		{
			// if user presses escape or 'back' button on android, exit program
			if (Input.GetKeyDown(KeyCode.Escape))
				Application.Quit();
		}


	}
}
