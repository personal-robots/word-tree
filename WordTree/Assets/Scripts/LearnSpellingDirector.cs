using UnityEngine;
using System.Collections;

// Main game controller for "Learn Spelling" scene.
// Creates two sets of words, handles word explosion and animations

namespace WordTree
{
	public class LearnSpellingDirector : MonoBehaviour {

		// called on start, initialize stuff
		void Start () {

			// create two sets of words - movable and target
			LoadSpellingLesson (ProgressManager.currentWord);

			// subscribe buttons to gestures
			GameObject[] buttons = GameObject.FindGameObjectsWithTag ("Button");
			foreach (GameObject button in buttons)
				button.AddComponent<GestureManager> ().AddAndSubscribeToGestures (button);

			// play word's sound
			GameObject word = GameObject.FindGameObjectWithTag ("WordObject");
			word.GetComponent<AudioSource>().Play();

			// start pulsing movable letters
			StartCoroutine (StartPulsing (.5f));

			// then explode the letters
			StartCoroutine(ExplodeWord(1));

			// then enable collisions to occur
			StartCoroutine (EnableCollisions(2));

		}

		// create all letters and word object
		void LoadSpellingLesson(string word)
		{
			// get properties of current word being learned
			WordProperties prop = WordProperties.GetWordProperties (word);
			string[] phonemes = prop.Phonemes (); // phonemes in word
			float objScale = prop.ObjScale (); // scale of object

			// create movable and target letters
			WordCreation.CreateMovableAndTargetWords (word, phonemes);

			// create word object
			CreateWordImage (word, objScale);

		}

		// create word object
		void CreateWordImage(string word, float scale)
		{
			float y = 2; // y-position of object

			// instantiate word object from properties given
			ObjectProperties Obj = ObjectProperties.CreateInstance (word, "WordObject", new Vector3 (0, y, 0), new Vector3 (scale, scale, 1), ProgressManager.currentLevel + "/" + word, "Words/" + word);
			ObjectProperties.InstantiateObject (Obj);
		}

		// explode letters of word
		// currently handles words with 3-5 letters
		IEnumerator ExplodeWord(float delayTime)
		{
			// wait for scene to load before exploding
			yield return new WaitForSeconds (delayTime);

			// find movable letters
			GameObject[] gos = GameObject.FindGameObjectsWithTag ("MovableLetter");

			Vector3[] posn = new Vector3[gos.Length]; // contains desired position to move each letter to
			Vector3[] shuffledPosn = new Vector3[gos.Length]; // contains the new positions after being shuffled

			int y1 = 3; // y-position
			int y2 = 2; // y-position
			int z = -2; // z-position

			// set final positions for letters after explosion
			if (gos.Length == 3) {

				posn = new Vector3[3] {
					new Vector3 (-6, 0, z),
					new Vector3 (5, y2, z),
					new Vector3 (7, -y1, z)
				};
			}
			if (gos.Length == 4) {

				posn = new Vector3[4] {
					new Vector3 (-7, -y1, z),
					new Vector3 (-5, y2, z),
					new Vector3 (5, y2, z),
					new Vector3 (7, -y1, z)
				};
			}
			if (gos.Length == 5) {

				posn = new Vector3[5] {
					new Vector3 (-7, -y2, z),
					new Vector3 (-5, y2, z),
					new Vector3 (4, y1, z),
					new Vector3 (8, 0, z),
					new Vector3 (7, -y1, z)
				};
			}

			// shuffle the letters' positions
			shuffledPosn = ShuffleArray (posn);

			for (int i=0; i<gos.Length; i++) {
				// move letter to desired position
				LeanTween.move(gos[i],shuffledPosn[i],1.0f);

				// rotate letter around once
				LeanTween.rotateAround (gos[i], Vector3.forward, 360f, 1.0f);

			}
			Debug.Log ("Exploded draggable letters");
		}

		// shuffle array
		Vector3[] ShuffleArray(Vector3[] array)
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

		// enable collisions between target and movable letters
		IEnumerator EnableCollisions(float delayTime)
		{
			// wait for letters to explode before enabling collisions
			// so letters don't collide prematurely and stick together
			yield return new WaitForSeconds (delayTime);

			// find target letters
			GameObject[] gos = GameObject.FindGameObjectsWithTag ("TargetLetter"); 
			foreach (GameObject go in gos)
				// add collision manager so we can get trigger enter events
				go.AddComponent<CollisionManager> ();
			Debug.Log ("Enabled Collisions");
		}

		// start pulsing draggable letters
		IEnumerator StartPulsing(float delayTime)
		{
			// wait for scene to load before pulsing
			yield return new WaitForSeconds (delayTime);

			// start pulsing letters
			GameObject[] gos = GameObject.FindGameObjectsWithTag ("MovableLetter");
			foreach (GameObject go in gos) {
				go.GetComponent<PulseBehavior> ().StartPulsing (go);
			}
		}

		// play celebratory animation when word is completed
		// word object spins around to a cheerful sound
		public static void CelebratoryAnimation(float delayTime)
		{
			 
			float time = 1f; // time to complete animation

			GameObject go = GameObject.FindGameObjectWithTag ("WordObject");

			Debug.Log ("Spinning " + go.name);

			// spin object around once
			LeanTween.rotateAround (go, Vector3.forward, 360f, time).setDelay(delayTime);

			// scale object up
			LeanTween.scale (go,new Vector3(go.transform.localScale.x*1.3f,go.transform.localScale.y*1.3f,1),time).setDelay (delayTime);

			// move object down
			LeanTween.moveY (go, 1.5f, time).setDelay (delayTime);

			// move target letters down
			GameObject[] tar = GameObject.FindGameObjectsWithTag ("TargetLetter");
			foreach (GameObject letter in tar)
				LeanTween.moveY (letter,-3f,time).setDelay(delayTime);

			// play sound 
			Debug.Log ("Playing clip for congrats");
			AudioSource audio = go.AddComponent<AudioSource> ();
			audio.clip = Resources.Load ("Audio/CongratsSound") as AudioClip;
			audio.PlayDelayed (delayTime);

		}


	}
}
