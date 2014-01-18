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

namespace MyzukaRuGrabberCore
{
    internal static class CoreInternal
    {
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
        /// <param name="HTMLPage"></param>
        /// <returns></returns>
        internal static ParsedItemType DetectItemType(HtmlDocument HTMLPage)
        {
            if(HTMLPage == null) {throw new ArgumentNullException("HTMLPage");}
            Byte album_symptom = 0;
            Byte song_symptom = 0;


            HtmlNode node1 = HTMLPage.DocumentNode.SelectSingleNode
                ("//div[@class='centerblock gr']/div[starts-with(@class, 'in2')]//h1[@class = 'blue' or @class = 'green']");
            if (node1 == null) { return ParsedItemType.Unknown; }
            String node1_outerhtml = node1.OuterHtml;
            if (node1_outerhtml.Contains("blue", StringComparison.OrdinalIgnoreCase) == true)
            {
                song_symptom++;
            }
            else if (node1_outerhtml.Contains("green", StringComparison.OrdinalIgnoreCase) == true)
            {
                album_symptom++;
            }
            else
            {
                return ParsedItemType.Unknown;
            }

            HtmlNode node2 = HTMLPage.DocumentNode.SelectSingleNode
                ("//div[@class='centerblock gr']/div[starts-with(@class, 'in2')]//table[@style and @class]/tr");
            if (node2 == null) { return ParsedItemType.Unknown; }
            
            String node2_text_1st_part = StringTools.SubstringHelpers.GetSubstringToToken
                (node2.InnerText, "Описание:", true, StringTools.Direction.FromStartToEnd, StringComparison.OrdinalIgnoreCase);
            
            if (node2_text_1st_part.Contains("Альбом:", StringComparison.OrdinalIgnoreCase) == true)
            {
                song_symptom++;
            }
            if (node2_text_1st_part.Contains("Размер:", StringComparison.OrdinalIgnoreCase) == true)
            {
                song_symptom++;
            }
            if (node2_text_1st_part.Contains("Длительность:", StringComparison.OrdinalIgnoreCase) == true)
            {
                song_symptom++;
            }
            
            if (node2_text_1st_part.Contains("Кол-во песен:", StringComparison.OrdinalIgnoreCase) == true)
            {
                album_symptom++;
            }
            if (node2_text_1st_part.Contains("Дата релиза:", StringComparison.OrdinalIgnoreCase) == true)
            {
                album_symptom++;
            }
            if (node2_text_1st_part.Contains("Описание:", StringComparison.OrdinalIgnoreCase) == true)
            {
                album_symptom++;
            }
            String node2_text_2nd_part = StringTools.SubstringHelpers.GetSubstringToToken
                (node2.InnerText, "Описание:", true, StringTools.Direction.FromEndToStart, StringComparison.OrdinalIgnoreCase);
            if (StringTools.ContainsHelpers.ContainsAllOf
                (node2_text_2nd_part, StringComparison.OrdinalIgnoreCase, "скачать альбом") == true)
            {
                album_symptom++;
            }
            else
            {
                song_symptom++;
            }

            if (album_symptom == 5 && song_symptom == 0)
            { return ParsedItemType.Album; }

            if (song_symptom == 5 && album_symptom == 0)
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
            HtmlNode node = HTMLPage.DocumentNode.SelectSingleNode(
                "//div[@class='centerblock gr']/div[starts-with(@class, 'in2')]//table[@style and @class]/tr[1]/td[1]/img[@src]");
            if (node == null) { return null; }
            String raw_image_URI = node.GetAttributeValue("src", "");
            if (raw_image_URI.HasAlphaNumericChars() == false) {return null;}
            String err_mes;
            Uri output = CoreInternal.TryFixReturnURI(raw_image_URI, out err_mes);
            return output;
        }

