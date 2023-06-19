using Interaction;
using PlotTwist.BenFox.Mechanics.Dialogues;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinedInteractionShapes : MonoBehaviour
{
    private List<Transform> triggers = new List<Transform>();
    private GameObject shapes;
    private SimpleInteraction[] simpleInteraction;
    private float time = 0.0f;
    private bool Check()
    {
       gameObject.transform.parent.GetComponentInChildren<SimpleInteraction>(includeInactive:true);
        foreach(var trigger in triggers)
        {
            if (trigger.gameObject.activeSelf)
                return true;

            else
                continue;
        }
        return false;
    }
    private void Start()
    {
        simpleInteraction = this.transform.parent.GetComponentsInChildren<SimpleInteraction>();
        foreach (var interaction in simpleInteraction)
        {
            interaction.OnObjectDisable += OnObjectDisabled;
            interaction.OnObjectEnable += OnObjectEnabled;
        }
        

        shapes = this.GetComponentInChildren<ShapesAnimator>().gameObject;
        foreach (Transform child in this.transform.parent)
        {
            if (child.GetComponent<SimpleInteraction>() != null)
            {
                triggers.Add(child);
            }
        }

        if (Check())
            shapes.SetActive(true);
        else
            shapes.SetActive(false);
    }

    //private void Update()
    //{
    //    time += Time.deltaTime;
    //    if (time < 5f)
    //    {
    //        if (Check())
    //            shapes.SetActive(true);
    //        else
    //            shapes.SetActive(false);
    //    }
    //    else
    //    {
    //        time = 0.0f;
    //    }
        
    //}
    private void OnObjectEnabled()
    {
        if (Check())
            shapes.SetActive(true);
        else
            shapes.SetActive(false);
    }
    private void OnObjectDisabled()
    {
        if (Check())
            shapes.SetActive(true);
        else
            shapes.SetActive(false);
    }
    private void OnDestroy()
    {
        foreach (var interaction in simpleInteraction)
        {
            interaction.OnObjectDisable -= OnObjectDisabled;
            interaction.OnObjectEnable -= OnObjectEnabled;
        }
    }
}
