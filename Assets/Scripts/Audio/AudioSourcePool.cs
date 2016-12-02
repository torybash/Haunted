using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioSourcePool {

	private Queue<AudioSource> sources;

	private Transform audioSourcePool;

	public AudioSourcePool(int size){
		sources = new Queue<AudioSource>(size);
		audioSourcePool = new GameObject("_AudioSourcePool").transform;
		for (int i = 0; i < size; i++) {
			var sourceContainer = new GameObject("_AudioSource_" + i);
			sourceContainer.transform.SetParent(audioSourcePool);
			var source = sourceContainer.AddComponent<AudioSource>();
			sources.Enqueue(source);
		}
	}

	public AudioSource Get(){
		if (sources.Count == 0) {
			Debug.LogError("No audio sources available! Can not play any sound.");
			return null;
		}
		return sources.Dequeue();
	}

	public void Return(AudioSource source){
		source.transform.SetParent(audioSourcePool.transform);
		sources.Enqueue(source);
	}
}
