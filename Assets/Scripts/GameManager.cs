using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject SSPrefab;
    [SerializeField] GameObject[] Collectable;
    [SerializeField] GameObject Asteroid;

    Vector2 SpawnPos = new Vector2(10, 0);
    public List<GameObject> Enemies = new List<GameObject>();
    GameObject Ship;
    public int score { get; set; } = 0;
    float difficulty = 0;

    Spaceship Player;
    [SerializeField] Text AmmoText, GameScoreText;

    [SerializeField] TextMeshProUGUI[] ShipKilledText;
    [SerializeField] int[] ShipKilledCount = new int[4];

    [SerializeField] TextMeshProUGUI HighscoreText, ScoreText;
    [SerializeField] GameObject GameplayCanvas, HighscoreCanvas;

    public void BTN_Quit()
    {
        Application.Quit();
    }

    public void BTN_Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Gameplay");
    }

    void Update()
    {
        if(Player == null) GameOver();
        else
        {
            AmmoText.text = "Ammo: " + Player.ammo.ToString() + "/" + Player.maxAmmo.ToString();
            GameScoreText.text = "Score: " + score.ToString();

            for (int i = 0; i < Enemies.Count; i++) if (Enemies[i] == null) Enemies.RemoveAt(i);
        }
    }

    void Start()
    {
        Player = GameObject.Find("Player").GetComponent<Spaceship>();

        difficulty = 10;
        SpawnSingle(-4);

        // Spawns a enemy every 7s from diffculty 0 - 4, every 6s 5 - 9 and 5s at 10
        //InvokeRepeating("ChooseEnemy", 1, 7 - difficulty/5);
    }

    public void GameOver()
    {

        GameplayCanvas.SetActive(false);
        HighscoreCanvas.SetActive(true);
        Time.timeScale = 0;

        if (PlayerPrefs.GetFloat("Highscore") < score) PlayerPrefs.SetFloat("Highscore", score);
 
        HighscoreText.text = "Highscore: " + PlayerPrefs.GetFloat("Highscore").ToString();
        ScoreText.text = "Score: " + score.ToString();
    }

    public void UpdateKillCount(int shipType)
    {
        ShipKilledCount[shipType - 1]++;

        ShipKilledText[0].text = "Basic: " + ShipKilledCount[0];
        ShipKilledText[1].text = "Glass: " + ShipKilledCount[1];
        ShipKilledText[2].text = "Burst: " + ShipKilledCount[2];
        ShipKilledText[3].text = "Strong: " + ShipKilledCount[3];
    }

    void ChooseEnemy()
    {
        if (difficulty < 10) difficulty += .3f;
        float Speed = Random.Range(-3f, -1f);

        // 25% chance to spawn a collectable
        if (Random.Range(0, 4) == 1) SpawnPowerup();

        // 25% chance to spawn a hazard
        if (Random.Range(0, 4) == 1) SpawnHazard();


        int i = Random.Range(0, (int)difficulty / 2);
        if (i == 0) SpawnStaggered(Speed);
        if (i == 1) SpawnPair(Speed);
        if (i == 2) SpawnSplit(Speed);
        if (i == 3) SpawnChevron(Speed);
        if (i == 4) SpawnSingle(Speed);
    }

    void SpawnPowerup()
    {
        GameObject GOCollect = null;
        int Index = Random.Range(0, 3);
        SpawnPos = new Vector2(10, Random.Range(-1.5f, 5f));

        if (Index == 0) GOCollect = Instantiate(Collectable[0], SpawnPos, Quaternion.identity);
        if (Index == 1) GOCollect = Instantiate(Collectable[1], SpawnPos, Quaternion.identity);
        if (Index == 2) GOCollect = Instantiate(Collectable[2], SpawnPos, Quaternion.identity);

        GOCollect.GetComponent<Rigidbody>().velocity = new Vector2(-2, 0);
        Destroy(GOCollect, 12);
    }

    void SpawnHazard()
    {
        GameObject GOHazard = null;
        SpawnPos = new Vector2(10, Random.Range(-1.5f, 5f));

        GOHazard = Instantiate(Asteroid, SpawnPos, Quaternion.identity);

        Vector3 RotationSpeed = new Vector3(Random.Range(0, 2f), Random.Range(0, 2f), Random.Range(0, 2f));
        GOHazard.GetComponent<Hazard>().damage = 3;
        GOHazard.GetComponent<Rigidbody>().angularDrag = 0;
        GOHazard.GetComponent<Rigidbody>().angularVelocity = RotationSpeed;
        GOHazard.GetComponent<Rigidbody>().velocity = new Vector2(-3, Random.Range(-.5f, .5f));

        Destroy(GOHazard, 12);
    }

    void SpawnEnemy(Vector2 SpawnPos, int type, float Speed)
    {
        // Stops the type from being 0 (Player) and outside of the max enum (currently 4)
        type = Mathf.Clamp(type, 1, Enum.GetNames(typeof(Spaceship.shipType)).Length - 1);

        Ship = Instantiate(SSPrefab, SpawnPos, Quaternion.identity);
        Ship.GetComponent<Spaceship>().shipClass = (Spaceship.shipType)type;
        Ship.GetComponent<EnemySpaceship>().Speed = Speed;
        Enemies.Add(Ship);
    }

    void SpawnPair(float Speed)
    {
        SpawnPos = new Vector2(SpawnPos.x, Random.Range(-1.5f, 3f));

        print("Pair, " + SpawnPos);

        SpawnPos.y++;
        SpawnEnemy(SpawnPos, 1, Speed);

        SpawnPos.y++;
        SpawnEnemy(SpawnPos, 2, Speed);
    }

    void SpawnStaggered(float Speed)
    {

        // Will select a random number of ships based on the diffculty;
        // Diffculty 0 - 5 = 1, 6 - 7 = 2 & 8 - 10 = 3
        int numOfShips = Random.Range(1, Mathf.Clamp((int)difficulty / 2, 0, 4));
        SpawnPos = new Vector2(SpawnPos.x, Random.Range(-1.5f, 5f - numOfShips));

        // Sets the Ship's type using the diffculty so 0 - 3 Spawns "Basic", 4 - 6 "Burst"
        // 7 - 8 "Glass" and 9 - 10 "Strong"
        int shipType = (int)(difficulty / 2.2f);

        print("Staggered " + SpawnPos + ", Type: " + (Spaceship.shipType)shipType + ", Num: " + numOfShips);
        for (int i = 0; i < numOfShips; i++)
        {
            SpawnEnemy(SpawnPos, shipType, Speed);
            SpawnPos.x++;
            SpawnPos.y++;
        }
    }

    void SpawnSplit(float Speed)
    {
        print("Split");

        SpawnEnemy(new Vector2(SpawnPos.x, 5f), 2, Speed);
        SpawnEnemy(new Vector2(SpawnPos.x, -1.5f), 3, Speed);
    }

    void SpawnChevron(float Speed)
    {
        SpawnPos = new Vector2(SpawnPos.x, Random.Range(.5f, 5f));
        SpawnEnemy(SpawnPos, 2, Speed);

        SpawnPos += new Vector2(-1, -1);
        SpawnEnemy(SpawnPos, 3, Speed);

        SpawnPos += new Vector2(1, -1);
        SpawnEnemy(SpawnPos, 2, Speed);
    }

    void SpawnSingle(float Speed) 
    {
        SpawnPos = new Vector2(SpawnPos.x, Random.Range(-1.5f, 5f));
        SpawnEnemy(SpawnPos, 4, Speed);
    }
}
