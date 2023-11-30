using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Doozy.Engine.UI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PDollarGestureRecognizer;

public class Demo_OP : MonoBehaviour {

	public Transform gestureOnScreenPrefab;

	private List<Gesture> trainingSet = new List<Gesture>();

	private List<Point> points = new List<Point>();
	private int strokeId = -1;

	private Vector3 virtualKeyPosition = Vector2.zero;

	Rect drawArea;

	private RuntimePlatform platform;//运行平台
	private int vertexCount = 0;

	private List<LineRenderer> gestureLinesRenderer = new List<LineRenderer>();
	private LineRenderer currentGestureLineRenderer;

	//GUI
	private bool recognized=false;
	private string newGestureName = "";
	//private string message;

	[Header("识别所需的UI")]
	public Image drawImage;//画画的区域
	public Text recongnized_result;
	public Button recongnize;//识别按钮；
	public Button add;//增加用户手势识别记录
	public InputField GestureName;//获取手势识别记录的
	[Header("子弹时间检测器")]
	public GameObject BT_Detector_Box;

	void Start () {
		drawArea=new Rect(drawImage.transform.GetComponent<RectTransform>().rect.x,drawImage.transform.GetComponent<RectTransform>().rect.y,drawImage.transform.GetComponent<RectTransform>().rect.width,drawImage.transform.GetComponent<RectTransform>().rect.height);
		platform = Application.platform;
		//drawArea = new Rect(0, 0, Screen.width - Screen.width / 3, Screen.height);//绘画的区间

		//Load pre-made gestures
		// TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/10-stylus-MEDIUM/");//载入预先手势文件
		// foreach (TextAsset gestureXml in gesturesXml)//对于所有识别文件中储存的收视数据
		// 	trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));

		//Load user custom gestures
		string[] filePaths = Directory.GetFiles(Application.persistentDataPath, "*.xml");//载入用户手势文件
		foreach (string filePath in filePaths)
			trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));//载入所有的手势文件xml
	}

	void Update () {

		if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer) {
			if (Input.touchCount > 0) {//当平台为手机平台时
				virtualKeyPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
			}
		} else {
			if (Input.GetMouseButton(0)) {//当平台为PC平台
				virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
			}
		}

		if (RectTransformUtility.RectangleContainsScreenPoint(drawImage.transform.GetComponent<RectTransform>(), Input.mousePosition)){//当点击到图片中的时候
			if (Input.GetMouseButtonDown(0)) {
				if (recognized) {//如果已经recongnized，则需要把笔迹全部清除
					recognized = false;
					strokeId = -1;
					points.Clear();
					foreach (LineRenderer lineRenderer in gestureLinesRenderer) {
						lineRenderer.SetVertexCount(0);//渲染的边数变成0
						Destroy(lineRenderer.gameObject);
					}
					gestureLinesRenderer.Clear();//清空渲染
				}

				++strokeId;//每画一个新的手势就要赋予一个新的ID
				
				Transform tmpGesture = Instantiate(gestureOnScreenPrefab, transform.position, transform.rotation) as Transform;
				currentGestureLineRenderer = tmpGesture.GetComponent<LineRenderer>();
				gestureLinesRenderer.Add(currentGestureLineRenderer);
				vertexCount = 0;
			}
			if (Input.GetMouseButton(0)) {
				//对顶点数目进行更改
				//Debug.Log(virtualKeyPosition.x.ToString()+'\n'+ (-virtualKeyPosition.y).ToString());
				points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, strokeId));
				//相同的笔画有者相同的strokeID
				currentGestureLineRenderer.SetVertexCount(++vertexCount);
				currentGestureLineRenderer.SetPosition(vertexCount - 1, Camera.main.ScreenToWorldPoint(new Vector3(virtualKeyPosition.x, virtualKeyPosition.y, 10)));
			}
		}
	}
	public void Onclick_Recongnized(){//点击识别时候的反应
			recognized = true;

			Gesture candidate = new Gesture(points.ToArray());
			Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());
			
			recongnized_result.text = gestureResult.GestureClass + " " + gestureResult.Score;
			if(gestureResult.GestureClass.ToString()==BT_Detector_Box.GetComponent<BT_Detector>().box_name)
				BT_Detector_Box.GetComponent<BT_Detector>().Detect_Successfully();
			Debug.Log(gestureResult.GestureClass);
	}
	public void Onclick_Add(){
		if(GestureName.text!=null){//当输入框不为空时
			newGestureName=GestureName.text;//获取输入框中的输入
			string fileName = String.Format("{0}/{1}-{2}.xml", Application.persistentDataPath, newGestureName, DateTime.Now.ToFileTime());
			#if !UNITY_WEBPLAYER
				GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
			#endif
			trainingSet.Add(new Gesture(points.ToArray(), newGestureName));

			newGestureName = "";
		}
		else
		{
			recongnized_result.text="新手势的名称不能为空";
		}
	}
	//void OnGUI() {

		// GUI.Box(drawArea, "Draw Area");

		// GUI.Label(new Rect(10, Screen.height - 40, 500, 50), message);

		// if (GUI.Button(new Rect(Screen.width - 100, 10, 100, 30), "Recognize")) {

		// 	recognized = true;

		// 	Gesture candidate = new Gesture(points.ToArray());
		// 	Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());
			
		// 	message = gestureResult.GestureClass + " " + gestureResult.Score;
		// }

		// GUI.Label(new Rect(Screen.width - 200, 150, 70, 30), "Add as: ");
		// newGestureName = GUI.TextField(new Rect(Screen.width - 150, 150, 100, 30), newGestureName);

		// if (GUI.Button(new Rect(Screen.width - 50, 150, 50, 30), "Add") && points.Count > 0 && newGestureName != "") {

		// 	string fileName = String.Format("{0}/{1}-{2}.xml", Application.persistentDataPath, newGestureName, DateTime.Now.ToFileTime());

		// 	#if !UNITY_WEBPLAYER
		// 		GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
		// 	#endif

		// 	trainingSet.Add(new Gesture(points.ToArray(), newGestureName));

		// 	newGestureName = "";
		//}
	//}
}
