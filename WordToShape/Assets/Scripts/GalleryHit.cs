using UnityEngine;
using System.Collections;

public class GalleryHit : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast (ray.origin, ray.direction, Mathf.Infinity);
			if (hit) {
				Debug.Log (hit.collider.gameObject.name);
			}
		} 
	}

	void OnMouseDown(){
		Debug.Log ("down");
	}
}
