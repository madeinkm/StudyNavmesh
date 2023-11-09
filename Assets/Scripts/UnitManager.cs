using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    private List<NavMeshAgent> listNavMeshAgent = new List<NavMeshAgent>();

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

    public void AddAgent(NavMeshAgent _agent)
    {
        if (listNavMeshAgent.Exists(x => x == _agent))
        {
            return;
        }
        listNavMeshAgent.Add(_agent);
    }

    public void RemoveAgent(NavMeshAgent _agent)
    {
        listNavMeshAgent.Remove(_agent);//�ڱ����� ���� data�� ������ �ϸ� �����Ѵ�.
    }

    public void moveAllAgent(Vector3 _point)
    {
        int count = listNavMeshAgent.Count;
        for (int iNum = 0; iNum < count; iNum++)
        {
            NavMeshAgent agent = listNavMeshAgent[iNum];
            agent.SetDestination(_point);
        }

        //foreach (NavMeshAgent agent in listNavMeshAgent)  // foreach���� �Ἥ ������ �����ѵ����� ���� but ������ �� �Ⱦ�
        //{
        //    agent.SetDestination(_point);
        //}
    }
}
