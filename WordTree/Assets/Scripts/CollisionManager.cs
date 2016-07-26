using UnityEngine;
using System.Collections;
using TouchScript.Gestures;
using TouchScript.Behaviors;
using TouchScript.Hit;
using System.Collections.Generic;
using System.Linq;

//<summary>
// Manage collision events
// Governs what happens when movable and target objects collide
//</summary>
namespace WordTree
{
	public class CollisionManager : MonoBehaviour
	{
		GestureManager gestureManager;

		//<summary>
		// Called when an object enters the collider of another object
		// Set the target object as the trigger
		//</summary>
		void OnTriggerEnter2D(Collider2D other)
		{
			GestureManager gestureManager = GameObject.
				FindGameObjectWithTag(Constants.Tags.TAG_GESTURE_MANAGER).GetComponent<GestureManager>();
			// if the current scene is Learn Spelling
			if (Application.loadedLevelName == "4. Learn Spelling") 
			{
				// if the collided objects are the same letter
				// i.e. if the movable letter that entered matches the target letter
				if (other.name == gameObject.name)
				{
					Debug.Log("Collision on " + other.name);
					// change color of target letter to match that of movable letter
					gameObject.GetComponent<SpriteRenderer>().color = other.GetComponent<SpriteRenderer>()
						.color;
					Color color = gameObject.GetComponent<Renderer>().material.color;
					color.a = 1.0f;
					gameObject.GetComponent<Renderer>().material.color = color;
					// destroy movable letter
					Debug.Log("Destroyed draggable letter " + other.gameObject.name);
					Destroy(other.gameObject);
					// disable collisions for target letter
					// so all other movable letters can no longer collide with this letter
					Debug.Log("Disabled collisions for " + gameObject.name);
					Destroy(gameObject.GetComponent<CollisionManager>());
					// pulse target letter once
					LeanTween.scale(gameObject, new Vector3(WordCreation.letterScale * 1.2f, 
						WordCreation.letterScale * 1.2f, 1), .6f);
					LeanTween.scale(gameObject, new Vector3(WordCreation.letterScale, 
						WordCreation.letterScale, 1), .5f).setDelay(.4f);
					// check if user has completed word
					if (CheckCompletedWord()) 
					{
						// sound out the word
						GameObject[] tar = GameObject.FindGameObjectsWithTag
							(Constants.Tags.TAG_TARGET_LETTER);
						GameObject audioManager = GameObject.Find("AudioManager");
						audioManager.GetComponent<AudioManager>().SpellOutWord(tar);
						// play celebratory animation
						LearnSpellingDirector.CelebratoryAnimation((tar.Length + 1.5f) * 
							AudioManager.clipLength);
						// add word to completedWord list
						ProgressManager.AddCompletedWord(ProgressManager.currentWord);
					}
				}
			}
			//only want to get these components if on the spelling or sound game
			if (Application.loadedLevelName != "4. Learn Spelling") 
			{
				//get properties component 
				Properties prop = other.gameObject.GetComponent<Properties>();
				//We track whether or not letters have been dragged into the right spots using this isinblank flag, 
				//which is set to true when the letter is in the right place.
				//only change the flag if the gameObject has a properties component 
				if (prop != null) 
				{
					prop.isinblank = true; 
				}
				//check if word is spelled correctly
				if (other.name == gameObject.name)
				{
					prop.iscorrect = true;
				}
				// if the current scene is Spelling Game
				if (Application.loadedLevelName == "5. Spelling Game") 
				{
					Debug.Log("Collision on " + other.name);
					// stop pulsing letter
					other.gameObject.GetComponent<PulseBehavior>().StopPulsing(other.gameObject);
					// move letter to center of target blank
					// z-position of letter = -1, so letter is in front of the blank (z-position of blank = 0)
					other.gameObject.transform.position = new Vector3(gameObject.transform.position.x, 
						gameObject.transform.position.y, -1);
					// change color of letter to indicate collision succesfully occurred
					other.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
					Debug.Log("I changed my color");
					// pulse letter once
					LeanTween.scale(other.gameObject, new Vector3(WordCreation.letterScale * 1.25f,
						WordCreation.letterScale * 1.25f, 1), .3f);
					LeanTween.scale(other.gameObject, new Vector3(WordCreation.letterScale,
						WordCreation.letterScale, 1), .3f).setDelay(.2f);
					// disable collisions for target blank
					// so no other movable letters can collide with this blank anymore
					Debug.Log("Disabled collisions for Blank " + gameObject.name);
					Destroy(gameObject.GetComponent<CollisionManager>());
					// if all letters have been dragged to a blank
					if (CheckCompletedTargets(Constants.Tags.TAG_MOVABLE_LETTER))
					{
						// if the user spelled the word correctly
						if (CheckCorrectSpelling(Constants.Tags.TAG_MOVABLE_LETTER)) 
						{
							// find all movable letters
							GameObject[] mov = GameObject.FindGameObjectsWithTag
								(Constants.Tags.TAG_MOVABLE_LETTER);
							// disable touch gestures for sound & hint buttons
							gestureManager.DisableGestures(GameObject.Find("SoundButton"));
							gestureManager.GetComponent<GestureManager>().DisableGestures(GameObject.Find
								("HintButton"));
							// mark all correct letters by changing their colors
							MarkCorrectLetters(0f);
							// sound out the word
							SpellingGameDirector.SpellOutWord();
							// play celebratory animation
							SpellingGameDirector.CelebratoryAnimation((mov.Length + 1.5f) *
								AudioManager.clipLength);
							// add word to completedWord list
							ProgressManager.AddCompletedWord(ProgressManager.currentWord);
							//reset letters in case user wants to play again on the same screen
							foreach (GameObject letter in mov) {
								letter.GetComponent<Properties>().isinblank = false;
								letter.GetComponent<Properties>().iscorrect = false;
							}
						}
						// if user spelled word incorrectly
						if (!CheckCorrectSpelling(Constants.Tags.TAG_MOVABLE_LETTER)) 
						{
							// play try again animation
							SpellingGameDirector.TryAgainAnimation();
							// mark the correct letters by changing their color
							MarkCorrectLetters(1f);
							// move incorrect letters back to original position
							ResetIncorrectLetters(1f);
							// play word's sound
							GameObject.FindGameObjectWithTag(Constants.Tags.TAG_WORD_OBJECT).
							GetComponent<AudioSource>().PlayDelayed(1f);
							// flash hint button to call attention to it
							FlashHintButton(2f);
						}
					
					}
				}
				// if current scene is Sound Game
				if (Application.loadedLevelName == "6. Sound Game") 
				{
					Debug.Log("Collision on " + other.name);
					// stop pulsing sound blank
					other.gameObject.GetComponent<PulseBehavior>().StopPulsing(other.gameObject);
					// place sound blank at opening of jar
					other.gameObject.transform.position = new Vector3(gameObject.transform.position.x,
						gameObject.transform.position.y + 2.2f, 1);
					// pulse sound blank once
					LeanTween.scale(other.gameObject, new Vector3(.4f, .4f, 1), .3f);
					LeanTween.scale(other.gameObject, new Vector3(.3f, .3f, 1), .3f).setDelay(.2f);
					// change color of jar to match color of sound blank
					// to indicate successful collision
					Color color = other.gameObject.GetComponent<SpriteRenderer>().color;
					Vector2 posn = gameObject.transform.position;
					Collider2D[] jar = Physics2D.OverlapCircleAll(posn, .1f, 1, 1.5f, 0f);
					LeanTween.color(jar[0].gameObject, color, .01f);
					// disable collisions for target letter
					Debug.Log("Disabled collisions for Letter " + gameObject.name);
					Destroy(gameObject.GetComponent<CollisionManager>());
					// if all sound blanks have been dragged onto a jar
					if (CheckCompletedTargets(Constants.Tags.TAG_MOVABLE_BLANK)) 
					{
						// if user did not drag all sound blanks to correct letter
						if (!CheckCorrectSpelling(Constants.Tags.TAG_MOVABLE_BLANK)) 
						{
							// play try again animation
							SoundGameDirector.TryAgainAnimation();
							// reset correct sound blanks back to original size
							MarkCorrectSounds(1f);
							// move incorrect sound blanks back to original position
							ResetIncorrectSounds(1f);
							// play word's sound 
							GameObject.FindGameObjectWithTag(Constants.Tags.TAG_WORD_OBJECT).
							GetComponent<AudioSource>().PlayDelayed(1f);
							// flash hint button to call attention to it
							FlashHintButton(2f);	
						}
						// if user dragged all sound blanks to their correct corresponding letters
						if (CheckCorrectSpelling(Constants.Tags.TAG_MOVABLE_BLANK)) 
						{
							// find all sound blanks
							GameObject[] mov = GameObject.FindGameObjectsWithTag
								(Constants.Tags.TAG_MOVABLE_BLANK);
							// disable touch gestures for sound + hint buttons
							gestureManager.DisableGestures(GameObject.Find("SoundButton"));
							gestureManager.DisableGestures(GameObject.Find("HintButton"));
							// reset correct sound blanks back to original size
							MarkCorrectSounds(0f);
							// sound out word
							GameObject[] tar = GameObject.FindGameObjectsWithTag
								(Constants.Tags.TAG_TARGET_LETTER);
							GameObject audioManager = GameObject.Find("AudioManager");
							audioManager.GetComponent<AudioManager>().SpellOutWord(tar);
							// play celebratory animation
							SoundGameDirector.CelebratoryAnimation((mov.Length + 1.5f) *
							AudioManager.clipLength);
							// add completed word to completedWord list
							ProgressManager.AddCompletedWord(ProgressManager.currentWord);	
							//reset letters in case user wants to play again on the same screen
							//or user wants to listen to the sounds 
							foreach (GameObject letter in mov) 
							{
								letter.GetComponent<Properties>().isinblank = false;
								letter.GetComponent<Properties>().iscorrect = false;
							}
						}
					}
				}
			}
		}

