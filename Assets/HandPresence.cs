using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandPresence : MonoBehaviour
{

    [SerializeField]
    Canvas menu;

    [SerializeField]
    GameObject player;

    public 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            Sprite aSprite;
            /*GameObject tempPlayer = new GameObject();
            tempPlayer.transform.position = player.transform.position;
            Quaternion tempRotation = player.transform.rotation;
            tempRotation.x = 0;
            tempRotation.z = 0;
            tempPlayer.transform.rotation = tempRotation;
            Destroy(tempPlayer);
            Vector3 spawnPos = player.transform.position + (tempPlayer.transform.forward * 6f);
                menu.transform.rotation = player.transform.rotation;
            menu.transform.position = spawnPos;*/

            menu.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.1f, this.transform.position.z);
            menu.transform.rotation = this.transform.rotation * Quaternion.Euler(0, -90, 0);

            Image image = menu.GetComponent<Image>();
            aSprite = Resources.Load<Sprite>("CardsMenu/pc1");        // was "Images/A.png"
            image.sprite = aSprite;
            Debug.Log("Pressed");
        }
        else if (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch) && menu.transform.position != new Vector3(0, 0, 0)) {
            menu.transform.position = new Vector3(0, 0, 0);
        }
    }
}
