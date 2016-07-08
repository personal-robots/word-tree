using UnityEngine;
using System.Collections;

// Stores properties of each level, including what words are included in the level, 
// and the position and scale of the backgound image customized for that level.

namespace WordTree
{
	public class LevelProperties : ScriptableObject {

		string[] words; // words in the level
		Vector3 backgroundPosn; // position of background object
		float backgroundScale; // scale of background object

		// Get words in the level
		public string[] Words()
		{
			return this.words;
		}

		// Get position of background object
		public Vector3 BackgroundPosn()
		{
			return this.backgroundPosn;
		}

		// Get scale of background object
		public float BackgroundScale()
		{
			return this.backgroundScale;
		}

		// Set properties of the level
		void Init (string[] words, Vector3 backgroundPosn, float backgroundScale)
		{
			this.words = words;
			this.backgroundPosn = backgroundPosn;
			this.backgroundScale = backgroundScale;
		}
		
		// Create an instance of LevelProperties, with the properties of the level set
		static LevelProperties CreateInstance(string[] words, Vector3 backgroundPosn, float backgroundScale)
		{
			LevelProperties prop = ScriptableObject.CreateInstance<LevelProperties>();
			prop.Init(words, backgroundPosn, backgroundScale);
			return prop;
		}

		// Get properties of the level
		// info for all levels stored here, so we can access when instantiating a level
		public static LevelProperties GetLevelProperties(string level)
		{
			switch(level) 
			{

			case "Animals":
			
				return CreateInstance(new string[] {"Cat","Dog","Rat","Pig","Hen"},new Vector3(0,-1,2),3.5f);

			case "Transportation":
				
				return CreateInstance(new string[] {"Bus","Jet","Cab","Van","Car"},new Vector3(0,-1,2),4.5f);

			case "Bathroom":
				
				return CreateInstance(new string[] {"Mop","Gel","Can","Tub","Lid"},new Vector3(0,2.5f,2),3.5f);

			case "Kitchen":
				
				return CreateInstance(new string[] {"Jug","Rag","Jar","Pan","Cup"},new Vector3(0,-2.5f,2),3.5f);

			case "Picnic":
				
				return CreateInstance(new string[] {"Bun","Ham","Jam","Bag","Taco"},new Vector3(0,3,2),3.5f);

			case "Pond":
				
				return CreateInstance(new string[] {"Log","Fly","Mud","Bug","Web"},new Vector3(0,3,2),3.5f);

			case "Bedroom":
				
				return CreateInstance(new string[] {"Rug","Fan","Mat","Crib","Bed"},new Vector3(0,3.5f,2),3.5f);

			case "School":
				
				return CreateInstance(new string[] {"Pen","Pad","Pin","Mug","Desk"},new Vector3(0,-1,2),3.5f);

			case "Playground":
				
				return CreateInstance(new string[] {"Net","Sun","Top","Box","Yoyo"},new Vector3(0,1,2),3.5f);

			case "Clothing":
				
				return CreateInstance(new string[] {"Cap","Hat","Wig","Tux","Vest"},new Vector3(0,2.5f,2),3.5f);

			case "Garden":
				
				return CreateInstance(new string[] {"Nut","Pot","Ant","Kiwi","Plum"},new Vector3(0,0,2),3.5f);

			case "Camping":
				
				return CreateInstance(new string[] {"Bat","Star","Map","Fox","Tent"},new Vector3(0,2.5f,2),3.5f);

			default:

				return null;


			}
	
		}


	}
}
