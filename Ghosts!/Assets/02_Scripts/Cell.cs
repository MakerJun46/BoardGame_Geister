using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Cell : MonoBehaviour
{
    public GameObject this_Cell_Ghost;
    GameObject CanMove_PS;
    GameObject CanCatch_PS;
    public int cell_Index;

    public bool isEscapeCell;

    private void Start()
    {
        CanMove_PS = transform.GetChild(0).gameObject;
        CanCatch_PS = transform.GetChild(1).gameObject;
    }
    private void Update()
    {
        
    }

    public void TurnOff_PS()
    {
        CanMove_PS.SetActive(false);
        CanCatch_PS.SetActive(false);
    }

    public void TurnOn_CanMove()
    {
        CanMove_PS.SetActive(true);
    }

    public void TurnOn_CanCatch()
    {
        CanCatch_PS.SetActive(true);
    }

    public GameObject Ghost_inCell()
    {
        return this_Cell_Ghost;
    }

    public void setGhost_inCell(GameObject ghost)
    {
        this_Cell_Ghost = ghost;
    }
}
