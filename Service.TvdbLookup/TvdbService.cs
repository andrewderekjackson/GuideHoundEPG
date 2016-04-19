using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Xml.Linq;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using Services.TvdbLookup;
using Services.TvdbLookup.Model;
using System.IO;


namespace Services.TvdbLookup
{
    public class TvdbService {

        private TvdbServiceConfiguration configuration;

        public TvdbService(TvdbServiceConfiguration configuration) {
            this.configuration = configuration;    
        }

        public TvdbServiceConfiguration Configuration { 
            get {
                return configuration;
            }
            set {
                configuration = value;
            }
        }

        public List<TvdbSeriesBase> Search(string title) {
            // http://www.thetvdb.com/api/GetSeries.php?seriesname=Test
            
            Uri requestUri = new Uri(String.Format("http://www.thetvdb.com/api/GetSeries.php?seriesname={0}", HttpUtility.UrlEncode(title)));

            WebRequest request = WebRequest.Create(requestUri);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode==HttpStatusCode.OK) {

                XmlReader xmlReader = XmlReader.Create(response.GetResponseStream());
                XDocument document = XDocument.Load(xmlReader);

                var series = from s in document.Descendants("Series")
                             select new TvdbSeriesBase {
                                   SeriesId = (string)s.GetValueOrDefault("seriesid",""),
                                   SeriesName = (string)s.GetValueOrDefault("SeriesName",""),
                                   Banner = (string)s.GetValueOrDefault("banner",""),
                                   TvdbId = (string)s.GetValueOrDefault("id",""),
                                   ImdbId = (string)s.GetValueOrDefault("IMDB_ID",""),
                                   Overview = (string)s.GetValueOrDefault("Overview",""),
                                   Language= (string)s.GetValueOrDefault("language",""),
                                   FirstAired = DateTime.Parse((String)s.GetValueOrDefault("FirstAired",DateTime.MinValue.ToString()))
                               };

                return series.ToList();
                

            } else {
                return null;
            }
        }

        public Bitmap DownloadBannerImage(string bannerPath) {
           
            // http://images.thetvdb.com/banners/graphical/80348-g.jpg

            try {
                WebRequest requestPic = WebRequest.Create(bannerPath);
                WebResponse responsePic = requestPic.GetResponse();

                Bitmap bitmap = new Bitmap(responsePic.GetResponseStream());
                return bitmap;
            } catch (Exception) {
                return null;
            }

        }


