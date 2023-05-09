using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPun
{
    [Header("Player Vars")]
    public string playerPrefabLocation;
    public PlayerController[] players;
    public Transform[] spawnpoints;
    public int playersAlive;
    public List<Transform> tempSpawns;
    public Color[] playerColors;

    private int playersInGame;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        playersAlive = players.Length;

        foreach (Transform point in spawnpoints)
        {
            tempSpawns.Add(point);
        }

        photonView.RPC("imInGame", RpcTarget.AllBuffered);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    void imInGame()
    {
        playersInGame++;
        if (PhotonNetwork.IsMasterClient && playersInGame == PhotonNetwork.PlayerList.Length)
        {
            photonView.RPC("spawnPlayer", RpcTarget.All);
        }
    }

    public Transform randomSpawnPos()
    {
        int number = Random.Range(0, tempSpawns.Count);
        Transform spawnPoint = tempSpawns[number];
        tempSpawns.Remove(spawnPoint);
        return spawnPoint;
    }

    [PunRPC]
    void spawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, randomSpawnPos().position, Quaternion.identity);
        playerObj.GetComponent<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }
}
