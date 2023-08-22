using System.Collections.Generic;
using UnityEngine;

public class GizmosExample : MonoBehaviour
{
     public Color _myColor; // Set the color of your gizmo. Alpha is transparency
     
     private void OnDrawGizmos() // you can use OnDrawGizmosSelected to only show gizmos when selecting the object.
     {
         Gizmos.color = _myColor;
         Gizmos.matrix = transform.localToWorldMatrix;
         Gizmos.DrawCube(Vector3.zero, Vector3.one);
     }
}
