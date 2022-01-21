using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    // Start is called before the first frame update
    void Start()
    {
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect to Master");
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방에 입장");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("new player : " + newPlayer.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("GameSet", RpcTarget.All);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Connect()
    {
        Debug.Log("접속 시도");
        PhotonNetwork.ConnectUsingSettings();
    }

    [PunRPC]
    public void GameSet()
    {
        if (PhotonNetwork.IsMasterClient) // player A
        {
            Debug.Log("마스터 클라이언트라서 플레이어 A");
            GameManager.instance.Set_Player_A();
        }
        else    // player B
        {
            Debug.Log("마스터 클라이언트 아니라서 플레이어 B");
            GameManager.instance.Set_Player_B();
        }
    }
}
