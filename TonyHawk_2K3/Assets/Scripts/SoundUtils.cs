using UnityEngine;
using System.Collections;

/**
 * Utility class for playing sounds on a GameObject in Unity.
 * 
 * Allows for multiple sounds to be played  on the same obejct at the same time,
 * instead of causing the previously started sound to stop.
 * 
 * @author - Tyler Hadidon (hadidotj)
 * @date - 08-09-2014
 */
[System.Serializable]
public class SoundUtils {

	/**
	 * Plays a unique sound on the given game object. It does not stop other sounds that may be playing.
	 * @param gameObject - The GameObject to play the sound on
	 * @param clip - The audio clip to play
	 * @param volume - The volume to play at
	 */
	public static void playSound(GameObject gameObject, AudioClip clip, float volume = 1.0f) {
		playSoundStartTime(gameObject, clip, 0.0f, volume);
	}

	/**
	 * Plays a unique sound on the given game object starting at the given startTime. It does not stop other
	 * sounds that may be playing.
	 * @param gameObject - The GameObject to play the sound on
	 * @param clip - The audio clip to play
	 * @param startTime - The time to start the clip at (between 0 and clip.length())
	 * @param volume - The volume to play at
	 */
	public static void playSoundStartTime(GameObject gameObject, AudioClip clip, float startTime = 0, float volume = 1.0f) {
		AudioSource src = gameObject.AddComponent<AudioSource> () as AudioSource;
		src.clip = clip;
		src.volume = volume;
		src.rolloffMode = AudioRolloffMode.Linear;
		src.minDistance = 1.0f;
		src.maxDistance = 20.0f;
		src.Play();
		if(startTime != 0) {
			src.time = startTime;
		}
		GameObject.Destroy (src, clip.length);
	}

	/**
	 * Stops all other sounds and plays the given sound clip.
	 * @param gameObject - The GameObject to stop all sounds on and play the given sound on
	 * @param clip - The audio clip to play
	 * @param volume - The volume to play at
	 */
	public static void playSingleSound(GameObject gameObject, AudioClip clip, float volume = 1.0f) {
		stopAll (gameObject);
		playSound(gameObject, clip, volume);
	}

	/**
	 * Loops a sound on the given game object. It does not stop other sounds that may be playing.
	 * @param gameObject - The GameObject to play the sound on
	 * @param clip - The audio clip to loop
	 * @param volume - The volume to play at
	 */
	public static void loopSound(GameObject gameObject, AudioClip clip, float volume = 1.0f) {
		AudioSource src = gameObject.AddComponent<AudioSource> () as AudioSource;
		src.clip = clip;
		src.loop = true;
		src.volume = volume;
		src.Play();
	}

	/**
	 * Given a GameObject, it will stop all sounds with the given AudioClip.
	 * @param gameObject - The GameObject to stop the sounds on
	 * @param clip - The AudioClip to search for
	 */
	public static void stopAllForClip(GameObject gameObject, AudioClip clip) {
		AudioSource[] srcs = gameObject.GetComponents<AudioSource> () as AudioSource[];
		foreach(AudioSource src in srcs) {
			if( src.clip == clip) {
				GameObject.Destroy(src);
			}
		}
	}

	/**
	 * Stops all the sounds on the given GameObject by deleting all AudioSource components.
	 * @param gameObject - The GameObject to stop all sounds on.
	 */
	public static void stopAll(GameObject gameObject) {
		AudioSource[] srcs = gameObject.GetComponents<AudioSource> () as AudioSource[];
		foreach(AudioSource src in srcs) {
			GameObject.Destroy(src);
		}
	}

	/**
	 * Given a GameObject, determines if it is currently playing a sound.
	 * @param gameObject - The GameObject to check to see if it is playing a sound
	 * @return True if the GameObject has an AudioSource that is playing
	 */
	public static bool isPlaying(GameObject gameObject) {
		AudioSource[] srcs = gameObject.GetComponents<AudioSource> () as AudioSource[];
		foreach(AudioSource src in srcs) {
			if(src.isPlaying) {
				return true;
			}
		}
		return false;
	}

	/**
	 * @{see #isPlaying}
	 */
	public static bool isNotPlaying(GameObject gameObject) {
		return !isPlaying(gameObject);
	}

	/**
	 * @{see #isPlaying}
	 */
	public static bool isSilent(GameObject gameObject) {
		return isNotPlaying(gameObject);
	}

	/**
	 * Given a GameObject, determines if it is currently playing a given AudioClip.
	 * @param gameObject - The GameObject to check to see if it is playing a sound
	 * @return True if the GameObject has an AudioSource that is playing
	 */
	public static bool isPlayingClip(GameObject gameObject, AudioClip clip) {
		AudioSource[] srcs = gameObject.GetComponents<AudioSource> () as AudioSource[];
		foreach(AudioSource src in srcs) {
			if(src.clip == clip && src.isPlaying) {
				return true;
			}
		}
		return false;
	}

	/**
	 * @{see #isPlayingClip}
	 */
	public static bool isNotPlayingClip(GameObject gameObject, AudioClip clip) {
		return !isPlayingClip(gameObject, clip);
	}
}