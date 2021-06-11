using System;
using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager main;

    public GameObject ballPrefab;
    
    public float xBound = 3f;
    public float yBound = 3f;
    public float ballSpeed = 3f;
    public float respawnDelay = 2f;
    public int[] playerScores;

    public Text mainText;
    public Text[] playerTexts;

    private Entity _ballEntityPrefab;
    private EntityManager _entityManager;
    private BlobAssetStore _blobStore;

    private WaitForSeconds _oneSecond;
    private WaitForSeconds _delay;

    public void PlayerScored(int playerIndex)
    {
        if (playerIndex > 1 || playerIndex < 0)
            throw new ArgumentOutOfRangeException();
        
        playerScores[playerIndex]++;
        playerTexts[playerIndex].text = playerScores[playerIndex].ToString("D");

        StartCoroutine(CountdownAndSpawnBall());
    }

    private void Awake()
    {
        if (main != null && main != this)
        {
            Destroy(gameObject);
            return;
        }

        main = this;
        playerScores = new int[2];
        
        var world = World.DefaultGameObjectInjectionWorld;
        // var conversionSystem = world.GetExistingSystem<GameObjectConversionSystem>();
        _blobStore = new BlobAssetStore();
        var settings = GameObjectConversionSettings.FromWorld(world, new BlobAssetStore());
        
        _ballEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(ballPrefab, settings);
        _entityManager = world.EntityManager;
        
        _oneSecond = new WaitForSeconds(1f);
        _delay = new WaitForSeconds(respawnDelay);

        StartCoroutine(CountdownAndSpawnBall());
    }

    private void OnDestroy()
    {
        _blobStore.Dispose();
    }

    private IEnumerator CountdownAndSpawnBall()
    {
        mainText.text = "Get Ready!";
        yield return _delay;

        mainText.text = "3";
        yield return _oneSecond;

        mainText.text = "2";
        yield return _oneSecond;

        mainText.text = "1";
        yield return _oneSecond;

        mainText.text = string.Empty;

        SpawnBall();
    }

    private void SpawnBall()
    {
        Entity ball = _entityManager.Instantiate(_ballEntityPrefab);

        Vector3 dir = new Vector3(
            UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1,
            UnityEngine.Random.Range(-0.5f, 0.5f),
            0f
        ).normalized;
        Vector3 vel = dir * ballSpeed;

        PhysicsVelocity physVel = new PhysicsVelocity()
        {
            Linear = vel,
            Angular = float3.zero
        };

        _entityManager.AddComponentData(ball, physVel);
    }
}