		//<summary>
		// return a list of letters not yet completed
		// i.e. target objects that haven't undergone a collision with a movable object yet
		//</summary>
		static List <GameObject> UnoccupiedTargets(string tag)
		{
			// find target objects
			GameObject[] tar = GameObject.FindGameObjectsWithTag(tag);
			List<GameObject> unoccupiedTargets = new List<GameObject>();
			int z = 0;
			float radius = 0;
			// set parameters for Physics2D.OverlapCircleAll function 
			// z = z-position of movable object to search for
			// radius = distance around target object to search
			if (tag == Constants.Tags.TAG_TARGET_BLANK) 
			{
				z = -1;
				radius = .5f;
			}
			if (tag == Constants.Tags.TAG_TARGET_LETTER) 
			{
				z = 1;
				radius = 1.5f;
			}
			for (int i = 0; i < tar.Length; i++)
			{
				// want position of the target object as a Vector2
				Vector2 posn = tar[i].transform.position;
				// find all colliders within specified radius of target object
				// to determine if there has been a movable object dragged onto it
				Collider2D[] mov = Physics2D.OverlapCircleAll(posn, radius, 1, z, z);
				// if no colliders found, add target object to unoccupiedTargets list
				if (mov.Length == 0) 
				{
					unoccupiedTargets.Add(tar[i]);
				}
			}
			return unoccupiedTargets;
		}

