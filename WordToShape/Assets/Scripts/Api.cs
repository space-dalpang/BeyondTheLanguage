using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Api : MonoBehaviour
{
	public delegate void OnGetWord(JSONObject word);

	public void GetWords(OnGetWord _OnGetWord) {
		string url = "http://localhost/words";
		WWW www = new WWW (url);
		StartCoroutine (WaitForRequest (www, _OnGetWord));

	}

	IEnumerator WaitForRequest (WWW www, OnGetWord _OnGetWord)
	{

		yield return www;

		// check for errors

		if (www.error == null) {
			StartCoroutine(HandleJson (www.text, _OnGetWord));
		} else {
			Debug.Log ("WWW Error: " + www.error);
		}

	}

	IEnumerator HandleJson (string json, OnGetWord _OnGetWord)
	{
		JSONObject jo = new JSONObject (json);
		List<JSONObject> words = jo.GetField ("words").list;
		foreach (JSONObject word in words) {
			_OnGetWord (word);
			yield return new WaitForSeconds(0.2f);
		}
	}
}
