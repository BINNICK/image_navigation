using UnityEngine;

public class GPSCoordinatesLoader : MonoBehaviour
{
    public static Vector2 LoadGPSCoordinates()
    {
        // �ؽ�Ʈ ���� �ε�
        TextAsset gpsData = Resources.Load<TextAsset>("GPSCoordinates");

        if (gpsData != null)
        {
            // ���� ������ �а�, ��ǥ�� �и�
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
