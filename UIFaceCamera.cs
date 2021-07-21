using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFaceCamera : MonoBehaviour 
{
	/**
	 * UI Face Camera: Script for the data information UI to face the camera always
	 **/ 

	// Instance variable to store the scene camera
	private Camera cam;

	// Starts first
	void Start () 
	{
		// Find the camera in the scene
		cam = FindObjectOfType<Camera> ();
	}
	
	// Called once per frame
	void Update () 
	{
		// Make the transform of the UI element 
		// to which this script attached to face the camera's position
		transform.LookAt (cam.transform.position);

		// Align the rotation of the UI element 
		// to which this script attached to the camera's rotation to avoid inverted text
		transform.rotation = cam.transform.rotation;
	}
}
