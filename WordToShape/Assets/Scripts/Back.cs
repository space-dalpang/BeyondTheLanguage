using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Back : MonoBehaviour {
	
	private Button backButton;

	// Use this for initialization
	void Start () 
	{
		backButton = GetComponent<Button> ();
		backButton.onClick.AddListener(delegate{goBack();});
		backButton.GetComponent<CanvasRenderer> ().SetAlpha (0);
	}

	void goBack() 
	{
		CanvasGroup cg = GameObject.Find ("InputCanvas").GetComponent<CanvasGroup> ();
		StartCoroutine(FadeIn(cg));
	}

	IEnumerator FadeIn(CanvasGroup canvasGroup)
	{
		float time = 1f;
		while(canvasGroup.alpha < 1f)
		{
			canvasGroup.alpha += Time.deltaTime / time;
			yield return null;
		}
	}

}
