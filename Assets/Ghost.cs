using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class Ghost : MonoBehaviour {

	[SerializeField] SkinnedMeshRenderer meshRend;

	public float lighted;

	[SerializeField] private float ligthIncreaseSpeed = 0.5f;
	[SerializeField] private float ligthDecreaseSpeed = 0.2f;
	[SerializeField] private float hideSpeed = 20f;
	[SerializeField] private float phantomDuration = 2.5f;
//	[SerializeField] private float phantomCooldown = 12f;
	[SerializeField] private float trapEnergyUse = 0.45f;
	[SerializeField] private float phantomEnergyUse = 0.45f;
	[SerializeField] private float energyRechargeSpeed = 0.1f; //energy pr sec
	[SerializeField] private float torchActivateRadius = 2.5f;
	[SerializeField] private float torchLightRadius = 5.0f;
	[SerializeField] private float torchLightSlowdown = 0.5f;

	[SerializeField] private ParticleSystem phantomingParticles;

	[SerializeField] private GhostTrap trapPrefab;


	public float energy;

	public InputData input;

	public PlayerStatPanel panel {get; set;}

	private Material faceMat;

	private Color defaultFaceEmissionClr;

	private bool holdingHide;

	private float phantomEndTime;
	private float nextPhantomAllowedTime;

	private float currHideFrac;

	private bool justUsedTrap;
	private bool justUsedPhantom;
	private bool justUsedActivate;

//	private AudioSource hideAudio;

	private Animator anim { get{ return GetComponent<Animator>(); } }


	void Awake(){
		faceMat = meshRend.materials[1];
		defaultFaceEmissionClr = faceMat.GetColor("_EmissionColor");
	}

	void Update(){
		

		if (input.type == InputType.Joystick){
			if (Input.GetButton("Joy" + input.idx + "A")){
				Debug.Log("player " + input.idx  + " pressed A!");
				if (!justUsedActivate) {
					justUsedActivate = true;
					Activate();
				}
			}else{
				justUsedActivate = false;
			}
			if (Input.GetButton("Joy" + input.idx + "X")){
				Debug.Log("player " + input.idx  + " pressed X!");
				if (!justUsedPhantom) {
					justUsedPhantom = true;
					Phantoming();
				}
			}else{
				justUsedPhantom = false;
			}
			if (Input.GetButton("Joy" + input.idx + "B")){
				Debug.Log("player " + input.idx  + " pressed B!");

				Hiding();
			}else{
				holdingHide = false;
//				if (hideAudio != null && hideAudio.isPlaying) hideAudio.Stop();
			}
			if (Input.GetButton("Joy" + input.idx + "Y")){
				Debug.Log("player " + input.idx  + " pressed Y!");
				if (!justUsedTrap) {
					justUsedTrap = true;
					PlaceTrap();
				}
			}else{
				justUsedTrap = false;
			}
		}else if (input.type == InputType.Keyboard){
			if (Input.GetButtonDown("A")){
				Activate();
			}
			if (Input.GetButtonDown("X")){
				Phantoming();
			}
			if (Input.GetButton("B")){
				Hiding();
			}else{
				holdingHide = false;
			}
			if (Input.GetButtonDown("Y")){
				PlaceTrap();
			}
		}


		//Close to torch
		var colliders = Physics.OverlapSphere(transform.position + Vector3.up * 1.5f, torchLightRadius, LayerMask.GetMask("Torches"));
		foreach (var item in colliders) {
			var torch = item.GetComponent<TorchLight>();
			if (torch.turnedOn) {
				lighted = Mathf.Max(lighted, torchLightSlowdown);
			}
		}


		//Energy recharge
		if (energy < 1 && !holdingHide){
			energy = Mathf.Clamp01(energy + energyRechargeSpeed * Time.deltaTime);
			panel.SetEnergy(energy);
		}

		//Hiding color
		var goalHideFrac = holdingHide ? 1 : 0;
		currHideFrac = Mathf.MoveTowards(currHideFrac, goalHideFrac, hideSpeed * Time.deltaTime);
//		var goalClr = holdingHide ? Color.black : defaultFaceEmissionClr;
//		var currClr = faceMat.GetColor("_EmissionColor");
		faceMat.SetColor("_EmissionColor", Color.Lerp(defaultFaceEmissionClr, Color.black, currHideFrac));
		panel.SetAlpha(1 - currHideFrac);

		//Phantoming
		if (Time.time > phantomEndTime){
			phantomingParticles.Stop();
			gameObject.layer = LayerMask.NameToLayer("Default");
		}

		//Lighted cooldown
		if (lighted > 0){
			lighted -= Time.deltaTime * ligthDecreaseSpeed;
			lighted = Mathf.Clamp01(lighted);
			anim.SetFloat("Lighted", lighted);
		}

		GetComponent<PlayerMovement>().slowdownFrac = lighted;
	}

	public void HitByLight(){
		lighted += Time.deltaTime * ligthIncreaseSpeed;

		if (lighted > 1){
			Debug.Log("EXPLODE!?");
		}
	}

	private void Activate(){
		var colliders = Physics.OverlapSphere(transform.position + Vector3.up * 1.5f, torchActivateRadius, LayerMask.GetMask("Torches"));
		foreach (var item in colliders) {
			var torch = item.GetComponent<TorchLight>();
			if (torch.turnedOn) torch.SetTurnedOn(false);
		}
	}

	private void Hiding(){
		if (holdingHide) return;
		holdingHide = true;

//		hideAudio = SoundManager.I.PlaySound("Ambience_MonstersBelly_00", null, true);
	}

	private void PlaceTrap(){
		if (energy > trapEnergyUse){
			energy = Mathf.Clamp01(energy - trapEnergyUse);
			panel.SetEnergy(energy);

			var trap = Instantiate(trapPrefab);
			trap.transform.position = transform.position; //TODO set y correctly?

			trap.Init();
		}
	}

	private void Phantoming(){
		if (energy > phantomEnergyUse){
//				if (Time.time > nextPhantomAllowedTime){
//			nextPhantomAllowedTime = Time.time + phantomCooldown;

			phantomEndTime = Time.time + phantomDuration;
			gameObject.layer = LayerMask.NameToLayer("Phantoms");
			phantomingParticles.Play();

//			panel.UseSkill(0, phantomCooldown);

			energy = Mathf.Clamp01(energy - phantomEnergyUse);
			panel.SetEnergy(energy);

			SoundManager.I.PlaySound("grunt2", transform);
		}
	}
}
