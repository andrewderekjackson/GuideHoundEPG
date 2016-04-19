using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Core.Logging;
using Core.UI;
using Microsoft.Win32.TaskScheduler;
using GuideHoundEPG.Common;
using GuideHoundEPG.UI.Controllers;
using MessageBox = System.Windows.Forms.MessageBox;
using GalaSoft.MvvmLight.Command;

namespace GuideHoundEPG.UI.ViewModel {

    public class SchedulerOptionsViewModel : Core.UI.ViewModel {

        private static readonly ILogger logWriter = Logger.GetLogger();


        public ConfigViewModel Config { get; set; }

        public SchedulerOptionsViewModel() {
        }

        public SchedulerOptionsViewModel(ConfigViewModel configViewModel) {
            this.Config = configViewModel;
        }


        public ICommand OpenTaskSchedulerCommand {
            get {
                return new RelayCommand(() =>
                {

                    string tm = Path.Combine(Environment.SystemDirectory, "control.exe");

                    ProcessStartInfo psi = new ProcessStartInfo(tm, @"schedtasks");
                    psi.UseShellExecute = false;

                    try {
                        Process.Start(psi);
                    } catch (Exception ex) {
                        MessageBox.Show("Unable to launch the windows task scheduler due to an error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        logWriter.Log(LogLevel.Error, ex, "Unable to open the windows task scheduler.");
                    }

                });
            }
        }

        public ICommand InstallScheduledTaskCommand {
            get {
                return new RelayCommand(() =>
                {

                    DialogResult result = MessageBox.Show("Do you want to create a scheduled task for GuideHound EPG?",
                                                          "Create Scheduled Task", MessageBoxButtons.YesNo,
                                                          MessageBoxIcon.Question);

                    if (result == DialogResult.No) {
                        return;
                    }

                    string pathToCommand = Path.Combine(EnvironmentInfo.AppPath, "GuideHoundEPG.exe");

                    try {

                        // Get the service on the local machine
                        using (TaskService ts = new TaskService()) {

                            // Create a new task definition and assign properties
                            TaskDefinition td = ts.NewTask();
                            td.RegistrationInfo.Description = "GuideHound EPG";

                            // Create a trigger that will fire the task at this time every other day
                            td.Triggers.Add(new DailyTrigger {
                                                                 StartBoundary = DateTime.Today.AddHours(3)
                                                             });

                            // Create an action that will launch Notepad whenever the trigger fires
                            td.Actions.Add(new ExecAction(pathToCommand, String.Empty, EnvironmentInfo.AppPath));

                            td.Principal.LogonType = TaskLogonType.ServiceAccount;
                            td.Principal.UserId = "System";

                            // Register the task in the root folder
                            ts.RootFolder.RegisterTaskDefinition(@"GuideHound EPG", td);

                            MessageBox.Show("Scheduled task created successfully.", "Success", MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);

                        }
                    }
                    catch (Exception ex) {
                        MessageBox.Show("Unable to create scheduled task:" + Environment.NewLine + ex.Message, "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                        logWriter.Log(LogLevel.Error, ex, "Unable to create scheduled task.");
                    }
                });
            }

        }


    }
}
