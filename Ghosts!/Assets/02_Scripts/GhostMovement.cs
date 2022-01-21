using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GhostMovement : MonoBehaviour
{
    public static GhostMovement instance;

    public GameObject GhostCross;

    public GameObject select_Ghost;

    public PhotonView PV;

    private void Awake()
    {
        instance = this;
    }


    void Start()
    {
    }

    void Update()
    {
        if(GameManager.instance.isPlayerTurn)
        {
            SelectGhost();
        }
    }

    public void SelectGhost()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

            Debug.Log("Hit : " + hitInfo.transform.gameObject.name);

            if (hit && hitInfo.transform.gameObject.tag == "Ghost")
            {
                if(!hitInfo.transform.gameObject.GetComponent<Ghost>().Turn_On_PS && hitInfo.transform.GetComponent<PhotonView>().IsMine)
                {
                    if(PhotonNetwork.IsMasterClient)
                        playerA_Select_Off();
                    else
                        playerB_Select_Off();

                    hitInfo.transform.gameObject.GetComponent<Ghost>().find_neighbor_Cell();
                    hitInfo.transform.gameObject.GetComponent<Ghost>().TurnOn_CanMove_PS();

                    select_Ghost = hitInfo.transform.gameObject;

                    PV.RPC("SetGhostCross", RpcTarget.All, hitInfo.transform.gameObject.GetComponent<PhotonView>().ViewID);
                }
                else
                {
                    hitInfo.transform.gameObject.GetComponent<Ghost>().TurnOff_CanMove_PS();
                    GhostCross.SetActive(false);

                    select_Ghost = null;
                }
            }

            else if(hit && hitInfo.transform.gameObject.tag == "Cell" && select_Ghost != null)
            {
                Move(hitInfo.transform);
            }
        }
    }

    public void Move(Transform target)
    {
        if (target.gameObject.GetComponent<Cell>().isEscapeCell &&
        select_Ghost.GetComponent<Ghost>().ghost_Type == Ghost.GhostType.bad)   // 나쁜 유령이 EscapeCell로 이동하려하면 바로 return
            return;

        else if(target.gameObject.GetComponent<Cell>().isEscapeCell &&
            select_Ghost.GetComponent<Ghost>().ghost_Type == Ghost.GhostType.good)  // 착한 유령이 EscapeCell로 이동한 경우 (승리하는 경우)
        {
            if (PhotonNetwork.IsMasterClient) // 근데 A 플레이어가
            {
                if (target.gameObject.GetComponent<Cell>().cell_Index == 0 || target.gameObject.GetComponent<Cell>().cell_Index == 5) 
                    return; // A측 진영에 들어갈려고 하는 경우 return (이동 불가)
            }
            else
            {
                if (target.gameObject.GetComponent<Cell>().cell_Index == 30 || target.gameObject.GetComponent<Cell>().cell_Index == 35)
                    return;
            }

            // 위 조건에 걸리지 않으면 (상대 진영 escapecell에 들어갔고, 내 유령의 ghost_Type 이 good 이라면
            GameManager.instance.isPlayerWin = true;    // 승리
            GameManager.instance.PV.RPC("GameOver", RpcTarget.All);
        }
        StartCoroutine(moveGhost(target));

        PV.RPC("MoveGhost_setCell", RpcTarget.All, select_Ghost.GetComponent<PhotonView>().ViewID, target.gameObject.GetComponent<Cell>().cell_Index);

        select_Ghost.GetComponent<Ghost>().TurnOff_CanMove_PS();

        PV.RPC("OffGhostCross", RpcTarget.All);

        GameManager.instance.PV.RPC("Change_Turn", RpcTarget.All);
    }

    public void playerA_Select_Off()
    {
        foreach(GameObject go in GameManager.instance.PlayerA_Ghosts)
        {
            go.GetComponent<Ghost>().TurnOff_CanMove_PS();
        }
    }

    public void playerB_Select_Off()
    {
        foreach (GameObject go in GameManager.instance.PlayerB_Ghosts)
        {
            go.GetComponent<Ghost>().TurnOff_CanMove_PS();
        }
    }

    public IEnumerator moveGhost(Transform target)
    {
        while(select_Ghost.transform.position != target.position)
        {
            select_Ghost.transform.position = Vector3.Lerp(select_Ghost.transform.position, target.position, Time.deltaTime * 15);
            yield return null;
        }

        GameManager.instance.Board_Cells[select_Ghost.GetComponent<Ghost>().cell_Index].this_Cell_Ghost = null;
        select_Ghost.GetComponent<Ghost>().cell_Index = target.GetComponent<Cell>().cell_Index;
    }
    

    [PunRPC]
    public void SetGhostCross(int ViewID)
    {
        Transform target = PhotonView.Find(ViewID).gameObject.transform;

        GhostCross.SetActive(true);
        GhostCross.transform.parent = target.transform;
    }

    [PunRPC]
    public void OffGhostCross()
    {
        GhostCross.SetActive(false);
    }

    [PunRPC]
    public void MoveGhost_setCell(int ViewID, int cell_index)
    {
        GameObject go = PhotonView.Find(ViewID).gameObject;

        GameManager.instance.Board_Cells[go.GetComponent<Ghost>().cell_Index].this_Cell_Ghost = null;

        go.GetComponent<Ghost>().cell_Index = cell_index;

        GameManager.instance.Board_Cells[cell_index].this_Cell_Ghost = go;
    }
}
