using UnityEngine;
using System.Collections;

// Main game controller for the Intro scene
// Sets up kid avatars for user to pick

namespace WordTree
{
	public class IntroDirector : MonoBehaviour {

		// Called on start, used to initialize stuff
		void Start () {
			//create instance of grestureManager
			GestureManager gestureManager =GameObject.FindGameObjectWithTag(Constants.Tags.TAG_GESTURE_MANAGER)
				.GetComponent<GestureManager> ();


			// find kid
			GameObject[] kids = GameObject.FindGameObjectsWithTag(Constants.Tags.TAG_KID);

			foreach (GameObject kid in kids) {
				// start pulsing kid
				kid.AddComponent<PulseBehavior>().StartPulsing(kid);

				// subscribe kid to touch gestures
				gestureManager.AddAndSubscribeToGestures(kid);
			}

			// find IntroDirector
			GameObject dir = GameObject.Find("IntroDirector");

			// load Background music onto IntroDirector
			dir.AddComponent<AudioSource> ().clip = Resources.Load("Audio/BackgroundMusic/WordTree") as AudioClip;
			// play background music if attached
			if (dir.GetComponent<AudioSource>().clip != null)
				dir.GetComponent<AudioSource>().Play();

			ProgressManager.lockStatus = "";
	
		}

		// Update is called once per frame
		void Update(){

			GameObject dir = GameObject.Find("IntroDirector");
			//If attached audio (background music) has stopped playing, play the audio
			//For keeping background music playing in a loop
			if (!dir.GetComponent<AudioSource>().isPlaying)
				dir.GetComponent<AudioSource>().volume = .25f;
				dir.GetComponent<AudioSource>().Play();
			// if user presses escape or 'back' button on android, exit program
			if (Input.GetKeyDown(KeyCode.Escape))
				Application.Quit();
			

		}
	

	}
}
