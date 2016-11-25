using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MeshGen;

public class RandomShapes : MonoBehaviour {

	public float interval = 0.5f;
	public static bool go = true;
	float timer = 0f;

	Polygon CreateFlatPyramid() {
		var size = 20f;
		var eulerX = (Mathf.PI / 2 + Mathf.Asin(1f/3f)) * Mathf.Rad2Deg;
		var eulerY = 90;
		var vertices = new List<Vector3>() {
			Vector3.up * size * 0.1f,
			Quaternion.Euler(eulerX, 0, 0) * Vector3.up * size,
			Quaternion.Euler(eulerX, eulerY * 1, 0) * Vector3.up * size,
			Quaternion.Euler(eulerX, eulerY * 2, 0) * Vector3.up * size,
			Quaternion.Euler(eulerX, eulerY * 3, 0) * Vector3.up * size
		};
		return Factory.Create("Pyramid", vertices);
	}

	Polygon CreateSphericalPolygon(int numVertices = 15, float radius = 1f) {
		var vertices = new List<Vector3>();
		var center = new Vector3(0, 0, 0);
		for (int i = 0; numVertices > i; i++) {
			vertices.Add(Random.onUnitSphere * radius + center);
		}
		return Factory.Create("RandomShape", vertices);
	}

	void Create() {
		Object[] prefabs = Util.getPreFabs ();

		int n = Random.Range (1, prefabs.Length);

		if (n < prefabs.Length-1) {
			GameObject newObj;	
			GameObject go = prefabs [n] as GameObject;

			//GameObject newObj = Instantiate (go);
			newObj = new GameObject ("SentiObject");

			MeshFilter meshFilter = newObj.AddComponent<MeshFilter> ();
			meshFilter.mesh = go.GetComponentInChildren<MeshFilter> ().sharedMesh;

			Mesh mesh = meshFilter.mesh;
			mesh.Optimize ();
			mesh.RecalculateNormals ();
			mesh.RecalculateBounds ();

			MeshRenderer meshRenderer = newObj.AddComponent<MeshRenderer> ();
			meshRenderer.material = go.GetComponentInChildren<MeshRenderer> ().sharedMaterial;
			meshRenderer.material.color = Random.ColorHSV (0f, 1f, 1f, 1f, 0.5f, 1f);


			Rigidbody rigid = newObj.AddComponent<Rigidbody> ();
			rigid.mass = 5;
			rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

			MeshCollider collider = newObj.AddComponent<MeshCollider> ();
			collider.convex = true;
			newObj.transform.position = transform.position + new Vector3(0, 8, 0) + Random.insideUnitSphere;

			newObj.transform.rotation = Random.rotation;
			newObj.tag = "temp";
		} else {
			var polygon = CreateSphericalPolygon ().Rigidize ();
			polygon.gameObject.transform.position = transform.position + new Vector3 (0, 8, 0) + Random.insideUnitSphere;
			polygon.gameObject.tag = "temp";
		}

	}

	void Start() {
		CreateFlatPyramid().gameObject.transform.position = transform.position;
	}

	void Update() {
		timer += Time.deltaTime;
		if (timer > interval) {
			timer = 0f;
			if(go)
				Create();
		}
	}

}