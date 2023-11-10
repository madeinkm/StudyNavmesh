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
    /// 대기를 할 수 있게 만들어주기 위함 변수들
    /// </summary>
    [SerializeField] private float waitMinTime = 1.0f; 
    [SerializeField] private float waitMaxTime = 3.0f; 
    private float waitTime;// 기다릴 시간
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

#if UNITY_EDITOR //OnDrawGizmos를 쓰려면 꼭 써야함 안그러면 빌드 안됨
    private void OnDrawGizmos()
    {
        if (showRange == true)
        {
            Handles.color = colorRange;
            Handles.DrawWireDisc(transform.position, Vector3.up, range);
        }        
    }
#endif //OnDrawGizmos를 쓰려면 꼭 써야함 안그러면 빌드 안됨


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
        if(agent.isOnOffMeshLink == true)// offmeshlink를 사용했다면
        {
            if(setOffMesh == false) 
            {
                setOffMesh = true;
                linkData = agent.currentOffMeshLinkData;
                linkStartPos = linkData.startPos + new Vector3(0f, agent.height * 0.5f, 0f);
                linkEndPos = linkData.endPos + new Vector3(0f, agent.height * 0.5f, 0f);

                agent.isStopped = true; // isStopped를 true로 하면 agent가 켜져있지만 동작 X
                jumpSpeed = Vector3.Distance(linkStartPos, linkEndPos) / agent.speed;
                maxHeightJump = (linkEndPos - linkStartPos).y + jumpHeight;
            }

            rotating(new Vector2(linkEndPos.x , linkEndPos.z));

            ratioJump += (Time.deltaTime / jumpSpeed);
            
            Vector3 movePos = Vector3.Lerp(linkStartPos, linkEndPos, ratioJump); // Lerp는 a 에서 b로 시간만큼 이동시켜주는 함수
            movePos.y = linkStartPos.y + (maxHeightJump * ratioJump) + (-jumpHeight * Mathf.Pow(ratioJump, 2)); // pow는 제곱근
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

    private void rotating(Vector2 _hit, bool _smooth = true) // hit  x z, smoot는 부드럽게 쳐다볼지 아닐지 // object가 회전을 시키는 코드임
    {
        float _targetRotation = Mathf.Atan2(_hit.x - transform.position.x, _hit.y - transform.position.z) * Mathf.Rad2Deg;
        if (_smooth)
        {
            float RotationSpeed = 0.0f;
            _targetRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref RotationSpeed, fTimeToConcentrationRotaion);
            //주석 생활화
        }
        transform.rotation = Quaternion.Euler(0.0f, _targetRotation, 0.0f);
    }

    /// <summary>
    /// 아래 update문은 자동으로 움직이는 코드 
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
        if(timer >= waitTime) // 시간이 충족 되면 랜덤한 시간을 다시 정의
        {
            timer = 0.0f;
            waitTime = Random.Range(waitMinTime, waitMaxTime);
            return true;
        }
        timer += Time.deltaTime;
        return false;
    }

    /// <summary>
    ///  현재 유닛으로 부터 random한 range만큼의 원형(구)으로 특정 위치를 랜덤하게 전달
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
    /// object가 도착을 했는지 안했는지 확인하는 함수
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
