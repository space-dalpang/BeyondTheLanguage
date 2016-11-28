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
		string host = "192.168.99.100:8080";
//		string host = "localhost";
		string url = "http://" + host + "/senti?q=" + WWW.EscapeURL(query);
		WWW www = new WWW (url);
		yield return www;
		if (www.error == null) {
			JSONObject jo = new JSONObject (www.text);
			cb (jo);
		} else {
			errorCb (url + ": " + www.error);
			Debug.Log ("WWW Error: " + www.error);
		}

	}
}
