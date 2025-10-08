using System;

public interface ISkillObserver<T>
{
    void OnNext(T value);
    void OnCompleted();
    void OnError(Exception error);

}