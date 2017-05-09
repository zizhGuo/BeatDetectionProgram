using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAudio.Wave;
using System;

public class WavManager : MonoBehaviour
{

    WaveFileReader wfr = new WaveFileReader(@"D:\BeaetDetection\Assets\QHC.wav");
    // Mp3FileReader wfr = new Mp3FileReader(@"C:\Users\William\Desktop\Unity\Practice\Audio D1\Assets\QHC.mp3");/// <summary>
    /// //////////////////////////
    /// </summary>
    // WaveStream wavStream = WaveFormatConversionStream.CreatePcmStream(rrr);
    // WaveFileWriter.
    BeatDetectorTest detector = new BeatDetectorTest();

    float[] _data;
    int length;
    float[] beatTime;
    int peakTime = 0;
    // float duration;

    public float jumpSpeed = 500f;
    bool grounded = true;
    float time = 0f;

    public int timeDuration = 232;
    public float delayTime = 1.7f;
    public int interval = 10;
    public int amplitude = 5000;
    //Color color;


    // Use this for initialization
    void Start()
    {
        // color = GetComponent<Renderer>().material.color;

        detector.WavReader(wfr); // Read audio file
        length = detector.length;
        //detector.Normalize(detector.realData, length);
        //for (int i = 0; i < 10000; i++)
        //{
        //    Debug.Log(detector.Normalize(detector.realData, 10000)[i + 40000]);
        //}
        //for (int i = 0; i < 10000; i++)
        //{
        //    Debug.Log(detector.realData[i + 40000]);
        //}

        detector.VarDefine();  //Define the variables
        detector.AudioProcess(); // Audio Process and Beats Detecting
        //for (int i = 0; i < length / 1024; i++)
        //{
        //    Debug.Log(detector.energy_peak[i]);
        //}

        //Debug.Log(length);
        //_data = new float[length]; // input realData to _data
        //for (int i = 0; i < length; i++)
        //{
        //    _data[i] = detector.realData[i];
        //}
        //  duration = wfr.TotalTime;
        //beatTime = new float[length/1024];
        //for (int i = 0; i < length / 1024; i++)
        //{
        //    if (detector.energy_peak[i] == 1)
        //    {
        //        beatTime[peakTime] = i * 232 / (length / 1024);
        //        peakTime++;
        //    }
        //}
        StartCoroutine(ColorChange());
        //for (int i = 0; i < 232; i++)
        //{
        //    Debug.Log(beatTime[i]);
        //}
        // GetComponent<Renderer>().material.color = Color.black;




    }

    void Jump()
    {
        if (grounded == true)
        {

            // GetComponent<Rigidbody>().AddForce(Vector3.up * jumpSpeed);
            grounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        grounded = true;

    }

    private void Update()
    {
        for (int i = 1; i < length / 1024 - 108; i++)
        {
            //Debug.DrawLine(new Vector3((i - 1) * interval, _data[i * 1024], 0), new Vector3(i * interval, _data[(i + 1) * 1024], 0), Color.blue);
            //Debug.DrawLine(new Vector3((i - 1) * interval, detector.energy_peak[i] * amplitude, 0), new Vector3(i * interval, detector.energy_peak[i + 1] * amplitude, 0), Color.red);
            Debug.DrawLine(new Vector3((i - 1) * interval, detector.conv[i - 1] * amplitude, 0), new Vector3(i * interval, detector.conv[i] * amplitude, 0), Color.blue);
            Debug.DrawLine(new Vector3((i - 1) * interval, detector._beat[i] * amplitude, 0), new Vector3(i * interval, detector._beat[i + 1] * amplitude, 0), Color.black);
        }



        //time = time + Time.deltaTime;
        //for (float i = 0; i < 232; i += Time.deltaTime)
        //{
        //    for (int j = 0; j < 232; j++)
        //    {
        //        if (i == beatTime[j])
        //        {
        //            Jump();
        //            GetComponent<Renderer>().material.color = Color.black;
        //        }
        //        else
        //        {
        //            GetComponent<Renderer>().material.color = Color.red;
        //        }
        //    }
        //}
    }

    private IEnumerator ColorChange()
    {
        //for (float i = 0; i < delayTime; i += Time.deltaTime)
        //{
        //    yield return 0;
        //}
        for (float i = 0; i < timeDuration; i += Time.deltaTime)
        {
            if (detector._beat[(int)((i / timeDuration) * (length / 1024))] == 1)
            {
                GetComponent<Renderer>().material.color = Color.black;
                //color = Color.black;

            }
            else
            {
                GetComponent<Renderer>().material.color = Color.red;
                //color = Color.red;
            }
            yield return 0;
        }
        // yield return 0;
    }
}
