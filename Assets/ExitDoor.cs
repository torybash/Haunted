using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class ExitDoor : MonoBehaviour {

	private Animator anim {get {return GetComponent<Animator>(); } }

	private bool open;

	public void Open(){
		//TODO
		if (open) return;

		open = true;
		Debug.Log("SURVIVORS WIN!!");

		anim.Play("Open");

		SoundManager.I.PlaySound("doorOpen_1", transform);
	}
}
