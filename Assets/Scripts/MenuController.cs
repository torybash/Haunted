using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MenuController : MonoBehaviour {

	[SerializeField] private UIPanel mainPanel;
	[SerializeField] private UIPanel inputSelectPanel;


	[SerializeField] private InputDevice inputTemplate;
	[SerializeField] private Sprite keyboardSprite;
	[SerializeField] private Sprite gamepadSprite;


	[SerializeField] private RectTransform ghostCont;
	[SerializeField] private RectTransform survivorCont;
	[SerializeField] private RectTransform centerCont;

	[SerializeField] private bool DEBUGGING;

	private List<InputDevice> devices = new List<InputDevice>();

	void Start(){
		mainPanel.SetEnabled(true);
		inputSelectPanel.SetEnabled(false);
	}


	void Update(){
		//TODO detect new/lost input devices
		if (inputSelectPanel.isEnabled && Input.GetJoystickNames().Length != devices.Count - 1){
			OnPlayClicked();
		}

		if (Input.GetButton("Submit")){
			bool hasGhost = devices.Exists(x => x.input.team == Team.Ghosts);
			bool hasSurvivor = devices.Exists(x => x.input.team == Team.Survivors);

			if ((hasGhost && hasSurvivor) || DEBUGGING){
				Manager.I.StartGame(devices.Where(x => x.input.team != Team.Center).Select(x => x.input).ToList());
			}
		}

	}


	public void OnPlayClicked(){
		mainPanel.SetEnabled(false);
		inputSelectPanel.SetEnabled(true);


		foreach (var item in devices) {
			if (item) Destroy(item.gameObject);
		}
		devices.Clear();
		CreateInputDevice(InputType.Keyboard, 0);
		for (int i = 0; i < Input.GetJoystickNames().Length; i++) {
			CreateInputDevice(InputType.Joystick, i + 1);
		}
	}


	public void OnAllReady(){
//		mainPanel.isEnabled = false;
//		inputSelectPanel.isEnabled = true;
//
//		CreateInputDevice(InputType.Keyboard, 0);
//		for (int i = 0; i < Input.GetJoystickNames().Length; i++) {
//			CreateInputDevice(InputType.Joystick, i + 1);
//		}
	}

	private void CreateInputDevice(InputType type, int idx){
		var device = (InputDevice) Instantiate(inputTemplate, inputTemplate.transform.parent);
		device.gameObject.SetActive(true);
		switch (type) {
		case InputType.Keyboard:
			device.GetComponent<Image>().sprite = keyboardSprite;
			break;
		case InputType.Joystick:
			device.GetComponent<Image>().sprite = gamepadSprite;
			break;
		}

		device.Init(this, type, idx);

		MoveDevice(device, Team.Center);
		devices.Add(device);
	}

	private void UpdateInputDevices(){
		
	}


	public void MoveDevice(InputDevice device, Team pos){
		switch (pos) {
		case Team.Survivors:
			device.transform.SetParent(survivorCont);
			break;
		case Team.Center:
			device.transform.SetParent(centerCont);
			break;
		case Team.Ghosts:
			device.transform.SetParent(ghostCont);
			break;
		}
			
		var newPos = device.GetComponent<RectTransform>().anchoredPosition;
		newPos.x = 0;
		newPos.y = -100f - device.input.idx * 80f;
		device.GetComponent<RectTransform>().anchoredPosition = newPos;
	}
}


public enum InputType{
	Keyboard,
	Joystick
}

