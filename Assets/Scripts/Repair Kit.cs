using UnityEngine;

public class RepairKit : Collectables
{
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "Player")
        {
            CreatePopup();
            collision.GetComponent<Spaceship>().health += 20;
        }
    }
}