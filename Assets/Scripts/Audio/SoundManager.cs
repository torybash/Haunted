using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

	[SerializeField] SoundLibrary soundLib;

	private Dictionary<string, AudioClip> soundDict = new Dictionary<string, AudioClip>();

	private AudioSourcePool audioSourcePool;

	private const float audioSourceExtraTime = 0.1f;

	public static SoundManager I;


	void Awake(){
		I = this;

		audioSourcePool = new AudioSourcePool(64);

		foreach (var snd in soundLib.sounds) {
			soundDict.Add(snd.name, snd);
		}
	}

	public AudioSource PlaySound(string sndName, Transform parent, bool loop = false){
		var source = audioSourcePool.Get();
		var clip = soundDict[sndName];

		if (source != null && clip != null){
			if (parent) source.transform.SetParent(parent);
			source.transform.localPosition = Vector3.zero;

			source.loop = loop;
			source.clip = clip;
			source.Play();

			StartCoroutine(AudioSourceReturnCR(source));

			return source;
		}

		return null;
	}


	private IEnumerator AudioSourceReturnCR(AudioSource source){
//		yield return new WaitForSeconds(source.clip.length + audioSourceExtraTime);


		while (source.isPlaying){
			yield return null;
		}

//		if (source.isPlaying){
//			Debug.LogError("AudioSourceReturnCR ERROR - audioSource is still playing! - source.time: " +source.time + ", source.clip: " + source.clip.length);
//			yield break;
//		}

		audioSourcePool.Return(source);
	}

	#region DBG
//	void OnGUI(){
//		GUILayout.Space(100);
//		if (GUILayout.Button("PLAY Click_0!")){
//			SoundManager.Instance.PlaySound(SoundType.Click_0);
//		}
//		if (GUILayout.Button("PLAY Whoosh_0!")){
//			SoundManager.Instance.PlaySound(SoundType.Whoosh_0);
//		}
//
//		if (GUILayout.RepeatButton("SPAM Whoosh_0!")){
//			SoundManager.Instance.PlaySound(SoundType.Whoosh_0);
//		}
//	}
	#endregion DBG
}
