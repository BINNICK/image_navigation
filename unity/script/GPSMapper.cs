using UnityEngine;

public class GPSMapper : MonoBehaviour
{
    public GameObject objectToSpawn; // ����Ƽ �����Ϳ��� �Ҵ��� ������Ʈ

    // ���� ������ GPS ��ǥ ����
    public float longitudeMin = 127.44995f;
    public float longitudeMax = 127.46380f;
    public float latitudeMin = 36.62162f;
    public float latitudeMax = 36.6339f;

    // ����Ƽ �������� ��ü ũ�� (���� ����)
    // ��: ���̳뿡�� ������ ũ�⸦ ���� ������ ��ȯ�� ��
    private float objectWidth = 122.4f; // X�� ���� (����)
    private float objectDepth = 137.4f; // Z�� ���� (����) - Y���� ������� ����

    
    void Start()
    {
        if (objectToSpawn == null)
        {
            Debug.LogError("No object to spawn has been assigned.");
            return;
        }

        // GPSCoordinatesLoader���� ��ǥ �ҷ�����
        Vector2 gpsCoordinates = GPSCoordinatesLoader.LoadGPSCoordinates();
        float testLongitude = gpsCoordinates.x;
        float testLatitude = gpsCoordinates.y;

        Vector3 unityPosition = ConvertGPSToUnityPosition(testLongitude, testLatitude);
        GameObject spawnedObject = Instantiate(objectToSpawn, unityPosition, Quaternion.identity);

        // ����������, ������ ��ü�� �߰� ������ ������ �� �ֽ��ϴ�.
        // ���� ���, ������ ��ü�� �̸��� �����ϰų�, �ٸ� ������Ʈ�� �߰�/������ �� �ֽ��ϴ�.
        spawnedObject.name = "Spawned Object at Test Location";
    }

    public Vector3 ConvertGPSToUnityPosition(float longitude, float latitude)
    {
        // �浵�� ������ �߰��� ���
        float longitudeCenter = (longitudeMax + longitudeMin) / 2;
        float latitudeCenter = (latitudeMax + latitudeMin) / 2;

        // �浵�� �������� �߰����� �� ��, ������ ������ ����
        float xPosition = (longitude - longitudeCenter) * (objectWidth / (longitudeMax - longitudeMin));
        float zPosition = (latitude - latitudeCenter) * (objectDepth / (latitudeMax - latitudeMin));

        return new Vector3(xPosition, 0, zPosition);
    }
    public Vector2 ConvertUnityToGPSCoordinates(Vector3 unityPosition)
    {
        float longitudeCenter = (longitudeMax + longitudeMin) / 2;
        float latitudeCenter = (latitudeMax + latitudeMin) / 2;
        float longitude = ((unityPosition.x * (longitudeMax - longitudeMin)) / objectWidth) + longitudeCenter;
        float latitude = ((unityPosition.z * (latitudeMax - latitudeMin)) / objectDepth) + latitudeCenter;
        return new Vector2(longitude, latitude);
    }


}
