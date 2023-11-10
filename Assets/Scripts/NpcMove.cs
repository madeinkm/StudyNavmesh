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
    private float maxHeightJump;
    [SerializeField] private float jumpHeight = 5.0f;
    private float ratioJump;
    [SerializeField] private float fTimeToConcentrationRotaion = 0.1f;

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
                maxHeightJump = (linkEndPos - linkStartPos).y + jumpHeight;
            }

            rotating(new Vector2(linkEndPos.x , linkEndPos.z));

            ratioJump += (Time.deltaTime / jumpSpeed);
            
            Vector3 movePos = Vector3.Lerp(linkStartPos, linkEndPos, ratioJump); // Lerp�� a ���� b�� �ð���ŭ �̵������ִ� �Լ�
            movePos.y = linkStartPos.y + (maxHeightJump * ratioJump) + (-jumpHeight * Mathf.Pow(ratioJump, 2)); // pow�� ������
            transform.position = movePos;

            if (ratioJump >= 1.0f)
            {
                ratioJump = 0.0f;
                agent.CompleteOffMeshLink();
                agent.isStopped = false;
                setOffMesh = false;
            }
        }
    }

    private void rotating(Vector2 _hit, bool _smooth = true) // hit  x z, smoot�� �ε巴�� �Ĵٺ��� �ƴ��� // object�� ȸ���� ��Ű�� �ڵ���
    {
        float _targetRotation = Mathf.Atan2(_hit.x - transform.position.x, _hit.y - transform.position.z) * Mathf.Rad2Deg;
        if (_smooth)
        {
            float RotationSpeed = 0.0f;
            _targetRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref RotationSpeed, fTimeToConcentrationRotaion);
            //�ּ� ��Ȱȭ
        }
        transform.rotation = Quaternion.Euler(0.0f, _targetRotation, 0.0f);
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
