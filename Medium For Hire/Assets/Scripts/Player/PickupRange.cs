using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(LineRenderer))]

public class PickupRange : MonoBehaviour
{
    [Header("Visual Configuration")]
    public int segments = 28;              // Smoothness of the circle
    public float lineWidth = 0.03f;        // Line thickness
    public Texture dashTexture;            // Your broken-line texture
    //public bool updateEveryFrame = true;


    [Header("References")]
    private PlayerStats playerStats;
    private PlayerEvents Events;

    private CircleCollider2D circleCollider;
    private LineRenderer lineRenderer;


    private void Initialize()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();

        //lineRenderer.useWorldSpace = false;
        //lineRenderer.loop = false;

        playerStats = PlayerStats.Instance;
        Events = PlayerController.Instance.Events;

        // take stats
        DoScaleStats();

        // hook event
        Events.OnAfterGetUpgrade += OnAfterGetUpgrade;
    }

    private void DoScaleStats()
    {
        circleCollider.radius = playerStats.GetPlayerStat(Stat.BasePickupRange);
        circleCollider.radius *= (playerStats.GetPlayerStat(Stat.PickupRangePercent) / 100f);
    }
    private void OnAfterGetUpgrade()
    {
        DoScaleStats();
    }

    void Start()
    {
        // initialize everything
        Initialize();

        // ======
        SetupLineRenderer();
        DrawCircle();
    }

    void Update()
    {
        DrawCircle();
    }

    // LINE RENDERER


    void SetupLineRenderer()
    {
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        if (dashTexture != null)
        {
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.material.mainTexture = dashTexture;
            lineRenderer.textureMode = LineTextureMode.Tile; // Repeat the texture along the line
        }
    }

    void DrawCircle()
    {
        float radius = circleCollider.radius;
        lineRenderer.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * 2f * Mathf.PI;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }


    // LINE RENDERER
}