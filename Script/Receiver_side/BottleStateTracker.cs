using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BottleStateTracker : MonoBehaviour
{
    public GameObject tracker1; //left
    public GameObject tracker2; //right
    public GameObject tracker3; //center
    public GameObject bottleTrack;
    public bool isPouring;
   

    int layerMask = 1 << 8;
    float ray_distance = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        isPouring = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = tracker1.transform.TransformDirection(Vector3.forward) * ray_distance;
        Debug.DrawRay(tracker1.transform.position, forward, Color.yellow);

        forward = tracker2.transform.TransformDirection(Vector3.forward) * ray_distance;
        Debug.DrawRay(tracker2.transform.position, forward, Color.green);

        forward = tracker3.transform.TransformDirection(Vector3.forward) * (ray_distance + 0.5f);
        Debug.DrawRay(tracker3.transform.position, forward, Color.red);

    }

    private void FixedUpdate()
    {
        RaycastHit hit_left;
        bool hit_result_1 = Physics.Raycast(tracker1.transform.position, tracker1.transform.TransformDirection(Vector3.forward), out hit_left, ray_distance * 100, layerMask);
        RaycastHit hit_right;
        bool hit_result_2 = Physics.Raycast(tracker2.transform.position, tracker2.transform.TransformDirection(Vector3.forward), out hit_right, ray_distance * 100, layerMask);
        RaycastHit hit_center;
        bool hit_result_3 = Physics.Raycast(tracker3.transform.position, tracker3.transform.TransformDirection(Vector3.forward), out hit_center, ray_distance * 100, layerMask);

        // Does the ray intersect any objects excluding the player layer
        if (hit_result_1 || hit_result_2 || hit_result_3)
        {
            if ((hit_right.collider.tag == "pouringPoint") && (hit_right.distance <= ray_distance))
            {
                if ((bottleTrack.transform.rotation.z <= 1 && bottleTrack.transform.rotation.z > 0.5) ||
                    (bottleTrack.transform.rotation.z <= -0.5 && bottleTrack.transform.rotation.z > -1))
                {
                    //Debug.Log("rotation z = " + bottle.transform.rotation.z);
                    isPouring = true;
                }
                isPouring = true;
            }
            else if((hit_left.collider.tag == "pouringPoint") && (hit_left.distance <= ray_distance))
            {
                if ((bottleTrack.transform.rotation.z <= 1 && bottleTrack.transform.rotation.z > 0.5) ||
                    (bottleTrack.transform.rotation.z <= -0.5 && bottleTrack.transform.rotation.z > -1))
                {
                    //Debug.Log("rotation z = " + bottle.transform.rotation.z);
                    isPouring = true;
                    //Debug.Log("Did Hit");
                }
                isPouring = true;
            }
            else if ((hit_center.collider.tag == "pouringPoint") && (hit_center.distance <= ray_distance + 0.5f))
            {
                if ((bottleTrack.transform.rotation.z <= 1 && bottleTrack.transform.rotation.z > 0.5) ||
                    (bottleTrack.transform.rotation.z <= -0.5 && bottleTrack.transform.rotation.z > -1))
                {
                    //Debug.Log("rotation z = " + bottle.transform.rotation.z);
                    isPouring = true;
                    //Debug.Log("Did Hit");
                }
                isPouring = true;

            }
        }
        else
        {
            isPouring = false;
        }
    }
}
