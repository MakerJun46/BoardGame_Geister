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

    public List<GameObject> PlayerA_Catched_Good_Ghosts;
    public List<GameObject> PlayerA_Catched_Bad_Ghosts;
    public List<GameObject> PlayerB_Catched_Good_Ghosts;
    public List<GameObject> PlayerB_Catched_Bad_Ghosts;

    public List<GameObject> PlayerA_Catched_Good_Ghost_Positions;
    public List<GameObject> PlayerA_Catched_Bad_Ghost_Positions;
    public List<GameObject> PlayerB_Catched_Good_Ghost_Positions;
    public List<GameObject> PlayerB_Catched_Bad_Ghost_Positions;

    public List<Cell> PlayerA_Start_Positions;
    public List<Cell> PlayerB_Start_Positions;

    public List<Cell> Board_Cells;

    public GameObject TilePosition_Parent;
    public GameObject PlayerTurn_Fire;
    public GameObject Opposite_Turn_Fire;

    public GameObject PlayerA_UI;
    public GameObject PlayerB_UI;

    public bool isSorted;
    public bool isSettingAComplete;
    public bool isSettingBComplete;
    public bool isPlayerTurn;
    public bool isGameStarted;
    public bool isPlayerWin;

    public string PlayerCode;

    public GameObject GameOverText;
    public List<Material> numbers;

    public MeshRenderer A_GoodGhost_Number;
    public MeshRenderer A_BadGhost_Number; 
    public MeshRenderer B_GoodGhost_Number; 
    public MeshRenderer B_Badhost_Number; 

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
        PlayerA_UI.SetActive(true);
        PlayerB_UI.SetActive(true);

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
        PlayerB_UI.SetActive(true);
        PlayerA_UI.SetActive(true);

        GameObject.Find("MainCamera_A").gameObject.SetActive(false);

        PV.RPC("Setting_B_Complete", RpcTarget.All);
    }

    public void Detect_Winner()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if(PlayerB_Catched_Bad_Ghosts.Count == 4) // A 플레이어가 나쁜 유령 모두 잡힌 경우 : A 승리
            {
                isPlayerWin = true;
                PV.RPC("GameOver", RpcTarget.All);
            }
            else if(PlayerA_Catched_Good_Ghosts.Count == 4) // A 플레이어가 착한 유령 모두 잡은 경우 : A 승리
            {
                isPlayerWin = true;
                PV.RPC("GameOver", RpcTarget.All);
            }
        }
        else if(!PhotonNetwork.IsMasterClient)
        {
            if(PlayerA_Catched_Bad_Ghosts.Count == 4) // B 플레이어가 나쁜 유령 모두 잡힌 경우 : B 승리
            {
                isPlayerWin = true;
                PV.RPC("GameOver", RpcTarget.All);
            }
            else if(PlayerB_Catched_Good_Ghosts.Count == 4) // B 플레이어가 착한 유령 모두 잡은 경우 : B 승리
            {
                isPlayerWin = true;
                PV.RPC("GameOver", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void Change_Turn()
    {
        isPlayerTurn = !isPlayerTurn;

        Turn_Fire_On();

        PV.RPC("UpdateUI_Numbers", RpcTarget.All);

        Detect_Winner();
    }



    [PunRPC]
    public void UpdateUI_Numbers()
    {
        A_GoodGhost_Number.material = numbers[4 - PlayerB_Catched_Good_Ghosts.Count];
        A_BadGhost_Number.material = numbers[4 - PlayerB_Catched_Bad_Ghosts.Count];
        B_GoodGhost_Number.material = numbers[4 - PlayerA_Catched_Good_Ghosts.Count];
        B_Badhost_Number.material = numbers[4 - PlayerA_Catched_Bad_Ghosts.Count];
    }

    [PunRPC]
    public void Turn_Fire_On()
    {
        if (isPlayerTurn)
        {
            StartCoroutine(LightOn(PlayerTurn_Fire.transform.GetChild(0).GetComponent<ParticleSystem>()
                , PlayerTurn_Fire.transform.GetChild(2).GetComponent<Light>(), PlayerTurn_Fire));
            StartCoroutine(LightOff(Opposite_Turn_Fire.transform.GetChild(0).GetComponent<ParticleSystem>()
                , Opposite_Turn_Fire.transform.GetChild(2).GetComponent<Light>(), Opposite_Turn_Fire));
        }
        else
        {
            StartCoroutine(LightOn(Opposite_Turn_Fire.transform.GetChild(0).GetComponent<ParticleSystem>()
                , Opposite_Turn_Fire.transform.GetChild(2).GetComponent<Light>(), Opposite_Turn_Fire));
            StartCoroutine(LightOff(PlayerTurn_Fire.transform.GetChild(0).GetComponent<ParticleSystem>()
                , PlayerTurn_Fire.transform.GetChild(2).GetComponent<Light>(), PlayerTurn_Fire));
        }
    }

    IEnumerator LightOn(ParticleSystem PS, Light light, GameObject parent)
    {
        parent.SetActive(true);

        while(PS.startColor.a < 1)
        {
            Color tmp = PS.startColor;
            tmp.a += 0.02f;
            PS.startColor = tmp;

            light.intensity += 0.04f;

            yield return null;
        }
    }

    IEnumerator LightOff(ParticleSystem PS, Light light, GameObject parent)
    {
        while (PS.startColor.a > 1)
        {
            Color tmp = PS.startColor;
            tmp.a -= 0.02f;
            PS.startColor = tmp;

            light.intensity -= 0.04f;

            yield return null;
        }

        parent.SetActive(false);
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

        Debug.Log(go + "의 cell index 변경 : " + cell_index);
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
        GameOverText.SetActive(true);
        GameOverText.GetComponent<Text>().text = "GameOver!!";
        Text winner = GameOverText.transform.GetChild(0).GetComponent<Text>();

        if (PhotonNetwork.IsMasterClient && isPlayerWin)
            winner.text = "PlayerA Win!!";
        else if (PhotonNetwork.IsMasterClient && !isPlayerWin)
            winner.text = "PlayerB Win!!";
        else if (!PhotonNetwork.IsMasterClient && isPlayerWin)
            winner.text = "PlayerB Win!!";
        else if (!PhotonNetwork.IsMasterClient && !isPlayerWin)
            winner.text = "PlayerA Win!!";

        else
        {
            winner.text = "Bug";
        }
    }
}
