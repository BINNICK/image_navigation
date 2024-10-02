using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(NavMeshAgent))]
public class AgentMover : MonoBehaviour
{
    public NavMeshAgent agent; // NavMeshAgent ������Ʈ
    public GPSMapper gpsMapper; // GPSMapper ��ũ��Ʈ�� �ִ� ������Ʈ
    
    private LineRenderer lineRenderer;
    private Vector2 targetGPSCoordinates; // GPS ��ǥ ������ ���� ����

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

        // GPSCoordinatesLoader���� ��ǥ �ҷ�����
        targetGPSCoordinates = GPSCoordinatesLoader.LoadGPSCoordinates();

        // ��ũ��Ʈ���� ���� ������ GPS ��ǥ�� ����Ͽ� ������Ʈ�� �̵���Ű�� �ڵ� �߰�
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
        yield return new WaitForSeconds(delay); // ��� ��꿡 �ð��� �ɸ� �� �����Ƿ�, ��� ��ٸ� �� ��θ� �׸��ϴ�.

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

