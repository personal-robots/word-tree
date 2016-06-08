using UnityEngine;
using System.Collections;

//Main game controller for "Choose Object" scene
//Creates word objects for user to pick, sets up kid avatar

namespace WordTree
{
	public class ChooseObjectDirector : MonoBehaviour {

		//Called on start, used to initialize stuff
		void Start () {

			//Create objects and background
			LoadLevel (ProgressManager.currentLevel);

			//Set up kid
			GameObject kid = GameObject.FindGameObjectWithTag ("Kid");
			kid.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("Graphics/" + ProgressManager.chosenKid);

			//Play Grow Animation for kid
			GrowKid ();

			//Load audio for kid
			kid.AddComponent<AudioSource> ().clip = Resources.Load ("Audio/KidSpeaking/" + ProgressManager.currentLevel) as AudioClip;
			//Check if audio clip is attached
			if (kid.GetComponent<AudioSource>().clip != null) {
				kid.GetComponent<AudioSource>().priority = 255;
				//Play audio clip attached to kid if there is one
				kid.GetComponent<AudioSource>().Play ();
			}

			//Find ChooseObjectDirector gameObject
			GameObject dir = GameObject.Find ("ChooseObjectDirector");
			//Load background music for scene onto ChooseObjectDirector
			dir.AddComponent<AudioSource> ().clip = Resources.Load ("Audio/BackgroundMusic/" + ProgressManager.currentLevel) as AudioClip;
			//Check if audio clip is attached
			if (dir.GetComponent<AudioSource>().clip != null) {
				dir.GetComponent<AudioSource>().volume = .7f;
				//Start playing background music if attached
				dir.GetComponent<AudioSource>().Play ();
			}

			//Subscribe buttons to touch gestures
			GameObject button = GameObject.FindGameObjectWithTag ("Button");
			button.AddComponent<GestureManager> ().AddAndSubscribeToGestures (button);

			//Find word objects
			GameObject[] gos = GameObject.FindGameObjectsWithTag ("WordObject");
			foreach (GameObject go in gos) {

				//Start pulsing for each object
				Debug.Log ("Started pulsing for " + go.name);
				go.GetComponent<PulseBehavior> ().StartPulsing (go);

				//Check if word has been completed by user

				//If word not completed, darken and fade out object
				if (!ProgressManager.IsWordCompleted(go.name)){
					SetColorAndTransparency (go,Color.grey,.9f);
				}
				//If word completed, brighten and fill in object
				if (ProgressManager.IsWordCompleted(go.name)){
					Debug.Log ("Word Completed: " + go.name);
					SetColorAndTransparency(go,Color.white,1f);
				}
			}

			//Check if this level has been completed, i.e. if all words in the level have been completed
			if (CheckCompletedLevel ()) {
				//If level completed, add to completedLevels list and unlock the next level
				ProgressManager.AddCompletedLevel (ProgressManager.currentLevel);
				ProgressManager.UnlockNextLevel (ProgressManager.currentLevel);
			}


		}

		//Update is called once per frame
		void Update(){

			//Find ChooseObjectDirector
			GameObject dir = GameObject.Find ("ChooseObjectDirector");

			//If attached audio (background music) has stopped playing, play the audio
			//For keeping background music playing in a loop
			if (!dir.GetComponent<AudioSource>().isPlaying)
				dir.GetComponent<AudioSource>().Play ();
			
		}

		//Animation to grow kid to desired size
		void GrowKid()
		{
			float scale = .5f; //desired scale to grow kid to
			GameObject kid = GameObject.FindGameObjectWithTag ("Kid");
			//Scale up kid to desired size
			LeanTween.scale (kid, new Vector3 (scale, scale, 1f), 1f);
		}

		//Change color and transparency of objects
		//For user to keep track of which words have been completed
		void SetColorAndTransparency(GameObject go, Color color, float transparency)
		{
			//Set transparency
			Color temp = go.GetComponent<Renderer>().material.color;
			temp.a = transparency;
			go.GetComponent<Renderer>().material.color = temp;

			//Set color
			go.GetComponent<SpriteRenderer>().color = color;

		}

		//Check if level has been completed, i.e. if all words have been completed
		//Return true if level completed, otherwise return false
		bool CheckCompletedLevel()
		{
			int numCompleted = 0; //counter for number of words completed in the scene

			//Find word objects
			GameObject[] gos = GameObject.FindGameObjectsWithTag ("WordObject");
			foreach (GameObject go in gos) {
				//if current scene is Learn Spelling
				if (ProgressManager.currentMode == 1){
					//completed a word; update counter
					if (ProgressManager.completedWordsLearn.Contains (go.name))
						numCompleted = numCompleted + 1;
				}
				//if current scene is Spelling Game
				if (ProgressManager.currentMode == 2){
					//completed a word; update counter
					if (ProgressManager.completedWordsSpell.Contains (go.name))
						numCompleted = numCompleted + 1;
				}
				//if current scene is Sound Game
				if (ProgressManager.currentMode == 3){
					//completed a word; update counter
					if (ProgressManager.completedWordsSound.Contains (go.name))
						numCompleted = numCompleted + 1;
				}
			}

			//check if all words have been completed
			//by comparing number of completed words with the number of word objects in the scene
			if (numCompleted == gos.Length) {
				Debug.Log ("Level Completed: " + ProgressManager.currentLevel);
				return true;
			}
			return false;
					
		}

		//Initialize background object
		//Takes in the file name for background image, desired position of image, and scale of image.
		void CreateBackGround(string name, Vector3 posn, float scale)
		{
			//Find background object
			GameObject background = GameObject.Find ("Background");

			//load image
			SpriteRenderer spriteRenderer = background.AddComponent<SpriteRenderer>();
			Sprite sprite = Resources.Load<Sprite>("Graphics/Backgrounds/"+name);
			if (sprite == null)
				Debug.Log("ERROR: could not load background");
			spriteRenderer.sprite = sprite;

			//set position
			background.transform.position = posn;

			//set scale
			background.transform.localScale = new Vector3(scale,scale,1);

		}

		//Initialize scene - create objects and background
		//string level: name of level to load
		void LoadLevel(string level)
		{
			//Get properties of the level, including the words included in the level, the position of the background image, 
			// and the desired scale of the background image
			LevelProperties prop = LevelProperties.GetLevelProperties (level);
			string[] words = prop.Words ();
			Vector3 backgroundPosn = prop.BackgroundPosn ();
			float backgroundScale = prop.BackgroundScale ();

			//Create word objects
			LevelCreation.CreateWordObjects (level, words);

			//Create background
			CreateBackGround (level, backgroundPosn, backgroundScale);

		}



	}
}
