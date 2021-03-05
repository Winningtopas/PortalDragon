using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float speed = 10f;

    public GameObject portal;

    [SerializeField]
    GameObject[] portals = new GameObject[2];

    public float lifeTime = 2f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        lifeTime -= Time.deltaTime;



        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        Debug.DrawRay(transform.position, forward, Color.green);

        if (lifeTime <= 0.0f)
        {
            OnDeath();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        rb.velocity = forward * speed * Time.deltaTime;
    }

    void OnDeath()
    {
        for (int i = 0; i < portals.Length; i++)
        {
            Instantiate(portal, new Vector3(transform.position.x + 10 * i, transform.position.y, transform.position.z), Quaternion.identity);
        }
        Destroy(this.gameObject);
    }
}