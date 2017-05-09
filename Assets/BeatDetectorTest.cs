using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAudio.Wave;
using System;

public class BeatDetectorTest
{
    // private static readonly BeatDetectorTest _instance = new BeatDetectorTest();
    public int length;
    public float[] realData;
    public float[] processedData;
    public float[] energy1024;
    public float[] energy44100;
    public float[] energy_peak;
    public float[] _beat;
    const float K_ENERGY_RATIO = 1.3f;
    const int K_TRAIN_DIMP_SIZE = 108;

    public float[] conv;
    int tempo = 0;

    public void VarDefine()
    {
        energy1024 = new float[length / 1024];
        energy44100 = new float[length / 1024];
        energy_peak = new float[length / 1024 + 21];
        _beat = new float[length / 1024+21];
        conv = new float[length / 1024];
        for (int i = 0; i < length / 1024 + 21; i++)
        {
            energy_peak[i] = 0;
        }
    }

    public void WavReader(WaveFileReader wfr)/////////////////////////////////
    {
        Debug.Log("Read Start");
        length = (int)wfr.SampleCount;/////////////////////////////////////
       
        switch (length % 4)
        {
            case 1:
                length = length - 1;
                break;
            case 2:
                length = length - 2;
                break;
            case 3:
                length = length - 3;
                break;
        }
        realData = new float[length];
        byte[] buffer = new byte[length];
        int read = 0;
        //float readf = 0;
        read = wfr.Read(buffer, 0, length);
        //readf = read;
        for (int i = 0; i < read; i += 2)/////////////////////////
        {
            realData[i] = (BitConverter.ToInt16(buffer, i));
        }

        //for (int i = 0; i < read / 4; i++)
        //{
        //    if (BitConverter.IsLittleEndian)
        //    {
        //        Array.Reverse(buffer, i * 4, 4);
        //    }
        //    realData[i] = BitConverter.ToSingle(buffer, 4 * i);
        //}
        //KeepOn = wfr.TryReadFloat(out readf);
        //}
    }



    public int Energy(float[] data, int offset, int window)
    {
        float energy = 0;
        for (int i = offset; (i < offset + window) && (i < length); i++)
        {
            energy = energy + data[i] * data[i] / window;
        }
      //  Debug.Log(energy);
        return (int)energy;
    }

    //public float[] Normalize(float[] data, int size) // delete var max_val
    //{
    //    float max = 0;
    //    float min = 0;
    //    for (int i = 0; i < size; i++)
    //    {
    //        if (data[i] > max)
    //        {
    //            max = data[i];
    //        }
    //        if (data[i] < min)
    //        {
    //            min = data[i];
    //        }
    //    }
    //    //float ratio = max_val / max;
    //    for (int i = 0; i < size; i++)
    //    {
    //        data[i] = (data[i] - min) / (max - min);
    //    }
    //    return data;
    //}


    void Normalize(ref float[] signal, int size, float max_val)
    {
        // recherche de la valeur max du signal
        float max = 0f;
        for (int i = 0; i < size; i++)
        {
            if (Math.Abs(signal[i]) > max) max = Math.Abs(signal[i]);
        }
        // ajustage du signal
        float ratio = max_val / max;
        for (int i = 0; i < size; i++)
        {
            signal[i] = signal[i] * ratio;
        }
    }

    int Search_max(ref float[] signal, int pos, int fenetre_half_size, int length)
    {
        float max = 0f;
        int max_pos = pos;
        for (int i = pos - fenetre_half_size; i <= pos + fenetre_half_size; i++)
        {
            if (signal[i] > max)
            {
                max = signal[i];
                max_pos = i;
            }
        }
        return max_pos;
    }

