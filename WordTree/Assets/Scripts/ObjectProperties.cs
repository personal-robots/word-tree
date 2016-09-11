using UnityEngine;
using System.Collections;

//<summary>
// Properties of a game object
//</summary>
namespace WordTree
{
	public class ObjectProperties : ScriptableObject
	{
		// name of object
		private string objName;
		// tag of object
		private string tag;
		// initial position
		private Vector3 initPosn;
		// scale of object
		private Vector3 scale;
		// name of image to load
		private string sprite;
		// name of audio clip to load
		private string audioFile;
		//<summary>	
		// set all properties
		//</summary>
		public void Init(string name, string tag, Vector3 posn, Vector3 scale, string sprite, 
			string audioFile)
		{
			this.objName = name;
			this.tag = tag;
			this.initPosn = posn;
			this.scale = scale;
			this.sprite = sprite;
			this.audioFile = audioFile;
		}

		//<summary>
		// create instance of ObjectProperties class
		//</summary>
		public static ObjectProperties CreateInstance(string name, string tag, Vector3 posn, Vector3 scale, 
			string sprite, string audioFile)
		{
			ObjectProperties prop = ScriptableObject.CreateInstance<ObjectProperties>();
			prop.Init(name, tag, posn, scale, sprite, audioFile);
			return prop;
		}

		//<summary>
		// instantiate new game object with specified properties
		//</summary>
		public static void InstantiateObject(ObjectProperties prop)
		{
			// create new game object
			GameObject go = new GameObject();
			// set name
			go.name = prop.Name();
			//Add properties component to each letter/sound created
			Properties props = go.AddComponent<Properties>();
			Debug.Log("Created new object " + go.name);
			// set tag
			go.tag = prop.Tag();
			// set initial position
			go.transform.position = prop.InitPosn();
			// set scale of sprite image
			go.transform.localScale = prop.Scale();
			// load sprite image
			SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
			// image file needs to be in an existing Assets/Resources folder or subfolder
			Sprite sprite = Resources.Load<Sprite>("Graphics/" + prop.Sprite());
			if (sprite == null) 
			{
				Debug.LogError("ERROR: could not load sprite " + prop.Name());
			}
			spriteRenderer.sprite = sprite;
			// load audio clip
			if (prop.AudioFile() != null) 
			{
				AudioSource audioSource = go.AddComponent<AudioSource>();
				// audio file needs to be in an existing Assets/Resources folder or subfolder
				AudioClip clip = Resources.Load("Audio/" + prop.AudioFile()) as AudioClip;
				if (clip == null) 
				{
					Debug.LogError("ERROR: could not load audioclip " + prop.AudioFile());
					audioSource.clip = clip;
					audioSource.playOnAwake = false;
				}
				if (go.tag != Constants.Tags.TAG_TARGET_LETTER && go.tag != 
					Constants.Tags.TAG_TARGET_BLANK && go.tag != Constants.Tags.TAG_JAR) 
				{
					// add polygon collider that matches shape of object
					// used to detect touches and collisions
					go.AddComponent<PolygonCollider2D>();
				}
				if (go.tag == Constants.Tags.TAG_TARGET_LETTER || go.tag == Constants.Tags.TAG_TARGET_BLANK) 
				{
					// add circle collider
					// used to detect touches and collisions
					CircleCollider2D cc2d = go.AddComponent<CircleCollider2D>();
					// set as trigger so OnTriggerEnter2D function is called when collider is hit
					cc2d.isTrigger = true;
					// set radius for circle collider
					// want radius to be small, to make it easier for users to drag letters or sound blanks to where they want
					// without accidentally colliding with another object
					if (go.tag == Constants.Tags.TAG_TARGET_LETTER) 
					{
						if (ProgressManager.currentMode == 1) 
						{
							cc2d.radius = .1f;
						}
						if (ProgressManager.currentMode == 3) 
						{
							cc2d.radius = 5f;
						}
					}
					if (go.tag == Constants.Tags.TAG_TARGET_BLANK) 
					{
						cc2d.radius = .3f;
					}
				} 

				if (go.tag == Constants.Tags.TAG_MOVABLE_LETTER || go.tag == Constants.Tags.TAG_MOVABLE_BLANK) 
				{
					// add rigidbody if object is draggable
					Rigidbody2D rb2d = go.AddComponent<Rigidbody2D>();
					// remove object from physics engine's control, because we don't want
					// the object to move with gravity, forces, etc. - we do the moving
					rb2d.isKinematic = true;
					// don't want gravity, otherwise object will fall
					rb2d.gravityScale = 0;
				}
				// subscribe to gestures
				GestureManager gm = go.AddComponent<GestureManager>();
				gm.AddAndSubscribeToGestures(go); 
				// add pulse behavior - draws attention to interactive objects
				go.AddComponent<PulseBehavior>();
				if (go.tag == "Hint") 
				{
					// set hint letter color to black
					go.GetComponent<SpriteRenderer>().color = Color.black;
					// create hint letter behind the background
					go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, 3);
				}	
			}
		}

		//<summary>
		// get object name
		//</summary>
		public string Name()
		{
			return this.objName;
		}

		//<summary>
		// get object tag
		//</summary>
		public string Tag()
		{
			return this.tag;
		}

		//<summary>
		// get initial position
		//</summary>
		public Vector3 InitPosn()
		{
			return this.initPosn;
		}

		//<summary>
		// get object scale
		//</summary>
		public Vector3 Scale()
		{
			return this.scale;
		}

		//<summary>
		// get file name of sprite
		//</summary>
		public string Sprite()
		{
			return this.sprite;
		}

		//<summary>
		// get file name of audio clip
		//</summary>
		public string AudioFile()
		{
			return this.audioFile;
		}
			
	}
}
