using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour // by Brackeys
{
    [SerializeField] private Transform[] backgrounds;
    [SerializeField] private bool isInfiniteHorizontal;
    [SerializeField] private bool isInfiniteVertical;
    public float smoothing = 1f;

    private float[] _parallaxScales;
    private float[] _startPosX;
    private Vector2[] _backgroundSpriteLength;
    private Vector3 _previousCamPos; // The position of camera in the previous frame
    
    [Tooltip("Reference to the main camera transform")]
    [SerializeField] protected new Transform camera; // Reference to the main camera transform
    // Start is called before the first frame update
    private void Start()
    {
        _previousCamPos = camera.position;
        _parallaxScales = new float[backgrounds.Length];
        _startPosX = new float[backgrounds.Length];
        _backgroundSpriteLength = new Vector2[backgrounds.Length];
        for (int i = 0; i < backgrounds.Length; i++)
        {
            _parallaxScales[i] = backgrounds[i].position.z * -1;
            _startPosX[i] = backgrounds[i].position.x;
            _backgroundSpriteLength[i].x = backgrounds[i].GetComponent<SpriteRenderer>().bounds.size.x;
            _backgroundSpriteLength[i].y = backgrounds[i].GetComponent<SpriteRenderer>().bounds.size.y;
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float parallax = (_previousCamPos.x - camera.position.x) * _parallaxScales[i];
            float parallaxY = (_previousCamPos.y - camera.position.y) * _parallaxScales[i];
            float targetPosX = backgrounds[i].position.x + parallax;
            float targetPosY = backgrounds[i].position.y + parallaxY;
            Vector3 targetPos = new Vector3(targetPosX, targetPosY, backgrounds[i].position.z);
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, targetPos, smoothing);

            if (isInfiniteHorizontal)
            {
                if (Mathf.Abs(camera.transform.position.x - backgrounds[i].position.x) >= _backgroundSpriteLength[i].x)
                {
                    float offsetPosX = (camera.transform.position.x - backgrounds[i].position.x) %
                                       _backgroundSpriteLength[i].x;
                    backgrounds[i].position =
                        new Vector3(camera.transform.position.x + offsetPosX, backgrounds[i].position.y,
                            backgrounds[i].position.z);
                }
            }

            if (isInfiniteVertical)
            {
                if (Mathf.Abs(camera.transform.position.y - backgrounds[i].position.y) >= _backgroundSpriteLength[i].y)
                {
                    float offsetPosY = (camera.transform.position.y - backgrounds[i].position.y) %
                                       _backgroundSpriteLength[i].y;
                    backgrounds[i].position = new Vector3(backgrounds[i].position.x,
                        camera.transform.position.y + offsetPosY,
                        backgrounds[i].position.z);
                }
            }
        }

        _previousCamPos = camera.position;
    }
}
