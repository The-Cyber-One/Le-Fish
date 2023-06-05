using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorScript : MonoBehaviour
{
    public GameObject player;
    private Vector3 Restaurant;
    private Vector3 Menu;
    private float speed = 1f;
   [SerializeField] private bool notMoving,moveUp, moveDown;
    // Start is called before the first frame update
    void Start()
    {
        Restaurant = new Vector3(transform.position.x, 4, transform.position.z);
        Menu = new Vector3(transform.position.x, 0, transform.position.z);
        notMoving = true;
        moveDown = false;
        moveUp = false;
    }
    public void MoveToRestaurant()
    {
        if (notMoving)
        {
            player.transform.SetParent(transform, true);
            moveUp = true;
        }
    }
    public void MoveToMenu()
    {
        if (notMoving)
        {
            player.transform.SetParent(transform, true);
            moveDown = true;  
        }
    }
    public void Update()
    {
        if (moveUp)
        {
            notMoving = false;
            transform.position = Vector3.MoveTowards(transform.position, Restaurant, speed * Time.deltaTime);
        }
        if(transform.position.y == Restaurant.y)
        {
            notMoving = true;
            moveUp=false;
        }
        if (moveDown)
        {
            notMoving=false;
            transform.position = Vector3.MoveTowards(transform.position, Menu, speed * Time.deltaTime);
        }
        if(transform.position.y == Menu.y)
        {
            notMoving=true;
            moveDown=false; 
        }
        if(!moveUp && !moveDown)
        {
            player.transform.SetParent(null, true);
        }
    }
}
