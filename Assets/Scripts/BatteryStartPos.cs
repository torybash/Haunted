using UnityEngine;
using System.Collections;

public class BatteryStartPos : MonoBehaviour {

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(transform.position, 0.5f);
	}
}
