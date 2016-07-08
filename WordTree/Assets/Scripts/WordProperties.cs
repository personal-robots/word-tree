using UnityEngine;
using System.Collections;

// Stores properties of each word, including its phonemes and the scale of its object/image.

namespace WordTree
{

	public class WordProperties : ScriptableObject {

		string[] phonemes; // phonemes in the word
		float objScale; // scale of word object

		// Get phonemes in the word
		public string[] Phonemes()
		{
			return this.phonemes;
		}

		// Get scale of word object
		public float ObjScale()
		{
			return this.objScale;
		}

		// Set the word's properties
		void Init (string[] phonemes, float objScale)
		{
			this.phonemes = phonemes;
			this.objScale = objScale;
		}
		
		// Create an instance of WordProperties with the desired properties set
		static WordProperties CreateInstance(string[] phonemes, float objScale)
		{
			WordProperties prop = ScriptableObject.CreateInstance<WordProperties>();
			prop.Init(phonemes, objScale);
			return prop;
		}


		// Get properties of a word
		// info for all words stored here, so we can access when instantiating a word
		public static WordProperties GetWordProperties(string word)
		{
			switch (word) 
			{
			
			// Animals

			case "Cat":
				return CreateInstance(new string[] {"C","A-short","T"}, .5f);
				
			case "Dog":
				
				return CreateInstance(new string[] {"D","O-short","G"}, .6f);

			case "Hen":
				
				return CreateInstance(new string[] {"H","E-short","N-end"}, .6f);

			case "Pig":
				
				return CreateInstance(new string[] {"P","I-short","G"}, .5f);

			case "Rat":
				
				return CreateInstance(new string[] {"R","A-short","T"}, .6f);


			// Transportation

			case "Bus":
				
				return CreateInstance(new string[] {"B","U-short","S"}, .7f);
				
			case "Cab":
				
				return CreateInstance(new string[] {"C","A-short","B"}, .7f);
			
			case "Car":

				return CreateInstance(new string[] {"C","O-short","R-end"}, .6f);
				
			case "Jet":
				
				return CreateInstance(new string[] {"J","E-short","T"}, .8f);

			case "Van":
				
				return CreateInstance(new string[] {"V","A-nasal","N-end"}, .7f);
			
			// Bathroom
				
			case "Can":
				
				return CreateInstance(new string[] {"C","A-nasal","N-end"}, .5f);
				
			case "Gel":
				
				return CreateInstance(new string[] {"J","E-short","L-end"}, .5f);
				
			case "Lid":
				
				return CreateInstance(new string[] {"L","I-short","D"}, .5f);
				
			case "Mop":
				
				return CreateInstance(new string[] {"M","O-short","P"}, .6f);
				
			case "Tub":
				
				return CreateInstance(new string[] {"T","U-short","B"}, .6f);
			
			// Kitchen
				
			case "Cup":
				
				return CreateInstance(new string[] {"C","U-short","P"}, .6f);
				
			case "Jar":
				
				return CreateInstance(new string[] {"J","O-short","R-end"}, .5f);

			case "Jug":
				
				return CreateInstance(new string[] {"J","U-short","G"}, .5f);
				
			case "Pan":
				
				return CreateInstance(new string[] {"P","A-nasal","N-end"}, .7f);

			case "Rag":
				
				return CreateInstance(new string[] {"R","A-short","G"}, .5f);

			// Picnic
				
			case "Bag":
				
				return CreateInstance(new string[] {"B","A-short","G"}, .5f);
				
			case "Bun":
				
				return CreateInstance(new string[] {"B","U-short","N-end"}, .6f);
				
			case "Ham":
				
				return CreateInstance(new string[] {"H","A-nasal","M"}, .7f);
				
			case "Jam":
				
				return CreateInstance(new string[] {"J","A-nasal","M"}, .5f);
				
			case "Taco":
				
				return CreateInstance(new string[] {"T","O-short","C","O-long"}, .6f);
			
		    //Pond
				
			case "Bug":
				
				return CreateInstance(new string[] {"B","U-short","G"}, .7f);
				
			case "Fly":
				
				return CreateInstance(new string[] {"F","L","I-long"}, .6f);
				
			case "Log":
				
				return CreateInstance(new string[] {"L","O-short","G"}, .7f);
				
			case "Mud":
				
				return CreateInstance(new string[] {"M","U-short","D"}, .8f);
				
			case "Web":
				
				return CreateInstance(new string[] {"W","E-short","B"}, .7f);

			//Bedroom
				
			case "Bed":
				
				return CreateInstance(new string[] {"B","E-short","D"}, 1f);
				
			case "Crib":
				
				return CreateInstance(new string[] {"C","R","I-short","B"}, .7f);
				
			case "Fan":
				
				return CreateInstance(new string[] {"F","A-nasal","N-end"}, .6f);
				
			case "Mat":
				
				return CreateInstance(new string[] {"M","A-short","T"}, .7f);
				
			case "Rug":

				return CreateInstance(new string[] {"R","U-short","G"}, .7f);

			// School
				
			case "Desk":
				
				return CreateInstance(new string[] {"D","E-short","S","K"}, .6f);
				
			case "Mug":
				
				return CreateInstance(new string[] {"M","U-short","G"}, .5f);
				
			case "Pad":
				
				return CreateInstance(new string[] {"P","A-short","D"}, .5f);
				
			case "Pen":
				
				return CreateInstance(new string[] {"P","E-short","N-end"}, .7f);
				
			case "Pin":
				
				return CreateInstance(new string[] {"P","I-short","N-end"}, .5f);

			// Playground
				
			case "Box":
				
				return CreateInstance(new string[] {"B","O-short","X"}, .7f);
				
			case "Net":
				
				return CreateInstance(new string[] {"N","E-short","T"}, .6f);
				
			case "Sun":
				
				return CreateInstance(new string[] {"S","U-short","N-end"}, .5f);
				
			case "Top":
				
				return CreateInstance(new string[] {"T","O-short","P"}, .5f);
				
			case "Yoyo":
				
				return CreateInstance(new string[] {"Y","O-long","Y","O-long"}, .5f);

			// Clothing
				
			case "Cap":
				
				return CreateInstance(new string[] {"C","A-short","P"}, .5f);
				
			case "Hat":
				
				return CreateInstance(new string[] {"H","A-short","T"}, .5f);
				
			case "Tux":
				
				return CreateInstance(new string[] {"T","U-short","X"}, .6f);
				
			case "Vest":
				
				return CreateInstance(new string[] {"V","E-short","S","T"}, .4f);
				
			case "Wig":
				
				return CreateInstance(new string[] {"W","I-short","G"}, .5f);

			// Garden
				
			case "Ant":
				
				return CreateInstance(new string[] {"A-nasal","N-end","T"}, .6f);
				
			case "Kiwi":
				
				return CreateInstance(new string[] {"K","E-long","W","E-long"}, .6f);
				
			case "Nut":
				
				return CreateInstance(new string[] {"N","U-short","T"}, .5f);
				
			case "Plum":
				
				return CreateInstance(new string[] {"P","L","U-short","M"}, .5f);
				
			case "Pot":
				
				return CreateInstance(new string[] {"P","O-short","T"}, .6f);
			
			// Camping
				
			case "Bat":
				
				return CreateInstance(new string[] {"B","A-short","T"}, .8f);
				
			case "Fox":
				
				return CreateInstance(new string[] {"F","O-short","X"}, .7f);
				
			case "Map":
				
				return CreateInstance(new string[] {"M","A-short","P"}, .6f);
				
			case "Star":
				
				return CreateInstance(new string[] {"S","T","O-short","R-end"}, .7f);
				
			case "Tent":
				
				return CreateInstance(new string[] {"T","E-short","N-end","T"}, .9f);

			default:

				return null;

			}
		}

	}
}
