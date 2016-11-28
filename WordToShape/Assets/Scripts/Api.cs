using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Api : MonoBehaviour
{
	public delegate void OnGetSenti (JSONObject result);

	public delegate void OnErrorSenti (string error);

	public void GetSenti (string query, OnGetSenti cb, OnErrorSenti errorCb)
	{
		StartCoroutine (InternalGetSenti (query, cb, errorCb));
	}

	private IEnumerator InternalGetSenti (string query, OnGetSenti cb, OnErrorSenti errorCb)
	{
		string url = "http://localhost/senti?q=" + WWW.EscapeURL(query);
		WWW www = new WWW (url);
		yield return www;
		if (www.error == null) {
			JSONObject jo = new JSONObject (www.text);
			cb (jo);
		} else {
			errorCb (www.error);
			Debug.Log ("WWW Error: " + www.error);
		}

	}
}
