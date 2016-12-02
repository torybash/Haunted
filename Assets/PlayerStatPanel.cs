﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerStatPanel : MonoBehaviour {

	[Header("References")]
	[SerializeField] Image portraitImg;
//	[SerializeField] List<PlayerSkill> skills;
	[SerializeField] Image energyBarFill;



//	[SerializeField, Range(0f,1f)] float DBGfillAmount;

	private Color fullColor;
	private Color emptyColor;


	[Header("Values")]
	[SerializeField] Sprite survSprite;
//	[SerializeField] List<Sprite> survSkillSprites;
//
	[SerializeField] Sprite ghostSprite;
//	[SerializeField] List<Sprite> ghostSkillSprites;

	[SerializeField] Color survFullColor;
	[SerializeField] Color survEmptyColor;

	[SerializeField] Color ghostFullColor;
	[SerializeField] Color ghostEmptyColor;


	void OnValidate(){
//		energyBarFill.fillAmount = DBGfillAmount;
//		energyBarFill.color = Color.Lerp(emptyColor, fullColor, energyBarFill.fillAmount);
	}

	public void Init(InputData input){

		energyBarFill.fillAmount = 1f;

		if (input.team == Team.Survivors){
			portraitImg.sprite = survSprite;
			fullColor = survFullColor;
			emptyColor = survEmptyColor;
//			for (int i = 0; i < survSkillSprites.Count; i++) {
//				skills[i].Init(survSkillSprites[i]);
//			}
		}else if (input.team == Team.Ghosts){
			portraitImg.sprite = ghostSprite;
			fullColor = ghostFullColor;
			emptyColor = ghostEmptyColor;
//			for (int i = 0; i < ghostSkillSprites.Count; i++) {
//				skills[i].Init(ghostSkillSprites[i]);
//			}
		}
	}

	public void SetEnergy(float newAmount){
		energyBarFill.fillAmount = newAmount;
		//		skills[idx].UseSkill(cooldown);
	}

	public void UseSkill(int idx, float cooldown){
//		skills[idx].UseSkill(cooldown);
	}

	public void Hurt(){
		//TODO
	}


	public void SetAlpha(float alpha){
		GetComponent<CanvasGroup>().alpha = alpha;
	}
}
