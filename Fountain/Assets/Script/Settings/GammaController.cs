using UnityEngine;

public class GammaController : MonoBehaviour
{
    public Material gammaMaterial; // 在Inspector中拖入刚才创建的材质
    [Range(0.5f, 2.2f)]
    public float gammaValue = 1.0f; // 默认1.0代表无变化

    private void Start()
    {
        // 确保摄像机有这个组件
        if (GetComponent<Camera>().enabled)
        {
            // 更新材质的参数
            UpdateGamma();
        }
    }

    private void UpdateGamma()
    {
        if (gammaMaterial != null)
        {
            gammaMaterial.SetFloat("_Gamma", gammaValue);
        }
    }

    // 这个方法会被Unity自动调用，用于渲染后期效果
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (gammaMaterial != null)
        {
            // 使用材质处理图像
            Graphics.Blit(source, destination, gammaMaterial);
        }
        else
        {
            // 如果没有材质，直接拷贝
            Graphics.Blit(source, destination);
        }
    }

    // 可以把这个方法绑定到Slider的OnValueChanged事件上
    public void SetGamma(float value)
    {
        gammaValue = value;
        UpdateGamma();
    }
}
