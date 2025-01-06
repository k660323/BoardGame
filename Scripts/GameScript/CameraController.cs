using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed;

    private Camera mainCamera;

    private float distanceX,distanceZ;

    private float dist;
    private Vector3 MouseStart;

    public GameObject[] Window;

    bool isStopMove;
    int activeCount;

    void Awake()
    {
        mainCamera = GetComponent<Camera>();
        isStopMove = false;
        zoomSpeed = 30f;
        activeCount = 0;
        dist = transform.position.z;  // Distance camera is above map
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0; i < Window.Length; i++)
        {
            if (Window[i].activeInHierarchy == true)
            {
                isStopMove = true;
                activeCount = 0;
                break;
            }
            else
            {
                activeCount++;
            }

            if (activeCount == Window.Length)
            {
                isStopMove = false;
                activeCount = 0;
            }
        }


        if (!isStopMove)
        { 
            Zoom();
            Move();
        }
    }
             
    void Zoom()
    {
        float distance = Input.GetAxis("Mouse ScrollWheel") * -1 * zoomSpeed; // 휠 위로 0.1 가만히 0 휠 아래 -0.1
        if (distance != 0 && 9 <= mainCamera.fieldOfView + distance && mainCamera.fieldOfView + distance <= 82)
            mainCamera.fieldOfView += distance;

        if (Input.GetMouseButtonDown(2)) // 마우스 휠 클릭
        {
            mainCamera.fieldOfView = 60;
        }       
    }

    void Move()
    {
        if (Input.GetMouseButtonDown(0)) // 한번 눌러짐 체크
        {
            MouseStart = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -dist); // 마우스 찍은 위치 카메라 z축 빼줘야 제대로 나옴
            MouseStart = Camera.main.ScreenToWorldPoint(MouseStart);//카메라 좌표로 변환
            MouseStart.y = transform.position.y; // 카메라와 동일한 y 좌표          
            //결론 카메라 y동일 카메라화면 바로앞에서의 x,z의 거리
        }
        else if (Input.GetMouseButton(0)) // 눌러짐 지속
        {
            var MouseMove = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -dist);
            MouseMove = Camera.main.ScreenToWorldPoint(MouseMove);
            MouseMove.y = transform.position.y;

            distanceX = (transform.position - ((MouseMove - MouseStart))).x;
            distanceZ = (transform.position - ((MouseMove - MouseStart))).z;
            if (distanceX > -20 && distanceX < 25 && distanceZ > -20 && distanceZ < 25)
                transform.position = transform.position - ((MouseMove - MouseStart));
        }
    }
}
