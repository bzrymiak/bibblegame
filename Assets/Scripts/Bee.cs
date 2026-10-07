using UnityEngine;

// touching a bee ends the run outright - same as letting the swarm edge catch you.
// the blueberry doesn't save you from this, only from obstacles and enemies
[RequireComponent(typeof(CircleCollider2D))]
public class Bee : MonoBehaviour
{
    public string caughtMessage = "The bees caught Bibble!";

    private void Reset()
    {
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<Player>() == null) return;
        if (GameManager.Instance == null) return;

        GameManager.Instance.LoseRun(caughtMessage);
    }
}
