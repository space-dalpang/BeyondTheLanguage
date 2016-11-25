using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public static class Util {
	
	public static IEnumerator FadeOut(CanvasGroup canvasGroup)
	{
		float time = 1f;
		while(canvasGroup.alpha > 0)
		{
			canvasGroup.alpha -= Time.deltaTime / time;
			yield return null;
		}
	}

	public static IEnumerator FadeIn(CanvasGroup canvasGroup)
	{
		float time = 1f;
		while(canvasGroup.alpha < 1f)
		{
			canvasGroup.alpha += Time.deltaTime / time;
			yield return null;
		}
	}

	public static Object[] getPreFabs(){
		return UnityEngine.Resources.LoadAll ("");
	}

	public static void DestroyGameObjectsWithTag(string tag, float time)
	{
		GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);
		foreach (GameObject target in gameObjects) {
			GameObject.Destroy(target, time);
		}
	}


}
