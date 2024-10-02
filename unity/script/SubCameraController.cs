using UnityEngine;
using System.Collections;

public class SubCameraController : MonoBehaviour
{
    public Transform agentTransform; // 에이전트의 Transform
    public GPSMapper gpsMapper; // GPSMapper 스크립트가 있는 오브젝트
    public Camera mainCamera; // 메인 카메라
    public Camera subCamera; // 서브 카메라
    private Vector2 targetGPSCoordinates; // 파일에서 불러온 목적지 GPS 좌표

    void Start()
    {
        // GPSCoordinatesLoader에서 목적지의 GPS 좌표 불러오기
        targetGPSCoordinates = GPSCoordinatesLoader.LoadGPSCoordinates();

        // GPS 좌표를 Unity 월드 좌표로 변환
        Vector3 destination = gpsMapper.ConvertGPSToUnityPosition(targetGPSCoordinates.x, targetGPSCoordinates.y);

        // 에이전트와 목적지 사이의 중간 지점 계산
        Vector3 midpoint = (agentTransform.position + destination) / 2.0f;

        // 에이전트와 목적지 사이의 거리에 따라 카메라 위치 조정
        float distance = Vector3.Distance(agentTransform.position, destination);
        subCamera.transform.position = midpoint + new Vector3(0, distance * 0.5f, -distance * 0.5f);
        subCamera.transform.LookAt(midpoint);

        // 초기에 서브 카메라 활성화
        subCamera.enabled = true;
        mainCamera.enabled = false;

        // 메인 카메라를 2초 후에 활성화하기 위한 코루틴 시작
        StartCoroutine(ActivateMainCameraAfterDelay(2f));
    }
    void Update()
    {
        // 사용자 입력을 통해 카메라 전환
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // "1" 키를 누르면 메인 카메라 활성화
            SwitchToCamera(mainCamera);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // "2" 키를 누르면 서브 카메라 활성화
            SwitchToCamera(subCamera);
        }
    }
    IEnumerator ActivateMainCameraAfterDelay(float delay)
    {
        // 지정된 시간(초)만큼 대기
        yield return new WaitForSeconds(delay);

        SwitchToCamera(mainCamera);
    }
        void SwitchToCamera(Camera camera)
    {
        // 모든 카메라를 먼저 비활성화
        mainCamera.enabled = false;
        subCamera.enabled = false;

        // 선택한 카메라만 활성화
        camera.enabled = true;
    }
}
