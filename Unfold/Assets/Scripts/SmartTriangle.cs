using UnityEngine;

namespace Unfold
{
    public class SmartTriangle
    {
        private TrianglesStorage _pool;
        private MeshTriangle _meshTriangle;

        private TriangleData _currentTriangle;
        private TriangleData _targetTriangle;
        
        private float _canMoveValue;
        private SmartTriangle[] _children;

        private bool _hasChildren;
	    private bool _isComplete;

	    private const float DELTA = 0.1f;
	    private const float SPEED = 10.0f;//TODO to setup parameters

		public SmartTriangle(TriangleData triangle, TrianglesStorage pool, float minimumArea)
        {
            _currentTriangle = _targetTriangle = triangle;
            _pool = pool;

            _hasChildren = _targetTriangle.GetArea() > minimumArea;
            if (_hasChildren)
            {
                _children = SubDivider.SubDivideTriangle(triangle, _pool, minimumArea);
            }
        }

	    public void PlaceToStartPosition(Vector3 offset)
	    {
			_currentTriangle.SetVector((_targetTriangle + offset).GetCentroid());
	    }

	    public void Setup(MeshAnimator meshAnimator)
	    {
		    if (_hasChildren)
		    {
			    foreach (var child in _children)
			    {
				    child.Setup(meshAnimator);
			    }
		    }
		    else
		    {
			    _canMoveValue = meshAnimator.GetAnimationValue(_targetTriangle.GetCentroid());
		    }
		}

		#region Temp
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
                _meshTriangle = _pool.GetTriangle();
                _meshTriangle.UseTriangle(_currentTriangle);
            }
            _meshTriangle.UpdateTriangle(_currentTriangle);
        }
		#endregion

		public bool UpdateMeshData(float unfoldValue)
        {
            if (_isComplete)
            {
                return true;
            }
            if (_hasChildren)
            {
                UpdateChildren(unfoldValue);
            }
            else
            {
				if (unfoldValue < _canMoveValue)
	            {
		            return false;
	            }
	            UpdateSelf();
			}

            return _isComplete;
        }

        private void Clear()
        {
            if (_children != null && _children.Length != 0)
            {
                ClearChildren();
            }

            _meshTriangle.ClearTriangle();
            _pool.ReturnTriangle(_meshTriangle);
            _pool = null;
            _meshTriangle = null;
        }

        private void UpdateSelf()
        {
            if (_meshTriangle == null)
            {
                _meshTriangle = _pool.GetTriangle();
                _meshTriangle.UseTriangle(_currentTriangle);
            }

            _currentTriangle.Lerp(_targetTriangle, Time.deltaTime * SPEED);
	        _isComplete = _currentTriangle.TheSame(_targetTriangle, DELTA);
            if (_isComplete)
            {
                _currentTriangle = _targetTriangle;
            }
            
            _meshTriangle.UpdateTriangle(_currentTriangle);
        }
        
        private void UpdateChildren(float unfoldValue)
        {
	        _isComplete = true;

            for (int i = 0; i < _children.Length; i++)
            {
	            _isComplete = _children[i].UpdateMeshData(unfoldValue) && _isComplete;
            }

            if (_isComplete)
            {
				Combine();
			}
        }

	    private void Combine()
	    {
		    ClearChildren();

		    _meshTriangle = _pool.GetTriangle();
		    _meshTriangle.UseTriangle(_currentTriangle);
		    _meshTriangle.UpdateTriangle(_currentTriangle);

		    _isComplete = true;
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
