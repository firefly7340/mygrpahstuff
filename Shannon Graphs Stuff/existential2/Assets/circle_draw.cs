
using UnityEngine;
using System.Collections;
using Vectrosity;
using System.Collections.Generic;

public class circle_draw: MonoBehaviour {
	public Vector3 point_touched;
	public Vector3 point_released;
	public Vector3 point_current;
	public Vector3 distance;
	public float radius;
	public Vector3 origin;
	public bool flagged;
	public VectorLine myLine;
	public List<Vector3>[] point_array;
	public int i;
	public SphereCollider[] sphere_array;
	RaycastHit hit;

	void Start () {
		i = 0;
		point_array = new List<Vector3>[100];//max 100 circles
		point_array[i] = new List<Vector3> (200); //200 vertices (actually 100??)
		myLine = new VectorLine("circleline", point_array[i], 6.0f);
		sphere_array = new SphereCollider[100];//100 sphere colliders to go with those circles
		//myLine.collider = true;
		//myLine.MakeCircle (new Vector3 (-2,-2,-2), 5);
		//myLine.Draw ();
	}
	void Update () {

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Input.GetMouseButtonDown (0)) {
			if (Physics.Raycast (ray, out hit) == false) {//click does not touch an object
				point_touched = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				flagged = true;
				print ("HEY");
			} else {
				flagged = false;
			}
		}
		if (Input.GetMouseButton (0)) {
			point_current= Camera.main.ScreenToWorldPoint (Input.mousePosition);
			distance = point_touched - point_current;
			radius = (distance.magnitude) / 2;
			origin = point_touched - (distance / 2);
			myLine.MakeCircle (origin, radius);
			//print (origin);
			//print ("made it!");
			if (flagged == true){
				myLine.Draw ();
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			if (Physics.Raycast (ray, out hit) == true) {
				myLine.MakeCircle (origin, 0); //deals with this line
				myLine.Draw();
				i = i + 1;
			}
			else if (Physics.Raycast (ray, out hit) == false) {
				point_released = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				distance = point_touched - point_released;
				radius = (distance.magnitude) / 2;
				origin = point_touched - (distance / 2);
				myLine.MakeCircle (origin, radius);
				//print (origin);
				//print ("made it!");
				if (flagged == true){
				myLine.Draw ();
				}
				i = i + 1;
				point_array[i] = new List<Vector3> (200); //200 vertices (actually 100??)
				myLine = new VectorLine("circleline", point_array[i], 6.0f);
				//myLine.collider = true;
				//myLine.collider = true;
			}
		}
	}
}

//expand circle manually, stop if there is a block, propogating up the tree eventually, attach a circle collider over circle line
//if touch is within certain distance of line of outside circle- if the collider is a child of the larger circle, can I get the information from this
//line to resize? left click to move, right click to resize