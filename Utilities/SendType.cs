namespace Networking
{
    /// <summary>
    /// Defines how message will be send. 
    /// </summary>
    internal enum SendType
    {
        SendReliable,       //  System send and check if message arrived to destination.
        SendUnreiable,      //  System send message without only.
        UpdateReliable,     //  System send and check if message arrived to destination. If message was already send override it on destination. 
        UpdateUnreiable     //  System send message. If message was already send override it on destination.
    }
}