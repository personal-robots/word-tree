using UnityEngine;
using System.Collections;

// The object this behavior is attached to will "pulse", enlarging
// a little and shrinking back to the original size repeatedly

namespace WordTree
{
	public class PulseBehavior : MonoBehaviour {

		// Starts pulsing an object
		// Indicates that the object can be interacted with
		public void StartPulsing(GameObject go, float delayTime = 0)
		{
			float scaleUpBy = 1.2f; // how much to scale up object by
			float time = Random.Range(.8f,1.0f); // time to complete one pulse, randomized so each object is pulsing at different rate (looks better)
			
			LeanTween.scale(go, new Vector3(go.transform.localScale.x * scaleUpBy, go.transform.localScale.y * scaleUpBy, 
			                                go.transform.localScale.z * scaleUpBy), time)
				.setEase(LeanTweenType.easeOutSine).setLoopPingPong().setDelay(delayTime);
		}

		// Stops pulsing the object
		public void StopPulsing (GameObject go)
		{
			LeanTween.cancel(go);
		}

	}
}