    public void AudioProcess()
    {
        for (int i = 0; i < length / 1024; i++)
        {
            energy1024[i] = Energy(realData, 1024 * i, 4096);
           // Debug.Log(energy1024[i]);
          //  Debug.Log(length);
        }

        energy44100[0] = 0;
        float sum = 0;
        for (int i = 0; i < 43; i++)
        {
            sum = sum + energy1024[i];
        }
        energy44100[0] = sum / 43;

        for (int i = 1; i < length / 1024; i++)
        {
            if ((i + 42) < length / 1024)
            {
                sum = sum - energy1024[i - 1] + energy1024[i + 42];
                energy44100[i] = sum / 43;
            }
        }

        for (int i = 21; i < length / 1024; i++)
        {
            if (energy1024[i] > K_ENERGY_RATIO * energy44100[i - 21])
            {
                energy_peak[i] = 1;
            }
        }

        // Calculate BPM
        List<int> T = new List<int>();
        int i_prec = 0;
        for (int i = 1; i < length / 1024; i++)
        {
            if ((energy_peak[i] == 1) && (energy_peak[i - 1] == 0))
            {
                int di = i - i_prec;
                if (di > 5)
                {
                    T.Add(di);
                    i_prec = i;
                }
            }
        }

        int T_occ_max = 0;
        float T_occ_moy = 0f;


        int[] occurences_T = new int[86];
        for (int i = 0; i < 86; i++) occurences_T[i] = 0;
        for (int i = 1; i < T.Count; i++)
        {
            if (T[i] <= 86) occurences_T[T[i]]++;
        }
        int occ_max = 0;
        for (int i = 1; i < 86; i++)
        {
            if (occurences_T[i] > occ_max)
            {
                T_occ_max = i;
                occ_max = occurences_T[i];
            }
        }
        // on fait la moyenne du max + son max de voisin pour + de précision
        int voisin = T_occ_max - 1;
        if (occurences_T[T_occ_max + 1] > occurences_T[voisin]) voisin = T_occ_max + 1;
        float div = occurences_T[T_occ_max] + occurences_T[voisin];

        if (div == 0) T_occ_moy = 0;
        else T_occ_moy = (float)(T_occ_max * occurences_T[T_occ_max] + (voisin) * occurences_T[voisin]) / div;

        // clacul du tempo en BPMs
        tempo = (int)(60f / (T_occ_moy * (1024f / 44100f)));



        float[] train_dimp = new float[K_TRAIN_DIMP_SIZE];
        float espace = 0f;
        train_dimp[0] = 1f;
        for (int i = 1; i < K_TRAIN_DIMP_SIZE; i++)
        {
            if (espace >= T_occ_moy)
            {
                train_dimp[i] = 1;
                espace = espace - T_occ_moy; // on garde le depassement
            }
            else train_dimp[i] = 0;
            espace += 1f;
        }

        // convolution avec l'énergir instantannée de la music
        for (int i = 0; i < length / 1024 - K_TRAIN_DIMP_SIZE; i++)
        {
            for (int j = 0; j < K_TRAIN_DIMP_SIZE; j++)
            {
                conv[i] = conv[i] + energy1024[i + j] * train_dimp[j];
            }

        }
        Normalize(ref conv, length / 1024, 1f);

        for (int i = 1; i < length / 1024; i++)
            _beat[i] = 0;

        float max_conv = 0f;
        int max_conv_pos = 0;
        for (int i = 1; i < length / 1024; i++)
        {
            if (conv[i] > max_conv)
            {
                max_conv = conv[i];
                max_conv_pos = i;
            }
        }
        _beat[max_conv_pos] = 1f;

        // les suivants
        // vers la droite
        int ii = max_conv_pos + T_occ_max;
        while ((ii < length / 1024 - 21) && (conv[ii] > 0f))
        {
            // on cherche un max dans les parages
            int conv_max_pos_loc = Search_max(ref conv, ii, 2, length);
            _beat[conv_max_pos_loc] = 1f;
            ii = conv_max_pos_loc + T_occ_max;
        }
        //// vers la gauche
        ii = max_conv_pos - T_occ_max;
        while (ii > 0)
        {
            // on cherche un max dans les parages
            int conv_max_pos_loc = Search_max(ref conv, ii, 2, length);
            _beat[conv_max_pos_loc] = 1f;
            ii = conv_max_pos_loc - T_occ_max;
        }
    }
}

