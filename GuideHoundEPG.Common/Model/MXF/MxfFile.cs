using System.Collections.Generic;
using System;
using System.Linq;

namespace GuideHoundEPG.Common.Model.MXF {
    
    public class MxfFile {
        public List<Provider> Providers { get; set; }
        
        public MxfFile() {
            Providers = new List<Provider>();
        }

        /// <summary>
        /// Adds a provider to the list of providers.
        /// </summary>
        /// <param name="provider"></param>
        public void AddProvider(Provider provider) {

            string newProviderId = "provider" + (Providers.Count + 1).ToString();

            if (!String.IsNullOrEmpty(provider.Id) 
                || Providers.Contains(provider) 
                || Providers.Where(e=>e.Id==newProviderId).Count()!=0 ) {

                    throw new MxfValidationException("Provider has already been added."); 
            }

            // make sure we have at least one lineup with one marked as primary.
            if (provider.Lineups.Count == 0) {
                throw new MxfValidationException("No lineups have been added.");
            } else {
                if (provider.Lineups.Where(e => (e.IsPrimaryLineup)).Count() != 1) {
                    throw new MxfValidationException("A provider must have a primary lineup.");
                }
            }

            provider.Id = newProviderId;
            Providers.Add(provider);
            
        }
    }
}