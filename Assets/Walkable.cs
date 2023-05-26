using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System;
using Valve.VR.InteractionSystem;
using UnityEngine.XR;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class Walkable : MonoBehaviour
            {
                EyeData eye;
                public Transform bodyCollider;
                public SteamVR_Action_Vector2 walkAction;
                float walkSpeed = 4;
                public float rollspeed;
                public float gradient;
                float LeftBlink;
                float counttime = 0;
                bool flag=false;

                Vector3 CombineGazeRayorigin;
                public Vector3 CombineGazeRaydirection;
                //左目の視線格納変数
                Vector3 LeftGazeRayorigin;
                Vector3 LeftGazeRaydirection;
                //右目の視線格納変数
                Vector3 RightGazeRayorigin;
                Vector3 RightGazeRaydirection;
                //④焦点情報--------------------
                //両目の焦点格納変数
                //レイの始点と方向（多分③の内容と同じ）
                Ray CombineRay;
                private Quaternion HMDRotationQ;
                public Vector3 HMDRotation;

                public static FocusInfo CombineFocus;
                //レイの半径
                float CombineFocusradius;
                //レイの最大の長さ
                float CombineFocusmaxDistance;
                //オブジェクトを選択的に無視するために使用されるレイヤー ID
                int CombinefocusableLayer = 0;
                //------------------------------

                //右のまぶたの開き具合格納用関数
                float RightBlink;
                float cou = 0;
                float cou2 = 0;
                float cou3 = 0;
                float cou4 = 0;
                float hmdpos0=0;
                public float LocTec = 0;
                public float RolTec = 0;
                private SteamVR_Action_Boolean Iui = SteamVR_Actions.default_InteractUI;
                public SteamVR_Action_Boolean GrabG;
                public SteamVR_Action_Vector2 stick;
                public SteamVR_Action_Vector2 stickR;




                bool count = false;
                int swi = 0;
                private Vector3 ang_cam0 = new Vector3(0, 0, 0);
                private Vector3 HMDRotation0 = new Vector3(0, 0, 0);

                void Start()
                {
                    // ヘッドセットのある位置に合わせてPlayerの初期位置を移動
                    Vector3 pos = transform.position;
                    Vector3 player_ang = transform.eulerAngles;

                    pos.x -= Player.instance.hmdTransform.localPosition.x;
                    pos.z -= Player.instance.hmdTransform.localPosition.z;
                    pos.y = transform.position.y;


                    transform.position = pos;

                    // bodyCollider初期位置
                    Vector3 colpos = bodyCollider.transform.position;
                    colpos.x = Player.instance.hmdTransform.position.x;
                    colpos.z = Player.instance.hmdTransform.position.z;


                    bodyCollider.transform.position = colpos;

                    transform.eulerAngles = player_ang;

                }

                void LateUpdate()
                {


                }


                void Update()
                {

                    Vector3 player_pos = transform.position;
                    Vector3 body_pos = bodyCollider.transform.position;
                    Vector3 player_ang = transform.eulerAngles;
                    Camera mainCamera = Camera.main;
                    SRanipal_Eye_API.GetEyeData(ref eye);

                    Quaternion pla_ang = transform.rotation;
                    Vector3 ang_pla = transform.eulerAngles;
                    ang_pla.y = (Mathf.Repeat(ang_pla.y + 180, 360) - 180)  ;


                    HMDRotationQ = InputTracking.GetLocalRotation(XRNode.Head);


                    HMDRotation = HMDRotationQ.eulerAngles;

                    HMDRotation.y = Mathf.Repeat(HMDRotation.y + 180, 360) - 180;
                    float dif = (HMDRotation.y - HMDRotation0.y);
                    SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out CombineGazeRayorigin, out CombineGazeRaydirection, eye);
                    SRanipal_Eye.GetEyeOpenness(EyeIndex.RIGHT, out RightBlink, eye);
                    SRanipal_Eye.GetEyeOpenness(EyeIndex.LEFT, out LeftBlink, eye);

                    CombineGazeRaydirection.z = 0;                
                    /* if (Math.Abs(ang_cam1.y) > Math.Abs(ang_cam0.y))
                     {
                    ang_pla.y = ang_pla.y + dif * Math.Abs(dif);
                    ang_pla.y = ang_pla.y;// + dif;
                     }
                    //else { ang_pla.y = ang_pla.y + 1 / 2 * dif; }
                    */

                    // float Display_v= (float)0.3 / 8100 * Mathf.Pow((Mathf.Abs(HMDRotation.y) - 20), (float)1.5) * HMDRotation.y / Mathf.Abs(HMDRotation.y);
                    /*float wh= (float)0.01*Wheelc();
                    if (rollspeed > 0)
                    {
                        rollspeed = rollspeed + wh;
                    }
                    else
                    {
                        rollspeed = (float)0.003;
                    }*/
                    if (RolTec == 2)
                    {
                        ang_pla.y = ang_pla.y + dif;
                    }
                    if (RolTec==3)
                    {
                        ang_pla.y = ang_pla.y + Rollsp(rollspeed);
                    }
                    if (RolTec == 4)
                    {
                        if (CombineGazeRaydirection.magnitude > 0.5) { }
                    }
                   /* if (HMDRotation.y > 50  || HMDRotation.y < -50 )
                    {
                        ang_pla.y = ang_pla.y +rollspeed * HMDRotation.y/Mathf.Abs(HMDRotation.y);

                       // ang_pla.y = ang_pla.y + (float)0.3/8100* Mathf.Pow((Mathf.Abs(HMDRotation.y)-20),(float)1.5)*HMDRotation.y/Mathf.Abs(HMDRotation.y);

                    }
                    else if ((Mathf.Abs(HMDRotation.y)>10&& Mathf.Abs(HMDRotation.y) < 50))
                    {
                       // ang_pla.y = ang_pla.y +rollspeed*
                          ang_pla.y = ang_pla.y + (float)0.025*rollspeed*(Mathf.Abs(HMDRotation.y)-10)*HMDRotation.y / Mathf.Abs(HMDRotation.y);
                    };*/

                    HMDRotation0 = HMDRotation;

                    //Debug.Log(ang_pla + " , " + HMDRotation.y + 20 * Mathf.Deg2Rad);
                    Wheelc();

                    


                    body_pos.x = Player.instance.hmdTransform.position.x;
                    body_pos.z = Player.instance.hmdTransform.position.z;
                    bodyCollider.transform.position = body_pos;

                    SRanipal_Eye_API.GetEyeData(ref eye);

                    // Actionでの移動
                    Vector3 direction = Player.instance.hmdTransform.TransformDirection(new Vector3(0, 0, 1));
                    Vector3 conto= new Vector3(stick.axis.x, 0, stick.axis.y);
                    Vector3 contoR = new Vector3(stickR.axis.x, 0, 0);

                   
                    Vector3 directionGame= Player.instance.hmdTransform.TransformDirection(conto);
                    Debug.Log(stick.axis.x+"  " +stick.axis.y);
                    if (contoR.magnitude > 0.15)
                    {
                        ang_pla.y = ang_pla.y + stickR.axis.x * (float)0.15;
                    }
                    if (conto.magnitude > 0.15)
                    {
                        player_pos.x += walkSpeed * Time.deltaTime * directionGame.x;
                        player_pos.z += walkSpeed * Time.deltaTime * directionGame.z;
                    }
                    transform.position = player_pos;


                    if (LocTec == 1)
                    {
                        if ((GrabG.GetState(SteamVR_Input_Sources.RightHand)))
                        {
                            cou2 += 1;

                            if (cou2 > 10)
                            {
                                player_pos.x += walkSpeed * Time.deltaTime * direction.x;
                                player_pos.z += walkSpeed * Time.deltaTime * direction.z;
                                transform.position = player_pos;
                            }
                        }
                        else
                        {
                            cou2 = 0;
                        }
                    }


                    float hmdpos = Player.instance.hmdTransform.transform.localPosition.z;
                    if (LocTec==2)
                    {
                        if (hmdpos > 0.1 || cou3 == 1)
                        {

                            player_pos.x += walkSpeed * Time.deltaTime * direction.x;
                            player_pos.z += walkSpeed * Time.deltaTime * direction.z;
                            transform.position = player_pos;
                            cou3 = 1;

                        }
                        if (hmdpos < -0.1)
                        {
                            cou3 = 0;
                        }
                    }


                    if (LocTec == 3)
                    {
                        if ((RightBlink<0.6&&LeftBlink<0.6)||cou2>30)
                        {
                            cou2 += 1;

                            if (cou2 > 30)
                            {
                                player_pos.x += walkSpeed * Time.deltaTime * direction.x;
                                player_pos.z += walkSpeed * Time.deltaTime * direction.z;
                                transform.position = player_pos;
                            }
                        }
                        if (RightBlink ==0 || LeftBlink== 0)
                            if (cou2 > 30)
                            {
                                cou4++;
                                if(cou4>30)
                                {
                                    cou2 = 0;
                                }
                            }
                    }
                    

                   // Debug.Log(hmdpos);
                    
                    // transform.localEulerAngles 








                    // 高さ移動
                    //  player_pos.y = bodyCollider.transform.position.y;
                  //  transform.position = player_pos;
                    transform.eulerAngles = ang_pla;



                    




                }

                void Wheelc()
                {
                    /*if (Input.GetMouseButtonDown(0))
                    {*/
                   
                   /* if (GrabG.GetState(SteamVR_Input_Sources.RightHand))
                    {
                        if (flag == false) { Debug.Log("start"); }
                        flag =true;
                        
                    }
                    if (flag==true)
                    {
                        counttime += Time.deltaTime;

                    }
                    if (counttime > 5)
                    {
                        flag = false;
                        counttime = 0;
                        Debug.Log("End");
                    }*/
                    
                }
                float Rollsp(float sp)
                {
                    float speed=0;
                    if (HMDRotation.y > 50 || HMDRotation.y < -50)
                    {
                         speed= sp*HMDRotation.y / Mathf.Abs(HMDRotation.y);  

                       ;

                    }
                    else if ((Mathf.Abs(HMDRotation.y) > 10 && Mathf.Abs(HMDRotation.y) < 50))
                    {
                        // ang_pla.y = ang_pla.y +rollspeed*
                        speed = (float)0.025 * sp * (Mathf.Abs(HMDRotation.y) - 10)* HMDRotation.y / Mathf.Abs(HMDRotation.y);
                    };
                    return speed;
                }
                

            }
        }

    }
}
