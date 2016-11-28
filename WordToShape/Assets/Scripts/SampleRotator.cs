using UnityEngine;
using System.Collections;

public class SampleRotator : MonoBehaviour {


	private GameObject[] samples;

	// Use this for initialization
	void Start () {
		samples = GameObject.FindGameObjectsWithTag ("sample");
	}
	
	// Update is called once per frame
	void Update () {
		foreach (GameObject sample in samples) {
			sample.transform.localEulerAngles += new Vector3(0, 1, 0);
		}
	}
}
