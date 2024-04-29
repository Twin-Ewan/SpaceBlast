using UnityEngine;

public class PlayerController : Spaceship
{
    void Update()
    {
        if (Input.GetButton("Fire1")) Shoot();
        if (Input.GetKeyDown(KeyCode.R)) StartCoroutine(Reload());

        float moveSpeedFactor = 10;
        float verticalInput = Input.GetAxis("Vertical");
        // Gets the value of the Vertical input axis (supports controller and such)

        float newYPos = transform.position.y + verticalInput * moveSpeedFactor * Time.deltaTime;

        if (newYPos > -1.9f && newYPos < 5)
        {
            transform.Translate(new Vector3(0, verticalInput, 0) * moveSpeedFactor * Time.deltaTime);
        }
    }
}
