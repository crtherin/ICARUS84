using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    protected void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (!collider2D.CompareTag("Player"))
            return;

        PlayerPersistence playerPersistence = collider2D.GetComponent<PlayerPersistence>();

        if (playerPersistence == null)
            return;

        // playerPersistence.SetCheckpoint(this);
        // PersistenceManager.Save();
    }
}