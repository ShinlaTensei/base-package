#region Header
// Date: 10/06/2023
// Created by: Huynh Phong Tran
// File name: BezierCurve.cs
#endregion

using System;
using System.Collections.Generic;
using Base.Logging;
using UnityEngine;

namespace Base.Helper
{
    public interface ICurve
    {
        Vector3 Velocity(float t);
        Vector3 Interpolate(float t);
        void DrawGizmos(float t);
    }

    public class QuadCurve : ICurve
    {
        private Vector3 m_startPoint;
        private Vector3 m_endPoint;
        private Vector3 m_controlPoint;

        public QuadCurve()
        {
            m_startPoint = m_endPoint = m_controlPoint = Vector3.zero;
        }

        public QuadCurve(Vector3 startPoint, Vector3 endPoint, Vector3 controlPoint)
        {
            m_startPoint = startPoint;
            m_endPoint = endPoint;
            m_controlPoint = controlPoint;
        }
        
        public Vector3 Velocity(float t)
        {
            return (2f * m_startPoint - 4f * m_controlPoint + 2f * m_endPoint) * t + 2f * m_controlPoint - 2f * m_startPoint;
        }

        public Vector3 Interpolate(float t)
        {
            float d = 1f - t;
            return d * d * m_startPoint + 2f * d * t * m_controlPoint + t * t * m_endPoint;
        }
        
        public void DrawGizmos(float t) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(m_startPoint, m_controlPoint);
            Gizmos.DrawLine(m_controlPoint, m_endPoint);
         
            Gizmos.color = Color.white;
            Vector3 prevPt = m_startPoint;
         
            for (int i = 1; i <= 20; i++) {
                float pm = (float) i / 20f;
                Vector3 currPt = Interpolate(pm);
                Gizmos.DrawLine(currPt, prevPt);
                prevPt = currPt;
            }
         
            Gizmos.color = Color.blue;
            Vector3 pos = Interpolate(t);
            Gizmos.DrawLine(pos, pos + Velocity(t));
        }
    }

    public class CubicBez : ICurve
    {
        public Vector3 st, en, ctrl1, ctrl2;

        public CubicBez(Vector3 st, Vector3 en, Vector3 ctrl1, Vector3 ctrl2)
        {
            this.st = st;
            this.en = en;
            this.ctrl1 = ctrl1;
            this.ctrl2 = ctrl2;
        }


        public Vector3 Interpolate(float t)
        {
            float d = 1f - t;
            return d * d * d * st + 3f * d * d * t * ctrl1 + 3f * d * t * t * ctrl2 + t * t * t * en;
        }


        public Vector3 Velocity(float t)
        {
            return (-3f * st + 9f * ctrl1 - 9f * ctrl2 + 3f * en) * t * t
                + (6f * st - 12f * ctrl1 + 6f * ctrl2) * t
                - 3f * st + 3f * ctrl1;
        }
        
        public void DrawGizmos(float t) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(st, ctrl1);
            Gizmos.DrawLine(ctrl2, en);
         
            Gizmos.color = Color.white;
            Vector3 prevPt = st;
         
            for (int i = 1; i <= 20; i++) {
                float pm = (float) i / 20f;
                Vector3 currPt = Interpolate(pm);
                Gizmos.DrawLine(currPt, prevPt);
                prevPt = currPt;
            }
         
            Gizmos.color = Color.blue;
            Vector3 pos = Interpolate(t);
            Gizmos.DrawLine(pos, pos + Velocity(t));
        }
    }
    
    public class CRSpline : ICurve
    {
        public Vector3[] pts;
     
        public CRSpline(params Vector3[] pts) {
            this.pts = new Vector3[pts.Length];
            System.Array.Copy(pts, this.pts, pts.Length);
        }
     
     
        public Vector3 Interpolate(float t) {
            int numSections = pts.Length - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);
            float u = t * (float) numSections - (float) currPt;
                 
            Vector3 a = pts[currPt];
            Vector3 b = pts[currPt + 1];
            Vector3 c = pts[currPt + 2];
            Vector3 d = pts[currPt + 3];
         
            return .5f * (
                (-a + 3f * b - 3f * c + d) * (u * u * u)
                + (2f * a - 5f * b + 4f * c - d) * (u * u)
                + (-a + c) * u
                + 2f * b
            );
        }
     
     
        public Vector3 Velocity(float t) {
            int numSections = pts.Length - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);
            float u = t * (float) numSections - (float) currPt;
                 
            Vector3 a = pts[currPt];
            Vector3 b = pts[currPt + 1];
            Vector3 c = pts[currPt + 2];
            Vector3 d = pts[currPt + 3];
 
            return 1.5f * (-a + 3f * b - 3f * c + d) * (u * u)
                + (2f * a -5f * b + 4f * c - d) * u
                + .5f * c - .5f * a;
        }
     
     
        public void DrawGizmos(float t) {
            Gizmos.color = Color.white;
            Vector3 prevPt = Interpolate(0);
         
            for (int i = 1; i <= 20; i++) {
                float pm = (float) i / 20f;
                Vector3 currPt = Interpolate(pm);
                Gizmos.DrawLine(currPt, prevPt);
                prevPt = currPt;
            }
         
            Gizmos.color = Color.blue;
            Vector3 pos = Interpolate(t);
            Gizmos.DrawLine(pos, pos + Velocity(t));
        }
    }

    public class BezierCurve : SingletonNonMono<BezierCurve>
    {
        public enum CurveType
        {
            QuadCurve, CubicCurve, CRSpline
        }
        private IDictionary<CurveType, ICurve> m_curves = new Dictionary<CurveType, ICurve>();

        public IDictionary<CurveType, ICurve> Curves => m_curves;

        public static void AddCurve(CurveType curveType, params Vector3[] points)
        {
            if (!Instance.Curves.TryGetValue(curveType, out ICurve curve))
            {
                switch (curveType)
                {
                    case CurveType.QuadCurve:
                        QuadCurve quadCurve = new QuadCurve(points[0], points[1], points[2]);
                        Instance.Curves.TryAdd(curveType, quadCurve);
                        break;
                    case CurveType.CubicCurve:
                        CubicBez cubicBez = new CubicBez(points[0], points[1], points[2], points[3]);
                        Instance.Curves.TryAdd(curveType, cubicBez);
                        break;
                    case CurveType.CRSpline:
                        CRSpline crSpline = new CRSpline(points);
                        Instance.Curves.TryAdd(curveType, crSpline);
                        break;
                }
            }
            else
            {
                switch (curveType)
                {
                    case CurveType.QuadCurve:
                        curve = new QuadCurve(points[0], points[1], points[2]);
                        break;
                    case CurveType.CubicCurve:
                        curve = new CubicBez(points[0], points[1], points[2], points[3]);
                        break;
                    case CurveType.CRSpline:
                        curve = new CRSpline(points);
                        break;
                }
                
            }
        }

        public static Vector3 Velocity(CurveType curveType, float t)
        {
            if (!Instance.Curves.TryGetValue(curveType, out ICurve curve))
            {
                PDebug.ErrorFormat("[BezierCurve] Not found curve of type {0}", curveType.ToString());
                return Vector3.zero;
            }

            return curve.Velocity(t);
        }
        
        public static Vector3 Interpolate(CurveType curveType, float t)
        {
            if (!Instance.Curves.TryGetValue(curveType, out ICurve curve))
            {
                PDebug.ErrorFormat("[BezierCurve] Not found curve of type {0}", curveType.ToString());
                return Vector3.zero;
            }

            return curve.Interpolate(t);
        }
    }
}