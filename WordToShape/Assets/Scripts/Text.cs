using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class Text : MonoBehaviour
{
	[Range(0F,10F)]
	public float scale;
	private InputField mainInputField = null;
	private Api api = null;

	void Start ()
	{
		mainInputField = GetComponent<InputField> ();
		api = GetComponent<Api> ();

		// Add listener to catch the submit
		InputField.SubmitEvent submitEvent = new InputField.SubmitEvent ();
		submitEvent.AddListener (ValueChangeCheck);
		mainInputField.onEndEdit = submitEvent;
	}

	// Invoked when the value of the text field changes.
	public void ValueChangeCheck (string value)
	{
		api.GetSenti (value, OnGetSenti, OnErrorSenti);
		mainInputField.text = "";
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

		go.transform.position = new Vector3 (random.Next (4) - 2, random.Next (3) + 4, random.Next (4) - 2);
		go.transform.Rotate (new Vector3 (random.Next (90), 0, random.Next (90)));
		go.transform.localScale = new Vector3 (
			(random.Next (10) / 10F + level) * scale,
			(random.Next (10) / 10F + level) * scale,
			(random.Next (10) / 10F + level) * scale
		);
		go.GetComponent<Renderer> ().material.color = new Color32 (
			(byte)random.Next (255), 
			(byte)random.Next (255), 
			(byte)random.Next (255),
			(byte)255
		);
	}
}
