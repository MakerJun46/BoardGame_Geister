using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
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

    public bool isReady;
    public bool isSorted;

    void Start()
    {
        isReady = false;
        isSorted = false;
    }

    void Update()
    {
        if(isReady)
        {
            test();
            isReady = false;
        }
        
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

        isReady = true;
        isSorted = true;
    }

    public void test()
    {
        random = new System.Random();
        int randomIndex = random.Next(0, 8);

        foreach (GameObject go in PlayerA_Ghosts)
        {
            while (PlayerA_Start_Positions[randomIndex].this_Cell_Ghost != null)
            {
                randomIndex = random.Next(0, 8);
            }

            Debug.Log(go.name + " ÀÇ position : " + PlayerA_Start_Positions[randomIndex].transform.position + "rI : " + randomIndex);
            go.transform.position = PlayerA_Start_Positions[randomIndex].transform.position;
            PlayerA_Start_Positions[randomIndex].this_Cell_Ghost = go;
            go.GetComponent<Ghost>().cell_Index = PlayerA_Start_Positions[randomIndex].GetComponent<Cell>().cell_Index;
        }

        randomIndex = random.Next(0, 8);

        foreach (GameObject go in PlayerB_Ghosts)
        {
            while (PlayerB_Start_Positions[randomIndex].this_Cell_Ghost != null)
            {
                randomIndex = random.Next(0, 8);
            }


            Debug.Log(go.name + " ÀÇ position : " + PlayerB_Start_Positions[randomIndex].transform.position + "rI : " + randomIndex);
            go.transform.position = PlayerB_Start_Positions[randomIndex].transform.position;
            PlayerB_Start_Positions[randomIndex].this_Cell_Ghost = go;
            go.GetComponent<Ghost>().cell_Index = PlayerB_Start_Positions[randomIndex].GetComponent<Cell>().cell_Index;
        }
    }
}
