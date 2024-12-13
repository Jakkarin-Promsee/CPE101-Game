using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAreaAttack : MonoBehaviour
{
    public List<GameObject> playersInZone = new List<GameObject>();
    private GameObject player;
    public float damage;
    public float duration;

    public void Start()
    {
        StartCoroutine(AreaAttackController());
    }

    private IEnumerator AreaAttackController()
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            foreach (GameObject player in playersInZone)
            {
                player.GetComponent<PlayerController>().TakeDamage(damage);
            }

            yield return new WaitForSeconds(0.2f);
            elapsedTime += 0.2f;
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersInZone.Add(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersInZone.Remove(other.gameObject);
        }
    }
}
