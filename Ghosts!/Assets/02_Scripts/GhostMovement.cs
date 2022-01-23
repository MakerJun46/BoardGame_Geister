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

    public bool isGhostMoving;

    public Material Ghost_Body_Fade;
    public Material Ghost_Body;
    public Material Ghost_Eye_Fade;
    public Material Ghost_Eye;
    public Material Ghost_Good_Fade;
    public Material Ghost_Good;
    public Material Ghost_Bad_Fade;
    public Material Ghost_Bad;

    public AudioSource Ghost_Catched_Audio;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        isGhostMoving = false;
    }

    void Update()
    {
        if(GameManager.instance.isPlayerTurn && !isGhostMoving)
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

            if (hit && hitInfo.transform.gameObject.tag == "Ghost" && hitInfo.transform.GetComponent<PhotonView>().IsMine 
                    && !hitInfo.transform.GetComponent<Ghost>().isCatched) // ���� ���� Ÿ��
            {
                if(!hitInfo.transform.gameObject.GetComponent<Ghost>().Turn_On_PS)
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
            else if(hit && hitInfo.transform.gameObject.tag == "Ghost" && select_Ghost != null && !hitInfo.transform.GetComponent<PhotonView>().IsMine
                && !hitInfo.transform.GetComponent<Ghost>().isCatched) // ��� ���� Ÿ��
            {
                Cell hitGhostCell = GameManager.instance.Board_Cells[hitInfo.transform.GetComponent<Ghost>().cell_Index];
                foreach(Cell c in select_Ghost.GetComponent<Ghost>().neighbor_Cells)
                {
                    if(hitGhostCell.cell_Index == c.cell_Index)     // �̿��� cell�� Ghost�� ��쿡�� Catch
                    {
                        StartCoroutine(vanishGhost(hitInfo.transform.gameObject)); // ���� ���� ������� ���� -> ��� �Լ� ȣ��
                        Move(GameManager.instance.Board_Cells[hitInfo.transform.GetComponent<Ghost>().cell_Index].transform);
                    }
                }
            }

            else if(hit && hitInfo.transform.gameObject.tag == "Cell" && select_Ghost != null && hitInfo.transform.GetComponent<Cell>().this_Cell_Ghost == null) // �̵� �� ����
            {
                Cell hitCell = hitInfo.transform.GetComponent<Cell>();
                foreach(Cell c in select_Ghost.GetComponent<Ghost>().neighbor_Cells)    // �̿��� cell�� ��쿡�� move
                {
                    if(hitCell.cell_Index == c.cell_Index)
                    {
                        Move(hitInfo.transform);
                    }
                }
            }
        }
    }

    public void Move(Transform target) // Cell �� transform
    {
        if (target.gameObject.GetComponent<Cell>().isEscapeCell &&
        select_Ghost.GetComponent<Ghost>().ghost_Type == Ghost.GhostType.bad)   // ���� ������ EscapeCell�� �̵��Ϸ��ϸ� �ٷ� return
            return;

        else if(target.gameObject.GetComponent<Cell>().isEscapeCell &&
            select_Ghost.GetComponent<Ghost>().ghost_Type == Ghost.GhostType.good)  // ���� ������ EscapeCell�� �̵��� ��� (�¸��ϴ� ���)
        {
            if (PhotonNetwork.IsMasterClient) // �ٵ� A �÷��̾
            {
                if (target.gameObject.GetComponent<Cell>().cell_Index == 0 || target.gameObject.GetComponent<Cell>().cell_Index == 5) 
                    return; // A�� ������ ������ �ϴ� ��� return (�̵� �Ұ�)
            }
            else
            {
                if (target.gameObject.GetComponent<Cell>().cell_Index == 30 || target.gameObject.GetComponent<Cell>().cell_Index == 35)
                    return;
            }

            // �� ���ǿ� �ɸ��� ������ (��� ���� escapecell�� ����, �� ������ ghost_Type �� good �̶��
            StartCoroutine(moveGhost(target));
            select_Ghost.GetComponent<Ghost>().TurnOff_CanMove_PS();
            PV.RPC("OffGhostCross", RpcTarget.All);
            GameManager.instance.isPlayerWin = true;    // �¸�
            GameManager.instance.PV.RPC("GameOver", RpcTarget.All);
            return;
        }
        StartCoroutine(moveGhost(target));
        select_Ghost.GetComponent<Ghost>().TurnOff_CanMove_PS();
        PV.RPC("OffGhostCross", RpcTarget.All);

        PV.RPC("MoveGhost_setCell", RpcTarget.AllBuffered, select_Ghost.GetComponent<PhotonView>().ViewID, target.gameObject.GetComponent<Cell>().cell_Index);
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
        isGhostMoving = true;

        select_Ghost.GetComponent<AudioSource>().Play();

        while(select_Ghost.transform.position != target.position)
        {
            select_Ghost.transform.position = Vector3.Lerp(select_Ghost.transform.position, target.position, Time.deltaTime * 5);
            yield return null;
        }

        select_Ghost = null;
        isGhostMoving = false;

        GameManager.instance.PV.RPC("Change_Turn", RpcTarget.All);
    }

    public IEnumerator vanishGhost(GameObject go)
    {
        Transform ghost = go.transform.GetChild(0);

        ghost.GetChild(0).GetComponent<MeshRenderer>().material = Ghost_Eye_Fade;
        Material GhostType = go.GetComponent<Ghost>().ghost_Type == Ghost.GhostType.good ? Ghost_Good_Fade : Ghost_Bad_Fade;
        ghost.GetChild(1).GetComponent<MeshRenderer>().material = GhostType;
        ghost.GetChild(2).GetComponent<MeshRenderer>().material = Ghost_Body_Fade;

        while(Ghost_Body_Fade.color.a > 0)
        {
            Debug.Log($"alpha ���� �� : {Ghost_Body_Fade.color.a}");
            Color tmp = Ghost_Eye_Fade.color;
            tmp.a -= 0.02f;
            Ghost_Eye_Fade.color = tmp;

            tmp = Ghost_Body_Fade.color;
            tmp.a -= 0.02f;
            Ghost_Body_Fade.color = tmp;

            tmp = GhostType.color;
            tmp.a -= 0.02f;
            GhostType.color = tmp;

            yield return null;
        }

        PV.RPC("Catch_Opponent_Ghost", RpcTarget.All, go.transform.GetComponent<PhotonView>().ViewID); // catch ghost
        yield return new WaitForSeconds(0.5f);

        while(Ghost_Body_Fade.color.a < 1)
        {
            Debug.Log($"alpha ���� �� : {Ghost_Body_Fade.color.a}");
            Color tmp = Ghost_Eye_Fade.color;
            tmp.a += 0.02f;
            Ghost_Eye_Fade.color = tmp;

            tmp = Ghost_Body_Fade.color;
            tmp.a += 0.02f;
            Ghost_Body_Fade.color = tmp;

            tmp = GhostType.color;
            tmp.a += 0.02f;
            GhostType.color = tmp;

            yield return null;
        }

        ghost.GetChild(0).GetComponent<MeshRenderer>().material = Ghost_Eye;
        ghost.GetChild(1).GetComponent<MeshRenderer>().material = go.GetComponent<Ghost>().ghost_Type == Ghost.GhostType.good ? Ghost_Good : Ghost_Bad;
        ghost.GetChild(2).GetComponent<MeshRenderer>().material = Ghost_Body;
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

    [PunRPC]
    public void Catch_Opponent_Ghost(int ViewID)
    {
        Ghost_Catched_Audio.Play();

        GameObject go = PhotonView.Find(ViewID).gameObject;

        go.GetComponent<Ghost>().isCatched = true;
        go.GetComponent<Ghost>().Catched();

        if (go.GetComponent<Ghost>().ghost_Type == Ghost.GhostType.good)    // Catched good Ghost
        {
            if ((PhotonNetwork.IsMasterClient && go.GetComponent<PhotonView>().IsMine)      // ���� A�÷��̾��̰� �� ������ �����ٸ� 
                || (!PhotonNetwork.IsMasterClient && !go.GetComponent<PhotonView>().IsMine))    // || ���� B�÷��̾�� ��� ������ ��Ҵٸ�
            {
                go.transform.position = GameManager.instance.PlayerB_Catched_Good_Ghost_Positions[GameManager.instance.PlayerB_Catched_Good_Ghosts.Count].transform.position;

                GameManager.instance.PlayerB_Catched_Good_Ghosts.Add(go);

                go.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if((!PhotonNetwork.IsMasterClient && go.GetComponent<PhotonView>().IsMine)  // ���� B�÷��̾�� �� ������ �����ٸ�
                || (PhotonNetwork.IsMasterClient && !go.GetComponent<PhotonView>().IsMine)) // || ���� A �÷��̾�� ��� ������ ��Ҵٸ�
            {
                go.transform.position = GameManager.instance.PlayerA_Catched_Good_Ghost_Positions[GameManager.instance.PlayerA_Catched_Good_Ghosts.Count].transform.position;

                GameManager.instance.PlayerA_Catched_Good_Ghosts.Add(go);

                go.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

        }
        else // Catched bad Ghost
        {
            if ((PhotonNetwork.IsMasterClient && go.GetComponent<PhotonView>().IsMine)      // ���� A�÷��̾��̰� �� ������ �����ٸ� 
                || (!PhotonNetwork.IsMasterClient && !go.GetComponent<PhotonView>().IsMine))    // || ���� B�÷��̾�� ��� ������ ��Ҵٸ�
            {
                go.transform.position = GameManager.instance.PlayerB_Catched_Bad_Ghost_Positions[GameManager.instance.PlayerB_Catched_Bad_Ghosts.Count].transform.position;

                GameManager.instance.PlayerB_Catched_Bad_Ghosts.Add(go);

                go.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if ((!PhotonNetwork.IsMasterClient && go.GetComponent<PhotonView>().IsMine)  // ���� B�÷��̾�� �� ������ �����ٸ�
                || (PhotonNetwork.IsMasterClient && !go.GetComponent<PhotonView>().IsMine)) // || ���� A �÷��̾�� ��� ������ ��Ҵٸ�
            {
                go.transform.position = GameManager.instance.PlayerA_Catched_Bad_Ghost_Positions[GameManager.instance.PlayerA_Catched_Bad_Ghosts.Count].transform.position;

                GameManager.instance.PlayerA_Catched_Bad_Ghosts.Add(go);

                go.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}