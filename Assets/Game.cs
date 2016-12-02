using UnityEngine;
using System.Collections;
using System.Runtime.ConstrainedExecution;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	[SerializeField] GameObject survivorPrefab;
	[SerializeField] GameObject ghostPrefab;

	[SerializeField] Transform survivorStartPos;
	[SerializeField] Transform ghostStartPos;

	[SerializeField] List<PlayerStatPanel> playerPanels;

	List<Survivor> survivors = new List<Survivor>();
	List<Ghost> ghosts = new List<Ghost>();

	public List<Survivor> Survivor {get {return survivors;}}
	public List<Ghost> Ghosts {get {return ghosts;}}

	public static Game I;


	void Awake(){
		I = this;
	}

	void Start () {
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


}
