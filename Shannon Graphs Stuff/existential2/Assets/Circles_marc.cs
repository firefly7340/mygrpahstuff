using UnityEngine;
using System.Collections;
using Vectrosity;
using System.Collections.Generic;

public class Circles_marc: MonoBehaviour {

	Vector2[] lineArray = new Vector2[101];
	VectorLine circ;
	Texture2D tex;

	// Use this for initialization
	void Start () {
		tex = new Texture2D (1, 1, TextureFormat.RGB24, false);
		tex.SetPixel(0, 0, Color.black);
		tex.Apply ();

		List<Vector2> linePoints = new List<Vector2>(lineArray);
		circ = new VectorLine ("circle", linePoints, tex, 5, LineType.Continuous);
		circ.MakeCircle(new Vector2(100, 100), 60, 100, 0);
		circ.Draw ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
