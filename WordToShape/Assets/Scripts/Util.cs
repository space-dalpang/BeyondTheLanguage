using UnityEngine;
using System.Collections;


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
}
