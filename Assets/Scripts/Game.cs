using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Xml.Linq;

public class Game : MonoBehaviour {

	[SerializeField] GameObject survivorPrefab;
	[SerializeField] GameObject ghostPrefab;
	[SerializeField] Battery batteryPrefab;
	[SerializeField] Key keyPrefab;

	[SerializeField] Transform survivorStartPos;
	[SerializeField] Transform ghostStartPos;

	[SerializeField] List<PlayerStatPanel> playerPanels;


	[SerializeField] int batteryCount = 3;

	[SerializeField] UIPanel endPanel;
	[SerializeField] Text endText;

	[SerializeField] Color survivorColor;
	[SerializeField] Color ghostColor;

	List<Survivor> survivors = new List<Survivor>();
	List<Ghost> ghosts = new List<Ghost>();

	public List<Survivor> Survivor {get {return survivors;}}
	public List<Ghost> Ghosts {get {return ghosts;}}

	private bool ended;

	public static Game I;


	void Awake(){
		I = this;
	}

	void Start () {
		var batteryStartPositions = GameObject.FindObjectsOfType<BatteryStartPos>().OrderBy(a => Random.Range(0, int.MaxValue)).Take(batteryCount).ToList();
		foreach (var batteryPos in batteryStartPositions) {
			var battery = Instantiate(batteryPrefab);
			battery.transform.position = batteryPos.transform.position;
		}


		var keyStartPositions = GameObject.FindObjectsOfType<KeyStartPos>();
		var toUseDoorIdx = Random.Range(0, keyStartPositions.Max(x => x.doorIdx) + 1);

		var keyPos = keyStartPositions.Where(x => x.doorIdx == toUseDoorIdx).OrderBy(a => Random.Range(0, int.MaxValue)).FirstOrDefault();

		var key = Instantiate(keyPrefab);
		key.transform.position = keyPos.transform.position;
		key.doorIdx = toUseDoorIdx;

//		foreach (var batteryPos in batteryStartPositions) {
//			var battery = Instantiate(batteryPrefab);
//			battery.transform.position = batteryPos.transform.position;
//		}

		foreach (var playerPanel in playerPanels) {
			playerPanel.GetComponent<UIPanel>().SetEnabled(false);
		}

		if (Manager.HasInstance()){
			foreach (var input in Manager.I.inputs) {
				CreateCharacter(input);
			}
		}else{ //DEBUGGING!
			var input = new InputData{type = InputType.Keyboard, idx = 0, team = Team.Ghosts};
			CreateCharacter(input);

			var input2 = new InputData{type = InputType.Keyboard, idx = 1, team = Team.Survivors};
			CreateCharacter(input2);
		}

		SoundManager.I.PlaySound("Gate_Open_00", null);

//		SoundManager.I.PlaySound("Ambience_MonstersBelly_00", null, true);
	}


	void Update(){
		if (survivors.All(x => x.dead)){
			StartCoroutine(DoEndGame(Team.Ghosts));
		}
	}

	public void SurvivorsWin(){
		StartCoroutine(DoEndGame(Team.Survivors));
	}

	public void PickedUpKey(int doorIdx){
		var doors = GameObject.FindObjectsOfType<ExitDoor>();
		doors.Where(x => x.doorIdx == doorIdx).FirstOrDefault().Activate();
	}


	private void CreateCharacter(InputData input){
		var playerPanel = playerPanels[input.idx];
		playerPanel.GetComponent<UIPanel>().SetEnabled(true);
		playerPanel.Init(input);

		GameObject go = null;
		if (input.team == Team.Ghosts){
			go = Instantiate(ghostPrefab);
			go.transform.position = ghostStartPos.position;
			ghosts.Add(go.GetComponent<Ghost>());
			go.GetComponent<Ghost>().input = input;
			go.GetComponent<Ghost>().panel = playerPanel;
		}else if (input.team == Team.Survivors){
			go = Instantiate(survivorPrefab);
			go.transform.position = survivorStartPos.position;
			survivors.Add(go.GetComponent<Survivor>());
			go.GetComponent<Survivor>().input = input;
			go.GetComponent<Survivor>().panel = playerPanel;
		}

		if (go != null){
			var playerMove = go.GetComponent<PlayerMovement>();
			playerMove.Init(input);
		}
	}



	private IEnumerator DoEndGame(Team winnerTeam){
		if (ended) yield break ;

		ended = true;
		switch (winnerTeam) {
		case Team.Ghosts:
			endText.text = "GHOSTS WIN!";
			endText.color = ghostColor;
			SoundManager.I.PlaySound("Jingle_Achievement_00", null);
			break;
		case Team.Survivors:
			endText.text = "SURVIVORS WIN!";
			endText.color = survivorColor;
			SoundManager.I.PlaySound("Jingle_Lose_00", null);

			break;
		}

		float fadeDuration = 0.5f;
		float endTime = Time.time + fadeDuration;
		endPanel.SetEnabled(true);
		endPanel.GetComponent<CanvasGroup>().alpha = 0f;
		while (Time.time < endTime){
			float t = 1 - (endTime- Time.time) / fadeDuration;
			endPanel.GetComponent<CanvasGroup>().alpha = t;
			yield return null;
		}

		yield return new WaitForSeconds(0.25f);

		if (winnerTeam == Team.Ghosts){
			SoundManager.I.PlaySound("Laugh_Evil_02", null);
		}

		yield return new WaitForSeconds(5f);

		SceneManager.LoadScene("Menu");
	}
}
