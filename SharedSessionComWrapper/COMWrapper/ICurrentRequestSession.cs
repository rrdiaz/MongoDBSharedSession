using System;
using System.Runtime.InteropServices;
namespace BancoEstado.SharedSessionServer.COMWrapper
{
    [ComVisible(true)]
    [Guid("77854B80-5B6D-403A-BF03-8F12A127BCD7")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    interface ICurrentRequestSession
    {
        SessionValue CreateSessionValue();
        void Delete();
        void Dispose();
        string Get(string key);
        string GetSessionId();
        string Id { get; }
        void PutInSessionServer(SessionValue keyValue);
        bool RemoveSession { get; }
        string SessionCookieName { get; }
        void Set(string key, string value);
        bool ValueChanged { get; }
    }
}
