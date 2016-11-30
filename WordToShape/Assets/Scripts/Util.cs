using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


public static class Util {
	
	public static IEnumerator FadeOut(CanvasGroup canvasGroup)
	{
		float time = 1f;
		Text.userInputLock = true;
		while(canvasGroup.alpha > 0)
		{
			canvasGroup.alpha -= Time.deltaTime / time;
			yield return null;
		}
		canvasGroup.alpha = 0;
		Text.userInputLock = false;
		yield return null;
	}

	public static IEnumerator FadeIn(CanvasGroup canvasGroup)
	{
		float time = 1f;
		Text.userInputLock = true;
		while(canvasGroup.alpha < 1f)
		{
			canvasGroup.alpha += Time.deltaTime / time;
			yield return null;
		}
		canvasGroup.alpha = 1f;
		Text.userInputLock = false;
		yield return null;
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

	public static void ChangeLayerRecursively(Transform trans, string name){
		foreach (Transform child in trans) {
			child.gameObject.layer = LayerMask.NameToLayer(name);
			ChangeLayerRecursively (child, name);
		}
	}

	public static string FormatString(string text){
		StringBuilder sb = new StringBuilder ();
		int len = 0;
		for (int i = 0; i < text.Length; i++) {
			sb.Append (text [i]);
			if (len > 20 && text [i] == ' ') {
				sb.Append ('\n');
				len = 0;
			}
			len++;
		}
		string formatted = sb.ToString ();
		return formatted;
	}

}
