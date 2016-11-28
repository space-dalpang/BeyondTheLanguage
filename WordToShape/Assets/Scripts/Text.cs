using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using BeyondTheLanguage;
using MeshGen;

public class Text : MonoBehaviour
{
	[Range(0F,10F)]
	public float scale;
	public InputField mainInputField;
	public Material[] sentiMaterials;
	private Button runButton = null;
	private Api api = null;
	private GameObject[] samples;
	static public List<Vector3> samplesSize = new List<Vector3>();

	void Start ()
	{
		runButton = GetComponent<Button> ();
		api = GetComponent<Api> ();

		runButton.onClick.AddListener(delegate{ValueChangeCheck(mainInputField);});

		Polygon polygon = CreateSphericalPolygon(mainInputField.text.GetHashCode() % 16 + 9, mainInputField.text.Length % 5 + 1);
		polygon.gameObject.transform.position = new Vector3(0, 2.4f, -200);

		samples = GameObject.FindGameObjectsWithTag ("sample");

		foreach (GameObject sample in samples) {
			samplesSize.Add (sample.transform.localScale);
		}

		mainInputField.Select ();
		mainInputField.ActivateInputField ();
	}

	public void ValueChangeCheck (InputField input)
	{
		api.GetSenti (input.text, OnGetSenti, OnErrorSenti);

		CanvasGroup inputCanvas = GameObject.Find("InputCanvas").GetComponent<CanvasGroup> ();
		CanvasGroup resultCanvas = GameObject.Find ("ResultCanvas").GetComponent<CanvasGroup> ();

		StartCoroutine(Util.FadeOut(inputCanvas));
		StartCoroutine(Util.FadeIn(resultCanvas));

		foreach (GameObject sample in samples){
			StartCoroutine (scaleAnimation (sample, sample.transform.localScale, Vector3.zero));
		}

		GameObject go = GameObject.Find ("Ground");

		Transform target = new GameObject ("genDummy").transform;
		target.position = go.transform.position + new Vector3(0, 8, 0);

		Camera.main.GetComponent<MouseCamera> ().target = target;

		UnityEngine.UI.Text txt = GameObject.Find ("ResultText").GetComponent<UnityEngine.UI.Text>();
		txt.text = input.text;

		input.text = "";
	}

	Polygon CreateSphericalPolygon(int numVertices = 15, float radius = 1f) {
		var vertices = new List<Vector3>();
		var center = new Vector3(0, 0, 0);
		for (int i = 0; numVertices > i; i++) {
			vertices.Add(UnityEngine.Random.onUnitSphere * radius + center);
		}
		return Factory.Create("RandomShape", vertices);

	}

	private void OnGetSenti (JSONObject result)
	{
		StartCoroutine (GenWordObject (result));
	}

	private void OnErrorSenti (string error)
	{
		// todo
	}

