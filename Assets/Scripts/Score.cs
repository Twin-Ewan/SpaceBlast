using UnityEngine;

public class Score : Collectables
{
    [SerializeField] int scoreAmount = 2000;
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "Player")
        {
            CreatePopup();
            GameObject.Find("GameManager").GetComponent<GameManager>().score += scoreAmount;
        }
    }
}
