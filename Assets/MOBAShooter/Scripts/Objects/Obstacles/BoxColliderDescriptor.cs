using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obstacle
{
    public class BoxColliderDescriptor : MonoBehaviour
    {
        private BoxCollider _collider;
        private Vector3 _extents;
        private Vector3 _center;
        private Vector3 _extentX;
        private Vector3 _extentY;
        private Vector3 _extentZ;
        private Vector3 _rotatedExtentX;
        private Vector3 _rotatedExtentY;
        private Vector3 _rotatedExtentZ;
        
        void Start()
        {
            xzBoundaries = new List<Vector2>();
            xzBoundaries.Add(Vector2.zero);  //UpperLeft
            xzBoundaries.Add(Vector2.zero);  //UpperRight
            xzBoundaries.Add(Vector2.zero);  //LowerLeft
            xzBoundaries.Add(Vector2.zero);  //LowerRight
            _collider = GetComponent<BoxCollider>();
            Quaternion currentRot = transform.rotation;
            transform.rotation = Quaternion.identity;
            _extents = _collider.bounds.extents;
            transform.rotation = currentRot;
            _extentX = Vector3.right * _extents.x;
            _extentY = Vector3.up * _extents.y;
            _extentZ = Vector3.forward * _extents.z;
            _center = _collider.bounds.center;
            RotateExtents();
            CalculateXZBoundary();
        }

        void RotateExtents()
        {
            _rotatedExtentX = transform.rotation * _extentX;
            _rotatedExtentY = transform.rotation * _extentY;
            _rotatedExtentZ = transform.rotation * _extentZ;
        }

        void CalculateXZBoundary()
        {
            //See this from above (Y axis perspective)
            Vector3 upperLeft = -_rotatedExtentX + _rotatedExtentZ;
            Vector3 upperRight = _rotatedExtentX + _rotatedExtentZ;
            Vector3 lowerLeft = -_rotatedExtentX + -_rotatedExtentZ;
            Vector3 lowerRight = _rotatedExtentX + -_rotatedExtentZ;

            xzBoundaries[0] = (new Vector2(_center.x + upperLeft.x, _center.z + upperLeft.z));      //UpperLeft
            xzBoundaries[1] = (new Vector2(_center.x + upperRight.x, _center.z + upperRight.z));    //UpperRight
            xzBoundaries[2] = (new Vector2(_center.x + lowerLeft.x, _center.z + lowerLeft.z));      //LowerLeft
            xzBoundaries[3] = (new Vector2(_center.x + lowerRight.x, _center.z + lowerRight.z));    //LowerRight
        }

        public List<Vector2> xzBoundaries { get; private set; }
    }
}
