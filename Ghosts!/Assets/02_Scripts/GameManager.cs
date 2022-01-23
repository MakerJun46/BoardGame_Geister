using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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

    public List<Material> numbers;

    public GameObject TilePosition_Parent;
    public GameObject PlayerTurn_Fire;
    public GameObject Opposite_Turn_Fire;
    public GameObject Lantern_Fire;

    public GameObject PlayerA_UI;
    public GameObject PlayerB_UI;

    public bool isSorted;
    public bool isSettingAComplete;
    public bool isSettingBComplete;
    public bool isPlayerTurn;
    public bool isGameStarted;
    public bool isPlayerWin;

    public string PlayerCode;

    public MeshRenderer A_GoodGhost_Number;
    public MeshRenderer A_BadGhost_Number; 
    public MeshRenderer B_GoodGhost_Number; 
    public MeshRenderer B_Badhost_Number;

    public GameObject WaitingPanel;
    public GameObject GameReadyScene;
    public GameObject GameOverScene;

    public int GameReplayCount;

    public AudioSource ButtonClick_Audio;
    public AudioSource GameOver_Audio;
    public GameObject GameReadyScene_Audio;

    void Start()
    {
        GameReplayCount = 0;
    }

    public void ResetValue()
    {
        isSorted = false;
        isSettingAComplete = false;
        isSettingBComplete = false;
        isGameStarted = false;
        isPlayerWin = false;

        for (int i = 0; i < Board_Cells.Count; i++)
        {
            Board_Cells[i].cell_Index = i;
        }

        GhostMovement.instance.isGhostMoving = false;

        PlayerA_Ghosts.Clear();
        PlayerB_Ghosts.Clear();
        PlayerA_Catched_Bad_Ghosts.Clear();
        PlayerA_Catched_Good_Ghosts.Clear();
        PlayerB_Catched_Bad_Ghosts.Clear();
        PlayerB_Catched_Good_Ghosts.Clear();

        GameReplayCount++;
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && isSettingAComplete && isSettingBComplete && !isGameStarted)
        {
            GameReadyScene_Audio.SetActive(false);

            isGameStarted = true;
            PV.RPC("Turn_Fire_On", RpcTarget.All);
            PV.RPC("Off_StartPanel", RpcTarget.All);
        }
    }

    public void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;

        while(n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void Set_Player_A()
    {
        PlayerCode = "A";

        random = new System.Random();
        int randomIndex = random.Next(0, 8);
        int index = 0;

        Shuffle<Cell>(PlayerA_Start_Positions);

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

        GameObject.Find("Camera").transform.Find("MainCamera_A").gameObject.SetActive(true);
        GameObject.Find("Camera").transform.Find("MainCamera_B").gameObject.SetActive(false);

        PV.RPC("Setting_A_Complete", RpcTarget.All);
    }

    public void Set_Player_B()
    {
        PlayerCode = "B";

        random = new System.Random();
        int randomIndex = random.Next(0, 8);
        int index = 0;

        Shuffle<Cell>(PlayerB_Start_Positions);

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

        GameObject.Find("Camera").transform.Find("MainCamera_A").gameObject.SetActive(false);
        GameObject.Find("Camera").transform.Find("MainCamera_B").gameObject.SetActive(true);

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

    public void WatingPlayer()
    {
        WaitingPanel.SetActive(true);
        ButtonClick_Audio.Play();
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
        ParticleSystem.MainModule main = PS.main;

        parent.GetComponent<AudioSource>().Play();

        while (light.intensity < 2)
        {
            Color tmp = main.startColor.color;
            tmp.a += 0.02f;
            main.startColor = tmp;
            light.intensity += 0.04f;

            yield return null;
        }
    }

    IEnumerator LightOff(ParticleSystem PS, Light light, GameObject parent)
    {
        ParticleSystem.MainModule main = PS.main;
        while (light.intensity > 0)
        {
            Color tmp = main.startColor.color;
            tmp.a -= 0.02f;
            main.startColor = tmp;
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
        GameOverScene.SetActive(true);

        GameOver_Audio.Play();
        GameReadyScene.SetActive(true);
        
        TextMeshProUGUI winner = GameOverScene.transform.Find("Winner").GetComponent<TextMeshProUGUI>();

        if (PhotonNetwork.IsMasterClient && isPlayerWin)
            winner.text = "PlayerAWin";
        else if (PhotonNetwork.IsMasterClient && !isPlayerWin)
            winner.text = "PlayerBWin";
        else if (!PhotonNetwork.IsMasterClient && isPlayerWin)
            winner.text = "PlayerBWin";
        else if (!PhotonNetwork.IsMasterClient && !isPlayerWin)
            winner.text = "PlayerAWin";

        else
        {
            winner.text = "Bug";
        }
    }

    [PunRPC]
    public void Off_StartPanel()
    {
        GameReadyScene.SetActive(false);
        GameOverScene.SetActive(false);
        WaitingPanel.SetActive(false);

        Lantern_Fire.SetActive(true);
    }
}