		//<summary>
		// get how the user spelled out the word
		//</summary>
		static GameObject[] UserAnswer(string tag)
		{
			// find target objects
			GameObject[] tar = GameObject.FindGameObjectsWithTag(tag);
			GameObject[] userAnswer = new GameObject[tar.Length];
			int z = 0;
			float radius = 0;
			// set parameters for Physics2D.OverlapCircleAll function
			// z = z-position of movable object to search for
			// radius = distance around target object to search
			if (tag == "TargetBlank") 
			{
				z = -1;
				radius = .7f;
			}
			if (tag == "TargetLetter") 
			{
				z = 1;
				radius = 1.5f;
			}
			for (int i = 0; i < tar.Length; i++) 
			{
				// want position of target object as a Vector2
				Vector2 posn = tar[i].transform.position;
				// find all colliders within specified radius of target object
				// to determine the movable object that user has dragged onto it
				Collider2D[] mov = Physics2D.OverlapCircleAll(posn, radius, 1, z, z);
				// If more than one collider found, need to decrease radius parameter 
				// of Physics2D.OverlapCircleAll function so that results are accurate.
				// Want to get the one movable object that has been dragged onto the target object.
				if (mov.Length > 1) {
					Debug.LogError("Decrease OverlapCircleAll radius");
				}
				// if only one collider found, add the letter to the user's spelling of the word
				if (mov.Length != 0)
				{
					userAnswer[i] = mov[0].gameObject;
				}
			}
			return userAnswer;
		}

