using UnityEngine;
using System.Collections;

public class KeyStartPos : MonoBehaviour {

	public int doorIdx;

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(transform.position, 0.5f);
	}
}
