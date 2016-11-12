using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Api : MonoBehaviour
{

	void Start ()
	{
		string url = "http://localhost/words";
		WWW www = new WWW (url);
		StartCoroutine (WaitForRequest (www));

	}

	IEnumerator WaitForRequest (WWW www)
	{

		yield return www;

		// check for errors

		if (www.error == null) {
			HandleJson (www.text);
		} else {
			Debug.Log ("WWW Error: " + www.error);
		}

	}

	void HandleJson (string json)
	{
		JSONObject jo = new JSONObject (json);
		List<JSONObject> words = jo.GetField ("words").list;
		foreach (JSONObject word in words) {
			Debug.Log (Regex.Unescape (word.GetField ("name").str));
		}
	}
}
