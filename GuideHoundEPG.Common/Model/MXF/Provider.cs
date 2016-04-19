using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace GuideHoundEPG.Common.Model.MXF {
    public class Provider  {

        public Provider() {
            Keywords = new ObservableCollection<Keyword>();
            KeywordGroups = new ObservableCollection<KeywordGroup>();
            Programs = new ObservableCollection<ProgramInfo>();
            Series = new ObservableCollection<SeriesInfo>();
            ScheduleEntries = new ObservableCollection<ScheduleEntry>();
            Channels = new ObservableCollection<Channel>();
            GuideImages = new ObservableCollection<GuideImage>();
            Affiliates = new ObservableCollection<Affiliate>();
            Lineups = new ObservableCollection<Lineup>();
            Services = new ObservableCollection<Service>();
            People = new ObservableCollection<Person>();

            //(Services as INotifyCollectionChanged).CollectionChanged += CollectionWithGuideImagesChanged;
            //(Affiliates as INotifyCollectionChanged).CollectionChanged += CollectionWithGuideImagesChanged;
            //(Programs as INotifyCollectionChanged).CollectionChanged += CollectionWithGuideImagesChanged;
        }

        void CollectionWithGuideImagesChanged(object sender, NotifyCollectionChangedEventArgs e) {
            UpdateGuideImages();
        }

        public String Id { get; set; }
        public String Name { get; set; }
        public String DisplayName { get; set; }
        public String Copyright { get; set; }

        public ObservableCollection<Lineup> Lineups { get; set; }
        public ObservableCollection<Affiliate> Affiliates { get; set; }
        public ObservableCollection<Keyword> Keywords { get; set; }
        public ObservableCollection<KeywordGroup> KeywordGroups { get; set; }
        public ObservableCollection<ProgramInfo> Programs { get; set; }
        public ObservableCollection<SeriesInfo> Series { get; set; }
        public ObservableCollection<Service> Services { get; set; }
        public ObservableCollection<ScheduleEntry> ScheduleEntries { get; set; }
        public ObservableCollection<Channel> Channels { get; set; }
        public ObservableCollection<GuideImage> GuideImages { get; set; }
        public ObservableCollection<Person> People { get; set; }

        private List<ModelValidationError> modelValidationErrors = new List<ModelValidationError>();

        /// <summary>
        /// Adds a lineup to the provider.
        /// </summary>
        /// <param name="lineup"></param>
        public void AddLineup(Lineup lineup) {
            if (!Lineups.Contains(lineup)) {
                Lineups.Add(lineup);
            }
        }

        /// <summary>
        /// Adds an affiliate to the provider.
        /// </summary>
        /// <param name="affiliate"></param>
        public void AddAffiliate(Affiliate affiliate) {

            if (!Affiliates.Contains(affiliate)) {
                Affiliates.Add(affiliate);
            }

            UpdateGuideImages();
            
        }

        private void UpdateGuideImages() {

            foreach (var program in Programs) {
                if (program.GuideImage != null) {
                    GuideImage image = GuideImages.FirstOrDefault(e=>e.Uid==program.GuideImage.Uid);
                    if (image==null) {
                        GuideImages.Add(program.GuideImage);
                    } else {
                        image.UpdateFrom(program.GuideImage);
                    }
                }
            }

            foreach (var service in Services) {
                if (service.LogoImage != null) {
                    GuideImage image = GuideImages.FirstOrDefault(e => e.Uid == service.LogoImage.Uid);
                    if (image == null) {
                        GuideImages.Add(service.LogoImage);
                    } else {
                        image.UpdateFrom(service.LogoImage);
                    }
                }
            }

            foreach (var affiliate in Affiliates) {
                if (affiliate.LogoImage != null) {
                    GuideImage image = GuideImages.FirstOrDefault(e => e.Uid == affiliate.LogoImage.Uid);
                    if (image == null) {
                        GuideImages.Add(affiliate.LogoImage);
                    } else {
                        image.UpdateFrom(affiliate.LogoImage);
                    }
                }
            }

            foreach (var series in Series) {
                if (series.GuideImage != null) {
                    GuideImage image = GuideImages.FirstOrDefault(e => e.Uid == series.GuideImage.Uid);
                    if (image == null) {
                        GuideImages.Add(series.GuideImage);
                    } else {
                        image.UpdateFrom(series.GuideImage);
                    }
                }
            }

            //foreach (var service in Services) {
            //    if ((service.LogoImage != null) && !GuideImages.Contains(service.LogoImage)) {
            //        GuideImages.Add(service.LogoImage);
            //    }
            //}

            //foreach (var affiliate in Affiliates) {
            //    if ((affiliate.LogoImage != null) && !GuideImages.Contains(affiliate.LogoImage)) {
            //        GuideImages.Add(affiliate.LogoImage);
            //    }
            //}

        }

        /// <summary>
        /// Adds a service to the provider for an affiliate.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="affiliate"></param>
        public void AddService(Service service) {

            if (!Services.Contains(service)) {

                string newServiceId = "s" + (Services.Count + 1);

                if (service.Affiliate == null) {
                    throw new InvalidOperationException("An affiliate is required.");
                }

                AddAffiliate(service.Affiliate);

                service.Id = newServiceId;
                Services.Add(service);
            }

            UpdateGuideImages();

        }

        /// <summary>
        /// Adds a service to the provider for an affiliate.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="affiliate"></param>
        public void AddChannel(Channel channel) {

            if (channel.Lineup == null) {
                throw new InvalidOperationException("A lineup is required.");
            }
            AddLineup(channel.Lineup);

            if (channel.Service == null) {
                throw new InvalidOperationException("A service is required.");
            }
            AddService(channel.Service);

            if (!Channels.Contains(channel)) {
                this.Channels.Add(channel);
            }

        }

        #region IModelValidation

        public bool Validate() {
                
            modelValidationErrors.Clear();

            // make sure we have a lineup with a primary lineup.
            if (Lineups.Count == 0) {
                modelValidationErrors.Add(new ModelValidationError { Error = "No lineups have been added." });
            } else {
                if (Lineups.Where(e => (e.IsPrimaryLineup)).Count() != 1) {
                    modelValidationErrors.Add(new ModelValidationError { Error = "No primary lineup." });
                }
            }

            return (modelValidationErrors.Count == 0);

        }

        public List<ModelValidationError> GetValidationErrors() {
            Validate();
            return modelValidationErrors; 
        }

        #endregion

        ///// <summary>
        ///// Sets the channel logo for a service.
        ///// </summary>
        ///// <param name="service"></param>
        ///// <param name="guideImage"></param>
        //public void SetChannelLogo(Service service, GuideImage guideImage) {

        //    string newChannelLogoId = "i" + (Services.Count + 1);
        //    if (!GuideImages.Contains(guideImage)) {

        //        guideImage.Id = newChannelLogoId;
        //        guideImage.OutputImageType = GuideImageType.ChannelLogo;
                    
        //        // AddGuideImage(guideImage);
        //    }

        //    service.LogoImage = guideImage;

        //}

        public void BuildProvider() {
            UpdateGuideImages();
        }

        /// <summary>
        /// Adds a program to the provider.
        /// </summary>
        /// <param name="program"></param>
        public void AddProgram(ProgramInfo program) {

            if (String.IsNullOrEmpty(program.Id) || Programs.FirstOrDefault(e=>e.Id==program.Id)!=null) {
                throw new InvalidOperationException("A program needs to have a valid ID which is unique.");
            }

            if (program.DurationInMinutes <= 0) {
                throw new InvalidOperationException("Program needs a duration which is greater than 0.");
            }

            if (!Programs.Contains(program)) {
                Programs.Add(program);
            }

            if (program.Series != null && !Series.Contains(program.Series)) {
                Series.Add(program.Series);
            }

        }

        public void AddScheduleEntries(ScheduleEntry entry) {

            if (!ScheduleEntries.Contains(entry)) {
                ScheduleEntries.Add(entry);
            }

        }

        public void AddPerson(Person person) {
            if (!People.Contains(person)) {
                People.Add(person);
            }
        }
    }
}