using UnityEngine;
using System.Collections;

public class DELME_InputTest : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < Input.GetJoystickNames().Length; i++) {
			Debug.Log("Joy " + i + " Input.GetJoystickNames: "+ Input.GetJoystickNames()[i]);
		}	
	}
}
