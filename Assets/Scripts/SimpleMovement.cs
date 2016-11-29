using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class SimpleMovement : MonoBehaviour {

	private Rigidbody rb { get { return GetComponent<Rigidbody>(); } }
	
	[SerializeField] private float speed = 2.5f;

	void FixedUpdate () {
		var newPos = rb.position + new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed;
		rb.MovePosition(newPos);

		var screenPos = Camera.main.WorldToScreenPoint(transform.position);
		//var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//mouseWorldPos.z = transform.position.z;

		float angle = Mathf.Atan2(-(screenPos.y - Input.mousePosition.y), screenPos.x - Input.mousePosition.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.up);
		Debug.Log("angle: " + angle + ", screenPos:" + screenPos.ToString("F5") + ", Input.mousePosition: "+ Input.mousePosition.ToString("F5"));
	}
}
