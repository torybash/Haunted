using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Light))]
public class TorchLight : MonoBehaviour {



	[SerializeField] Light lght;

	[SerializeField] private float strength;
	[SerializeField] private float variability;
	[SerializeField] private float speed;

	[SerializeField] private float onIntensity;
	[SerializeField] private float offIntensity;

	[SerializeField] private bool startTurnedOn = true;

	public bool turnedOn {get; private set;}


	void Start(){
		SetTurnedOn(startTurnedOn);
	}

	void Update () {
		if (turnedOn){
			var newIntense = strength + Random.Range(-variability, variability);
			lght.intensity	= Mathf.MoveTowards(lght.intensity, newIntense, speed) ;	
		}
	}


	public void SetTurnedOn(bool on){
		turnedOn = on;
		lght.intensity = on ? onIntensity : offIntensity;
	}


}
