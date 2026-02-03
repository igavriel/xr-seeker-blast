using UnityEngine;

public class RayGun : MonoBehaviour
{
    [SerializeField] public OVRInput.RawButton shootingButton;
    [SerializeField] public LineRenderer laserPrefab;
    [SerializeField] public Transform shootingPoint;
    [SerializeField] public float maxDistance = 5f;
    [SerializeField] public float laserShowTimer = 0.3f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Util.AssertObject(laserPrefab, "Laser prefab is not assigned");
        Util.AssertObject(shootingPoint, "Shooting point is not assigned");
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(shootingButton))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        LineRenderer laser = Instantiate(laserPrefab);
        laser.positionCount = 2;
        laser.SetPosition(0, shootingPoint.position);
        Vector3 endPoint = shootingPoint.position + shootingPoint.forward * maxDistance;
        laser.SetPosition(1, endPoint);
        Destroy(laser.gameObject, laserShowTimer);
    }
}
