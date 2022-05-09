using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;

namespace SsmsLite.Core.Ui
{
    public class EmptyAutomationPeer : FrameworkElementAutomationPeer
    {
        public EmptyAutomationPeer(FrameworkElement owner) : base(owner) { }

        protected override string GetNameCore()
        {
            return "EmptyAutomationPeer";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Window;
        }

        protected override List<AutomationPeer> GetChildrenCore()
        {
            return new List<AutomationPeer>();
        }
    }
}
