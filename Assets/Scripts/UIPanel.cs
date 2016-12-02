using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanel : MonoBehaviour {

	private CanvasGroup grp { get {return GetComponent<CanvasGroup>(); } }
		
	public bool isEnabled;
//	public bool isEnabled {get {return _isEnabled;} set {SetEnabled(value); } }


	void OnValidate(){
		SetEnabled(isEnabled);
	}

	public void SetEnabled(bool enabled){
		isEnabled = enabled;

		grp.alpha = isEnabled ? 1 : 0;
		grp.blocksRaycasts = isEnabled;
		grp.interactable = isEnabled;
	}
}
