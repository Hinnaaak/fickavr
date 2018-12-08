using UnityEngine;
using System.Collections;

public class NetworkController : MonoBehaviour
{
    string _room = "3D_4Wins";

    void Start()
    {
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
        PhotonNetwork.Instantiate("networkedPlayer", Vector3.zero, Quaternion.identity, 0);
    }
}