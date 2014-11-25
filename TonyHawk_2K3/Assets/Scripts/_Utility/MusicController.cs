using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {

	public AudioClip[] clips;
	private int clipIndex;

	public float maxVolume;


	// Use this for initialization
	void Start () {
		// Choose a random clip index
		clipIndex = Random.Range(0, clips.Length);
	}

	public IEnumerator FadeMusicIn(float fadeTime) {
		// Make sure the audio is not playing
		if (SoundUtils.isNotPlayingClip(gameObject, clips[clipIndex])) {
			StopCoroutine("FadeMusicOut");

			SoundUtils.loopSound(gameObject, clips[clipIndex], 0.0f);
			float volume = audio.volume;
			float timer = 0.0f;

			// Fade in the volume
			while (timer < fadeTime) {
				timer += Time.deltaTime;
				audio.volume = Mathf.Lerp(volume, maxVolume, timer / (fadeTime - Time.deltaTime));

				yield return null;
			}

			audio.volume = maxVolume;
		}
	}

	public IEnumerator FadeMusicOut(float fadeTime) {
		// Make sure the audio is playing
		if (SoundUtils.isPlayingClip(gameObject, clips[clipIndex])) {
			StopCoroutine("FadeMusicIn");

			float volume = audio.volume;
			float timer = 0.0f;

			// Fade out the volume
			while (timer < fadeTime) {
				timer += Time.deltaTime;
				audio.volume = Mathf.Lerp (volume, 0.0f, timer / (fadeTime - Time.deltaTime));

				yield return null;
			}

			audio.volume = 0.0f;
			audio.Stop();
		}
	}
}
