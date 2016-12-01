﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using BeyondTheLanguage;
using MeshGen;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

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
	private List<GameObject> galleryObjects = new List<GameObject>();
	public Camera UICamera;
	static public bool userInputLock = false;

	void Start ()
	{
		runButton = GetComponent<Button> ();
		api = GetComponent<Api> ();

		runButton.onClick.AddListener(delegate{ValueChangeCheck(mainInputField);});

		Polygon introPolygon = CreateSphericalPolygon(mainInputField.text.GetHashCode() % 16 + 9, mainInputField.text.Length % 5 + 1);
		introPolygon.gameObject.transform.position = new Vector3(0, 2.4f, -200);
		introPolygon.gameObject.tag = "introPolygon";

		samples = GameObject.FindGameObjectsWithTag ("sample");

		foreach (GameObject sample in samples) {
			sample.AddComponent<SampleRotator> ();
			samplesSize.Add (sample.transform.localScale);
		}

		mainInputField.Select ();
		mainInputField.ActivateInputField ();

		LoadSaves ();
	}

	public void ValueChangeCheck (InputField input)
	{
		if (userInputLock) {
			return;
		}
		if (input.text.Trim() == "")
			return;

		api.GetSenti (input.text, OnGetSentiForVCC, OnErrorSenti);

//		setSceneToResult (input.text);

//		Save (input.text, GameObject.FindGameObjectsWithTag ("temp"));

//		GenGalleryObject (input.text);

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

	private void OnGetSentiForVCC (JSONObject result)
	{
		string text = Regex.Unescape (result.GetField ("text").str);
		StartCoroutine (GenWordObject (result));
		GenGalleryObject (result);
		setSceneToResult (text);
		Save (text, GameObject.FindGameObjectsWithTag ("temp"));
//		Regex.Unescape (result.GetField ("text").str);
	}

	private void OnErrorSenti (string error)
	{
		// todo
		UnityEngine.UI.Text txt = GameObject.Find ("ResultText").GetComponent<UnityEngine.UI.Text>();
		txt.text = error;
	}

	void Update(){
		if (Input.GetMouseButtonDown (0)) {
			if (userInputLock){
				return;
			}
			Ray ray = UICamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast (ray.origin, ray.direction, Mathf.Infinity);
			if (hit) {
				string text = hit.collider.gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text;

				api.GetSenti (text, OnGetSenti, OnErrorSenti);
				setSceneToResult (text);
			}
		} 
	}
		
	private void setSceneToResult(string text){
		GameObject.FindGameObjectWithTag ("introPolygon").GetComponent<Renderer>().enabled = false;

		CanvasGroup inputCanvas = GameObject.Find("InputCanvas").GetComponent<CanvasGroup> ();
		CanvasGroup resultCanvas = GameObject.Find ("ResultCanvas").GetComponent<CanvasGroup> ();

		StartCoroutine(Util.FadeOut(inputCanvas));
		StartCoroutine(Util.FadeIn(resultCanvas));

		foreach (GameObject sample in samples){
			StartCoroutine (scaleAnimation (sample, sample.transform.localScale, Vector3.zero));
		}

		foreach (GameObject galleryObject in galleryObjects){
			StartCoroutine (scaleAnimation (galleryObject, galleryObject.transform.localScale, Vector3.zero));
		}

		GameObject go = GameObject.Find ("Ground");

		Transform target = new GameObject ("genDummy").transform;
		target.position = go.transform.position + new Vector3(0, 8, 0);

		MouseCamera mouseCamera = Camera.main.GetComponent<MouseCamera> ();
		mouseCamera.target = target;
		mouseCamera.targetFov = 40f;

		UnityEngine.UI.Text txt = GameObject.Find ("ResultText").GetComponent<UnityEngine.UI.Text>();

		txt.text = Util.FormatString(text);
	}

	private IEnumerator scaleAnimation(GameObject go, Vector3 startScale, Vector3 targetScale){
		userInputLock = true;
		float elapsedTime = 0.0f;
		while (elapsedTime <= 1.0f)
		{
			go.transform.localScale = Vector3.Slerp(startScale, targetScale, elapsedTime / 0.9f);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		go.transform.localScale = targetScale;
		userInputLock = false;
	}

	private IEnumerator GenWordObject (JSONObject result)
	{
		Util.DestroyGameObjectsWithTag ("temp", 1f);
		RandomShapes.go = false;

		JSONObject words = result.GetField ("words");
		if (words.list.Count == 0) {
			Polygon polygon = CreateSphericalPolygon (mainInputField.text.GetHashCode () % 16 + 9, mainInputField.text.Length % 5 + 1);
			polygon.gameObject.transform.position = new Vector3 (0, 8, 0) + UnityEngine.Random.insideUnitSphere;
			polygon.gameObject.tag = "temp";
		} else {
			long unmatchedCount = result.GetField ("senti").GetField ("unmatched").i;
			string analyzed = Regex.Unescape (result.GetField ("analyzed").str);
			while (unmatchedCount > 0) {
				unmatchedCount -= 12;
				int level = Random.Range (1, 3);
				string name = analyzed + unmatchedCount;
				GameObject go = CreateSphericalPolygon (name.GetHashCode () % 16 + 9, name.Length % 2 + 1).gameObject;
				go.tag = "temp";
				go.GetComponent<MeshRenderer> ().material.color = Random.ColorHSV (0f, 1f, 1f, 1f, 0.5f, 1f);
				RandomTransformGameObject (go, level, name.GetHashCode ());

				Vector3 targetScale = new Vector3 ();
				targetScale = go.transform.localScale;
				go.transform.localScale = Vector3.zero;
				StartCoroutine (scaleAnimation (go, Vector3.zero, targetScale));

				yield return new WaitForSeconds (0.2f);
			}
		}

		foreach (JSONObject word in words.list) {
			string name = Regex.Unescape (word.GetField ("name").str);
			string category = Regex.Unescape (word.GetField ("category").str);
			int level = (int)word.GetField ("level").n;
			//Debug.Log ("word: " + name + " " + category + " " + level);
			GameObject go = GenSentiObject (name, category, level);
			go.tag = "temp";
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
			targetMaterial = go.GetComponent<MeshRenderer> ().material;
			targetMaterial.color = Random.ColorHSV (0f, 1f, 1f, 1f, 0.5f, 1f);
			break;
		}

		GameObject newObj;

		newObj = new GameObject ("SentiObject");

		MeshFilter meshFilter = newObj.AddComponent<MeshFilter> ();
		meshFilter.mesh = go.GetComponentInChildren<MeshFilter> ().sharedMesh;

		Mesh mesh = meshFilter.mesh;
		mesh.Optimize ();
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();

		MeshRenderer meshRenderer = newObj.AddComponent<MeshRenderer> ();
		meshRenderer.material = targetMaterial;

		//		Rigidbody rigid = newObj.AddComponent<Rigidbody> ();
		//		rigid.mass = 5;
		//		rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

		MeshCollider collider = newObj.AddComponent<MeshCollider> ();
		collider.convex = true;

		// Destroy (go);
		return newObj;
	}

	private void RandomTransformGameObject (GameObject go, int level, int seed)
	{
		System.Random random = new System.Random (seed);

		//go.transform.position = new Vector3 (random.Next (4) - 2, random.Next (3) + 5, random.Next (4) - 2);
		go.transform.position = new Vector3(0, 8, 0) + Random.insideUnitSphere * 2f;
		go.transform.rotation = Random.rotation;

		go.transform.localScale = new Vector3 (
			(random.Next (7) / 10F + level/6 + 0.3F) * scale,
			(random.Next (7) / 10F + level/6 + 0.3F) * scale,
			(random.Next (7) / 10F + level/6 + 0.3F) * scale
		);

		//go.GetComponent<Renderer> ().material.color = Random.ColorHSV (0f, 1f, 1f, 1f, 0.5f, 1f);
	}

	private void Save(string text, GameObject[] objects){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open (Application.persistentDataPath + "/" + System.DateTime.Now.ToString("yyMMddHHmmss") + ".dat", FileMode.Create);

		SentiData data = new SentiData ();

		data.text = text;
		//		data.scales = new Vector3[objects.Length];
		//		data.positions = new Vector3[objects.Length];
		//		data.rotations = new Quaternion[objects.Length];
		//
		//		for (int i=0; i < objects.Length; i++) {
		//			data.scales[i] = objects [i].transform.localScale;
		//			data.positions [i] = objects [i].transform.position;
		//			data.rotations [i] = objects [i].transform.localRotation;
		//		}

		bf.Serialize (file, data);
		file.Close ();
	}

	private SentiData Load(string filename){
		if (File.Exists (Application.persistentDataPath + "/" + filename)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/" + filename, FileMode.Open);
			SentiData data = (SentiData)bf.Deserialize (file);
			file.Close ();

			return data;
		}
		return null;
	}

	private void LoadSaves(){
		DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath);
		List<FileInfo> fileInfo = info.GetFiles("*.dat").ToList();

		//fileInfo.Reverse ();

		foreach (FileInfo file in fileInfo) {
			SentiData data = Load (file.Name);
			api.GetSenti(data.text, GenGalleryObject, OnErrorSenti);
		}
	}

	public void reloadGallery(){
		foreach (GameObject gallery in galleryObjects) {
			Destroy (gallery);
		}
		galleryObjects.Clear ();

		LoadSaves ();

		foreach (GameObject gallery in galleryObjects) {
			gallery.transform.localScale = new Vector3 (0, 0, 0);
		}

		GameObject content = GameObject.Find ("Content");
		content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 2600);
	}

	private void GenGalleryObject (string text){
		api.GetSenti(text, GenGalleryObject, OnErrorSenti);
	}


	private void GenGalleryObject (JSONObject result)
	{
		JSONObject words = result.GetField ("words");
		GameObject galleryObject = new GameObject ("GalleryObject");
		GameObject gallerySenti = new GameObject ("GallerySentis");
		gallerySenti.transform.SetParent (galleryObject.transform);
		gallerySenti.transform.position = new Vector3 (0, 0, 10);
		//gallerySenti.AddComponent<SampleRotator> ();

		GameObject content = GameObject.Find ("Content");
		galleryObject.transform.parent = content.transform;

		if (words.list.Count == 0) {
			Polygon polygon = CreateSphericalPolygon (mainInputField.text.GetHashCode () % 16 + 9, mainInputField.text.Length % 5 + 1);
			RandomTransformGameObject (polygon.gameObject, Random.Range (1, 3), result.GetField ("text").str.GetHashCode());
			polygon.gameObject.transform.position -= new Vector3 (0, 8, 0);
			polygon.gameObject.transform.SetParent(gallerySenti.transform);
		} else {
			long unmatchedCount = result.GetField ("senti").GetField ("unmatched").i;
			string analyzed = Regex.Unescape (result.GetField ("analyzed").str);
			while (unmatchedCount > 0) {
				unmatchedCount -= 12;
				int level = Random.Range (1, 3);
				string name = analyzed + unmatchedCount;
				GameObject go = CreateSphericalPolygon (name.GetHashCode () % 16 + 9, name.Length % 2 + 1).gameObject;
				go.GetComponent<MeshRenderer> ().material.color = Random.ColorHSV (0f, 1f, 1f, 1f, 0.5f, 1f);
				RandomTransformGameObject (go, level, name.GetHashCode ());
				go.transform.position -= new Vector3 (0, 8, 0);
				go.transform.SetParent(gallerySenti.transform);
			}
		}

		foreach (JSONObject word in words.list) {
			string name = Regex.Unescape (word.GetField ("name").str);
			string category = Regex.Unescape (word.GetField ("category").str);
			int level = (int)word.GetField ("level").n;
			GameObject go = GenSentiObject (name, category, level);
			RandomTransformGameObject (go, level, name.GetHashCode ());
			go.transform.position -= new Vector3 (0, 8, 0);
			go.transform.SetParent(gallerySenti.transform);
		}

		GameObject textObject = Instantiate(GameObject.Find ("ResultText"));
		UnityEngine.UI.Text text = textObject.GetComponent<UnityEngine.UI.Text> ();
		text.text = Util.FormatString(result.GetField ("text").str);
		text.fontSize = 18;
		textObject.transform.localPosition = new Vector3 (0, 0, 0);
		textObject.transform.localScale = new Vector3 (0.015f, 0.015f, 0.015f);
		textObject.transform.SetParent(galleryObject.transform);
		textObject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (160, 300);
		textObject.GetComponent<UnityEngine.UI.Text> ().verticalOverflow = VerticalWrapMode.Truncate;

		Util.ChangeLayerRecursively (galleryObject.transform, "UI");
		galleryObject.transform.localScale = new Vector3 (30f, 30f, 30f);

		BoxCollider2D collider = galleryObject.AddComponent<BoxCollider2D> ();
		collider.size = new Vector2 (4, 4);
		//galleryObject.AddComponent<GalleryHit> ();
		galleryObject.tag = "gallery";
		//galleryObjects.Insert (0, galleryObject);
		galleryObjects.Add(galleryObject);

		if(galleryObjects.Count % 2 == 1)
			content.GetComponent<RectTransform> ().sizeDelta += new Vector2 (0, 250);

		alignGallerys ();
	}

	private void alignGallerys(){
		for (int i = 0; i < galleryObjects.Count(); i++) {
			int gridX = i % 2;
			int gridY = Mathf.FloorToInt (i / 2f);

			galleryObjects[galleryObjects.Count() - i - 1].transform.localPosition = new Vector3 (gridX * 250 + 400, gridY * -250 - 465, 0);
		}
	}
}

[System.Serializable]	
class SentiData{
	public string text;
	//	public Vector3[] scales;
	//	public Vector3[] positions;
	//	public Quaternion[] rotations;
}