	void Update(){
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

	private IEnumerator GenWordObject (JSONObject result)
	{
		Util.DestroyGameObjectsWithTag ("temp", 1f);
		RandomShapes.go = false;

		JSONObject words = result.GetField ("words");
		if (words.list.Count == 0) {
			Polygon polygon = CreateSphericalPolygon(mainInputField.text.GetHashCode() % 16 + 9, mainInputField.text.Length % 5 + 1);
			polygon.gameObject.transform.position = new Vector3(0, 8, 0) + UnityEngine.Random.insideUnitSphere;
			polygon.gameObject.tag = "temp";
		}

		foreach (JSONObject word in words.list) {
			string name = Regex.Unescape (word.GetField ("name").str);
			string category = Regex.Unescape (word.GetField ("category").str);
			int level = (int)word.GetField ("level").n;
			//Debug.Log ("word: " + name + " " + category + " " + level);
			GameObject go = GenSentiObject (name, category, level);
			RandomTransformGameObject (go, level, name.GetHashCode ());

			Vector3 targetScale = new Vector3();
			targetScale = go.transform.localScale;
			go.transform.localScale = Vector3.zero;
			StartCoroutine (scaleAnimation (go, Vector3.zero, targetScale));

			yield return new WaitForSeconds (0.5f);
		}
	}


	private GameObject GenSentiObject (string name, string category, int level)
	{	
		GameObject go = null;

		Material targetMaterial;

		switch (category) {
		case "기쁨":
			go = UnityEngine.Resources.Load ("senti-mesh-pleasure-" + Random.Range (1, 3)) as GameObject;
			targetMaterial = sentiMaterials[0];
			break;
		case "흥미":
			go = UnityEngine.Resources.Load ("senti-mesh-interest-" + Random.Range(1, 1)) as GameObject;
			targetMaterial = sentiMaterials[1];
			break;
		case "놀람":
			go = UnityEngine.Resources.Load ("senti-mesh-surprised-" + Random.Range(1, 1)) as GameObject;
			targetMaterial = sentiMaterials[2];
			break;
		case "공포":
			go = UnityEngine.Resources.Load ("senti-mesh-fear-" + Random.Range (1, 3)) as GameObject;
			targetMaterial = sentiMaterials[3];
			break;
		case "지루함":
			go = UnityEngine.Resources.Load ("senti-mesh-boredom-" + Random.Range(1, 3)) as GameObject;
			targetMaterial = sentiMaterials[4];
			break;
		case "분노":
			go = UnityEngine.Resources.Load ("senti-mesh-anger-" + Random.Range(1, 1)) as GameObject;
			targetMaterial = sentiMaterials[5];
			break;
		case "혐오":
			go = UnityEngine.Resources.Load ("senti-mesh-hate-" + Random.Range(1, 3)) as GameObject;
			targetMaterial = sentiMaterials[6];
			break;
		case "통증":
			go = UnityEngine.Resources.Load ("senti-mesh-pain-" + Random.Range (1, 3)) as GameObject;
			targetMaterial = sentiMaterials[7];
			break;
		case "중성":
			go = UnityEngine.Resources.Load ("senti-mesh-medium-" + Random.Range(1, 3)) as GameObject;
			targetMaterial = sentiMaterials[8];
			break;
		case "슬픔":
			go = UnityEngine.Resources.Load ("senti-mesh-sad-" + Random.Range(1, 3)) as GameObject;
			targetMaterial = sentiMaterials[9];
			break;
		default:
			Polygon polygon = CreateSphericalPolygon (name.GetHashCode () % 16 + 9, name.Length % 5 + 1).Rigidize ();
			go = polygon.gameObject;
			go.tag = "temp";
			targetMaterial = go.GetComponent<Material> ();
			targetMaterial.color = Random.ColorHSV (0f, 1f, 1f, 1f, 0.5f, 1f);
			break;
		}

		GameObject newObj;

		//GameObject newObj = Instantiate (go);
		newObj = new GameObject ("SentiObject");

		MeshFilter meshFilter = newObj.AddComponent<MeshFilter> ();
		meshFilter.mesh = go.GetComponentInChildren<MeshFilter> ().sharedMesh;

		Mesh mesh = meshFilter.mesh;
		mesh.Optimize ();
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();

		MeshRenderer meshRenderer = newObj.AddComponent<MeshRenderer> ();
		meshRenderer.material = targetMaterial;
		//meshRenderer.material = go.GetComponentInChildren<MeshRenderer> ().sharedMaterial;
		//meshRenderer.material.color = color;

//		Rigidbody rigid = newObj.AddComponent<Rigidbody> ();
//		rigid.mass = 5;
//		rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

		MeshCollider collider = newObj.AddComponent<MeshCollider> ();
		collider.convex = true;

		newObj.tag = "temp";

		// Destroy (go);
		return newObj;
	}

	private void RandomTransformGameObject (GameObject go, int level, int seed)
	{
		System.Random random = new System.Random (seed);

		//go.transform.position = new Vector3 (random.Next (4) - 2, random.Next (3) + 5, random.Next (4) - 2);
		go.transform.position = new Vector3(0, 8, 0) + Random.insideUnitSphere;
		go.transform.rotation = Random.rotation;

		go.transform.localScale = new Vector3 (
			(random.Next (10) / 10F + level/3) * scale,
			(random.Next (10) / 10F + level/3) * scale,
			(random.Next (10) / 10F + level/3) * scale
		);

		//go.GetComponent<Renderer> ().material.color = Random.ColorHSV (0f, 1f, 1f, 1f, 0.5f, 1f);
	}
}
