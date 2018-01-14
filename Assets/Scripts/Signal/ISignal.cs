using System;
using System.Collections.Generic;

public interface ISignal
{
    void AddListener(Action<ISignal,object[]> action);
    void AddOnceListener(Action<ISignal,object[]> action);
    void RemoveListener(Action<ISignal,object[]> action);
    List<Type> GetTypes();
    void Dispatch(object[] args);
}