using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinSpawner : MonoBehaviour
{
    public int coinCount = 10; 
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private RectTransform targetPoint;
    [SerializeField] private float delayBetweenCoins = 0.5f;

    public event Action CoinReachedTarget;
    public static CoinSpawner Instance;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
 
   public void SpawnCoins()
    {
        for (int i = 0; i < coinCount; i++)
        {
            float randomX = Random.Range(spawnArea.rect.xMin, spawnArea.rect.xMax);
            float randomY = Random.Range(spawnArea.rect.yMin, spawnArea.rect.yMax);

            Vector2 localPosition = new Vector2(randomX, randomY);
            Vector3 worldPosition = spawnArea.TransformPoint(localPosition);

            GameObject coin = Instantiate(coinPrefab, worldPosition, Quaternion.identity, spawnArea);

            StartCoroutine(MoveCoinToTarget(coin, i * delayBetweenCoins +2));
        }
    }

    IEnumerator MoveCoinToTarget(GameObject coin, float delay)
    {
        yield return new WaitForSeconds(delay);

        CoinMover mover = coin.GetComponent<CoinMover>();
        if (mover != null)
        {
            mover.OnReachedTarget += HandleCoinReachedTarget;
            mover.MoveToTarget(targetPoint.position);
        }
    }

    void HandleCoinReachedTarget(GameObject coin)
    {
        CoinReachedTarget?.Invoke();
        Destroy(coin);
    }
}
