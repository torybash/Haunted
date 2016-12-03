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

	private bool justClickedStart;

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

			if (Input.GetButtonDown("Submit") && input.team != Team.Center){
				SetReady(!isReady);
			}
		}else if (input.type == InputType.Joystick){
			move = Input.GetAxis("Joy" + input.idx + "LeftStickX");
			if (Mathf.Abs(move) > 0) MoveDir(move > 0 ? 1 : -1);

			if (Input.GetButton("Joy" + input.idx + "Start")){
				if (!justClickedStart && input.team != Team.Center) {
					SetReady(!isReady);
				}
				justClickedStart = true;
			}else{
				justClickedStart = false;
			}
		}

		if (Mathf.Abs(move) > 0) MoveDir(move > 0 ? 1 : -1);
		else justMoved = false;
	}

	private void MoveDir(int dir){ 		// -1=left : 1=rigth 
		if (justMoved || isReady) return;

		int newPos = (int) input.team + dir;
		newPos = Mathf.Clamp(newPos, 0, (int) Team.Ghosts);

		var oldTeam = input.team;
		input.team = (Team) newPos;
		if (input.team != oldTeam) menu.MoveDevice(this, input.team);

		justMoved = true;
	}

	private void SetReady(bool ready){
		if (isReady) SoundManager.I.PlaySound("Bonecrack_01", null);
//		else SoundManager.I.PlaySound("Bonecrack_02", null);

		isReady = ready;

		GetComponent<Image>().color = isReady ? readyColor : Color.white;
	}
}


[Serializable]
public class InputData{
	public InputType type;
	public int idx;
	public Team team;
}