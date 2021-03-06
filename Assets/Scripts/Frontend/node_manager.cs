﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node_manager : MonoBehaviour {

    public static List<circle_drawer> all_cuts = new List<circle_drawer>();
    public static List<variable_drawer> all_vars = new List<variable_drawer>();

    public GameObject circle_prefab;
    public GameObject variable_prefab;
    public static circle_drawer currently_selected_circle;
    public static variable_drawer currently_selected_variable;

    [HideInInspector]
    public static bool currently_scaling_cut = false;
    [HideInInspector]
    public static bool currently_moving_cut = false;
    [HideInInspector]
    public static bool currently_moving_var = false;

    Vector2 mouse_position;
    Vector2 clicked_point;
    Vector2 offset;

    public static float min_circle_radius = 0.5f;
    public static float selection_width = 0.05f;
    public static float variable_selection_radius = 0.25f;

	public static bool on_button = false;

    void Update () {

        // get the 2d worldspace position of the mouse
        mouse_position = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

        // ok, what are we doing right now?
        if (currently_scaling_cut)         // we're in the middle of scaling a cut
        {
            // the circle radius is the distance to the mouse -- unless the cut is too small, in which case it snaps to the min size
            currently_selected_circle.radius = Mathf.Max(Vector2.Distance(mouse_position, currently_selected_circle.transform.position), min_circle_radius);
            
            // if the player releases the left mouse button
            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)){
                currently_scaling_cut = false;
                currently_selected_circle = null;
            }

        }
        else if (currently_moving_cut)    // we're in the middle of moving a cut
        {
            currently_selected_circle.transform.position = new Vector3(offset.x+mouse_position.x, offset.y+mouse_position.y, 0f);

            // if the player releases the left mouse button
            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)){
                currently_moving_cut = false;
            }

        }
        else if (currently_moving_var)    // we're in the middle of moving a variable
        {
            currently_selected_variable.transform.position = new Vector3(offset.x+mouse_position.x, offset.y+mouse_position.y, 0f);

            // if the player releases the left mouse button
            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)){
                currently_moving_var = false;
            }
        }
        else                               // we're not editing anything right now, so we can start
        {
            // is the mouse on the edge of a circle or variable? if so, select it. variable have priority over circles
            currently_selected_circle = null;
            currently_selected_variable = null;
            for( int i = 0; i < all_cuts.Count; i++ )
            {
                circle_drawer cir = all_cuts[i];
                Vector2 center = new Vector2(cir.transform.position.x, cir.transform.position.y);

                if( Mathf.Abs(Vector2.Distance(center, mouse_position) - cir.radius) < selection_width ) {
                    currently_selected_circle = cir;
                }
            }

            for( int i = 0; i < all_vars.Count; i++ )
            {
                variable_drawer var = all_vars[i];
                Vector2 center = new Vector2(var.transform.position.x, var.transform.position.y);

                if( Vector2.Distance(center, mouse_position) < variable_selection_radius ) {
                    currently_selected_variable = var;
                    currently_selected_circle = null;   // even if we are over a circle, we're over a variable so we don't care lol. deselect the circle
                }
            }



            if (Input.GetMouseButtonDown(0)){
				Vector2 rayPos = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
				RaycastHit2D hit = Physics2D.Raycast(rayPos,Vector2.zero, 0f);
                // we clicked. do we have anything selected? if so, move that. otherwise, make a new circle

                if (currently_selected_circle) {
                    currently_moving_cut = true;
                    clicked_point = mouse_position;
                    offset = new Vector2(currently_selected_circle.transform.position.x,currently_selected_circle.transform.position.y) - clicked_point;
                }
                else if (currently_selected_variable) {
                    currently_moving_var = true;
                    clicked_point = mouse_position;
                    offset = new Vector2(currently_selected_variable.transform.position.x,currently_selected_variable.transform.position.y) - clicked_point;
                }
                else {
					if (!on_button) {
						AddCircle (mouse_position);
					}
                }
            }

            if (Input.GetMouseButtonDown(1)){
                // we right-clicked. do we have a circle selected? if so, scale it. otherwise, make a new variable

                if (currently_selected_circle) {
                    currently_scaling_cut = true;
                }
                else if (!currently_selected_variable){ // if we're not selecting a variable already
                    AddVariable(mouse_position, "Q");   // add a new one
                }
            }

            if (Input.GetMouseButtonDown(2)){
                // we middle-clicked. do we have anything selected? if so, delete it

                if (currently_selected_circle) {
                    RemoveCircle(currently_selected_circle);
                }
                else if (currently_selected_variable) {
                    RemoveVariable(currently_selected_variable);
                }
            }
        }
    }

    // put a new circle in the scene
    void AddCircle(Vector2 pos)
    {    
        currently_selected_circle = Instantiate(circle_prefab, new Vector3(pos.x,pos.y,0f), Quaternion.identity).GetComponent<circle_drawer>();
        clicked_point = pos;
        currently_scaling_cut = true;

        all_cuts.Add(currently_selected_circle);
    }


    // put a new variable in the scene
    void AddVariable(Vector2 pos, string name)
    {    
        currently_selected_variable = Instantiate(variable_prefab, new Vector3(pos.x,pos.y,0f), Quaternion.identity).GetComponent<variable_drawer>();
        all_vars.Add(currently_selected_variable);
        currently_selected_variable.set_text(name);

        currently_moving_var = true;
        clicked_point = mouse_position;
        offset = Vector2.zero;
    }

	public void ClickAddVariable(string name)
	{    
		Vector2 pos;
		mouse_position = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
		AddVariable(mouse_position, name);
		currently_selected_variable.set_text(name);
		/*mouse_position = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
		pos = mouse_position;
		currently_selected_variable = Instantiate(variable_prefab, new Vector3(pos.x,pos.y,0f), Quaternion.identity).GetComponent<variable_drawer>();
		all_vars.Add(currently_selected_variable);
		currently_selected_variable.set_text(name);

		currently_moving_var = true;
		clicked_point = mouse_position;
		offset = Vector2.zero;
		*/
	}

	public void OnButton(){
		on_button = true;
	}

	public void OffButton(){
		on_button = false;
	}

    // delete a circle and remove it from the list
    void RemoveCircle(circle_drawer cir)
    {
        all_cuts.Remove(cir);
        Destroy(cir.gameObject);
    }

    // delete a variable and remove it from the list
    void RemoveVariable(variable_drawer var)
    {
        all_vars.Remove(var);
        Destroy(var.gameObject);
    }


    // does this other circle intersect with this other circle?
    public static bool intersect (circle_drawer cir_a, circle_drawer cir_b){

        float dist = Vector2.Distance(new Vector2(cir_a.transform.position.x, cir_a.transform.position.y), new Vector2(cir_b.transform.position.x, cir_b.transform.position.y));
        float threshold = 0.05f;

        if (dist > (cir_a.radius + cir_b.radius) + threshold) {
            // no overlap
            return false;
        }
        else if (dist <= (cir_a.radius - cir_b.radius) - threshold) 
        {
            // b is completely inside a
            return false;
        }
        else if (dist <= (cir_b.radius - cir_a.radius) - threshold) 
        {
            // a is completely inside b
            return false;
        }
        
        // we got overlap
        return true;
    }

    // does this circle intersect with this variable?
    public static bool intersect (circle_drawer cir, variable_drawer var){

        float dist = Vector2.Distance(new Vector2(cir.transform.position.x, cir.transform.position.y), new Vector2(var.transform.position.x, var.transform.position.y));
        float threshold = 0.05f;

        if (dist > (cir.radius + variable_selection_radius) + threshold) {
            // no overlap
            return false;
        }
        else if (dist <= (cir.radius - variable_selection_radius) - threshold)
        {
            // variable is completely inside circle
            return false;
        }
        
        // we got overlap
        return true;
    }

    // does this variable intersect with this other variable?
    public static bool intersect (variable_drawer var_a, variable_drawer var_b){

        float dist = Vector2.Distance(new Vector2(var_a.transform.position.x, var_a.transform.position.y), new Vector2(var_b.transform.position.x, var_b.transform.position.y));
        float threshold = 0.05f;

        if (dist > (variable_selection_radius*2) + threshold) {
            // no overlap
            return false;
        }

        // we got overlap
        return true;
    }
}


//okay, gotta make functions for the seperate prefab set up that we want, make a public
//function for each, call when button is clicked, second drop down easy to make, just duplicate, 
//rename buttons as needed - learn to not make it draw circles when interacting with drop downs
//just do raycast hit??