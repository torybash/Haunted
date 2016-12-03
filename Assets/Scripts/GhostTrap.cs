using UnityEngine;
using System.Collections;


public class GhostTrap : MonoBehaviour {

	[SerializeField] float fadeDuration;
	[SerializeField] float duration;

	private Material mat;

	private bool active;

	void Awake(){
		mat = GetComponent<Renderer>().material;
	}
		

	public void Init(){
		active = true;
		StartCoroutine(DoStart());
	}

	public bool SteppedOn(){
		if (!active) return false;

		Disappear();
		SoundManager.I.PlaySound("bookPlace1", transform);

		return true;
	}

	private void Disappear(){
		if (!active) return;

		active = false;
		StartCoroutine(DoDisappear());
	}

	private IEnumerator DoStart(){
		yield return new WaitForSeconds(duration);
		Disappear();
	}


	private IEnumerator DoDisappear(){
		
		float endTime = Time.time + fadeDuration;
		while (Time.time < endTime){

			float t = (endTime - Time.time) / fadeDuration; // 1 --> 0

			var col = mat.color;
			col.a = t;
			mat.color = col;
			yield return null;
		}
		Destroy(gameObject);
	}
}
