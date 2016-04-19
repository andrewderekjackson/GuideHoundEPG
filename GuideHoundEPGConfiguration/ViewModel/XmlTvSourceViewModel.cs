using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using Core.UI;

namespace GuideHoundEPG.UI.ViewModel {

    public class XmlTvSourceViewModel : Core.UI.ViewModel, IDataErrorInfo {

        public XmlTvSourceViewModel() {
            this.SourceEnabled = true;
        }

        public string SourceName {
            get { return sourceName; }
            set { 
                sourceName = value; 
                RaisePropertyChanged("SourceName");
                RaisePropertyChanged("SourceDisplayName"); 
            }
        }

        public string SourceDisplayName {
            get {
                if (sourceEnabled) {
                    return sourceName;
                } else {
                    return String.Format("{0} ({1})", sourceName, "Disabled");
                }
            }
        }

        public string SourceUri {
            get { return sourceUri; }
            set { 
                sourceUri = value; 
                RaisePropertyChanged("SourceUri"); 
            }
        }

        public string SourceCache {
            get { return sourceCache; }
            set {
                sourceCache = value;
                RaisePropertyChanged("SourceCache");
            }
        }

        public string CommandString {
            get { return commandString; }
            set {
                commandString = value;
                RaisePropertyChanged("CommandString");
            }
        }

        public string CommandStartFolder {
            get { return commandStartFolder; }
            set {
                commandStartFolder = value;
                RaisePropertyChanged("CommandStartFolder");
            }
        }

        public bool CommandEnabled {
            get { return commandEnabled; }
            set {
                commandEnabled = value;
                RaisePropertyChanged("CommandEnabled");
            }
        }


        
        public bool SourceEnabled {
            get { return sourceEnabled; }
            set {
                sourceEnabled = value;
                RaisePropertyChanged("SourceEnabled");
                RaisePropertyChanged("SourceDisplayName"); 
            }
        }

        private string sourceUri;
        private string sourceName;
        private string sourceCache;
        private string commandString;
        private bool canSave;
        private bool commandEnabled;
        private bool sourceEnabled;
        private string commandStartFolder;

        public string Error {
            get { return null; }
        }

        public string this[string columnName] {
            get {

                if (columnName == "SourceName") {
                    if (String.IsNullOrEmpty(SourceName)) {
                        CanSave = false;
                        return "Please enter a source name";
                    }
                }

                if (columnName == "SourceUri") {
                    if (String.IsNullOrEmpty(SourceUri)) {
                        CanSave = false;
                        return "Please enter a source filename or internet address.";
                    
                    }
                }
                
                CanSave = true;
                return String.Empty;
            }
        }

        public bool CanSave {
            get {
                return canSave;
            }
            set {
                canSave = true;
                RaisePropertyChanged("CanSave");
            }
            
        }
       
       
    }

    public class IsNotNullOrEmptyValidationRule : ValidationRule {
        
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo) {

            string strVal = value as string;
            if (string.IsNullOrEmpty(strVal)) {
                return new ValidationResult(false, "This is a required field");
            }

            return ValidationResult.ValidResult;
        }

    }
}
