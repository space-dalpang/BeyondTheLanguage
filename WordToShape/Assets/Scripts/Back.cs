using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using BeyondTheLanguage;

public class Back : MonoBehaviour {
	
	private Button backButton;

	// Use this for initialization
	void Start () 
	{
		backButton = GetComponent<Button> ();
		backButton.onClick.AddListener(delegate{goBack();});
	}

	void goBack() 
	{
		CanvasGroup inputCanvas = GameObject.Find("InputCanvas").GetComponent<CanvasGroup> ();
		CanvasGroup layoutCanvas = GameObject.Find ("LayoutCanvas").GetComponent<CanvasGroup> ();

		StartCoroutine(Util.FadeIn(inputCanvas));
		StartCoroutine(Util.FadeOut(layoutCanvas));
		GameObject go = GameObject.Find ("InputDummy");
		Camera.main.GetComponent<MouseCamera> ().target = go.transform;
	}

}
