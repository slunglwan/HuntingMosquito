using System;

public interface ISkillObservable<T>
{
    void Subscribe(ISkillObserver<T> observer);
    void UnSubscribe(ISkillObserver<T> observer);
    void Notify(T value);
}