using UnityEngine;

public class RayGun : MonoBehaviour
{
    [SerializeField] public OVRInput.RawButton shootingButton;
    [SerializeField] public LineRenderer laserPrefab;
    [SerializeField] public GameObject rayImpactPrefab;
    [SerializeField] public Transform shootingPoint;
    [SerializeField] public LayerMask layerMask;
    [SerializeField] public AudioSource laserSound;
    [SerializeField] public AudioClip laserSoundClip;
    [SerializeField] public float maxDistance = 5f;
    [SerializeField] public float laserShowTimer = 0.3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Util.AssertObject(laserPrefab, "RayGun - Laser prefab is not assigned");
        Util.AssertObject(rayImpactPrefab, "RayGun - Ray impact prefab is not assigned");
        Util.AssertObject(shootingPoint, "RayGun - Shooting point is not assigned");
        Util.AssertObject(laserSound, "RayGun - Laser sound is not assigned");
        Util.AssertObject(laserSoundClip, "RayGun - Laser sound clip is not assigned");
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
        laserSound.PlayOneShot(laserSoundClip);
        // create a raycast
        Ray ray = new Ray(shootingPoint.position, shootingPoint.forward);
        Vector3 endPoint = Vector3.zero;
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
        {
            endPoint = hit.point;
            // we use minus cause its opposite of the normal
            Quaternion rotation = Quaternion.LookRotation(-hit.normal);
            GameObject rayImpact = Instantiate(rayImpactPrefab, hit.point, rotation);
            Destroy(rayImpact, 1);
        }
        else
        {
            endPoint = shootingPoint.position + shootingPoint.forward * maxDistance;
        }

        // create a laser line
        LineRenderer laser = Instantiate(laserPrefab);
        laser.positionCount = 2;
        laser.SetPosition(0, shootingPoint.position);
        laser.SetPosition(1, endPoint);
        Destroy(laser.gameObject, laserShowTimer);
    }
}
