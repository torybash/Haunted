using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class ExitDoor : MonoBehaviour {

	private Animator anim {get {return GetComponent<Animator>(); } }

	[SerializeField] Light lght;

	private bool open;

	public int doorIdx;
	public bool active;


	public void Activate(){
		if (active) return;
		active = true;
		lght.gameObject.SetActive(true);
	}

	public void Open(){
		//TODO
		if (open) return;

		open = true;
		Debug.Log("SURVIVORS WIN!!");

		anim.Play("Open");

		SoundManager.I.PlaySound("doorOpen_1", transform);
	}
}
