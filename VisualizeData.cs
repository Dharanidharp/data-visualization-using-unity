using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class VisualizeData : MonoBehaviour 
{
	/**
	 * Visualize Data - Main script that handles the data visualization
	 **/ 

	// Instance variables for data point sphere prefabs
	[Header("Data Point Prefabs")] // Unity inspector header
	[SerializeField] // To show the value in inspector
	private GameObject dataPointPrefabMO;
	[SerializeField] // To show the value in inspector
	private GameObject dataPointPrefabUO;

	// Instance variables for data point holders - the prefabs will be instantiated as a child of these game objects
	private GameObject dataPointsHolderMO;
	private GameObject dataPointsHolderUO;

	// Instance variables for all the MO - met office and UO - urban observatory data points
	private GameObject[] MOPoints;
	private GameObject[] UOPoints;

	// Instance variables to store the UI elements - canvas, text components and panels which display the data information
	//private Canvas[] defaultInfoPanels;
	private Text[] defaultTempInfo;
	private Text[] defaultMonthInfo;
	private GameObject quickAccessCanvas;
	private GameObject introCanvas;
	private GameObject axesCanvas;
	private GameObject statusPanel;
	private Text timer;
	private Text startTime;
	private Text endTime;
	private Text statusText;

	private Toggle sortToggle;
	private Toggle zToggle;
	public GameObject checkMark;

	// Booleans to check whether the data has been instantiated
	private bool dataInstantiatedMO = false;
	private bool dataInstantiatedUO = false;
	private bool isDataSorted = false;
	private bool isZDepthActive = false;

	// Store the csv file
	[SerializeField] private string inputFile;

	// Initialise an array of 4 for the number of columns in the csv file
	[SerializeField] private int[] columnNumbers;

	// Instance variable for column names
	[SerializeField] private string[] columnNames;

	// Instance variable for month names
	[SerializeField] private string[] monthNames;

	// Instance variable for indexes
	[SerializeField] private float[] indexes;

	// Instance variable for met office temperatures
	[SerializeField] private float[] temperatureMetOffice;

	// Instance variable for urban observatory temperatures
	[SerializeField] private float[] temperatureUrbanObservatory;

	// Instance variable for data from the CSV file
	[SerializeField] private List<Dictionary<string, object>> dataList;

	// Use this for initialization
	void Start () 
	{
		// Call the initialise method
		InitialiseCSVData ();
	}

	/**
	 * A single method that initialises all the values
	 * Store the respective row values from the csv data file
	 */
	public void InitialiseCSVData()
	{
		// Initialise the prefabs
		dataPointPrefabMO = Resources.Load<GameObject>("prefabs/SphereMO");
		dataPointPrefabUO = Resources.Load<GameObject>("prefabs/SphereUO");

		// Get the parent objects that hold all the respective data points for Met office and Urban Observatory
		dataPointsHolderMO = GameObject.FindGameObjectWithTag ("MODataHolder");
		dataPointsHolderUO = GameObject.FindGameObjectWithTag ("UODataHolder");

		// Initialise Met office data points
		MOPoints = new GameObject[12];
		// Initialise Urban observatory data points
		UOPoints = new GameObject[12];

		// Get the HUD canvas element in the scene and store it
		quickAccessCanvas = GameObject.FindGameObjectWithTag("QuickAccess");
		introCanvas = GameObject.FindGameObjectWithTag ("Intro");
		axesCanvas = GameObject.FindGameObjectWithTag ("AxisInfo");
		statusPanel = GameObject.FindGameObjectWithTag ("Status");
		timer = GameObject.FindGameObjectWithTag ("Timer").GetComponent<Text> ();
		startTime = GameObject.FindGameObjectWithTag ("StartTime").GetComponent<Text> ();
		endTime = GameObject.FindGameObjectWithTag ("EndTime").GetComponent<Text> ();
		statusText = GameObject.FindGameObjectWithTag ("StatusText").GetComponent<Text> ();
		axesCanvas.SetActive (false);
		statusPanel.SetActive (false);

		sortToggle = GameObject.FindGameObjectWithTag ("SortButton").GetComponent<Toggle> ();
		zToggle = GameObject.FindGameObjectWithTag ("ZDepth").GetComponent<Toggle> ();

		sortToggle.isOn = false;
		zToggle.isOn = false;

		// Initialise the array's that store the csv file data
		columnNumbers = new int[4];
		columnNames = new string[4]; // for the 4 column names in the csv file
		monthNames = new string[12]; // for all the month names Jan - Dec
		indexes = new float[12]; // for indexes from 1-12 that correspond to the months
		temperatureMetOffice = new float[12]; // met office temperature for each corresponding month
		temperatureUrbanObservatory = new float[12]; // urban observatory temperature for each corresponding month

		// Initialise the UI elements that display the stored csv data in the scene
		//defaultInfoPanels = new Canvas[12]; // stores the respective canvases of each data point
		defaultTempInfo = new Text[12]; // stores all the temperature information in the form of Text array
		defaultMonthInfo = new Text[12]; // stores all the month names in the form of Text array

		// Initialise the name of the csv input file
		inputFile = "temperatureData";

		// Read the data from the csv file using the CSVFileReader
		dataList = CSVFileReader.Read (inputFile);

		// Store all the columns of the CSV file as a list
		List<string> columnsList = new List<string> (dataList [1].Keys);

		// loop through all the columns in the data file and store the respective column names
		for(int i = 0; i < columnNumbers.Length; i++)
		{
			columnNumbers [i] = i;
			columnNames [i] = columnsList [columnNumbers[i]];
		}

		// Store the respective row values from the csv data file
		for(int i = 0; i < dataList.Count; i++)
		{
			indexes[i] = Convert.ToSingle (dataList[i][columnNames[0]]);
			temperatureMetOffice [i] = Convert.ToSingle(dataList[i][columnNames[1]]);
			temperatureUrbanObservatory [i] = Convert.ToSingle(dataList[i][columnNames[2]]);;
			monthNames[i] = Convert.ToString(dataList [i] [columnNames [3]]);

			MOPoints [i] = Instantiate (dataPointPrefabMO, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
			MOPoints [i].gameObject.SetActive (false);
			// Make this point's transform as a child of the data points holder
			MOPoints [i].transform.SetParent (dataPointsHolderMO.transform, false);

			UOPoints [i] = Instantiate (dataPointPrefabUO, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
			UOPoints [i].gameObject.SetActive (false);
			// Make this point's transform as a child of the data points holder
			UOPoints [i].transform.SetParent (dataPointsHolderUO.transform, false);
		}
	}

	// Handles met office data instantiation
	public void MetOfficeData()
	{
		dataPointsHolderMO.GetComponent<LineRenderer> ().enabled = true;

		if (startTime.text == "") 
		{
			startTime.text = ("Start time: " + Math.Round (Timer ()) + " s");
		} else 
		{
			startTime.text = startTime.text;
		}

		// Hide Introduction information
		introCanvas.SetActive (false);
		axesCanvas.SetActive (true);
		statusText.color = Color.green;
		statusText.text = "Met office data loaded";
		StartCoroutine (ToggleOverlay (statusPanel, 1.5f));

		// if data not instantiated
		if (!dataInstantiatedMO) 
		{
			// loop through all the indexes and create a data point for each index
			for (int i = 0; i < indexes.Length; i++) 
			{
				// if urban observatory data is active when met office data needs to load
				if (dataInstantiatedUO) 
				{
					// loop through all the urban observatory data points
					for(int j = 0; j < indexes.Length; j++)
					{
						// Make all the UO data points inactive
						UOPoints[i].gameObject.SetActive(false);
					}
				}

				// Instantiate a prefab at each index location - X: months(1-12/Jan - Dec), Y: met office temperatures for each month
				//MOPoints [i] = Instantiate (dataPointPrefabMO, new Vector3 (indexes [i], temperatureMetOffice [i], 0), Quaternion.identity) as GameObject;

				MOPoints [i].gameObject.SetActive (true);
				MOPoints [i].transform.position = new Vector3 (indexes [i], temperatureMetOffice [i], 0);

				// Get the line renderer component of each data point and set their positions to the positions of their respective data points in the scene
				MOPoints[i].GetComponent<LineRenderer> ().SetPosition (0, new Vector3 (indexes [i], 0, 0));
				MOPoints[i].GetComponent<LineRenderer> ().SetPosition (1, new Vector3 (indexes [i], temperatureMetOffice [i], 0));

				// set met office to true
				dataInstantiatedMO = true;

				// Store Text components of the children objects whose common parent is the DataPoint transform
				defaultTempInfo [i] = MOPoints [i].transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<Text> ();
				defaultMonthInfo [i] = MOPoints [i].transform.GetChild (0).GetChild (0).GetChild (1).GetComponent<Text> ();

				defaultTempInfo[i].text = Convert.ToString(temperatureMetOffice[i]) + " C";

				defaultMonthInfo [i].gameObject.transform.position = new Vector3 (indexes [i], -0.5f, 0);
				defaultMonthInfo [i].text = monthNames [i];

				// Make this point's transform as a child of the data points holder
				//MOPoints [i].transform.SetParent (dataPointsHolderMO.transform, false);

				// Store the month name 
				string dataPointNameMO = monthNames [i];

				// Change each instantiated points name to their respective month names 
				MOPoints[i].transform.name = dataPointNameMO;

				//Debug.Log ("Month: " + MOPoints [i].name + ", Temperature: " + MOPoints [i].transform.position.y + " C, " + "Source: " + columnNames[1]);
			}

			if (!dataInstantiatedMO) 
			{
				dataPointsHolderMO.GetComponent<LineRenderer> ().startWidth = 0;
				dataPointsHolderMO.GetComponent<LineRenderer> ().endWidth = 0;
			} else 
			{
				// Store the Vector3 positions of the line renderer
				var graphPoints = new Vector3[dataPointsHolderMO.GetComponent<LineRenderer> ().positionCount];

				// Set the graph points values to indexes as x values and temperatures as y values and z = 0
				for (int j = 0; j < graphPoints.Length; j++) 
				{
					graphPoints [j] = new Vector3 (indexes [j], temperatureMetOffice [j], 0);
				}

				// Set the positions of the line renderer to the values of graph points position
				dataPointsHolderMO.GetComponent<LineRenderer> ().SetPositions (graphPoints);
			}

		} else if(dataInstantiatedMO)
		{
			for(int i = 0; i < MOPoints.Length; i++)
			{
				if (MOPoints [i].gameObject.activeInHierarchy) 
				{
					statusText.color = Color.yellow;
					statusText.text = "Warning! Met office data already instantiated";
					StartCoroutine (ToggleOverlay (statusPanel, 3.0f));
				} 
				else 
				{
					MOPoints [i].gameObject.SetActive (true);

					MOPoints [i].gameObject.SetActive (true);
					MOPoints [i].transform.position = new Vector3 (indexes [i], temperatureMetOffice [i], 0);

					// Get the line renderer component of each data point and set their positions to the positions of their respective data points in the scene
					MOPoints[i].GetComponent<LineRenderer> ().SetPosition (0, new Vector3 (indexes [i], 0, 0));
					MOPoints[i].GetComponent<LineRenderer> ().SetPosition (1, new Vector3 (indexes [i], temperatureMetOffice [i], 0));

					// set met office to true
					dataInstantiatedMO = true;

					// Store Text components of the children objects whose common parent is the DataPoint transform
					defaultTempInfo [i] = MOPoints [i].transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<Text> ();
					defaultMonthInfo [i] = MOPoints [i].transform.GetChild (0).GetChild (0).GetChild (1).GetComponent<Text> ();

					defaultTempInfo[i].text = Convert.ToString(temperatureMetOffice[i]) + " C";

					defaultMonthInfo [i].gameObject.transform.position = new Vector3 (indexes [i], -0.5f, 0);
					defaultMonthInfo [i].text = monthNames [i];

					// Make this point's transform as a child of the data points holder
					//MOPoints [i].transform.SetParent (dataPointsHolderMO.transform, false);

					// Store the month name 
					string dataPointNameMO = monthNames [i];

					// Change each instantiated points name to their respective month names 
					MOPoints[i].transform.name = dataPointNameMO;

					// Store the Vector3 positions of the line renderer
					var graphPoints = new Vector3[dataPointsHolderMO.GetComponent<LineRenderer> ().positionCount];

					// Set the graph points values to indexes as x values and temperatures as y values and z = 0
					for (int j = 0; j < graphPoints.Length; j++) 
					{
						graphPoints [j] = new Vector3 (indexes [j], temperatureMetOffice [j], 0);
					}

					// Set the positions of the line renderer to the values of graph points position
					dataPointsHolderMO.GetComponent<LineRenderer> ().SetPositions (graphPoints);

					UOPoints [i].gameObject.SetActive (false);
				}
			}
		}
		//Debug.Log (isMODataSorted);
	}

	// Handles urban observatory data instantiation - same as the met office method with different objects
	public void UrbanObservatoryData()
	{
		dataPointsHolderUO.GetComponent<LineRenderer> ().enabled = true;

		if (startTime.text == "") 
		{
			startTime.text = ("Start time: " + Math.Round (Timer ()) + " s");
		} else 
		{
			startTime.text = startTime.text;
		}

		// Hide Introduction information
		introCanvas.SetActive (false);
		axesCanvas.SetActive (true);
		statusText.color = Color.green;
		statusText.text = "Urban observatory data loaded";
		StartCoroutine (ToggleOverlay (statusPanel, 3.0f));

		if (!dataInstantiatedUO) 
		{
			for (int i = 0; i < indexes.Length; i++) 
			{
				if (dataInstantiatedMO) 
				{
					for(int j = 0; j < indexes.Length; j++)
					{
						
						MOPoints [j].gameObject.SetActive (false);
					}
				}

				//UOPoints [i] = Instantiate (dataPointPrefabUO, new Vector3 (indexes [i], temperatureUrbanObservatory [i], 0), Quaternion.identity) as GameObject;

				UOPoints [i].gameObject.SetActive (true);
				UOPoints [i].transform.position = new Vector3 (indexes [i], temperatureUrbanObservatory [i], 0);

				UOPoints[i].GetComponent<LineRenderer> ().SetPosition (0, new Vector3 (indexes [i], 0, 0));
				UOPoints[i].GetComponent<LineRenderer> ().SetPosition (1, new Vector3 (indexes [i], temperatureUrbanObservatory [i], 0));
				dataInstantiatedUO = true;

				// Store Text components of the children objects whose common parent is the DataPoint transform
				defaultTempInfo [i] = UOPoints [i].transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<Text> ();
				defaultMonthInfo [i] = UOPoints [i].transform.GetChild (0).GetChild (0).GetChild (1).GetComponent<Text> ();

				defaultTempInfo[i].text = Convert.ToString(Math.Round(temperatureUrbanObservatory[i],1)) + " C";

				defaultMonthInfo [i].gameObject.transform.position = new Vector3 (indexes [i], -0.5f, 0);
				defaultMonthInfo [i].text = monthNames [i];

				// Make this point's transform as a child of the data points holder
				//UOPoints [i].transform.SetParent (dataPointsHolderUO.transform, false);

				// Store the month name 
				string dataPointNameUO = monthNames [i];

				// Change each instantiated points name to their respective month names 
				UOPoints[i].transform.name = dataPointNameUO;
			}

			if (!dataInstantiatedUO) 
			{
				dataPointsHolderUO.GetComponent<LineRenderer> ().startWidth = 0;
				dataPointsHolderUO.GetComponent<LineRenderer> ().endWidth = 0;
			} else 
			{
				var graphPoints = new Vector3[dataPointsHolderUO.GetComponent<LineRenderer> ().positionCount];

				for (int j = 0; j < graphPoints.Length; j++) 
				{
					graphPoints [j] = new Vector3 (indexes [j], temperatureUrbanObservatory [j], 0);
				}

				dataPointsHolderUO.GetComponent<LineRenderer> ().SetPositions (graphPoints);
			}

		} else if(dataInstantiatedUO)
		{
			for(int i = 0; i < UOPoints.Length; i++)
			{
				if (UOPoints [i].gameObject.activeInHierarchy) 
				{
					statusText.color = Color.yellow;
					statusText.text = "Warning! Urban observatory data already instantiated";
					StartCoroutine (ToggleOverlay (statusPanel, 3.0f));
				} else 
				{
					UOPoints [i].gameObject.SetActive (true);

					UOPoints [i].gameObject.SetActive (true);
					UOPoints [i].transform.position = new Vector3 (indexes [i], temperatureUrbanObservatory [i], 0);

					UOPoints[i].GetComponent<LineRenderer> ().SetPosition (0, new Vector3 (indexes [i], 0, 0));
					UOPoints[i].GetComponent<LineRenderer> ().SetPosition (1, new Vector3 (indexes [i], temperatureUrbanObservatory [i], 0));
					dataInstantiatedUO = true;

					// Store Text components of the children objects whose common parent is the DataPoint transform
					defaultTempInfo [i] = UOPoints [i].transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<Text> ();
					defaultMonthInfo [i] = UOPoints [i].transform.GetChild (0).GetChild (0).GetChild (1).GetComponent<Text> ();

					defaultTempInfo[i].text = Convert.ToString(Math.Round(temperatureUrbanObservatory[i],1)) + " C";

					defaultMonthInfo [i].gameObject.transform.position = new Vector3 (indexes [i], -0.5f, 0);
					defaultMonthInfo [i].text = monthNames [i];

					// Make this point's transform as a child of the data points holder
					//UOPoints [i].transform.SetParent (dataPointsHolderUO.transform, false);

					// Store the month name 
					string dataPointNameUO = monthNames [i];

					// Change each instantiated points name to their respective month names 
					UOPoints[i].transform.name = dataPointNameUO;

					var graphPoints = new Vector3[dataPointsHolderUO.GetComponent<LineRenderer> ().positionCount];

					for (int j = 0; j < graphPoints.Length; j++) {
						graphPoints [j] = new Vector3 (indexes [j], temperatureUrbanObservatory [j], 0);
					}

					dataPointsHolderUO.GetComponent<LineRenderer> ().SetPositions (graphPoints);

					MOPoints [i].gameObject.SetActive (false);
				}
			}
		}
	}

	// Sorts Met office data
	public void sortDataAsc(GameObject parentObj, GameObject[] obj, float[] values)
	{
		introCanvas.SetActive (false);
		axesCanvas.SetActive (true);

		GameObject newParentObj = parentObj;
		GameObject[] newObj = obj;
		float[] newValues = values;

		int count = 0;
		var monthTempPair = new Dictionary<string, float> ();

		// Loops through the temperature values of met office
		foreach(float i in newValues)
		{
			// Adds the respective month name and its temperature value to the dictionary
			monthTempPair.Add (monthNames [count], i);
			count++;
		}

		// Sort the dictionary in ascending order based on the values (temperature) in the dictionary
		var dictSort = from entry in monthTempPair
		               orderby entry.Value ascending
		               select entry;

		// Set the count to 0
		count = 0;

		// Loop through the dictionary 
		foreach(KeyValuePair<string, float> kvp in dictSort)
		{
			newObj [count].SetActive (true);

			// Set the scatterplot positions to the new values
			newObj [count].transform.position = new Vector3 (indexes [count], kvp.Value, 0);

			// Set the line renderer values to the new values
			newObj [count].GetComponent<LineRenderer> ().SetPosition (0, new Vector3 (indexes [count], 0, 0));
			newObj [count].GetComponent<LineRenderer> ().SetPosition (1, new Vector3 (indexes [count], kvp.Value, 0));

			// Store Text components of the children objects whose common parent is the DataPoint transform
			defaultTempInfo [count] = newObj [count].transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<Text> ();
			defaultMonthInfo [count] = newObj [count].transform.GetChild (0).GetChild (0).GetChild (1).GetComponent<Text> ();

			// Set the temperature text
			defaultTempInfo [count].text = Convert.ToString (Math.Round (kvp.Value, 1)) + " C";

			// Set the position of the months text object
			defaultMonthInfo [count].gameObject.transform.position = new Vector3 (indexes [count], -0.5f, 0);

			//Set the Month text
			defaultMonthInfo [count].text = kvp.Key;

			// Store the month name 
			string dataPointName = kvp.Key;

			// Change each instantiated points name to their respective month names 
			newObj [count].transform.name = dataPointName;
			count++;
		}

		var graphPoints = new Vector3[newParentObj.GetComponent<LineRenderer> ().positionCount];
		count = 0;
		foreach(KeyValuePair<string, float> kvp in dictSort) 
		{
			graphPoints [count] = new Vector3 (indexes [count], kvp.Value, 0);
			count++;
		}

		newParentObj.GetComponent<LineRenderer> ().SetPositions (graphPoints);

		isDataSorted = true;
	}

	public void zDepth(GameObject parentObj, GameObject[] obj, float[] values)
	{
		// New objects and values
		GameObject newParentObj = parentObj;
		GameObject[] newObj = obj;
		float[] newValues = values;

		for(int i = 0; i < newObj.Length; i++)
		{
			// Set the scatterplot values to new values
			newObj [i].transform.position = new Vector3 (indexes [i], newValues[i], newValues[i]);

			// Set the line renderer values to new values
			newObj [i].GetComponent<LineRenderer> ().SetPosition (0, new Vector3 (indexes [i], 0, newValues[i]));
			newObj [i].GetComponent<LineRenderer> ().SetPosition (1, new Vector3 (indexes [i], newValues[i], newValues[i]));

			// Get and Set the month text label position to the new values
			defaultMonthInfo [i] = newObj [i].transform.GetChild (0).GetChild (0).GetChild (1).GetComponent<Text> ();
			defaultMonthInfo [i].gameObject.transform.position = new Vector3 (indexes [i], -0.5f, newValues[i]);
		}

		// Set the line plot values to the new values
		var graphPoints = new Vector3[newParentObj.GetComponent<LineRenderer> ().positionCount];

		for (int j = 0; j < graphPoints.Length; j++) 
		{
			graphPoints [j] = new Vector3 (indexes [j], newValues [j], newValues[j]);
		}

		newParentObj.GetComponent<LineRenderer> ().SetPositions (graphPoints);
		isZDepthActive = true;
	}

	public float Timer()
	{
		float startTime = Time.time;
		float currentTime = 0;

		if (dataInstantiatedMO || dataInstantiatedUO) 
		{
			currentTime = startTime;
		}

		// display the visualization time
		timer.text = ("Visualization Time: " + Math.Round (currentTime) + " s");
		return startTime;
	}
		
	// Stop timer
	public void StopTimer()
	{
		Debug.Log ("End time: " + Timer ());
		endTime.text = ("End time: " + Math.Round (Timer ()) + " s");
		Time.timeScale = 0;
	}

	// Quit
	public void Quit()
	{
		Application.Quit ();
	}

	// Makes quick access menu active or inactive
	public void QuickAccessMenu()
	{
		if (Input.GetKey (KeyCode.LeftShift)) 
		{
			// pause
			Time.timeScale = 0.0f;
			// make the menu visible and active
			quickAccessCanvas.SetActive (true);
		} else 
		{
			// unpause
			Time.timeScale = 1.0f;
			// make the menu invisible and inactive
			quickAccessCanvas.SetActive (false);
		}
	}

	IEnumerator ToggleOverlay(GameObject ob, float delayTime)
	{
		if (ob == null) 
		{
			yield break;
		} else 
		{
			ob.gameObject.SetActive (true);

			// wait for delay time
			yield return new WaitForSeconds (delayTime);

			// set the game object to false
			ob.gameObject.SetActive (false);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		QuickAccessMenu ();
		Timer ();

		if (dataPointsHolderMO.transform.GetChild (0).gameObject.activeInHierarchy && sortToggle.isOn) 
		{
			sortDataAsc (dataPointsHolderMO, MOPoints, temperatureMetOffice);
		} 
		else if (dataPointsHolderUO.transform.GetChild (0).gameObject.activeInHierarchy && sortToggle.isOn) 
		{
			sortDataAsc (dataPointsHolderUO, UOPoints, temperatureUrbanObservatory);
		}

		if (dataPointsHolderMO.transform.GetChild (0).gameObject.activeInHierarchy && zToggle.isOn) 
		{
			zDepth (dataPointsHolderMO, MOPoints, temperatureMetOffice);
		} 
		else if (dataPointsHolderUO.transform.GetChild (0).gameObject.activeInHierarchy && zToggle.isOn) 
		{
			zDepth (dataPointsHolderUO, UOPoints, temperatureUrbanObservatory);
		}

		if (!dataPointsHolderMO.transform.GetChild (0).gameObject.activeInHierarchy) 
		{
			dataPointsHolderMO.GetComponent<LineRenderer> ().enabled = false;
		} 
		if (!dataPointsHolderUO.transform.GetChild (0).gameObject.activeInHierarchy) 
		{
			dataPointsHolderUO.GetComponent<LineRenderer> ().enabled = false;
		}

		if (zToggle.isOn) 
		{
			sortToggle.enabled = false;
			checkMark.SetActive (false);
		} else 
		{
			sortToggle.enabled = true;
			checkMark.SetActive (true);
		}
	}

	public GameObject[] getMOPoints()
	{
		return MOPoints;
	}

	public GameObject[] getUOPoints()
	{
		return UOPoints;
	}

	public GameObject getMetOfficeDataPrefab()
	{
		return dataPointPrefabMO;
	}

	public GameObject getUrbanObservatoryDataPrefab()
	{
		return dataPointPrefabUO;
	}

	public GameObject getMOHolder()
	{
		return dataPointsHolderMO;
	}

	public GameObject getUOHolder()
	{
		return dataPointsHolderUO;
	}

	public Toggle getSortToggle()
	{
		return sortToggle;
	}

	public Toggle getZToggle()
	{
		return zToggle;
	}

	public string[] getMonthNames()
	{
		return monthNames;
	}

	public float[] getMetOfficeTemps()
	{
		return temperatureMetOffice;
	}

	public float[] getUrbanObservatoryTemps()
	{
		return temperatureUrbanObservatory;
	}

	public float[] getIndex()
	{
		return indexes;
	}

	public int getIndexesLength()
	{
		return indexes.Length;
	}

	public bool isMODataInstantiated()
	{
		return dataInstantiatedMO;
	}

	public bool isUODataInstantiated()
	{
		return dataInstantiatedUO;
	}

	public bool isDataSortedSuccessfully()
	{
		return isDataSorted;
	}

	public bool isZDepthApplied()
	{
		return isZDepthActive;
	}
}
