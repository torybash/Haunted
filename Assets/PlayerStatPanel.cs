using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerStatPanel : MonoBehaviour {

	[Header("References")]
	[SerializeField] Image portraitImg;
	[SerializeField] List<PlayerSkill> skills;

	[Header("Values")]
	[SerializeField] Sprite survSprite;
	[SerializeField] List<Sprite> survSkillSprites;

	[SerializeField] Sprite ghostSprite;
	[SerializeField] List<Sprite> ghostSkillSprites;


	public void Init(InputData input){
		if (input.team == Team.Survivors){
			portraitImg.sprite = survSprite;
			for (int i = 0; i < survSkillSprites.Count; i++) {
				skills[i].Init(survSkillSprites[i]);
			}
		}else if (input.team == Team.Ghosts){
			portraitImg.sprite = ghostSprite;
			for (int i = 0; i < ghostSkillSprites.Count; i++) {
				skills[i].Init(ghostSkillSprites[i]);
			}
		}
	}


	public void UseSkill(int idx, float cooldown){
		skills[idx].UseSkill(cooldown);
	}

	public void Hurt(){
		//TODO
	}
}
