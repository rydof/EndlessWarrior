using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCoins : MonoBehaviour
{
    [SerializeField] private int money = 0;

    public int Money
    {
        get => money;
        set
        {
            money = value;
            OnMoneyChanged?.Invoke(money);
        }
    }

    public delegate void MoneyChanged(int money);
    public event MoneyChanged OnMoneyChanged;

    private Rigidbody2D playerRb;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        if (playerRb == null)
            Debug.LogError("PlayerCoins: No Rigidbody2D found on the player!");
        else
            OnMoneyChanged?.Invoke(Money);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.CompareTag("Coin"))
        {
            Money++;
            Destroy(other.gameObject);
        }
    }

    public IEnumerator MoveCoinsToPlayer()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        List<Coroutine> moveCoroutines = new();

        foreach (var coin in coins)
        {
            if (coin == null) continue;

            Rigidbody2D coinRb = coin.GetComponent<Rigidbody2D>();
            if (coinRb == null) continue;

            moveCoroutines.Add(StartCoroutine(MoveCoinToPlayer(coinRb)));
        }

        foreach (var coroutine in moveCoroutines)
            yield return coroutine;
    }

    private IEnumerator MoveCoinToPlayer(Rigidbody2D coinRb)
    {
        if (coinRb == null) yield break;

        float moveDuration = 0.5f;
        float elapsedTime = 0f;
        Vector2 start = coinRb.position;
        Vector2 target = playerRb.position;

        while (elapsedTime < moveDuration)
        {
            if (coinRb == null || playerRb == null) yield break;

            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            coinRb.MovePosition(Vector2.Lerp(start, target, t));
            yield return null;
        }

        coinRb.MovePosition(playerRb.position);
    }

    public int GetUncollectedMoney()
    {
        return GameObject.FindGameObjectsWithTag("Coin").Length;
    }
}
