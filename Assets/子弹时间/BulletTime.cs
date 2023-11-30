using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;

public class BulletTime : MonoBehaviour
{
    [Header("预制体数组以及其大小")]
    [SerializeField]
    GameObject[] cube_team;
    public int team_size;
    [Header("预制体生成锚点")]
    public Transform init_cube_pivots;
    [HideInInspector]
    public bool situation=false;//是否进入了bullettime
    float time=0;//计时，每5秒生成一个预制体
    [SerializeField]
    float target_Timescale=1;
    [Header("BulletTime出现速度")]
    public float slow_Speed;
    // Update is called once per frame
    void Update()
    {
        int random_cube=Random.Range(0,team_size);
        //注意，此处代码需要改进，当timescale变成之前地十分之一时候，生成速率也要变成之前的十分之一
        if(time<1f){
            time+=Time.deltaTime;
        }
        else{
            Instantiate(cube_team[random_cube],init_cube_pivots.position,Quaternion.Euler(0,-539.536f,0));
            time=0;
        }
        int i=Random.Range(0,team_size);
        //TimeScale减小，deltatime就会减小，FixedUpdate也会减小，但Update执行的速度没有变化，DoozyUI的执行也不会有变化
        if(Input.GetKey(KeyCode.A)){
            Start_Bullettime();
        }
        if(Input.GetKey(KeyCode.Q)){
            End_Bullettime();
        }
        if(Time.timeScale!=target_Timescale)
        {
            if(Time.deltaTime<=0)
            {
                Time.timeScale=Mathf.Lerp(Time.timeScale,target_Timescale,0.0001f);
            }
            else{
                Time.timeScale=Mathf.Lerp(Time.timeScale,target_Timescale,slow_Speed*Time.deltaTime);
            }
            if(Mathf.Abs(Time.timeScale-target_Timescale)<=0.05f){
                Time.timeScale=target_Timescale;
            }
        }
    }
    public void Start_Bullettime()
    {
        target_Timescale=0;
    }
    public void End_Bullettime(){
        target_Timescale=1;
    }
}
