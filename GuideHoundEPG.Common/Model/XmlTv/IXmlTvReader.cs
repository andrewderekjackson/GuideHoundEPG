using System.Collections.Generic;

namespace GuideHoundEPG.Common.Model.XmlTv {
    /// <summary>
    /// Interface for xmltv reader implementations
    /// </summary>
    public interface IXmlTvReader {
        /// <summary>
        /// The display name of the reader.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Loads the program list from the xml document.
        /// </summary>
        List<XmlTvProgram> LoadProgramList(string file);

        /// <summary>
        /// Loads the channel list from the xml document.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        List<XmlTvChannel> LoadChannelList(string file);
    }
}