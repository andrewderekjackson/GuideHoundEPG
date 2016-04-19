using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GuideHoundEPG.Common.Model.Configuration {
    [Serializable, XmlType("epgDownloaderConfig")]
    public class EPGDownloaderConfig {

        public EPGDownloaderConfig() {
            this.Sources = new List<Source>();            
            
            this.Output = new Output();
            this.CategoryDetection = new CategoryDetection();
            this.CategoryMapping = new CategoryMapping();
            this.MetadataOptions = new MetadataOptions();
            this.SchedulerOptions = new SchedulerOptions();
        }

        [XmlAttribute("providerName")]
        public String ProviderName { get; set; }
        
        [XmlElement("output")]
        public Output Output{ get; set; }

        [XmlArray("sources")]
        public List<Source> Sources { get; set; }

        [XmlElement("categoryMapping")]
        public CategoryMapping CategoryMapping { get; set; }

        [XmlElement("categoryDetection")]
        public CategoryDetection CategoryDetection{ get; set; }

        [XmlElement("metadataOptions")]
        public MetadataOptions MetadataOptions{ get; set; }

        [XmlElement("schedulerOptions")]
        public SchedulerOptions SchedulerOptions { get; set; }

    }

    [Serializable, XmlType("source")]
    [XmlInclude(typeof(XmlSource))]
    public abstract class Source {

        public Source() {
            this.SourceEnabled = true;
        }

        [XmlAttribute("sourceName")]
        public String SourceName { get; set; }

        [XmlAttribute("sourceEnabled")]
        public bool SourceEnabled { get; set; }

        [XmlAttribute("command")]
        public String Command { get; set; }

        [XmlAttribute("commandEnabled")]
        public bool CommandEnabled { get; set; }

        [XmlAttribute("commandStartFolder")]
        public string CommandStartFolder { get; set; }

    }

    [Serializable, XmlType("xmlTvSource")]
    public class XmlSource : Source {

        public XmlSource():base() {
            this.Channels = new List<Channel>();
        }

        [XmlAttribute("sourceUri")]
        public String SourceUri { get; set; }

        [XmlArray("channels")]
        public List<Channel> Channels { get; set; }
    
    }

    [Serializable, XmlType("categoryMapping")]
    public class CategoryMapping {

        public CategoryMapping() {
            CategoryMap = new List<CategoryMap>();
        }

        [XmlAttribute("categoryMappingEnabled")]
        public bool CategoryMappingEnabled { get; set; }

        [XmlArray("categoryMap")]
        public List<CategoryMap> CategoryMap { get; set; }

    }

    [Serializable, XmlType("categoryMap")]
    public class CategoryMap {
        
        public CategoryMap() {
        }

        [XmlAttribute("sourceCategory")]
        public string SourceCategory { get; set; }

        [XmlAttribute("mappedCategory")]
        public ProgramCategory MappedCategory { get; set; }

    }

    [Serializable, XmlType("categoryDetection")]
    public class CategoryDetection {
        public CategoryDetection() {
            Keywords = new List<CategoryKeyword>();
        }

        [XmlAttribute("basicDetectionEnabled")]
        public bool BasicDetectionEnabled { get; set; }

        [XmlAttribute("movieDurationMatchingEnabled")]
        public bool MovieDurationMatchingEnabled { get; set; }

        [XmlAttribute("minimumMovieDurationInMinutes")]
        public int MinimumMovieDurationInMinutes { get; set; }

        [XmlAttribute("maximumMovieDurationInMinutes")]
        public int MaximumMovieDurationInMinutes { get; set; }

        [XmlAttribute("keywordDetectionEnabled")]
        public bool KeywordDetectionEnabled { get; set; }

        [XmlAttribute("useDefaultIfUnmatched")]
        public bool UseDefaultIfUnmatched { get; set; }

        [XmlAttribute("unmatchedCategory")]
        public ProgramCategory UnmatchedCategory { get; set; }

        [XmlArray("keywords")]
        public List<CategoryKeyword> Keywords { get; set; }

    }

    [Serializable, XmlType("keyword")]
    public class CategoryKeyword
    {

        [XmlAttribute("word")]
        public String Word { get; set; }

        [XmlAttribute("removeKeyword")]
        public bool RemoveKeyword{ get; set; }

        [XmlAttribute("category")]
        public ProgramCategory Category { get; set; }

    }

    [Serializable, XmlType("output")]
    public class Output {

        public Output() {
            ImportIntoMediaCenter = true;
        }

        [XmlAttribute("importIntoMediaCenter")]
        public bool ImportIntoMediaCenter { get; set; }

    }

    [Serializable, XmlType("channel")]
    public class Channel
    {

        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("xmlTvId")]
        public String XmlTvId { get; set; }

        [XmlAttribute("name")]
        public String Name { get; set; }

        [XmlAttribute("logoUrl")]
        public String LogoUrl { get; set; }

        [XmlAttribute("isAudio")]
        public bool IsAudio { get; set; }

        [XmlAttribute("isEnabled")]
        public bool IsEnabled { get; set; }
    }

    [Serializable, XmlType("metadata")]
    public class MetadataOptions
    {

        [XmlAttribute("tvSeriesMetadataServiceEnabled")]
        public bool TvSeriesMetadataServiceEnabled { get; set; }

        [XmlAttribute("movieMetadataServiceEnabled")]
        public bool MovieMetadataServiceEnabled { get; set; }


    }

    [Serializable, XmlType("schedulerOptions")]
    public class SchedulerOptions {

        public SchedulerOptions() {
        }

        [XmlAttribute("schedulerEnabled")]
        public bool SchedulerEnabled { get; set; }

        [XmlAttribute("startTime")]
        public DateTime StartTime { get; set; }

    }
}