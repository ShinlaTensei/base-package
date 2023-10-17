using System.Collections;
using System.Collections.Generic;
using Base;
using UnityEngine;

namespace Base
{
    public class GridBase
    {
        private int _width, _height;
        private List<int> _gridList;
        private float _cellSize;

        public int Width => _width;
        public int Height => _height;
        public float CellSize => _cellSize;

        public List<int> GridList => _gridList;
        public GridBase(int width, int height)
        {
            _width = width;
            _height = height;
        
            _gridList = new List<int>(width * height);
        }

        public GridBase(int width, int height, float cellSize)
        {
            _height = height;
            _width = width;
            _cellSize = cellSize;

            _gridList = new List<int>(width * height);
        }

        public void Add(int value)
        {
            _gridList.Add(value);
        }

        public void Add(int index, int value)
        {
            _gridList[index] = value;
        }
        
        public void Add(int x, int y, int value)
        {
            int index = _width * y + x;
            _gridList[index] = value;
        }

        public void SetValue(int index, int value)
        {
            _gridList[index] = value;
        }

        public void Remove(int value)
        {
            if (_gridList.Contains(value))
            {
                _gridList.Remove(value);
            }
            else if (value < _gridList.Count)
            {
                _gridList.RemoveAt(value);
            }
        }

        public int GetValue(int x, int y)
        {
            x = Mathf.Clamp(x, 0, _width - 1);
            y = Mathf.Clamp(y, 0, _height - 1);
            return _gridList[_width * y + x];
        }

        public int GetIndex(int value)
        {
            return _gridList.IndexOf(value);
        }

        public Vector2 Get(int index)
        {
            index = Mathf.Clamp(index, 0, _gridList.Capacity - 1);

            int y = index / _width;
            int x = index - (_width * y);

            return new Vector2(x, y);
        }

        private Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y) * _cellSize;
        }

        public Vector3 GetWorldPosition(int x, int y, RectTransform.Axis axis)
        {
            if (axis is RectTransform.Axis.Horizontal)
            {
                return new Vector3(x, 0, y) * _cellSize;
            }
            
            if (axis is RectTransform.Axis.Vertical)
            {
                return GetWorldPosition(x, y);
            }

            return Vector3.zero;
        }

        public void DrawGizmos(Transform root)
        {
            if (Width > 0 && Height > 0 && CellSize > 0)
            {
                Gizmos.color = Color.black;
                for (int y = 0; y < Height; ++y)
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        Vector3 position = GetWorldPosition(x, y, RectTransform.Axis.Horizontal);
                        Vector3 final = root.TransformPoint(position);
                        Gizmos.DrawSphere(final, .1f);
                    }
                }
            }
        }
    }
}

