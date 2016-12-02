using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class Survivor : MonoBehaviour {

	[SerializeField] private Transform flashlightEnd;

	[SerializeField] private float defaultLightRange = 7f;
	[SerializeField] private float defaultLightAngle = 65f;
	[SerializeField] private float focusLightRange = 14f;
	[SerializeField] private float focusLightAngle = 32.5f;
	[SerializeField] private float flashlightFocusSpeed = 8f;
	[SerializeField] private float dashDuration = 0.4f;
	[SerializeField] private float dashCooldown = 5f;

	[SerializeField] private ParticleSystem dashParticles;
	[SerializeField] private Light flashlightLight;

	[SerializeField] private GameObject keyInHand;


	private PlayerMovement movement {get{ return GetComponent<PlayerMovement>(); } }

	public PlayerStatPanel panel {get; set;}

	public InputData input;

	private bool holdingFocus;
	private float currFocusFrac;

	private float dashEndTime;
	private float nextDashAllowedTime;

	private Animator anim { get{ return GetComponent<Animator>(); } }

	private ItemType _carryItem;

	private ItemType carryItem{
		get {return _carryItem;}
		set {
			_carryItem = value;
			keyInHand.SetActive(false);

			switch (_carryItem) {
			case ItemType.Key:
				keyInHand.SetActive(true);
				break;
//			case ItemType.Key:
//				keyInHand.SetActive(true);
//				break;
			}
		}
	}

	void Update () {
		//Flashlight detection
		foreach (var ghost in Game.I.Ghosts) {
			RaycastHit hit;

			Debug.DrawLine(flashlightEnd.position, flashlightEnd.position + transform.forward * flashlightLight.range, Color.green, 0.05f);

			var dir = (ghost.transform.position - flashlightEnd.transform.position).normalized;
//			if (Vector3.Dot(dir, transform.forward))
//			Debug.Log("Vector3.Dot(dir, transform.forward): "+ Vector3.Dot(dir, transform.forward));
//			Debug.Log("Vector3.Angle(dir, transform.forward): "+ Vector3.Angle(dir, transform.forward));

			if (Vector3.Angle(dir, transform.forward) > flashlightLight.spotAngle / 2f) continue;

			if (Physics.Raycast(flashlightEnd.position, dir, out hit, flashlightLight.range)){
				if (hit.collider.GetComponent<Ghost>() != null){
					hit.collider.GetComponent<Ghost>().HitByLight();
				}
			}
		}

		holdingFocus = false;

		if (input.type == InputType.Joystick){
			if (Input.GetButton("Joy" + input.idx + "A")){
				Debug.Log("player " + input.idx  + " pressed A!");
			}
			if (Input.GetButton("Joy" + input.idx + "X")){
				Debug.Log("player " + input.idx  + " pressed X!");

				Dash();
			}
			if (Input.GetButton("Joy" + input.idx + "B")){
				Debug.Log("player " + input.idx  + " pressed B!");

				FocusFlashlight();
			}
			if (Input.GetButton("Joy" + input.idx + "Y")){
				Debug.Log("player " + input.idx  + " pressed Y!");
			}
		}else if (input.type == InputType.Keyboard){
			if (Input.GetButtonDown("A")){

			}
			if (Input.GetButtonDown("X")){
				Dash();
			}
			if (Input.GetButton("B")){
				FocusFlashlight();
			}
		}

		//Flashlight focus
		var goalFrac = holdingFocus ? 1 : 0;
		currFocusFrac = Mathf.MoveTowards(currFocusFrac, goalFrac, flashlightFocusSpeed * Time.deltaTime);
		flashlightLight.range = Mathf.Lerp(defaultLightRange, focusLightRange, currFocusFrac);
		flashlightLight.spotAngle = Mathf.Lerp(defaultLightAngle, focusLightAngle, currFocusFrac);

		//Phantoming
		if (Time.time > dashEndTime){
			dashParticles.Stop();
			movement.EnableDash(false);
		}
	}


	private void FocusFlashlight(){
		holdingFocus = true;
	}

	private void Dash(){
		if (Time.time > nextDashAllowedTime){
			nextDashAllowedTime = Time.time + dashCooldown;

			dashEndTime = Time.time + dashDuration;
			movement.EnableDash(true);
			dashParticles.Play();

			panel.UseSkill(0, dashCooldown);
		}
	}






	void OnTriggerEnter(Collider other){
		if (other.tag == "Key" && carryItem == ItemType.None){
			carryItem = ItemType.Key;
			other.gameObject.SetActive(false);

			SoundManager.I.PlaySound("metalLatch", transform);
		}else if (other.tag == "Door" && carryItem == ItemType.Key){
			other.GetComponent<ExitDoor>().Open();


		}
	}

	void OnCollisionEnter(Collision coll){
		if (coll.collider.tag == "Ghost"){
			Debug.Log("SURVIVOR DIE!!");
		}
	}
}
