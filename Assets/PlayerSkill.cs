using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerSkill : MonoBehaviour {


	[SerializeField] Image skillImg;
	[SerializeField] Image cooldownImg;


	public void Init(Sprite skillSprite){
		skillImg.sprite = skillSprite;
		cooldownImg.fillAmount = 0;
	}

	public void UseSkill(float cooldown){
		cooldownImg.fillAmount = 1;
		StartCoroutine(DoCooldown(cooldown));
	}

	private IEnumerator DoCooldown(float cooldown){
		float endTime = Time.time + cooldown;
		while (Time.time < endTime){
			float t = (endTime - Time.time) / cooldown; //1 --> 0
			t = Mathf.Clamp01(t);
			cooldownImg.fillAmount = t;
			yield return null;
		}
		cooldownImg.fillAmount = 0f;
	}
}
