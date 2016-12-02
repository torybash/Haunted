using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class PlayerMovement : MonoBehaviour {

	private Rigidbody rb { get { return GetComponent<Rigidbody>(); } }

	public InputData input;

	[SerializeField] private float speed = 0.1f;
	[SerializeField] private float dashSpeed = 0.3f;

	private float lastAngle;

	private bool isDashEnabled;

	public float slowdownFrac;

	private Animator anim { get{ return GetComponent<Animator>(); } }


	public void Init(InputData input){
		this.input = input;
	}


	void FixedUpdate () {
		var screenPos = Camera.main.WorldToScreenPoint(transform.position);

		var newPos = rb.position;
		float angle = lastAngle;

		var currSpeed = (1 - slowdownFrac) * speed;
		if (input.type == InputType.Keyboard){
			newPos = rb.position + new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * currSpeed;
			angle = Mathf.Atan2(-(screenPos.y - Input.mousePosition.y), screenPos.x - Input.mousePosition.x) * Mathf.Rad2Deg - 90f;
		}else{
			newPos = rb.position + new Vector3(Input.GetAxis("Joy" + input.idx + "LeftStickX"), 0, Input.GetAxis("Joy" + input.idx + "LeftStickY")) * currSpeed;
			var lookAxis = new Vector2(Input.GetAxis("Joy" + input.idx + "RightStickX"), Input.GetAxis("Joy" + input.idx + "RightStickY"));
			if (lookAxis.magnitude > 0){
				angle = Mathf.Atan2(lookAxis.x, lookAxis.y) * Mathf.Rad2Deg;	
			}
		}

		if (isDashEnabled){
			var dashDir = new Vector3(Mathf.Sin(Mathf.Deg2Rad * lastAngle), 0, Mathf.Cos(Mathf.Deg2Rad * lastAngle));
			newPos = rb.position + dashDir * dashSpeed;
		}else{
			lastAngle = angle;
		}

		float distToMove = (newPos - transform.position).magnitude;
		anim.SetFloat("Speed", distToMove / speed);
//		anim.SetFloat("Speed", distToMove);

		rb.MovePosition(newPos);
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
	}


	public void EnableDash(bool enable){
		isDashEnabled = enable;
	}
}
