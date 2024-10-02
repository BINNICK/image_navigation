using UnityEngine;

public class GPSMapper : MonoBehaviour
{
    public GameObject objectToSpawn; // 유니티 에디터에서 할당할 오브젝트

    // 실제 세계의 GPS 좌표 범위
    public float longitudeMin = 127.44995f;
    public float longitudeMax = 127.46380f;
    public float latitudeMin = 36.62162f;
    public float latitudeMax = 36.6339f;

    // 유니티 내에서의 객체 크기 (미터 단위)
    // 예: 라이노에서 측정한 크기를 미터 단위로 변환한 값
    private float objectWidth = 122.4f; // X축 길이 (미터)
    private float objectDepth = 137.4f; // Z축 길이 (미터) - Y축은 사용하지 않음

    
    void Start()
    {
        if (objectToSpawn == null)
        {
            Debug.LogError("No object to spawn has been assigned.");
            return;
        }

        // GPSCoordinatesLoader에서 좌표 불러오기
        Vector2 gpsCoordinates = GPSCoordinatesLoader.LoadGPSCoordinates();
        float testLongitude = gpsCoordinates.x;
        float testLatitude = gpsCoordinates.y;

        Vector3 unityPosition = ConvertGPSToUnityPosition(testLongitude, testLatitude);
        GameObject spawnedObject = Instantiate(objectToSpawn, unityPosition, Quaternion.identity);

        // 선택적으로, 생성된 객체에 추가 설정을 적용할 수 있습니다.
        // 예를 들어, 생성된 객체의 이름을 설정하거나, 다른 컴포넌트를 추가/조정할 수 있습니다.
        spawnedObject.name = "Spawned Object at Test Location";
    }

    public Vector3 ConvertGPSToUnityPosition(float longitude, float latitude)
    {
        // 경도와 위도의 중간값 계산
        float longitudeCenter = (longitudeMax + longitudeMin) / 2;
        float latitudeCenter = (latitudeMax + latitudeMin) / 2;

        // 경도와 위도에서 중간값을 뺀 후, 스케일 비율을 적용
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
