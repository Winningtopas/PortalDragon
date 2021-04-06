using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

    public List<GameObject> portals = new List<GameObject>();
    public List<GameObject> cameras = new List<GameObject>();

    private GameObject GameMaster;

    //void Awake () {
    //    portals = FindObjectsOfType<Portal>();
    //}

    private void Start()
    {
        GameMaster = GameObject.Find("GameMaster");
    }

    private void Update()
    {
        //portals = GameMaster.GetComponent<PortalSetUp>().portals;
        //cameras = GameMaster.GetComponent<PortalSetUp>().cameras;
    }

    //void OnPreCull () {
    //    //for (int i = 0; i < portals.Length; i++) {
    //    //    portals[i].PrePortalRender ();
    //    //}
    //    //for (int i = 0; i < portals.Length; i++) {
    //    //    portals[i].Render ();
    //    //}

    //    //for (int i = 0; i < portals.Count; i++) {
    //        //portals[i].GetComponent<Portal>().PostPortalRender();
    //        //cameras[i].GetComponent<PortalCamera>().PrePortalRenderer();
    //    //}

    //}

}