using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_Detector : MonoBehaviour
{
    public GameObject BT_Controller;
    GameObject inner_box;//在碰撞体中的箱子
    public string box_name;
    private void OnTriggerEnter(Collider other)
    {
        inner_box=other.gameObject;
        if(other.gameObject.tag=="A"||other.gameObject.tag=="E"||other.gameObject.tag=="N"||other.gameObject.tag=="Y"||other.gameObject.tag=="Z"){
            BT_Controller.GetComponent<BulletTime>().Start_Bullettime();
            box_name=other.gameObject.tag;
        }
            
    }
    public void Detect_Successfully(){
        Destroy(inner_box);
        BT_Controller.GetComponent<BulletTime>().End_Bullettime();
    }
}
