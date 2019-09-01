using System.IO;
using UnityEngine;
using UnityEngine.UI;
     
public class ShowFps : MonoBehaviour {
    private Text _fpsText;
    private double _deltaTime;
    private StreamWriter _write;
    private int _frameNumber;

    private void Start() {
        _fpsText = gameObject.GetComponent<Text>();

        
        
#if UNITY_ANDROID
        _write = File.CreateText(Application.persistentDataPath + "/fpstest.txt");
#else
        _write = File.CreateText("fpstest.txt");
#endif 
        
        //_write = File.CreateText("fpstest.txt");
        _write.WriteLine("Frame, FPS");
    }

    private void Update () {
        var fps = 1.0f / Time.deltaTime;

        if (fps > 35) {
            fps -= 30;
        }

        var str = fps.ToString("0.00").Replace(',', '.');

        //_fpsText.text = str;

        _write.WriteLine(_frameNumber + "," + str);
        
        _frameNumber++;   
    }
}