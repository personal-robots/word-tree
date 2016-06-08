using UnityEngine;
using System.Collections;

//Manages audio events for sound-blending
//Works for words with 3-5 letters

namespace WordTree
{
	public class AudioManager : MonoBehaviour {

		static public float clipLength = .9f; //time allocated to play audio clip for each letter
		public float pulseLength = .15f; //time to scale up object once
		public float scaleUp = 1.3f; //how much to scale up object by

		//Play each letter's sound and pulse it, then do same for entire word
		public void SpellOutWord(GameObject[] gos)
		{
			
			PlaySoundAndPulseLetter (gos[0], 0);
			PlaySoundAndPulseLetter (gos[1], 1);
			PlaySoundAndPulseLetter (gos[2], 2);
			if (gos.Length >= 4)
				PlaySoundAndPulseLetter (gos[3], 3);
			if (gos.Length >= 5)
				PlaySoundAndPulseLetter (gos[4], 4);
			
			PlaySoundAndPulseWord (gos);
			
		}

		//Play audio clip for while pulsing the letter once
		//float index: order/position of the letter in the word
		//Synchronizes the pulse and sound for each letter
		void PlaySoundAndPulseLetter(GameObject go, float index)
		{
			//Check if object has audio clip attached
			if (go.GetComponent<AudioSource>() != null && go.GetComponent<AudioSource>().clip != null) {
				Debug.Log ("Playing clip for " + go.name);
				//Wait for previous letters to complete before playing audio clip
				go.GetComponent<AudioSource>().PlayDelayed (index*clipLength);  
			}

			//Wait for previous letters to complete before pulsing
			StartCoroutine(PulseLetter (go,index*clipLength));
		}

		//Play audio clip for while pulsing the word once
		//Synchronizes the pulse and sound for the word
		void PlaySoundAndPulseWord(GameObject[] gos)
		{	
			//Check if word object has audio clip attached
			GameObject go = GameObject.FindGameObjectWithTag ("WordObject");
			if (go.GetComponent<AudioSource>() != null && go.GetComponent<AudioSource>().clip != null) {
				Debug.Log ("Playing clip for " + go.name);
				//Wait for previous letters to complete before playing audio clip
				go.GetComponent<AudioSource>().PlayDelayed ((gos.Length) * clipLength);
			}

			//Wait for all letters to complete before pulsing
			StartCoroutine (PulseWord (gos,(gos.Length)*clipLength));

		}

		//Pulse letter once, i.e. grow and then shrink the letter back to original size
		IEnumerator PulseLetter(GameObject go, float delayTime)
		{
			//Wait for specified seconds, want to time it so that letter pulses right after previous letter finishes
			yield return new WaitForSeconds (delayTime);

			//if object is a letter
			if (go.tag == "TargetLetter" || go.tag == "MovableLetter") {
				// scale up letter
				LeanTween.scale (go, new Vector3 (scaleUp * WordCreation.letterScale, scaleUp * WordCreation.letterScale, 1), pulseLength).setDelay (.2f);
				// then scale down letter back to original size
				LeanTween.scale (go, new Vector3 (1f * WordCreation.letterScale, 1f * WordCreation.letterScale, 1), clipLength * .5f).setDelay (clipLength * .5f);
			}

			//if object is a blank
			if (go.tag == "TargetBlank") {
				//Set desired initial (default) scale of blank
				float xScale = .7f; // horizontal scale of blank
				float yScale = 1.5f; // vertical scale of blank

				//Scale up blank
				LeanTween.scale (go, new Vector3 (1.15f * xScale, 1.15f * yScale, 1), pulseLength).setDelay (.2f);
				//Then scale down blank back to original size
				LeanTween.scale (go, new Vector3 (xScale, yScale, 1), clipLength * .5f).setDelay (clipLength * .5f);
			}
			Debug.Log ("Pulse on " + go.name);

		}

		//Pulse word once, i.e. grown and then shrink all the letters of the word at the same time
		IEnumerator PulseWord(GameObject[] gos, float delayTime)
		{	
			//Wait for pulsing of all letters to complete
			yield return new WaitForSeconds (delayTime);

			//if object is a letter
			if (gos [0].tag == "TargetLetter" || gos [0].tag == "MovableLetter") {
				for (int i=0; i < gos.Length; i++) {
					// scale up letter
					LeanTween.scale (gos [i], new Vector3 (scaleUp * WordCreation.letterScale, scaleUp * WordCreation.letterScale, 1), pulseLength).setDelay(.2f);
					// then scale down letter to original size
					LeanTween.scale (gos [i], new Vector3 (1f * WordCreation.letterScale, 1f * WordCreation.letterScale, 1), clipLength * .5f).setDelay (clipLength * .5f);
				}
			}

			//if object is a blank
			if (gos [0].tag == "TargetBlank") {
				//Set desired initial (default) scale of blank
				float xScale = .7f; // horizontal scale of blank
				float yScale = 1.5f; // vertical scale of blank

				for (int i=0; i < gos.Length; i++) {
					//Scale up blank
					LeanTween.scale (gos [i], new Vector3 (1.15f * xScale, 1.15f * yScale, 1), pulseLength).setDelay (.2f);
					//Then scale down blank back to original size
					LeanTween.scale (gos [i], new Vector3 (xScale, yScale, 1), clipLength * .5f).setDelay (clipLength * .5f);
				}
			}
			Debug.Log ("Pulse on word");
			
		}


	}
}
