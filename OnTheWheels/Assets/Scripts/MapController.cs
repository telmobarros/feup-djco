﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

	// ========================================================================================
	// Cars

    public class Car
    {
        public Sprite sprite;
        public float acceleration;
        public float grassModifier;
        public float dirtModifier;
        public float maxNitroTank;
        public float maxLifePoints;
        public float resistance;

        public Car(Sprite sprite, float acceleration, float grassModifier, float dirtModifier, float maxNitroTank, float maxLifePoints, float resistance)
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
	public static Car policeCar;
    public static bool CopHumanController = true;
	static int currentPoliceSprite = 0;
	Sprite[] policeSprites = new Sprite[2];


	class InitialPosition
	{
		public Vector3 Position;
		public Quaternion Rotation;

		public InitialPosition(float x, float y, float angle) {
			Position = new Vector3(x, y, 0);
			Rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
		}
	}

	// starting positions of the player and corresponding possible positions of the cop
    public GameObject Player;
    public GameObject Cop;
	InitialPosition[] playerStartingPositions = new InitialPosition[] {
		new InitialPosition(1976, -4880, 225),
		new InitialPosition(3762, -732,  295),
		new InitialPosition(3795, -5560, 285),
		new InitialPosition(2680, -8430, 20),
		new InitialPosition(1120, -8362, 20)
	};

	InitialPosition[] copStartingPositions = new InitialPosition[] {
		new InitialPosition(2082, -4350, 180),
		new InitialPosition(3655, -1278, 20),
		new InitialPosition(4412, -5428, 232),
		new InitialPosition(2280, -8280, 290),
		new InitialPosition(1340, -8841, 20)
	};
		
	// ========================================================================================
	// Powerups & Cheatsheets

	static string[] powerUpsTypes = {
		"PowerUp",
		"ResistancePowerUp",
		"InstantNitro",
		"InstantSlowDown",
		"Shield",
		"Rocket",
		"Shield",
		"Switch",
		"SwitchOpp"
	};
	static List<Vector3> powerUpsPositions = new List<Vector3>(new Vector3[]{
        new Vector3(2670,  800,  0),
		new Vector3(2200, -230,  0),
		new Vector3(2500, -250,  0),
		new Vector3(4800, -1200, 0),
		new Vector3(4800, -1800, 0),
		new Vector3(4000, -2050, 0),
		new Vector3(2950, -2580, 0),
		new Vector3(1200, -2550, 0),
		new Vector3(1060, -3480, 0),
		new Vector3(2200, -4030, 0),
		new Vector3(3000, -4500, 0),
		new Vector3(4150, -4700, 0),
		new Vector3(4700, -3900, 0),
		new Vector3(5000, -4300, 0),
		new Vector3(4030, -5140, 0),
		new Vector3(4570, -5900, 0),
		new Vector3(2140, -5370, 0),
		new Vector3(200,  -5150, 0),
		new Vector3(230,  -5800, 0),
		new Vector3(1100, -6560, 0),
		new Vector3(1600, -7500, 0),
		new Vector3(1000, -8160, 0),
		new Vector3(1940, -8810, 0),
		new Vector3(2340, -9500, 0),
		new Vector3(3600, -9400, 0),
		new Vector3(2590, -7060, 0),
		new Vector3(3720, -6800, 0),
		new Vector3(2810, -7920, 0),
		new Vector3(3200, -8900, 0),
		new Vector3(3900, -8130, 0)
	});
	GameObject Powerup;
	static List<Vector3> ActivePowerUpsPositions = new List<Vector3>();

	public static int nCheatsheets = 5;
	static Vector3[] cheatsheetsPositions = {
		new Vector3(1600, -8000, 0),
		new Vector3(2000, -5750, 0),
		new Vector3(3560, -5540, 0),
		new Vector3(2100, -4070, 0),
		new Vector3(1200, -2900, 0),
		new Vector3(4220, -2160, 0),
		new Vector3(4370, -1020, 0),
		new Vector3(2330, -100,  0),
		new Vector3(2130, -1700, 0),
		new Vector3(4190, -7300, 0)
	};
	GameObject Cheatsheet;
	List<Vector3> ActiveCheatsheetsPositions = new List<Vector3> ();


	// ========================================================================================
	// End

	private static GameObject End;
	private static Vector3 EndPosition = new Vector3(2872, -3390, 0);
	public static void SpawnEnd () {
		End = GameObject.Instantiate(Resources.Load("End") as GameObject);
		End.transform.position = EndPosition;
	}

	// ========================================================================================
	// Counters

	public static float timeCounter = 0;
	public static float spriteCounter = 0;

    
	// ========================================================================================
	// Scene

    void Start ()
    {
		// Car Initialization
		policeSprites[0] = Resources.Load<Sprite> ("Cars/Police1");
		policeSprites[1] = Resources.Load<Sprite> ("Cars/Police2");
		int scenarioIndex = Random.Range (0, playerStartingPositions.Length);
		Cop.transform.position = copStartingPositions[scenarioIndex].Position;
		Cop.transform.rotation = copStartingPositions[scenarioIndex].Rotation;
		Cop.GetComponent<CarController>().playerControlled = MapController.CopHumanController;
		Cop.GetComponent<CarController>().throttleKey = KeyCode.UpArrow;
		Cop.GetComponent<CarController>().brakeKey = KeyCode.DownArrow;
		Cop.GetComponent<CarController>().leftKey = KeyCode.LeftArrow;
		Cop.GetComponent<CarController>().rightKey = KeyCode.RightArrow;
		Cop.GetComponent<CarController>().handbrakeKey = KeyCode.RightControl;
		Cop.GetComponent<CarController>().nitroKey = KeyCode.Space;
		Cop.GetComponent<CarController>().GetComponent<SpriteRenderer>().sprite = policeSprites[currentPoliceSprite];
		Cop.GetComponent<CarController>().acceleration = policeCar.acceleration;
		Cop.GetComponent<CarController>().grassModifier = policeCar.grassModifier;
		Cop.GetComponent<CarController>().dirtModifier = policeCar.dirtModifier;
		Cop.GetComponent<CarController>().maxNitroTank = policeCar.maxNitroTank;
		Cop.GetComponent<CarController>().maxLifePoints = policeCar.maxLifePoints;
		Cop.GetComponent<CarController>().resistance = policeCar.resistance;
		Cop.AddComponent<PolygonCollider2D> ();
        Cop.tag = "Cop";
        Cop.name = "Cop";

		Player.transform.position = playerStartingPositions[scenarioIndex].Position;
		Player.transform.rotation = playerStartingPositions[scenarioIndex].Rotation;
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
		Player.AddComponent<PolygonCollider2D> ();
        Player.tag = "Player";
        Player.name = "Player";


		// Camera Assignment

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


		// Powerups
		for (int i = 0; i < powerUpsPositions.Count; i++) {
			string typeName = powerUpsTypes[Random.Range (0, powerUpsTypes.Length)];
			typeName = "Rocket";
			Powerup = GameObject.Instantiate(Resources.Load(typeName) as GameObject);
			Powerup.AddComponent<Rotation> ();
			Powerup.transform.position = powerUpsPositions[i];
			ActivePowerUpsPositions.Add(powerUpsPositions[i]);
		}


		// Cheatsheet placement
		List<Vector3> pos = new List<Vector3>(cheatsheetsPositions);
		for (int i = 0; i < nCheatsheets; i++) {
			int index = Random.Range (0, pos.Count);
			Cheatsheet = GameObject.Instantiate(Resources.Load("Cheatsheet") as GameObject);
			Cheatsheet.AddComponent<Rotation> ();
			Cheatsheet.transform.position = pos[index];
			ActiveCheatsheetsPositions.Add(pos[index]);
			pos.RemoveAt (index);
		}
    }


	void Update () {
		timeCounter += Time.deltaTime;
		spriteCounter += Time.deltaTime;
	}


	void LateUpdate() {
		if (spriteCounter > 0.3) {
			currentPoliceSprite = (currentPoliceSprite + 1) % policeSprites.Length;
			Cop.GetComponent<CarController>().GetComponent<SpriteRenderer>().sprite = policeSprites[currentPoliceSprite];
			spriteCounter = 0;
		}
	}
}
