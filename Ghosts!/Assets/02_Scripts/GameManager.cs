using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PhotonView PV;
    
    private void Awake()
    {
        instance = this;
    }

    System.Random random;

    public List<GameObject> PlayerA_Ghosts;
    public List<GameObject> PlayerB_Ghosts;

    public List<Cell> PlayerA_Start_Positions;
    public List<Cell> PlayerB_Start_Positions;

    public List<Cell> Board_Cells;

    public GameObject TilePosition_Parent;
    public GameObject PlayerTurn_Fire;

    public bool isSorted;
    public bool isSettingAComplete;
    public bool isSettingBComplete;
    public bool isPlayerTurn;
    public bool isGameStarted;

    public string PlayerCode;

    void Start()
    {
        Screen.SetResolution(1600, 900, false);

        isSorted = false;
        isSettingAComplete = false;
        isSettingBComplete = false;
        isGameStarted = false;
    }

    void Update()
    {        
        if(PhotonNetwork.IsMasterClient && isSettingAComplete && isSettingBComplete && !isGameStarted)
        {
            isGameStarted = true;
            Debug.LogError("GameStart");
        }
    }

    public void Loading()
    {
        Debug.Log("Loading..");
        Transform tmp = Board_Cells[0].transform;
        for(int i = 1; i < Board_Cells.Count; i++)
        {
            if (tmp.position == Board_Cells[i].transform.position)
                return;
        }

        for (int i = 0; i < Board_Cells.Count; i++)
        {
            Board_Cells[i].cell_Index = i;
        }

        isSorted = true;
    }

    public void Set_Player_A()
    {
        PlayerCode = "A";

        random = new System.Random();
        int randomIndex = random.Next(0, 8);

        int index = 0;

        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject good = PhotonNetwork.Instantiate("Ghost_good", PlayerA_Start_Positions[index].transform.position, Quaternion.identity);
                GameObject bad = PhotonNetwork.Instantiate("Ghost_bad", PlayerA_Start_Positions[index + 1].transform.position, Quaternion.identity);

                PV.RPC("Player_A_Ghosts_Set_Parent", RpcTarget.All, good.GetComponent<PhotonView>().ViewID);
                PV.RPC("Player_A_Ghosts_Set_Parent", RpcTarget.All, bad.GetComponent<PhotonView>().ViewID);

                int[] parameters_good = { good.GetComponent<PhotonView>().ViewID, PlayerA_Start_Positions[index].GetComponent<Cell>().cell_Index };
                int[] parameters_bad = { bad.GetComponent<PhotonView>().ViewID, PlayerA_Start_Positions[index + 1].GetComponent<Cell>().cell_Index };
            
                PV.RPC("GhostSetCell", RpcTarget.All, parameters_good);
                //PV.RPC("GhostSetCall", RpcTarget.All, parameters_bad);

                index += 2;
            }
        }

        isPlayerTurn = true;

        PlayerTurn_Fire = GameObject.Find("PlayerA_Turn_Candle").transform.Find("fire").gameObject;

        GameObject.Find("MainCamera_B").gameObject.SetActive(false);

        isSettingAComplete = true;
    }

    public void Set_Player_B()
    {
        PlayerCode = "B";

        random = new System.Random();
        int randomIndex = random.Next(0, 8);

        int index = 0;

        if (!PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject good = PhotonNetwork.Instantiate("Ghost_good", PlayerB_Start_Positions[index].transform.position, Quaternion.Euler(0, 180, 0));
                GameObject bad = PhotonNetwork.Instantiate("Ghost_bad", PlayerB_Start_Positions[index + 1].transform.position, Quaternion.Euler(0, 180, 0));

                PV.RPC("Player_B_Ghosts_Set_Parent", RpcTarget.All, good.GetComponent<PhotonView>().ViewID);
                PV.RPC("Player_B_Ghosts_Set_Parent", RpcTarget.All, bad.GetComponent<PhotonView>().ViewID);

                int[] parameters_good = { good.GetComponent<PhotonView>().ViewID, PlayerB_Start_Positions[index].GetComponent<Cell>().cell_Index };
                int[] parameters_bad = { bad.GetComponent<PhotonView>().ViewID, PlayerB_Start_Positions[index + 1].GetComponent<Cell>().cell_Index };

                //PV.RPC("GhostSetCell", RpcTarget.All, parameters_good);
                //PV.RPC("GhostSetCall", RpcTarget.All, parameters_bad);

                index += 2;
            }
        }

        isPlayerTurn = true;

        PlayerTurn_Fire = GameObject.Find("PlayerB_Turn_Candle").transform.Find("fire").gameObject;

        GameObject.Find("MainCamera_A").gameObject.SetActive(false);

        isSettingBComplete = true;
    }

    [PunRPC]
    public void Change_Turn()
    {
        isPlayerTurn = !isPlayerTurn;

        if(isPlayerTurn)
        {
            PlayerTurn_Fire.SetActive(true);
        }
        else
        {
            PlayerTurn_Fire.SetActive(false);
        }
    }
    [PunRPC]
    public void Turn_Fire_On()
    {
        if(isPlayerTurn)
            PlayerTurn_Fire.SetActive(true);
    }

    [PunRPC]
    public void Player_A_Ghosts_Set_Parent(int ViewID)
    {
        GameObject go = PhotonView.Find(ViewID).gameObject;

        go.transform.parent = GameObject.Find("Player_A_Objects").transform;

        PlayerA_Ghosts.Add(go);
    }

    [PunRPC]
    public void Player_B_Ghosts_Set_Parent(int ViewID)
    {
        GameObject go = PhotonView.Find(ViewID).gameObject;

        go.transform.parent = GameObject.Find("Player_B_Objects").transform;

        PlayerB_Ghosts.Add(go);
    }

    [PunRPC]
    public void GhostSetCell(int[] parameters)
    {
        GameObject go = PhotonView.Find(parameters[0]).gameObject;

        go.GetComponent<Ghost>().cell_Index = parameters[1];
    }
    [PunRPC]
    public void Setting_A_Complete()
    {
        isSettingAComplete = true;
    }

    [PunRPC]
    public void Setting_B_Complete()
    {
        isSettingBComplete = true;
    }
}