		//<summary>
		// return a list of the correctly spelled letters
		//</summary>
		static List<GameObject> CorrectObjects(string tag)
		{
			// get user's spelling of the word
			GameObject[] userAnswer = UserAnswer(tag);
			//find all target objects
			GameObject[] tar = GameObject.FindGameObjectsWithTag(tag);
			List<GameObject> correctObjects = new List<GameObject>();
			for (int i = 0; i < tar.Length; i++) 
			{
				// check if the letter and the corresponding blank
				// have same name
				if (userAnswer[i].name == tar[i].name) 
				{
					// if so, add letter to list of correctObjects
					correctObjects.Add(userAnswer[i]);
				}
			}
			return correctObjects;
		}

		//<summary>
		// return a list of the incorrectly spelled letters
		//</summary>
		static List<GameObject>IncorrectObjects(string tag)
		{
			// get user's spelling of word
			GameObject[] userAnswer = UserAnswer(tag);
			// find all target objects
			GameObject[] tar = GameObject.FindGameObjectsWithTag(tag);
			List<GameObject> incorrectObjects = new List<GameObject>();
			for (int i = 0; i < tar.Length; i++)
			{
				// if letter and blank name do not match
				if (userAnswer[i].name != tar[i].name) 
				{
					// add letter to list of incorrectObjects
					incorrectObjects.Add(userAnswer[i]);
				}
			}
			return incorrectObjects;
		}

		//<summary>
		// re-enable collisions after failed attempt to spell word
		// i.e. movable and target objects can collide again
		//</summary>
		public static void EnableCollisions(GameObject go, string tag)
		{
			// get unoccupied target objects
			List<GameObject> unoccupiedTargets = UnoccupiedTargets(tag);
			foreach (GameObject target in unoccupiedTargets) 
			{
				if (target.GetComponent<CollisionManager>() == null)
				{
					// add collision manager so we get trigger enter events
					// when a movable object enters the collider of a target object
					target.AddComponent<CollisionManager>();
					Debug.Log("Enabled collisions for target " + go.name);
				}
			}
		}

		//<summary>
		// change color of correctly spelled letters
		//</summary>
		void MarkCorrectLetters(float delayTime)
		{
			// get the correct letters
			List<GameObject> correctLetters = CorrectObjects(Constants.Tags.TAG_TARGET_BLANK);
			foreach (GameObject go in correctLetters)
			{
				// change color of letter
				// currently changes letter to green color
				LeanTween.color(go, Color.yellow, .1f).setDelay(delayTime);
				go.transform.localScale = new Vector3(WordCreation.letterScale, WordCreation.letterScale, 1);

			}
		}

		//<summary>
		// return correct sound blanks back to original size
		//</summary>
		void MarkCorrectSounds(float delayTime)
		{
			List<GameObject> correctSounds = CorrectObjects(Constants.Tags.TAG_TARGET_LETTER);
			foreach (GameObject go in correctSounds)
			{
				go.transform.localScale = new Vector3(.3f, .3f, 1);

			}
		}

		//<summary>
		//move incorrect letters back to original position so user
		// can try spelling word again
		//</summary>
		public void ResetIncorrectLetters(float delayTime)
		{
			List<GameObject> incorrectLetters = IncorrectObjects(Constants.Tags.TAG_TARGET_BLANK);
			foreach (GameObject go in incorrectLetters) 
			{
				// make sure letters are scaled back to original size
				go.transform.localScale = new Vector3(WordCreation.letterScale,
					WordCreation.letterScale, 1);
				// change letter back to original color
				LeanTween.color(go, Color.white, .1f).setDelay(delayTime);
				Debug.Log("Resetting incorrect letter " + go.name);
				// move letter up in y-direction and away from the target blanks
				Vector3 posn = new Vector3(go.transform.position.x, 0, -2);
				//spin incorrect letters
				LeanTween.rotateAroundLocal(go, Vector3.up, 360f, 1);
				LeanTween.move(go, posn, 1.0f).setDelay(delayTime);
				// begin pulsing letters
				go.GetComponent<PulseBehavior>().StartPulsing(go, delayTime);
				// allow for letters to be dragged again by user
				go.GetComponent<GestureManager>().EnableGestures(go);
				//reset the blank variable so user can try again with these letters
				go.GetComponent<Properties>().isinblank = false;
			}
		}

