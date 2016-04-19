using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Microsoft.Win32.TaskScheduler;
using GuideHoundEPG.Common.Configuration;
using GuideHoundEPG.Common.Model.Configuration;
using System.Diagnostics;

namespace TestConsoleApp {
    class Program {
        static void Main(string[] args) {

            // Get the service on the local machine
            using (TaskService ts = new TaskService()) {

                // Create a new task definition and assign properties
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "Does something";

                // Create a trigger that will fire the task at this time every other day
                td.Triggers.Add(new DailyTrigger { DaysInterval = 1});

                // Create an action that will launch Notepad whenever the trigger fires
                td.Actions.Add(new ExecAction("notepad.exe", "c:\\test.log", null));
                
                td.Principal.LogonType = TaskLogonType.ServiceAccount;
                td.Principal.UserId = "System";

                // Register the task in the root folder
                ts.RootFolder.RegisterTaskDefinition(@"Test", td);

                // Remove the task we just created
                //ts.RootFolder.DeleteTask("Test");

            }

            


        }
    }
}
