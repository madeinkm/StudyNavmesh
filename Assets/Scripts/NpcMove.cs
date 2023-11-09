using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class NpcMove : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private float range;

    [SerializeField] private bool showRange;
    [SerializeField] private Color colorRange;

    /// <summary>
    /// ��⸦ �� �� �ְ� ������ֱ� ���� ������
    /// </summary>
    [SerializeField] private float waitMinTime = 1.0f; 
    [SerializeField] private float waitMaxTime = 3.0f; 
    private float waitTime;// ��ٸ� �ð�
    private float timer = 0;

    private bool setOffMesh = false;
    private OffMeshLinkData linkData;
    private Vector3 linkStartPos;
    private Vector3 linkEndPos;
    private float jumpSpeed;

#if UNITY_EDITOR //OnDrawGizmos�� ������ �� ����� �ȱ׷��� ���� �ȵ�
    private void OnDrawGizmos()
    {
        if (showRange == true)
        {
            Handles.color = colorRange;
            Handles.DrawWireDisc(transform.position, Vector3.up, range);
        }        
    }
#endif //OnDrawGizmos�� ������ �� ����� �ȱ׷��� ���� �ȵ�


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();        
    }

    private void Start()
    {
        UnitManager.Instance.AddAgent(agent);
    }

    private void Update()
    {
        doJump();

    }

    private void doJump()
    {
        if(agent.isOnOffMeshLink == true)// offmeshlink�� ����ߴٸ�
        {
            if(setOffMesh == false) 
            {
                setOffMesh = true;
                linkData = agent.currentOffMeshLinkData;
                linkStartPos = linkData.startPos + new Vector3(0f, agent.height * 0.5f, 0f);
                linkEndPos = linkData.endPos + new Vector3(0f, agent.height * 0.5f, 0f);

                agent.isStopped = true; // isStopped�� true�� �ϸ� agent�� ���������� ���� X
                jumpSpeed = Vector3.Distance(linkStartPos, linkEndPos) / agent.speed;
            }                    
        }
    }

    /// <summary>
    /// �Ʒ� update���� �ڵ����� �����̴� �ڵ� 
    /// </summary>
    //void Update()
    //{
    //    if (isArrive() == true && waiting() == true)
    //    {
    //        Vector3 point = getRandomPoint();
    //        agent.SetDestination(point);
    //    }
    //}

    private bool waiting() 
    {
        if(timer >= waitTime) // �ð��� ���� �Ǹ� ������ �ð��� �ٽ� ����
        {
            timer = 0.0f;
            waitTime = Random.Range(waitMinTime, waitMaxTime);
            return true;
        }
        timer += Time.deltaTime;
        return false;
    }

    /// <summary>
    ///  ���� �������� ���� random�� range��ŭ�� ����(��)���� Ư�� ��ġ�� �����ϰ� ����
    /// </summary>
    /// <returns></returns> 
    private Vector3 getRandomPoint()
    {
        Vector3 randomPosition = transform.position + Random.insideUnitSphere * range;
        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, range, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }
    /// <summary>
    /// object�� ������ �ߴ��� ���ߴ��� Ȯ���ϴ� �Լ�
    /// </summary>
    /// <returns></returns>
    private bool isArrive()
    {
        if (agent.velocity == Vector3.zero)
        {
            return true;
        }
        return false;      
    }

    //public void SetDestination(Vector3 _point)
    //{
    //    agent.SetDestination(_point);
    //}
}
