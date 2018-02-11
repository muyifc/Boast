using UnityEngine;
using System.Collections;

public class CardLayoutData : MonoBehaviour {
    [Tooltip("x=StartX,y=StartY,z=SpaceX,w=SpaceY")]
    public Vector4 spacing;

    [Tooltip("Child localScale")]
    public float scale = 1f;

    public int col = -1;

    public Vector3 GetNextPos(){
        int count = transform.childCount-1;
        int curCol = col;
        if(col == -1) curCol = count+1;
        return transform.TransformPoint(new Vector3(spacing.x + spacing.z * (count % curCol),spacing.y + spacing.w * (int)(count / curCol),0));
    }
}