        /// <summary>
        /// Возвращает URI альбома, HTML-код страницы которого подан на вход
        /// </summary>
        /// <param name="HTMLPage"></param>
        /// <returns></returns>
        private static Uri ExtractAlbumUri(HtmlDocument HTMLPage)
        {
            String node = HTMLPage.DocumentNode.SelectSingleNode
                ("//div[@style]/form[@action and @method='post']").OuterHtml;
            Int32 out_pos;
            String raw_URI = StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                (node, "Login?returnUrl=", "method", 0, StringComparison.OrdinalIgnoreCase, out out_pos);
            String err_mes;
            Uri output = CoreInternal.TryFixReturnURI(raw_URI, out err_mes);
            if (output == null)
            {
                throw new InvalidOperationException("Невозможно извлечь URI альбома: " + err_mes);
            }
            return output;
        }

        /// <summary>
        /// Возвращает название элемента как для песни, так и для альбома. При неудаче возвращает NULL.
        /// </summary>
        /// <param name="InputHTML"></param>
        /// <returns></returns>
        private static String TryGrabCaption(HtmlDocument InputHTML)
        {
            HtmlNode caption_node = InputHTML.DocumentNode.SelectSingleNode
                ("//div[@class='centerblock gr']/div[starts-with(@class, 'in2')]//h1[@class = 'blue' or @class = 'green']");
            if (caption_node == null) { return null; }
            return HttpUtility.HtmlDecode(caption_node.InnerText.Trim());
        }

        /// <summary>
        /// Извлекает из страницы песни и возвращает URI на скачку файла песни
        /// </summary>
        /// <param name="HTMLPage"></param>
        /// <returns></returns>
        internal static Uri ExtractDownloadSongURI(HtmlDocument HTMLPage)
        {
            HtmlNode link_node = HTMLPage.DocumentNode.SelectSingleNode
                ("//div[@id]/table[@width='100%']/tr[starts-with(@id,'trSong_')]/td[4][@class='downloadSong']/a[@href]");
            if(link_node==null) {throw new InvalidOperationException("Невозможно извлечь из страницы песни блок HTML-кода со ссылкой на скачку песни");}
            String raw_link_URI = link_node.GetAttributeValue("href", "");
            String err_mes;
            Uri link_URI = CoreInternal.TryFixReturnURI(raw_link_URI, out err_mes);
            if (link_URI == null)
            {
                throw new InvalidOperationException("Невозможно извлечь URI текущей страницы песни: " + err_mes);
            }
            return link_URI;
        }

        /// <summary>
        /// Извлекает из страницы песни и возвращает URI на страницу альбома, к которому относится данная песня
        /// </summary>
        /// <param name="HTMLPage"></param>
        /// <returns></returns>
        internal static Uri ExtractAlbumURIFromSongPage(HtmlDocument HTMLPage)
        {
            if (HTMLPage == null) { throw new ArgumentNullException("HTMLPage"); }

            String err_mes;
            HtmlNode song_header_node = HTMLPage.DocumentNode.SelectSingleNode(
                "//div[@class='centerblock gr']/div[starts-with(@class, 'in2')]/div/table[@style and @class]/tr/td[2][@class='infoSong']");
            if(song_header_node==null) 
            {throw new InvalidOperationException("Невозможно извлечь из страницы песни блок HTML-кода с метаинформацией по песне");}

            Int32 pos;
            String raw_album_data = StringTools.SubstringHelpers.GetInnerStringBetweenTokens(song_header_node.InnerHtml,
                "Альбом:", "Длительность:", 0, StringComparison.OrdinalIgnoreCase, out pos);
            if (raw_album_data == null) 
            {throw new InvalidOperationException("Невозможно извлечь из страницы песни блок HTML-кода с данными по альбому, к которому принадлежит эта песня");}
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml("<root>"+raw_album_data+"</root>");
            HtmlNode link_node = doc.DocumentNode.SelectSingleNode("/root/a[@href and @class]");
            String raw_album_URI = link_node.GetAttributeValue("href", "");

            Uri album_URI = TryFixReturnURI(raw_album_URI, out err_mes);
            if (album_URI == null)
            {
                throw new InvalidOperationException(String.Format(
                  "Невозможно извлечь URI на страницу альбома из строки '{0}': {1}",
                  raw_album_URI, err_mes));
            }
            return album_URI;
        }

