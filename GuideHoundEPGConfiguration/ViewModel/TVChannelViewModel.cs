using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Core.UI;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace GuideHoundEPG.UI.ViewModel
{
    public class TVChannelViewModel : Core.UI.ViewModel {

        public TVChannelViewModel() {
        }

        public int ChannelId
        {
            get { return channelId; }
            set { channelId = value; 
                RaisePropertyChanged("ChannelId"); }
        }

        public string XmlTvChannelId
        {
            get { return xmlTvChannelId; }
            set { xmlTvChannelId = value; 
                RaisePropertyChanged("XmlTvChannelId"); }
        }
        
        public String ChannelName {
            get { return channelName; }
            set { channelName = value; 
                RaisePropertyChanged("ChannelName");}
        }
        
        public string LogoUrl {
            get { return logoUrl; }
            set { logoUrl = value; 
                RaisePropertyChanged("LogoUrl");
                RaisePropertyChanged("HasLogoUrl");
            }
        }

        public bool HasLogoUrl {
            get { return !String.IsNullOrEmpty(LogoUrl); }
            
        }

        public bool IsAudio {
            get { return isAudio; }
            set {
                isAudio = value;
                RaisePropertyChanged("IsAudio");
            }
        }

        public bool IsEnabled {
            get { return isEnabled; }
            set {
                isEnabled = value;
                RaisePropertyChanged("IsEnabled");
            }
        }

        public XmlTvSourceViewModel Source {
            get { return source; }
            set { 
                source = value; 
                RaisePropertyChanged("Source");
                RaisePropertyChanged("SourceName"); 
            }
        }

        public string SourceName {
            get { return Source==null ? "Unknown" : Source.SourceName; }
        }

        public ICommand ClearLogo {
            get {
                return clearLogoCommand ?? (clearLogoCommand = new RelayCommand(() => {
                    LogoUrl = String.Empty;
                }));
            }
        }


        private int channelId;
        private string xmlTvChannelId;
        private string channelName;
        private string logoUrl;
        private XmlTvSourceViewModel source;
        private bool isAudio;
        private bool isEnabled;
        private ICommand clearLogoCommand;


       
    }
}
