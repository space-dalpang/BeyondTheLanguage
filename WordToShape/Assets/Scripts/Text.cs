using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using BeyondTheLanguage;
using MeshGen;
using System;


public class Text : MonoBehaviour
{
	[Range(0F,10F)]
	public float scale;
	public InputField mainInputField;
	private Button runButton = null;
	private Api api = null;
	private MouseCamera mouseCamera;

	void Start ()
	{
		Debug.Log (mainInputField);
		runButton = GetComponent<Button> ();

		api = GetComponent<Api> ();

		runButton.onClick.AddListener(delegate{ValueChangeCheck(mainInputField);});
	}

	public void ValueChangeCheck (InputField input)
	{
		api.GetSenti (input.text, OnGetSenti, OnErrorSenti);
		input.text = "";


		GameObject parent = GameObject.Find("InputCanvas");
		CanvasGroup cg = parent.GetComponent<CanvasGroup> ();

		Button backButton = GameObject.Find ("BackButton").GetComponent<Button> ();

		StartCoroutine(FadeOut(cg));
	}

	IEnumerator FadeOut(CanvasGroup canvasGroup)
	{
		float time = 1f;
		while(canvasGroup.alpha > 0)
		{
			canvasGroup.alpha -= Time.deltaTime / time;
			yield return null;
		}
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
		JSONObject words = result.GetField ("words");
		if (words.list.Count == 0) {
			Polygon polygon = CreateSphericalPolygon(mainInputField.text.GetHashCode() % 16 + 9, mainInputField.text.Length % 5 + 1).Rigidize();
			polygon.gameObject.transform.position = new Vector3(0, 8, 0) + UnityEngine.Random.insideUnitSphere;
		}

		foreach (JSONObject word in words.list) {
			string name = Regex.Unescape (word.GetField ("name").str);
			string category = Regex.Unescape (word.GetField ("category").str);
			int level = (int)word.GetField ("level").n;
			Debug.Log ("word: " + name + " " + category + " " + level);
			GameObject go = GenSentiObject (category);
			RandomTransformGameObject (go, level, name.GetHashCode ());
			yield return new WaitForSeconds (0.5f);
		}
	}

	private GameObject GenSentiObject (String category)
	{	
		GameObject go = null;
		switch (category) {
		case "기쁨":
		case "슬픔":
		case "통증":
			go = GameObject.CreatePrimitive (PrimitiveType.Cube);
			break;
		case "공포":
		case "분노":
		case "혐오":
			go = GameObject.CreatePrimitive (PrimitiveType.Capsule);
			break;
		case "중성":
		case "흥미":
			go = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
			break;
		case "놀람":
		case "지루함":
		default:
			go = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			break;
		}
		Rigidbody gameObjectsRigidBody = go.AddComponent<Rigidbody> ();
		gameObjectsRigidBody.mass = 5;

		return go;
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
