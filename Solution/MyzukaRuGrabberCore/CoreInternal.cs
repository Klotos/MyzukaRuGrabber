using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using MyzukaRuGrabberCore.DataModels;
using KlotosLib;
using KlotosLib.StringTools;

namespace MyzukaRuGrabberCore
{
    internal static class CoreInternal
    {
        private const StringComparison StrComp = StringComparison.OrdinalIgnoreCase;

        /// <summary>
        /// Пытается сделать запрос по указанному URI, скачать страницу и преобразовать её в HTML-документ
        /// </summary>
        /// <param name="PageURI"></param>
        /// <param name="UserAgent"></param>
        /// <param name="ErrorMessage">Сообщение об ошибке, если она произошла</param>
        /// <returns></returns>
        internal static HtmlAgilityPack.HtmlDocument TryGrabPage(Uri PageURI, String UserAgent, out String ErrorMessage)
        {
            ErrorMessage = null;
            
            if(PageURI == null) {throw new ArgumentNullException("PageURI");}

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PageURI);
            request.Method = "GET";
            request.UserAgent = UserAgent;
            
            StringBuilder raw_HTML;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    raw_HTML = response.GetTextContent();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = String.Format("Ошибка при получении ответа от сервера по URI '{0}'. {1}",
                    PageURI, ex.TotalMessage());
                return null;
            }
            HtmlAgilityPack.HtmlDocument HTML_doc = new HtmlDocument();
            try
            {
                HTML_doc.LoadHtml(raw_HTML.ToString());
            }
            catch (Exception ex)
            {
                ErrorMessage = String.Format(
                    "Ошибка при попытке преобразовать полученный от сервера по URI '{0}' ответ в HTML документ. {1}",
                    PageURI.ToString(), ex.TotalMessage());
                return null;
            }
            
