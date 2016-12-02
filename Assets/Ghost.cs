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
	[SerializeField] private float phantomCooldown = 12f;

	[SerializeField] private ParticleSystem phantomingParticles;

	public InputData input;

	public PlayerStatPanel panel {get; set;}

	private Material faceMat;

	private Color defaultFaceEmissionClr;

	private bool holdingHide;

	private float phantomEndTime;
	private float nextPhantomAllowedTime;

	private Animator anim { get{ return GetComponent<Animator>(); } }


	void Awake(){
		faceMat = meshRend.materials[1];
		defaultFaceEmissionClr = faceMat.GetColor("_EmissionColor");
	}

	void Update(){
		holdingHide = false;

		if (input.type == InputType.Joystick){
			if (Input.GetButton("Joy" + input.idx + "A")){
				Debug.Log("player " + input.idx  + " pressed A!");

			}
			if (Input.GetButton("Joy" + input.idx + "X")){
				Debug.Log("player " + input.idx  + " pressed X!");

				Phantoming();
			}
			if (Input.GetButton("Joy" + input.idx + "B")){
				Debug.Log("player " + input.idx  + " pressed B!");

				Hiding();
			}
			if (Input.GetButton("Joy" + input.idx + "Y")){
				Debug.Log("player " + input.idx  + " pressed Y!");
			}
		}else if (input.type == InputType.Keyboard){
			if (Input.GetButtonDown("A")){

			}
			if (Input.GetButtonDown("X")){
				Phantoming();
			}
			if (Input.GetButton("B")){
				Hiding();
			}
		}


		//Hiding color
		var goalClr = holdingHide ? Color.black : defaultFaceEmissionClr;
		var currClr = faceMat.GetColor("_EmissionColor");
		faceMat.SetColor("_EmissionColor", Color.Lerp(currClr, goalClr, hideSpeed * Time.deltaTime));

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
			Debug.Log("EXPLODE!");
		}
	}


	private void Hiding(){
		holdingHide = true;
	}

	private void Phantoming(){
		if (Time.time > nextPhantomAllowedTime){
			nextPhantomAllowedTime = Time.time + phantomCooldown;

			phantomEndTime = Time.time + phantomDuration;
			gameObject.layer = LayerMask.NameToLayer("Phantoms");
			phantomingParticles.Play();

			panel.UseSkill(0, phantomCooldown);

			SoundManager.I.PlaySound("painr", transform);
		}
	}
}
