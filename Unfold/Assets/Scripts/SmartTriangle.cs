using UnityEngine;

namespace Unfold
{
	public class Vector3RefWrapper
	{
		public Vector3 Vector3 { get; private set; }

		public Vector3RefWrapper(Vector3 vector3)
		{
			Vector3 = vector3;
		}
	}

    public class SmartTriangle
    {
        private TrianglesStorage _storage;
        private MeshTriangle _meshTriangle;

        private TriangleData _currentTriangle;
        private TriangleData _targetTriangle;
        private const float DELTA = 0.1f;
        private const float SPEED = 10.0f;
        private float _directionValue;
        private SmartTriangle[] _children;

        private bool _hasChildren;
        private bool _isFirst = true;
        public bool IsSet { get; private set; }

        public SmartTriangle(TriangleData triangle, TrianglesStorage storage, float minimumArea)
        {
            IsSet = false;
            _currentTriangle = _targetTriangle = triangle;
            _storage = storage;

            _hasChildren = _targetTriangle.GetArea() > minimumArea;
            if (_hasChildren)
            {
                _children = SubDivider.SubDivideTriangle(triangle, _storage, minimumArea);
            }
        }

        public void Show()
        {
            if (_hasChildren)
            {
                foreach (var smartTriangle in _children)
                {
                    smartTriangle.Show();
                }
            }
            else
            {
                ShowSelf();
            }
        }

        private void ShowSelf()
        {
            if (_meshTriangle == null)
            {
                _meshTriangle = _storage.GetTriangle();
                _meshTriangle.UseTriangle(_currentTriangle);
            }
            _meshTriangle.UpdateTriangle(_currentTriangle);
        }

        public bool UpdateMeshData(float unfoldValue)
        {
            if (IsSet)
            {
                return true;
            }
            if (_hasChildren)
            {
                UpdateChildren(unfoldValue);
            }
            else
            {
	            if (_isFirst)
	            {
		            SetFirst();
					_isFirst = false;
	            }
	            else
	            {
					if (unfoldValue < _directionValue)
		            {
			            return false;
		            }
		            UpdateSelf();
				}
            }

            return IsSet;
        }

	    private void SetFirst()
	    {
		    _currentTriangle.SetVector((_targetTriangle + new Vector3(0, 10, 0)).GetCentroid());
	    }

        public void SetAnimator(MeshAnimator meshAnimator)
        {
            if (_hasChildren)
            {
                foreach (var child in _children)
                {
                    child.SetAnimator(meshAnimator);
                }
            }
            else
            {
                _directionValue = meshAnimator.GetAnimationValue(_targetTriangle.GetCentroid());
            }
        }

        private void Clear()
        {
            if (_children != null && _children.Length != 0)
            {
                ClearChildren();
            }

            _meshTriangle.ClearTriangle();
            _storage.ReturnTriangle(_meshTriangle);
            _storage = null;
            _meshTriangle = null;
        }

        private void UpdateSelf()
        {
            if (_meshTriangle == null)
            {
                _meshTriangle = _storage.GetTriangle();
                _meshTriangle.UseTriangle(_currentTriangle);
            }

            _currentTriangle.Lerp(_targetTriangle, Time.deltaTime * SPEED);
            IsSet = _currentTriangle.TheSame(_targetTriangle, DELTA);
            if (IsSet)
            {
                _currentTriangle = _targetTriangle;
            }
            
            _meshTriangle.UpdateTriangle(_currentTriangle);
        }
        
        private void UpdateChildren(float unfoldValue)
        {
            IsSet = true;

            for (int i = 0; i < _children.Length; i++)
            {
                IsSet = _children[i].UpdateMeshData(unfoldValue) && IsSet;
            }

            if (IsSet)
            {
				Combine();
			}
        }

	    private void Combine()
	    {
		    ClearChildren();

		    _meshTriangle = _storage.GetTriangle();
		    _meshTriangle.UseTriangle(_currentTriangle);
		    _meshTriangle.UpdateTriangle(_currentTriangle);

		    IsSet = true;
	    }

		private void ClearChildren()
        {
            for (int i = 0; i < _children.Length; i++)
            {
                _children[i].Clear();
            }
            _children = null;
            _hasChildren = false;
        }
    }
}
