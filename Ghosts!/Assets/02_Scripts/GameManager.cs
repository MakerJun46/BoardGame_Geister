using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using UnityEngine.UI;

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
    public GameObject Opposite_Turn_Fire;

    public bool isSorted;
    public bool isSettingAComplete;
    public bool isSettingBComplete;
    public bool isPlayerTurn;
    public bool isGameStarted;
    public bool isPlayerWin;

    public string PlayerCode;

    public Text GameOverText;

    void Start()
    {
        Screen.SetResolution(800, 600, false);

        isSorted = false;
        isSettingAComplete = false;
        isSettingBComplete = false;
        isGameStarted = false;
        isPlayerWin = false;

        for (int i = 0; i < Board_Cells.Count; i++)
        {
            Board_Cells[i].cell_Index = i;
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && isSettingAComplete && isSettingBComplete && !isGameStarted)
        {
            isGameStarted = true;
            PV.RPC("Turn_Fire_On", RpcTarget.All);
            Debug.Log("GameStart");
        }
    }

    public void Loading()
    {
        Debug.Log("Loading..");
        Transform tmp = Board_Cells[0].transform;
        for (int i = 1; i < Board_Cells.Count; i++)
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

        for (int i = 0; i < 4; i++)
        {
            GameObject good = PhotonNetwork.Instantiate("Ghost_good", PlayerA_Start_Positions[index].transform.position, Quaternion.identity);
            GameObject bad = PhotonNetwork.Instantiate("Ghost_bad", PlayerA_Start_Positions[index + 1].transform.position, Quaternion.identity);

            PV.RPC("Player_A_Ghosts_Set_Parent", RpcTarget.All, good.GetComponent<PhotonView>().ViewID);
            PV.RPC("Player_A_Ghosts_Set_Parent", RpcTarget.All, bad.GetComponent<PhotonView>().ViewID);

            PV.RPC("GhostSetCell", RpcTarget.All, good.GetComponent<PhotonView>().ViewID, PlayerA_Start_Positions[index].GetComponent<Cell>().cell_Index);
            PV.RPC("GhostSetCell", RpcTarget.All, bad.GetComponent<PhotonView>().ViewID, PlayerA_Start_Positions[index + 1].GetComponent<Cell>().cell_Index);

            index += 2;
        }

        isPlayerTurn = true;

        PlayerTurn_Fire = GameObject.Find("PlayerA_Turn_Candle").transform.GetChild(5).gameObject;
        Opposite_Turn_Fire = GameObject.Find("PlayerB_Turn_Candle").transform.GetChild(5).gameObject;

        GameObject.Find("MainCamera_B").gameObject.SetActive(false);

        PV.RPC("Setting_A_Complete", RpcTarget.All);
    }

    public void Set_Player_B()
    {
        PlayerCode = "B";

        random = new System.Random();
        int randomIndex = random.Next(0, 8);

        int index = 0;

        for (int i = 0; i < 4; i++)
        {
            GameObject good = PhotonNetwork.Instantiate("Ghost_good", PlayerB_Start_Positions[index].transform.position, Quaternion.Euler(0, 180, 0));
            GameObject bad = PhotonNetwork.Instantiate("Ghost_bad", PlayerB_Start_Positions[index + 1].transform.position, Quaternion.Euler(0, 180, 0));

            PV.RPC("Player_B_Ghosts_Set_Parent", RpcTarget.All, good.GetComponent<PhotonView>().ViewID);
            PV.RPC("Player_B_Ghosts_Set_Parent", RpcTarget.All, bad.GetComponent<PhotonView>().ViewID);

            PV.RPC("GhostSetCell", RpcTarget.All, good.GetComponent<PhotonView>().ViewID, PlayerB_Start_Positions[index].GetComponent<Cell>().cell_Index);
            PV.RPC("GhostSetCell", RpcTarget.All, bad.GetComponent<PhotonView>().ViewID, PlayerB_Start_Positions[index + 1].GetComponent<Cell>().cell_Index);

            index += 2;
        }

        isPlayerTurn = false;

        PlayerTurn_Fire = GameObject.Find("PlayerB_Turn_Candle").transform.GetChild(5).gameObject;
        Opposite_Turn_Fire = GameObject.Find("PlayerA_Turn_Candle").transform.GetChild(5).gameObject;

        GameObject.Find("MainCamera_A").gameObject.SetActive(false);

        PV.RPC("Setting_B_Complete", RpcTarget.All);
    }

    [PunRPC]
    public void Change_Turn()
    {
        isPlayerTurn = !isPlayerTurn;

        Turn_Fire_On();
    }
    [PunRPC]
    public void Turn_Fire_On()
    {
        if (isPlayerTurn)
        {
            PlayerTurn_Fire.SetActive(true);
            Opposite_Turn_Fire.SetActive(false);
        }
        else
        {
            Opposite_Turn_Fire.SetActive(true);
            PlayerTurn_Fire.SetActive(false);
        }
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
    public void GhostSetCell(int ViewID, int cell_index)
    {
        GameObject go = PhotonView.Find(ViewID).gameObject;

        go.GetComponent<Ghost>().cell_Index = cell_index;

        Board_Cells[cell_index].this_Cell_Ghost = go;

        Debug.Log(go + "ÀÇ cell index º¯°æ : " + cell_index);
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

    [PunRPC]
    public void GameOver()
    {
        GameOverText.text = "GameOver!!";
        Text winner = GameOverText.transform.GetChild(0).GetComponent<Text>();

        if (PhotonNetwork.IsMasterClient && isPlayerWin)
            winner.text = "PlayerA Win!!";
        else if (!PhotonNetwork.IsMasterClient && isPlayerWin)
            winner.text = "PlayerB Win!!";

        else
        {
            winner.text = "Bug";
        }
    }
}
