using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour {
	
	[Header("Enemy Spawn Management")]
	public float respawnDuration = 5.0f;
	public List<GameObject> spawnPoints = new List<GameObject>();
	public GameObject target;
	
	[Header("Enemy Status")]
	public float startHealth = 100f;
	public float startMoveSpeed = 1f;
	public float startDamage = 15f;
	public int startEXP = 3;
	public int growthEXP = 3;
	public int startFund = 5;
	public int growthFund = 5;

	public float upgradeDuration = 60f;	// Increase all enemy stats every 30 seconds

	private float upgradeTimer;
	[SerializeField]
	private float currentHealth;
	[SerializeField]
	private float currentMoveSpeed;
	[SerializeField]
	private float currentDamage;
	[SerializeField]
	private int currentEXP;
	[SerializeField]
	private int currentFund;

	private GameManager gameManager = null;
	
	
	private float spawnTimer;

	private PrefabManager prefabManager;
	private List<GameObject> enemies = new List<GameObject>();

	void Start() {
		currentHealth = startHealth;
		currentMoveSpeed = startMoveSpeed;
		currentDamage = startDamage;
		currentEXP = startEXP;
		currentFund = startFund;

		prefabManager = PrefabManager.GetInstance();
		enemies.Add(prefabManager.GetPrefab("Zombie"));

		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}

	void Update() {
		if(spawnTimer < respawnDuration) {
			spawnTimer += Time.deltaTime;
		}
		else {
			SpawnEnemy();
		}

		if(upgradeTimer < upgradeDuration) {
			upgradeTimer += Time.deltaTime;
		}
		else {
			UpgradeEnemy();
		}
	}

	float GetDistanceFrom(Vector3 src, Vector3 dist) {
		return Vector3.Distance(src, dist);
	}

	// GameObject getClosestPlayer(Transform spawnPoint) {
	// 	float minDist = 10000000f;
	// 	GameObject closestTarget = null;
	// 	List<GameObject> players = networkManager.Players;

	// 	foreach(GameObject player in players) {
	// 		float dist = GetDistanceFrom(spawnPoint.position, player.transform.position);
			
	// 		if(dist < minDist) {
	// 			minDist = dist;
	// 			closestTarget = player;
	// 		}
	// 	}

	// 	return closestTarget;
	// }

	void SpawnEnemy() {
		if(spawnTimer < respawnDuration) return;

		foreach(GameObject spawnPoint in spawnPoints) {
			GameObject zombie = enemies[0];
			zombie.GetComponent<Chasing>().target = target;
			zombie.GetComponent<Chasing>().damage = currentDamage;
			zombie.GetComponent<NavMeshAgent>().speed = currentMoveSpeed;
			zombie.GetComponent<HealthManager>().SetHealth(currentHealth);
			zombie.GetComponent<KillReward>().exp = currentEXP;
			zombie.GetComponent<KillReward>().fund = currentFund;

			// Boost rotating speed
			float rotateSpeed = 120f + currentMoveSpeed;
			rotateSpeed = Mathf.Max(rotateSpeed, 200f);	// Max 200f
			zombie.GetComponent<NavMeshAgent>().angularSpeed = rotateSpeed;

			// PhotonNetwork.Instantiate("Zombie", spawnPoint.transform.position, spawnPoint.transform.rotation, 0);
			Instantiate(zombie, spawnPoint.transform.position, spawnPoint.transform.rotation);
		}
		
		spawnTimer = 0f;
	}

	void UpgradeEnemy() {
		print("ENEMY UPGRADED");

		currentHealth += 5;

		if(currentMoveSpeed < 4f) {
			currentMoveSpeed += 0.2f;
		}
		if(currentDamage < 51f) {
			currentDamage += 2f;
		}
		
		currentEXP += growthEXP;
		currentFund += growthFund;

		upgradeTimer = 0;
	}
}
