using UnityEngine;
using System.Linq;
using System.Collections;
using System;

[RequireComponent(typeof(Animator))]
public class Survivor : MonoBehaviour {

	[SerializeField] private Transform flashlightEnd;

	[SerializeField] private float defaultLightRange = 7f;
	[SerializeField] private float defaultLightAngle = 65f;
	[SerializeField] private float focusLightRange = 14f;
	[SerializeField] private float focusLightAngle = 32.5f;
	[SerializeField] private float flashlightFocusSpeed = 8f;
	[SerializeField] private float dashDuration = 0.4f;
	[SerializeField] private float dashEnergyUse = 0.28f;
//	[SerializeField] private float dashCooldown = 5f;
	[SerializeField] private float flashlightEnergyUse = 0.25f;
	[SerializeField] private float batteryEnergyAmount = 0.5f;
	[SerializeField] private float torchActivateRadius = 2.5f;
	[SerializeField] private float trapSlowDuration = 2.2f;
	[SerializeField] private float trapSlowFrac = 0.75f;

	[SerializeField] private ParticleSystem dashParticles;
	[SerializeField] private Light flashlightLight;

	[SerializeField] private GameObject keyInHand;

	[SerializeField] private float energy;

	private PlayerMovement movement {get{ return GetComponent<PlayerMovement>(); } }

	public PlayerStatPanel panel {get; set;}

	public InputData input;

	private bool holdingFocus;
	private float currFocusFrac;

	private float dashEndTime;
//	private float nextDashAllowedTime;

	private float trapSlowedEndTime;


	private bool flashlightOn = true;

	private bool justUsedDash;
	private bool justUsedToggle;
	private bool justUsedActivate;


	public bool dead;


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
		if (dead) return;

		//Flashlight detection
		foreach (var ghost in Game.I.Ghosts) {
			RaycastHit hit;

			Debug.DrawLine(flashlightEnd.position, flashlightEnd.position + transform.forward * flashlightLight.range, Color.green, 0.05f);

			var dir = (ghost.transform.position - flashlightEnd.transform.position).normalized;
//			if (Vector3.Dot(dir, transform.forward))
//			Debug.Log("Vector3.Dot(dir, transform.forward): "+ Vector3.Dot(dir, transform.forward));
//			Debug.Log("Vector3.Angle(dir, transform.forward): "+ Vector3.Angle(dir, transform.forward));

			if (Vector3.Angle(dir, transform.forward) > flashlightLight.spotAngle / 2f) continue;

			panel.SetSpooked(false);
			if (Physics.Raycast(flashlightEnd.position, dir, out hit, flashlightLight.range)){
				if (hit.collider.GetComponent<Ghost>() != null){
					hit.collider.GetComponent<Ghost>().HitByLight();
					panel.SetSpooked(true);
				}
			}
		}

		holdingFocus = false;

		if (input.type == InputType.Joystick){
			if (Input.GetButton("Joy" + input.idx + "A")){
				Debug.Log("player " + input.idx  + " GetButton A!");
				if (!justUsedActivate){
					Activate();
					justUsedActivate = true;
				}
			}else{
				justUsedActivate = false;
			}
			if (Input.GetButton("Joy" + input.idx + "X")){
//				Debug.Log("player " + input.idx  + " pressed X!");
				if (!justUsedDash){
					Dash();
					justUsedDash = true;
				}
			}else{
				justUsedDash = false;
			}
			if (Input.GetButton("Joy" + input.idx + "B")){
//				Debug.Log("player " + input.idx  + " pressed B!");
				FocusFlashlight();
			}
			if (Input.GetButton("Joy" + input.idx + "Y")){
//				Debug.Log("player " + input.idx  + " pressed Y!");
				if (!justUsedToggle){
					justUsedToggle = true;
					ToggleFlashlight();
				}

			}else{
				justUsedToggle = false;
			}
		}else if (input.type == InputType.Keyboard){
			if (Input.GetButtonDown("A")){
				Activate();
			}
			if (Input.GetButtonDown("X")){
				Dash();
			}
			if (Input.GetButton("B")){
				FocusFlashlight();
			}
			if (Input.GetButtonDown("Y")){
				ToggleFlashlight();
			}
		}


