using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using BeyondTheLanguage;

public class Back : MonoBehaviour {
	
	private Button backButton;
	public InputField input;
	// Use this for initialization
	void Start () 
	{
		backButton = GetComponent<Button> ();
		backButton.onClick.AddListener(delegate{goBack();});
	}

	void goBack() 
	{
		Util.DestroyGameObjectsWithTag ("temp", 1f);
		RandomShapes.go = true;

		CanvasGroup inputCanvas = GameObject.Find("InputCanvas").GetComponent<CanvasGroup> ();
		CanvasGroup resultCanvas = GameObject.Find ("ResultCanvas").GetComponent<CanvasGroup> ();
		GameObject.FindGameObjectWithTag ("introPolygon").GetComponent<Renderer>().enabled = true;

		StartCoroutine(Util.FadeIn(inputCanvas));
		StartCoroutine(Util.FadeOut(resultCanvas));

		GameObject[] samples = GameObject.FindGameObjectsWithTag ("sample");

		int k = 0;
		foreach (GameObject sample in samples) {
			StartCoroutine(scaleAnimation(sample, sample.transform.localScale, Text.samplesSize[k++]));
		}

		//GameObject.Find ("SubmitButton").GetComponent<Text> ().reloadGallery ();

		GameObject[] gallerys = GameObject.FindGameObjectsWithTag ("gallery");

		foreach (GameObject gallery in gallerys) {
			StartCoroutine(scaleAnimation(gallery, gallery.transform.localScale, new Vector3(50f, 50f, 50f)));
		}

		GameObject go = GameObject.Find ("InputDummy");
		Camera.main.GetComponent<MouseCamera> ().target = go.transform;

		input.Select ();
		input.ActivateInputField ();
	}

	private IEnumerator scaleAnimation(GameObject go, Vector3 startScale, Vector3 targetScale){
		float elapsedTime = 0.0f;
		while (elapsedTime < 1.0f)
		{
			go.transform.localScale = Vector3.Slerp(startScale, targetScale, elapsedTime / 1.0f);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}

}
