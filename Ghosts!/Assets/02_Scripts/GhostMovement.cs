using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    public static GhostMovement instance;

    private void Awake()
    {
        instance = this;
    }

    public bool isSelect_Turn;

    void Start()
    {
        isSelect_Turn = true;
    }

    void Update()
    {
        if(isSelect_Turn)
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
                hitInfo.transform.gameObject.GetComponent<Ghost>().TurnOn_CanMove_PS();
            }
        }
    }
}
