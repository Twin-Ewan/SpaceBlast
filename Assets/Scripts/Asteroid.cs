using UnityEngine;

public class Asteroid : Hazard
{
    [SerializeField] AudioClip Explosion;
    [SerializeField] GameObject explosionPrefab;

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null || 
            collision.gameObject.GetComponent<Hazard>() != null)
        {
            GameObject GOExplode = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
            GOExplode.AddComponent<AudioSource>();
            GOExplode.GetComponent<AudioSource>().PlayOneShot(Explosion, PlayerPrefs.GetFloat("Volume"));

            Destroy(GOExplode, 1);
            Destroy(gameObject);
        }
    }
}
