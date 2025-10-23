using UnityEngine;

public class FlashingMaterial : MonoBehaviour
{
    public Color colorA = Color.blue;
    public Color colorB = Color.yellow;
    
    private Material _material;

    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
    }
    
    private void Update() {
        float combining = Mathf.PingPong(Time.time, 1f);
        _material.color = Color.Lerp(colorA, colorB, combining);
    }

}
