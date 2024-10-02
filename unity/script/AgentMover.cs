using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(NavMeshAgent))]
public class AgentMover : MonoBehaviour
{
    public NavMeshAgent agent; // NavMeshAgent 컴포넌트
    public GPSMapper gpsMapper; // GPSMapper 스크립트가 있는 오브젝트
    
    private LineRenderer lineRenderer;
    private Vector2 targetGPSCoordinates; // GPS 좌표 저장을 위한 변수

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent is not assigned.");
        }

        if (gpsMapper == null)
        {
            Debug.LogError("GPSMapper is not assigned.");
        }

        // GPSCoordinatesLoader에서 좌표 불러오기
        targetGPSCoordinates = GPSCoordinatesLoader.LoadGPSCoordinates();

        // 스크립트에서 직접 설정한 GPS 좌표를 사용하여 에이전트를 이동시키는 코드 추가
        MoveAgentToGPS(targetGPSCoordinates.x, targetGPSCoordinates.y);
    }

    public void MoveAgentToGPS(float longitude, float latitude)
    {
        if (agent != null && gpsMapper != null)
        {
            Vector3 targetPosition = gpsMapper.ConvertGPSToUnityPosition(longitude, latitude);
            NavMeshHit closestHit;
            if (NavMesh.SamplePosition(targetPosition, out closestHit, 1000, NavMesh.AllAreas))
            {
                agent.SetDestination(closestHit.position);
                StartCoroutine(DrawPathWithDelay());
            }
            else
            {
                Debug.LogError("No nearby NavMesh point found.");
            }
        }
    }

    IEnumerator DrawPathWithDelay(float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay); // 경로 계산에 시간이 걸릴 수 있으므로, 잠시 기다린 후 경로를 그립니다.

        if (agent.path.corners.Length < 2) yield break;

        lineRenderer.positionCount = agent.path.corners.Length;
        lineRenderer.SetPositions(agent.path.corners);
    }

    // Optional: Call this method to clear the drawn path when necessary
    public void ClearPath()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }
    
}

