using UnityEngine;
using System.Collections;

public class FakeVR : MonoBehaviour {

    public bool useCompass = true; // allows for more accurate tracking, but requires location services / GPS to be enabled by the user
    public float horizontalMultiplier = 360f; // when not using compass: tilting the phone determines y-axis rotation. this value sets the speed
    public float harshness = 7f; //  Higher values mean faster reactions, but less smoothness. values between 5 and 20 seem to work well.

    private float xPos = 0;

    void Start ()
    {
        if (useCompass)
        {
            if (!Input.location.isEnabledByUser)
            {
                useCompass = false;
                Debug.LogError("Location services are not enabled!");
            }

            Input.location.Start();
            Input.compass.enabled = true;
        }
    }

	void Update () {
        if (!useCompass)
        {
            xPos += Input.acceleration.x * Time.deltaTime * horizontalMultiplier;
            xPos = (xPos + 360f) % 360f;
        }
        else
        {
            xPos = Input.compass.trueHeading;
        }

        Vector3 projectedAccel = Vector3.ProjectOnPlane(Input.acceleration.normalized, Vector3.right);

        Quaternion rot = Quaternion.FromToRotation(Vector3.down, projectedAccel.normalized);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rot.eulerAngles.x, xPos, rot.eulerAngles.z), harshness * Time.deltaTime);
    }
}
