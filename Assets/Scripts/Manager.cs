using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Manager : MonoBehaviour {

	public static Manager I;

	public List<InputData> inputs;

	public static bool HasInstance(){
		return I != null;
	}

	void Awake(){
		I = this;
		DontDestroyOnLoad(gameObject);
	
		SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) => {
			
		};
	}

	

	public void StartGame(List<InputData> inputs){
		this.inputs = inputs;
		SceneManager.LoadScene("Scene");

	}


	
}



public enum Team{
	Survivors,
	Center,
	Ghosts
}


public enum ItemType{
	None,
	Key,
	TrapDoor, //TODO?
}