            return HTML_doc;
        }

        internal static HtmlAgilityPack.HtmlDocument TryGrabPageWithCancellation(Uri PageURI, String UserAgent, CancellationToken CancToken)
        {
            if (PageURI == null) { throw new ArgumentNullException("PageURI"); }
            CancToken.ThrowIfCancellationRequested();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PageURI);
            request.Method = "GET";
            request.UserAgent = UserAgent;
            CancToken.ThrowIfCancellationRequested();
            StringBuilder raw_HTML;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    raw_HTML = response.GetTextContent();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException
                    (String.Format("Произошла ошибка при получении ответа от сервера по URI '{0}'", PageURI), ex);
            }
            HtmlAgilityPack.HtmlDocument HTML_doc = new HtmlDocument();
            CancToken.ThrowIfCancellationRequested();
            try
            {
                HTML_doc.LoadHtml(raw_HTML.ToString());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format(
                    "Произошла ошибка при попытке преобразовать полученный от сервера по URI '{0}' HTTP-ответ "+
                    "с содержимым длиной {1} символов в HTML документ", PageURI.ToString(), raw_HTML.Length), ex);
            }
            return HTML_doc;
        }

        /// <summary>
        /// Определяет, является ли указанная страница представлением альбома (true) или одной песни (false). 
        /// Если распознавание невозможно, возвращает NULL.
        /// </summary>
        /// <param name="htmlPage"></param>
        /// <returns></returns>
        internal static ParsedItemType DetectItemType(HtmlDocument htmlPage)
        {
            if(htmlPage == null) {throw new ArgumentNullException("htmlPage");}
            Byte album_symptom = 0;
            Byte song_symptom = 0;

            const String bodyContentPath = "//body/div[@class='all']/div[@class='main']/div[@class='content']/div[@id='bodyContent' and @class='inner']";
            HtmlNode bodyContent = htmlPage.DocumentNode.SelectSingleNode(bodyContentPath);
            if (bodyContent == null) { return ParsedItemType.Unknown; }

            List<HtmlNode> metaNodes = bodyContent.ChildNodes.Where(node =>
                node.Name.Equals("meta", StringComparison.Ordinal) && (node.GetAttributeValue("itemprop", "").IsIn("url", "name"))
            ).ToList();
            if (metaNodes.Count == 2)
            {
                album_symptom++;
            }
            else if(metaNodes.Count == 0)
            {
                song_symptom++;
            }
            else
            {
                return ParsedItemType.Unknown;
            }
            if (album_symptom == 1 && 
                metaNodes.Single(node => node.GetAttributeValue("itemprop", "") == "url").GetAttributeValue("content", "").StartsWith("/Album/"))
            {
                album_symptom++;
            }
            
            const String itemData = "//div[@class='main-details']/div[@class='cont']/div[@class='tbl']/table/tr";
            HtmlNodeCollection rows = bodyContent.SelectNodes(itemData);
            for (int i = 0; i < rows.Count; i++)
            {
                HtmlNode oneRow = rows[i];
                List<HtmlNode> cells = oneRow.ChildNodes.Where(node => node.Name.Equals("td", StringComparison.OrdinalIgnoreCase)).ToList();
                if (cells.Count == 1)
                {
                    song_symptom++;
                    continue;
                }
                else if (cells.Count > 2)
                {
                    return ParsedItemType.Unknown;
                }
                HtmlNode td_Attr = cells[0];
                HtmlNode td_Val = cells[1];
                if (td_Attr.InnerText.Equals("Тип:"))
                {
                    album_symptom++;
                    break;
                }
                if (td_Attr.FirstChild.InnerText.Equals("Альбом:"))
                {
                    song_symptom++;
                    break;
                }
            }
            
            if (album_symptom == 3 && song_symptom == 0)
            { return ParsedItemType.Album; }

            if (song_symptom == 3 && album_symptom == 0)
            { return ParsedItemType.Song; }
            
            return ParsedItemType.Unknown;
        }
        
        /// <summary>
        /// Пытается извлечь и вернуть URI изображения, работает как со страницами альбомов, так и песен. При неудаче возвращает NULL.
        /// </summary>
        /// <param name="HTMLPage"></param>
        /// <returns></returns>
        private static Uri TryGrabImageUri(HtmlDocument HTMLPage)
        {
            const string albumImageElementXpath = "//body/div[@class='all']/div[@class='main']/div[@class='content']/div[@id='bodyContent' and @class='inner']"+
                "/div[@class='main-details']/div[@class='side']/div[@class='vis']/img[@src and @itemprop='image']";
            const string songImageElementXpath = "//body/div[@class='all']/div[@class='main']/div[@class='content']/div[@id='bodyContent' and @class='inner']"+
                "/div[@itemprop='tracks' and @itemscope and @itemtype]/div[@class='main-details']/div[@class='side']/div[@class='vis']/img[@src]";
            HtmlNode imageElement = HTMLPage.DocumentNode.SelectSingleNode(albumImageElementXpath);
            if (imageElement == null)
            {
                imageElement = HTMLPage.DocumentNode.SelectSingleNode(songImageElementXpath);
            }
            if (imageElement == null)
            {
                return null;
            }
            String rawImageUri = imageElement.Attributes["src"].Value;
            Tuple<Uri, String> result = TryFixReturnURI(rawImageUri);
            if (result.Item1 == null)
            {
                throw new InvalidOperationException(
                    "Невозможно подготовить корректный URI, содержащий ссылку на изображение альбома или композиции: " + result.Item2);
            }
            return result.Item1;
        }

        /// <summary>
        /// Возвращает URI альбома, HTML-код страницы которого подан на вход
        /// </summary>
        /// <param name="HTMLPage"></param>
        /// <returns></returns>
        private static Uri ExtractAlbumUri(HtmlDocument HTMLPage)
        {
            const string metaElementXpath =
                "//body/div[@class='all']/div[@class='main']/div[@class='content']/div[@id='bodyContent' and @class='inner']/meta[@itemprop='url']";
            HtmlNode metaElement = HTMLPage.DocumentNode.SelectSingleNode(metaElementXpath);
            if (metaElement == null)
            {
                throw new InvalidOperationException("Невозможно найти HTML элемент, содержащий URL страницы альбома");
            }
            String rawUri = metaElement.Attributes["content"].Value;
            Tuple<Uri, String> result = TryFixReturnURI(rawUri);
            if (result.Item1 == null)
            {
                throw new InvalidOperationException("Невозможно извлечь URI альбома: " + result.Item2);
            }
            return result.Item1;
        }

        /// <summary>
        /// Возвращает название элемента как для песни, так и для альбома. При неудаче возвращает NULL.
        /// </summary>
        /// <param name="InputHTML"></param>
        /// <returns></returns>
        private static String TryGrabCaption(HtmlDocument InputHTML)
        {
            const string albumCaptionXpath = 
                "//body/div[@class='all']/div[@class='main']/div[@class='content']/div[@id='bodyContent' and @class='inner']/h1";
            const string singCaptionXpath =
                "//body/div[@class='all']/div[@class='main']/div[@class='content']/div[@id='bodyContent' and @class='inner']/div[@itemprop='tracks' and @itemscope and @itemtype]/h1";
            HtmlNode caption_node = InputHTML.DocumentNode.SelectSingleNode(albumCaptionXpath);
            if (caption_node == null)
            {
                caption_node = InputHTML.DocumentNode.SelectSingleNode(singCaptionXpath);
            }
            if (caption_node == null)
            {
                return null;
            }
            return HttpUtility.HtmlDecode(caption_node.InnerText.Trim());
        }

        /// <summary>
        /// Извлекает из страницы песни и возвращает URI на скачку файла песни
        /// </summary>
        /// <param name="htmlPage"></param>
        /// <returns></returns>
        internal static Uri ExtractDownloadSongURI(HtmlDocument htmlPage)
        {
            const string downloadAelementXpath =
                "//body/div[@class='all']/div[@class='main']/div[@class='content']/div[@id='bodyContent' and @class='inner']"+
            "/div[@itemprop='tracks' and @itemscope and @itemtype]/div[starts-with(@id,'playerDiv') and contains(@class, 'player-inline')]"+
            "/div[@class='options']/div[@class='top']/a[@id and contains(@class, 'dl') and @itemprop='audio' and @href and @title]";
            HtmlNode aElement = htmlPage.DocumentNode.SelectSingleNode(downloadAelementXpath);
            if (aElement == null) { throw new InvalidOperationException("Невозможно извлечь из страницы песни блок HTML-кода со ссылкой на скачку песни"); }
            Tuple<Uri, String> link = TryFixReturnURI(aElement.GetAttributeValue("href", ""));
            if (link.Item1 == null)
            {
                throw new InvalidOperationException("Невозможно извлечь URI текущей страницы песни: " + link.Item2);
            }
            return link.Item1;
        }

        /// <summary>
        /// Извлекает из страницы песни и возвращает URI на страницу альбома, к которому относится данная песня
        /// </summary>
        /// <param name="htmlPage"></param>
        /// <returns></returns>
        internal static Uri ExtractAlbumURIFromSongPage(HtmlDocument htmlPage)
        {
            if (htmlPage == null) { throw new ArgumentNullException("htmlPage"); }

            const String albumLinkAElementXpath =
                "//body/div[@class='all']/div[@class='main']/div[@class='content']/div[@id='bodyContent' and @class='inner']/div[@class='breadcrumbs']"+
                "/a[@itemprop='url' and @itemscope and @href and @itemtype='http://data-vocabulary.org/Breadcrumb' and starts-with(@href, '/Album/')]";
            HtmlNode albumLinkAElement = htmlPage.DocumentNode.SelectSingleNode(albumLinkAElementXpath);
            if (albumLinkAElement == null)
            { throw new InvalidOperationException("Невозможно извлечь из страницы песни ссылку на альбом, к которому песня относится"); }
            String rawAlbumLink = albumLinkAElement.Attributes["href"].Value;
            Tuple<Uri, String> parsedLink = TryFixReturnURI(rawAlbumLink);
            if (parsedLink.Item1 == null)
            {
                throw new InvalidOperationException(String.Format(
                  "Невозможно извлечь URI на страницу альбома из строки '{0}': {1}",
                  rawAlbumLink, parsedLink.Item2));
            }
            return parsedLink.Item1;
        }

        /// <summary>
        /// Пытается очистить от шелухи и возвратить URI
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static Tuple<Uri, String> TryFixReturnURI(String input)
        {
            String errorMessage = null;
            if (input.HasAlphaNumericChars() == false)
            {
                errorMessage = "URI = '" + input.ToStringS("NULL") + "' заведомо некорректен";
                return new Tuple<Uri, string>(null, errorMessage);
            }
            const String protocol = "https://";
            const String main_domain = "myzuka.me";

            input = HttpUtility.HtmlDecode(input);

            Uri intermediateUri;
            Boolean success1 = Uri.TryCreate(input, UriKind.RelativeOrAbsolute, out intermediateUri);
            if (success1 == false)
            {
                return new Tuple<Uri, string>(null, String.Format("Невозможно корректно распарсить строку '{0}' как корректный URI", input));
            }
            if (intermediateUri.IsAbsoluteUri)
            {
                return new Tuple<Uri, string>(intermediateUri, null);
            }
            else
            {
                Uri absoluteUri;
                string parsedUriString = intermediateUri.ToString();
                if (parsedUriString.StartsWith("/") == false)
                {
                    parsedUriString = "/" + parsedUriString;
                }
                string concatenatedUri = protocol + main_domain + parsedUriString;
                Boolean success2 = Uri.TryCreate(concatenatedUri, UriKind.Absolute, out absoluteUri);
                if (success2 == false)
                {
                    return new Tuple<Uri, string>(null, String.Format("Невозможно корректно распарсить строку '{0}' как корректный абсолютный URI", 
                        concatenatedUri));
                }
                return new Tuple<Uri, string>(absoluteUri, null);
            }
        }

        /// <summary>
        /// Возвращает текстовые данные, вычленяя их из части HTML-кода
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static String ExtractFromHTML(String Input)
        {
            if(Input.HasAlphaNumericChars()==false) 
            {throw new ArgumentException("Входная строка не содержит цифробуквенных символов", "Input");}
            String temp = Input.MultiReplace(' ', new char[]{'\r', '\n', '\t'}).Trim();
            temp = KlotosLib.HTML.HtmlTools.IntelliRemoveHTMLTags(temp);
            temp = KlotosLib.StringTools.SubstringHelpers.ShrinkSpaces(temp).Trim();
            temp = HttpUtility.HtmlDecode(temp);
            return temp;
        }

        internal static AlbumHeader ParseAlbumHeader(HtmlDocument htmlPage)
        {
            if (htmlPage == null) { throw new ArgumentNullException("htmlPage", "Входной документ не может быть NULL"); }
            
            String caption = CoreInternal.TryGrabCaption(htmlPage);
            
            const string tableMetadataXpath = 
                "//body/div[@class='all']/div[@class='main']/div[@class='content']/div[@id='bodyContent' and @class='inner']/div[@class='main-details']/div[@class='cont']/div[@class='tbl']/table/tr";
            HtmlNodeCollection metadataRows = htmlPage.DocumentNode.SelectNodes(tableMetadataXpath);

            AlbumHeader output = new AlbumHeader();
            
            for (Int32 rowIndex = 0; rowIndex < metadataRows.Count; rowIndex++)
            {
                HtmlNode oneRow = metadataRows[rowIndex];
                HtmlNode defCell = oneRow.ChildNodes.First(c => c.Name.Equals("td", StringComparison.OrdinalIgnoreCase));
                HtmlNode valCell = oneRow.ChildNodes.Last(c => c.Name.Equals("td", StringComparison.OrdinalIgnoreCase));
                if (!Object.ReferenceEquals(defCell, valCell))
                {
                    output.Accept(defCell.InnerText.Trim(), KlotosLib.StringTools.SubstringHelpers.ShrinkSpaces(valCell.InnerText.Trim().CleanString()));
                }
                else if (defCell.GetAttributeValue("colspan", "") == "2")
                {
                    HtmlNode descrNode = defCell.Descendants("div").Single(d => d.GetAttributeValue("id", "") == "inner_desc");
                    output.Description = HttpUtility.HtmlDecode(KlotosLib.StringTools.SubstringHelpers.ShrinkSpaces(descrNode.InnerText.Trim().CleanString()));
                }
                else
                {
                    throw new InvalidOperationException("Обнаружена неизвестная структура в таблице с метаинформацией по альбому");
                }
            }

            if (caption.StartsWith(output.Artist, StringComparison.OrdinalIgnoreCase) == true)
            {
                caption = caption.TrimStart(output.Artist, StringComparison.OrdinalIgnoreCase, false);
                caption = caption.TrimStart(new char[2] { ' ', '-' });
            }
            output.Title = caption;

            const string additionalMetadataXpath = "//body/div[@class='all']/div[@class='main']/div[@class='content']/div[@id='bodyContent' and @class='inner']/div[@class='main-details']/div[@class='cont']/div[@class='labels']/div[@class='item']";
            HtmlNodeCollection additionalMetadataNodes = htmlPage.DocumentNode.SelectNodes(additionalMetadataXpath);
            output.Format = additionalMetadataNodes[0].InnerText.Trim();
            HtmlNode numTracks = additionalMetadataNodes[1].Descendants("meta").Single(m => m.GetAttributeValue("itemprop", "") == "numTracks");
            output.SongsCount = Byte.Parse(numTracks.GetAttributeValue("content", "0"));
            output.AlbumImageURI = CoreInternal.TryGrabImageUri(htmlPage);
            output.AlbumPageURI = CoreInternal.ExtractAlbumUri(htmlPage);

            return output;
        }

        /// <summary>
        /// Парсит все песни на странице альбома и возвращает результат в виде списка моделей песен
        /// </summary>
        /// <param name="InputHTML">HTML-код страницы альбома в виде подготовленного HTML-документа</param>
        /// <param name="AHeader">Распарсенный хидер альбома, информацию о песнях которого следует извлечь</param>
        /// <returns></returns>
        internal static List<OneSongHeader> ParseAllSongsInAlbum(HtmlDocument InputHTML, AlbumHeader AHeader)
        {
            if(InputHTML==null) {throw new ArgumentNullException("InputHTML");}
            if(AHeader == null) {throw new ArgumentNullException("AHeader");}

            List<OneSongHeader> output = new List<OneSongHeader>();

            const String tracklistXpath =
                "//body/div[@class='all']/div[@class='main']/div[@class='content']/div[@id='bodyContent' and @class='inner']"+
                "/div[starts-with(@id, 'playerDiv') and normalize-space(@class)='player-inline' and @itemprop='tracks' and @itemscope='itemscope' and @itemtype]";
            HtmlNodeCollection tracklistNodes = InputHTML.DocumentNode.SelectNodes(tracklistXpath);
            if (tracklistNodes.IsNullOrEmpty())
            {
                throw new InvalidOperationException("Невозможно извлечь блок HTML-кода с метаинформацией по треклисту альбома");
            }
            for (Int32 divIndex = 0; divIndex < tracklistNodes.Count; divIndex++)
            {
                HtmlNode oneTrack = tracklistNodes[divIndex];
                
                Byte number = Byte.Parse(oneTrack.ChildNodes.Single(
                    child => child.Name == "div" && child.GetAttributeValue("class", "").Equals("position", StrComp)).InnerText);

                HtmlNode detailsNode = oneTrack.ChildNodes.Single(
                    child => child.Name == "div" && child.GetAttributeValue("class", "").Equals("details", StrComp));
                String size = detailsNode.ChildNodes.Single(child => child.Name == "div" && child.GetAttributeValue("class", "").Equals("time", StrComp))
                    .InnerText.Trim();
                String title = detailsNode.ChildNodes.Single(child => child.Name == "p" && child.HasAttributes == false)
                        .ChildNodes.Single(subchild => subchild.Name == "a" && subchild.Attributes.Contains("href"))
                    .InnerText.Trim();
                title = HttpUtility.HtmlDecode(title);

                Predicate<HtmlNode> isValidArtistNode = delegate(HtmlNode node)
                {
                    return node.Name == "a" && node.GetAttributeValue("class", "").Equals("strong", StrComp) && node.Attributes.Contains("href");
                };
                String artist = "";
                HtmlNode firstArtistNode = detailsNode.ChildNodes.FirstOrDefault(child =>
                        child.Name == "a" && child.GetAttributeValue("class", "").Equals("strong", StrComp) && child.Attributes.Contains("href"));
                if (firstArtistNode == null)
                {
                    throw new InvalidOperationException("Невозможно извлечь имя исполнителя");
                }
                artist += HttpUtility.HtmlDecode(firstArtistNode.InnerText);

                HtmlNode nextIterable = firstArtistNode.NextSibling;
                while (nextIterable != null && (isValidArtistNode.Invoke(nextIterable) == true || nextIterable.NodeType == HtmlNodeType.Text))
                {
                    artist += HttpUtility.HtmlDecode(nextIterable.InnerText);
                    nextIterable = nextIterable.NextSibling;
                }
                artist = KlotosLib.StringTools.SubstringHelpers.ShrinkSpaces(artist).CleanString().Trim();

                HtmlNode optionsNode = oneTrack.ChildNodes.Single(
                    child => child.Name == "div" && child.GetAttributeValue("class", "").Equals("options", StrComp));
                String[] rawDataAndBitrate = optionsNode.ChildNodes.Single(
                    child => child.Name == "div" && child.GetAttributeValue("class", "").Equals("data", StrComp))
                    .InnerText.Split('|');
                if (rawDataAndBitrate.Length != 2)
                {
                    throw new InvalidOperationException("Невозможно корректно извлечь значение продолжительности и битрейта трека");
                }
                String duration = rawDataAndBitrate[0].Trim();
                String bitrate = rawDataAndBitrate[1].Trim();

                String rawUri = optionsNode.ChildNodes.Single(child => child.Name == "div" && child.GetAttributeValue("class", "").Equals("top", StrComp))
                    .ChildNodes.Single(
                        subchild => subchild.Name == "a" && subchild.Attributes.Contains("href") 
                        && subchild.Attributes.Contains("title") && subchild.InnerText.Equals("Скачать", StrComp)
                    ).Attributes["href"].Value;
                Tuple<Uri, String> fixedUri = TryFixReturnURI(rawUri);
                if (fixedUri.Item1 == null)
                {
                    throw new InvalidOperationException(String.Format(
                        "Невозможно извлечь из страницы альбома URI на страницу песни, полученную из строки '{0}': {1}",
                        rawUri, fixedUri.Item2));
                }

                output.Add(
                    new OneSongHeader(number, title, title, artist, AHeader.Title, AHeader.Genre,
                        duration, size, bitrate, AHeader.Format, AHeader.Uploader, null, fixedUri.Item1, true)//TODO: availability
                    );
            }
            return output;
        }
        
        /// <summary>
        /// Извлекает и возвращает всю информацию по одной песне из страницы песни
        /// </summary>
        /// <param name="htmlPage"></param>
        /// <returns></returns>
        internal static OneSongHeader ParseOneSongHeader(HtmlDocument htmlPage)
        {
            const string metadataTableElementXpath = 
                "//body/div[@class='all']/div[@class='main']/div[@class='content']/div[@id='bodyContent' and @class='inner']" +
                "/div[@itemprop='tracks' and @itemscope and @itemtype]/div[@class='main-details']/div[@class='cont']/div[@class='tbl']/table/tr";
            HtmlNodeCollection metadataRows = htmlPage.DocumentNode.SelectNodes(metadataTableElementXpath);
            if (metadataRows.IsNullOrEmpty() == true)
            {
                throw new InvalidOperationException("Невозможно извлечь HTML-таблицу с метаинформацией по песне");
            }

            OneSongHeader outputHeader = new OneSongHeader();
            for (Int32 rowIndex = 0; rowIndex < metadataRows.Count; rowIndex++)
            {
                HtmlNode oneRow = metadataRows[rowIndex];
                HtmlNode defCell = oneRow.ChildNodes.First(c => c.Name.Equals("td", StringComparison.OrdinalIgnoreCase));
                HtmlNode valCell = oneRow.ChildNodes.Last(c => c.Name.Equals("td", StringComparison.OrdinalIgnoreCase));
                if (!Object.ReferenceEquals(defCell, valCell))
                {
                    outputHeader.Accept(defCell.InnerText.Trim(), 
                        KlotosLib.StringTools.SubstringHelpers.ShrinkSpaces(valCell.InnerText.Trim().CleanString()), 
                        valCell.InnerHtml.Trim());
                }
                else if (Object.ReferenceEquals(defCell, valCell) && defCell.GetAttributeValue("colspan", "") == "2")
                {
                    continue;
                }
                else
                {
                    throw new InvalidOperationException("Обнаружена неизвестная структура в таблице с метаинформацией по песне");
                }
            }

            const string nameSpanXpath = "//body/div[@class='all']/div[@class='main']/div[@class='content']/div[@id='bodyContent' and @class='inner']"+
                "/div[@class='breadcrumbs']/span[@itemscope and @itemtype and @itemprop='title']";
            HtmlNode nameSpanNode = htmlPage.DocumentNode.SelectSingleNode(nameSpanXpath);
            if (nameSpanNode == null)
            {
                throw new InvalidOperationException("Невозможно найти название песни");
            }

            outputHeader.Number = 1;
            outputHeader.Name = HttpUtility.HtmlDecode(nameSpanNode.InnerText.CleanString().Trim());
            
            outputHeader.Title = CoreInternal.TryGrabCaption(htmlPage);
            outputHeader.SongImageURI = CoreInternal.TryGrabImageUri(htmlPage);

            const string labelsDivXpath =
                "//body/div[@class='all']/div[@class='main']/div[@class='content']/div[@id='bodyContent' and @class='inner']" +
                "/div[@itemprop='tracks' and @itemscope and @itemtype]/div[@class='main-details']/div[@class='cont']/div[@class='labels']";
            HtmlNode labelDiv = htmlPage.DocumentNode.SelectSingleNode(labelsDivXpath);
            if (labelDiv == null)
            {
                throw new InvalidOperationException("Невозможно найти блок данных с информацией о битрейте и формате песни");
            }
            List<HtmlNode> subNodes = labelDiv.ChildNodes.Where(
                childNode => childNode.Name == "div" && childNode.GetAttributeValue("class", "").Equals("item", StrComp)).ToList();
            if (subNodes.IsNullOrEmpty() || subNodes.Count != 2)
            {
                throw new InvalidOperationException("Невозможно извлечь значения битрейта и формата песни");
            }
            outputHeader.Format = subNodes[0].InnerText.Trim();
            outputHeader.Bitrate = subNodes[1].InnerText.Trim();

            HtmlNode metaSongUriNode = labelDiv.ChildNodes.SingleOrDefault(metaChild => 
                metaChild.Name == "meta" && metaChild.GetAttributeValue("itemprop", "").Equals("url", StrComp) && metaChild.Attributes.Contains("content"));
            if (metaSongUriNode == null)
            {
                throw new InvalidOperationException("Невозможно извлечь ссылку на страницу песни");
            }
            String rawUri = metaSongUriNode.Attributes["content"].Value.Trim();
            Tuple<Uri, String> fixedUri = TryFixReturnURI(rawUri);
            if (fixedUri.Item1 == null)
            {
                throw new InvalidOperationException("Невозможно извлечь URI текущей страницы песни: " + fixedUri.Item2);
            }
            outputHeader.SongPageURI = fixedUri.Item1;

            outputHeader.IsAvailableForDownload = true; //TODO: detect missed files (need sample)
            return outputHeader;
        }
        
        /// <summary>
        /// Пытается скачать и вернуть один файл по указанной ссылке с указанными параметрами. 
        /// В случае ошибки возвращает NULL и сообщение об ошибке в ввыводном параметре.
        /// </summary>
        /// <param name="DownloadURI">Ссылка на скачивание файла</param>
        /// <param name="Referer">Referer - ссылка на страницу, с которой якобы осуществлён запрос на скачивание файла</param>
        /// <param name="UserAgent">UserAgent, который будет использоваться при выполнении запроса</param>
        /// <param name="ErrorMessage">выводной параметр, содержащий сообщение об ошибке, если она произошла, 
        /// или NULL в случае успеха</param>
        /// <returns></returns>
        internal static DownloadedFile TryDownloadFile(Uri DownloadURI, Uri Referer, String UserAgent, out String ErrorMessage)
        {
            if (DownloadURI == null) { throw new ArgumentNullException("DownloadURI", "DownloadURI не может быть NULL"); }
            if (Referer == null) { throw new ArgumentNullException("Referer", "Referer не может быть NULL"); }
            if (UserAgent == null) { throw new ArgumentNullException("UserAgent", "UserAgent не может быть NULL"); }

            ErrorMessage = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(DownloadURI);
            request.Method = "GET";
            request.UserAgent = UserAgent;
            request.Referer = Referer.OriginalString;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        ErrorMessage = "Код ответа сервера не является 200 OK: " + response.StatusCode.ToString();
                        return null;
                    }
                    
                    String content_disposition = response.Headers["Content-Disposition"];
                    String content_type = response.Headers["Content-Type"];
                    String raw_content_length = response.Headers["Content-Length"];
                    Int32 content_length = raw_content_length.TryParseNumber<Int32>(NumberStyles.Integer, null, -10);
                    if (content_type.HasAlphaNumericChars() == false
                        || raw_content_length.HasAlphaNumericChars() == false
                        || raw_content_length.Equals("0", StringComparison.InvariantCultureIgnoreCase) == true)
                    {
                        ErrorMessage = String.Format("Значения некоторых хидеров некорректны. "+
                            "Content-Disposition: {0}; Content-Type: {1}; Content-Length: {2}.",
                            content_disposition.ToStringS("NULL"), content_type.ToStringS("NULL"), raw_content_length.ToStringS("NULL"));
                        return null;
                    }
                    if (content_length == -10)
                    {
                        ErrorMessage = "Значение HTTP-хидера Content-Length некорректно и является : " +
                                       raw_content_length;
                        return null;
                    }

                    MemoryStream file_body = new MemoryStream(content_length);
                    using (Stream receiveStream = response.GetResponseStream())
                    {
                        StreamTools.CopyStream(receiveStream, file_body, false, false);
                    }

                    if (content_disposition.HasAlphaNumericChars() == false)
                    {
                        return new DownloadedFile("", content_length, file_body);
                    }
                    else
                    {
                        if (content_disposition.Contains("attachment", StringComparison.OrdinalIgnoreCase) == false &&
                        content_disposition.Contains("filename", StringComparison.OrdinalIgnoreCase) == false)
                        {
                            ErrorMessage =
                                "Значение HTTP-хидера Content-Disposition некорректно и является : " + content_disposition;
                            return null;
                        }
                        String original_filename_without_ext = KlotosLib.StringTools.SubstringHelpers.GetSubstringToToken(content_disposition,
                            "filename=", false, KlotosLib.StringTools.Direction.FromEndToStart, StringComparison.OrdinalIgnoreCase).Value;
                        Int32 out_pos;
                        String ext = KlotosLib.StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                            (response.ResponseUri.ToString(), "ex=", "&", 0, 0, false, Direction.FromStartToEnd, StringComparison.OrdinalIgnoreCase).Value;
                        String original_filename_full = ext.HasAlphaNumericChars() == false
                            || original_filename_without_ext.EndsWith(ext, StringComparison.OrdinalIgnoreCase) == true
                            ? original_filename_without_ext
                            : original_filename_without_ext + ext;

                        DownloadedFile output = new DownloadedFile(original_filename_full, content_length, file_body);
                        return output;
                    }
                }
            }
            catch (WebException wex)
            {
                ErrorMessage = String.Format(
                    "Ошибка при попытке скачать файл. Статус ответа сервера: {0}. Сообщение об ошибке: {1}.",
                    wex.Status.ToString(), wex.TotalMessage());
                return null;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Прозошло неизвестное исключение: " + ex.TotalMessage();
                return null;
            }
        }

        /// <summary>
        /// Пытается асинхронно, не блокируя текущий поток, скачать и вернуть один файл по указанной ссылке 
        /// с указанными параметрами. В случае ошибки возвращает NULL.
        /// </summary>
        /// <param name="DownloadURI">Ссылка на скачивание файла</param>
        /// <param name="Referer">Referer - ссылка на страницу, с которой якобы осуществлён запрос на скачивание файла</param>
        /// <param name="UserAgent">UserAgent, который будет использоваться при выполнении запроса</param>
        /// <returns></returns>
        internal static async Task<DownloadedFile> TryDownloadFileAsync(Uri DownloadURI, Uri Referer, String UserAgent)
        {
            if (DownloadURI == null) { throw new ArgumentNullException("DownloadURI", "DownloadURI не может быть NULL"); }
            if (Referer == null) { throw new ArgumentNullException("Referer", "Referer не может быть NULL"); }
            if (UserAgent == null) { throw new ArgumentNullException("UserAgent", "UserAgent не может быть NULL"); }
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(DownloadURI);
            request.Method = "GET";
            request.UserAgent = UserAgent;
            request.Referer = Referer.ToString();

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return null;
                    }
                    String content_disposition = response.Headers["Content-Disposition"];
                    String content_type = response.Headers["Content-Type"];
                    String raw_content_length = response.Headers["Content-Length"];
                    Int32 content_length = raw_content_length.TryParseNumber<Int32>(NumberStyles.Integer, null, -10);
                    if (content_disposition.HasAlphaNumericChars() == false
                        || content_type.HasAlphaNumericChars() == false
                        || raw_content_length.HasAlphaNumericChars() == false
                        || raw_content_length.Equals("0", StringComparison.InvariantCultureIgnoreCase) == true)
                    {
                        return null;
                    }
                    if (content_length == -10)
                    {
                        return null;
                    }
                    ////application/mp3
                    //if (content_type.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase) == false)
                    //{
                    //    return null;
                    //}
                    if (content_disposition.Contains("attachment", StringComparison.OrdinalIgnoreCase) == false ||
                        content_disposition.Contains("filename", StringComparison.OrdinalIgnoreCase) == false)
                    {
                        return null;
                    }
                    String original_filename_without_ext = KlotosLib.StringTools.SubstringHelpers.GetSubstringToToken(content_disposition,
                        "filename=", false, KlotosLib.StringTools.Direction.FromEndToStart, StringComparison.OrdinalIgnoreCase).Value;
                    Substring extracted_ext = KlotosLib.StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                        (response.ResponseUri.ToString(), "ex=", "&", 0, 0, false, Direction.FromStartToEnd,
                            StringComparison.OrdinalIgnoreCase);
                    String ext = extracted_ext == null ? null :extracted_ext.Value;
                    String original_filename_full = ext.HasAlphaNumericChars() == false
                        || original_filename_without_ext.EndsWith(ext, StringComparison.OrdinalIgnoreCase) == true
                        ? original_filename_without_ext
                        : original_filename_without_ext + ext;
                    MemoryStream file_body = new MemoryStream(content_length);
                    using (Stream receiveStream = response.GetResponseStream())
                    {
                        if (receiveStream == null)
                        {
                            return null;
                        }
                        await receiveStream.CopyToAsync(file_body);
                    }
                    DownloadedFile output = new DownloadedFile(original_filename_full, content_length, file_body);
                    return output;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Пытатся преобразовать указанный файл <paramref name="ReceivedFile"/> в битовую карту и в случае успеха вернуть её
        /// </summary>
        /// <param name="ReceivedFile"></param>
        /// <returns></returns>
        internal static Bitmap TryConvertFileToImage(DownloadedFile ReceivedFile)
        {
            if (ReceivedFile == null) { throw new ArgumentNullException("ReceivedFile", "ReceivedFile не может быть NULL"); }

            Bitmap output;
            try
            {
                output = new Bitmap(ReceivedFile.FileBody);

            }
            catch
            {
                output = null;
            }
            return output;
        }
    }
}
