using UnityEngine;

public class GPSCoordinatesLoader : MonoBehaviour
{
    public static Vector2 LoadGPSCoordinates()
    {
        // 텍스트 파일 로드
        TextAsset gpsData = Resources.Load<TextAsset>("GPSCoordinates");

        if (gpsData != null)
        {
            // 파일 내용을 읽고, 쉼표로 분리
            string[] coordinates = gpsData.text.Split(',');
            if (coordinates.Length >= 2)
            {
                float longitude = float.Parse(coordinates[0]);
                float latitude = float.Parse(coordinates[1]);

                return new Vector2(longitude, latitude);
            }
            else
            {
                Debug.LogError("GPS Coordinates format is incorrect.");
            }
        }
        else
        {
            Debug.LogError("GPSCoordinates.txt file not found in Resources.");
        }

        return Vector2.zero;
    }
}
