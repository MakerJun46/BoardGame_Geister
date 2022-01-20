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
    public bool isSettingComplete;
    public bool isPlayerTurn;

    public string PlayerCode;

    void Start()
    {
        Screen.SetResolution(1600, 900, false);

        isSorted = false;
        isSettingComplete = false;
    }

    void Update()
    {        
        if(!isSorted)
        {
            Loading();
        }
    }

    public void Loading()
    {
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

        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject good = PhotonNetwork.Instantiate("Ghost_good", Vector3.zero, Quaternion.identity);
                GameObject bad = PhotonNetwork.Instantiate("Ghost_bad", Vector3.zero, Quaternion.identity);

                good.GetComponent<Ghost>().PlayerCode = "A";
                bad.GetComponent<Ghost>().PlayerCode = "A";

                good.transform.parent = GameObject.Find("Player_A_Objects").transform;
                bad.transform.parent = GameObject.Find("Player_A_Objects").transform;

                PlayerA_Ghosts.Add(good);
                PlayerA_Ghosts.Add(bad);
            }

            foreach (GameObject go in PlayerA_Ghosts)
            {
                while (PlayerA_Start_Positions[randomIndex].this_Cell_Ghost != null)
                {
                    randomIndex = random.Next(0, 8);
                }

                go.transform.position = PlayerA_Start_Positions[randomIndex].transform.position;
                PlayerA_Start_Positions[randomIndex].this_Cell_Ghost = go;
                go.GetComponent<Ghost>().cell_Index = PlayerA_Start_Positions[randomIndex].GetComponent<Cell>().cell_Index;
            }
        }


        isPlayerTurn = true;

        PlayerTurn_Fire = GameObject.Find("PlayerA_Turn_Candle").transform.Find("fire").gameObject;

        

        isSettingComplete = true;
    }

    public void Set_Player_B()
    {
        PlayerCode = "B";

        random = new System.Random();
        int randomIndex = random.Next(0, 8);

        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject good = PhotonNetwork.Instantiate("Ghost_good", Vector3.zero, Quaternion.Euler(0, 180, 0));
                GameObject bad = PhotonNetwork.Instantiate("Ghost_bad", Vector3.zero, Quaternion.Euler(0, 180, 0));

                good.GetComponent<Ghost>().PlayerCode = "B";
                bad.GetComponent<Ghost>().PlayerCode = "B";

                good.transform.parent = GameObject.Find("Player_B_Objects").transform;
                bad.transform.parent = GameObject.Find("Player_B_Objects").transform;

                PlayerB_Ghosts.Add(good);
                PlayerB_Ghosts.Add(bad);
            }

            foreach (GameObject go in PlayerA_Ghosts)
            {
                while (PlayerB_Start_Positions[randomIndex].this_Cell_Ghost != null)
                {
                    randomIndex = random.Next(0, 8);
                }

                go.transform.position = PlayerB_Start_Positions[randomIndex].transform.position;
                PlayerB_Start_Positions[randomIndex].this_Cell_Ghost = go;
                go.GetComponent<Ghost>().cell_Index = PlayerB_Start_Positions[randomIndex].GetComponent<Cell>().cell_Index;
            }
        }

        isPlayerTurn = false;

        PlayerTurn_Fire = GameObject.Find("PlayerB_Turn_Candle").gameObject;

        isSettingComplete = true;
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
}
