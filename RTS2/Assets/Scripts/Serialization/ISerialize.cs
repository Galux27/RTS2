using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerialize
{

    public DataToSerialize GetDataToSerialize();
    public SerializedData Serialize();
    public void Deserialize(SerializedData data);

    public UID GetMyUID();
    
}