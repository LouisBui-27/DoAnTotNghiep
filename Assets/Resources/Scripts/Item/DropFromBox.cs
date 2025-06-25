using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropFromBox : MonoBehaviour
{
    private BoxItem sourceBox;

    public void SetSourceBox(BoxItem box)
    {
        sourceBox = box;
    }

    public void NotifyBox()
    {
        sourceBox?.NotifyCollected();
    }
}
