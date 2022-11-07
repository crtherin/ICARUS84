using UnityEngine;
using Object = UnityEngine.Object;

public enum MecanimEventParameterType
{
    NoParameter,
    BoolParameter,
    IntParameter,
    FloatParameter,
    StringParameter,
    ObjectParameter
}

[System.Serializable]
public class MecanimEvent
{
    public string FunctionName;

    public MecanimEventParameterType ParameterType;
    public int IntParameter;
    public bool BoolParameter;
    public float FloatParameter;
    public string StringParameter;
    public Object ObjectParameter;

    [HideInInspector] public bool CanTrigger;

    public static void TriggerEvent (GameObject target, MecanimEvent mecanimEvent)
    {
        if (mecanimEvent.ParameterType == MecanimEventParameterType.NoParameter)
            target.SendMessage (
                mecanimEvent.FunctionName,
                SendMessageOptions.DontRequireReceiver);
        else
            target.SendMessage (
                mecanimEvent.FunctionName,
                GetParameter (mecanimEvent),
                SendMessageOptions.DontRequireReceiver);
    }

    private static object GetParameter (MecanimEvent mecanimEvent)
    {
        switch (mecanimEvent.ParameterType)
        {
            case MecanimEventParameterType.NoParameter:
                return null;
            
            case MecanimEventParameterType.BoolParameter:
                return mecanimEvent.BoolParameter;

            case MecanimEventParameterType.IntParameter:
                return mecanimEvent.IntParameter;

            case MecanimEventParameterType.FloatParameter:
                return mecanimEvent.FloatParameter;

            case MecanimEventParameterType.StringParameter:
                return mecanimEvent.StringParameter;

            case MecanimEventParameterType.ObjectParameter:
                return mecanimEvent.ObjectParameter;

            default:
                throw new System.ArgumentOutOfRangeException ();
        }
    }
}

[System.Serializable]
public class MecanimEventTimed : MecanimEvent
{
    public float Time;
    public bool RepeatOnLoop;
    public bool AlwaysTrigger;
}