using UnityEngine;

public class EnemySpaceship : Spaceship
{
    public float Speed { get; set; }
    public EnemySpaceship() { direction = -1; }

    void Update()
    {
        Shoot();
        if (this.transform.position.x < -10) Destroy(this.gameObject);
        this.GetComponent<Rigidbody>().velocity = new Vector2(Speed, 0);
    }
}
