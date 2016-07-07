using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Main game controller for "Learn Spelling" scene.
// Creates two sets of words, handles word explosion and animations

namespace WordTree
{
	public class LearnSpellingDirector : MonoBehaviour {
		private List<Vector3> usedPos;
		private Rect rect_but;
		// called on start, initialize stuff
		void Start () {
			//create instance of grestureManager
			GestureManager gestureManager =GameObject.FindGameObjectWithTag
				(Constants.Tags.TAG_GESTURE_MANAGER).GetComponent<GestureManager> ();
			// create two sets of words - movable and target
			LoadSpellingLesson (ProgressManager.currentWord);

			// subscribe buttons to gestures
			GameObject[] buttons = GameObject.FindGameObjectsWithTag (Constants.Tags.TAG_BUTTON);
			foreach (GameObject button in buttons)
				gestureManager.AddAndSubscribeToGestures (button);

			// play word's sound
			GameObject word = GameObject.FindGameObjectWithTag (Constants.Tags.TAG_WORD_OBJECT);
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
			//List of possible locations for letters to go
			List<Vector3> points= new List<Vector3>(); 
			//List of numbers for regions 
			List<int> points_ind= new List<int>(new int[] {1,2,3,4,5});
			//Temporary list of numbers for use in for loop
			List<int> points_ind_temp= new List<int>(); 


			// wait for scene to load before exploding
			yield return new WaitForSeconds (delayTime);
			System.Random rnd = new System.Random();
			// find movable letters
			GameObject[] gos = GameObject.FindGameObjectsWithTag (Constants.Tags.TAG_MOVABLE_LETTER);

			Vector3[] posn = new Vector3[gos.Length]; // contains desired position to move each letter to
			Vector3[] shuffledPosn = new Vector3[gos.Length]; // contains the new positions after being shuffled
			//Possible regions where letter can move
			points.Add( new Vector3(-10,4,0)); 
			points.Add(new Vector3 (-10, -3,0));
			points.Add (new Vector3 (-6, 0,0));
			points.Add(new Vector3 (4, 3, 0));
			points.Add(new Vector3 (8, 0, 0));

							

				//Random rnd = new Random();
				if (gos.Length == 3) {
					for (int i= 0; i<3; i++){
					//Want to go through loop three times for three letters
					int index = rnd.Next(0,4-i);
					//Want random index between 0 and 4 
					//to get a number from the list of point names
					points_ind_temp.Add(points_ind[index]);
					//Add point to temporary list
					points_ind.RemoveAt(index);
					//Remove point so it is not used again 
				}	//Set vectors equal to an index on temporary list
				//which contains values from 0-4 
				//and then use that value for choosing an index from the original points list
					posn = new Vector3[3] {
						points[points_ind_temp[0]],
						points[points_ind_temp[1]],
					points[points_ind_temp[2]]
					};

				}
	
					
				
				
				if (gos.Length == 4) {
				for (int i = 0; i < 4; i++) {
					int index = rnd.Next (0, 4 - i);
					points_ind_temp.Add (points_ind [index]);
					points_ind.RemoveAt (index);
				}
					posn = new Vector3[4] {
					points[points_ind_temp[0]],
					points[points_ind_temp[1]],
					points[points_ind_temp[2]],
					points[points_ind_temp[3]]
					};
				}
				if (gos.Length == 5) {
				for (int i = 0; i < 5; i++) {
					int index = rnd.Next (0, 4 - i);
					points_ind_temp.Add (points_ind [index]);
					points_ind.RemoveAt (index);
				}
					posn = new Vector3[5] {
						points[points_ind_temp[0]],
						points[points_ind_temp[1]],
						points[points_ind_temp[2]],
						points[points_ind_temp[3]],
						points[points_ind_temp[4]]
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
			GameObject[] gos = GameObject.FindGameObjectsWithTag (Constants.Tags.TAG_TARGET_LETTER); 
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
			GameObject[] gos = GameObject.FindGameObjectsWithTag (Constants.Tags.TAG_MOVABLE_LETTER);
			foreach (GameObject go in gos) {
				go.GetComponent<PulseBehavior> ().StartPulsing (go);
			}
		}

		// play celebratory animation when word is completed
		// word object spins around to a cheerful sound
		public static void CelebratoryAnimation(float delayTime)
		{
			 
			float time = 1f; // time to complete animation

			GameObject go = GameObject.FindGameObjectWithTag (Constants.Tags.TAG_WORD_OBJECT);

			Debug.Log ("Spinning " + go.name);

			// spin object around once
			LeanTween.rotateAround (go, Vector3.forward, 360f, time).setDelay(delayTime);

			// scale object up
			LeanTween.scale (go,new Vector3(go.transform.localScale.x*1.3f,go.transform.localScale.y*1.3f,1),time).setDelay (delayTime);

			// move object down
			LeanTween.moveY (go, 1.5f, time).setDelay (delayTime);

			// move target letters down
			GameObject[] tar = GameObject.FindGameObjectsWithTag (Constants.Tags.TAG_TARGET_LETTER);
			foreach (GameObject letter in tar)
				LeanTween.moveY (letter,-3f,time).setDelay(delayTime);

			// play sound 
			Debug.Log ("Playing clip for congrats");
			AudioSource audio = go.AddComponent<AudioSource> ();
			audio.clip = Resources.Load ("Audio/CongratsSound") as AudioClip;
			audio.PlayDelayed (delayTime);

		}
		void Update ()
		{
			// if user presses escape or 'back' button on android, exit program
			if (Input.GetKeyDown (KeyCode.Escape))
				Application.Quit ();
		}	


	}
}
