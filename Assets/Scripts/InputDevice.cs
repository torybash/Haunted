using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(RectTransform))]
public class InputDevice : MonoBehaviour {

	[SerializeField] private Text joyStickNrText;

	[SerializeField] private Color readyColor;

	public bool isReady;
	public InputData input;

	public RectTransform rt {get {return GetComponent<RectTransform>(); } }



	private MenuController menu {get;set;}



	private bool justMoved;


	public void Init(MenuController menu, InputType type, int idx){
		this.menu = menu;
		input.type = type;
		input.idx = idx;
		input.team = Team.Center;
//		if (type == InputType.Joystick) 
		joyStickNrText.text = ""+(idx+1);

		SetReady(false);
	}


	void Update(){
		float move = 0;
		if (input.type == InputType.Keyboard){
			move = Input.GetAxis("Horizontal");
			if (Mathf.Abs(move) > 0) MoveDir(move > 0 ? 1 : -1);

			if (Input.GetButton("Submit") && input.team != Team.Center){
				SetReady(true);
			}
		}else if (input.type == InputType.Joystick){
			move = Input.GetAxis("Joy" + input.idx + "LeftStickX");
			if (Mathf.Abs(move) > 0) MoveDir(move > 0 ? 1 : -1);

			if (Input.GetButton("Joy" + input.idx + "Start") && input.team != Team.Center){
				SetReady(true);
			}
		}

		if (Mathf.Abs(move) > 0) MoveDir(move > 0 ? 1 : -1);
		else justMoved = false;
	}

	private void MoveDir(int dir){ 		// -1=left : 1=rigth 
		if (justMoved) return;

		int newPos = (int) input.team + dir;
		newPos = Mathf.Clamp(newPos, 0, (int) Team.Ghosts);

		input.team = (Team) newPos;
		menu.MoveDevice(this, input.team);

		justMoved = true;
	}

	private void SetReady(bool ready){
		isReady = ready;
	}
}


[Serializable]
public class InputData{
	public InputType type;
	public int idx;
	public Team team;
}