        /// <summary>
        /// Пытается очистить от шелухи и возвратить URI
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        internal static Uri TryFixReturnURI(String Input, out String ErrorMessage)
        {
            ErrorMessage = null;
            if (Input.HasAlphaNumericChars() == false)
            {
                ErrorMessage = "URI = '" + Input.ToStringS("NULL") + "' заведомо некорректен";
                return null;
            }
            const String main_domain = "http://myzuka.ru";
            const String download_domain = "http://fp";
            String temp_URI = Input.Trim(new char[2] { ' ', '"' });
            if (temp_URI.StartsWith(main_domain, StringComparison.OrdinalIgnoreCase) == false &&
                temp_URI.StartsWith(download_domain, StringComparison.OrdinalIgnoreCase) == false)
            {
                temp_URI = main_domain + temp_URI;
            }
            temp_URI = HttpTools.DecodeUri(temp_URI);
            Uri link_URI;
            Boolean res = Uri.TryCreate(temp_URI, UriKind.Absolute, out link_URI);
            if (res == false)
            {
                ErrorMessage = String.Format(
                  "Невозможно корректно распарсить как абсолютный URI подстроку '{0}', полученную из строки '{1}'",
                  temp_URI, Input);
                return null;
            }
            return link_URI;
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
            temp = KlotosLib.HtmlTools.IntelliRemoveHTMLTags(temp);
            temp = StringTools.SubstringHelpers.ShrinkSpaces(temp).Trim();
            temp = HttpUtility.HtmlDecode(temp);
            return temp;
        }

