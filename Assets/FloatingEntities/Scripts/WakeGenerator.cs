using UnityEngine;
using System.Collections;

public class WakeGenerator : MonoBehaviour
{
  public Vector3 offset;

  private OceanAdvanced ocean;
  private Vector3 last_position;
  private float speed;

  void Awake()
  {
    ocean = FindObjectOfType<OceanAdvanced>();
    speed = 0.0F;
    last_position = transform.localPosition;
  }

    void Update()
    {
      
        speed = (transform.localPosition- last_position).magnitude / Time.deltaTime;
        last_position = transform.localPosition;
        if (Time.time % 0.2F < 0.01F)
        {
            Vector3 p = transform.localPosition + transform.localRotation * offset;
            if (OceanAdvanced.GetWaterHeight(p) > p.y)
            {
                ocean.RegisterInteraction(p, Mathf.Clamp01(speed / 15.0F) * 0.5F);
            }
                
        }
    }

  void OnDrawGizmos()
  {
    Gizmos.color = Color.cyan;
    Gizmos.DrawWireSphere(transform.localPosition + transform.localRotation * offset, 0.5F);
  }
}