		//<summary>
		// move incorrect sound blanks back to original position
		// so user can try matching them to letters again
		//</summary>
		public void ResetIncorrectSounds(float delayTime)
		{
			List<GameObject> incorrectSounds = IncorrectObjects(Constants.Tags.TAG_TARGET_LETTER);
			foreach (GameObject go in incorrectSounds) 
			{
				// make sure sound blank is correct scale
				go.transform.localScale = new Vector3(.3f, .3f, 1);
				Debug.Log("Resetting incorrect sound " + go.name);
				// move sound blank up in y-direction and away from the target letters/jars
				Vector3 posn = new Vector3(go.transform.position.x, 0, -2);
				LeanTween.move(go, posn, 1.0f).setDelay(delayTime);
				// start pulsing sound blank
				go.GetComponent<PulseBehavior>().StartPulsing(go, delayTime);
				// allow for touch gestures for sound blank again
				go.GetComponent<GestureManager>().EnableGestures(go);
				// find the jar that the sound object was dragged onto
				Vector2 position = go.transform.position;
				Collider2D[] jar = Physics2D.OverlapCircleAll(position, 2f, 1, 1.5f, 0);
				//spin incorrect letters
				LeanTween.rotateAroundLocal(go, Vector3.up, 360f, 1);
				// change color of letter back to white
				LeanTween.color(jar[0].gameObject, Color.white, .01f).setDelay(delayTime);
				//reset the is in blank variable so user can try again with incorrect sounds
				go.GetComponent<Properties>().isinblank = false;
			}	
		}

		//<summary>
		// make a ghost letter appear in an uncompleted blank
		// as a hint to the user
		//<//summary>
		public static void ShowLetterHint()
		{
			List<GameObject> unoccupiedTargets = UnoccupiedTargets(Constants.Tags.TAG_TARGET_BLANK);
			int i = 0;
			string letterName = unoccupiedTargets[i].name; // get letter that we want to create a hint for
			// create a letter if not already created
			if (GameObject.Find("Hint" + letterName) == null) 
			{
				ObjectProperties letter = ObjectProperties.CreateInstance
					("Hint" + letterName, "Hint", unoccupiedTargets[i].transform.position,
						new Vector3(WordCreation.letterScale, WordCreation.letterScale, 1), 
						"Letters/" + letterName, null);
				ObjectProperties.InstantiateObject(letter);
			}
			// find the letter just created / already created
			GameObject hint = GameObject.Find("Hint" + letterName);
			// move letter to center of corresponding blank
			hint.transform.position = unoccupiedTargets[i].transform.position;
			// play phoneme sound of the letter
			unoccupiedTargets[i].GetComponent<AudioSource>().Play();
			// move letter in front of the background (to z = -3) to make visible
			LeanTween.moveZ(hint, -3, .01f).setDelay(0f);
			// change color of letter to purple and then back to black
			LeanTween.color(hint, Color.magenta, 1f).setDelay(0f);
			LeanTween.color(hint, Color.black, 1f).setDelay(1f);
			// move letter behind the background (to z = 3) to make invisible
			LeanTween.moveZ(hint, 3f, .01f).setDelay(2f);	                 
		}

