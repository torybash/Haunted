using UnityEngine;
using System.Collections;

public class Battery : MonoBehaviour {

	[SerializeField] float rotSpeed = 0.1f;

	
	void Update () {

		transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime, Space.World);
	}
}
