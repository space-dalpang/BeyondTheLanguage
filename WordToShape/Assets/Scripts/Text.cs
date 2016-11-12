using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class Text : MonoBehaviour
{
	private InputField mainInputField = null;

	private void OnGetWord (JSONObject word)
	{
		String wordName = Regex.Unescape (word.GetField ("name").str);
		ValueChangeCheck (wordName);
	}

	public void Start ()
	{
		mainInputField = GetComponent<InputField> ();
		// Add listener to catch the submit
		InputField.SubmitEvent submitEvent = new InputField.SubmitEvent ();
		submitEvent.AddListener (ValueChangeCheck);
		mainInputField.onEndEdit = submitEvent;

		// Add validation
//		mainInputField.characterValidation = InputField.CharacterValidation.Alphanumeric;
		Api api = GetComponent<Api> ();
		api.GetWords (OnGetWord);
	}

	// Invoked when the value of the text field changes.
	public void ValueChangeCheck (string value)
	{


		mainInputField.text = "";
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

	private int NormValue (int value, int min, int max)
	{
		return value % (max - min) + min;
	}
}
