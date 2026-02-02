using UnityEngine;

public class RayGun : MonoBehaviour
{
    public OVRInput.RawButton shootingButton;
    public LineRenderer laserPrefab;
    public Transform shootingPoint;
    public float maxDistance = 5f;
    public float laserShowTimer = 0.3f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Util.AssertObject(laserPrefab, "Laser prefab is not assigned");
        Util.AssertObject(shootingPoint, "Shooting point is not assigned");
        Util.AssertObject(shootingButton, "Shooting button is not assigned");
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(shootingButton))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        LineRenderer laser = Instantiate(laserPrefab);
        laser.positionCount = 2;
        laser.SetPosition(0, shootingPoint.position);
        Vector3 endPoint = shootingPoint.position + transform.forward * maxDistance;
        laser.SetPosition(1, endPoint);
        Destroy(laser.gameObject, laserShowTimer);
    }
}
