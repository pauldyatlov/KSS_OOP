using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public bool useECS = false;
    public bool spreadShot = false;

    [Header("General")]
    public Transform gunBarrel;

    public ParticleSystem shotVFX;
    public AudioSource shotAudio;
    public float fireRate = .1f;
    public int spreadAmount = 20;

    [Header("Bullets")]
    public GameObject bulletPrefab;

    float timer;

    EntityManager manager;
    Entity bulletEntityPrefab;

    private NativeArray<Entity> _bullets;


    void Start()
    {
        if (useECS)
        {
            var world = World.DefaultGameObjectInjectionWorld;
            manager = world.EntityManager;

            var settings =
                new GameObjectConversionSettings(world, GameObjectConversionUtility.ConversionFlags.AssignName);
            bulletEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(bulletPrefab, settings);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (Input.GetButton("Fire1") && timer >= fireRate)
        {
            Vector3 rotation = gunBarrel.rotation.eulerAngles;
            rotation.x = 0f;

            if (useECS)
            {
                if (spreadShot)
                    SpawnBulletSpreadECS(rotation);
                else
                    SpawnBulletECS(rotation);
            }
            else
            {
                if (spreadShot)
                    SpawnBulletSpread(rotation);
                else
                    SpawnBullet(rotation);
            }

            timer = 0f;

            if (shotVFX)
                shotVFX.Play();

            if (shotAudio)
                shotAudio.Play();
        }
    }

    void SpawnBullet(Vector3 rotation)
    {
        GameObject bullet = Instantiate(bulletPrefab);

        bullet.transform.position = gunBarrel.position;
        bullet.transform.rotation = Quaternion.Euler(rotation);
    }

    void SpawnBulletSpread(Vector3 rotation)
    {
        int max = spreadAmount / 2;
        int min = -max;

        Vector3 tempRot = rotation;
        for (int x = min; x < max; x++)
        {
            tempRot.x = (rotation.x + 3 * x) % 360;

            for (int y = min; y < max; y++)
            {
                tempRot.y = (rotation.y + 3 * y) % 360;

                SpawnBullet(tempRot);
            }
        }
    }

    void SpawnBulletECS(Vector3 rotation)
    {
        var entity = manager.Instantiate(bulletEntityPrefab);

        manager.SetComponentData(entity, new Translation { Value = gunBarrel.position });
        manager.SetComponentData(entity, new Rotation { Value = Quaternion.Euler(rotation) });
    }

    void SpawnBulletSpreadECS(Vector3 rotation)
    {
        int max = spreadAmount / 2;
        int min = -max;

        var totalAmount = spreadAmount * spreadAmount;
        _bullets = new NativeArray<Entity>(totalAmount, Allocator.TempJob);

        manager.Instantiate(bulletEntityPrefab, _bullets);
        var index = 0;

        Vector3 tempRot = rotation;
        for (int x = min; x < max; x++)
        {
            tempRot.x = (rotation.x + 3 * x) % 360;

            for (int y = min; y < max; y++)
            {
                tempRot.y = (rotation.y + 3 * y) % 360;

                var entity = _bullets[index];

                manager.SetComponentData(entity, new Translation { Value = gunBarrel.position });
                manager.SetComponentData(entity, new Rotation { Value = Quaternion.Euler(tempRot) });

                index++;
            }
        }

        _bullets.Dispose();
    }
}