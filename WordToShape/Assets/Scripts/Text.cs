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
	private Button runButton = null;
	private Api api = null;

	void Start ()
	{
		runButton = GetComponent<Button> ();

		api = GetComponent<Api> ();

		runButton.onClick.AddListener(delegate{ValueChangeCheck(mainInputField);});
	}

	public void ValueChangeCheck (InputField input)
	{
		api.GetSenti (input.text, OnGetSenti, OnErrorSenti);
		input.text = "";

		CanvasGroup inputCanvas = GameObject.Find("InputCanvas").GetComponent<CanvasGroup> ();
		CanvasGroup layoutCanvas = GameObject.Find ("LayoutCanvas").GetComponent<CanvasGroup> ();

		StartCoroutine(Util.FadeOut(inputCanvas));
		StartCoroutine(Util.FadeIn(layoutCanvas));

		GameObject go = GameObject.Find ("Ground");

		Transform target = new GameObject ().transform;
		target.position = go.transform.position + new Vector3(0, 8, 0);

		Camera.main.GetComponent<MouseCamera> ().target = target;
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

	private IEnumerator GenWordObject (JSONObject result)
	{
		Util.DestroyGameObjectsWithTag ("temp", 1f);

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
			Debug.Log ("word: " + name + " " + category + " " + level);
			GameObject go = GenSentiObject (name, category, level);
			RandomTransformGameObject (go, level, name.GetHashCode ());
			yield return new WaitForSeconds (0.5f);
		}
	}


	private GameObject GenSentiObject (string name, string category, int level)
	{	
		GameObject go = null;
		switch (category) {
		case "기쁨":
			go = UnityEngine.Resources.Load ("senti-mesh-pleasure-" + Random.Range(1, 2)) as GameObject;
			break;
		case "슬픔":
			go = UnityEngine.Resources.Load ("senti-mesh-sad-1") as GameObject;
			break;
		case "통증":
		case "공포":
		case "분노":
			go = UnityEngine.Resources.Load ("senti-mesh-anger-1") as GameObject;
			break;
		case "혐오":
			go = UnityEngine.Resources.Load ("senti-mesh-hate-1") as GameObject;
			break;
		case "중성":
			go = UnityEngine.Resources.Load ("senti-mesh-medium-1") as GameObject;
			break;
		case "흥미":
			go = UnityEngine.Resources.Load ("senti-mesh-interest-1") as GameObject;
			break;
		case "놀람":
			go = UnityEngine.Resources.Load ("senti-mesh-surprised-1") as GameObject;
			break;
		case "지루함":
			go = UnityEngine.Resources.Load ("senti-mesh-boredom-1") as GameObject;
			break;
		default:
			Polygon polygon = CreateSphericalPolygon (name.GetHashCode () % 16 + 9, name.Length % 5 + 1).Rigidize ();
			go = polygon.gameObject;
			polygon.gameObject.tag = "temp";
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
		meshRenderer.material = go.GetComponentInChildren<MeshRenderer> ().sharedMaterial;
		meshRenderer.material.color = Random.ColorHSV (0f, 1f, 1f, 1f, 0.5f, 1f);


//		Rigidbody rigid = newObj.AddComponent<Rigidbody> ();
//		rigid.mass = 5;
//		rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

		MeshCollider collider = newObj.AddComponent<MeshCollider> ();
		collider.convex = true;

		newObj.transform.position = transform.position + new Vector3(0, 8, 0) + Random.insideUnitSphere;
		newObj.transform.rotation = Random.rotation;

		newObj.tag = "temp";

		Destroy (go);

		return newObj;
	}

	private void RandomTransformGameObject (GameObject go, int level, int seed)
	{
		System.Random random = new System.Random (seed);

		go.transform.position = new Vector3 (random.Next (4) - 2, random.Next (3) + 5, random.Next (4) - 2);
		go.transform.Rotate (new Vector3 (random.Next (90), 0, random.Next (90)));
		go.transform.localScale = new Vector3 (
			(random.Next (10) / 10F + level) * scale + 1,
			(random.Next (10) / 10F + level) * scale + 1,
			(random.Next (10) / 10F + level) * scale + 1
		);
		go.GetComponent<Renderer> ().material.color = new Color32 (
			(byte)random.Next (255), 
			(byte)random.Next (255), 
			(byte)random.Next (255),
			(byte)255
		);
	}
}