        public TvdbSeriesFull GetFullSeries(string id) {
            
            // http://thetvdb.com/api/CE72338B4ECE6EDA/series/80348/all/en.zip
            
            Uri requestUri = new Uri(String.Format("http://thetvdb.com/api/{0}/series/{1}/all/en.zip", configuration.ApiKey, id));

            WebRequest request = WebRequest.Create(requestUri);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode==HttpStatusCode.OK) {

                // extract the zip file and read the contained files
                ZipInputStream zipStream = new ZipInputStream(response.GetResponseStream());

                ZipEntry entry = zipStream.GetNextEntry();
                String seriesString = null;
                String actorsString = null;
                String bannersString = null;
                while (entry != null) {
                    byte[] buffer = new byte[zipStream.Length];
                    int count = zipStream.Read(buffer, 0, (int) zipStream.Length);
                    if (entry.Name.Equals("en.xml")) {
                        seriesString = Encoding.UTF8.GetString(buffer);
                    }
                    else if (entry.Name.Equals("banners.xml")) {
                        bannersString = Encoding.UTF8.GetString(buffer);
                    }
                    else if (entry.Name.Equals("actors.xml")) {
                        actorsString = Encoding.UTF8.GetString(buffer);
                    }
                    entry = zipStream.GetNextEntry();
                }
                zipStream.Close();

                TvdbSeriesFull fullSeriesInfo = new TvdbSeriesFull();


                // parse the results

                // <Series>
                //  <id>80348</id>
                //  <Actors>|Zachary Levi|Yvonne Strahovski|Adam Baldwin|Bonita Friedericy|Julia Ling|Vik Sahay|Ryan McPartin|Scott Krinsky|Mark Christopher Lawrence|Sarah Lancaster|Joshua Gomez|</Actors>
                //  <Airs_DayOfWeek>Monday</Airs_DayOfWeek>
                //  <Airs_Time>8:00 PM</Airs_Time>
                //  <ContentRating>TV-PG</ContentRating>
                //  <FirstAired>2007-09-24</FirstAired>
                //  <Genre>|Action and Adventure|Comedy|Drama|</Genre>
                //  <IMDB_ID>tt0934814</IMDB_ID>
                //  <Language>en</Language>
                //  <Network>NBC</Network>
                //  <NetworkID></NetworkID>
                //  <Overview>Chuck is a nerdy computer tech who has a database of government secrets downloaded into his brain. He soon finds himself recruited by veteran NSA agent John Casey for espionage work with his new partner Sarah, leading Chuck to live a split life of computer geek and secret NSA missions.</Overview>
                //  <Rating>8.6</Rating>
                //  <Runtime>60</Runtime>
                //  <SeriesID>68724</SeriesID>
                //  <SeriesName>Chuck</SeriesName>
                //  <Status>Continuing</Status>
                //  <added></added>
                //  <addedBy></addedBy>
                //  <banner>graphical/80348-g.jpg</banner>
                //  <fanart>fanart/original/80348-10.jpg</fanart>
                //  <lastupdated>1249247683</lastupdated>
                //  <poster>posters/80348-9.jpg</poster>
                //  <zap2it_id>SH930779</zap2it_id>
                //</Series>

                // get the genres
                if (seriesString != null) {
                    XDocument seriesInfoDocument = XDocument.Parse(seriesString);

                    var seriesInfo = (from s in seriesInfoDocument.Descendants("Series")
                                      select new {
                                                     Genre = (string) s.GetValueOrDefault("Genre", ""),
                                                 }).FirstOrDefault();



                    string[] splitList = seriesInfo.Genre.Split('|');

                    foreach (var split in splitList) {
                        if (!String.IsNullOrEmpty(split)) {
                            fullSeriesInfo.Genres.Add(new TvdbGenre() {GenreName = split});
                        }
                    }

                }

                // find the poster url
                if (bannersString != null) {

                    //<Banner>
                    //  <id>43274</id>
                    //  <BannerPath>posters/80348-9.jpg</BannerPath>
                    //  <BannerType>poster</BannerType>
                    //  <BannerType2>680x1000</BannerType2>
                    //  <Language>en</Language>
                    //</Banner>

                    XDocument bannersInfoDocument = XDocument.Parse(bannersString);

                    var bannersInfo = (from s in bannersInfoDocument.Descendants("Banner")
                                       select new {
                                                      BannerId = (string) s.GetValueOrDefault("id", ""),
                                                      BannerPath = (string) s.GetValueOrDefault("BannerPath", ""),
                                                      BannerType = (string) s.GetValueOrDefault("BannerType", ""),
                                                      BannerType2 = (string) s.GetValueOrDefault("BannerType2", ""),
                                                      Language = (string) s.GetValueOrDefault("Language", "")
                                                  });

                    var banner = bannersInfo.Where(e => e.BannerType == "poster").FirstOrDefault();
                    if (banner != null) {
                        // actually download the banner iamge
                        string bannerPath = String.Format("http://images.thetvdb.com/banners/{0}", banner.BannerPath);
                        Bitmap bannerImage = DownloadBannerImage(bannerPath);

                        fullSeriesInfo.PosterImage = bannerImage;
                        fullSeriesInfo.PosterUrl = bannerPath;
                    }

                }

            return fullSeriesInfo;

            } else {
                return null;
            }
        }
        
    }

    public static class Extensions {

        public static object GetValueOrDefault(this XElement element, XName name, object defaultValue) {
            return element.Element(name) == null ? defaultValue : element.Element(name).Value;
        }

    }
    
}
