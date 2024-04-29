using UnityEngine;

public class Collectables : MonoBehaviour
{
    [SerializeField] protected Sprite Icon;

    void Start()
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = Icon;
    }


    protected void CreatePopup()
    {
        var popupObj = new GameObject(this.name + " TextPopup");
        Vector3 pos = this.transform.position;
        pos.z = -1;
        popupObj.transform.position = pos;
        popupObj.transform.localScale = new Vector3(0.2f, 0.2f, 1);

        popupObj.AddComponent<TextMesh>();
        popupObj.AddComponent<Rigidbody2D>();
         
        var popupText = popupObj.GetComponent<TextMesh>();

        popupText.anchor = TextAnchor.MiddleCenter;
        popupText.fontStyle = FontStyle.Bold;
        popupText.text = (this.name).ToUpper();

        popupObj.GetComponent<Rigidbody2D>().gravityScale = 0;
        popupObj.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 1);

        Destroy(popupObj, .5f);
        Destroy(gameObject);
    }
}
