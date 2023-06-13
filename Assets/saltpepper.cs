using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class saltpepper : MonoBehaviour
{
    public GameObject saltORpepper;
    public bool salt = true;
    public float rotationSpeed = 90f; // Adjust the speed of rotation as desired

    private Quaternion targetRotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Set the initial rotation target based on the initial value of salt
        if (salt)
        {
            targetRotation = Quaternion.Euler(-90, 180, 0);
        }
        else
        {
            targetRotation = Quaternion.Euler(-90, 0, 0);
        }
        // Smoothly rotate towards the target rotation
        saltORpepper.transform.rotation = Quaternion.Lerp(saltORpepper.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void TurnPepperORSaltMode()
    {
        salt = !salt; // Toggle the salt variable
    }
}