        internal static AlbumHeader ParseAlbumHeader(HtmlDocument HTMLPage)
        {
            if (HTMLPage == null) { throw new ArgumentNullException("HTMLPage", "Входной документ не может быть NULL"); }
            
            String caption = CoreInternal.TryGrabCaption(HTMLPage);

            HtmlNode main_body = HTMLPage.DocumentNode.SelectSingleNode(
                "//div[@class='centerblock gr']/div[starts-with(@class, 'in2')]/table[1]/tr/td[2]"
                );
            if(main_body==null) {throw new InvalidOperationException("Невозможно извлечь блок HTML-кода с метаинформацией по альбому");}

            HtmlNodeCollection uploader_and_updater = main_body.SelectNodes("//div[@class='loaderSong']/a[@class and @href]");
            if (uploader_and_updater.IsNullOrEmpty()==true) 
            { throw new InvalidOperationException("Невозможно извлечь блок HTML-кода с данными о пользователях, загрузивших и обновивших альбом"); }

            String uploader = uploader_and_updater[0].InnerText.Trim();
            String updater = "";
            if (uploader_and_updater.Count == 2)
            {
                updater = uploader_and_updater[1].InnerText.Trim();
            }
            
            String main_body_html = main_body.InnerHtml;
            Int32 pos_input;

            String genre = StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                (main_body_html, "Жанр:", "Исполнитель", 0, StringComparison.OrdinalIgnoreCase, out pos_input);
            if (genre.Contains("Нет данных", StringComparison.OrdinalIgnoreCase) == true)
            {
                genre = "Нет данных";
            }
            else
            {
                genre = HtmlTools.IntelliRemoveHTMLTags(genre).CleanString().Trim();
                genre = StringTools.SubstringHelpers.ShrinkSpaces(genre);
            }
            
            String artist = StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                (main_body_html, "Исполнитель:", "<br>", pos_input, StringComparison.OrdinalIgnoreCase, out pos_input).Trim();
            artist = HtmlTools.IntelliRemoveHTMLTags(artist).CleanString().Trim();
            artist = StringTools.SubstringHelpers.ShrinkSpaces(artist);

            String release_date = StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                (main_body_html, "Дата релиза:", "<br>", pos_input, StringComparison.OrdinalIgnoreCase, out pos_input);

            String type = StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                (main_body_html, "Тип:", "<br>", pos_input, StringComparison.OrdinalIgnoreCase, out pos_input).Trim();

            String count = StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                (main_body_html, "Кол-во песен:", "<br>", pos_input, StringComparison.OrdinalIgnoreCase, out pos_input).Trim();
            Byte count_parsed = Byte.Parse(count);

            String format = StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                (main_body_html, "Формат:", "<br>", pos_input, StringComparison.OrdinalIgnoreCase, out pos_input).Trim();
            
            String description = StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                (main_body_html, "Описание: <br>", "GetVipAccount", pos_input, StringComparison.OrdinalIgnoreCase, out pos_input);
            description = CoreInternal.ExtractFromHTML(description);

            if (caption.StartsWith(artist, StringComparison.OrdinalIgnoreCase) == true)
            {
                caption = caption.TrimStart(artist, StringComparison.OrdinalIgnoreCase, false);
                caption = caption.TrimStart(new char[2] { ' ', '-' });
            }

            AlbumHeader output = new AlbumHeader
                (caption, genre, artist, release_date, type, count_parsed, format, uploader, updater, description,
                CoreInternal.TryGrabImageUri(HTMLPage), CoreInternal.ExtractAlbumUri(HTMLPage));
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

            HtmlNodeCollection raw_songs_list = InputHTML.DocumentNode.SelectNodes(
                "//table[normalize-space(@width)='100%' and normalize-space(@class)='rectable rectable_center']" +
                "/tr[starts-with(@id,'trSong_')]");
            if (raw_songs_list == null) {throw new InvalidOperationException("Невозможно извлечь блок HTML-кода с метаинформацией по всем песням в альбоме");}

            foreach (HtmlNode one_raw_song in raw_songs_list)
            {
                String inner_block = "<root>" + one_raw_song.InnerHtml + "</root>";
                HtmlDocument inner_doc = new HtmlDocument();
                inner_doc.LoadHtml(inner_block);
                String number = inner_doc.DocumentNode.SelectSingleNode(
                    "/root/td[2]").InnerHtml.CleanString().Trim();

                String artist = inner_doc.DocumentNode.SelectSingleNode(
                    "/root/td[3]").InnerHtml;
                artist = ExtractFromHTML(artist);
                
                HtmlNode raw_title_node = inner_doc.DocumentNode.SelectSingleNode("/root/td[5][@class]/div");
                if(raw_title_node==null)
                { throw new InvalidOperationException("Невозможно извлечь блок HTML-кода с названием песни, которая имеет номер "+number); }
                String raw_title_text = raw_title_node.InnerText.CleanString();
                String title = HttpUtility.HtmlDecode(raw_title_text.TrimEnd("Файл утерян", StringComparison.OrdinalIgnoreCase, false));

                Boolean is_available = !raw_title_text.Contains("Файл утерян", StringComparison.InvariantCultureIgnoreCase);

                String duration = inner_doc.DocumentNode.SelectSingleNode(
                    "/root/td[6]").InnerHtml.CleanString().Trim();

                String size = inner_doc.DocumentNode.SelectSingleNode(
                    "/root/td[7]").InnerHtml.CleanString().Trim();

                String bitrate = inner_doc.DocumentNode.SelectSingleNode(
                    "/root/td[8]").InnerHtml.CleanString().Trim();
                
                HtmlNode raw_URI_node = inner_doc.DocumentNode.SelectSingleNode(
                    "/root/td[@class='downloadSong']/a[@href and @title]");
                if (raw_URI_node == null)
                { throw new InvalidOperationException(String.Format("Невозможно извлечь из страницы альбома блок HTML-кода со ссылкой на страницу песни '{0}-{1}'", 
                    number, title)); }

                String raw_URI = raw_URI_node.GetAttributeValue("href", "");
                if(raw_URI.IsStringNullOrEmpty()==true) { throw new InvalidOperationException(String.Format(
                    "Невозможно извлечь ссылку на страницу песни '{0}-{1}' из HTML-кода '{2}'", number, title, raw_URI_node.OuterHtml)); }
                String err_mes;
                Uri song_link = TryFixReturnURI(raw_URI, out err_mes);
                if (song_link == null)
                {
                    throw new InvalidOperationException(String.Format(
                        "Невозможно извлечь из страницы альбома URI на страницу песни, полученную из строки '{0}': {1}",
                        raw_URI, err_mes));
                }
                output.Add(
                    new OneSongHeader(Byte.Parse(number), title, title, artist, AHeader.Title, AHeader.Genre,
                        duration, size, bitrate, AHeader.Format, AHeader.Uploader, null, song_link, is_available)
                    );
            }
            return output;
        }
        
