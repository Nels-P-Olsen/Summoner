using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Growl.Connector;
using Growl.CoreLibrary;

namespace Summoner
{
    class GrowlHelper
    {
        public static void simpleGrowl(string title, string message = "")
        {
            GrowlConnector simpleGrowl = new GrowlConnector();
            Growl.Connector.Application thisApp = new Growl.Connector.Application(Constants.ApplicationName);
            NotificationType simpleGrowlType = new NotificationType("SIMPLEGROWL");
            simpleGrowl.Register(thisApp, new NotificationType[] { simpleGrowlType });
            Notification myGrowl = new Notification(Constants.ApplicationName, "SIMPLEGROWL", title, title, message);
            simpleGrowl.Notify(myGrowl);
        }
    }
}