		//Trap slow
		if (Time.time < trapSlowedEndTime){
			GetComponent<PlayerMovement>().slowdownFrac = trapSlowFrac;
		}else{
			GetComponent<PlayerMovement>().slowdownFrac = 0;
		}


		//Flashlight on
		if (flashlightOn){
			energy = Mathf.Clamp01(energy - flashlightEnergyUse * Time.deltaTime);
			panel.SetEnergy(energy);
			if (energy == 0 && flashlightOn){
				ToggleFlashlight();
			}
			//			panel.UseEnergy();
		}

		//Flashlight focus
		if (flashlightOn){
			var goalFrac = holdingFocus ? 1 : 0;
			currFocusFrac = Mathf.MoveTowards(currFocusFrac, goalFrac, flashlightFocusSpeed * Time.deltaTime);
			flashlightLight.range = Mathf.Lerp(defaultLightRange, focusLightRange, currFocusFrac);
			flashlightLight.spotAngle = Mathf.Lerp(defaultLightAngle, focusLightAngle, currFocusFrac);
		}


		//Phantoming
		if (Time.time > dashEndTime){
			dashParticles.Stop();
			movement.EnableDash(false);
		}


	}


	private void Activate(){
		var colliders = Physics.OverlapSphere(transform.position, torchActivateRadius, LayerMask.GetMask("Torches"));
		foreach (var item in colliders) {
			var torch = item.GetComponent<TorchLight>();
			if (torch) if (!torch.turnedOn) torch.SetTurnedOn(true);
		}
	}

	private void ToggleFlashlight(){
		flashlightOn = !flashlightOn;
		flashlightLight.enabled = flashlightOn;
		if (flashlightOn){
			flashlightLight.range = defaultLightRange;
			flashlightLight.spotAngle = defaultLightAngle;
		}
	}

	private void FocusFlashlight(){
		holdingFocus = true;
	}

	private void Dash(){
		if (energy > dashEnergyUse){
		//		if (Time.time > nextDashAllowedTime){
//			nextDashAllowedTime = Time.time + dashCooldown;

			energy = Mathf.Clamp01(energy - dashEnergyUse);
			panel.SetEnergy(energy);

			dashEndTime = Time.time + dashDuration;
			movement.EnableDash(true);
			dashParticles.Play();

//			panel.UseSkill(0, dashCooldown);
		}
	}



	private void PickupBattery(){
		energy = Mathf.Clamp01(energy + batteryEnergyAmount);
	}
		

	void OnTriggerEnter(Collider other){
		if (dead) return;

		if (other.tag == "Key" && carryItem == ItemType.None){
			carryItem = ItemType.Key;
			other.gameObject.SetActive(false);

			SoundManager.I.PlaySound("metalLatch", transform);
		}else if (other.tag == "Battery"){
			PickupBattery();
			Destroy(other.gameObject);
			SoundManager.I.PlaySound("bookPlace1", transform);

		}else if (other.tag == "Door" && carryItem == ItemType.Key){
			other.GetComponent<ExitDoor>().Open();
			carryItem = ItemType.None;

			Game.I.SurvivorsWin();

		}else if (other.tag == "Trap"){
			bool steppedOn = other.GetComponent<GhostTrap>().SteppedOn();

			if (steppedOn){
				trapSlowedEndTime = Time.time + trapSlowDuration;

				SoundManager.I.PlaySound("creak1", transform);
			}
		}
	}

	void OnCollisionEnter(Collision coll){
		if (coll.collider.tag == "Ghost"){
			Debug.Log("SURVIVOR DIE!!");
			anim.Play("Die");

			dead =  true;

			if (flashlightOn){
				ToggleFlashlight();
			}

			panel.SetDead(true);


			SoundManager.I.PlaySound("creak1", transform);

			GetComponent<PlayerMovement>().enabled = false;
			GetComponent<Rigidbody>().isKinematic = true;
		}
	}
}
