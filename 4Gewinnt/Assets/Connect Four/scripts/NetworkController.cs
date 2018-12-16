using UnityEngine;
using System.Collections;

public class NetworkController : MonoBehaviour
{
    string _room = "3D_4Wins";

    GameController gamecontroller;

    void Start()
    {
        GameObject fourWins = GameObject.FindGameObjectWithTag("4wins");
        this.gamecontroller = fourWins.GetComponent<GameController>();
        bool hi = PhotonNetwork.ConnectUsingSettings("0.1");
        Debug.Log("connecting: " + hi);
    }

    void OnJoinedLobby()
    {
        Debug.Log("joined lobby");

        RoomOptions roomOptions = new RoomOptions() { };
        PhotonNetwork.JoinOrCreateRoom(_room, roomOptions, TypedLobby.Default);
    }

    void OnJoinedRoom()
    {
        Debug.Log("joined Room");
        PhotonPlayer[] playerlist = PhotonNetwork.playerList;
        gamecontroller.setPlayer(playerlist.Length);
        PhotonNetwork.Instantiate("networkedPlayer", Vector3.zero, Quaternion.identity, 0);
    }
}