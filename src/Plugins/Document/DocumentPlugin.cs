using System;

namespace SsmsLite.Document
{
    public class DocumentPlugin
    {
        private readonly DocumentUi _documentUi;
        private bool _isRegistered;

        public DocumentPlugin(DocumentUi documentUi)
        {
            _documentUi = documentUi;
        }


        public void Register()
        {
            if (_isRegistered)
            {
                throw new Exception("DocumentPlugin is already registered");
            }

            _isRegistered = true;
            _documentUi.Register();
        }
    }
}
