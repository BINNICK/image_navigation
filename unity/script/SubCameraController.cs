using UnityEngine;
using System.Collections;

public class SubCameraController : MonoBehaviour
{
    public Transform agentTransform; // ������Ʈ�� Transform
    public GPSMapper gpsMapper; // GPSMapper ��ũ��Ʈ�� �ִ� ������Ʈ
    public Camera mainCamera; // ���� ī�޶�
    public Camera subCamera; // ���� ī�޶�
    private Vector2 targetGPSCoordinates; // ���Ͽ��� �ҷ��� ������ GPS ��ǥ

    void Start()
    {
        // GPSCoordinatesLoader���� �������� GPS ��ǥ �ҷ�����
        targetGPSCoordinates = GPSCoordinatesLoader.LoadGPSCoordinates();

        // GPS ��ǥ�� Unity ���� ��ǥ�� ��ȯ
        Vector3 destination = gpsMapper.ConvertGPSToUnityPosition(targetGPSCoordinates.x, targetGPSCoordinates.y);

        // ������Ʈ�� ������ ������ �߰� ���� ���
        Vector3 midpoint = (agentTransform.position + destination) / 2.0f;

        // ������Ʈ�� ������ ������ �Ÿ��� ���� ī�޶� ��ġ ����
        float distance = Vector3.Distance(agentTransform.position, destination);
        subCamera.transform.position = midpoint + new Vector3(0, distance * 0.5f, -distance * 0.5f);
        subCamera.transform.LookAt(midpoint);

        // �ʱ⿡ ���� ī�޶� Ȱ��ȭ
        subCamera.enabled = true;
        mainCamera.enabled = false;

        // ���� ī�޶� 2�� �Ŀ� Ȱ��ȭ�ϱ� ���� �ڷ�ƾ ����
        StartCoroutine(ActivateMainCameraAfterDelay(2f));
    }
    void Update()
    {
        // ����� �Է��� ���� ī�޶� ��ȯ
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // "1" Ű�� ������ ���� ī�޶� Ȱ��ȭ
            SwitchToCamera(mainCamera);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // "2" Ű�� ������ ���� ī�޶� Ȱ��ȭ
            SwitchToCamera(subCamera);
        }
    }
    IEnumerator ActivateMainCameraAfterDelay(float delay)
    {
        // ������ �ð�(��)��ŭ ���
        yield return new WaitForSeconds(delay);

        SwitchToCamera(mainCamera);
    }
        void SwitchToCamera(Camera camera)
    {
        // ��� ī�޶� ���� ��Ȱ��ȭ
        mainCamera.enabled = false;
        subCamera.enabled = false;

        // ������ ī�޶� Ȱ��ȭ
        camera.enabled = true;
    }
}
