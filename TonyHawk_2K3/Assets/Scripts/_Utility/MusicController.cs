using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {

	public AudioClip[] clips;
	private int clipIndex;
	
	public float maxVolume;
	public bool autoPlay = false;

	public bool autoCont = false;
	private bool hasPlayed = false;


	// Use this for initialization
	void Start() {
		// Add an audio source component
		if (gameObject.GetComponent<AudioSource>() == null) {
			gameObject.AddComponent<AudioSource>();

			audio.loop = false;
			audio.volume = 0.0f;
		}

		// Select a random clip
		clipIndex = Random.Range(0, clips.Length);
		audio.clip = clips[clipIndex];

		// Check if the music should be played
		if (autoPlay) Play(maxVolume);
	}

	void Update() {
		if (!audio.isPlaying && hasPlayed && autoCont) {
			// Select a random clip
			clipIndex = Random.Range(0, clips.Length);
			audio.clip = clips[clipIndex];

			Play(maxVolume);
		}
	}

	public void Play(float volume) {
		hasPlayed = true;

		audio.volume = volume;
		audio.Play ();
	}

	public void Stop() {
		audio.volume = 0.0f;
		audio.Stop ();
	}

	public void Pause() {
		audio.Pause();
	}

	public IEnumerator FadeMusicIn(float fadeTime) {
		// Stop volume fading coroutines
		StopCoroutine("FadeMusicOut");
		Play(0.0f);

		float timer = 0.0f;

		// Fade in the volume
		while (timer < fadeTime) {
			timer += Time.deltaTime;
			audio.volume = Mathf.Lerp(0.0f, maxVolume, timer / fadeTime);

			yield return null;
		}

		// Ensure the volume is maximized
		audio.volume = maxVolume;
	}

	public IEnumerator FadeMusicOut(float fadeTime) {
		// Stop volume fading coroutines
		StopCoroutine("FadeMusicIn");

		float startVol = audio.volume;
		float timer = 0.0f;

		// Fade out the volume
		while (timer < fadeTime) {
			timer += Time.deltaTime;
			audio.volume = Mathf.Lerp (startVol, 0.0f, timer / fadeTime);

			yield return null;
		}

		// Stop the audio source
		audio.Stop();
	}
}
