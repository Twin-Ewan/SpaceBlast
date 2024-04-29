using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int damage { get; set; }
    protected int health = 50;

    void OnTriggerEnter(Collider collision)
    {   
        if (collision.gameObject.GetComponent<Spaceship>() != null) Destroy(this.gameObject);
    }
}
