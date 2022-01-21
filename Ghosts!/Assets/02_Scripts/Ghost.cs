using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ghost : MonoBehaviour
{
    public int cell_Index;

    public List<Cell> neighbor_Cells;

    public bool Turn_On_PS;

    public string PlayerCode;

    public enum GhostType { good, bad};

    public GhostType ghost_Type;

    private void Start()
    {
        neighbor_Cells = new List<Cell>();
        Turn_On_PS = false;
    }

    public void find_neighbor_Cell()
    {
        neighbor_Cells.Clear();

        if (cell_Index < 30)
            neighbor_Cells.Add(GameManager.instance.Board_Cells[cell_Index + 6]);
        if(cell_Index % 6 != 5)
            neighbor_Cells.Add(GameManager.instance.Board_Cells[cell_Index + 1]);
        if (cell_Index % 6 != 0)
            neighbor_Cells.Add(GameManager.instance.Board_Cells[cell_Index - 1]);
        if (cell_Index > 5)
            neighbor_Cells.Add(GameManager.instance.Board_Cells[cell_Index - 6]);
    }

    public void TurnOn_CanMove_PS()
    {
        foreach(Cell c in neighbor_Cells)
        {
            if (c.this_Cell_Ghost == null && !c.isEscapeCell)
                c.TurnOn_CanMove();
            else if (c.this_Cell_Ghost == null && c.isEscapeCell)
                c.TurnOn_CanMove();
            else if (c.this_Cell_Ghost != null && !c.this_Cell_Ghost.GetComponent<PhotonView>().IsMine)  // isMine 아닌(상대) 유령이면 cancatch on
                c.TurnOn_CanCatch();
        }

        Turn_On_PS = true;
    }

    public void TurnOff_CanMove_PS()
    {
        foreach(Cell c in neighbor_Cells)
        {
            c.TurnOff_PS();
        }

        Turn_On_PS = false;
    }

}
