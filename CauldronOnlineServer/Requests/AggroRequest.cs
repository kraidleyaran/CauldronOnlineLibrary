using System;

namespace CauldronOnlineServer.Requests
{
    public class AggroRequest
    {
        public string Target;

        public bool Remove;
        //Removed, Owner
        public Action<bool,string> DoAfer;

        public AggroRequest(string target, Action<bool, string> doAfter, bool remove = false)
        {
            Target = target;
            DoAfer = doAfter;
            Remove = remove;
        }
    }
}