		//<summary>
		// give user hint about what letter a particular sound is pronouncing
		//</summary>
		public static void ShowSoundHint()
		{
			// find movable sound blanks
			GameObject[] mov = GameObject.FindGameObjectsWithTag(Constants.Tags.TAG_MOVABLE_BLANK);
			// loop through sound blanks
			foreach (GameObject go in mov) 
			{
				// choose first sound blank that is still draggable
				// i.e. hasn't been dragged onto a jar/letter yet
				if (go.GetComponent<TransformGesture>().enabled == true) 
				{
					// get position of sound blank
					Vector3 posn = go.transform.position;
					// sound blank rotates halfway around and fades out
					LeanTween.rotateAround(go, Vector3.right, 180, 1f);
					LeanTween.alpha(go, 0f, .5f).setDelay(.5f);
					// create letter if one doesn't already exist
					if (GameObject.Find("Hint" + go.name) == null) 
					{
						ObjectProperties letter = ObjectProperties.CreateInstance
							("Hint" + go.name, "Hint", posn, new Vector3(WordCreation.letterScale * .9f,
								WordCreation.letterScale * .9f, 1), "Letters/" + go.name, null);
						ObjectProperties.InstantiateObject(letter);
					}
					// find the letter just created / already created
					GameObject hint = GameObject.Find("Hint" + go.name);
					// move letter to center of corresponding sound blank
					hint.transform.position = new Vector3(go.transform.position.x, 
						go.transform.position.y, 3);
					// play phoneme sound of letter
					go.GetComponent<AudioSource>().PlayDelayed(1f);
					// letter fades in and moves in front of background to z = -3
					LeanTween.alpha(hint, 0f, .01f);
					LeanTween.moveZ(hint, -3, .01f).setDelay(1f);
					LeanTween.alpha(hint, 1f, .01f).setDelay(1f); 
					// letter changes color to green and then back to black
					LeanTween.color(hint, Color.green, 1f).setDelay(1f);
					LeanTween.color(hint, Color.black, 1f).setDelay(2f);
					// letter fades out and moves behind background to z = 3
					LeanTween.alpha(hint, 0f, .01f).setDelay(3f);
					LeanTween.moveZ(hint, 3, .01f).setDelay(3f);
					// sound blank rotates back around and fades in
					LeanTween.alpha(go, 1f, .5f).setDelay(3f);
					LeanTween.rotateAround(go, Vector3.left, 180, 1f).setDelay(3f);
					// exit loop once a hint letter has been created
					break;
				}
			}
		}

		//<summary>
		// flash highlight on the hint button three times
		//</summary>
		void FlashHintButton(float delayTime)
		{
			float time = .3f; // time to complete one flash
			GameObject light = GameObject.Find("Highlight");
			// highlight moves in front of background then back behind background three times
			LeanTween.moveZ(light, 1, .01f).setDelay(delayTime);
			LeanTween.moveZ(light, 3, .01f).setDelay(delayTime + 1 * time);
			LeanTween.moveZ(light, 1, .01f).setDelay(delayTime + 2 * time);
			LeanTween.moveZ(light, 3, .01f).setDelay(delayTime + 3 * time);
			LeanTween.moveZ(light, 1, .01f).setDelay(delayTime + 4 * time);
			LeanTween.moveZ(light, 3, .01f).setDelay(delayTime + 5 * time);
		}

		//<summary>
		// check if all movable objects have been dragged onto a target object
		// i.e. has user finished attempting to spell the word / match the sounds with letters
		//</summary>
		bool CheckCompletedTargets(string tag)
		{
			// find movable objects
			GameObject[] mov = GameObject.FindGameObjectsWithTag(tag);
			foreach (GameObject go in mov) 
			{
				Properties props = go.GetComponent<Properties>();
				if (props != null)
				{
					if (!props.isinblank) 
					{
						Debug.Log("GameObject " + go.name + " is not in place yet.");
						return false;
					}
				} 
				else 
				{
					Debug.LogWarning("Tried to get Properties component for GameObject " + go.name + " but it was null!");
					return false;
				}
			}
			Debug.Log("Word Completed");
			return true;
		}

		//<summary>
		// check if user spelled the word correctly
		//</summary>
		bool CheckCorrectSpelling(string tag)
		{
			Debug.Log("Going to check spelling");
			// find movable objects
			GameObject[] mov = GameObject.FindGameObjectsWithTag(tag);
			foreach (GameObject go in mov) 
			{
				//check to see if all words have their 'is correct' variable set to true
				if (go.GetComponent<Properties>() == null)
				{
					return false;
				}
				if (!go.GetComponent<Properties>().iscorrect) 
				{
					return false;
				}
			}
			return true;
		}

		//<summary>
		// check to see if user has completed the word
		//</summary>
		bool CheckCompletedWord()
		{
			// find all movable letters
			GameObject[] mov = GameObject.FindGameObjectsWithTag(Constants.Tags.TAG_MOVABLE_LETTER);
			Debug.Log("Letters left: " + (mov.Length - 1));
			// if word is completed should only find 1 movable letter
			// since movable letters are destroyed after colliding with the correct target letter
			// and this function is called before the last movable letter is destroyed
			if (mov.Length == 1) 
			{
				Debug.Log("Word Completed: " + ProgressManager.currentWord);
				return true;
			}
			return false;	
		}
			
	}
}