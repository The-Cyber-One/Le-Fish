using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScreenScript : MonoBehaviour
{
    private Vector3 Restaurant;
    private Vector3 Menu;
    private float speed = 1f;
    public float restaurantY;
    public float menuY;
    public ParticleSystem menuSmoke;
    [SerializeField] private bool notMoving, moveUp, moveDown;
    // Start is called before the first frame update
    void Start()
    {
        Restaurant = new Vector3(transform.position.x, restaurantY, transform.position.z);
        Menu = new Vector3(transform.position.x, menuY, transform.position.z);
        notMoving = true;
        moveDown = false;
        moveUp = false;
    }
    public void MoveToRestaurant()
    {
        if (notMoving)
        {
            moveUp = true;
        }
    }
    public void MoveToMenu()
    {
        if (notMoving)
        {
            moveDown = true;
        }
    }
    public void Update()
    {
        if (moveUp)
        {
            menuSmoke.Play();
            notMoving = false;
            transform.position = Vector3.MoveTowards(transform.position, Restaurant, speed * Time.deltaTime);
        }
        if (transform.position.y == Restaurant.y)
        {
            menuSmoke.Stop();
            notMoving = true;
            moveUp = false;
        }
        if (moveDown)
        {
            menuSmoke.Play();
            notMoving = false;
            transform.position = Vector3.MoveTowards(transform.position, Menu, speed * Time.deltaTime);
        }
        if (transform.position.y == Menu.y)
        {
            menuSmoke.Stop();
            notMoving = true;
            moveDown = false;
        }
    }
}
