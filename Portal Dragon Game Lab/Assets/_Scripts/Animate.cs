using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate : MonoBehaviour
{
    [SerializeField]
    private Vector3[] nonAnimatedPositions;
    [SerializeField]
    private Transform[] partTransforms;

    [SerializeField]
    private float movementSpeedX = 2f;
    [SerializeField]
    private float waveHeightX = 0.9f;
    [SerializeField]
    private float waveFrequencyX = 1f;

    [SerializeField]
    private float movementSpeedY = 2f;
    [SerializeField]
    private float waveHeightY = 0.9f;
    [SerializeField]
    private float waveFrequencyY = 1f;

    [SerializeField]
    private bool ready = false;

    public GameObject target;
    [SerializeField]
    private float rotateSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        partTransforms = new Transform[22];
        nonAnimatedPositions = new Vector3[partTransforms.Length];
        GetDragonParts();
    }

    void GetDragonParts()
    {
        int children = transform.childCount;
        Transform currentChildTransform = transform;
        GameObject currentChild = transform.gameObject;
        int counter = 0;

        //spine
        for (int i = 0; i < children; i++)
        {
            if (currentChildTransform.childCount > 0)
            {
                if (currentChildTransform.GetChild(0).gameObject.tag == "Dragonbody")
                    currentChild = currentChildTransform.GetChild(0).gameObject;
                else if (currentChildTransform.GetChild(1).gameObject.tag == "Dragonbody")
                    currentChild = currentChildTransform.GetChild(1).gameObject;

                if (currentChildTransform.GetChild(0).gameObject.tag == "Dragonbody" || currentChildTransform.GetChild(1).gameObject.tag == "Dragonbody")
                {
                    currentChildTransform = currentChild.transform;
                    partTransforms[i] = currentChildTransform;
                    nonAnimatedPositions[i] = partTransforms[i].position;
                    children++;
                    counter++;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }

        children = transform.childCount;
        currentChildTransform = transform;
        currentChild = transform.gameObject;

        //hips
        for (int i = counter; i < children + counter; i++)
        {
            if (currentChildTransform.childCount > 0)
            {
                if (currentChildTransform.GetChild(0).gameObject.tag == "Dragonbody" && i != counter)
                {
                    currentChild = currentChildTransform.GetChild(0).gameObject;
                    currentChildTransform = currentChild.transform;
                    partTransforms[i] = currentChildTransform;
                    nonAnimatedPositions[i] = partTransforms[i].position;
                    children++;
                }
                else if (currentChildTransform.childCount > 1)
                {
                    if(currentChildTransform.GetChild(1).gameObject.tag == "Dragonbody")
                    {
                        currentChild = currentChildTransform.GetChild(1).gameObject;
                        currentChildTransform = currentChild.transform;
                        partTransforms[i] = currentChildTransform;
                        nonAnimatedPositions[i] = partTransforms[i].position;
                        children++;
                    }
                }
                else
                {
                    ready = true;
                    break;
                }
            }
            else
            {
                ready = true;
                break;
            }
        }
    }

    void AnimateParts()
    {
        for (int i = 0; i < partTransforms.Length; i++)
        {
            Vector3 pos = partTransforms[i].position;
            
            //float offset = waveHeight * Mathf.Sin((Time.time * movementSpeed) + (i * waveFrequency));
            float offsetX = waveHeightX * Mathf.Sin(Time.time * movementSpeedX + (i * waveFrequencyX));
            float offsetY = waveHeightY * Mathf.Sin(Time.time * movementSpeedY + (i * waveFrequencyY));

            partTransforms[i].position = new Vector3(-offsetX + nonAnimatedPositions[i].x, offsetY + nonAnimatedPositions[i].y, pos.z);
        }
    }

    void RotateParts()
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = target.transform.position - partTransforms[partTransforms.Length - 1].transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = rotateSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(partTransforms[partTransforms.Length - 1].transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(partTransforms[partTransforms.Length - 1].transform.position, newDirection * 10, Color.red);
        Debug.DrawRay(partTransforms[partTransforms.Length - 1].transform.position, new Vector3(-newDirection.x,newDirection.y,newDirection.z) * 10, Color.yellow);

        Vector3 good = new Vector3(-newDirection.x, newDirection.y, newDirection.z);


        // Calculate a rotation a step closer to the target and applies rotation to this object
        partTransforms[partTransforms.Length - 1].transform.rotation = Quaternion.LookRotation(newDirection);
    }

    // Update is called once per frame
    void Update()
    {
        if(ready)
            AnimateParts();
        //RotateParts();
    }
}
