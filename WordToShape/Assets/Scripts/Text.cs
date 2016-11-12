using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Text : MonoBehaviour {
	private InputField mainInputField = null;

	public void Start()
	{
		mainInputField = GetComponent<InputField> ();
		// Add listener to catch the submit
		InputField.SubmitEvent submitEvent = new InputField.SubmitEvent();
		submitEvent.AddListener(ValueChangeCheck);
		mainInputField.onEndEdit = submitEvent;

		// Add validation
//		mainInputField.characterValidation = InputField.CharacterValidation.Alphanumeric;

	}

	// Invoked when the value of the text field changes.
	public void ValueChangeCheck(string value)
	{


		mainInputField.text = "";
		int n = new System.Random(Time.time.ToString().GetHashCode()).Next(90);

		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		Rigidbody gameObjectsRigidBody = cube.AddComponent<Rigidbody>();
		gameObjectsRigidBody.mass = 5;
		cube.transform.position = new Vector3 (0, 5F, 0);
		cube.transform.Rotate(new Vector3 (n, 0, n));
		Debug.Log (value.GetHashCode ());
		cube.transform.localScale = new Vector3 (value.GetHashCode() % 6 + 2, value.GetHashCode() % 5 + 2, value.GetHashCode() % 3 + 2);
		cube.GetComponent<Renderer> ().material.color = new Color32 ((byte)(value.GetHashCode() % 254), (byte)(value.GetHashCode() % 128), (byte)(value.GetHashCode() % 64), 255);
//		cube.GetComponent<Renderer> ().material.color = new Color32 (0, 0, 0, 255);



	}
}
