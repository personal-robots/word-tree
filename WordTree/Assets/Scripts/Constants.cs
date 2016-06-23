using UnityEngine;
using System.Collections;

//Keep list of names for tags

namespace WordTree
{
	public class Constants : MonoBehaviour {

		//Names for tags of gameObjects
		public static class Tags{
			public const string TAG_MAIN_CAMERA = "MainCamera";
			public const string TAG_LOCK = "Lock";
			public const string TAG_KID = "Kid";
			public const string TAG_GESTURE_MANAGER= "GestureManager";
			public const string TAG_BUTTON="Button";
			public const string TAG_LEVEL_ICON= "LevelIcon";
			public const string TAG_WORD_OBJECT= "WordObject";
			public const string TAG_TARGET_LETTER = "TargetLetter";
			public const string TAG_MOVABLE_LETTER = "MovableLetter";
			public const string TAG_TARGET_BLANK = "TargetBlank";
			public const string TAG_MOVABLE_BLANK = "MovableBlank";
			public const string TAG_JAR= "Jar";
			public const string TAG_HINT = "Hint";
		}
	}
}