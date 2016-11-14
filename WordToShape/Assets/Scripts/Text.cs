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
	public GameObject[] meshs;
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
		mainInputField.text = "";
		addPrefab(value);

		int n = new System.Random (Time.time.ToString ().GetHashCode ()).Next (90);

		GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		Rigidbody gameObjectsRigidBody = cube.AddComponent<Rigidbody> ();
		gameObjectsRigidBody.mass = 5;

		cube.transform.position = new Vector3 (0, 5F, 0);
		cube.transform.Rotate (new Vector3 (n, 0, n));

		int valueHash = value.GetHashCode ();
		cube.transform.localScale = new Vector3 (NormValue(valueHash, 5, 20) / 10F, NormValue(valueHash, 7, 20) / 10F, NormValue(valueHash, 6, 20) / 10F);
		cube.GetComponent<Renderer> ().material.color = new Color32 ((byte)NormValue (valueHash, 3, 255), (byte)NormValue (valueHash, 4, 255), (byte)NormValue (valueHash, 5, 255), 255);
	}

	public void addPrefab (string value) 
	{
		int n = new System.Random(Time.time.ToString().GetHashCode()).Next(90);
//
//		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//		Rigidbody gameObjectsRigidBody = cube.AddComponent<Rigidbody>();
//		gameObjectsRigidBody.mass = 5;
//		cube.transform.position = new Vector3 (0, 5F, 0);
//		cube.transform.Rotate(new Vector3 (n, 0, n));
//		Debug.Log (value.GetHashCode ());
//		cube.transform.localScale = new Vector3 (value.GetHashCode() % 6 + 2, value.GetHashCode() % 5 + 2, value.GetHashCode() % 3 + 2);
//		cube.GetComponent<Renderer> ().material.color = new Color32 ((byte)(value.GetHashCode() % 254), (byte)(value.GetHashCode() % 128), (byte)(value.GetHashCode() % 64), 255);

//		GameObject gobj = meshs [value.GetHashCode () % meshs.Length];
		GameObject gobj = meshs [4];
		gobj.transform.position = new Vector3 (0, 5F, 0);

//		gobj.GetComponent<Renderer> ().material.color = new Color32 ((byte)(value.GetHashCode() % 254), (byte)(value.GetHashCode() % 128), (byte)(value.GetHashCode() % 64), 255);
		Rigidbody gameObjectsRigidBody = gobj.AddComponent<Rigidbody>();
//		gameObjectsRigidBody.mass = 5;

		Instantiate (gobj, new Vector3 (0, 5F, 0), new Quaternion(n, 0, n, 0));

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
