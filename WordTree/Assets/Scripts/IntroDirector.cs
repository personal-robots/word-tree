﻿using UnityEngine;
using System.Collections;

//<summary>
// Main game controller for the Intro scene
// Sets up kid avatars for user to pick
//</summary>
namespace WordTree
{
	public class IntroDirector : MonoBehaviour
	{
		//<summary>
		// Called on start, used to initialize stuff
		//</summary>
		void Start()
		{
			//Scale graphics to screen size
			Utilities.setCameraViewForScreen(); 
			//create instance of grestureManager
			GestureManager gestureManager = GameObject.FindGameObjectWithTag
				(Constants.Tags.TAG_GESTURE_MANAGER).GetComponent<GestureManager>();
			//create in stance of audioManager
			AudioManager audioManager = GameObject.FindGameObjectWithTag(Constants.Tags.TAG_AUDIO_MANAGER).
				GetComponent<AudioManager>();
			if (gestureManager != null) 
			{
				//GameObject dir = GameObject.Find("IntroDirector");
				// load Background music onto IntroDirector
				string file="Audio/BackgroundMusic/WordTree";
				audioManager.PlayFromFile(file);
				//For keeping background music playing in a loop
				audioManager.GetComponent<AudioSource>().loop = true;
				// find kid
				GameObject[] kids = GameObject.FindGameObjectsWithTag(Constants.Tags.TAG_KID);
				foreach (GameObject kid in kids) 
				{
					// start pulsing kid	
					kid.AddComponent<PulseBehavior>().StartPulsing(kid);
					// subscribe kid to touch gestures
					gestureManager.AddAndSubscribeToGestures(kid);
				}
				ProgressManager.lockStatus = "";
			} 
			else 
			{
				Debug.LogError("Cannot find gesture manager");
			}
		}

		//<summary>
		// Update is called once per frame
		//</summary>
		void Update()
		{
			// if user presses escape or 'back' button on android, exit program
			if (Input.GetKeyDown(KeyCode.Escape))
				Application.Quit();
		}
	
	}
}
