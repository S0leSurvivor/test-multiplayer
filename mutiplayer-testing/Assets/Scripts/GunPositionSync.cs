using UnityEngine;
using UnityEngine.Networking;

public class GunPositionSync : NetworkBehaviour
{
    [SerializeField] Transform cameraTransform;
    //[SerializeField] Transform handMount;
    [SerializeField] Transform gunPivot;
    [SerializeField] float threshold = 10f;
    [SerializeField] float smoothing = 5f;

    [SyncVar] float pitch;
    Vector3 lastOffset;
    float lastSyncedPitch;

    void Start()
    {

        if (isLocalPlayer)
            gunPivot.parent = cameraTransform;
        //else
          //  lastOffset = handMount.position - transform.position;
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            //this will give the x rotation of the camera
            pitch = cameraTransform.localRotation.eulerAngles.x;
            if (Mathf.Abs(lastSyncedPitch - pitch) >= threshold)
            {
                CmdUpdatePitch(pitch);
                lastSyncedPitch = pitch;
            }
        }
        else
        {
            Quaternion newRotation = Quaternion.Euler(pitch, 0f, 0f);

            //Vector3 currentOffset = handMount.position - transform.position;
            //gunPivot.localPosition += currentOffset - lastOffset;
            //lastOffset = currentOffset;

            // This has to be done inside update in order to maintain a smooth rotation
            gunPivot.localRotation = Quaternion.Lerp(gunPivot.localRotation,
                newRotation, Time.deltaTime * smoothing);
        }
    }

    [Command]
    void CmdUpdatePitch(float newPitch)
    {
        pitch = newPitch;
    }

}