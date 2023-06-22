using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorScript : MonoBehaviour
{
    public GameObject player;
    private Vector3 Restaurant;
    private Vector3 Menu;
    private float speed = 1f;
    public float restaurantY;
    public float menuY;
    public bool NotMoving;
    [SerializeField] private bool moveUp, moveDown;
    // Start is called before the first frame update
    void Start()
    {
        Restaurant = new Vector3(transform.position.x, restaurantY, transform.position.z);
        Menu = new Vector3(transform.position.x, menuY, transform.position.z);
        NotMoving = true;
        moveDown = false;
        moveUp = false;
    }
    public void MoveUp()
    {
        if (NotMoving)
        {
            player.transform.SetParent(transform, true);
            moveUp = true;
        }
    }
    public void MoveDown()
    {
        if (NotMoving)
        {
            player.transform.SetParent(transform, true);
            moveDown = true;
        }
    }
    public void Update()
    {
        if (moveUp)
        {
            NotMoving = false;
            transform.position = Vector3.MoveTowards(transform.position, Restaurant, speed * Time.deltaTime);
        }
        if (transform.position.y == Restaurant.y)
        {
            NotMoving = true;
            moveUp = false;
        }
        if (moveDown)
        {
            NotMoving = false;
            transform.position = Vector3.MoveTowards(transform.position, Menu, speed * Time.deltaTime);
        }
        if (transform.position.y == Menu.y)
        {
            NotMoving = true;
            moveDown = false;
        }
        if (!moveUp && !moveDown)
        {
            player.transform.SetParent(null, true);
        }
    }
}
