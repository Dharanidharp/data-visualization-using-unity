using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
	/**
	 * Camera Controller - Holder object holds the camera and is used to move around the scene
	*/

	// Instance variables for camera properties 
	private float speed = 20f;
	private float sensitivity = 3f;

	// Instance variable to store the scene camera
	private Camera cam;

	// Instance variable to store the scene camera holder game object
	private GameObject camHolder;
	// Rigidbody component attached to the holder object for camera movement
	private Rigidbody rb;

	// Instance of the visualize data script
	private VisualizeData vd;

	// Initialising the initial movement vector values to zero
	private Vector3 velocity = Vector3.zero;
	private Vector3 rotationH = Vector3.zero;
	private Vector3 rotationV = Vector3.zero;

	// Use this for initialization
	void Start () 
	{
		// Get the Rigidbody component of the camera holder
		rb = GetComponent<Rigidbody> ();
		// Get the VisualizeData script
		vd = FindObjectOfType<VisualizeData> ();
		// Get the scene camera
		cam = FindObjectOfType<Camera>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Get keyboard inputs
		float xMovement = Input.GetAxisRaw ("Horizontal");
		float zMovement = Input.GetAxisRaw ("Vertical");
		float yMovement = Input.GetAxisRaw ("Height");

		// Use the movement axes and multiply them with the transform axes
		Vector3 moveHorizontal = transform.right * xMovement;
		Vector3 moveVertical = transform.forward * zMovement;
		Vector3 moveUp = transform.up * yMovement;

		// Applies velocity along the x,z,y of the holder object
		Vector3 _velocity = (moveHorizontal + moveVertical + moveUp).normalized * speed;

		// Causes movement along x,z,y when input has been made
		Move (_velocity);

		// Camera rotation - horizontal delta - left, right
		float yRotation = Input.GetAxisRaw("Mouse X");
		Vector3 _HRotation = new Vector3 (0, yRotation, 0) * sensitivity;
		RotateH (_HRotation);

		// Camera rotation - vertical delta - up, down
		float xRotation = Input.GetAxisRaw("Mouse Y");
		Vector3 _VRotataion = new Vector3 (xRotation, 0, 0) * sensitivity;
		RotateV (_VRotataion);
	}

	// Called once per physics update
	void FixedUpdate()
	{
		// Enable Camera Rotation only when the data is instantiated
		if (vd.isMODataInstantiated () || vd.isUODataInstantiated()) 
		{
			DoMovement ();
			DoRotation ();
		} else 
		{
			return;
		}
	}

	// Move method - Applies velocity along x,z,y of camera holder
	public void Move(Vector3 _velocity)
	{
		velocity = _velocity;
	}

	// Rotate horizontal method - Applies rotation along y-axis for left, right camera rotation
	public void RotateH(Vector3 _rotation)
	{
		rotationH = _rotation;
	}

	// Rotate vertical method - Applies rotation along x-axis for up, down camera rotation
	public void RotateV(Vector3 _rotation)
	{
		rotationV = _rotation;
	}

	// Causes movement according to velocity
	void DoMovement()
	{
		if(velocity != Vector3.zero)
		{
			rb.MovePosition (rb.position + velocity * Time.fixedDeltaTime);
		}
	}

	// Causes rotation camera rotation
	void DoRotation()
	{
		// Rotates camera holder along y-axis - left,right 
		rb.MoveRotation (rb.rotation * Quaternion.Euler(rotationH));

		if(cam != null)
		{
			// Rotates the main camera along x-axis - up,down
			cam.transform.Rotate (-rotationV);
		}
	}


	/*
	// Casts a ray from the center of the camera 
	void FireRayCast()
	{
		// Gets collider hit information from the ray
		RaycastHit hit;

		// Ray cast from the centre of the camera
		Ray camRay = cam.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));

		// Store all the Met office and Urban observatory data points
		GameObject[] hitObjectMO = vd.getMOPoints();
		GameObject[] hitObjectUO = vd.getUOPoints();

		// When the end (out) of the ray hits
		if (Physics.Raycast (camRay, out hit)) 
		{
			// Loop through the data points
			for (int i = 0; i < vd.getIndexesLength(); i++) 
			{
				// When the ray hit collider name is same as the met office data point name
				if (hitObjectMO[i] != null && hit.collider.name == hitObjectMO [i].name) 
				{
					// Coroutine to change the data point color
					StartCoroutine (ChangeColor (hitObjectMO [i].GetComponent<MeshRenderer> ().material, 0.5f));
				} 
				// When the ray hit collider name is same as the urban observatory data point name

				else if (hitObjectUO [i] != null && hit.collider.name == hitObjectUO [i].name) 
				{
					// Coroutine to change the data point color
					StartCoroutine (ChangeColor (hitObjectUO [i].GetComponent<MeshRenderer> ().material, 0.5f));
				}
			}
		}
	}
	*/

	/** Coroutine for hiding the canvas UI
	IEnumerator HideCanvas(GameObject c, float delay)
	{
		// wait for delay time
		yield return new WaitForSeconds (delay);

		if(c == null)
		{
			yield break;
		} else {
			// set the game object to false
			c.gameObject.SetActive (false);
		}
	}
	**/

	/*
	// Coroutine for changing the color of the data point
	IEnumerator ChangeColor(Material m, float delay)
	{
		// set color to red when the ray cast hits the data point
		m.SetColor ("_Color", Color.red);
		// wait for the delay time
		yield return new WaitForSeconds (delay);
		// set the color back to grey after the delay time
		m.SetColor ("_Color", Color.grey);
	}
	*/
}
