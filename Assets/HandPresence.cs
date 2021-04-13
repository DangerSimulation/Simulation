using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HandPresence : MonoBehaviour
{

    [SerializeField]
    Canvas menu;

    [SerializeField]
    GameObject player;

    private Object[] textures;
    private int index;

    bool down = false;
    bool up = false;

    // Start is called before the first frame update
    void Start()
    {
        textures = Resources.LoadAll("CardsMenu", typeof(Texture2D));
        index = 0;
    }

    Sprite getImage()
    {
        Sprite aSprite = Resources.Load<Sprite>("CardsMenu/" + textures[index].name);
        return aSprite;
    }

    void nextIndex()
    {
        if (index == textures.Length - 1)
        {
            index = 0;
        }
        else
        {
            index++;
        }
        
    }

    void previousIndex()
    {
        if (index == 0)
        {
            index = textures.Length - 1;
        }
        else
        {
            index--;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            
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
            menu.transform.rotation = this.transform.rotation * Quaternion.Euler(0, 90, 90);




            if (OVRInput.Get(OVRInput.Button.Down, OVRInput.Controller.RTouch) && !down)
            {
                previousIndex();
                down = true;
            }
            else if(!OVRInput.Get(OVRInput.Button.Down, OVRInput.Controller.RTouch) && down)
            {
                down = false;
            }

            if (OVRInput.Get(OVRInput.Button.Up, OVRInput.Controller.RTouch) && !up)
            {
                nextIndex(); 
                up = true;
            }
            else if (!OVRInput.Get(OVRInput.Button.Up, OVRInput.Controller.RTouch) && up)
            {
                up = false;
            }

            Image image = menu.GetComponent<Image>();
            image.overrideSprite = getImage();


        }
        else if (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch) && menu.transform.position != new Vector3(0, 0, 0))
        {
            menu.transform.position = new Vector3(0, 0, 0);
        }
    }
}