        /// <summary>
        /// Извлекает и возвращает всю информацию по одной песне из страницы песни
        /// </summary>
        /// <param name="HTMLPage"></param>
        /// <returns></returns>
        internal static OneSongHeader ParseOneSongHeader(HtmlDocument HTMLPage)
        {
            String caption = CoreInternal.TryGrabCaption(HTMLPage);

            HtmlNode body_node = HTMLPage.DocumentNode.SelectSingleNode(
                "//div[@class='centerblock gr']/div[starts-with(@class, 'in2')]/div/table[@style and @class]/tr/td[2][@class='infoSong']");
            if (body_node == null) {throw new InvalidOperationException("Невозможно извлечь блок HTML-кода с метаинформацией по песне");}
            String body = body_node.InnerHtml;

            Int32 pos_input;

            String genre = StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                (body, "Жанр:", "Исполнитель", 0, StringComparison.OrdinalIgnoreCase, out pos_input);
            if (genre.Contains("Нет данных", StringComparison.OrdinalIgnoreCase) == true)
            {
                genre = "Нет данных";
            }
            else
            {
                genre = StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                    (genre, ">", "</a>", 0, StringComparison.OrdinalIgnoreCase, out pos_input);
                genre = HttpUtility.HtmlDecode(genre);
            }

            String artist = StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                (body, "Исполнитель:", "<br>", pos_input, StringComparison.OrdinalIgnoreCase, out pos_input).Trim();
            artist = CoreInternal.ExtractFromHTML(artist);
            
            String album = StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                (body, "Альбом:", "<br>", pos_input, StringComparison.OrdinalIgnoreCase, out pos_input).Trim();
            album = CoreInternal.ExtractFromHTML(album);

            String duration = StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                (body, "Длительность:", "<br>", pos_input, StringComparison.OrdinalIgnoreCase, out pos_input).Trim();
            duration = CoreInternal.ExtractFromHTML(duration);

            String size = StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                (body, "Размер:", "<br>", pos_input, StringComparison.OrdinalIgnoreCase, out pos_input).Trim();
            size = CoreInternal.ExtractFromHTML(size);

            String format = StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                (body, "<i class=\"format\">", "</i>", pos_input, StringComparison.OrdinalIgnoreCase, out pos_input).Trim();

            String bitrate = StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                (body, "<i class=\"bitrate\">", "</i>", pos_input, StringComparison.OrdinalIgnoreCase, out pos_input).Trim();

            HtmlNode uploader_node = body_node.ChildNodes.FindFirst("div");
            String uploader = uploader_node.InnerText.TrimStart("Загрузил:", StringComparison.OrdinalIgnoreCase, false).Trim();

            HtmlNode name_node = HTMLPage.DocumentNode.SelectSingleNode("//div[@id]/table[@width='100%']/tr[starts-with(@id,'trSong_')]/td[2]/div");
            if(name_node==null) {throw new InvalidOperationException("Невозможно извлечь из страницы песни блок HTML-кода с названием песни");}
            Boolean is_available = !name_node.InnerText.Contains("Файл утерян", StringComparison.InvariantCultureIgnoreCase);
            String name = HttpUtility.HtmlDecode(name_node.ChildNodes.Single(
                nd => nd.Name == "span" && nd.Attributes.Count > 0 && nd.Attributes.Contains("itemprop") && nd.Attributes["itemprop"].Value == "name"
            ).InnerText.CleanString().Trim());
            
            HtmlNode link_node = body_node.SelectSingleNode
                ("//div[@class='centerblock gr']/div[starts-with(@class, 'in2')]/div/table[@style and @class]/tr/td[2][@class='infoSong']/meta[@itemprop='url' and @content]");
            if(link_node==null) {throw new InvalidOperationException("Невозможно извлечь из страницы песни блок HTML-кода с URL на данную страницу");}

            String raw_link_URI = link_node.GetAttributeValue("content", "");
            String err_mes;
            Uri link_URI = CoreInternal.TryFixReturnURI(raw_link_URI, out err_mes);
            if (link_URI == null)
            {
                throw new InvalidOperationException("Невозможно извлечь URI текущей страницы песни: " + err_mes);
            }

            OneSongHeader song = new OneSongHeader(
                1, caption, name, artist, album, genre, duration, size, bitrate, format, uploader,
                CoreInternal.TryGrabImageUri(HTMLPage), link_URI, is_available);
            return song;
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
                    if (content_disposition.HasAlphaNumericChars() == false
                        || content_type.HasAlphaNumericChars() == false
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
                    ////application/mp3
                    //if (content_type.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase) == false)
                    //{
                    //    ErrorMessage = "Значение HTTP-хидера Content-Type некорректно и является : " + content_type;
                    //    return null;
                    //}
                    if (content_disposition.Contains("attachment", StringComparison.OrdinalIgnoreCase) == false &&
                        content_disposition.Contains("filename", StringComparison.OrdinalIgnoreCase) == false)
                    {
                        ErrorMessage =
                            "Значение HTTP-хидера Content-Disposition некорректно и является : " + content_disposition;
                        return null;
                    }
                    String original_filename_without_ext = StringTools.SubstringHelpers.GetSubstringToToken(content_disposition,
                        "filename=", false, StringTools.Direction.FromEndToStart, StringComparison.OrdinalIgnoreCase);
                    Int32 out_pos;
                    String ext = StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                        (response.ResponseUri.ToString(), "ex=", "&", 0, StringComparison.OrdinalIgnoreCase, out out_pos);
                    String original_filename_full = ext.HasAlphaNumericChars() == false
                        || original_filename_without_ext.EndsWith(ext, StringComparison.OrdinalIgnoreCase) == true
                        ? original_filename_without_ext
                        : original_filename_without_ext + ext;
                    MemoryStream file_body = new MemoryStream(content_length);
                    using (Stream receiveStream = response.GetResponseStream())
                    {
                        StreamTools.CopyStream(receiveStream, file_body, false, false);
                    }
                    DownloadedFile output = new DownloadedFile(original_filename_full, content_length, file_body);
                    return output;
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
                    String original_filename_without_ext = StringTools.SubstringHelpers.GetSubstringToToken(content_disposition,
                        "filename=", false, StringTools.Direction.FromEndToStart, StringComparison.OrdinalIgnoreCase);
                    Int32 out_pos;
                    String ext = StringTools.SubstringHelpers.GetInnerStringBetweenTokens
                        (response.ResponseUri.ToString(), "ex=", "&", 0, StringComparison.OrdinalIgnoreCase, out out_pos);
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
