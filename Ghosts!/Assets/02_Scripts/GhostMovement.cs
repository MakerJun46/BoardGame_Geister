using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GhostMovement : MonoBehaviour
{
    public static GhostMovement instance;

    public GameObject GhostCross;

    public GameObject select_Ghost;

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
                    GhostCross.SetActive(true);
                    GhostCross.transform.parent = hitInfo.transform;

                    select_Ghost = hitInfo.transform.gameObject;
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
                select_Ghost.GetComponent<Ghost>().TurnOff_CanMove_PS();
                GhostCross.SetActive(false);

                StartCoroutine(moveGhost(hitInfo.transform));
            }
        }
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
    
}
