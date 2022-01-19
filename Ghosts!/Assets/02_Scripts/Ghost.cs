using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public int cell_Index;


    public void TurnOn_CanMove_PS()
    {
        if (cell_Index < 30)
            GameManager.instance.Board_Cells[cell_Index + 6].TurnOn_CanMove();
        if (cell_Index % 6 != 5)
            GameManager.instance.Board_Cells[cell_Index + 1].TurnOn_CanMove();
        if (cell_Index % 6 != 0)
            GameManager.instance.Board_Cells[cell_Index - 1].TurnOn_CanMove();
        if (cell_Index > 5)
            GameManager.instance.Board_Cells[cell_Index - 6].TurnOn_CanMove();
    }

}
