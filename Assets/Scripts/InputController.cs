using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController Instance;
    private Camera mainCam;
    private UnitManager unitManager;

    [SerializeField] private NpcMove npc;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        mainCam = Camera.main;
        unitManager = UnitManager.Instance;
    }
    void Update()
    {
        if (npc != null && Input.GetMouseButtonDown(0)) 
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition); //���� ���콺�� �ִ� ��ġ�� ����ķ�� �����ͷ� ��Ƽ� ray�� ��������
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                //npc.SetDestination(hit.point);
                Debug.Log(hit.point);
                unitManager.moveAllAgent(hit.point);
            }            
        }        
    }
}
