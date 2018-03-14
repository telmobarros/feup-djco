﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

    //create a new type
    public class Car
    {
        public Sprite sprite;
        public float acceleration;
        public float grassModifier;
        public float dirtModifier;
        public float maxNitroTank;
        public float maxLifePoints;
        public float resistance;

        //define a constructor for the class
        public Car(Sprite sprite, float acceleration,float grassModifier, float dirtModifier, float maxNitroTank, float maxLifePoints, float resistance)
        {
            this.sprite = sprite;
            this.acceleration = acceleration;
            this.grassModifier = grassModifier;
            this.dirtModifier = dirtModifier;
            this.maxNitroTank = maxNitroTank;
            this.maxLifePoints = maxLifePoints;
            this.resistance = resistance;
        }
    }

    public static Car selectedCar;

    public static bool CopHumanController = true;

	// starting positions of the player and corresponding possible positions of the cop
	Dictionary<Vector3, Vector3[]> startingPositions = new Dictionary<Vector3, Vector3[]>() {
		{new Vector3(3857, -3716, 0), new Vector3[]{new Vector3(3857, -3760, 0), new Vector3(3857, -3790, 0)}},
		{new Vector3(3857, -3640, 0), new Vector3[]{new Vector3(3857, -3715, 0)}},
	};
    public GameObject Player;
    public GameObject Cop;


	public static int nPowerUps = 3;
	static string[] powerUpsTypes = {
		"PowerUp",
		"ResistancePowerUp",
		"InstantNitro",
		"InstantSlowDown",
		"Shield"
	};
	static List<Vector3> powerUpsPositions = new List<Vector3>(new Vector3[]{
        new Vector3(3900, -3115, 0),
		new Vector3(3945, -2625, 0),
		new Vector3(3800, -3115, 0),
		new Vector3(4000, -2625, 0),
	});

	static List<Vector3> activePowerUpsPositions = new List<Vector3>();
	static List<Vector3> usedPowerUpsPositions = new List<Vector3>();

	public static int nCheatsheets = 3;

    static Vector3[] cheatsheetsPositions = {
        new Vector3(3930, -3115, 0),
        new Vector3(3970, -2625, 0),
        new Vector3(3970, -2660, 0),
    };
    GameObject Cheatsheet;

	public static float timeCounter = 0;

    // Use this for initialization
    void Start ()
    {
		List<Vector3> keys = new List<Vector3>(startingPositions.Keys);
		Vector3 startingPlayerPosition = keys[Random.Range (0, keys.Count)]; // random position index of the player
		int rCopIndex = Random.Range (0, startingPositions[startingPlayerPosition].Length); // random position index of the cop
        //TODO initialize cop with AI or controlled
		Cop.transform.position = startingPositions[startingPlayerPosition][rCopIndex];
		Cop.GetComponent<CarController>().playerControlled = MapController.CopHumanController;
		Cop.GetComponent<CarController>().throttleKey = KeyCode.UpArrow;
		Cop.GetComponent<CarController>().brakeKey = KeyCode.DownArrow;
		Cop.GetComponent<CarController>().leftKey = KeyCode.LeftArrow;
		Cop.GetComponent<CarController>().rightKey = KeyCode.RightArrow;
		Cop.GetComponent<CarController>().handbrakeKey = KeyCode.RightControl;
		Cop.GetComponent<CarController>().nitroKey = KeyCode.Space;
        Cop.tag = "Cop";
        Cop.name = "Cop";

        //TODO initialize player with choosen car attributes
		Player.transform.position = startingPlayerPosition;
		Player.GetComponent<CarController>().throttleKey = KeyCode.W;
		Player.GetComponent<CarController>().brakeKey = KeyCode.S;
		Player.GetComponent<CarController>().leftKey = KeyCode.A;
		Player.GetComponent<CarController>().rightKey = KeyCode.D;
		Player.GetComponent<CarController>().handbrakeKey = KeyCode.LeftShift;
		Player.GetComponent<CarController>().nitroKey = KeyCode.LeftControl;
        Player.GetComponent<CarController>().GetComponent<SpriteRenderer>().sprite = selectedCar.sprite;
        Player.GetComponent<CarController>().acceleration = selectedCar.acceleration;
        Player.GetComponent<CarController>().grassModifier = selectedCar.grassModifier;
        Player.GetComponent<CarController>().dirtModifier = selectedCar.dirtModifier;
        Player.GetComponent<CarController>().maxNitroTank = selectedCar.maxNitroTank;
        Player.GetComponent<CarController>().maxLifePoints = selectedCar.maxLifePoints;
        Player.GetComponent<CarController>().resistance = selectedCar.resistance;
        Player.tag = "Player";
        Player.name = "Player";

        // assign camera to player car
        GameObject.FindGameObjectWithTag("Camera1").GetComponent<CameraController>().target = Player.transform;
		GameObject.FindGameObjectWithTag("MiniMapCamera1").GetComponent<CameraController>().target = Player.transform;

        // assign main camera to player cop
		if (Cop.GetComponent<CarController> ().playerControlled) {
			GameObject.FindGameObjectWithTag ("Camera1").GetComponent<Camera> ().rect = new Rect (0f, 0f, 0.5f, 1f);
			GameObject.FindGameObjectWithTag ("Camera2").GetComponent<CameraController> ().target = Cop.transform;
			GameObject.FindGameObjectWithTag ("Camera2").GetComponent<Camera> ().rect = new Rect (0.5f, 0f, 0.5f, 1f);
			GameObject.FindGameObjectWithTag("MiniMapCamera2").GetComponent<CameraController>().target = Cop.transform;
		} else {
			// Disables Camera 2
			GameObject.FindGameObjectWithTag ("Camera2").GetComponent<CameraController> ().gameObject.SetActive (false);
			GameObject.FindGameObjectWithTag ("HUD2").GetComponent<HUDController> ().gameObject.SetActive (false);
			GameObject.FindGameObjectWithTag ("MiniMapCamera2").GetComponent<CameraController> ().gameObject.SetActive (false);
		}

		// POWER UPS
		for (var i = 0; i < nPowerUps; i++) {
			int r1 = Random.Range (0, powerUpsPositions.Count); // random position
			int r2 = Random.Range (0, powerUpsTypes.Length); // random power up type
			Vector3 powerUpPosition = powerUpsPositions[r1]; // pick one position from the list
			string powerUpType = powerUpsTypes[r2]; // pick one power up type
			usedPowerUpsPositions.Add(powerUpPosition); // add to used positions
			powerUpsPositions.Remove(powerUpPosition); // remove from positions

			GameObject PowerUp = GameObject.Instantiate(Resources.Load(powerUpType) as GameObject);

			PowerUp.transform.position = powerUpPosition;
		}

        foreach (Vector3 cheatsheetPosition in cheatsheetsPositions)
        {
            // These should be pooled and re-used
            Cheatsheet = GameObject.Instantiate(Resources.Load("Cheatsheet") as GameObject);

            Cheatsheet.transform.position = cheatsheetPosition;
        }
    }
	
	// Update is called once per frame
	void Update () {
		timeCounter += Time.deltaTime;
	}

	public static void PlaceNewPowerUp(Vector3 position){
		usedPowerUpsPositions.Remove(position); // remove from used positions

		int r1 = Random.Range (0, powerUpsPositions.Count); // random position
		int r2 = Random.Range (0, powerUpsTypes.Length); // random power up type
		Vector3 powerUpPosition = powerUpsPositions[r1]; // pick one position from the list
		string powerUpType = powerUpsTypes[r2]; // pick one power up type
		usedPowerUpsPositions.Add(powerUpPosition); // add to used positions
		powerUpsPositions.Remove(powerUpPosition); // remove from positions

		GameObject PowerUp = GameObject.Instantiate(Resources.Load(powerUpType) as GameObject);
		PowerUp.transform.position = powerUpPosition;

		powerUpsPositions.Add(position); // after all add back to positions
	}
}
