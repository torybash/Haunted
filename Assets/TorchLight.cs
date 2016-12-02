using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class TorchLight : MonoBehaviour {


	private Light lght {get{ return GetComponent<Light>(); } }

	[SerializeField] private float strength;
	[SerializeField] private float variability;
	[SerializeField] private float speed;

	void Update () {
		var newIntense = strength + Random.Range(-variability, variability);

		lght.intensity	= Mathf.MoveTowards(lght.intensity, newIntense, speed) ;	
	}
}
