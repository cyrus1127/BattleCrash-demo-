using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CamMoveByMouseDrag : MonoBehaviour {

	public delegate void TouchToLeft();
	public static event TouchToLeft touchToLeft;
	public delegate void TouchToRight();
	public static event TouchToRight touchToRight;
	public delegate void TouchToFront();
	public static event TouchToFront touchToFront;
	public delegate void TouchToBack();
	public static event TouchToBack touchToBack;

	public List<string> tagsIgnore;
	public enum TouchStartMode { centered_screen , centered_touch_point , centered_touch_point_fixed_lenght};
	public TouchStartMode _touchStartMode = TouchStartMode.centered_touch_point;
	public enum ControlMode { _4Direct , _8Direct};
	public ControlMode _controlMode = ControlMode._4Direct;
	public float range = 10;
	public float mix_range = 20;
	public enum Direct {none, up, rightup, right, rightdown, down, leftup, left, leftdown };
	Direct _direct = Direct.none;

	private bool onTouched = false;
	private Vector3 ray_start = Vector3.zero;
	private Vector3 ray_move = Vector3.zero;
	private float n_ray_diff = 0f;
	private float n_ray_diff_y = 0f;


	[SerializeField] GameObject m_target_Camera;
	[SerializeField] bool m_MirrorDirection = false;

	bool isOver_UIOver = false;

	PointerEventData eventDataCurrentPosition;
	
	// Use this for initialization
	void Start () {
		//check the Module name
		BaseInputModule bim = EventSystem.current.currentInputModule;
		if( bim != null ){
			Debug.Log ( "Current Input Module name > " + bim.name );
		}else{
			Debug.Log ( "check no Input Module found " );
		}

		eventDataCurrentPosition = new PointerEventData(EventSystem.current);
	}
	
	// Update is called once per frame
	void Update () 
	{
		DoIgnore ();

		if( onTouched ){
			_direct = Get_Direct();
//			Debug.Log ("current mouse/touch Direction ? " + _direct);
		}

	}

	void OnMouseDown(){
		if (Input.GetMouseButton (0)) {
			ray_start = Input.mousePosition;
			onTouched = true;
		}
					Debug.Log ( "OnMouseDown > mousePosition :  " +  ray_start);
	}

	//MouseUp function is providing .... 
	void OnMouseUp(){

		//case check
		switch(_direct){
		case Direct.none:
		case Direct.up:
			this.toFront();
			break;
		case Direct.down:
			this.toBack();
			break;
		case Direct.left:
			this.toLeft();
			break;
		case Direct.right:
			this.toRight();
			break;
		}

		onTouched = false;
				Debug.Log ( "OnMouseUp" );
	}

	/// <summary>
	/// Adject_touch_point() with TouchStartMode 
	/// </summary>
	void Adject_touch_point(){
		//TODO
	}

	Vector2 Get_delta( Vector2 s_p , Vector2 e_p ){

		Vector2 r_p = s_p;

		r_p.x -= e_p.x;
		r_p.y = (r_p.y - e_p.y) * -1;

		return r_p;

	}

	public CamMoveByMouseDrag.Direct Get_Cur_Direct(){
		return _direct;
	}
	Direct Get_Direct(){

		Vector2 d_p = Get_delta(ray_start , Input.mousePosition);
		Direct n_d = Direct.none;
		Direct n_d_y = Direct.none; //For 8 direction

		if (d_p.x < -range || d_p.x > range) {

			// move L / R
			if( d_p.x < -range ){
				n_d = Direct.right;
			}else{
				n_d = Direct.left;
			}

		}
		if( d_p.y < -range || d_p.y > range ){
			//move U(forwoad) / D
			if( d_p.y < -range ){
				n_d_y = Direct.down;
			}else{
				n_d_y = Direct.up;
			}
		}

		switch(_controlMode){
		case ControlMode._4Direct:
			if( n_d == Direct.none )
				n_d = n_d_y;
			break;
		case ControlMode._8Direct:

			if( n_d != Direct.none && n_d_y != Direct.none ){
				//case 8 Direct
				if( n_d == Direct.right && n_d_y == Direct.up)
					n_d = Direct.rightup;
				if( n_d == Direct.right && n_d_y == Direct.down)
					n_d = Direct.rightdown;
				if( n_d == Direct.left && n_d_y == Direct.up)
					n_d = Direct.leftup;
				if( n_d == Direct.left && n_d_y == Direct.down)
					n_d = Direct.leftdown;
			}else{
				//case 4 Direct
				n_d = ( n_d_y != Direct.none ? n_d_y : n_d);
			}

			break;
		}



		return n_d;
	}

	public float getTouchMovement(){
		return n_ray_diff;
	}

	/// <summary>
	/// toLeft() , toRight() , toBack() , toFront() with unity delegate event
	/// </summary>
	public void toLeft(){ //Debug.Log ( "toLeft" ); 
		if (touchToLeft != null && !isOver_UIOver)
			touchToLeft();
	}
	public void toRight(){ //Debug.Log ( "toRight" );  
		if (touchToRight != null && !isOver_UIOver)
			touchToRight();
	}
	public void toBack(){ //Debug.Log ( "toBack" );   
		if (touchToBack != null && !isOver_UIOver)
			touchToBack();
	}
	public void toFront(){ //Debug.Log ( "toFront" );   
		if (touchToFront != null && !isOver_UIOver)
			touchToFront();
	}

	/// <summary>
	/// Do the ignore with Gameobject tap
	/// This function is a solution to solve the signal blocking of the UI touch event module handling.
	/// </summary>
	private void DoIgnore(){

		if( tagsIgnore.Count > 0 ){
			eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
			
					Debug.Log ( "Result count : " + results.Count );
			
			isOver_UIOver = false;
			
			foreach (RaycastResult rr in results) {
				string check_tag = rr.gameObject.tag;
				string check_name = rr.gameObject.name;
							Debug.Log ( "oncheck tag : " + check_tag );
				
				for( int i = 0; i < tagsIgnore.Count ; i++ ){
					if(check_tag == tagsIgnore[i]){
						break;
					}else{
						isOver_UIOver = true;
										Debug.Log( check_tag + " not inside the ignore list!!" );
					}
				}
				
							Debug.Log ( "oncheck  : [" + check_name + "] tag ["+ check_tag + "] > UIOver ? " + isOver_UIOver);
				
			}
		}

	}

}
