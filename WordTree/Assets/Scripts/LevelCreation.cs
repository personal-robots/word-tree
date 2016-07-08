using UnityEngine;
using System.Collections;

// Handles creating the word objects in the specified level.
// Currently works for levels with 5 words.
// Naming conventions: Name for level icon must be same as name for level. (e.g. "Animals")
//                     File name for background image must be same as name for level. (e.g. "Animals")
//					   File name for image of word must be same as the word. (e.g. "Dog")
//                     File name for audio clip of the word must be same as the word. (e.g. "Dog")
//                     Names for levels and words are specified in LevelProperties and WordProperties scripts.

namespace WordTree
{
	public class LevelCreation : MonoBehaviour {

		// Create word objects. Takes in the current level (category of words) and a list of objects (words).
		public static void CreateWordObjects(string level, string[] objects)
		{
			Vector3[] position = new Vector3[objects.Length]; // stores the desired position for each object
			float[] scale = new float[objects.Length]; // stores the appropriate scale for each object

			// Set x-position for each object
			if (objects.Length == 5) {
				float x1 = 6;
				float x2 = 3;
				float y1 = 2.5f;

				position = new Vector3[5] {
					new Vector3(-x1, 0, 0),
					new Vector3(0, y1, 0),
					new Vector3(x1, 0, 0),
					new Vector3(x2, -y1, 0),
					new Vector3(-x2, -y1, 0)
				};
			}

			for (int i=0; i<objects.Length; i++) {
				// Get appropriate scale for the object (info stored in WordProperties script)
				scale[i] = WordProperties.GetWordProperties(objects[i]).ObjScale();

				// Instantiate word object, according to the given input
				ObjectProperties obj = ObjectProperties.CreateInstance(objects [i], "WordObject", position [i], new Vector3 (scale [i], scale [i], 1), level + "/" + objects [i], "Words/" + objects [i]);
				ObjectProperties.InstantiateObject(obj);
			}


		}

	}
}
