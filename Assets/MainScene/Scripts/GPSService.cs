
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class GPSService : MonoBehaviour
{
    public Text textMsg;

    

    IEnumerator Start()
    {
        if(!Input.location.isEnabledByUser)
            yield break;

        Input.location.Start();

        int maxWait = 20;
        while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            textMsg.text = "Timed out";
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            textMsg.text = "Unable to determine device location";
            yield break;
        }
        else
        {
            while(true)
            {
                textMsg.text = "À§Ä¡: "
                       + Input.location.lastData.latitude + " "
                       + Input.location.lastData.longitude + " "
                       + Input.location.lastData.horizontalAccuracy;
                yield return new WaitForSeconds(1);
            }
        }

    }
}
