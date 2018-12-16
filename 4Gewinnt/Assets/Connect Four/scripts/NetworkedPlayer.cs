using UnityEngine;
using System.Collections;

// For use with Photon and SteamVR
public class NetworkedPlayer : Photon.MonoBehaviour
{
    public GameObject avatar;

    public Transform playerGlobal;
    public Transform playerLocal;
    private GameController gamecontroller;

    void Start()
    {
            GameObject fourWins = GameObject.FindGameObjectWithTag("4wins");
            this.gamecontroller = fourWins.GetComponent<GameController>();
            Debug.Log("Player instantiated");

        if (photonView.isMine)
        {
            Debug.Log("Player is mine");

            playerGlobal = GameObject.Find("PlayerController").transform;
            playerLocal = playerGlobal.Find("[VRTK_SDKManager]/SDKSetups/OculusVR/OVRCameraRig/TrackingSpace/CenterEyeAnchor");

            this.transform.SetParent(playerLocal);
            this.transform.localPosition = Vector3.zero;

            avatar.SetActive(false);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(playerGlobal.position);
            stream.SendNext(playerGlobal.rotation);
            stream.SendNext(playerLocal.localPosition);
            stream.SendNext(playerLocal.localRotation);
            if (gamecontroller.isMyTurn())
            {
                Debug.Log("writing");
                stream.SendNext(gamecontroller.getCoinPos());
                stream.SendNext(gamecontroller.isDropping);
            }
         
        }
        else
        {
            this.transform.position = (Vector3)stream.ReceiveNext();
            this.transform.rotation = (Quaternion)stream.ReceiveNext();
            avatar.transform.localPosition = (Vector3)stream.ReceiveNext();
            avatar.transform.localRotation = (Quaternion)stream.ReceiveNext();
            if(stream.Count == 9)
            {
                Debug.Log("reading");
                gamecontroller.coinMove((Vector3)stream.ReceiveNext());
                if ((bool)stream.ReceiveNext())
                {
                    gamecontroller.isDropping = true;
                    gamecontroller.DropCoin();
                }
            }
        }
    }
}