using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	[SerializeField] string version = "v0.0.0";
	[SerializeField] string roomName = "empty-room";
	[SerializeField] string playerName = "Player";
	[SerializeField] List<GameObject> players = new List<GameObject>();
	public GameObject player;
	public Transform spawnPoint;
	public GameObject enemySpawner;
	public GameObject lobbyCam;
	public GameObject lobbyUI;
	public GameObject inGameUI;

	void Start() {
		lobbyCam.SetActive(false);
		lobbyUI.SetActive(false);

		GameObject playerObj = Instantiate(player, spawnPoint.position, spawnPoint.rotation);

		inGameUI.SetActive(true);
		enemySpawner.SetActive(true);
		enemySpawner.GetComponent<EnemySpawner>().target = playerObj;
	}

}
