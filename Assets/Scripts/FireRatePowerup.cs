using System.Collections;
using UnityEngine;

public class FireRatePowerup : Collectables
{
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "Player")
        {
            CreatePopup();
            HalfFireRate();
        }
    }

    IEnumerable HalfFireRate() 
    {
        GameObject.Find("Player").GetComponent<PlayerController>().fireRate *= .5f;
        yield return new WaitForSeconds(5);
        GameObject.Find("Player").GetComponent<PlayerController>().fireRate *= 2;
    }
}
