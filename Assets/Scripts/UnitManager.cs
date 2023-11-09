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
        listNavMeshAgent.Remove(_agent);//자기한테 없는 data를 지우라고 하면 무시한다.
    }

    public void moveAllAgent(Vector3 _point)
    {
        int count = listNavMeshAgent.Count;
        for (int iNum = 0; iNum < count; iNum++)
        {
            NavMeshAgent agent = listNavMeshAgent[iNum];
            agent.SetDestination(_point);
        }

        //foreach (NavMeshAgent agent in listNavMeshAgent)  // foreach문을 써서 위에와 동일한동작이 가능 but 느려서 잘 안씀
        //{
        //    agent.SetDestination(_point);
        //}
    }
}
