using System;

[Serializable]
public class Properties
{
    
    public bool continueWithPreScne, playOnClick, playAfterLaunch;

    public static SceneLineItemPropertyObjects staticObj = ControlFlowManagerV2.Instance.getSceneLineItemPropertyObjects;

    //public void SetValueChangeDelegateSubscriber(Action methodPlayOnClick, Action methodAfterLaunch,Action methodCountinue)
    //{
    //    RemoveAllListenerOnToggle();
    //    staticObj.continueWithPreScneToggle.onValueChanged.AddListener(delegate { methodCountinue(); });
    //    staticObj.playAfterLaunchToggle.onValueChanged.AddListener(delegate { methodAfterLaunch(); });
    //    staticObj.playOnClickToggle.onValueChanged.AddListener(delegate { methodPlayOnClick(); });

    //}

    public void SetValueChangeDelegateSubscriber()
    {
       // RemoveAllListenerOnToggle();
        //update on the usage (properly)
        staticObj.continueWithPreScneToggle.onValueChanged.AddListener(delegate { MethodContinue(); });
        staticObj.playAfterLaunchToggle.onValueChanged.AddListener(delegate { MethodAfterLaunch(); });
        staticObj.playOnClickToggle.onValueChanged.AddListener(delegate { MethodPlayOnClick(); });

    }

    private void MethodPlayOnClick()
    {
        this.playOnClick = staticObj.playOnClickToggle.isOn;

    }

    private void MethodAfterLaunch()
    {
        this.playAfterLaunch = staticObj.playAfterLaunchToggle.isOn;

    }

    private void MethodContinue()
    {
        this.continueWithPreScne = staticObj.continueWithPreScneToggle.isOn;
    }

    public static void RemoveAllListenerOnToggle()
    {
        staticObj.continueWithPreScneToggle.onValueChanged.RemoveAllListeners();
        staticObj.playAfterLaunchToggle.onValueChanged.RemoveAllListeners();
        staticObj.playOnClickToggle.onValueChanged.RemoveAllListeners();

    }

    //reset on the new line item
    public static void ResetValues()
    {
        staticObj.continueWithPreScneToggle.isOn = false;
        staticObj.playAfterLaunchToggle.isOn = false;
        staticObj.playOnClickToggle.isOn = false;
    }

    public void SetBoolValueToToggle()
    {

        staticObj.continueWithPreScneToggle.isOn =this. continueWithPreScne;
        staticObj.playAfterLaunchToggle.isOn =this. playAfterLaunch;
        staticObj.playOnClickToggle.isOn = this.playOnClick;

    }

 



}
