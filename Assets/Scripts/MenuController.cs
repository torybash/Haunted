using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class MenuController : MonoBehaviour {

	[SerializeField] private UIPanel mainPanel;
	[SerializeField] private UIPanel inputSelectPanel;
	[SerializeField] private UIPanel countdownPanel;


	[SerializeField] private Text countdownText;

	[SerializeField] private InputDevice inputTemplate;
	[SerializeField] private Sprite keyboardSprite;
	[SerializeField] private Sprite gamepadSprite;


	[SerializeField] private RectTransform ghostCont;
	[SerializeField] private RectTransform survivorCont;
	[SerializeField] private RectTransform centerCont;

	[SerializeField] private int countdownStart = 5;
	[SerializeField] private float countdownDuration = 0.5f;



	private const bool DEBUGGING = false;

	private List<InputDevice> devices = new List<InputDevice>();

	void Start(){
		mainPanel.SetEnabled(true);
		inputSelectPanel.SetEnabled(false);

		SoundManager.I.PlaySound("Ambience_Hell_00", null, true);
	}


	void Update(){
		//TODO detect new/lost input devices
		if (inputSelectPanel.isEnabled && Input.GetJoystickNames().Length != devices.Count - 1){
			OnPlayClicked();
		}

		if (devices.All(x => (x.isReady || x.input.team == Team.Center))){
			bool hasGhost = devices.Exists(x => x.input.team == Team.Ghosts);
			bool hasSurvivor = devices.Exists(x => x.input.team == Team.Survivors);

			if ((hasGhost && hasSurvivor) && !countdownPanel.isEnabled || DEBUGGING){
				StartCoroutine(DoCountdown());
			}
		}


//		if (Input.GetButton("Submit")){
//			bool hasGhost = devices.Exists(x => x.input.team == Team.Ghosts);
//			bool hasSurvivor = devices.Exists(x => x.input.team == Team.Survivors);
//
//			if ((hasGhost && hasSurvivor) || DEBUGGING){
//				Manager.I.StartGame(devices.Where(x => x.input.team != Team.Center).Select(x => x.input).ToList());
//			}
//		}

	}


	public void OnPlayClicked(){
		SoundManager.I.PlaySound("Bonecrack_01", null);

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

		SoundManager.I.PlaySound("Bonecrack_01", null);
			
		var newPos = device.GetComponent<RectTransform>().anchoredPosition;
		newPos.x = 0;
		newPos.y = -100f - device.input.idx * 80f;
		device.GetComponent<RectTransform>().anchoredPosition = newPos;
	}


	private IEnumerator DoCountdown(){
		countdownPanel.SetEnabled(true);
		int count = countdownStart;
		while (count > 0 && devices.All(x => (x.isReady || x.input.team == Team.Center))){
			countdownText.text = "Starting in " + count + "...";
			yield return new WaitForSeconds(countdownDuration);
			count--;
		}

		if (!devices.All(x => (x.isReady || x.input.team == Team.Center))){
			countdownPanel.SetEnabled(false);
			yield break;
		}

		Manager.I.StartGame(devices.Where(x => x.input.team != Team.Center).Select(x => x.input).ToList());
	}
}


public enum InputType{
	Keyboard,
	Joystick
}

