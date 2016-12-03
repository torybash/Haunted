using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour {


	Quaternion startRotation;

	void Awake(){
		startRotation = transform.rotation;
	}

	void Update () {
		transform.rotation = startRotation;	
	}
}
